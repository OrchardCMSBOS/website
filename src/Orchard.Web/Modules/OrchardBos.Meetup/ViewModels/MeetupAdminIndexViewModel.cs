using System.Collections.Generic;
using OrchardBos.Meetup.Models;

namespace OrchardBos.Meetup.ViewModels
{
    public class MeetupAdminIndexViewModel
    {
        public List<MeetupPart> Meetups { get; set; }
        public dynamic Pager { get; set; }
    }

}
