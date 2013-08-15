using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using System;

namespace OrchardBos.Meetup.Models
{
    public class MeetupPart : ContentPart<MeetupPartRecord>
    {

        public string GroupId
        {
            get { return Record.GroupId; }
            set { Record.GroupId = value; }
        }

        public string MeetupAPIUrl
        {
            get { return Record.MeetupAPIUrl; }
            set { Record.MeetupAPIUrl = value; }
        }

        public string ApiKey
        {
            get { return Record.ApiKey; }
            set { Record.ApiKey = value; }
        }

        public DateTime? FetchTimeOfDay
        {
            get { return Record.FetchTimeOfDay; }
            set { Record.FetchTimeOfDay = value; }
        }


        public DateTime? LastAsOfDate
        {
            get { return Record.LastAsOfDate; }
            set { Record.LastAsOfDate = value; }
        }

        public DateTime? NextRun { get; set; }

        public bool FetchEnabled
        {
            get { return Record.FetchEnabled; }
            set { Record.FetchEnabled = value; }
        }
    }
}