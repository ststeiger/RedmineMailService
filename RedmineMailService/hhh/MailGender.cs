
namespace RedmineMailService
{


    public class MailGender
    {
	    
	    
	    public static void UpdateKontakt(string kt_uid, Genderize genderData)
	    {
		    string sql = @"
UPDATE T_AP_Kontakte 
	SET KT_KG_UID = @kg_uid  
WHERE KT_UID = @kt_uid
;";


		    using (System.Data.IDbCommand cmd = SQL.CreateCommand(sql))
		    {
			    string kg_uid = null;
			    
			    if (genderData.IsMale)
			    {
				    kg_uid = "E32B51CF-3F85-4257-BB9C-09546D27DD7B";
			    }
			    else if (genderData.IsFemale)
			    {
				    kg_uid = "5BF5A3AF-EE1F-4FE7-8212-D9ED655E86ED";
			    }
			    else
			    {
				    System.Console.WriteLine(genderData);
			    }
			    
			    SQL.AddParameter(cmd, "kg_uid", kg_uid);
			    SQL.AddParameter(cmd, "kt_uid", kt_uid);
			    
			    SQL.ExecuteNonQuery(cmd);
		    } // End Using cmd 
		    
	    } // End Sub UpdateKontakt 
	    
	    
	    // http://localhost:10004/Kamikatze/ajax/MailService.ashx
        // https://gender-api.com/en/account/sign-up/complete
        // RedmineMailService.MailGender.GetGenders();
        public static void GetGenders()
        {
            string sql = "SELECT KT_UID, KT_Vorname FROM T_AP_Kontakte WHERE KT_KG_UID IS NULL AND KT_Vorname <> '' ORDER BY KT_Vorname DESC";
            string baseUrl = "https://api.genderize.io/?name=";
	        
	        string dataDir = System.IO.Path.GetDirectoryName(typeof(MailGender).Assembly.Location);
	        dataDir = System.IO.Path.Combine(dataDir, "..", "..", "..", "Data");
	        dataDir = System.IO.Path.GetFullPath(dataDir);

            if("RZ".Equals(System.Environment.UserDomainName, System.StringComparison.OrdinalIgnoreCase))
                dataDir = @"C:\Users\stefan.steiger.RZ\Desktop\Data\Data";


            using (System.Data.DataTable dt = SQL.GetDataTable(sql))
            {

                // public WebProxy(string Host, int Port);
                //System.Net.WebProxy wp = new System.Net.WebProxy("http://193.109.47.193", 41083);
                System.Net.WebProxy wp = new System.Net.WebProxy("186.178.10.158", 8888);
                // wp.UseDefaultCredentials = true;
                // wp.UseDefaultCredentials = false;
                // wp.Credentials = new System.Net.NetworkCredential("usernameHere", "pa****rdHere");  //These can be replaced by user input
                wp.BypassProxyOnLocal = false;  //still use the proxy for local addresses

                foreach (System.Data.DataRow dr in dt.Rows)
                {
	                string uid = System.Convert.ToString(dr["KT_UID"]);
	                string name = System.Convert.ToString(dr["KT_Vorname"]);

                    if (string.IsNullOrEmpty(name) || name.Trim() == string.Empty)
                        continue;

                    int multiname1 = name.IndexOf(" ");
                    int multiname2 = name.IndexOf("-");

                    if (multiname1 != -1 || multiname2 != -1)
                    {
                        try
                        {
                            string[] names = name.Split(' ', '-');
                            name = names[names.Length - 1];
                            // name = names[0];
                            if (string.IsNullOrEmpty(name) || name.Trim() == string.Empty)
                                continue;
                        }
                        catch (System.Exception ex)
                        {
                            System.Console.WriteLine(ex.Message);
                            continue;
                        }
                        
                    }
                    
                    string url = baseUrl + System.Uri.EscapeUriString(name);
	                



	                string fileName = System.IO.Path.Combine(dataDir, name + ".txt");
	                
	                if (System.IO.File.Exists(fileName))
	                {
		                string json = System.IO.File.ReadAllText(fileName, System.Text.Encoding.UTF8);
		                Genderize genderData = Newtonsoft.Json.JsonConvert.DeserializeObject<Genderize>(json);
		                UpdateKontakt(uid, genderData);
		                System.Console.WriteLine(genderData.IsMale);
		                continue;
	                } // End if (System.IO.File.Exists(fileName))
	                
                    using (System.Net.WebClient wc = new System.Net.WebClient())
                    {
                        // wc.Proxy = wp;

                        string json = wc.DownloadString(url);
	                    
                        System.IO.File.WriteAllText(fileName, json, System.Text.Encoding.UTF8);
                        Genderize genderData = Newtonsoft.Json.JsonConvert.DeserializeObject<Genderize>(json);
	                    UpdateKontakt(uid, genderData);
	                    System.Console.WriteLine(genderData.IsMale);
                        System.Threading.Thread.Sleep(2000);
                    } // End Using wc 
	                
                } // End Using dr 
	            
            } // End using dt
	        
        } // End Sub GetGenders 
	    
	    
    } // End Class MailGender
	
	
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
	    
	    
    } // End Class Genderize 
	
	
} // End Namespace 
