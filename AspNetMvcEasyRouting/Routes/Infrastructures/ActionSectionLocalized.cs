using System.Collections.Generic;

namespace AspNetMvcEasyRouting.Routes.Infrastructures
{
    public class ActionSectionLocalized : IRouteElement
    {
        public string ActionName { get; set; }
        public LocalizedSectionList Translation { get; set; }

        public object Values { get; set; }
        public object Constraints { get; set; }
        public string Url { get; set; }
        public Dictionary<string, LocalizedSectionList> Tokens { get; set; }

        public ActionSectionLocalized(string actionName, LocalizedSectionList translation, object values = null, object constraints = null, string url = "")
        {
            this.ActionName = actionName;
            this.Translation = translation;
            this.Values = values;
            this.Constraints = constraints;
            this.Url = url;
        }

        public void AcceptRouteVisitor(IRouteVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}