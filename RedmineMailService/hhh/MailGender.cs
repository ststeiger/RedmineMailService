
namespace RedmineMailService
{
    /*
     
SELECT 
	 T_AP_Ref_KontaktGeschlecht.KG_UID
	,T_AP_Ref_KontaktGeschlecht.KG_Code
	,T_AP_Ref_KontaktGeschlecht.KG_Lang_DE
	,T_AP_Ref_KontaktGeschlecht.KG_Lang_FR
	,T_AP_Ref_KontaktGeschlecht.KG_Lang_IT
	,T_AP_Ref_KontaktGeschlecht.KG_Lang_EN
FROM T_AP_Ref_KontaktGeschlecht

SELECT * FROM T_AV_Ref_AdresseAnrede 


SELECT KT_Vorname FROM T_AP_Kontakte WHERE KT_KG_UID IS NULL AND KT_Vorname <> '' 
SELECT * FROM T_AV_Adressen WHERE T_AV_Adressen.ADR_AA_UID IS NULL AND xxx <> '' 


SELECT 
	 T_AV_Ref_AdresseAnrede.AA_UID	
	,T_AV_Ref_AdresseAnrede.AA_Code
	,T_AV_Ref_AdresseAnrede.AA_Lang_DE
	,T_AV_Ref_AdresseAnrede.AA_Lang_FR
	,T_AV_Ref_AdresseAnrede.AA_Lang_IT
	,T_AV_Ref_AdresseAnrede.AA_Lang_EN
	,T_AV_Adressen .*
FROM T_AV_Adressen 

LEFT JOIN T_AV_Ref_AdresseAnrede 
	ON T_AV_Ref_AdresseAnrede.AA_UID = T_AV_Adressen.ADR_AA_UID 
	AND T_AV_Ref_AdresseAnrede.AA_Status = 1  


SELECT 
	 T_AP_Kontakte.KT_UID 
	,T_AP_Kontakte.KT_Nummer 
	,T_AP_Kontakte.KT_Name 
	,T_AP_Kontakte.KT_Vorname 
	,T_AP_Kontakte._KT_Label 
	,T_AP_Kontakte.KT_eMail 
	 
	--,T_AP_Schluessel.* 
FROM T_AP_Schluessel 
INNER JOIN T_AP_Kontakte ON T_AP_Kontakte.KT_UID = T_AP_Schluessel.SL_KT_UID 
LEFT JOIN T_AV_Ref_AdresseAnrede
	ON T_AV_Ref_AdresseAnrede.AA_UID = 
		CASE 
			WHEN KT_KG_UID = 'E32B51CF-3F85-4257-BB9C-09546D27DD7B' THEN 'BC4A3A37-90CA-4F3F-8C6D-04F1BA6B3DF6'
			WHEN KT_KG_UID = '5BF5A3AF-EE1F-4FE7-8212-D9ED655E86ED' THEN '7EAFA9B7-F8E4-4933-AB9B-50C838058B56'
			ELSE NULL
		END 


-- Filter.MailingList.FI_ErrorStatus
-- DECLARE @BE_Hash varchar(40); SET @BE_Hash = '200CEB26807D6BF99FD6F4F0D1CA54D4'; 



SELECT 
	 RTR_UID AS V 
	,CASE T_Benutzer.BE_Language 
		WHEN 'FR' THEN T_RPT_Translations.RTR_Lang_FR 
		WHEN 'IT' THEN T_RPT_Translations.RTR_Lang_IT 
		WHEN 'EN' THEN T_RPT_Translations.RTR_Lang_EN 
		ELSE T_RPT_Translations.RTR_Lang_DE 
	END AS T 

	,CAST('false' as bit) AS S  
	,0 AS Sort 
FROM T_RPT_Translations 

INNER JOIN T_Benutzer ON T_Benutzer.BE_Hash = @BE_Hash 

WHERE RTR_ReportName = 'All' 
AND RTR_ItemCaption = 'All' 


UNION ALL 


SELECT 
	 EML_DispatchSetUID AS V
	,CONVERT(varchar(10), MIN(EML_SendStart), 104) AS T
	,CAST('false' as bit) AS S  
	,1 AS Sort   
FROM T_EML_Delivery 
WHERE (1=1) 
-- AND EML_DispatchSetUID	= 'A9D7D20F-9B47-4FAF-AB5A-C17FB987E05B' 
AND EML_Module = 'Schlüsselbestandeskontrolle SwissLife' 
-- AND EML_SendSuccess IS NOT NULL 
-- AND EML_SendSuccess IS NULL 

GROUP BY 
	EML_DispatchSetUID 

ORDER BY 
	 Sort 
	,T DESC 
	 

--------------


	 
DECLARE @withDelete bit 
-- SET @withDelete = 'false' 
SET @withDelete = 'true' 


IF @withDelete = 'true' 
BEGIN
	DELETE FROM T_FMS_ZO_Filter WHERE FI_FI_UID = '212FA9DE-BBA3-4904-A410-F2CFE42AEC1F'; 
	DELETE FROM T_FMS_Filter WHERE FI_UID = '212FA9DE-BBA3-4904-A410-F2CFE42AEC1F'; 
	DELETE FROM T_FMS_Translation WHERE FT_UID = '2C0C257A-F16D-4D8E-AD4A-25DED4527EEC'; 
END 




INSERT INTO T_FMS_Translation 
(
	 FT_UID
	,FT_Ch
	,FT_De
	,FT_En
	,FT_Fr
	,FT_It
	,FT_Ru
	,FT_Parameter
	,FT_Status
) 
SELECT 
	 '2C0C257A-F16D-4D8E-AD4A-25DED4527EEC' AS FT_UID 
	,N'-- E-Mail Status --' AS FT_Ch
	,N'-- E-Mail Status --' AS FT_De
	,N'-- E-mail status --' AS FT_En
	,N'-- Statut e-mails --' AS FT_Fr
	,N'-- Status e-mail --' AS FT_It
	,N'-- Статус  Эл. почта --' AS FT_Ru
	,NULL AS FT_Parameter
	,1 AS FT_Status
WHERE 0 = ( SELECT COUNT(*) FROM T_FMS_Translation WHERE FT_UID = '2C0C257A-F16D-4D8E-AD4A-25DED4527EEC' ) 
;





INSERT INTO T_FMS_Filter
(
	 FI_UID
	,FI_FT_UID
	,FI_FT_UID_Tooltip
	,FI_FT_UID_Placeholder
	,FI_DefaultValue
	,FI_useCookie
	,FI_isReadonly
	,FI_useDatepicker
	,FI_multipleSelect
	,FI_HTML_ID
	,FI_Kategorie
	,FI_SQLFilename
	,FI_Status
) 
SELECT 
	 '212FA9DE-BBA3-4904-A410-F2CFE42AEC1F' AS FI_UID
    ,'2C0C257A-F16D-4D8E-AD4A-25DED4527EEC' AS FI_FT_UID
	,NULL AS FI_FT_UID_Tooltip
	,NULL AS FI_FT_UID_Placeholder
	,NULL AS FI_DefaultValue
	,1 AS FI_useCookie
	,0 AS FI_isReadonly
	,0 AS FI_useDatepicker
	,0 AS FI_multipleSelect
	-- ,N'selValidMail' AS FI_HTML_ID
	-- ,N'cbValidMail' AS FI_HTML_ID 
	,N'seStatus' AS FI_HTML_ID 
	,N'iKAT' AS FI_Kategorie
	,N'Filter.MailingList.FI_ErrorStatus' AS FI_SQLFilename
	,1 AS FI_Status
WHERE 0 = ( SELECT COUNT(*) FROM T_FMS_Filter WHERE FI_UID = '212FA9DE-BBA3-4904-A410-F2CFE42AEC1F' ) 
;





INSERT INTO T_FMS_ZO_Filter 
(
	 FI_UID
	,FI_FI_UID
	,FI_FT_UID
	,FI_FT_UID_Tooltip
	,FI_FT_UID_Placeholder
	,FI_NA_UID
	,FI_DefaultValue
	,FI_useCookie
	,FI_isReadonly
	,FI_multipleSelect
	,FI_SQLFilename
	,FI_LoadOnEmptyParent
	,FI_hasEmpty
	,FI_Sort
) 
SELECT 
	 '955E9CAE-D44A-4976-AA83-8DE6B85C5C46' AS FI_UID
	,'212FA9DE-BBA3-4904-A410-F2CFE42AEC1F' AS FI_FI_UID
	,NULL AS FI_FT_UID
	,NULL AS FI_FT_UID_Tooltip
	,NULL AS FI_FT_UID_Placeholder
	,'5879055A-ED0F-467E-8E72-2529FA4C0B95' AS FI_NA_UID
	,NULL AS FI_DefaultValue
	,NULL AS FI_useCookie
	,0 AS FI_isReadonly
	,0 AS FI_multipleSelect
	,NULL AS FI_SQLFilename
	-- ,0 AS FI_LoadOnEmptyParent
	,1 AS FI_LoadOnEmptyParent
	,1 AS FI_hasEmpty
	,100 AS FI_Sort 
WHERE 0 = ( SELECT COUNT(*) FROM T_FMS_ZO_Filter WHERE FI_UID = '955E9CAE-D44A-4976-AA83-8DE6B85C5C46' ) 
;



-- Filter.MailingList.FI_ErrorStatus
-- DECLARE @BE_Hash varchar(40); SET @BE_Hash = '200CEB26807D6BF99FD6F4F0D1CA54D4'; 

SELECT 
	 EML_DispatchSetUID AS V
	,MIN(EML_SendStart) AS T
	,CAST('false' as bit) AS S  
FROM T_EML_Delivery 
WHERE (1=1) 
-- AND EML_DispatchSetUID	= 'A9D7D20F-9B47-4FAF-AB5A-C17FB987E05B' 
AND EML_Module = 'Schlüsselbestandeskontrolle SwissLife' 
-- AND EML_SendSuccess IS NOT NULL 
-- AND EML_SendSuccess IS NULL 

GROUP BY
	EML_DispatchSetUID

ORDER BY 
	T DESC 

    */
	
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
            string sql = "SELECT KT_UID, KT_Vorname FROM T_AP_Kontakte WHERE KT_KG_UID IS NULL AND KT_Vorname <> '' ";
            string baseUrl = "https://api.genderize.io/?name=";
	        
	        string dataDir = System.IO.Path.GetDirectoryName(typeof(MailGender).Assembly.Location);
	        dataDir = System.IO.Path.Combine(dataDir, "..", "..", "..", "Data");
	        dataDir = System.IO.Path.GetFullPath(dataDir);
	        
            using (System.Data.DataTable dt = SQL.GetDataTable(sql))
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
	                string uid = System.Convert.ToString(dr["KT_UID"]);
	                string name = System.Convert.ToString(dr["KT_Vorname"]);
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
