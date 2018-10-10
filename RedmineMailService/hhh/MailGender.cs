using System;
using System.Collections.Generic;
using System.Text;

namespace RedmineMailService
{
    public class MailGender
    {


        // http://localhost:10004/Kamikatze/ajax/MailService.ashx
        // https://gender-api.com/en/account/sign-up/complete
        // RedmineMailService.MailGender.GetGenders();
        public static void GetGenders()
        {
            string sql = "SELECT KT_Vorname FROM T_AP_Kontakte WHERE KT_Vorname <> '' ";
            string baseUrl = "https://api.genderize.io/?name=";


            using (System.Data.DataTable dt = SQL.GetDataTable(sql))
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    string name = System.Convert.ToString(dr["KT_Vorname"]);
                    string url = baseUrl + System.Uri.EscapeUriString(name);

                    string fileName = @"C:\Users\Administrator\Documents\Visual Studio 2017\Projects\RedmineMailService\RedmineMailService\Data\" + name + ".txt";

                    if (System.IO.File.Exists(fileName))
                        continue;

                    using (System.Net.WebClient wc = new System.Net.WebClient())
                    {
                        string json = wc.DownloadString(url);

                        System.IO.File.WriteAllText(@"C:\Users\Administrator\Documents\Visual Studio 2017\Projects\RedmineMailService\RedmineMailService\Data\" + name + ".txt", json, System.Text.Encoding.UTF8);
                        Genderize obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Genderize>(json);
                        System.Console.WriteLine(obj.IsMale);
                        System.Threading.Thread.Sleep(2000);
                    }

                }
            }

        }


    }

    // https://gender-api.com/get?name=Alice
    // GET https://gender-api.com/get?name=Diana&key=<your private server key>
    // https://api.genderize.io/?name=peter
    // https://api.genderize.io/?name[0]=peter&name[1]=lois&name[2]=stevie
    // https://genderize.io/

    public class RootObject
    {
        public string name { get; set; }
        public string name_sanitized { get; set; }
        public string gender { get; set; }
        public int samples { get; set; }
        public int accuracy { get; set; }
        public string duration { get; set; }
        public int credits_used { get; set; }
    }


    // {"name":"peter","gender":"male","probability":"0.99","count":796}
    public class Genderize
    {
        public string name { get; set; }
        public string gender { get; set; }
        public string probability { get; set; }
        public int count { get; set; }


        // KG: E32B51CF-3F85-4257-BB9C-09546D27DD7B
        // AA: BC4A3A37-90CA-4F3F-8C6D-04F1BA6B3DF6
        public bool IsMale
        {
            get
            {
                return System.StringComparer.InvariantCultureIgnoreCase.Equals(gender, "male");
            }
        }

        // KG: 5BF5A3AF-EE1F-4FE7-8212-D9ED655E86ED
        // AA: 7EAFA9B7-F8E4-4933-AB9B-50C838058B56
        public bool IsFemale
        {
            get
            {
                return System.StringComparer.InvariantCultureIgnoreCase.Equals(gender, "female");
            }
        }

        // AA: C7FE875C-1A60-4A0C-AFA9-030EB7FAE2F8
        public bool IsCompany
        {
            get
            {
                return !IsMale && !IsFemale;
            }
        }


    }




}
