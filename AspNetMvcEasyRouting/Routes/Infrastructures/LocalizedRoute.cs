using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AspNetMvcEasyRouting.Routes.Infrastructures
{
    public class LocalizedRoute : Route
    {
        public CultureInfo Culture { get; protected set; }
        public ActionSectionLocalized ActionTranslation { get; }
        public ControllerSectionLocalized ControllerTranslation { get; }
        public AreaSectionLocalized AreaSectionLocalized { get; }

        public LocalizedRoute(AreaSectionLocalized areaSectionLocalized, ControllerSectionLocalized controllerTranslation, ActionSectionLocalized actionTranslation, string url
            , RouteValueDictionary defaults, RouteValueDictionary constraints, CultureInfo culture)
            : this(areaSectionLocalized, controllerTranslation, actionTranslation, url, defaults, constraints, null, new MvcRouteHandler(), culture)
        {
        }

        public LocalizedRoute(AreaSectionLocalized areaSectionLocalized, ControllerSectionLocalized controllerTranslation, ActionSectionLocalized actionTranslation, string url
            , RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler, CultureInfo culture)
            : base(url, defaults, constraints, dataTokens, routeHandler)
        {
            this.AreaSectionLocalized = areaSectionLocalized;


            if (controllerTranslation == null)
            {
                throw new ArgumentNullException("controllerTranslation");
            }
            this.ControllerTranslation = controllerTranslation;

            if (actionTranslation == null)
            {
                throw new ArgumentNullException("actionTranslation");
            }
            this.ActionTranslation = actionTranslation;

            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }
            this.Culture = culture;

            if (dataTokens == null)
            {
                this.DataTokens = new RouteValueDictionary();
            }

            if (this.Defaults != null && this.Defaults.Keys.Contains(Constants.AREA))
            {
                if (this.DataTokens == null)
                {
                    this.DataTokens = new RouteValueDictionary();
                }
                this.DataTokens.Add(Constants.AREA, this.Defaults[Constants.AREA].ToString());
            }

            this.AdjustForNamespaces();
        }

        public virtual RouteData RouteDataFound(HttpContextBase httpContext, RouteData routeData)
        {
            if (httpContext.Request.Url != null)
            {
                if (httpContext.Request.Url.IsAbsoluteUri && httpContext.Request.Url.AbsolutePath == "/")
                {
                    if (httpContext.Request.Url.Host == "http://yourdomain.com" || httpContext.Request.Url.Host == "localhost")
                    {
                        this.Culture = LocalizedSection.FR;
                    }
                    else
                    {
                        this.Culture = LocalizedSection.EN;
                    }
                }
            }
            //this.Culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";//http://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
            Thread.CurrentThread.CurrentCulture = this.Culture;
            Thread.CurrentThread.CurrentUICulture = this.Culture;
            return routeData;
        }

        /// <summary>
        ///     Set the thread culture with the route culture
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var returnRouteData = base.GetRouteData(httpContext);
            if (returnRouteData != null)
            {
                returnRouteData = this.RouteDataFound(httpContext, returnRouteData);
            }
            return returnRouteData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var currentThreadCulture = Thread.CurrentThread.CurrentUICulture;
            //First step is to avoid route in the wrong culture
            if (this.Culture.Name != currentThreadCulture.Name)
            {
                return null;
            }

            //Second, set the right Area/Controller/Action to have MVC generating the URL with the localized string
            var replaceRoutingValues = true;
            LocalizedSection areaTranslated = null;
            LocalizedSection controllerTranslated = null;
            LocalizedSection actionTranslated = null;
            if (this.AreaSectionLocalized != null && values[Constants.AREA] != null) //If added in the RouteValue, it will be just there later during GetVirtualPath (merge from MVC's route creation code)
            {
                var valueToken = values[Constants.AREA];
                areaTranslated = this.AreaSectionLocalized.Translation.FirstOrDefault(d => d.CultureInfo.Name == currentThreadCulture.Name);
                replaceRoutingValues = areaTranslated != null && areaTranslated.TranslatedValue == valueToken;
            }

            if (replaceRoutingValues && this.ControllerTranslation != null)
            {
                var valueToken = values[Constants.CONTROLLER];
                controllerTranslated = this.ControllerTranslation.Translation.FirstOrDefault(d => d.CultureInfo.Name == currentThreadCulture.Name);
                replaceRoutingValues &= controllerTranslated != null && controllerTranslated.TranslatedValue == valueToken;
            }

            if (replaceRoutingValues && this.ActionTranslation != null)
            {
                var valueToken = values[Constants.ACTION];
                actionTranslated = this.ActionTranslation.Translation.FirstOrDefault(d => d.CultureInfo.Name == currentThreadCulture.Name);
                replaceRoutingValues &= actionTranslated != null && actionTranslated.TranslatedValue == valueToken;
            }

            //We need to find a translation that fit at least Controller and Action
            //if (!replaceRoutingValues)
            //{
            //    return null;
            //}

            //Switch text token to the right language
            if (this.ActionTranslation != null)
            {
                this.Url = this.ReplaceTokens(this.Url, this.ActionTranslation.Tokens);
            }

            // Check with the new values if the system can get an URL with the values in the culture desired
            var vitualPathData = this.GetVirtualPathForLocalizedRoute(requestContext, values);
            //vitualPathData.DataTokens
            // Asp.Net MVC found a URL, time to enhance the URL with localization replacement
            if (vitualPathData != null)
            {
                //This is to replace {action}, {controller} and {area} with the localized version
                vitualPathData.VirtualPath = LocalizedSection.ReplaceSection(this.Url, areaTranslated, controllerTranslated, actionTranslated);
                //Enhance url with replace or append route value dictionary 
                vitualPathData.VirtualPath = this.AdjustVirtualPathWithRoutes(vitualPathData.VirtualPath, values);
                //Default value if not defined in the route value
                vitualPathData.VirtualPath = this.AdjustVirtualPathWithActionTranslationDefaultValues(vitualPathData.VirtualPath, values);
                //Default value of the route
                vitualPathData.VirtualPath = this.AdjustVirtualPathWithDefaultValues(vitualPathData.VirtualPath);
                vitualPathData.VirtualPath = vitualPathData.VirtualPath.TrimEnd('/');
            }
            return vitualPathData;
        }

        /// <summary>
        ///     If after all replacement (this should be the last), we still have some route variables between {} we try to replace
        ///     with default value of
        ///     the route.
        /// </summary>
        /// <param name="currentVirtualPath"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string AdjustVirtualPathWithDefaultValues(string currentVirtualPath)
        {
            var values = this.Defaults;
            if (string.IsNullOrEmpty(currentVirtualPath) || values == null)
            {
                return currentVirtualPath;
            }
            var finalVirtualPath = currentVirtualPath;

            foreach (var key in values.Keys)
            {
                var toReplace = "{" + key + "}";
                finalVirtualPath = finalVirtualPath.Replace(toReplace, WebUtility.UrlEncode(values[key].ToString()));
            }
            return finalVirtualPath;
        }

        /// <summary>
        ///     Adjust virtual path with action translation default value not in the route. This is because we can define default
        ///     and the
        ///     value of default is only used when not more specific from the route.
        ///     Route has precedence on Default Value (this.ActionTranslation.Values)
        /// </summary>
        /// <param name="currentVirtualPath"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string AdjustVirtualPathWithActionTranslationDefaultValues(string currentVirtualPath, RouteValueDictionary values)
        {
            if (string.IsNullOrEmpty(currentVirtualPath) || values == null)
            {
                return currentVirtualPath;
            }
            var finalVirtualPath = currentVirtualPath;
            //This is for the case that optional parameter in the action are not defined in the URL
            if (this.ActionTranslation != null)
            {
                var rc = new RouteValueDictionary(this.ActionTranslation.Values);
                // If defined {word} is not in the URL, then we use the value from the actionTranslated
                foreach (var key in rc.Keys.Where(q => !values.ContainsKey(q)))
                {
                    var toReplace = "{" + key + "}";
                    finalVirtualPath = finalVirtualPath.Replace(toReplace, WebUtility.UrlEncode(rc[key].ToString()));
                }
            }
            return finalVirtualPath;
        }

        /// <summary>
        ///     Get all routes information that are not Area-Controller-Action and change the value from the URL.
        ///     If not in the URL, add the data in query string
        /// </summary>
        /// <param name="currentVirtualPath"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string AdjustVirtualPathWithRoutes(string currentVirtualPath, RouteValueDictionary values)
        {
            var finalVirtualPath = currentVirtualPath;
            if (values != null)
            {
                foreach (var key in values.Keys.Where(k => k != Constants.AREA && k != Constants.CONTROLLER && k != Constants.ACTION))
                {
                    var toReplace = "{" + key + "}";
                    if (values[key] != null)
                    {
                        var replaceWith = WebUtility.UrlEncode(values[key].ToString());
                        if (currentVirtualPath.Contains(toReplace))
                        {
                            finalVirtualPath = finalVirtualPath.Replace(toReplace, replaceWith);
                        }
                        else
                        {
                            finalVirtualPath = this.AddKeyValueToUrlAsQueryString(finalVirtualPath, toReplace, replaceWith);
                        }
                    }
                }
            }

            return finalVirtualPath;
        }

        public string ReplaceTokens(string url, Dictionary<string, LocalizedSectionList> tokens)
        {
            if (tokens != null)
            {
                foreach (var key in tokens.Keys)
                {
                    var tokenInCurrentCulture = tokens[key].FirstOrDefault(f => f.CultureInfo.Name == this.Culture.Name);
                    if (tokenInCurrentCulture != null)
                    {
                        var toReplace = "{" + key + "}";
                        return url.Replace(toReplace, WebUtility.UrlEncode(tokenInCurrentCulture.TranslatedValue));
                    }
                }
            }
            return url;
        }

        public string AddKeyValueToUrlAsQueryString(string url, string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && url != null)
            {
                key = key.Replace("{", "").Replace("}", "");
                if (url.Contains("?"))
                {
                    return url + "&" + key + "=" + value;
                }
                return url + "?" + key + "=" + value;
            }
            return url;
        }

        public virtual VirtualPathData GetVirtualPathForLocalizedRoute(RequestContext requestContext, RouteValueDictionary values)
        {
            return base.GetVirtualPath(requestContext, values);
        }

        protected override bool ProcessConstraint(HttpContextBase httpContext, object constraint, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return base.ProcessConstraint(httpContext, constraint, parameterName, values, routeDirection);
        }

        private void AdjustForNamespaces()
        {
            var namespaces = this.ControllerTranslation.NameSpaces;
            var useNamespaceFallback = namespaces == null || namespaces.Length == 0;
            this.DataTokens["UseNamespaceFallback"] = useNamespaceFallback;
            if ((namespaces != null) && (namespaces.Length > 0))
            {
                this.DataTokens["Namespaces"] = namespaces;
            }
        }
    }
}