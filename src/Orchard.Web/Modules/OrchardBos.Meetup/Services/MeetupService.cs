using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string TaskTypePrefix = "MeetupAggregator";

        public MeetupService(IOrchardServices services, IScheduledTaskManager taskManager) {
            _services = services;
            _taskManager = taskManager;
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


        public bool ProcessMeetupAggregation(MeetupPart meetup) {

            throw new NotImplementedException();
        }



        private void AssertValidPagerArguments(int? startIndex, int? pageSize) {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", "Value must be 0 or higher");

            if (pageSize == null || pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", "Value must be 1 or higher");
        }
    }
}