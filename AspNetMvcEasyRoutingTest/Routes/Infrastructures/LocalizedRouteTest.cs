using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Routing;
using AspNetMvcEasyRouting.Routes;
using AspNetMvcEasyRouting.Routes.Infrastructures;
using AspNetMvcEasyRoutingTest;
using AspNetMvcEasyRoutingTest.Routes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebSiteUnitTest.Infrastructures.Route
{
    [TestClass]
    public class LocalizedRouteTest : RouteTestBase
    {
        [TestMethod]
        public void GivenANewLocalizedRoute_WhenEveryParameterNull_ThenDefaultValuesUsed()
        {
            // Arrange


            // Act
            var exception = TestExtensions.ThrownAndReturn<ArgumentNullException>(() => { var local = new LocalizedRoute(null, null, null, null, null, null, null); });

            // Assert
            Assert.AreEqual("controllerTranslation", exception.ParamName);
        }

        [TestMethod]
        public void GivenANewLocalizedRoute_WhenActionParameterNull_ThenThrowException()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized();

            // Act
            var exception = TestExtensions.ThrownAndReturn<ArgumentNullException>(() => { var local = new LocalizedRoute(null, controller, null, null, null, null, null); });

            // Assert
            Assert.AreEqual("actionTranslation", exception.ParamName);
        }

        [TestMethod]
        public void GivenANewLocalizedRoute_WhenUrlParameterNull_ThenThrowException()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();

            // Act
            var exception = TestExtensions.ThrownAndReturn<ArgumentNullException>(() => { var local = new LocalizedRoute(null, controller, action, null, null, null, null); });

            // Assert
            Assert.AreEqual("url", exception.ParamName);
        }

        [TestMethod]
        public void GivenANewLocalizedRoute_WhenCultureParameterNull_ThenThrowException()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();

            // Act
            var exception = TestExtensions.ThrownAndReturn<ArgumentNullException>(() => { var local = new LocalizedRoute(null, controller, action, "url", null, null, null); });

            // Assert
            Assert.AreEqual("culture", exception.ParamName);
        }

        [TestMethod]
        public void GivenANewLocalizedRoute_WhenDataTokenParameterNull_ThenDataTokenInitialize()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();

            // Act
            var local = new LocalizedRoute(null, controller, action, "url", null, null, null, null, new CultureInfo("en-us"));


            // Assert
            Assert.IsNotNull(local.DataTokens);
        }

        [TestMethod]
        public void GivenANewLocalizedRoute_WhenRouteHasArea_ThenDataTokenHasArea()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();
            var areaName = "testarea";
            var defaultValues = new RouteValueDictionary();
            defaultValues.Add(Constants.AREA, areaName);

            // Act
            var local = new LocalizedRoute(null, controller, action, "url", defaultValues, null, null, null, new CultureInfo("en-us"));


            // Assert
            Assert.IsNotNull(local.DataTokens);
            Assert.AreEqual(areaName, local.DataTokens[Constants.AREA]);
        }

        [TestMethod]
        public void GivenALocalizedRoute_WhenReplaceTokensWithNull_ThenNoReplacement()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/one/{two}/three";
            Dictionary<string, LocalizedSectionList> tokens = null;

            // Act
            var urlAfterReplace = local.ReplaceTokens(url, tokens);

            // Assert
            Assert.AreEqual(url, urlAfterReplace);
        }

        [TestMethod]
        public void GivenALocalizedRoute_WhenReplaceTokensWithEmpty_ThenNoReplacement()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/one/{two}/three";
            var tokens = new Dictionary<string, LocalizedSectionList>();

            // Act
            var urlAfterReplace = local.ReplaceTokens(url, tokens);

            // Assert
            Assert.AreEqual(url, urlAfterReplace);
        }

        [TestMethod]
        public void GivenALocalizedRoute_WhenReplaceTokensWithTokenButNotrightCulture_ThenNoReplacement()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/one/{two}/three";
            var tokens = new Dictionary<string, LocalizedSectionList>();
            tokens.Add("two", new LocalizedSectionList {new LocalizedSection(new CultureInfo("ja-jp"), "deux")});

            // Act
            var urlAfterReplace = local.ReplaceTokens(url, tokens);

            // Assert
            Assert.AreEqual(url, urlAfterReplace);
        }

        [TestMethod]
        public void GivenALocalizedRoute_WhenReplaceTokensWithTokenWithRightCulture_ThenReplacement()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/one/{two}/three";
            var tokens = new Dictionary<string, LocalizedSectionList>();
            tokens.Add("two", new LocalizedSectionList {new LocalizedSection(new CultureInfo("en-us"), "deux")});

            // Act
            var urlAfterReplace = local.ReplaceTokens(url, tokens);

            // Assert
            Assert.AreEqual("/one/deux/three", urlAfterReplace);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAddKeyValueToUrlAsQueryString_WhenUrlNull_ThenNothingHappen()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            string url = null;

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, "key1", "value");

            // Assert
            Assert.AreEqual(url, urlAfterReplace);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAddKeyValueToUrlAsQueryString_WhenKeyNull_ThenNothingHappen()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/url/here/";

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, null, "value");

            // Assert
            Assert.AreEqual(url, urlAfterReplace);
        }


        [TestMethod]
        public void GivenALocalizedRoute_ForAddKeyValueToUrlAsQueryString_WhenValueNull_ThenQueryStringAdded()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/url/here/";

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, "key1", null);

            // Assert
            Assert.AreEqual(url + "?key1=", urlAfterReplace);
        }


        [TestMethod]
        public void GivenALocalizedRoute_ForAddKeyValueToUrlAsQueryString_WhenNoQueryStringInUrl_ThenQueryStringAddedWithQuestionMark()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/url/here/";

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, "key1", "value1");

            // Assert
            Assert.AreEqual(url + "?key1=value1", urlAfterReplace);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAddKeyValueToUrlAsQueryString_WhenQueryStringInUrl_ThenQueryStringAddedWithAmpersand()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/url/here?key0=value0";

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, "key1", "value1");

            // Assert
            Assert.AreEqual(url + "&key1=value1", urlAfterReplace);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPath_WhenQueryStringInUrl_ThenQueryStringAddedWithAmpersand()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/url/here?key0=value0";

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, "key1", "value1");

            // Assert
            Assert.AreEqual(url + "&key1=value1", urlAfterReplace);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithRoutes_WhenCurrentVirtualPathIsNull_ThenNothingHappen()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            string virtualDataPath = null;

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithRoutes(virtualDataPath, new RouteValueDictionary());

            // Assert
            Assert.AreEqual(virtualDataPath, virtualDataPathAfter);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithRoutes_WhenRouteNull_ThenNothingHappen()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere";

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithRoutes(virtualDataPath, null);

            // Assert
            Assert.AreEqual(virtualDataPath, virtualDataPathAfter);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithRoutes_WhenRouteItemIsFoundInUrl_ThenRouteChanged()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere/{token1}/andthere";
            var routeValues = new RouteValueDictionary();
            routeValues.Add("token1", "xxxxx");

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithRoutes(virtualDataPath, routeValues);

            // Assert
            Assert.AreEqual("urlhere/xxxxx/andthere", virtualDataPathAfter);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithRoutes_WhenRouteItemIsNotInUrl_ThenQueryStringAdded()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere/nothing/andthere";
            var routeValues = new RouteValueDictionary();
            routeValues.Add("token1", "xxxxx");

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithRoutes(virtualDataPath, routeValues);

            // Assert
            Assert.AreEqual("urlhere/nothing/andthere?token1=xxxxx", virtualDataPathAfter);
        }


        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithRoutes_WhenRouteItemIsNotInUrlAndOneInSide_ThenQueryStringAddedAndRouteChanged()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere/{token1}/andthere";
            var routeValues = new RouteValueDictionary();
            routeValues.Add("token1", "xxxxx");
            routeValues.Add("token2", "yyyyy");

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithRoutes(virtualDataPath, routeValues);

            // Assert
            Assert.AreEqual("urlhere/xxxxx/andthere?token2=yyyyy", virtualDataPathAfter);
        }


        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithActionTranslationDefaultValues_WhenUrlIsNull_ThenNothingDone()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            string virtualDataPath = null;

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithActionTranslationDefaultValues(virtualDataPath, new RouteValueDictionary());

            // Assert
            Assert.AreEqual(virtualDataPath, virtualDataPathAfter);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithActionTranslationDefaultValues_WhenRouteIsNull_ThenNothingDone()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere";

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithActionTranslationDefaultValues(virtualDataPath, null);

            // Assert
            Assert.AreEqual(virtualDataPath, virtualDataPathAfter);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithActionTranslationDefaultValues_WhenDefaultValueInUrl_ThenReplaced()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere/{token1}";
            var routeValues = new RouteValueDictionary();
            local.ActionTranslation.Values = new {token1 = "xox"};

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithActionTranslationDefaultValues(virtualDataPath, routeValues);

            // Assert
            Assert.AreEqual("urlhere/xox", virtualDataPathAfter);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithActionTranslationDefaultValues_WhenDefaultValueNotInUrl_ThenNotReplaced()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere/{token}";
            var routeValues = new RouteValueDictionary();
            local.ActionTranslation.Values = new {token1 = "xox"};

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithActionTranslationDefaultValues(virtualDataPath, routeValues);

            // Assert
            Assert.AreEqual("urlhere/{token}", virtualDataPathAfter);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithActionTranslationDefaultValues_WhenDefaultValueInUrlAndInRoute_ThenNotReplaced()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere/{token1}";
            var routeValues = new RouteValueDictionary();
            routeValues.Add("token1", "ok");
            local.ActionTranslation.Values = new {token1 = "xox"};

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithActionTranslationDefaultValues(virtualDataPath, routeValues);

            // Assert
            Assert.AreEqual("urlhere/{token1}", virtualDataPathAfter);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithDefaultValues_WhenDefaultValueNotInUrl_ThenUseDefaultFromRouteAndReplace()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere/{token1}";
            var routeValues = new RouteValueDictionary();
            routeValues.Add("token1", "ok");
            local.Defaults = routeValues;

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithDefaultValues(virtualDataPath);

            // Assert
            Assert.AreEqual("urlhere/ok", virtualDataPathAfter);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithDefaultValues_WhenDefaultValueNotFound_ThenNotReplacement()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere/{token1}";
            var routeValues = new RouteValueDictionary();
            routeValues.Add("token2", "ok");
            local.Defaults = routeValues;

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithDefaultValues(virtualDataPath);

            // Assert
            Assert.AreEqual("urlhere/{token1}", virtualDataPathAfter);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForGetVirtualPath_WhenPathNotFound_ThenReturnNull()
        {
            // Arrange
            var stub = this.GetStubbedLocalizedRoute();
            var requestContext = this.GetRouteDataForUrl("url");
            var virtualDataPath = new VirtualPathData(stub, "");
            var routeValues = new RouteValueDictionary();
            stub.VirtualPathData = null;

            // Act
            var virtualDataPathAfter = stub.GetVirtualPath(requestContext.HttpContext.Request.RequestContext, routeValues);

            // Assert
            Assert.IsNull(virtualDataPathAfter);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForGetVirtualPath_WhenRouteNotAllRouteValueInSameCulture_ThenRouteNotChanged()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized("controller1", "controller1");
            var action = this.GetEmptyActionSectionLocalized("action1", "action1");
            action.Translation.Clear();
            var areaName = "testarea";
            var defaultValues = new RouteValueDictionary();
            defaultValues.Add(Constants.AREA, areaName);
            var stub = new StubbedLocalizedRoute(null, controller, action, "{area}/{controller}/{action}", defaultValues, null, null, null, new CultureInfo("en-us"));
            var requestContext = this.GetRouteDataForUrl("{area}/{controller}/{action}");
            var virtualDataPath = new VirtualPathData(stub, "");
            var routeValues = new RouteValueDictionary();
            routeValues.Add(Constants.AREA, areaName);
            routeValues.Add(Constants.CONTROLLER, controller.ControllerName);
            routeValues.Add(Constants.ACTION, action.ActionName);
            stub.VirtualPathData = null;

            // Act
            var virtualDataPathAfter = stub.GetVirtualPath(requestContext.HttpContext.Request.RequestContext, routeValues);

            // Assert
            Assert.AreEqual(areaName, stub.Values[Constants.AREA]);
            Assert.AreEqual(controller.ControllerName, stub.Values[Constants.CONTROLLER]);
            Assert.AreEqual(action.ActionName, stub.Values[Constants.ACTION]);
        }

        [TestMethod]
        public void GivenALocalizedRoute_ForGetVirtualPath_WhenRouteFoundTranslationInSameCulture_ThenRouteChange()
        {
            // Arrange
            var area = this.GetEmptyAreaSectionLocalized("area1", "area1");
            var controller = this.GetEmptyControllerSectionLocalized("controller1", "controller1");
            var action = this.GetEmptyActionSectionLocalized("action1", "action1");
            var areaName = "testarea";
            var defaultValues = new RouteValueDictionary();
            defaultValues.Add(Constants.AREA, areaName);
            var stub = new StubbedLocalizedRoute(area, controller, action, "{area}/{controller}/{action}", defaultValues, null, null, null, new CultureInfo("en-us"));
            var requestContext = this.GetRouteDataForUrl("{area}/{controller}/{action}");
            var virtualDataPath = new VirtualPathData(stub, "");
            var routeValues = new RouteValueDictionary();
            routeValues.Add(Constants.AREA, "area1");
            routeValues.Add(Constants.CONTROLLER, "controller1");
            routeValues.Add(Constants.ACTION, "action1");
            stub.VirtualPathData = virtualDataPath;

            // Act
            var virtualDataPathAfter = stub.GetVirtualPath(requestContext.HttpContext.Request.RequestContext, routeValues);

            // Assert
            Assert.AreEqual("area1", stub.Values[Constants.AREA]);
            Assert.AreEqual("controller1", stub.Values[Constants.CONTROLLER]);
            Assert.AreEqual("action1", stub.Values[Constants.ACTION]);
        }

        private AreaSectionLocalized GetEmptyAreaSectionLocalized(string english = null, string french = null)
        {
            return new AreaSectionLocalized("myareaname"
                , new LocalizedSectionList {new LocalizedSection(LocalizedSection.EN, english), new LocalizedSection(LocalizedSection.EN, french)}
                , null);
        }

        private ControllerSectionLocalized GetEmptyControllerSectionLocalized(string english = null, string french = null)
        {
            return new ControllerSectionLocalized("mycontrollername"
                , new LocalizedSectionList {new LocalizedSection(LocalizedSection.EN, english), new LocalizedSection(LocalizedSection.EN, french)}
                , null);
        }

        private ActionSectionLocalized GetEmptyActionSectionLocalized(string english = null, string french = null)
        {
            return new ActionSectionLocalized("myactioname"
                , new LocalizedSectionList {new LocalizedSection(LocalizedSection.EN, english), new LocalizedSection(LocalizedSection.EN, french)}
                , null);
        }

        private LocalizedRoute GetLocalizedRoute()
        {
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();
            var areaName = "testarea";
            var defaultValues = new RouteValueDictionary();
            defaultValues.Add(Constants.AREA, areaName);
            var local = new LocalizedRoute(null, controller, action, "url", defaultValues, null, null, null, new CultureInfo("en-us"));
            return local;
        }

        private StubbedLocalizedRoute GetStubbedLocalizedRoute()
        {
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();
            var areaName = "testarea";
            var defaultValues = new RouteValueDictionary();
            defaultValues.Add(Constants.AREA, areaName);
            var local = new StubbedLocalizedRoute(null, controller, action, "url", defaultValues, null, null, null, new CultureInfo("en-us"));
            return local;
        }

        //}

        //    // Assert
        //    //action.


        //    // Act
        //    var stub = new StubbedLocalizedRoute(area, controller, action, "{area}/{controller}/{action}", defaultValues, null, null, null, new CultureInfo("en-us"));
        //    defaultValues.Add(Constants.AREA, areaName);
        //    var defaultValues = new RouteValueDictionary();
        //    string areaName = "testarea";
        //    var action = this.GetEmptyActionSectionLocalized("actionen", "actionfr");
        //    var controller = this.GetEmptyControllerSectionLocalized("controlleren", "controllerfr");
        //    var area = this.GetEmptyAreaSectionLocalized("areaen", "areafr");
        //    // Arrange
        //{
        //public void GivenALocalizedRoute_ForGetLocalizedUrl_WhenRouteAreaControllerAction_ThenUrlLocalized()


        //[TestMethod]
    }

    public class StubbedLocalizedRoute : LocalizedRoute
    {
        public VirtualPathData VirtualPathData { get; set; }
        public RouteValueDictionary Values { get; set; }

        public StubbedLocalizedRoute(AreaSectionLocalized areaSectionLocalized, ControllerSectionLocalized controllerTranslation, ActionSectionLocalized actionTranslation, string url
            , RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler, CultureInfo culture)
            : base(areaSectionLocalized, controllerTranslation, actionTranslation, url, defaults, constraints, dataTokens, routeHandler, culture)
        {
        }

        public override VirtualPathData GetVirtualPathForLocalizedRoute(RequestContext requestContext, RouteValueDictionary values)
        {
            this.Values = values;
            return this.VirtualPathData;
        }
    }
}