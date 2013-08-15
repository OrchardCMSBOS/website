using System.Linq;
using System.Web.Mvc;
using OrchardBos.Meetup.Models;
using OrchardBos.Meetup.Services;
using OrchardBos.Meetup.ViewModels;
using Orchard.Settings;
using Orchard;
using Orchard.Logging;
using Orchard.Localization;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Mvc.Extensions;

namespace OrchardBos.Meetup.Controllers
{
    public class AdminController : Controller
    {
        private readonly IOrchardServices _services;
        private readonly IMeetupService _meetupService;
        private readonly ISite _site;
        private readonly INotifier _notifier;

        public AdminController(IOrchardServices services, IMeetupService meetupService)
        {
            _services = services;
            _meetupService = meetupService;
            _site = services.WorkContext.CurrentSite;
            Shape = services.New;
            Log = NullLogger.Instance;
            T = NullLocalizer.Instance;
            _notifier = services.Notifier;
        }

        dynamic Shape { get; set; }
        public ILogger Log { get; set; }
        public Localizer T { get; set; }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var pager = new Pager(_site, pagerParameters);
            var meetups = _meetupService.GetMeetups(pager.GetStartIndex(pager.Page), pager.PageSize, q => q.OrderByDescending(x => x.GroupId)).ToList();
            var meetupCount = _meetupService.CountMeetups();
            var pagerShape = Shape.Pager(pager).TotalItemCount(meetupCount);

            var model = new MeetupAdminIndexViewModel
            {
                Meetups = meetups,
                Pager = pagerShape
            };

            return View(model);
        }

    }
}