using Orchard.UI.Navigation;
using Orchard.Localization;

namespace OrchardBos.Meetup
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Meetup Groups"), "3",
                menu => menu.Add(T("List"), "0", item => item.Action("Index", "Admin", new { area = "OrchardBos.Meetup" })));
        }

        public string MenuName { get { return "admin"; } }
    }
}