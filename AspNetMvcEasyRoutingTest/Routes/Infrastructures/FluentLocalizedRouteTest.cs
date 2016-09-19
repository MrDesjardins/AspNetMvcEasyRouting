using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using AspNetMvcEasyRouting.Routes;
using AspNetMvcEasyRouting.Routes.Infrastructures;
using Xunit;


namespace AspNetMvcEasyRoutingTest.Routes.Infrastructures
{
    
    public class FluentLocalizedRouteTest:RouteTestBase
    {

        public FluentLocalizedRouteTest()
        {
        }

        [Fact]
        public void GivenUrl_WhenDoesnotExist_ThenNull()
        {
            // Arrange
            // -

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/page/123").RouteData;

            // Assert
            Assert.Null(routeData);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenDefineLocalization_Then()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR, "http://website.fr")
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area1", routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenNormalRoute_ThenRouteFound()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area1", routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaTwoControllerAction_WhenNormalRoute_ThenRouteFound()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualController("controller1", "controller_en", "controller_fr")
                .WithBilingualAction("action1", "action_en", "action_fr")
                .WithUrl("{controller}/{action}")
              .ForBilingualController("controller2", "controller2_en", "controller2_fr")
                .WithBilingualAction("action2", "action2_en", "action2_fr")
                .WithUrl("{controller}/{action}")
              .ToList()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("controller_en/action_en").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Null(routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenDefaultValueSet_ThenRouteHasDefaultValue()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithDefaultValues("pagevalue", "123")
              .WithUrl("{area}/{controller}/{action}/page/{pagevalue}")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/page/123").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area1", routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal("123", routeData.Values["pagevalue"]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenDefaultValueAsObjectSet_ThenRouteHasDefaultValue()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithDefaultValues(new { pagevalue ="123"})
              .WithUrl("{area}/{controller}/{action}/page/{pagevalue}")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/page/123").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area1", routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal("123", routeData.Values["pagevalue"]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenOneMirrorUrl_ThenRouteRetrieved()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}/page")
              .WithMirrorUrl("boom")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("boom").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area1", routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenTwoMirrorUrl_ThenRouteCanBeRetrievedIn3DifferentWays()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}/page")
              .WithMirrorUrl("boom")
              .WithMirrorUrl("boom2")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData1 = base.GetRouteDataForUrl("boom").RouteData;
            RouteData routeData2 = base.GetRouteDataForUrl("boom2").RouteData;
            RouteData routeData3 = base.GetRouteDataForUrl("area_en/controller_en/action_en/page").RouteData;

            // Assert
            Assert.NotNull(routeData1);
            Assert.NotNull(routeData2);
            Assert.NotNull(routeData3);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenToken_ThenRouteWithTokenTranslated()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithTranslatedTokens("token1", "token_en", "token_fr")
              .WithUrl("{area}/{controller}/{action}/page/{token1}")
              .WithMirrorUrl("boom")
              .ToListArea()
            ;
            routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/page/token_en").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area1", routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenNamespace_ThenRouteWithNamespace()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .AssociateToNamespace("namespace1")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}")
              .WithMirrorUrl("boom")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area1", routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal("namespace1", ((string[]) routeData.DataTokens["Namespaces"])[0]);
            Assert.False(Convert.ToBoolean(routeData.DataTokens["UseNamespaceFallback"]));
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenTwoNamespace_ThenRouteWithNamespace()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .AssociateToNamespace("namespace1")
              .AssociateToNamespace("namespace2")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}")
              .WithMirrorUrl("boom")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area1", routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            var namespaces = (string[])routeData.DataTokens["Namespaces"];
            Assert.True(namespaces.Any(d => d == "namespace1"));
            Assert.True(namespaces.Any(d => d == "namespace2"));
            Assert.False(Convert.ToBoolean(routeData.DataTokens["UseNamespaceFallback"]));
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenConstaintValid_ThenRouteFound()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithConstraints("page", @"\d+")
              .WithUrl("{area}/{controller}/{action}/{page}")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/1").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area1", routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal("1", routeData.Values["page"]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenConstaintNotValid_ThenRouteFound()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithConstraints("page", @"\d+")
              .WithUrl("{area}/{controller}/{action}/{page}")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/asd").RouteData;

            // Assert
            Assert.Null(routeData);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenConstaintWithObject_ThenRouteFound()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithConstraints(new {page = @"\d+"})
              .WithUrl("{area}/{controller}/{action}/{page}")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/1").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area1", routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal("1", routeData.Values["page"]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenMoreThanOneArea_ThenRouteFound()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}/{page}")
              .ForBilingualArea("area2", "area_en2", "area_fr2")
              .WithBilingualController("controller2", "controller2_en", "controller2_fr")
              .WithBilingualAction("action2", "action2_en", "action2_fr")
              .WithUrl("{area}/{controller}/{action}/{page}")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en2/controller2_en/action2_en/1").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area2", routeData.Values[Constants.AREA]);
            Assert.Equal("controller2", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action2", routeData.Values[Constants.ACTION]);
            Assert.Equal("1", routeData.Values["page"]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenAnAreaControllerActionWithDomainRoute_WhenUseDomainRouteArea_ThenRouteFound()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .UseEmptyUrl()
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("http://mywebsite.com").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("area1", routeData.Values[Constants.AREA]);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }
        [Fact]
        public void GivenAnAreaControllerActionWithDomainRoute_WhenUseDomainRouteController_ThenRouteFound()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .UseEmptyUrl()
              .ToList()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("http://mywebsite.com").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("action1", routeData.Values[Constants.ACTION]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }


        [Fact]
        public void GivenAnAreaControllerAction_WhenTokenRepeated_ThenRouteIgnoreLastOne()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithTranslatedTokens("token1", "token_en", "token_fr")
              .WithTranslatedTokens("token1", "token_2en", "token_2fr")
              .WithUrl("{area}/{controller}/{action}/page/{token1}")
              .WithMirrorUrl("boom")
              .ToListArea()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/page/token_en").RouteData;
            RouteData routeData2 = base.GetRouteDataForUrl("area_en/controller_en/action_en/page/token_2en").RouteData;
            RouteData routeData3 = base.GetRouteDataForUrl("area_en/controller_en/action_en/page/token_3en").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.NotNull(routeData2);
            Assert.NotNull(routeData3);
        }
        [Fact]
        public void GivenANotExistingRoute_When404PageDefined_ThenRouteTo404()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualController("controller1", "controller_en", "controller_fr")
                  .WithBilingualAction("action1", "action_en", "action_fr")
                  .UseEmptyUrl()
              .ForPageNotFound("errorcontroller", "erroraction")
              .ToList()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/notfound").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("errorcontroller", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("erroraction", routeData.Values[Constants.ACTION]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [Fact]
        public void GivenANotExistingRoute_When404PageUndefined_ThenNull()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .UseEmptyUrl()
              .ToList()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/notfound").RouteData;

            // Assert
            Assert.Null(routeData);
        }

        [Fact]
        public void GivenANotExistingRoute_When404PageDefinedAtBuildRouteLevel_ThenRouteTo404()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForPageNotFound("errorcontroller", "erroraction")
              .ToList()
            ;
            base.routeCollection.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/notfound").RouteData;

            // Assert
            Assert.NotNull(routeData);
            Assert.Equal("errorcontroller", routeData.Values[Constants.CONTROLLER]);
            Assert.Equal("erroraction", routeData.Values[Constants.ACTION]);
            Assert.Equal(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }


        [Fact]
        public void GivenAConfigurationWithControllerActionTokenAndRoutePart_WhenUsingUrlHelper_ThenReturnEnglishUrl()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{action}/{typeemailtoken}/{typeemail}/{emailkeytoken}/{emailkey}")
              .WithTranslatedTokens("typeemailtoken", "type", "categorie")
              .WithTranslatedTokens("emailkeytoken", "by-email-token", "par-jeton-courriel")
              .ToList()
            ;
            base.routeCollection.AddRoutes(routesInStructure);
            UrlHelper helper = GetUrlHelper();
            var routeParameters = new RouteValueDictionary{
                                    {"typeemail", "1"},
                                    {"emailkey", "2"}
            };

            //Act
            string url = helper.Action("action1", "controller1", routeParameters);

            //Assert
            Assert.Equal("/action_en/type/1/by-email-token/2", url);
        }

        [Fact]
        public void GivenAConfigurationWithControllerActionAndRoutePart_WhenUsingUrlHelper_ThenReturnEnglishUrl()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{action}/type/{typeemail}/by-email-token/{emailkey}")
              .ToList()
            ;
            base.routeCollection.AddRoutes(routesInStructure);
            UrlHelper helper = GetUrlHelper();
            var routeParameters = new RouteValueDictionary{
                                    {"typeemail", "1"},
                                    {"emailkey", "2"}
            };

            //Act
            string url = helper.Action("action1", "controller1", routeParameters);

            //Assert
            Assert.Equal("/action_en/type/1/by-email-token/2", url);
        }

        [Fact]
        public void GivenAConfigurationWithControllerAction_WhenUsingUrlHelper_ThenReturnEnglishUrl()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .InLocalRouteBuilder(LocalizedSection.EN)
              .InLocalRouteBuilder(LocalizedSection.FR)
              .ForBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{controller}/{action}")
              .ToList()
            ;
            base.routeCollection.AddRoutes(routesInStructure);
            UrlHelper helper = GetUrlHelper();


            //Act
            string url = helper.Action("action1", "controller1");

            //Assert
            Assert.Equal("/controller_en/action_en", url);
        }
    }
}