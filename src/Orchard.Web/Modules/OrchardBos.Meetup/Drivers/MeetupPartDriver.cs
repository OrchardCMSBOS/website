using OrchardBos.Meetup.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace OrchardBos.Meetup.Drivers
{
    public class MeetupPartDriver : ContentPartDriver<MeetupPart>
    {
        protected override string Prefix
        {
            get { return "Meetup"; }
        }

        protected override DriverResult Editor(MeetupPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Meetup_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Meetup", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(MeetupPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override DriverResult Display(MeetupPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_MeetupPart", () => shapeHelper.Parts_Meetup(
                GroupId: part.GroupId,
                ApiKey: part.ApiKey,
                MeetupAPIUrl: part.MeetupAPIUrl
            ));
        }
    }
}