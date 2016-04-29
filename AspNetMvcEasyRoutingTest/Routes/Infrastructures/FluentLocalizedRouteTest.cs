using System;
using System.Linq;
using System.Web.Routing;
using AspNetMvcEasyRouting.Routes;
using AspNetMvcEasyRouting.Routes.Infrastructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetMvcEasyRoutingTest.Routes.Infrastructures
{
    [TestClass]
    public class FluentLocalizedRouteTest:RouteTestBase
    {

        [TestMethod]
        public void GivenUrl_WhenDoesnotExist_ThenNull()
        {
            // Arrange
            // -

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/page/123").RouteData;

            // Assert
            Assert.IsNull(routeData);
        }

        [TestMethod]
        public void GivenAnAreaControllerAction_WhenNormalRoute_ThenRouteFound()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}")
              .ToListArea()
            ;
            RouteTable.Routes.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en").RouteData;

            // Assert
            Assert.IsNotNull(routeData);
            Assert.AreEqual("area1", routeData.Values[Constants.AREA]);
            Assert.AreEqual("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.AreEqual("action1", routeData.Values[Constants.ACTION]);
            Assert.AreEqual(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [TestMethod]
        public void GivenAnAreaTwoControllerAction_WhenNormalRoute_ThenRouteFound()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .ForBilingualController("controller1", "controller_en", "controller_fr")
                .WithBilingualAction("action1", "action_en", "action_fr")
                .WithUrl("{controller}/{action}")
              .ForBilingualController("controller2", "controller2_en", "controller2_fr")
                .WithBilingualAction("action2", "action2_en", "action2_fr")
                .WithUrl("{controller}/{action}")
              .ToList()
            ;
            RouteTable.Routes.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("controller_en/action_en").RouteData;

            // Assert
            Assert.IsNotNull(routeData);
            Assert.IsNull(routeData.Values[Constants.AREA]);
            Assert.AreEqual("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.AreEqual("action1", routeData.Values[Constants.ACTION]);
            Assert.AreEqual(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [TestMethod]
        public void GivenAnAreaControllerAction_WhenDefaultValueSet_ThenRouteHasDefaultValue()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithDefaultValues("pagevalue", "123")
              .WithUrl("{area}/{controller}/{action}/page/{pagevalue}")
              .ToListArea()
            ;
            RouteTable.Routes.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/page/123").RouteData;

            // Assert
            Assert.IsNotNull(routeData);
            Assert.AreEqual("area1", routeData.Values[Constants.AREA]);
            Assert.AreEqual("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.AreEqual("action1", routeData.Values[Constants.ACTION]);
            Assert.AreEqual("123", routeData.Values["pagevalue"]);
            Assert.AreEqual(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [TestMethod]
        public void GivenAnAreaControllerAction_WhenDefaultValueAsObjectSet_ThenRouteHasDefaultValue()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithDefaultValues(new { pagevalue ="123"})
              .WithUrl("{area}/{controller}/{action}/page/{pagevalue}")
              .ToListArea()
            ;
            RouteTable.Routes.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/page/123").RouteData;

            // Assert
            Assert.IsNotNull(routeData);
            Assert.AreEqual("area1", routeData.Values[Constants.AREA]);
            Assert.AreEqual("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.AreEqual("action1", routeData.Values[Constants.ACTION]);
            Assert.AreEqual("123", routeData.Values["pagevalue"]);
            Assert.AreEqual(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [TestMethod]
        public void GivenAnAreaControllerAction_WhenOneMirrorUrl_ThenRouteRetrieved()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}/page")
              .WithMirrorUrl("boom")
              .ToListArea()
            ;
            RouteTable.Routes.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("boom").RouteData;

            // Assert
            Assert.IsNotNull(routeData);
            Assert.AreEqual("area1", routeData.Values[Constants.AREA]);
            Assert.AreEqual("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.AreEqual("action1", routeData.Values[Constants.ACTION]);
            Assert.AreEqual(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [TestMethod]
        public void GivenAnAreaControllerAction_WhenTwoMirrorUrl_ThenRouteCanBeRetrievedIn3DifferentWays()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}/page")
              .WithMirrorUrl("boom")
              .WithMirrorUrl("boom2")
              .ToListArea()
            ;
            RouteTable.Routes.AddRoutes(routesInStructure);

            // Act
            RouteData routeData1 = base.GetRouteDataForUrl("boom").RouteData;
            RouteData routeData2 = base.GetRouteDataForUrl("boom2").RouteData;
            RouteData routeData3 = base.GetRouteDataForUrl("area_en/controller_en/action_en/page").RouteData;

            // Assert
            Assert.IsNotNull(routeData1);
            Assert.IsNotNull(routeData2);
            Assert.IsNotNull(routeData3);
            Assert.AreEqual(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [TestMethod]
        public void GivenAnAreaControllerAction_WhenToken_ThenRouteWithTokenTranslated()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithTranslatedTokens("token1", "token_en", "token_fr")
              .WithUrl("{area}/{controller}/{action}/page/{token1}")
              .WithMirrorUrl("boom")
              .ToListArea()
            ;
            RouteTable.Routes.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en/page/token_en").RouteData;

            // Assert
            Assert.IsNotNull(routeData);
            Assert.AreEqual("area1", routeData.Values[Constants.AREA]);
            Assert.AreEqual("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.AreEqual("action1", routeData.Values[Constants.ACTION]);
            Assert.AreEqual(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [TestMethod]
        public void GivenAnAreaControllerAction_WhenNamespace_ThenRouteWithNamespace()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .AssociateToNamespace("namespace1")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}")
              .WithMirrorUrl("boom")
              .ToListArea()
            ;
            RouteTable.Routes.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en").RouteData;

            // Assert
            Assert.IsNotNull(routeData);
            Assert.AreEqual("area1", routeData.Values[Constants.AREA]);
            Assert.AreEqual("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.AreEqual("action1", routeData.Values[Constants.ACTION]);
            Assert.AreEqual("namespace1", ((string[]) routeData.DataTokens["Namespaces"])[0]);
            Assert.IsFalse(Convert.ToBoolean(routeData.DataTokens["UseNamespaceFallback"]));
            Assert.AreEqual(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        [TestMethod]
        public void GivenAnAreaControllerAction_WhenTwoNamespace_ThenRouteWithNamespace()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
              .ForBilingualArea("area1", "area_en", "area_fr")
              .WithBilingualController("controller1", "controller_en", "controller_fr")
              .AssociateToNamespace("namespace1")
              .AssociateToNamespace("namespace2")
              .WithBilingualAction("action1", "action_en", "action_fr")
              .WithUrl("{area}/{controller}/{action}")
              .WithMirrorUrl("boom")
              .ToListArea()
            ;
            RouteTable.Routes.AddRoutes(routesInStructure);

            // Act
            RouteData routeData = base.GetRouteDataForUrl("area_en/controller_en/action_en").RouteData;

            // Assert
            Assert.IsNotNull(routeData);
            Assert.AreEqual("area1", routeData.Values[Constants.AREA]);
            Assert.AreEqual("controller1", routeData.Values[Constants.CONTROLLER]);
            Assert.AreEqual("action1", routeData.Values[Constants.ACTION]);
            var namespaces = (string[])routeData.DataTokens["Namespaces"];
            Assert.IsTrue(namespaces.Any(d => d == "namespace1"));
            Assert.IsTrue(namespaces.Any(d => d == "namespace2"));
            Assert.IsFalse(Convert.ToBoolean(routeData.DataTokens["UseNamespaceFallback"]));
            Assert.AreEqual(LocalizedSection.EN, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }
    }
}