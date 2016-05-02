using System.Linq;
using System.Web.Routing;
using AspNetMvcEasyRouting.Routes;
using AspNetMvcEasyRouting.Routes.Infrastructures;
using Xunit;
using Assert = Xunit.Assert;

namespace AspNetMvcEasyRoutingTest.Routes
{
    
    public class RouteExtensionsTest
    {
        [Fact]
        public void GivenTwoRouteValueDictionary_WhenBothDoesNotContainSameKey_ThenValueAreNotMerged()
        {
            // Arrange
            var valueObject1 = new RouteValueDictionary { { "key1", "value1" } };
            var valueObject2 = new RouteValueDictionary { { "key2", "value2" } };

            // Act
            valueObject1.Extend(valueObject2);

            // Assert
            Assert.Equal("value1", valueObject1["key1"].ToString());
        }

        [Fact]
        public void GivenTwoRouteValueDictionary_WhenBothContainSameKey_ThenValueAreMerged()
        {
            // Arrange
            var valueObject1 = new RouteValueDictionary {{"key1", "value1"}};
            var valueObject2 = new RouteValueDictionary {{"key1", "value2"}};

            // Act
            valueObject1.Extend(valueObject2);

            // Assert
            Assert.Equal("value1 value2", valueObject1["key1"].ToString());
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenSingleRoute_ThenTwoRoutesAdded()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
                .InLocalRouteBuilder(LocalizedSection.EN)
                .InLocalRouteBuilder(LocalizedSection.FR)
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
            Assert.Equal(2, routeCollection.Count);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenTwoRoutes_ThenFourRoutesAdded()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
                .InLocalRouteBuilder(LocalizedSection.EN)
                .InLocalRouteBuilder(LocalizedSection.FR)
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
            Assert.Equal(4, routeCollection.Count);
        }

        [Fact]
        public void GivenAnAreaControllerAction_WhenTwoControllerAndThreeRoutes_ThenSixRoutesAdded()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
                .InLocalRouteBuilder(LocalizedSection.EN)
                .InLocalRouteBuilder(LocalizedSection.FR)
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
            Assert.Equal(6, routeCollection.Count);
        }

        [Fact]
        public void GivenAnControllerAction_WhenTwoControllerAndThreeRoutes_ThenSixRoutesAdded()
        {
            // Arrange
            var routesInStructure = FluentLocalizedRoute.BuildRoute()
                .InLocalRouteBuilder(LocalizedSection.EN)
                .InLocalRouteBuilder(LocalizedSection.FR)
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
            Assert.Equal(6, routeCollection.Count);
        }

    }
}
