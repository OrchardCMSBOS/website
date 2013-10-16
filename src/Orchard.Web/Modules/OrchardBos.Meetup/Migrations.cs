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

        public int UpdateFrom1() {

            SchemaBuilder.CreateTable("MeetingListingEntry", table => table
				.Column<int>("Id", c => c.PrimaryKey().Identity())

				// Create a column named "EventDate" of type "datetime"
				.Column<DateTime>("EventDate")

                // Create the "MeetingDescription" column
                .Column<string>("MeetingDescription")

                .Column<int>("MeetupId"));

            return 2;
        }

        public int UpdateFrom2() {

            SchemaBuilder.AlterTable("MeetingListingEntry", table => table

                    .AlterColumn("MeetingDescription",column => column.WithType(DbType.String).WithLength(4000)));

            SchemaBuilder.AlterTable("MeetingListingEntry", table => table

                    .AddColumn<string>("Name", column => column.WithType(DbType.String).WithLength(255)));

            SchemaBuilder.AlterTable("MeetingListingEntry", table => table

                .AddColumn<string>("MeetupMeetingId", column => column.WithType(DbType.String).WithLength(128)));
            

            return 3;
        }

        public int UpdateFrom3() {

            SchemaBuilder.CreateTable("MeetingDisplayPartRecord", table => table

                    // The following method will create an "Id" column for us and set it is the primary key for the table
                    .ContentPartRecord()

                    // Create a column for the meetup we want to link to
                    .Column<string>("GroupIdss")
                );

            ContentDefinitionManager.AlterPartDefinition("MeetingDisplayPart", f => f.Attachable(false));

            ContentDefinitionManager.AlterTypeDefinition("MeetingDisplayWidget", cfg => cfg
                                                    .WithPart("MeetingDisplayPart")
                                                    .WithPart("WidgetPart")
                                                    .WithPart("CommonPart")
                                                    .WithSetting("Stereotype", "Widget"));

            return 4;
        }
    }
}
