using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;
using AspNetMvcEasyRouting.Routes;
using AspNetMvcEasyRouting.Routes.Infrastructures;
using Xunit;
using Assert = Xunit.Assert;

namespace AspNetMvcEasyRoutingTest.Routes.Infrastructures
{
    
    public class LocalizedRouteTest : RouteTestBase
    {
        private Locale enlishLocal;
        public LocalizedRouteTest()
        {
            this.enlishLocal = new Locale(new CultureInfo("en-US"));
        }
        [Fact]
        public void GivenANewLocalizedRoute_WhenEveryParameterNull_ThenDefaultValuesUsed()
        {
            // Arrange


            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => { var local = new LocalizedRoute(null, null, null, null, null, null, null); });

            // Assert
            Assert.Equal("controllerTranslation", exception.ParamName);
        }

        [Fact]
        public void GivenANewLocalizedRoute_WhenActionParameterNull_ThenThrowException()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized();

            // Act
            var exception = Assert.Throws<ArgumentNullException >(() => { var local = new LocalizedRoute(null, controller, null, null, null, null, null); });

            // Assert
            Assert.Equal("actionTranslation", exception.ParamName);
        }

        [Fact]
        public void GivenANewLocalizedRoute_WhenUrlParameterNull_ThenThrowException()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => { var local = new LocalizedRoute(null, controller, action, null, null, null, null); });

            // Assert
            Assert.Equal("url", exception.ParamName);
        }

        [Fact]
        public void GivenANewLocalizedRoute_WhenCultureParameterNull_ThenThrowException()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => { var local = new LocalizedRoute(null, controller, action, "url", null, null, null); });

            // Assert
            Assert.Equal("locale", exception.ParamName);
        }

        [Fact]
        public void GivenANewLocalizedRoute_WhenDataTokenParameterNull_ThenDataTokenInitialize()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();

            // Act
            var local = new LocalizedRoute(null, controller, action, "url", null, null, null, null, this.enlishLocal);


            // Assert
            Assert.NotNull(local.DataTokens);
        }

        [Fact]
        public void GivenANewLocalizedRoute_WhenRouteHasArea_ThenDataTokenHasArea()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();
            var areaName = "testarea";
            var defaultValues = new RouteValueDictionary();
            defaultValues.Add(Constants.AREA, areaName);

            // Act
            var local = new LocalizedRoute(null, controller, action, "url", defaultValues, null, null, null, this.enlishLocal);


            // Assert
            Assert.NotNull(local.DataTokens);
            Assert.Equal(areaName, local.DataTokens[Constants.AREA]);
        }

        [Fact]
        public void GivenALocalizedRoute_WhenReplaceTokensWithNull_ThenNoReplacement()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/one/{two}/three";
            Dictionary<string, LocalizedSectionList> tokens = null;

            // Act
            var urlAfterReplace = local.ReplaceTokens(url, tokens);

            // Assert
            Assert.Equal(url, urlAfterReplace);
        }

        [Fact]
        public void GivenALocalizedRoute_WhenReplaceTokensWithEmpty_ThenNoReplacement()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/one/{two}/three";
            var tokens = new Dictionary<string, LocalizedSectionList>();

            // Act
            var urlAfterReplace = local.ReplaceTokens(url, tokens);

            // Assert
            Assert.Equal(url, urlAfterReplace);
        }

        [Fact]
        public void GivenALocalizedRoute_WhenReplaceTokensWithTokenButNotrightCulture_ThenNoReplacement()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/one/{two}/three";
            var tokens = new Dictionary<string, LocalizedSectionList>();
            tokens.Add("two", new LocalizedSectionList {new LocalizedSection(new Locale(new CultureInfo("ja-jp")), "deux")});

            // Act
            var urlAfterReplace = local.ReplaceTokens(url, tokens);

            // Assert
            Assert.Equal(url, urlAfterReplace);
        }

        [Fact]
        public void GivenALocalizedRoute_WhenReplaceTokensWithTokenWithRightCulture_ThenReplacement()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/one/{two}/three";
            var tokens = new Dictionary<string, LocalizedSectionList>();
            tokens.Add("two", new LocalizedSectionList {new LocalizedSection(new Locale(new CultureInfo("en-us")), "deux")});

            // Act
            var urlAfterReplace = local.ReplaceTokens(url, tokens);

            // Assert
            Assert.Equal("/one/deux/three", urlAfterReplace);
        }

        [Fact]
        public void GivenALocalizedRoute_ForAddKeyValueToUrlAsQueryString_WhenUrlNull_ThenNothingHappen()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            string url = null;

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, "key1", "value");

            // Assert
            Assert.Equal(url, urlAfterReplace);
        }

        [Fact]
        public void GivenALocalizedRoute_ForAddKeyValueToUrlAsQueryString_WhenKeyNull_ThenNothingHappen()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/url/here/";

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, null, "value");

            // Assert
            Assert.Equal(url, urlAfterReplace);
        }


        [Fact]
        public void GivenALocalizedRoute_ForAddKeyValueToUrlAsQueryString_WhenValueNull_ThenQueryStringAdded()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/url/here/";

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, "key1", null);

            // Assert
            Assert.Equal(url + "?key1=", urlAfterReplace);
        }


        [Fact]
        public void GivenALocalizedRoute_ForAddKeyValueToUrlAsQueryString_WhenNoQueryStringInUrl_ThenQueryStringAddedWithQuestionMark()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/url/here/";

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, "key1", "value1");

            // Assert
            Assert.Equal(url + "?key1=value1", urlAfterReplace);
        }

        [Fact]
        public void GivenALocalizedRoute_ForAddKeyValueToUrlAsQueryString_WhenQueryStringInUrl_ThenQueryStringAddedWithAmpersand()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/url/here?key0=value0";

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, "key1", "value1");

            // Assert
            Assert.Equal(url + "&key1=value1", urlAfterReplace);
        }

        [Fact]
        public void GivenALocalizedRoute_ForAdjustVirtualPath_WhenQueryStringInUrl_ThenQueryStringAddedWithAmpersand()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var url = "/url/here?key0=value0";

            // Act
            var urlAfterReplace = local.AddKeyValueToUrlAsQueryString(url, "key1", "value1");

            // Assert
            Assert.Equal(url + "&key1=value1", urlAfterReplace);
        }

        [Fact]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithRoutes_WhenCurrentVirtualPathIsNull_ThenNothingHappen()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            string virtualDataPath = null;

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithRoutes(virtualDataPath, new RouteValueDictionary());

            // Assert
            Assert.Equal(virtualDataPath, virtualDataPathAfter);
        }

        [Fact]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithRoutes_WhenRouteNull_ThenNothingHappen()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere";

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithRoutes(virtualDataPath, null);

            // Assert
            Assert.Equal(virtualDataPath, virtualDataPathAfter);
        }

        [Fact]
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
            Assert.Equal("urlhere/xxxxx/andthere", virtualDataPathAfter);
        }

        [Fact]
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
            Assert.Equal("urlhere/nothing/andthere?token1=xxxxx", virtualDataPathAfter);
        }


        [Fact]
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
            Assert.Equal("urlhere/xxxxx/andthere?token2=yyyyy", virtualDataPathAfter);
        }


        [Fact]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithActionTranslationDefaultValues_WhenUrlIsNull_ThenNothingDone()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            string virtualDataPath = null;

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithActionTranslationDefaultValues(virtualDataPath, new RouteValueDictionary());

            // Assert
            Assert.Equal(virtualDataPath, virtualDataPathAfter);
        }

        [Fact]
        public void GivenALocalizedRoute_ForAdjustVirtualPathWithActionTranslationDefaultValues_WhenRouteIsNull_ThenNothingDone()
        {
            // Arrange
            var local = this.GetLocalizedRoute();
            var virtualDataPath = "urlhere";

            // Act
            var virtualDataPathAfter = local.AdjustVirtualPathWithActionTranslationDefaultValues(virtualDataPath, null);

            // Assert
            Assert.Equal(virtualDataPath, virtualDataPathAfter);
        }

        [Fact]
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
            Assert.Equal("urlhere/xox", virtualDataPathAfter);
        }

        [Fact]
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
            Assert.Equal("urlhere/{token}", virtualDataPathAfter);
        }

        [Fact]
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
            Assert.Equal("urlhere/{token1}", virtualDataPathAfter);
        }

        [Fact]
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
            Assert.Equal("urlhere/ok", virtualDataPathAfter);
        }

        [Fact]
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
            Assert.Equal("urlhere/{token1}", virtualDataPathAfter);
        }

        [Fact]
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
            Assert.Null(virtualDataPathAfter);
        }

        [Fact]
        public void GivenALocalizedRoute_ForGetVirtualPath_WhenRouteNotAllRouteValueInSameCulture_ThenRouteNotChanged()
        {
            // Arrange
            var controller = this.GetEmptyControllerSectionLocalized("controller1", "controller1");
            var action = this.GetEmptyActionSectionLocalized("action1", "action1");
            action.Translation.Clear();
            var areaName = "testarea";
            var defaultValues = new RouteValueDictionary();
            defaultValues.Add(Constants.AREA, areaName);
            var stub = new StubbedLocalizedRoute(null, controller, action, "{area}/{controller}/{action}", defaultValues, null, null, null, this.enlishLocal);
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
            Assert.Equal(areaName, stub.Values[Constants.AREA]);
            Assert.Equal(controller.ControllerName, stub.Values[Constants.CONTROLLER]);
            Assert.Equal(action.ActionName, stub.Values[Constants.ACTION]);
        }

        [Fact]
        public void GivenALocalizedRoute_ForGetVirtualPath_WhenRouteFoundTranslationInSameCulture_ThenRouteChange()
        {
            // Arrange
            var area = this.GetEmptyAreaSectionLocalized("area1", "area1");
            var controller = this.GetEmptyControllerSectionLocalized("controller1", "controller1");
            var action = this.GetEmptyActionSectionLocalized("action1", "action1");
            var areaName = "testarea";
            var defaultValues = new RouteValueDictionary();
            defaultValues.Add(Constants.AREA, areaName);
            var stub = new StubbedLocalizedRoute(area, controller, action, "{area}/{controller}/{action}", defaultValues, null, null, null, this.enlishLocal);
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
            Assert.Equal("area1", stub.Values[Constants.AREA]);
            Assert.Equal("controller1", stub.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", stub.Values[Constants.ACTION]);
        }


        [Fact]
        public void GivenALocalizedRoute_ForGetVirtualPath_WhenEnglishWithTokens_ThenReturnEnglishUrl()
        {
            // Arrange
            var routeValueDictionary = new RouteValueDictionary{
                                    {"typeemail", "1"},
                                    {"emailkey", "2"}
            };

            var dataTokens = new RouteValueDictionary{
                                    {"typeemailtoken", "type"},
                                    {"emailkeytoken", "by-email-token"}
            };
            var controller = this.GetEmptyControllerSectionLocalized("controller1", "controller1");
            var action = this.GetEmptyActionSectionLocalized("action_en", "action_fr");
            var stub = new StubbedLocalizedRoute(null, controller, action, "{action}/{typeemailtoken}/{typeemail}/{emailkeytoken}/{emailkey}", routeValueDictionary, null, dataTokens, null, this.enlishLocal);
            var requestContext = this.GetRouteDataForUrl("{area}/{controller}/{action}");
            var virtualDataPath = new VirtualPathData(stub, "");
            stub.VirtualPathData = virtualDataPath;

            //Act
            var virtualDataPathAfter = stub.GetVirtualPath(requestContext.HttpContext.Request.RequestContext, routeValueDictionary);

            //Assert
            Assert.Equal("{action}/{typeemailtoken}/1/{emailkeytoken}/2", virtualDataPathAfter.VirtualPath);
        }


        private AreaSectionLocalized GetEmptyAreaSectionLocalized(string english = null, string french = null)
        {
            return new AreaSectionLocalized("myareaname"
                , new LocalizedSectionList {new LocalizedSection(new Locale(LocalizedSection.EN), english), new LocalizedSection(new Locale(LocalizedSection.EN), french)}
                , null);
        }

        private ControllerSectionLocalized GetEmptyControllerSectionLocalized(string english = null, string french = null)
        {
            return new ControllerSectionLocalized("mycontrollername"
                , new LocalizedSectionList {new LocalizedSection(new Locale(LocalizedSection.EN), english), new LocalizedSection(new Locale(LocalizedSection.EN), french)}
                , null);
        }

        private ActionSectionLocalized GetEmptyActionSectionLocalized(string english = null, string french = null)
        {
            return new ActionSectionLocalized("myactioname"
                , new LocalizedSectionList {new LocalizedSection(new Locale(LocalizedSection.EN), english), new LocalizedSection(new Locale(LocalizedSection.EN), french)}
                , null);
        }

        private LocalizedRoute GetLocalizedRoute()
        {
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();
            var areaName = "testarea";
            var defaultValues = new RouteValueDictionary();
            defaultValues.Add(Constants.AREA, areaName);
            var local = new LocalizedRoute(null, controller, action, "url", defaultValues, null, null, null, this.enlishLocal);
            return local;
        }

        private StubbedLocalizedRoute GetStubbedLocalizedRoute()
        {
            var controller = this.GetEmptyControllerSectionLocalized();
            var action = this.GetEmptyActionSectionLocalized();
            var areaName = "testarea";
            var defaultValues = new RouteValueDictionary();
            defaultValues.Add(Constants.AREA, areaName);
            var local = new StubbedLocalizedRoute(null, controller, action, "url", defaultValues, null, null, null, this.enlishLocal);
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


        //[Fact]
    }

    public class StubbedLocalizedRoute : LocalizedRoute
    {
        public VirtualPathData VirtualPathData { get; set; }
        public RouteValueDictionary Values { get; set; }

        public StubbedLocalizedRoute(AreaSectionLocalized areaSectionLocalized, ControllerSectionLocalized controllerTranslation, ActionSectionLocalized actionTranslation, string url
            , RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler, Locale culture)
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