
using Redmine.Net.Api.Types;


namespace Redmine.Net.Api.Async
{
    /// <summary>
    /// 
    /// </summary>
    public delegate void Task();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRes">The type of the resource.</typeparam>
    /// <returns></returns>
    public delegate TRes Task<out TRes>();

    /// <summary>
    /// 
    /// </summary>
    public static class RedmineManagerAsync
    {
        /// <summary>
        /// Gets the current user asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static Task<User> GetCurrentUserAsync(this RedmineManager redmineManager,
            System.Collections.Specialized.NameValueCollection parameters = null)
        {
            return delegate { return redmineManager.GetCurrentUser(parameters); };
        }

        /// <summary>
        /// Creates the or update wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="wikiPage">The wiki page.</param>
        /// <returns></returns>
        public static Task<WikiPage> CreateOrUpdateWikiPageAsync(this RedmineManager redmineManager, string projectId,
            string pageName, WikiPage wikiPage)
        {
            return delegate { return redmineManager.CreateOrUpdateWikiPage(projectId, pageName, wikiPage); };
        }

        /// <summary>
        /// Deletes the wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public static Task DeleteWikiPageAsync(this RedmineManager redmineManager, string projectId, string pageName)
        {
            return delegate { redmineManager.DeleteWikiPage(projectId, pageName); };
        }

        /// <summary>
        /// Gets the wiki page asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public static Task<WikiPage> GetWikiPageAsync(this RedmineManager redmineManager
            , string projectId,
            System.Collections.Specialized.NameValueCollection parameters
            , string pageName
            , uint version = 0)
        {
            return delegate { return redmineManager.GetWikiPage(projectId, parameters, pageName, version); };
        }

        /// <summary>
        /// Gets all wiki pages asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public static Task<System.Collections.Generic.IList<WikiPage>> 
            GetAllWikiPagesAsync(this RedmineManager redmineManager,
            System.Collections.Specialized.NameValueCollection parameters
            , string projectId)
        {
            return delegate { return redmineManager.GetAllWikiPages(projectId); };
        }

        /// <summary>
        /// Adds the user to group asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static Task AddUserToGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            return delegate { redmineManager.AddUserToGroup(groupId, userId); };
        }

        /// <summary>
        /// Removes the user from group asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static Task RemoveUserFromGroupAsync(this RedmineManager redmineManager, int groupId, int userId)
        {
            return delegate { redmineManager.RemoveUserFromGroup(groupId, userId); };
        }

        /// <summary>
        /// Adds the watcher to issue asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static Task AddWatcherToIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            return delegate { redmineManager.AddWatcherToIssue(issueId, userId); };
        }

        /// <summary>
        /// Removes the watcher from issue asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static Task RemoveWatcherFromIssueAsync(this RedmineManager redmineManager, int issueId, int userId)
        {
            return delegate { redmineManager.RemoveWatcherFromIssue(issueId, userId); };
        }

        /// <summary>
        /// Gets the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static Task<T> GetObjectAsync<T>(this RedmineManager redmineManager
            , string id
            , System.Collections.Specialized.NameValueCollection parameters) where T : class, new()
        {
            return delegate { return redmineManager.GetObject<T>(id, parameters); };
        }

        /// <summary>
        /// Creates the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj) where T : class, new()
        {
            return CreateObjectAsync(redmineManager, obj, null);
        }

        /// <summary>
        /// Creates the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="obj">The object.</param>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        public static Task<T> CreateObjectAsync<T>(this RedmineManager redmineManager, T obj, string ownerId)
            where T : class, new()
        {
            return delegate { return redmineManager.CreateObject(obj, ownerId); };
        }

        /// <summary>
        /// Gets the paginated objects asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static Task<PaginatedObjects<T>> GetPaginatedObjectsAsync<T>(this RedmineManager redmineManager,
            System.Collections.Specialized.NameValueCollection parameters) 
            where T : class, new()
        {
            return delegate { return redmineManager.GetPaginatedObjects<T>(parameters); };
        }

        /// <summary>
        /// Gets the objects asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static Task<System.Collections.Generic.List<T>> 
            GetObjectsAsync<T>(this RedmineManager redmineManager,
            System.Collections.Specialized.NameValueCollection parameters) 
            where T : class, new()
        {
            return delegate { return redmineManager.GetObjects<T>(parameters); };
        }

        /// <summary>
        /// Updates the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="obj">The object.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public static Task UpdateObjectAsync<T>(this RedmineManager redmineManager, string id, T obj,
            string projectId = null) where T : class, new()
        {
            return delegate { redmineManager.UpdateObject(id, obj, projectId); };
        }

        /// <summary>
        /// Deletes the object asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static Task DeleteObjectAsync<T>(this RedmineManager redmineManager, string id,
            System.Collections.Specialized.NameValueCollection parameters) 
            where T : class, new()
        {
            return delegate { redmineManager.DeleteObject<T>(id); };
        }

        /// <summary>
        /// Uploads the file asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Task<Upload> UploadFileAsync(this RedmineManager redmineManager, byte[] data, string fileName)
        {
            return delegate { return redmineManager.UploadFile(data, fileName); };
        }

        /// <summary>
        /// Downloads the file asynchronous.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public static Task<byte[]> DownloadFileAsync(this RedmineManager redmineManager, string address)
        {
            return delegate { return redmineManager.DownloadFile(address); };
        }
    }
}