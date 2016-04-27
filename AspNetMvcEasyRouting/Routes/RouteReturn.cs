using System.Collections.Generic;
using System.Linq;

namespace AspNetMvcEasyRouting.Routes
{
    /// <summary>
    ///     Data returned from a visit into a Area-Controller-Action tree
    /// </summary>
    public class RouteReturn
    {
        /// <summary>
        ///     This contain string to replace. For example, "controller" which will be used to replace "{controller}" with the
        ///     value.
        /// </summary>
        public Dictionary<string, string> UrlParts { get; set; }

        /// <summary>
        ///     Indicate if we found a match with the UrlParts provided
        /// </summary>
        public bool HasFoundRoute { get; set; }

        /// <summary>
        ///     This is set when the Action is found with the url template. This string will have a format like this :
        ///     {area}/{controller}/{action}/{value1}/{token1}
        /// </summary>
        public string UrlTemplate { get; set; }

        public RouteReturn()
        {
            this.UrlParts = new Dictionary<string, string>();
            this.HasFoundRoute = false;
        }

        /// <summary>
        ///     Url to be consumed by the caller
        /// </summary>
        /// <returns></returns>
        public string FinalUrl()
        {
            if (this.HasFoundRoute)
            {
                var finalUrl = this.UrlTemplate;
                var allParts = this.UrlParts.ToList();
                foreach (var keyValuePair in allParts)
                {
                    finalUrl = finalUrl.Replace("{" + keyValuePair.Key + "}", keyValuePair.Value);
                }
                return finalUrl.TrimEnd('/');
            }
            throw new RouteNotFound("Route not found for pieces of Url requested");
        }
    }
}