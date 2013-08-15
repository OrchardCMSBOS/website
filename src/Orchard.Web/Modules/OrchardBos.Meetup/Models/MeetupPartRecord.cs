using System;
using Orchard.ContentManagement.Records;

namespace OrchardBos.Meetup.Models
{
    public class MeetupPartRecord : ContentPartRecord
    {
        public virtual string GroupId { get; set; }
        public virtual string MeetupAPIUrl { get; set; }
        public virtual string ApiKey { get; set; }
        public virtual DateTime? FetchTimeOfDay { get; set; }
        public virtual DateTime? LastAsOfDate { get; set; }
        public virtual bool FetchEnabled { get; set; }
    }
}