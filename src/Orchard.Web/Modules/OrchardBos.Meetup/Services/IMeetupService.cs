using System;
using System.Collections.Generic;
using System.Linq;
using OrchardBos.Meetup.Models;
using Orchard;
using Orchard.ContentManagement;

namespace OrchardBos.Meetup.Services
{
    public interface IMeetupService : IDependency
    {
        IEnumerable<MeetupPart> GetMeetups(int? startIndex = null, int? pageSize = 10, Func<IContentQuery<MeetupPart, MeetupPartRecord>, IContentQuery<MeetupPart, MeetupPartRecord>> orderBy = null);
        int CountMeetups();
        MeetupPart GetMeetup(int id);
        bool ProcessMeetupAggregation(MeetupPart meetup);
    }
}