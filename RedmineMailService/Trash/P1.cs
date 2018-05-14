
using System.Collections.Generic;
using System.Windows.Forms;


namespace RedmineClient
{


    static class Tests
    {


        static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            foreach (char c in normalizedString)
            {
                System.Globalization.UnicodeCategory unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                } // End if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark) 

            } // Next c 

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        } // End Function RemoveDiacritics 


        public static string GenerateVariableSlug(this string phrase)
        {
            string str = RemoveDiacritics(phrase).ToLowerInvariant();
            // invalid chars           
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "_"); // hyphens   
            str = str.Replace("-", "_");


            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("^[0-9].*");
            if (reg.IsMatch(str))
                str = "___" + str;
            
            return str;
        } // End Function GenerateVariableSlug 


        public static Redmine.Net.Api.Types.IdentifiableName GetUser(string login)
        {
            Redmine.Net.Api.RedmineManager redman = RedmineFactory.CreateInstance();

            List<Redmine.Net.Api.Types.User> users = redman.GetObjects<Redmine.Net.Api.Types.User>();
            System.Console.WriteLine(users);

            for (int i = 0; i < users.Count; ++i)
            {

                if (string.Equals(users[i].Login, login, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return new Redmine.Net.Api.Types.IdentifiableName() { Id = users[i].Id };
                } // End if (string.Equals(users[i].Login, login, System.StringComparison.InvariantCultureIgnoreCase)) 

            } // Next i 

            return null;
        } // End Function GetUser 


        public static void TestMain()
        {
#if false
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
#endif

            Redmine.Net.Api.RedmineManager redman = RedmineFactory.CreateInstance();


#if false
            List<Redmine.Net.Api.Types.Project> projects = redman.GetObjects<Redmine.Net.Api.Types.Project>(100, 0);
            foreach (Redmine.Net.Api.Types.Project thisProject in projects)
            {
                string str = "public static Redmine.Net.Api.Types.IdentifiableName " + GenerateVariableSlug(thisProject.Identifier) + " = new Redmine.Net.Api.Types.IdentifiableName() { Id = " + thisProject.Id + " };";
                System.Console.WriteLine(str);

                
            } // Next thisProject 

            System.Collections.Specialized.NameValueCollection parameters = 
                new System.Collections.Specialized.NameValueCollection { { "status_id", "*" } };

            // https://stackoverflow.com/questions/14839712/nginx-reverse-proxy-passthrough-basic-authenication
            // https://stackoverflow.com/questions/19751313/forward-request-headers-from-nginx-proxy-server
            
            
            List<Redmine.Net.Api.Types.Issue> issues = redman.GetObjects<Redmine.Net.Api.Types.Issue>(parameters);
            foreach (Redmine.Net.Api.Types.Issue thisIssue in issues)
            {
                System.Console.WriteLine("#{0}: {1}", thisIssue.Id, thisIssue.Subject);
            } // Next issue 

            
            List<Redmine.Net.Api.Types.IssuePriority> priorities = redman.GetObjects<Redmine.Net.Api.Types.IssuePriority>();
            foreach (Redmine.Net.Api.Types.IssuePriority thisPriority in priorities)
            {
                System.Console.WriteLine(thisPriority);
            }


            List<Redmine.Net.Api.Types.Tracker> trackers = redman.GetObjects<Redmine.Net.Api.Types.Tracker>();
            foreach (Redmine.Net.Api.Types.Tracker thisTracker in trackers)
            {
                System.Console.WriteLine(thisTracker);
            }
#endif


            // https://www.redmine.org/projects/redmine/wiki/Rest_API
            // Enable REST API: in Administration -> Settings -> Authentication => Enable REST web service

            /*
            redman.CreateObject<Redmine.Net.Api.Types.Project>(
                new Redmine.Net.Api.Types.Project()
                {
                    // Id = 77,
                    Name = "1112ApiTestProject - Name",
                    CreatedOn = System.DateTime.Now,
                    Description = "111Api Test Project",
                    EnabledModules = null,
                    HomePage = "",
                    Identifier = "111ApiTestProject".ToLowerInvariant(),
                    InheritMembers = false,
                    IsPublic = false,
                    IssueCategories = null,
                    Parent = null,
                    Status = 0,
                    Trackers = null, // Enabled "categories" - Supportanfrage, Anpassung, Fehler, Handling
                    UpdatedOn = System.DateTime.Now
                }
            );
            */

            /*
            //Create a issue.
            Redmine.Net.Api.Types.Issue newIssue = new Redmine.Net.Api.Types.Issue
            { 
                 Subject = "test" 
                ,Project = new Redmine.Net.Api.Types.IdentifiableName { Id = 1 } 
            };
            redman.CreateObject(newIssue);
            */



            

            
            //List<Redmine.Net.Api.Types.Upload> trackers = redman.GetObjects<Redmine.Net.Api.Types.Upload>();
            //foreach (Redmine.Net.Api.Types.Upload thisUpload in trackers)
            //{
            //    System.Console.WriteLine(thisUpload);
            //}


            /*
            // https://www.redmine.org/boards/2/topics/42476
            // https://github.com/zapadi/redmine-net-api/wiki/Upload-file
            byte[] fileData = System.IO.File.ReadAllBytes(@"D:\stefan.steiger\Downloads\signature.png");
            Redmine.Net.Api.Types.Upload fileToAttach = redman.UploadFile(fileData);
            System.Console.WriteLine(fileToAttach);
            fileToAttach.FileName = "foobar.png";
            fileToAttach.Description = "File uploaded using REST API";
            fileToAttach.ContentType = "image/png";
            */

            // List<Redmine.Net.Api.Types.Tracker> ls = redman.GetObjects<Redmine.Net.Api.Types.Tracker>("apitestproject");
            // System.Console.WriteLine(ls);


            //// attachment
            //Redmine.Net.Api.Types.Attachment attach = new Redmine.Net.Api.Types.Attachment();
            //attach.ContentUrl = "http://redmine.cor.local/attachments/download/9/7_6R55O.jpg";
            ////attach.Id = 9;
            //attach.FileName = "foobar.png";
            //attach.FileSize = fileData.Length;
            //attach.CreatedOn = System.DateTime.Now;
            //attach.ContentType = "image/png";
            //attach.Description = "OMG";


            //List<Redmine.Net.Api.Types.Attachment> attachments = new List<Redmine.Net.Api.Types.Attachment>();
            //attachments.Add(attach);


            /*
            List<Redmine.Net.Api.Types.Tracker> trackers = redman.GetObjects<Redmine.Net.Api.Types.Tracker>();
            System.Console.WriteLine(trackers);


            List<Redmine.Net.Api.Types.Upload> uploads = new List<Redmine.Net.Api.Types.Upload>();
            uploads.Add(fileToAttach);
            */

            List<Redmine.Net.Api.Types.IssueCustomField> CustomFields = 
                new List<Redmine.Net.Api.Types.IssueCustomField>
                {
                        new Redmine.Net.Api.Types.IssueCustomField
                        {
                            Id = 2,
                            // Name="Kundenname",

                            Values = new List<Redmine.Net.Api.Types.CustomFieldValue>
                            {
                                new Redmine.Net.Api.Types.CustomFieldValue
                                {
                                    Info = "INTERN"
                                }
                            }
                        }
                };


            string fn = @"D:\username\Pictures\umm-al-maa-idehan-ubari-sand-sea-libya-1.jpg";
            fn = "/home/<user>/Pictures/DA-NANG-CITY.jpg";
            
            byte[] documentData = System.IO.File.ReadAllBytes(fn);

            // Fixed: https://www.redmine.org/projects/redmine/wiki/Rest_api
            string fileName = System.IO.Path.GetFileName(fn);
            Redmine.Net.Api.Types.Upload attachment = redman.UploadFile(documentData, fileName);
            // attachment.FileName = fileName;
            attachment.Description = "A test file upload";
            attachment.ContentType = "image/jpeg";
            // image/jpeg image/png image/gif image/bmp image/svg+xml image/tiff image/webp application/x-msmetafile

            List<Redmine.Net.Api.Types.Upload> attachments = new List<Redmine.Net.Api.Types.Upload>();
            attachments.Add(attachment);
            

            redman.CreateObject<Redmine.Net.Api.Types.Issue>(
                new Redmine.Net.Api.Types.Issue()
                {
                    // Id = 123,
                    CreatedOn = System.DateTime.Now,
                    Author = null,
                    //Project = Projects.test,
                    Project = new Redmine.Net.Api.Types.IdentifiableName() { Id = 6, Name = "15-Basic-V4" },
                    Subject = "i1iiI am a extemely new test Issue",
                    Description = "I i1iiiiam a extemely new test",
                    // Attachments = attachments,
                    Attachments = null,
                    Priority = Priorities.hoch,
                    StartDate = System.DateTime.Now,
                    EstimatedHours = 5.5f,
                    DueDate = System.DateTime.Now.AddHours(5.5),
                    SpentHours = 1.1f,
                    DoneRatio = 20.0f,

                    // AssignedTo = new Redmine.Net.Api.Types.IdentifiableName() { Id = 4 }, // Sprenger
                    // AssignedTo = new Redmine.Net.Api.Types.IdentifiableName() { Id = 6 }, // Zihlmann
                    AssignedTo = GetUser("nobody"),
                    Category = null,
                    Children = null,
                    Changesets = null,
                    ClosedOn = System.DateTime.Now,


                    IsPrivate = false,
                    Journals = null,

                    Notes = null,
                    Relations = null,
                    // Tracker = null,
                    Tracker = Trackers.Fehler,

                    Watchers = null,
                    ParentIssue = null,

                    PrivateNotes = false,
                    // Status = new Redmine.Net.Api.Types.IdentifiableName() { Id = 1 },
                    Status = IssueStatus.Neu,
                    UpdatedOn = System.DateTime.Now,

                    // Uploads = uploads,
                    // Uploads = null,
                    Uploads = attachments,

                    FixedVersion = new Redmine.Net.Api.Types.IdentifiableName() { Id = 1 },
                    CustomFields = CustomFields
                }
            );
            

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
             System.Console.ReadKey();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace RedmineClient 
