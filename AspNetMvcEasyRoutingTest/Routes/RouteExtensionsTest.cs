using System;
using System.Web.Routing;
using AspNetMvcEasyRouting.Routes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebSiteUnitTest.Helpers
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
    }
}
