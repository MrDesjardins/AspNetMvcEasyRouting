using FrontEndSharingLayer.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using ApplicationTier.FrontEndSharingLayer.Routes.Infrastructures;
using WebSite.Infrastructures.Routes;

namespace WebSite.Helpers
{
    public static class RouteExtensions
    {
        /// <summary>
        /// The extend method takes values from the source and add them into the destination. You do not need to use the return value
        /// because the destination object will already have the element of the source.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static RouteValueDictionary Extend(this RouteValueDictionary destination, IEnumerable<KeyValuePair<string, object>> source)
        {
            if (source == null)
            {
                return destination;
            }
            foreach (var srcElement in source.ToList())
            {
                if (destination.ContainsKey(srcElement.Key))
                {
                    destination[srcElement.Key] += " " + srcElement.Value;
                }
                else
                {
                    destination[srcElement.Key] = srcElement.Value;    
                }
                
            }
            return destination;
        }


      
    }


    public static class RouteCollectionExtension
    {

        public static void AddRoutes(this RouteCollection routes, List<AreaSectionLocalized> areaRoutes)
        {
            foreach (var area in areaRoutes)
            {
                routes.AddRoutes(area.ControllerTranslations, area);
            }
        }

        /// <summary>
        /// Add route in the RouteCollection by adding all localized routes of the Area and Controller Section pre-defined
        /// </summary>
        /// <param name="routes">RouteCollection to add controller and area routes</param>
        /// <param name="controllerRoutes">Controller routes</param>
        /// <param name="areaSectionLocalized">Area routes</param>
        public static void AddRoutes(this RouteCollection routes, List<ControllerSectionLocalized> controllerRoutes, AreaSectionLocalized areaSectionLocalized = null)
        {
            foreach (var controller in controllerRoutes)
            {
                foreach (var controllerTranslation in controller.Translation)
                {
                    foreach (var action in controller.ActionTranslations)
                    {
                        var urlAction = action.Url;

                        foreach (var actionTranslation in action.Translation)
                        {
                            if (controllerTranslation.CultureInfo == actionTranslation.CultureInfo)
                            {
                                //Start with action by handling the case of anonymous object for action or RouteValue. We wanta RouteValueDictionary at the end.
                                RouteValueDictionary values = null;
                                if (action.Values is RouteValueDictionary)
                                {
                                    values = action.Values as RouteValueDictionary;
                                }
                                else
                                {
                                    values = new RouteValueDictionary(action.Values);
                                }

                                //If the area is defined, we look up to see if we have one with the current Culture. If so, we add it to the route definition; else Null.
                                LocalizedSection areaTranslation = null;
                                if (areaSectionLocalized != null && areaSectionLocalized.Translation.Any(d => d.CultureInfo.Name == controllerTranslation.CultureInfo.Name))
                                {
                                    values[Constants.AREA] = areaSectionLocalized.AreaName;
                                    areaTranslation = areaSectionLocalized.Translation.FirstOrDefault(d => d.CultureInfo.Name == controllerTranslation.CultureInfo.Name);
                                }
                                values[Constants.CONTROLLER] = controller.ControllerName;
                                values[Constants.ACTION] = action.ActionName;

                                //Depending if the constraint was an anonymous object or a RouteValueDictionnary, we do or not a conversion to RouteValueDictionnary.
                                RouteValueDictionary constraints = null;
                                if (action.Constraints is RouteValueDictionary)
                                {
                                    constraints = action.Constraints as RouteValueDictionary;
                                }
                                else
                                {
                                    constraints = new RouteValueDictionary(action.Constraints);
                                }
                                //Replace remaining section with section localized value if defined, otherwise, the section remains there

                                var newUrl = LocalizedSection.ReplaceSection(urlAction, areaTranslation, controllerTranslation, actionTranslation);

                                //Add everything to the route. AreaSection can be null.
                                routes.Add(new LocalizedRoute(
                                      areaSectionLocalized
                                    , controller
                                    , action
                                    , newUrl
                                    , values
                                    , constraints
                                    , actionTranslation.CultureInfo
                                    )
                               );
                            }
                        }
                    }
                }
            }
        }
    }


}