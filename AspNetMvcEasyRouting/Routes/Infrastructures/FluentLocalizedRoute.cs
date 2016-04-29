using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace AspNetMvcEasyRouting.Routes.Infrastructures
{
    public static class FluentLocalizedRoute
    {
        public static RouteBuilder BuildRoute()
        {
            return new RouteBuilder();
        }
    }

    public class RouteBuilder : IRouteBuilder
    {
        public ControllerSectionLocalizedList ControllerList { get; }
        public AreaSectionLocalizedList AreaList { get; }

        public RouteBuilder()
        {
            this.ControllerList = new ControllerSectionLocalizedList();
            this.AreaList = new AreaSectionLocalizedList();
        }

        public IRouteBuilderControllerAndControllerConfiguration ForBilingualController(string controllerName, string controllerEnglishLocalizedString, string controllerFrenchLocalizedString)
        {
            var controllerSectionLocalized = new ControllerSectionLocalized(controllerName, new LocalizedSectionList
                {
                    new LocalizedSection(LocalizedSection.EN, controllerEnglishLocalizedString)
                     ,new LocalizedSection(LocalizedSection.FR, controllerFrenchLocalizedString)
                }
                , null);
            this.ControllerList.Add(controllerSectionLocalized);
            if (this.AreaList.Any())
            {
                this.AreaList.Last().ControllerTranslations.Add(controllerSectionLocalized);
            }
            var rbc = new RouteBuilderController(controllerSectionLocalized, this);
            return rbc;
        }

        public IRouteBuilderArea ForBilingualArea(string areaName, string areaEnglishLocalizedString, string areaFrenchLocalizedString)
        {
            var areaLocalized = new AreaSectionLocalized(areaName, new LocalizedSectionList
                {
                     new LocalizedSection(LocalizedSection.EN, areaEnglishLocalizedString)
                    ,new LocalizedSection(LocalizedSection.FR, areaFrenchLocalizedString)
                }
                , null);
            this.AreaList.Add(areaLocalized);
            var rbc = new RouteBuilderArea(areaLocalized, this);
            return rbc;
        }
    }

    public interface IRouteBuilder
    {
        IRouteBuilderControllerAndControllerConfiguration ForBilingualController(string controllerName, string controllerEnglishLocalizedString, string controllerFrenchLocalizedString);
        IRouteBuilderArea ForBilingualArea(string areaName, string areaEnglishLocalizedString, string areaFrenchLocalizedString);
    }

    public interface IRouteAreaBuilder
    {
        IRouteBuilderController ForBilingualArea(string areaName, string areaEnglishLocalizedString, string areaFrenchLocalizedString);
    }


    public interface IRouteBuilderController
    {
        IRouteBuilderAction WithBilingualAction(string actionName, string actionEnglishLocalizedString, string actionFrenchLocalizedString);
    }

    public class RouteBuilderController : IRouteBuilderControllerAndControllerConfiguration
    {
        private readonly ControllerSectionLocalized currentControllerSection;
        private readonly RouteBuilder routeBuilder;

        public RouteBuilderController(ControllerSectionLocalized controllerSection, RouteBuilder routeBuilder)
        {
            this.currentControllerSection = controllerSection;
            this.routeBuilder = routeBuilder;
        }

        public IRouteBuilderControllerAndControllerConfiguration AssociateToNamespace(string @namespace)
        {
            if (this.currentControllerSection.NameSpaces == null)
            {
                this.currentControllerSection.NameSpaces = new[] {@namespace};
            }
            else
            {
                var currentNamespaces = this.currentControllerSection.NameSpaces;
                var len = this.currentControllerSection.NameSpaces.Length;
                Array.Resize(ref currentNamespaces, len + 1);
                currentNamespaces[len] = @namespace;
                this.currentControllerSection.NameSpaces = currentNamespaces;
            }

            return this;
        }

        public IRouteBuilderAction WithBilingualAction(string actionName, string actionEnglishLocalizedString, string actionFrenchLocalizedString)
        {
            if (this.currentControllerSection.ActionTranslations == null)
            {
                this.currentControllerSection.ActionTranslations = new ActionSectionLocalizedList();
            }
            var currentAction = new ActionSectionLocalized(actionName, new LocalizedSectionList
            {
                new LocalizedSection(LocalizedSection.EN, actionEnglishLocalizedString)
                ,
                new LocalizedSection(LocalizedSection.FR, actionFrenchLocalizedString)
            });
            this.currentControllerSection.ActionTranslations.Add(currentAction);
            return new RouteBuilderAction(this.currentControllerSection, currentAction, this.routeBuilder, this);
        }
    }

    public interface IRouteBuilderArea
    {
        IRouteBuilderControllerAndControllerConfiguration WithBilingualController(string controllerName, string controllerEnglishLocalizedString, string controllerFrenchLocalizedString);
    }

    public class RouteBuilderArea : IRouteBuilderArea
    {
        private readonly AreaSectionLocalized currentControllerSection;
        private readonly RouteBuilder routeBuilder;

        public RouteBuilderArea(AreaSectionLocalized controllerSection, RouteBuilder routeBuilder)
        {
            this.currentControllerSection = controllerSection;
            this.routeBuilder = routeBuilder;
        }

        public IRouteBuilderControllerAndControllerConfiguration WithBilingualController(string controllerName, string controllerEnglishLocalizedString, string controllerFrenchLocalizedString)
        {
            if (this.currentControllerSection.ControllerTranslations == null)
            {
                this.currentControllerSection.ControllerTranslations = new ControllerSectionLocalizedList();
            }

            var controllerSectionLocalized = new ControllerSectionLocalized(controllerName, new LocalizedSectionList
            {
                 new LocalizedSection(LocalizedSection.EN, controllerEnglishLocalizedString)
                ,new LocalizedSection(LocalizedSection.FR, controllerFrenchLocalizedString)
            }, null);


            if (this.routeBuilder.AreaList.Any())
            {
                this.routeBuilder.AreaList.Last().ControllerTranslations.Add(controllerSectionLocalized);
            }
            else
            {
                this.currentControllerSection.ControllerTranslations.Add(controllerSectionLocalized);
            }
            return new RouteBuilderController(controllerSectionLocalized, this.routeBuilder);
        }
    }

    public class RouteBuilderAction : IRouteBuilderAction
        , IRouteBuilderAction_Defaults
        , IRouteBuilderAction_Constraints
        , IRouteBuilderAction_Url
        , IRouteBuilderAction_ToList
        , IRouteBuilderAction_ToListOnlyWithAnd
        , IRouteBuilderAction_ToListWithoutUrl
    {
        private ActionSectionLocalized currentAction;
        private readonly ControllerSectionLocalized currentControllerSection;
        private readonly ActionSectionLocalizedList listActions = new ActionSectionLocalizedList();
        private readonly RouteBuilder routeBuilder;
        private readonly RouteBuilderController routeBuilderController;

        public RouteBuilderAction(ControllerSectionLocalized controllerSection
            , ActionSectionLocalized currentAction
            , RouteBuilder routeBuilder
            , RouteBuilderController routeBuilderController)
        {
            this.currentControllerSection = controllerSection;
            this.currentAction = currentAction;
            this.routeBuilder = routeBuilder;
            this.routeBuilderController = routeBuilderController;
        }

        public ControllerSectionLocalizedList ToList()
        {
            return this.routeBuilder.ControllerList;
        }

        public AreaSectionLocalizedList ToListArea()
        {
            return this.routeBuilder.AreaList;
        }

        public IRouteBuilderAction_ToList UseEmptyUrl()
        {
            this.currentAction.Url = string.Empty;
            return this;
        }

        public IRouteBuilderAction_ToList UseDefaulUrl()
        {
            this.currentAction.Url = "{area}/{controller}/{action}";
            return this;
        }

        public IRouteBuilderAction_Constraints WithConstraints(object constraints)
        {
            if (this.currentAction.Constraints == null)
            {
                this.currentAction.Constraints = new RouteValueDictionary();
            }
            var rvd = this.currentAction.Constraints as RouteValueDictionary;
            if (rvd != null)
            {
                var c = constraints as RouteValueDictionary;
                if (c == null)
                {
                    c = new RouteValueDictionary(constraints);
                }
                c.ToList().ForEach(x => rvd.Add(x.Key, x.Value));
            }
            this.currentAction.Constraints = rvd;
            return this;
        }

        public IRouteBuilderAction_Constraints WithConstraints(string constraintName, object constraint)
        {
            if (this.currentAction.Constraints == null)
            {
                this.currentAction.Constraints = new RouteValueDictionary();
            }
            var rvd = this.currentAction.Constraints as RouteValueDictionary;
            if (rvd != null)
            {
                rvd.Add(constraintName, constraint);
            }
            return this;
        }

        public IRouteBuilderAction_Defaults WithDefaultValues(object values)
        {
            if (this.currentAction.Values == null)
            {
                this.currentAction.Values = new RouteValueDictionary();
            }
            var rvd = this.currentAction.Values as RouteValueDictionary;
            if (rvd != null)
            {
                var c = values as RouteValueDictionary;
                if (c == null)
                {
                    c = new RouteValueDictionary(values);
                }
                c.ToList().ForEach(x => rvd.Add(x.Key, x.Value));
            }
            this.currentAction.Values = rvd;

            return this;
        }

        public IRouteBuilderAction_Defaults WithDefaultValues(string defaultName, object value)
        {
            if (this.currentAction.Values == null)
            {
                this.currentAction.Values = new RouteValueDictionary();
            }
            var rvd = this.currentAction.Values as RouteValueDictionary;
            if (rvd != null)
            {
                rvd.Add(defaultName, value);
            }
            return this;
        }

        public IRouteBuilderAction_ToListWithoutUrl WithUrl(string url)
        {
            this.currentAction.Url = url;
            return this;
        }

        public IRouteBuilderAction_ToListOnlyWithAnd WithMirrorUrl(string url)
        {
            this.AddInActionList();
            var mirrorAction = new ActionSectionLocalized(this.currentAction.ActionName
                , this.currentAction.Translation
                , this.currentAction.Values
                , this.currentAction.Constraints
                , url);
            var s = new RouteBuilderAction(this.currentControllerSection, mirrorAction, this.routeBuilder, this.routeBuilderController);
            this.currentControllerSection.ActionTranslations.Add(mirrorAction);
            this.currentAction = mirrorAction;
            return s;
        }

        public IRouteBuilderController And()
        {
            this.AddInActionList();
            return this.routeBuilderController;
        }

        public IRouteBuilderControllerAndControllerConfiguration ForBilingualController(string controllerName, string controllerEnglishLocalizedString, string controllerFrenchLocalizedString)
        {
            this.AddInActionList();
            return this.routeBuilder.ForBilingualController(controllerName, controllerEnglishLocalizedString, controllerFrenchLocalizedString);
        }

        public IRouteBuilderArea ForBilingualArea(string areaName, string areaEnglishLocalizedString, string areaFrenchLocalizedString)
        {
            this.AddInActionList();
            return this.routeBuilder.ForBilingualArea(areaName, areaEnglishLocalizedString, areaFrenchLocalizedString);
        }

        public IRouteBuilderAction_ToList WithTranslatedTokens(string tokenKey, string english, string french)
        {
            if (this.currentAction != null)
            {
                if (this.currentAction.Tokens == null)
                {
                    this.currentAction.Tokens = new Dictionary<string, LocalizedSectionList>();
                }
                var tokenToAdd = new Dictionary<string, LocalizedSectionList>();
                if (this.currentAction.Tokens.Keys.Any(g => g == tokenKey))
                {
                    //Already exist, tbd what we do here, for now nothing
                }
                else
                {
                    this.currentAction.Tokens.Add(tokenKey, new LocalizedSectionList
                    {
                        new LocalizedSection(LocalizedSection.EN, english)
                        ,
                        new LocalizedSection(LocalizedSection.FR, french)
                    });
                }
            }
            return this;
        }

        public IRouteBuilderAction_ToListWithoutUrl AddDomainDefaultRoute(string controller, string action)
        {
            var controller1 = this.ForBilingualController("{controller}", "{controller}", "{controller}");
            var action1 = controller1.WithBilingualAction("{action}", "{action}", "{action}");
            var action2 = action1.WithDefaultValues(Constants.CONTROLLER, controller);
            var action3 = action2.WithDefaultValues(Constants.ACTION, action);
            var action4 = action3.WithUrl("{controller}/{action}");
            return action4;
        }

        public IRouteBuilderAction WithBilingualAction(string actionName, string actionEnglishLocalizedString, string actionFrenchLocalizedString)
        {
            this.AddInActionList();
            return this.routeBuilderController.WithBilingualAction(actionName, actionEnglishLocalizedString, actionFrenchLocalizedString);
        }

        private void AddInActionList()
        {
            if (this.currentAction != null)
            {
                this.listActions.Add(this.currentAction);
            }
        }
    }

    public interface IRouteBuilderAction : IRouteBuilderAction_Defaults, IRouteBuilderAction_Constraints, IRouteBuilderAction_Url, ITranslatedTokens, IRouteBuilderAction_ToList
    {
    }

    public interface IRouteBuilderAction_Defaults : IRouteBuilderAction_Constraints, IRouteBuilderAction_Url, IRouteBuilderAction_ToList
    {
        IRouteBuilderAction_Defaults WithDefaultValues(object values);
        IRouteBuilderAction_Defaults WithDefaultValues(string defaultName, object value);
    }

    public interface IRouteBuilderAction_Constraints : IRouteBuilderAction_Url, IRouteBuilderAction_ToList
    {
        IRouteBuilderAction_Constraints WithConstraints(object constraints);
        IRouteBuilderAction_Constraints WithConstraints(string constraintName, object constraint);
    }

    public interface IRouteBuilderAction_Url : IRouteBuilderAction_ToList, IRouteBuilder, IUrl
    {
        IRouteBuilderAction_ToList UseEmptyUrl();
        IRouteBuilderAction_ToList UseDefaulUrl();
    }

    public interface IRouteBuilderAction_ToList : IRouteBuilder, IAndAction, ITranslatedTokens, IDomainRoute, IUrl, IRouteBuilderAction_ToListOnly
    {
    }

    public interface IRouteBuilderAction_ToListWithoutUrl : IRouteBuilder, IAndAction, ITranslatedTokens, IDomainRoute, IRouteBuilderAction_ToListOnly, IUrlMirrorUrl
    {
    }

    public interface IRouteBuilderAction_ToListOnly
    {
        ControllerSectionLocalizedList ToList();
        AreaSectionLocalizedList ToListArea();
    }

    public interface IAndAction
    {
        IRouteBuilderController And();
    }

    public interface ITranslatedTokens
    {
        IRouteBuilderAction_ToList WithTranslatedTokens(string tokenKey, string english, string french);
    }

    public interface IDomainRoute
    {
        IRouteBuilderAction_ToListWithoutUrl AddDomainDefaultRoute(string controller, string action);
    }

    public interface IAssociateNamespace
    {
        IRouteBuilderControllerAndControllerConfiguration AssociateToNamespace(string @namespace);
    }

    public interface IRouteBuilderControllerAndControllerConfiguration : IRouteBuilderController, IAssociateNamespace
    {
    }

    public interface IRouteBuilderAction_ToListOnlyWithAnd : IRouteBuilderAction_ToListOnly, IAndAction, IUrlMirrorUrl, IRouteBuilder
    {
    }


    public interface IUrl : IUrlMirrorUrl
    {
        IRouteBuilderAction_ToListWithoutUrl WithUrl(string url);
    }

    public interface IUrlMirrorUrl
    {
        IRouteBuilderAction_ToListOnlyWithAnd WithMirrorUrl(string url);
    }
}