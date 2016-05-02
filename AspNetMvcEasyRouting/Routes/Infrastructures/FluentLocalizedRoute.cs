using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Routing;

namespace AspNetMvcEasyRouting.Routes.Infrastructures
{
    public static class FluentLocalizedRoute
    {
        public static ILocalRouteBuilder BuildRoute()
        {
            return new RouteBuilder();
        }
    }

    public interface ILocalRouteBuilder
    {
        IRouteBuilderAndILocalRouteBuilder InLocalRouteBuilder(CultureInfo culture);
        IRouteBuilderAndILocalRouteBuilder InLocalRouteBuilder(CultureInfo culture, string domainUrl);

    }

    public interface IRouteBuilderAndILocalRouteBuilder: IRouteBuilder, ILocalRouteBuilder
    {

    }

    public class RouteBuilder : IRouteBuilderAndILocalRouteBuilder
    {
        public ControllerSectionLocalizedList ControllerList { get; }
        public AreaSectionLocalizedList AreaList { get; }

        public readonly List<Locale> listSupportedLocale = new List<Locale>();

        public RouteBuilder()
        {
            this.ControllerList = new ControllerSectionLocalizedList();
            this.AreaList = new AreaSectionLocalizedList();
        }

        public IRouteBuilderControllerAndControllerConfiguration ForBilingualController(string controllerName, params string[] controllerLocalizedString)
        {
            if (this.listSupportedLocale.Count != controllerLocalizedString.Length)
            {
                throw new ArgumentException(string.Format("Localized string must be an array of {0} string for ForBilingualController controller {1}"
                    , this.listSupportedLocale.Count
                    , controllerName));
            }

            var section = new LocalizedSectionList();
            for (int index = 0; index < this.listSupportedLocale.Count; index++)
            {
                var supportedLocal = this.listSupportedLocale[index];
                section.Add(new LocalizedSection(new Locale(supportedLocal.CultureInfo), controllerLocalizedString[index]));
            }

            var controllerSectionLocalized = new ControllerSectionLocalized(controllerName, section, null);
            this.ControllerList.Add(controllerSectionLocalized);
            if (this.AreaList.Any())
            {
                this.AreaList.Last().ControllerTranslations.Add(controllerSectionLocalized);
            }
            var rbc = new RouteBuilderController(controllerSectionLocalized, this);
            return rbc;
        }

        public IRouteBuilderArea ForBilingualArea(string areaName, params string[] areaLocalizedString)
        {
            if (this.listSupportedLocale.Count != areaLocalizedString.Length)
            {
                throw new ArgumentException(string.Format("Localized string must be an array of {0} string for ForBilingualArea area {1}"
                    , this.listSupportedLocale.Count
                    , areaName));
            }
            var section = new LocalizedSectionList();
            for (int index = 0; index < this.listSupportedLocale.Count; index++)
            {
                var supportedLocal = this.listSupportedLocale[index];
                section.Add(new LocalizedSection(new Locale(supportedLocal.CultureInfo), areaLocalizedString[index]));
            }
            var areaLocalized = new AreaSectionLocalized(areaName, section, null);
            this.AreaList.Add(areaLocalized);
            var rbc = new RouteBuilderArea(areaLocalized, this);
            return rbc;
        }

        public IRouteBuilderAndILocalRouteBuilder InLocalRouteBuilder(CultureInfo culture)
        {
            this.listSupportedLocale.Add(new Locale(culture));
            return this;
        }

        public IRouteBuilderAndILocalRouteBuilder InLocalRouteBuilder(CultureInfo culture, string domainUrl)
        {
            this.listSupportedLocale.Add(new Locale(culture, domainUrl));
            return this;
        }

    }

    public interface IRouteBuilder
    {
        IRouteBuilderControllerAndControllerConfiguration ForBilingualController(string controllerName, params string[] controllerLocalizedString);
        IRouteBuilderArea ForBilingualArea(string areaName, params string[] areaLocalizedString);
    }

    public interface IRouteAreaBuilder
    {
        IRouteBuilderController ForBilingualArea(string areaName, params string[] areaLocalizedString);
    }


    public interface IRouteBuilderController
    {
        IRouteBuilderAction WithBilingualAction(string actionName, params string[] actionLocalizedString);
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

        public IRouteBuilderAction WithBilingualAction(string actionName, params string[] actionLocalizedString)
        {
            if (this.routeBuilder.listSupportedLocale.Count != actionLocalizedString.Length)
            {
                throw new ArgumentException(string.Format("Localized string must be an array of {0} string for WithBilingualAction action {1}"
                    , this.routeBuilder.listSupportedLocale.Count
                    , actionName));
            }

            var section = new LocalizedSectionList();
            for (int index = 0; index < this.routeBuilder.listSupportedLocale.Count; index++)
            {
                var supportedLocal = this.routeBuilder.listSupportedLocale[index];
                section.Add(new LocalizedSection(new Locale(supportedLocal.CultureInfo), actionLocalizedString[index]));
            }
            
            if (this.currentControllerSection.ActionTranslations == null)
            {
                this.currentControllerSection.ActionTranslations = new ActionSectionLocalizedList();
            }
            var currentAction = new ActionSectionLocalized(actionName, section);
            this.currentControllerSection.ActionTranslations.Add(currentAction);
            return new RouteBuilderAction(this.currentControllerSection, currentAction, this.routeBuilder, this);
        }
    }

    public interface IRouteBuilderArea
    {
        IRouteBuilderControllerAndControllerConfiguration WithBilingualController(string controllerName, params string[] areaLocalizedString);
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

        public IRouteBuilderControllerAndControllerConfiguration WithBilingualController(string controllerName, params string[] controllerLocalizedString)
        {
            if (this.routeBuilder.listSupportedLocale.Count != controllerLocalizedString.Length)
            {
                throw new ArgumentException(string.Format("Localized string must be an array of {0} string for WithBilingualController controller {1}"
                    , this.routeBuilder.listSupportedLocale.Count
                    , controllerName));
            }

            var section = new LocalizedSectionList();
            for (int index = 0; index < this.routeBuilder.listSupportedLocale.Count; index++)
            {
                var supportedLocal = this.routeBuilder.listSupportedLocale[index];
                section.Add(new LocalizedSection(new Locale(supportedLocal.CultureInfo), controllerLocalizedString[index]));
            }


            if (this.currentControllerSection.ControllerTranslations == null)
            {
                this.currentControllerSection.ControllerTranslations = new ControllerSectionLocalizedList();
            }

            var controllerSectionLocalized = new ControllerSectionLocalized(controllerName, section, null);


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

        public IRouteBuilderControllerAndControllerConfiguration ForBilingualController(string controllerName, params string[] controllerLocalizedString)
        {
            this.AddInActionList();
            return this.routeBuilder.ForBilingualController(controllerName, controllerLocalizedString);
        }

        public IRouteBuilderArea ForBilingualArea(string areaName, params string[] areaLocalizedString)
        {
            this.AddInActionList();
            return this.routeBuilder.ForBilingualArea(areaName, areaLocalizedString);
        }

        public IRouteBuilderAction_ToList WithTranslatedTokens(string tokenKey, params string[] localizedToken)
        {
            if (this.routeBuilder.listSupportedLocale.Count != localizedToken.Length)
            {
                throw new ArgumentException(string.Format("Localized string must be an array of {0} string for WithTranslatedTokens token {1}"
                    , this.routeBuilder.listSupportedLocale.Count
                    , tokenKey));
            }

            var section = new LocalizedSectionList();
            for (int index = 0; index < this.routeBuilder.listSupportedLocale.Count; index++)
            {
                var supportedLocal = this.routeBuilder.listSupportedLocale[index];
                section.Add(new LocalizedSection(new Locale(supportedLocal.CultureInfo), localizedToken[index]));
            }

            if (this.currentAction != null)
            {
                if (this.currentAction.Tokens == null)
                {
                    this.currentAction.Tokens = new Dictionary<string, LocalizedSectionList>();
                }

                if (this.currentAction.Tokens.Keys.Any(g => g == tokenKey))
                {
                    //Already exist, tbd what we do here, for now nothing
                }
                else
                {
                    this.currentAction.Tokens.Add(tokenKey, section);
                }
            }
            return this;
        }

        public IRouteBuilderAction WithBilingualAction(string actionName, params string[] actionLocalizedString)
        {
            this.AddInActionList();
            return this.routeBuilderController.WithBilingualAction(actionName, actionLocalizedString);
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

    public interface IRouteBuilderAction_ToList : IRouteBuilder, IAndAction, ITranslatedTokens, IUrl, IRouteBuilderAction_ToListOnly
    {
    }

    public interface IRouteBuilderAction_ToListWithoutUrl : IRouteBuilder, IAndAction, ITranslatedTokens, IRouteBuilderAction_ToListOnly, IUrlMirrorUrl
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
        IRouteBuilderAction_ToList WithTranslatedTokens(string tokenKey, params string[] tokenTranslated);
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