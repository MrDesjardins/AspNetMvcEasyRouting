using System;

namespace AspNetMvcEasyRouting.Routes
{
    /// <summary>
    ///     Exception triggered when a route is requested but cannot be found
    /// </summary>
    public class RouteNotFound : Exception
    {
        public RouteNotFound()
        {
        }

        public RouteNotFound(string message) : base(message)
        {
        }
    }
}