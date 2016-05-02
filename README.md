# Asp.Net Mvc Easy Routing
[![Build Status](https://travis-ci.org/MrDesjardins/AspNetMvcEasyRouting.svg?branch=master)](https://travis-ci.org/MrDesjardins/AspNetMvcEasyRouting)
Asp.Net Mvc Easy Routing is a solution to create localized route easily with a fluent Api.

This allow to have Area and Controller based route in Asp.Net MVC in a Fluent way for multiple language (as this moment English and French hardcoded).

## Examples
Here is examples with Area:

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
			
Here is examples with controllers:

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

## Documentations

You can find more detail here:
 - [Localize Url without culture in Url](http://patrickdesjardins.com/blog/how-to-localized-mvc-routing-with-area-without-specifying-local-in-the-url-with-a-fluent-api)
 - [Route Fluent Api](http://patrickdesjardins.com/blog/improve-the-custom-localized-mvc-routing-with-fluent-api)
 - [Localized url with Asp.Net MVC and traversal pattern](http://patrickdesjardins.com/blog/localized-url-with-asp-net-mvc)
 
## Nuget Package
 You can find a Nuget package of this project at [Nuget.org](https://www.nuget.org/packages/AspNetMvcEasyRouting/)