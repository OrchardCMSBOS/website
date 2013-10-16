using System;
using System.Collections.Generic;
using System.Linq;
using OrchardBos.Meetup.Models;
using Orchard;
using Orchard.ContentManagement;

namespace OrchardBos.Meetup.Services
{
    public interface IMeetupService : IDependency {
        IEnumerable<MeetingListingEntry> GetMeetingListingForGroup(int id, int? startIndex = null, int? pageSize = 10, Func<IEnumerable<MeetingListingEntry>, IOrderedEnumerable<MeetingListingEntry>> orderBy = null);
        IContentQuery<MeetupPart, MeetupPartRecord> GetMeetupsForAggregation();
        IEnumerable<MeetupPart> GetMeetups(int? startIndex = null, int? pageSize = 10, Func<IContentQuery<MeetupPart, MeetupPartRecord>, IContentQuery<MeetupPart, MeetupPartRecord>> orderBy = null);
        IEnumerable<MeetingListingEntry> GetMeetupListing(int id, int? startIndex = null, int? pageSize = 10, Func<IEnumerable<MeetingListingEntry>, IOrderedEnumerable<MeetingListingEntry>> orderBy = null);
        int CountMeetups();
        int CountEventData(int id);
        MeetupPart GetMeetup(int id);
        bool ProcessMeetupAggregation(MeetupPart meetup);
    }
}