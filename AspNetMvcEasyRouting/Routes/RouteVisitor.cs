using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Routing;
using AspNetMvcEasyRouting.Routes.Infrastructures;

namespace AspNetMvcEasyRouting.Routes
{
    /// <summary>
    ///     A route element is an Area, a Controller, an Action or a list of all these threes.
    /// </summary>
    public interface IRouteElement
    {
        /// <summary>
        ///     Entry point for the visitor into the element
        /// </summary>
        /// <param name="visitor"></param>
        void AcceptRouteVisitor(IRouteVisitor visitor);
    }

    /// <summary>
    ///     A visitor is the code that will traverse the configuration (tree) of routes.
    /// </summary>
    public interface IRouteVisitor
    {
        /// <summary>
        ///     Flag to indicate that a route has been found and that subsequent visits call be cancelled.
        ///     This is to improve performance.
        /// </summary>
        bool HasFoundRoute { get; }

        /// <summary>
        ///     Logic to be done by the visitor when this one visit an Area
        /// </summary>
        /// <param name="element"></param>
        /// <returns>True if has found a route that match the area criteria</returns>
        bool Visit(AreaSectionLocalized element);

        /// <summary>
        ///     Logic to be done by the visitor when this one visit a Controller
        /// </summary>
        /// <param name="element"></param>
        /// <returns>True if has found a route that match the controller criteria</returns>
        bool Visit(ControllerSectionLocalized element);

        /// <summary>
        ///     Logic to be done by the visitor when this one visit an Action
        /// </summary>
        /// <param name="element"></param>
        /// <returns>True if has found a route that match the actopm criteria</returns>
        bool Visit(ActionSectionLocalized element);
    }

    /// <summary>
    ///     Visitor to find from generic information a localized route from an three of routes
    /// </summary>
    public class RouteLocalizedVisitor : IRouteVisitor
    {
        private readonly string action;
        private readonly string area;
        private readonly string controller;
        private readonly RouteReturn result;
        private readonly string[] tokens;
        private readonly string[] urlInput;


        public CultureInfo Culture { get; }


        /// <summary>
        /// </summary>
        /// <param name="culture">Culture used for the route to Url convertion</param>
        /// <param name="area">Area requested. Can be null.</param>
        /// <param name="controller">Controller requested. This cannot be null.</param>
        /// <param name="action">Action requested. This cannot be null</param>
        /// <param name="urlInput">Specific input. Can be null.</param>
        /// <param name="tokens">Custom localized token. Can be null.</param>
        public RouteLocalizedVisitor(CultureInfo culture, string area, string controller, string action, string[] urlInput, string[] tokens)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            this.Culture = culture;
            this.area = area;
            this.controller = controller;
            this.action = action;
            this.urlInput = urlInput;
            this.tokens = tokens;
            this.result = new RouteReturn();
        }

        /// <summary>
        ///     Visitor action for area. If the area match, the result is updated with the localized area name
        /// </summary>
        /// <param name="element">Area visited</param>
        /// <returns>True if found; False if not found</returns>
        public bool Visit(AreaSectionLocalized element)
        {
            if (element.AreaName == this.area)
            {
                this.result.UrlParts[Constants.AREA] = element.Translation.First(d => d.Locale.CultureInfo.Name == this.Culture.Name).TranslatedValue;
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Visitor action for controller. If the controller match, the result is updated with the localized controller name
        /// </summary>
        /// <param name="element">Controller visited</param>
        /// <returns>True if found; False if not found</returns>
        public bool Visit(ControllerSectionLocalized element)
        {
            if (element.ControllerName == this.controller)
            {
                this.result.UrlParts[Constants.CONTROLLER] = element.Translation.First(d => d.Locale.CultureInfo.Name == this.Culture.Name).TranslatedValue;
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Visitor action for action. If the action match, the result is updated with the localized action name
        /// </summary>
        /// <param name="element">Action visited</param>
        /// <returns>True if found; False if not found</returns>
        public bool Visit(ActionSectionLocalized element)
        {
            var urlPartToAddIfGoodPart = new Dictionary<string, string>();
            if (element.ActionName == this.action)
            {
                if (!this.ExtractTokens(element, urlPartToAddIfGoodPart))
                {
                    return false;
                }

                if (!this.ExtractUrlPartValues(element, urlPartToAddIfGoodPart))
                {
                    return false;
                }

                this.result.UrlParts[Constants.ACTION] = element.Translation.First(d => d.Locale.CultureInfo.Name == this.Culture.Name).TranslatedValue;
            }
            else
            {
                return false;
            }


            this.RemoveOptionalWithDefaultEmpty(element, urlPartToAddIfGoodPart);
            urlPartToAddIfGoodPart.ToList().ForEach(x => this.result.UrlParts.Add(x.Key, x.Value)); //Merge the result
            this.result.UrlTemplate = element.Url;
            this.result.HasFoundRoute = true;
            return true;
        }

        /// <summary>
        ///     Indicate if a route has been found. This mean that every condition was met
        /// </summary>
        public bool HasFoundRoute
        {
            get { return this.result.HasFoundRoute; }
        }


        public RouteReturn Result()
        {
            return this.result;
        }

        /// <summary>
        ///     Remove optional value by adding this one in the UrlPart with Empty string which make the GetFinalUrl to replace the
        ///     {xxx} with nothing
        /// </summary>
        /// <param name="element"></param>
        /// <param name="urlPartToAddIfGoodPart"></param>
        private void RemoveOptionalWithDefaultEmpty(ActionSectionLocalized element, Dictionary<string, string> urlPartToAddIfGoodPart)
        {
            if (element.Values != null)
            {
                var dict = (RouteValueDictionary) element.Values;
                foreach (var keyValues in dict)
                {
                    var remove = this.urlInput == null || (this.urlInput != null && this.urlInput.All(f => f != keyValues.Key));
                    if (remove)
                    {
                        urlPartToAddIfGoodPart[keyValues.Key] = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        ///     If the user request a url than we let it through (to let the user replace with his value). If not defined in
        ///     UrlPart, then use default value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="urlPartToAddIfGoodPart"></param>
        /// <returns></returns>
        private bool ExtractUrlPartValues(ActionSectionLocalized element, Dictionary<string, string> urlPartToAddIfGoodPart)
        {
            //Default Values : check if there, nothing to replace
            if (this.urlInput != null)
            {
                foreach (var input in this.urlInput)
                {
                    if (element.Url.IndexOf(input, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        var routeValues = (RouteValueDictionary) element.Values;
                        var isDefinedValue = (routeValues != null) && routeValues.Keys.Contains(input);
                        if (isDefinedValue)
                        {
                            var defaultValue = routeValues[input].ToString();
                            if (defaultValue == string.Empty)
                            {
                                urlPartToAddIfGoodPart[input] = "{" + input + "}";
                            }
                            else
                            {
                                urlPartToAddIfGoodPart[input] = defaultValue;
                            }
                        }
                        else
                        {
                            //Default if not empty
                            urlPartToAddIfGoodPart[input] = "{" + input + "}";
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        ///     Get localized value for every tokens
        /// </summary>
        /// <param name="element"></param>
        /// <param name="urlPartToAddIfGoodPart"></param>
        /// <returns></returns>
        private bool ExtractTokens(ActionSectionLocalized element, Dictionary<string, string> urlPartToAddIfGoodPart)
        {
            if (this.tokens != null)
            {
                if (element.Tokens == null)
                {
                    return false;
                }
                for (var i = 0; i < this.tokens.Length; i++)
                {
                    if (element.Tokens.ContainsKey(this.tokens[i]))
                    {
                        var tokenFound = element.Tokens[this.tokens[i]];
                        var tokenTranslation = tokenFound.First(d => d.Locale.CultureInfo.Name == this.Culture.Name);
                        urlPartToAddIfGoodPart[this.tokens[i]] = tokenTranslation.TranslatedValue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}