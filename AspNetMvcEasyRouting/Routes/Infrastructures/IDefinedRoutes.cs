namespace AspNetMvcEasyRouting.Routes.Infrastructures
{
    public interface IDefinedRoutes
    {
        ControllerSectionLocalizedList Website { get; }
        AreaSectionLocalizedList Contest { get; }
        AreaSectionLocalizedList Api { get; }
        AreaSectionLocalizedList Moderation { get; }
        AreaSectionLocalizedList Administration { get; }
    }
}