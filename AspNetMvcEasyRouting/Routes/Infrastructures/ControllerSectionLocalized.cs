namespace AspNetMvcEasyRouting.Routes.Infrastructures
{
    public class ControllerSectionLocalized : IRouteElement
    {
        public string ControllerName { get; set; }
        public LocalizedSectionList Translation { get; set; }
        public ActionSectionLocalizedList ActionTranslations { get; set; }
        public string[] NameSpaces { get; set; }

        public ControllerSectionLocalized(string controllerName, LocalizedSectionList translation, ActionSectionLocalizedList actionsList)
        {
            this.ControllerName = controllerName;
            this.Translation = translation;
            this.ActionTranslations = actionsList;
        }

        public void AcceptRouteVisitor(IRouteVisitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var action in this.ActionTranslations)
                {
                    action.AcceptRouteVisitor(visitor);
                    if (visitor.HasFoundRoute)
                    {
                        break;
                    }
                }
            }
        }
    }
}