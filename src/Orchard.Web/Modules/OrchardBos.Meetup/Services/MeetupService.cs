using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json;
using OrchardBos.Meetup.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Tasks.Scheduling;

namespace OrchardBos.Meetup.Services
{
    public class MeetupService : IMeetupService {
        private readonly IOrchardServices _services;
        private readonly IScheduledTaskManager _taskManager;
        private readonly IRepository<MeetingListingEntry> _meetingEntryRepository;
        private const string TaskTypePrefix = "MeetupAggregator";

        public MeetupService(IOrchardServices services, IScheduledTaskManager taskManager, IRepository<MeetingListingEntry> meetingEntryRepository)
        {
            _services = services;
            _taskManager = taskManager;
            _meetingEntryRepository = meetingEntryRepository;
        }

        public IEnumerable<MeetingListingEntry> GetMeetingListingForGroup(int id, int? startIndex = null, int? pageSize = 10, Func<IEnumerable<MeetingListingEntry>, IOrderedEnumerable<MeetingListingEntry>> orderBy = null)
        {
            var query = _meetingEntryRepository.Fetch(x => x.MeetupId == id);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (startIndex != null)
            {
                AssertValidPagerArguments(startIndex, pageSize);
                query = query.Skip(startIndex.Value).Take(pageSize.Value);
            }

            return query;
        }

        public IContentQuery<MeetupPart, MeetupPartRecord> GetMeetupsForAggregation()
        {
            return _services.ContentManager
                .Query<MeetupPart, MeetupPartRecord>().Where(x => x.FetchEnabled);
        }

        public IEnumerable<MeetupPart> GetMeetups(int? startIndex = null, int? pageSize = 10, Func<IContentQuery<MeetupPart, MeetupPartRecord>, IContentQuery<MeetupPart, MeetupPartRecord>> orderBy = null) {
            var query = _services.ContentManager.Query<MeetupPart, MeetupPartRecord>();
            IEnumerable<MeetupPart> meetups;

            if (query.Count() > 0) {
                if (orderBy != null) {
                    query = orderBy(query);
                }

                if (startIndex != null) {
                    AssertValidPagerArguments(startIndex, pageSize);
                    meetups = query.Slice(startIndex.Value, pageSize.Value);
                }
                else {
                    meetups = query.List();
                }

                foreach (var meetup in meetups) {
                    var meetupTaskType = TaskTypePrefix + "_" + meetup.GroupId;
                    var task = _taskManager.GetTasks(meetup.ContentItem).SingleOrDefault(t => t.TaskType == meetupTaskType);
                    meetup.NextRun = task != null ? task.ScheduledUtc : default(DateTime?);
                    yield return meetup;
                }
            }
        }

        public int CountMeetups() {
            return _services.ContentManager.Query<MeetupPart, MeetupPartRecord>().Count();
        }

        public MeetupPart GetMeetup(int id) {
            return _services.ContentManager.Get<MeetupPart>(id);
        }

        public IEnumerable<MeetingListingEntry> GetMeetupListing(int id, int? startIndex = null, int? pageSize = 10, Func<IEnumerable<MeetingListingEntry>, IOrderedEnumerable<MeetingListingEntry>> orderBy = null)
        {
            var query = _meetingEntryRepository.Fetch(x => x.MeetupId == id);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (startIndex != null)
            {
                AssertValidPagerArguments(startIndex, pageSize);
                query = query.Skip(startIndex.Value).Take(pageSize.Value);
            }

            return query;
        }

        public int CountEventData(int id)
        {
            return _meetingEntryRepository.Count(x => x.MeetupId == id);
        }

        public bool ProcessMeetupAggregation(MeetupPart meetup) {

            if (!string.IsNullOrEmpty(meetup.MeetupAPIUrl))
            {
                using (var httpGetTokenClient = new HttpClient())
                {
                    //get all past, cancelled, and upcoming events : http://www.meetup.com/meetup_api/docs/2/events/
                    var responseContainingToken = httpGetTokenClient.GetAsync(meetup.MeetupAPIUrl + "?group_id=" + meetup.GroupId + "&key=" + meetup.ApiKey + "&status=upcoming,past,cancelled").Result;

                    if (responseContainingToken.IsSuccessStatusCode) {
                        dynamic response = JsonConvert.DeserializeObject<dynamic>(responseContainingToken.Content.ReadAsStringAsync().Result);
                        string meetingId = string.Empty; 
                        string meetingDescription;
                        string meetingName;
                        double dateEpochValue;


                        if (response.results != null && response.results.Count > 0) {
                            foreach (dynamic d in response.results) {

                                meetingName = Convert.ToString(d.name.Value);
                                meetingDescription = Convert.ToString(d.description.Value);
                                dateEpochValue = Convert.ToDouble(d.created.Value);
                                meetingId = Convert.ToString(d.id.Value);

                                var epochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                                var createdDate = epochDate.AddMilliseconds(dateEpochValue);

                                //if we do not have this meeting in the db yet, lets save it
                                MeetingListingEntry iep = _meetingEntryRepository.Table.SingleOrDefault(x => meetingId == x.MeetupMeetingId);

                                if (iep == null) {
                                    iep = new MeetingListingEntry() {
                                        MeetupMeetingId = meetingId,
                                        EventDate = createdDate,
                                        Name = meetingName,
                                        MeetupId = meetup.Id,
                                        MeetingDescription = meetingDescription
                                    };
                                }

                                SaveMeetingEntry(iep);

                            }
                        }

                        //update the record for last meeting date
                        meetup.LastAsOfDate = DateTime.UtcNow;

                    }
                }

            }

            return false;
        }

        public void SaveMeetingEntry(MeetingListingEntry entry)
        {
            _meetingEntryRepository.Create(entry);
        }

        public MeetingListingEntry GetMeetingEntry(int id)
        {
            return _meetingEntryRepository.Get(id);
        }


        private void AssertValidPagerArguments(int? startIndex, int? pageSize) {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", "Value must be 0 or higher");

            if (pageSize == null || pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", "Value must be 1 or higher");
        }
    }
}