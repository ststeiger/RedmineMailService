
namespace Redmine.Net.Api.Types
{
    interface IRedmineManager
    {
        int PageSize { get; set; }
        string ImpersonateUser { get; set; }

        User GetCurrentUser(System.Collections.Specialized.NameValueCollection parameters = null);

        void AddUserToGroup(int groupId, int userId);
        void RemoveUserFromGroup(int groupId, int userId);
        void AddWatcherToIssue(int issueId, int userId);
        void RemoveWatcherFromIssue(int issueId, int userId);
        WikiPage GetWikiPage(string projectId, System.Collections.Specialized.NameValueCollection parameters, string pageName, uint version = 0);
        System.Collections.Generic.IList<WikiPage> GetAllWikiPages(string projectId);
        WikiPage CreateOrUpdateWikiPage(string projectId, string pageName, WikiPage wikiPage);
        void DeleteWikiPage(string projectId, string pageName);
        Upload UploadFile(byte[] data);
        byte[] DownloadFile(string address);
        System.Collections.Generic.List<T> GetObjectList<T>(System.Collections.Specialized.NameValueCollection parameters) where T : class, new();
        System.Collections.Generic.List<T> GetObjectList<T>(System.Collections.Specialized.NameValueCollection parameters, out int totalCount) where T : class, new();
        System.Collections.Generic.List<T> GetTotalObjectList<T>(System.Collections.Specialized.NameValueCollection parameters) where T : class, new();
        System.Collections.Generic.List<T> GetObjects<T>(System.Collections.Specialized.NameValueCollection parameters) where T : class, new();
        PaginatedObjects<T> GetPaginatedObjects<T>(System.Collections.Specialized.NameValueCollection parameters) where T : class, new();
        T GetObject<T>(string id, System.Collections.Specialized.NameValueCollection parameters) where T : class, new();

        T CreateObject<T>(T obj, string ownerId = null) where T : class, new();
        void UpdateObject<T>(string id, T obj) where T : class, new();
        void UpdateObject<T>(string id, T obj, string projectId) where T : class, new();
        void DeleteObject<T>(string id, System.Collections.Specialized.NameValueCollection parameters) where T : class, new();
    }
}