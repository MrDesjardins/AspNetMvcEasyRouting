using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AspNetMvcEasyRoutingTest.Routes
{
    public class RouteTestBase : ContextBoundObject
    {
        [TestInitialize]
        public void BetweenTest()
        {
            RouteTable.Routes.Clear();
        }

        /// <summary>
        ///     From URL to Controller/Action, this method return the route and http context after passing the URL
        ///     into the RouteTable.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected RouteAndContext GetRouteDataForUrl(string url)
        {
            var context = this.FakeHttpContext(requestUrl: "~/" + url);
            RouteData routeData = RouteTable.Routes.GetRouteData(context);
            return new RouteAndContext(context, routeData);
        }

        /// <summary>
        ///     Generate an UrlHelper to be used to generate URL string.
        /// </summary>
        /// <param name="appPath">
        ///     Define the application path. By default, if all routing start from the domain than this should
        ///     not be set.
        /// </param>
        /// <returns>Html</returns>
        protected UrlHelper GetUrlHelper(string appPath = "~/")
        {
            HttpContextBase httpContext = this.FakeHttpContext("/", appPath);
            var routeData = /*RouteTable.Routes.GetRouteData(httpContext) ?? */ new RouteData();
            RequestContext requestContext = new RequestContext(httpContext, routeData);
            UrlHelper helper = new UrlHelper(requestContext, RouteTable.Routes);
            return helper;
        }

        private HttpContextBase FakeHttpContext(string appPath = "~/", string requestUrl = "/")
        {
            // Mocks
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var requestContext = new Mock<RequestContext>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            // Query String Parameters
            var routePart = requestUrl;
            var queryStringPart = requestUrl;

            if (routePart.Contains("?"))
            {
                var indexQueryString = routePart.IndexOf("?", StringComparison.InvariantCulture);
                routePart = requestUrl.Substring(0, indexQueryString);
                queryStringPart = requestUrl.Substring(indexQueryString + 1, requestUrl.Length - indexQueryString - 1);
                var parameters = new NameValueCollection();
                var parametersList = queryStringPart.Split('&');
                foreach (var paramter in parametersList)
                {
                    var keyAndvalue = paramter.Split('=');
                    parameters.Add(keyAndvalue[0], keyAndvalue[1]);
                }

                request.Setup(req => req.Params).Returns(parameters);
            }

            // Setup all Http Context
            request.Setup(req => req.ApplicationPath).Returns(appPath);
            request.Setup(req => req.AppRelativeCurrentExecutionFilePath).Returns(routePart);
            request.Setup(req => req.PathInfo).Returns(string.Empty);
            request.Setup(req => req.ServerVariables).Returns(new NameValueCollection());
            response.Setup(res => res.ApplyAppPathModifier(It.IsAny<string>())).Returns((string virtualPath) => virtualPath);
            user.Setup(usr => usr.Identity).Returns(identity.Object);
            identity.Setup(ident => ident.IsAuthenticated).Returns(true);
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.User).Returns(user.Object);
            requestContext.Setup(d => d.HttpContext).Returns(context.Object);
            request.Setup(d => d.RequestContext).Returns(requestContext.Object);
            return context.Object;
        }

        /// <summary>
        ///     Class used to return the RouteData and the HttpContextBase
        /// </summary>
        public class RouteAndContext
        {
            public HttpContextBase HttpContext { get; private set; }
            public RouteData RouteData { get; private set; }

            public RouteAndContext(HttpContextBase httpContextBase, RouteData routeData)
            {
                this.HttpContext = httpContextBase;
                this.RouteData = routeData;
            }
        }
    }
}