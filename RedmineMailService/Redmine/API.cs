
namespace RedmineMailService
{


    public class RedmineAPI
    {
        
        public static void CreateIssue()
        {
            Redmine.Net.Api.RedmineManager manager = CreateManager();

            // byte[] documentData = null;
            // string fileName = "";
            // Redmine.Net.Api.Types.Upload attachment = manager.UploadFile(documentData, fileName);

            System.Collections.Generic.List<Redmine.Net.Api.Types.Upload> attachments = 
                new System.Collections.Generic.List<Redmine.Net.Api.Types.Upload>();

            // attachments.Add(attachment);

            

            System.Collections.Generic.List<Redmine.Net.Api.Types.IssueCustomField> CustomFields =
                new System.Collections.Generic.List<Redmine.Net.Api.Types.IssueCustomField>
                {
                        new Redmine.Net.Api.Types.IssueCustomField
                        {
                            Id = 2,
                            // Name="Kundenname",

                            Values = new System.Collections.Generic.List<Redmine.Net.Api.Types.CustomFieldValue>
                            {
                                new Redmine.Net.Api.Types.CustomFieldValue
                                {
                                    Info = "INTERN"
                                }
                            }
                        }
                };


            manager.CreateObject(
                new Redmine.Net.Api.Types.Issue()
                {
                    // Id = 123,
                    CreatedOn = System.DateTime.Now,
                    // Author = null,
                    Author = manager.GetUserByEmail("servicedesk@cor-management.ch"),
                    //Project = Projects.test,
                    Project = new Redmine.Net.Api.Types.IdentifiableName() { Id = 6, Name = "15-Basic-V4" },
                    Subject = "i1iiI am a extemely new test Issue",
                    Description = "I i1iiiiam a extemely new test",
                    // Attachments = attachments,
                    Attachments = null,
                    Priority = ProjectData.Priorities.hoch,
                    StartDate = System.DateTime.Now,
                    EstimatedHours = 5.5f,
                    DueDate = System.DateTime.Now.AddHours(5.5),
                    SpentHours = 1.1f,
                    DoneRatio = 20.0f,

                    // AssignedTo = new Redmine.Net.Api.Types.IdentifiableName() { Id = 4 }, // Sprenger
                    AssignedTo = manager.GetUserByLogin("nobody"),

                    Category = null,
                    Children = null,
                    Changesets = null,
                    ClosedOn = System.DateTime.Now,


                    IsPrivate = false,
                    Journals = null,

                    Notes = null,
                    Relations = null,
                    // Tracker = null,
                    Tracker = ProjectData.Trackers.Fehler,

                    Watchers = null,
                    ParentIssue = null,

                    PrivateNotes = false,
                    // Status = new Redmine.Net.Api.Types.IdentifiableName() { Id = 1 },
                    Status = ProjectData.IssueStatus.Neu,
                    UpdatedOn = System.DateTime.Now,

                    // Uploads = uploads,
                    // Uploads = null,
                    Uploads = attachments,

                    FixedVersion = new Redmine.Net.Api.Types.IdentifiableName() { Id = 1 },
                    CustomFields = CustomFields
                }
            );
        }


        private static Redmine.Net.Api.RedmineManager CreateManager()
        {
            /*
            Redmine.Net.Api.RedmineManager rm2 = new Redmine.Net.Api.RedmineManager(
                 "host"
                ,"apikey"
                ,Redmine.Net.Api.MimeFormat.Xml
                ,false // Verify SSL-certificate
                ,null // Proxy
                ,System.Net.SecurityProtocolType.Tls
            );


            Redmine.Net.Api.RedmineManager rm = new Redmine.Net.Api.RedmineManager(
                 "host"
                ,Redmine.Net.Api.MimeFormat.Xml
                ,false // Verify SSL-certificate
                ,null // Proxy
                ,System.Net.SecurityProtocolType.Tls
            );
            */


            string dbUserId = SecretManager.GetSecret<string>("DefaultDbUser");
            string dbPw = SecretManager.GetSecret<string>("DefaultDbPassword");


            Redmine.Net.Api.RedmineManager redmineManager = new Redmine.Net.Api.RedmineManager(
                 // "http://redmine.cor.local"
                 //"http://redmine.cor-management.ch"

                 //"https://servicedesk.cor-management.ch/Servicedesk/"
                 "http://localhost:3000/"
                , SecretManager.GetSecret<string>("RedmineSuperUser")
                , SecretManager.GetSecret<string>("RedmineSuperUserPassword")
                , Redmine.Net.Api.MimeFormat.Xml
                , false // Verify SSL-certificate
                , null // Proxy
                       // ,new System.Net.WebProxy("http://127.0.0.1:80") // http(s)://<IP>:<Port> e.g. http://127.0.0.1:80
                , System.Net.SecurityProtocolType.Tls
            );

            return redmineManager;
        } // End Sub CreateManager 


    }


}
