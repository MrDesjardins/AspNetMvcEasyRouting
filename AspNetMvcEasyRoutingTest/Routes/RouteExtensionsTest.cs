using System.Linq;
using System.Web.Routing;
using AspNetMvcEasyRouting.Routes;
using AspNetMvcEasyRouting.Routes.Infrastructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetMvcEasyRoutingTest.Routes
{
    [TestClass]
    public class RouteExtensionsTest
    {
        [TestMethod]
        public void GivenTwoRouteValueDictionary_WhenBothDoesNotContainSameKey_ThenValueAreNotMerged()
        {
            // Arrange
            var valueObject1 = new RouteValueDictionary { { "key1", "value1" } };
            var valueObject2 = new RouteValueDictionary { { "key2", "value2" } };

            // Act
            valueObject1.Extend(valueObject2);

            // Assert
            Assert.AreEqual("value1", valueObject1["key1"].ToString());
        }

        [TestMethod]
        public void GivenTwoRouteValueDictionary_WhenBothContainSameKey_ThenValueAreMerged()
        {
            // Arrange
            var valueObject1 = new RouteValueDictionary {{"key1", "value1"}};
            var valueObject2 = new RouteValueDictionary {{"key1", "value2"}};

            // Act
            valueObject1.Extend(valueObject2);

            // Assert
            Assert.AreEqual("value1 value2", valueObject1["key1"].ToString());
        }

        [TestMethod]
        public void GivenAnAreaControllerAction_WhenSingleRoute_ThenTwoRoutesAdded()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
                .ForBilingualArea("area1", "area_en", "area_fr")
                .WithBilingualController("controller1", "controller_en", "controller_fr")
                .WithBilingualAction("action1", "action_en", "action_fr")
                .UseDefaulUrl()
                .ToListArea()
            ;

            var routeCollection = new RouteCollection();

            // Act
            routeCollection.AddRoutes(routesInStructure);

            // Assert
            Assert.AreEqual(2, routeCollection.Count);
        }

        [TestMethod]
        public void GivenAnAreaControllerAction_WhenTwoRoutes_ThenFourRoutesAdded()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
                .ForBilingualArea("area1", "area_en", "area_fr")
                .WithBilingualController("controller1", "controller_en", "controller_fr")
                    .WithBilingualAction("action1", "action_en", "action_fr")
                    .UseDefaulUrl()
                .And().WithBilingualAction("action2", "action2_en", "action2_fr")
                    .UseDefaulUrl()
                .ToListArea()
            ;

            var routeCollection = new RouteCollection();

            // Act
            routeCollection.AddRoutes(routesInStructure);

            // Assert
            Assert.AreEqual(4, routeCollection.Count);
        }

        [TestMethod]
        public void GivenAnAreaControllerAction_WhenTwoControllerAndThreeRoutes_ThenSixRoutesAdded()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
                .ForBilingualArea("area1", "area_en", "area_fr")
                .WithBilingualController("controller1", "controller_en", "controller_fr")
                    .WithBilingualAction("action1", "action_en", "action_fr")
                    .UseDefaulUrl()
                .And().WithBilingualAction("action2", "action2_en", "action2_fr")
                    .UseDefaulUrl()
                .ForBilingualController("controller2", "controller2_en", "controller2_fr")
                    .WithBilingualAction("action3", "action3_en", "action3_fr")
                        .UseDefaulUrl()
                .ToListArea()
            ;

            var routeCollection = new RouteCollection();

            // Act
            routeCollection.AddRoutes(routesInStructure);

            // Assert
            Assert.AreEqual(6, routeCollection.Count);
        }

        [TestMethod]
        public void GivenAnControllerAction_WhenTwoControllerAndThreeRoutes_ThenSixRoutesAdded()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
                .ForBilingualController("controller1", "controller_en", "controller_fr")
                    .WithBilingualAction("action1", "action_en", "action_fr")
                    .UseDefaulUrl()
                .And().WithBilingualAction("action2", "action2_en", "action2_fr")
                    .UseDefaulUrl()
                .ForBilingualController("controller2", "controller2_en", "controller2_fr")
                    .WithBilingualAction("action3", "action3_en", "action3_fr")
                        .UseDefaulUrl()
                .ToList()
            ;

            var routeCollection = new RouteCollection();

            // Act
            routeCollection.AddRoutes(routesInStructure);

            // Assert
            Assert.AreEqual(6, routeCollection.Count);
        }

    }
}
