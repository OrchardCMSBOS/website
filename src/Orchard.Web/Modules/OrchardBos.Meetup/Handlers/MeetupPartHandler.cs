using OrchardBos.Meetup.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace OrchardBos.Meetup.Handlers
{

    public class MeetupPartHandler : ContentHandler
    {
        public MeetupPartHandler(IRepository<MeetupPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}