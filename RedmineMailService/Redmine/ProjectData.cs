
namespace RedmineMailService.ProjectData
{


    //SELECT 
    //     id 
    //    ,'public static Redmine.Net.Api.Types.IdentifiableName ' 
    //    ,REPLACE(identifier, '-', '_') 
    //    ,'= new Redmine.Net.Api.Types.IdentifiableName() { Id = ' + CAST(id AS varchar(20)) + ', Name = "' + identifier + '" }; '
    //    ,name
    //    --,description
    //    --,homepage
    //    --,is_public
    //    --,parent_id
    //    --,created_on
    //    --,updated_on
    //    ,identifier
    //    --,status
    //    --,lft
    //    --,rgt
    //    --,inherit_members
    //    --,default_version_id
    //FROM projects 

    //ORDER BY identifier 

    public sealed class Projects
    {
        public static Redmine.Net.Api.Types.IdentifiableName anpassungen_erweiterungen = new Redmine.Net.Api.Types.IdentifiableName() { Id = 142, Name = "anpassungen_erweiterungen" };
        public static Redmine.Net.Api.Types.IdentifiableName anpassungen_erweiterungen2 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 143, Name = "anpassungen_erweiterungen2" };
        public static Redmine.Net.Api.Types.IdentifiableName aperture = new Redmine.Net.Api.Types.IdentifiableName() { Id = 25, Name = "aperture" };
        public static Redmine.Net.Api.Types.IdentifiableName basic_schnittstellen = new Redmine.Net.Api.Types.IdentifiableName() { Id = 57, Name = "basic-schnittstellen" };
        public static Redmine.Net.Api.Types.IdentifiableName basic_v3 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 135, Name = "basic-v3" };
        public static Redmine.Net.Api.Types.IdentifiableName basic_v4 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 6, Name = "basic-v4" };
        public static Redmine.Net.Api.Types.IdentifiableName basisv_v35 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 136, Name = "basisv-v35" };
        public static Redmine.Net.Api.Types.IdentifiableName bkb = new Redmine.Net.Api.Types.IdentifiableName() { Id = 109, Name = "bkb" };
        public static Redmine.Net.Api.Types.IdentifiableName campus_sursee = new Redmine.Net.Api.Types.IdentifiableName() { Id = 19, Name = "campus-sursee" };
        public static Redmine.Net.Api.Types.IdentifiableName change_request_170724 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 149, Name = "change-request-170724" };
        public static Redmine.Net.Api.Types.IdentifiableName change_request_170824 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 148, Name = "change-request-170824" };
        public static Redmine.Net.Api.Types.IdentifiableName change_request_2018 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 161, Name = "change-request-2018" };
        public static Redmine.Net.Api.Types.IdentifiableName change_request_ab_sept_2017 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 154, Name = "change-request-ab-sept-2017" };
        public static Redmine.Net.Api.Types.IdentifiableName cor_crm = new Redmine.Net.Api.Types.IdentifiableName() { Id = 11, Name = "cor-crm" };
        public static Redmine.Net.Api.Types.IdentifiableName cor_demo = new Redmine.Net.Api.Types.IdentifiableName() { Id = 132, Name = "cor-demo" };
        public static Redmine.Net.Api.Types.IdentifiableName cor_intern = new Redmine.Net.Api.Types.IdentifiableName() { Id = 63, Name = "cor-intern" };
        public static Redmine.Net.Api.Types.IdentifiableName dms = new Redmine.Net.Api.Types.IdentifiableName() { Id = 28, Name = "dms" };
        public static Redmine.Net.Api.Types.IdentifiableName formulare = new Redmine.Net.Api.Types.IdentifiableName() { Id = 1, Name = "formulare" };
        public static Redmine.Net.Api.Types.IdentifiableName helevtia_basel = new Redmine.Net.Api.Types.IdentifiableName() { Id = 140, Name = "helevtia-basel" };
        public static Redmine.Net.Api.Types.IdentifiableName helvetia = new Redmine.Net.Api.Types.IdentifiableName() { Id = 155, Name = "helvetia" };
        public static Redmine.Net.Api.Types.IdentifiableName interne_prozesse = new Redmine.Net.Api.Types.IdentifiableName() { Id = 78, Name = "interne-prozesse" };
        public static Redmine.Net.Api.Types.IdentifiableName julius_baer = new Redmine.Net.Api.Types.IdentifiableName() { Id = 110, Name = "julius-baer" };
        public static Redmine.Net.Api.Types.IdentifiableName kundenevent = new Redmine.Net.Api.Types.IdentifiableName() { Id = 73, Name = "kundenevent" };
        public static Redmine.Net.Api.Types.IdentifiableName kundenprojekte = new Redmine.Net.Api.Types.IdentifiableName() { Id = 99, Name = "kundenprojekte" };
        public static Redmine.Net.Api.Types.IdentifiableName ldap_service = new Redmine.Net.Api.Types.IdentifiableName() { Id = 27, Name = "ldap-service" };
        public static Redmine.Net.Api.Types.IdentifiableName ldap_service_sursee = new Redmine.Net.Api.Types.IdentifiableName() { Id = 32, Name = "ldap-service-sursee" };
        public static Redmine.Net.Api.Types.IdentifiableName ldap_web_verwaltung = new Redmine.Net.Api.Types.IdentifiableName() { Id = 62, Name = "ldap-web-verwaltung" };
        public static Redmine.Net.Api.Types.IdentifiableName lidl = new Redmine.Net.Api.Types.IdentifiableName() { Id = 138, Name = "lidl" };
        public static Redmine.Net.Api.Types.IdentifiableName marketing = new Redmine.Net.Api.Types.IdentifiableName() { Id = 72, Name = "marketing" };
        public static Redmine.Net.Api.Types.IdentifiableName migration_v3_auf_v4 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 88, Name = "migration-v3-auf-v4" };
        public static Redmine.Net.Api.Types.IdentifiableName modul_dokumanagement = new Redmine.Net.Api.Types.IdentifiableName() { Id = 90, Name = "modul-dokumanagement" };
        public static Redmine.Net.Api.Types.IdentifiableName modul_servicedesk = new Redmine.Net.Api.Types.IdentifiableName() { Id = 89, Name = "modul-servicedesk" };
        public static Redmine.Net.Api.Types.IdentifiableName outlook_als_adress_db = new Redmine.Net.Api.Types.IdentifiableName() { Id = 79, Name = "outlook-als-adress-db" };
        public static Redmine.Net.Api.Types.IdentifiableName pendenzen = new Redmine.Net.Api.Types.IdentifiableName() { Id = 160, Name = "pendenzen" };
        public static Redmine.Net.Api.Types.IdentifiableName phonak = new Redmine.Net.Api.Types.IdentifiableName() { Id = 144, Name = "phonak" };
        public static Redmine.Net.Api.Types.IdentifiableName post = new Redmine.Net.Api.Types.IdentifiableName() { Id = 108, Name = "post" };
        public static Redmine.Net.Api.Types.IdentifiableName raiffeisen = new Redmine.Net.Api.Types.IdentifiableName() { Id = 111, Name = "raiffeisen" };
        public static Redmine.Net.Api.Types.IdentifiableName redmine_tool = new Redmine.Net.Api.Types.IdentifiableName() { Id = 64, Name = "redmine-tool" };
        public static Redmine.Net.Api.Types.IdentifiableName reports = new Redmine.Net.Api.Types.IdentifiableName() { Id = 2, Name = "reports" };
        public static Redmine.Net.Api.Types.IdentifiableName rockwell = new Redmine.Net.Api.Types.IdentifiableName() { Id = 141, Name = "rockwell" };
        public static Redmine.Net.Api.Types.IdentifiableName rsi = new Redmine.Net.Api.Types.IdentifiableName() { Id = 103, Name = "rsi" };
        public static Redmine.Net.Api.Types.IdentifiableName sauter = new Redmine.Net.Api.Types.IdentifiableName() { Id = 100, Name = "sauter" };
        public static Redmine.Net.Api.Types.IdentifiableName schnittstellen = new Redmine.Net.Api.Types.IdentifiableName() { Id = 45, Name = "schnittstellen" };
        public static Redmine.Net.Api.Types.IdentifiableName schule_erlen = new Redmine.Net.Api.Types.IdentifiableName() { Id = 146, Name = "schule-erlen" };
        public static Redmine.Net.Api.Types.IdentifiableName snb = new Redmine.Net.Api.Types.IdentifiableName() { Id = 112, Name = "snb" };
        public static Redmine.Net.Api.Types.IdentifiableName srf = new Redmine.Net.Api.Types.IdentifiableName() { Id = 105, Name = "srf" };
        public static Redmine.Net.Api.Types.IdentifiableName srg = new Redmine.Net.Api.Types.IdentifiableName() { Id = 104, Name = "srg" };
        public static Redmine.Net.Api.Types.IdentifiableName stadtzuerich = new Redmine.Net.Api.Types.IdentifiableName() { Id = 114, Name = "stadtzuerich" };
        public static Redmine.Net.Api.Types.IdentifiableName suisse_public = new Redmine.Net.Api.Types.IdentifiableName() { Id = 83, Name = "suisse-public" };
        public static Redmine.Net.Api.Types.IdentifiableName swisscom = new Redmine.Net.Api.Types.IdentifiableName() { Id = 106, Name = "swisscom" };
        public static Redmine.Net.Api.Types.IdentifiableName swisslife = new Redmine.Net.Api.Types.IdentifiableName() { Id = 113, Name = "swisslife" };
        public static Redmine.Net.Api.Types.IdentifiableName swissre = new Redmine.Net.Api.Types.IdentifiableName() { Id = 101, Name = "swissre" };
        public static Redmine.Net.Api.Types.IdentifiableName testprojekt = new Redmine.Net.Api.Types.IdentifiableName() { Id = 137, Name = "testprojekt" };
        public static Redmine.Net.Api.Types.IdentifiableName untertest = new Redmine.Net.Api.Types.IdentifiableName() { Id = 93, Name = "untertest" };
        public static Redmine.Net.Api.Types.IdentifiableName update_2017_05_31 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 145, Name = "update-2017-05-31" };
        public static Redmine.Net.Api.Types.IdentifiableName update_2017_09_22 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 147, Name = "update-2017-09-22" };
        public static Redmine.Net.Api.Types.IdentifiableName update_2017_09_27 = new Redmine.Net.Api.Types.IdentifiableName() { Id = 153, Name = "update-2017-09-27" };
        public static Redmine.Net.Api.Types.IdentifiableName verkauf = new Redmine.Net.Api.Types.IdentifiableName() { Id = 139, Name = "verkauf" };
        public static Redmine.Net.Api.Types.IdentifiableName verkaufsfoerdermittel = new Redmine.Net.Api.Types.IdentifiableName() { Id = 81, Name = "verkaufsfoerdermittel" };
        public static Redmine.Net.Api.Types.IdentifiableName vws_portal = new Redmine.Net.Api.Types.IdentifiableName() { Id = 26, Name = "vws-portal" };
        public static Redmine.Net.Api.Types.IdentifiableName webseite_seo = new Redmine.Net.Api.Types.IdentifiableName() { Id = 74, Name = "webseite-seo" };
        public static Redmine.Net.Api.Types.IdentifiableName wincasa = new Redmine.Net.Api.Types.IdentifiableName() { Id = 107, Name = "wincasa" };
    }



    //SELECT 
    //    'public static Redmine.Net.Api.Types.IdentifiableName '
    //    --,name
    //    ,SUBSTRING(REPLACE(name, ' ', '_'), 3, 10000) 
    //    ,'= new Redmine.Net.Api.Types.IdentifiableName() { Id ='
    //    ,id
    //    ,'} ;'
    //    --,is_default
    //    --,type
    //    --------,project_id
    //    --,parent_id
    //    --,position_name
    //FROM enumerations
    //WHERE type = 'IssuePriority' 
    //AND active = 1 
    //ORDER BY position 
    public sealed class Priorities
    {
        public static Redmine.Net.Api.Types.IdentifiableName sehr_hoch = new Redmine.Net.Api.Types.IdentifiableName() { Id = 1 };
        public static Redmine.Net.Api.Types.IdentifiableName hoch = new Redmine.Net.Api.Types.IdentifiableName() { Id = 2 };
        public static Redmine.Net.Api.Types.IdentifiableName mittel = new Redmine.Net.Api.Types.IdentifiableName() { Id = 3 };
        public static Redmine.Net.Api.Types.IdentifiableName niedrig = new Redmine.Net.Api.Types.IdentifiableName() { Id = 4 };
    } // End Class Priorities 




    //SELECT 
    //    'public static Redmine.Net.Api.Types.IdentifiableName '
    //    --,REPLACE(name, ' ', '_') AS name 
    //    ,REPLACE(REPLACE(REPLACE(name, ' ', '_'), '(', ''), ')','') AS name 
    //    ,'= new Redmine.Net.Api.Types.IdentifiableName() { Id ='
    //    ,id
    //    ,'} ;'
    //FROM issue_statuses
    //WHERE (1=1) 
    //ORDER BY position 
    public sealed class IssueStatus
    {
        public static Redmine.Net.Api.Types.IdentifiableName Neu = new Redmine.Net.Api.Types.IdentifiableName() { Id = 1 };
        public static Redmine.Net.Api.Types.IdentifiableName In_Bearbeitung = new Redmine.Net.Api.Types.IdentifiableName() { Id = 2 };
        public static Redmine.Net.Api.Types.IdentifiableName in_Abklärung_COR = new Redmine.Net.Api.Types.IdentifiableName() { Id = 3 };
        public static Redmine.Net.Api.Types.IdentifiableName in_Abklärung_Kunde = new Redmine.Net.Api.Types.IdentifiableName() { Id = 4 };
        public static Redmine.Net.Api.Types.IdentifiableName installiert_Test = new Redmine.Net.Api.Types.IdentifiableName() { Id = 7 };
        public static Redmine.Net.Api.Types.IdentifiableName abgenommen_Test = new Redmine.Net.Api.Types.IdentifiableName() { Id = 8 };
        public static Redmine.Net.Api.Types.IdentifiableName installiert_produktiv = new Redmine.Net.Api.Types.IdentifiableName() { Id = 9 };
        public static Redmine.Net.Api.Types.IdentifiableName fehlerhaft = new Redmine.Net.Api.Types.IdentifiableName() { Id = 10 };
        public static Redmine.Net.Api.Types.IdentifiableName abgelehnt = new Redmine.Net.Api.Types.IdentifiableName() { Id = 11 };
        public static Redmine.Net.Api.Types.IdentifiableName zurückgezogen = new Redmine.Net.Api.Types.IdentifiableName() { Id = 6 };
        public static Redmine.Net.Api.Types.IdentifiableName unbestimmt_aufgeschoben = new Redmine.Net.Api.Types.IdentifiableName() { Id = 12 };
        public static Redmine.Net.Api.Types.IdentifiableName testen = new Redmine.Net.Api.Types.IdentifiableName() { Id = 14 };
        public static Redmine.Net.Api.Types.IdentifiableName erledigt = new Redmine.Net.Api.Types.IdentifiableName() { Id = 13 };
        public static Redmine.Net.Api.Types.IdentifiableName abgeschlossen = new Redmine.Net.Api.Types.IdentifiableName() { Id = 5 };

        // Zusätzliche:
        // unbestimmt_aufgeschoben
        // testen
        // erledigt
    } // End Class IssueStatus 




    //SELECT 
    //    'public static Redmine.Net.Api.Types.IdentifiableName '
    //    ,REPLACE(name, ' ', '_') AS name 
    //    ,'= new Redmine.Net.Api.Types.IdentifiableName() { Id ='
    //    ,id
    //    ,'} ;'
    //FROM trackers
    //WHERE (1=1) 
    //ORDER BY position 
    
    public sealed class Trackers
    {
        public static Redmine.Net.Api.Types.IdentifiableName Testtracker = new Redmine.Net.Api.Types.IdentifiableName() { Id = 7 };
        public static Redmine.Net.Api.Types.IdentifiableName Kundenanfrage = new Redmine.Net.Api.Types.IdentifiableName() { Id = 3 };
        public static Redmine.Net.Api.Types.IdentifiableName Anpassung = new Redmine.Net.Api.Types.IdentifiableName() { Id = 2 };
        public static Redmine.Net.Api.Types.IdentifiableName Fehler = new Redmine.Net.Api.Types.IdentifiableName() { Id = 1 };
        public static Redmine.Net.Api.Types.IdentifiableName Bedienung = new Redmine.Net.Api.Types.IdentifiableName() { Id = 4 };
        public static Redmine.Net.Api.Types.IdentifiableName Internes = new Redmine.Net.Api.Types.IdentifiableName() { Id = 5 };
        public static Redmine.Net.Api.Types.IdentifiableName Änderungswunsch = new Redmine.Net.Api.Types.IdentifiableName() { Id = 6 };
    } // End Class Trackers 


} // End Namespace RedmineClient 
