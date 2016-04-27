using System.Collections.Generic;

namespace AspNetMvcEasyRouting.Routes.Infrastructures
{
    /// <summary>
    ///     Action list that gives an entry point to visit
    /// </summary>
    public class ActionSectionLocalizedList : List<ActionSectionLocalized>
    {
        public void AcceptRouteVisitor(IRouteVisitor visitor)
        {
            foreach (var element in this)
            {
                ((IRouteElement) element).AcceptRouteVisitor(visitor);
                if (visitor.HasFoundRoute)
                {
                    break;
                }
            }
        }
    }
}