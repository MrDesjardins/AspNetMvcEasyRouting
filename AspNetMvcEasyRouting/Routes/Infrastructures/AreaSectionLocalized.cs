namespace AspNetMvcEasyRouting.Routes.Infrastructures
{
    public class AreaSectionLocalized : IRouteElement
    {
        public string AreaName { get; set; }
        public LocalizedSectionList Translation { get; set; }
        public ControllerSectionLocalizedList ControllerTranslations { get; set; }

        public AreaSectionLocalized(string areaName, LocalizedSectionList translation, ControllerSectionLocalizedList controllersList)
        {
            this.AreaName = areaName;
            this.Translation = translation;
            this.ControllerTranslations = controllersList;
        }

        public void AcceptRouteVisitor(IRouteVisitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var controller in this.ControllerTranslations)
                {
                    controller.AcceptRouteVisitor(visitor);
                    if (visitor.HasFoundRoute)
                    {
                        break;
                    }
                }
            }
        }
    }
}