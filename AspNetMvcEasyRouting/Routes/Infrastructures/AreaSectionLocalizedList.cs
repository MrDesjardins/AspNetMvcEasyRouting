using System.Collections.Generic;

namespace AspNetMvcEasyRouting.Routes.Infrastructures
{
    /// <summary>
    ///     Area list that gives an entry point to visit
    /// </summary>
    public class AreaSectionLocalizedList : List<AreaSectionLocalized>
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