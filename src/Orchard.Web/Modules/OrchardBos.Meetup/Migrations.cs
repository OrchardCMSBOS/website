using System;
using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Meetup {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("MeetupPartRecord", table => table

                            // The following method will create an "Id" column for us and set it is the primary key for the table
                            .ContentPartRecord()


                            .Column<string>("GroupId")

                            .Column<string>("MeetupAPIUrl")
                            .Column<string>("ApiKey")
                            .Column<DateTime>("LastAsOfDate", c => c.Nullable())
                            .Column<DateTime>("FetchTimeOfDay", c => c.Nullable())
                            .Column<bool>("FetchEnabled")
                );



            ContentDefinitionManager.AlterPartDefinition("MeetupPart", f => f.Attachable(false));

            ContentDefinitionManager.AlterTypeDefinition("MeetupGroup", cfg => cfg
                                .WithPart("MeetupPart")
                                .WithPart("CommonPart")
                                .WithPart("TitlePart")
                                .WithPart("AutoroutePart", builder => builder
                                    .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: 'meetups/{TypeDefinition.Fields.Meetup.GroupId}', Description: 'group-pattern'}]")
                                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                                .Creatable(false)
                                .Draftable(false));


            // Return the version that this feature will be after this method completes
            return 1;
        }

    }
}
