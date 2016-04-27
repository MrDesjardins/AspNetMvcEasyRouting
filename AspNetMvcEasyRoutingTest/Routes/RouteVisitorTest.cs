using System;
using System.Web.Mvc;
using System.Web.Routing;
using AspNetMvcEasyRouting.Routes;
using AspNetMvcEasyRouting.Routes.Infrastructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetMvcEasyRoutingTest.Routes
{
    [TestClass]
    public class RouteVisitorTest
    {
        public static AreaSectionLocalizedList RoutesArea = FluentLocalizedRoute.BuildRoute()
            .ForBilingualArea("moderator", "Moderation-en", "Moderation")
            .WithBilingualController("Symbol", "Symbol-en", "Symbole")
            .WithBilingualAction("SymbolChangeList", "Symbol-Change-List", "Liste-symbole-renommer")
            .UseDefaulUrl()
            .And().WithBilingualAction("SymbolChangeList", "Symbol-Change-List", "Liste-symbole-renommer")
            .WithUrl("{area}/{controller}/{action}/{value1}")
            .And().WithBilingualAction("SymbolChangeList", "Symbol-Change-List", "Liste-symbole-renommer")
            .WithUrl("{area}/{controller}/{action}/{value1}/{token1}")
            .WithTranslatedTokens("token1", "tokenen", "tokenfr")
            .And().WithBilingualAction("GetSplitsForSymbol", "ListSplit", "Liste-split")
            .WithUrl("{area}/{controller}/{action}/{" + "symbol_in" + "}")
            .ForBilingualController("Audit", "Audit", "Audit")
            .WithBilingualAction("AuditByContest", "Contest", "Concours")
            .WithConstraints(new RouteValueDictionary {{"myid", @"\d+"}})
            .WithUrl("{area}/{action}/{" + "myid" + "}/{controller}")
            .ToListArea();

        public static ControllerSectionLocalizedList RoutesController = FluentLocalizedRoute.BuildRoute()
            .ForBilingualController("Home", "Home", "Demarrer")
            .WithBilingualAction("Index", "Index", "Index")
            .UseEmptyUrl()
            .WithMirrorUrl("{controller}/{action}")
            .And().WithBilingualAction("Testimonials", "Testimonials", "Temoignages")
            .WithUrl("{action}")
            .ForBilingualController("Account", "Account-en", "Compte")
            .WithBilingualAction("Profile", "Profile-en", "Afficher-Profile")
            .WithDefaultValues(new {username = UrlParameter.Optional})
            .WithUrl("{action}/{username}")
            .And().WithBilingualAction("ActivateAccount", "ActivateAccount", "Activer-compte")
            .WithUrl("{controller}/{action}/{emailAddress}/{now}")
            .WithTranslatedTokens("now", "Now", "Maintenant")
            .ForBilingualController("c", "c-en", "c-fr")
            .WithBilingualAction("a", "a-en", "a-fr")
            .WithUrl("{controller}/{action}")
            .And().WithBilingualAction("a2", "a2-en", "a2-fr")
            .WithDefaultValues(new {v1 = "boom"})
            .WithUrl("{controller}/{action}/{v1}")
            .ToList();

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void GivenARouteLocalizedVisitor_WhenControllerNotDefined_ThenException()
        {
            // Arrange
            var visitor = new RouteLocalizedVisitor(LocalizedSection.EN, "moderator", null, "SymbolChangeList", null, null);

            // Act & Assert
            RoutesArea.AcceptRouteVisitor(visitor);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void GivenARouteLocalizedVisitor_WhenActionNotDefined_ThenException()
        {
            // Arrange
            var visitor = new RouteLocalizedVisitor(LocalizedSection.EN, "moderator", "ControllerName", null, null, null);

            // Act & Assert
            RoutesArea.AcceptRouteVisitor(visitor);
        }

        [TestMethod]
        public void GivenARouteToVisit_WhenAreaControllerActionUnique_ThenReturnThisUniqueRoute()
        {
            // Arrange
            var visitor = new RouteLocalizedVisitor(LocalizedSection.EN, "moderator", "Symbol", "SymbolChangeList", null, null);

            // Act
            RoutesArea.AcceptRouteVisitor(visitor);

            // Assert
            var result = visitor.Result().FinalUrl();
            Assert.AreEqual("Moderation-en/Symbol-en/Symbol-Change-List", result);
        }

        [TestMethod]
        public void GivenARouteToVisit_WhenAreaControllerActionNotUniqueButValueUnique_ThenReturnRouteWithValue()
        {
            // Arrange
            var visitor = new RouteLocalizedVisitor(LocalizedSection.EN, "moderator", "Symbol", "SymbolChangeList", new[] {"value1"}, null);

            // Act
            RoutesArea.AcceptRouteVisitor(visitor);

            // Assert
            var result = visitor.Result().FinalUrl();
            Assert.AreEqual("Moderation-en/Symbol-en/Symbol-Change-List/{value1}", result);
        }

        [TestMethod]
        public void GivenARouteToVisit_WhenAreaControllerActionValueNotUniqueButTokenUnique_ThenReturnRouteWithToken()
        {
            // Arrange
            var visitor = new RouteLocalizedVisitor(LocalizedSection.EN, "moderator", "Symbol", "SymbolChangeList", new[] {"value1"}, new[] {"token1"});

            // Act
            RoutesArea.AcceptRouteVisitor(visitor);

            // Assert
            var result = visitor.Result().FinalUrl();
            Assert.AreEqual(result, "Moderation-en/Symbol-en/Symbol-Change-List/{value1}/tokenen");
        }


        [TestMethod]
        public void GivenARouteToVisit_WhenNoArea_ThenReturnRouteWithoutArea()
        {
            // Arrange
            var visitor = new RouteLocalizedVisitor(LocalizedSection.EN, null, "c", "a", null, null);

            // Act
            RoutesController.AcceptRouteVisitor(visitor);

            // Assert
            var result = visitor.Result().FinalUrl();
            Assert.AreEqual("c-en/a-en", result);
        }

        [TestMethod]
        public void GivenARouteToVisit_WhenNoAreaWithDefaultValue_ThenReturnRouteWithoutAreaWithDefaultValue()
        {
            // Arrange
            var visitor = new RouteLocalizedVisitor(LocalizedSection.EN, null, "Account", "Profile", null, null);

            // Act
            RoutesController.AcceptRouteVisitor(visitor);

            // Assert
            var result = visitor.Result().FinalUrl();
            Assert.AreEqual("Profile-en", result);
        }

        [TestMethod]
        public void GivenARouteToVisit_WhenNoAreaWithDefaultValueSet_ThenReturnRouteWithoutAreaWithDefaultValue()
        {
            // Arrange
            var visitor = new RouteLocalizedVisitor(LocalizedSection.EN, null, "Account", "Profile", new[] {"username"}, null);

            // Act
            RoutesController.AcceptRouteVisitor(visitor);

            // Assert
            var result = visitor.Result().FinalUrl();
            Assert.AreEqual("Profile-en/{username}", result);
        }

        [TestMethod]
        public void GivenARouteToVisit_WhenNoAreaWithDefaultValueSetNotEmpty_ThenReturnRouteWithoutAreaWithDefaultValue()
        {
            // Arrange
            var visitor = new RouteLocalizedVisitor(LocalizedSection.EN, null, "c", "a2", new[] {"v1"}, null);

            // Act
            RoutesController.AcceptRouteVisitor(visitor);

            // Assert
            var result = visitor.Result().FinalUrl();
            Assert.AreEqual("c-en/a2-en/boom", result);
        }


        [TestMethod]
        [ExpectedException(typeof (RouteNotFound))]
        public void GivenARouteToVisit_WhenNoFound_ThenThrowException()
        {
            // Arrange
            var visitor = new RouteLocalizedVisitor(LocalizedSection.EN, null, "NotFound", "DoesntExist", null, null);

            // Act 
            RoutesController.AcceptRouteVisitor(visitor);

            // Assert
            visitor.Result().FinalUrl();
        }
    }
}