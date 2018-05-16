/*
   Copyright 2011 - 2016 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Logging;
using Redmine.Net.Api.Types;


namespace Redmine.Net.Api
{
    /// <summary>
    ///     The main class to access Redmine API.
    /// </summary>
    public class RedmineManager
    {
        /// <summary>
        /// </summary>
        public const int DEFAULT_PAGE_SIZE_VALUE = 25;

        private static readonly System.Collections.Generic.Dictionary<System.Type, string> routes = 
            new System.Collections.Generic.Dictionary<System.Type, string>
        {
            {typeof(Issue), "issues"},
            {typeof(Project), "projects"},
            {typeof(User), "users"},
            {typeof(News), "news"},
            {typeof(Query), "queries"},
            {typeof(Version), "versions"},
            {typeof(Attachment), "attachments"},
            {typeof(IssueRelation), "relations"},
            {typeof(TimeEntry), "time_entries"},
            {typeof(IssueStatus), "issue_statuses"},
            {typeof(Tracker), "trackers"},
            {typeof(IssueCategory), "issue_categories"},
            {typeof(Role), "roles"},
            {typeof(ProjectMembership), "memberships"},
            {typeof(Group), "groups"},
            {typeof(TimeEntryActivity), "enumerations/time_entry_activities"},
            {typeof(IssuePriority), "enumerations/issue_priorities"},
            {typeof(Watcher), "watchers"},
            {typeof(IssueCustomField), "custom_fields"},
            {typeof(CustomField), "custom_fields"}
        };

        private readonly string basicAuthorization;
        private readonly System.Net.CredentialCache cache;
        private string host;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineManager" /> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        /// <param name="proxy">The proxy.</param>
        /// <param name="securityProtocolType">Use this parameter to specify a SecurityProtcolType. Note: it is recommended to leave this parameter at its default value as this setting also affects the calling application process.</param>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">
        ///     Host is not defined!
        ///     or
        ///     The host is not valid!
        /// </exception>
        public RedmineManager(string host, MimeFormat mimeFormat = MimeFormat.Xml
            , bool verifyServerCert = true
            , System.Net.IWebProxy proxy = null
            , System.Net.SecurityProtocolType securityProtocolType = default(System.Net.SecurityProtocolType))
        {
            if (string.IsNullOrEmpty(host)) throw new RedmineException("Host is not defined!");
            PageSize = 25;

            if (default(System.Net.SecurityProtocolType) == securityProtocolType)
            {
                securityProtocolType = System.Net.ServicePointManager.SecurityProtocol;
            }

            Host = host;
            MimeFormat = mimeFormat;
            Proxy = proxy;
            SecurityProtocolType = securityProtocolType;

            System.Net.ServicePointManager.SecurityProtocol = securityProtocolType;
            if (!verifyServerCert)
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback += RemoteCertValidate;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineManager" /> class.
        ///     Most of the time, the API requires authentication. To enable the API-style authentication, you have to check Enable
        ///     REST API in Administration -&gt; Settings -&gt; Authentication. Then, authentication can be done in 2 different
        ///     ways:
        ///     using your regular login/password via HTTP Basic authentication.
        ///     using your API key which is a handy way to avoid putting a password in a script. The API key may be attached to
        ///     each request in one of the following way:
        ///     passed in as a "key" parameter
        ///     passed in as a username with a random password via HTTP Basic authentication
        ///     passed in as a "X-Redmine-API-Key" HTTP header (added in Redmine 1.1.0)
        ///     You can find your API key on your account page ( /my/account ) when logged in, on the right-hand pane of the
        ///     default layout.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        /// <param name="proxy">The proxy.</param>
        /// <param name="securityProtocolType">Use this parameter to specify a SecurityProtcolType. Note: it is recommended to leave this parameter at its default value as this setting also affects the calling application process.</param>
        public RedmineManager(string host
            , string apiKey
            , MimeFormat mimeFormat = MimeFormat.Xml
            , bool verifyServerCert = true
            , System.Net.IWebProxy proxy = null
            , System.Net.SecurityProtocolType securityProtocolType = default(System.Net.SecurityProtocolType))
            : this(host, mimeFormat, verifyServerCert, proxy, securityProtocolType)
        {
            ApiKey = apiKey;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineManager" /> class.
        ///     Most of the time, the API requires authentication. To enable the API-style authentication, you have to check Enable
        ///     REST API in Administration -&gt; Settings -&gt; Authentication. Then, authentication can be done in 2 different
        ///     ways:
        ///     using your regular login/password via HTTP Basic authentication.
        ///     using your API key which is a handy way to avoid putting a password in a script. The API key may be attached to
        ///     each request in one of the following way:
        ///     passed in as a "key" parameter
        ///     passed in as a username with a random password via HTTP Basic authentication
        ///     passed in as a "X-Redmine-API-Key" HTTP header (added in Redmine 1.1.0)
        ///     You can find your API key on your account page ( /my/account ) when logged in, on the right-hand pane of the
        ///     default layout.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="login">The login.</param>
        /// <param name="password">The password.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <param name="verifyServerCert">if set to <c>true</c> [verify server cert].</param>
        /// <param name="proxy">The proxy.</param>
        /// <param name="securityProtocolType">Use this parameter to specify a SecurityProtcolType. Note: it is recommended to leave this parameter at its default value as this setting also affects the calling application process.</param>
        public RedmineManager(string host, string login, string password, MimeFormat mimeFormat = MimeFormat.Xml,
            bool verifyServerCert = true
            , System.Net.IWebProxy proxy = null
            , System.Net.SecurityProtocolType securityProtocolType = default(System.Net.SecurityProtocolType))
            : this(host, mimeFormat, verifyServerCert, proxy, securityProtocolType)
        {
            cache = new System.Net.CredentialCache
            {
                { new System.Uri(host), "Basic", new System.Net.NetworkCredential(login, password) }
            };

            string token = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", login, password)));
            basicAuthorization = string.Format("Basic {0}", token);
        }

        /// <summary>
        ///     Gets the sufixes.
        /// </summary>
        /// <value>
        ///     The sufixes.
        /// </value>
        public static System.Collections.Generic.Dictionary<System.Type, string> Sufixes
        {
            get { return routes; }
        }

        /// <summary>
        ///     Gets the host.
        /// </summary>
        /// <value>
        ///     The host.
        /// </value>
        public string Host
        {
            get { return host; }
            private set
            {
	            host = value;

                System.Uri uriResult;
                if (!System.Uri.TryCreate(host, System.UriKind.Absolute, out uriResult) ||
                    !(uriResult.Scheme == System.Uri.UriSchemeHttp || uriResult.Scheme == System.Uri.UriSchemeHttps))
                {
                    host = "http://" + host;
                }

                if (!System.Uri.TryCreate(host, System.UriKind.Absolute, out uriResult))
                    throw new RedmineException("The host is not valid!");
            }
        }

        /// <summary>
        ///     The ApiKey used to authenticate.
        /// </summary>
        /// <value>
        ///     The API key.
        /// </value>
        public string ApiKey { get; private set; }

        /// <summary>
        ///     Maximum page-size when retrieving complete object lists
        ///     <remarks>
        ///         By default only 25 results can be retrieved per request. Maximum is 100. To change the maximum value set
        ///         in your Settings -&gt; General, "Objects per page options".By adding (for instance) 9999 there would make you
        ///         able to get that many results per request.
        ///     </remarks>
        /// </summary>
        /// <value>
        ///     The size of the page.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        ///     As of Redmine 2.2.0 you can impersonate user setting user login (eg. jsmith). This only works when using the API
        ///     with an administrator account, this header will be ignored when using the API with a regular user account.
        /// </summary>
        /// <value>
        ///     The impersonate user.
        /// </value>
        public string ImpersonateUser { get; set; }

        /// <summary>
        ///     Gets the MIME format.
        /// </summary>
        /// <value>
        ///     The MIME format.
        /// </value>
        public MimeFormat MimeFormat { get; private set; }

        /// <summary>
        ///     Gets the proxy.
        /// </summary>
        /// <value>
        ///     The proxy.
        /// </value>
        public System.Net.IWebProxy Proxy { get; private set; }

        /// <summary>
        ///     Gets the type of the security protocol.
        /// </summary>
        /// <value>
        ///     The type of the security protocol.
        /// </value>
        public System.Net.SecurityProtocolType SecurityProtocolType { get; private set; }

        /// <summary>
        ///     Returns the user whose credentials are used to access the API.
        /// </summary>
        /// <param name="parameters">The accepted parameters are: memberships and groups (added in 2.1).</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        ///     An error occurred during deserialization. The original exception is available
        ///     using the System.Exception.InnerException property.
        /// </exception>
        public User GetCurrentUser(System.Collections.Specialized.NameValueCollection parameters = null)
        {
            string url = UrlHelper.GetCurrentUserUrl(this);
            return WebApiHelper.ExecuteDownload<User>(this, url, "GetCurrentUser", parameters);
        }

        /// <summary>
        ///     Adds the watcher to issue.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public void AddWatcherToIssue(int issueId, int userId)
        {
            string url = UrlHelper.GetAddWatcherUrl(this, issueId, userId);
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.POST, DataHelper.UserData(userId, MimeFormat), "AddWatcher");
        }

        /// <summary>
        ///     Removes the watcher from issue.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public void RemoveWatcherFromIssue(int issueId, int userId)
        {
            string url = UrlHelper.GetRemoveWatcherUrl(this, issueId, userId);
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.DELETE, string.Empty, "RemoveWatcher");
        }

        /// <summary>
        ///     Adds an existing user to a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        public void AddUserToGroup(int groupId, int userId)
        {
            string url = UrlHelper.GetAddUserToGroupUrl(this, groupId);
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.POST, DataHelper.UserData(userId, MimeFormat), "AddUser");
        }




        public IdentifiableName GetUserByLogin(string login)
        {
            System.Collections.Generic.List<User> users = this.GetObjects<User>();

            for (int i = 0; i < users.Count; ++i)
            {
                if (string.Equals(users[i].Login, login, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return new IdentifiableName()
                    { Id = users[i].Id };
                } // End if (string.Equals(users[i].Login, login, System.StringComparison.InvariantCultureIgnoreCase)) 

            } // Next i 

            return null;
        } // End Function GetByLogin 
        

        public IdentifiableName GetUserByEmail(string email)
        {
            System.Collections.Generic.List<User> users = this.GetObjects<User>();

            for (int i = 0; i < users.Count; ++i)
            {
                if (string.Equals(users[i].Email, email, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return new IdentifiableName()
                    { Id = users[i].Id };
                } // End if (string.Equals(users[i].Email, email, System.StringComparison.InvariantCultureIgnoreCase)) 

            } // Next i 

            return null;
        } // End Function GetByEmail 



        /// <summary>
        ///     Removes an user from a group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="userId">The user id.</param>
        public void RemoveUserFromGroup(int groupId, int userId)
        {
            string url = UrlHelper.GetRemoveUserFromGroupUrl(this, groupId, userId);
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.DELETE, string.Empty, "DeleteUser");
        }

        /// <summary>
        ///     Creates or updates a wiki page.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        /// <param name="wikiPage">The wiki page to create or update.</param>
        /// <returns></returns>
        public WikiPage CreateOrUpdateWikiPage(string projectId, string pageName, WikiPage wikiPage)
        {
            string result = RedmineSerializer.Serialize(wikiPage, MimeFormat);
            if (string.IsNullOrEmpty(result)) return null;

            string url = UrlHelper.GetWikiCreateOrUpdaterUrl(this, projectId, pageName);
            return WebApiHelper.ExecuteUpload<WikiPage>(this, url, HttpVerbs.PUT, result, "CreateOrUpdateWikiPage");
        }

        /// <summary>
        /// Gets the wiki page.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public WikiPage GetWikiPage(string projectId
            , System.Collections.Specialized.NameValueCollection parameters
            , string pageName
            , uint version = 0)
        {
            string url = UrlHelper.GetWikiPageUrl(this, projectId, parameters, pageName, version);
            return WebApiHelper.ExecuteDownload<WikiPage>(this, url, "GetWikiPage", parameters);
        }

        /// <summary>
        ///     Returns the list of all pages in a project wiki.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <returns></returns>
        public System.Collections.Generic.List<WikiPage> GetAllWikiPages(string projectId)
        {
            string url = UrlHelper.GetWikisUrl(this, projectId);
            PaginatedObjects<WikiPage> result = WebApiHelper.ExecuteDownloadList<WikiPage>(this, url, "GetAllWikiPages");
            return result != null ? result.Objects : null;
        }

        /// <summary>
        ///     Deletes a wiki page, its attachments and its history. If the deleted page is a parent page, its child pages are not
        ///     deleted but changed as root pages.
        /// </summary>
        /// <param name="projectId">The project id or identifier.</param>
        /// <param name="pageName">The wiki page name.</param>
        public void DeleteWikiPage(string projectId, string pageName)
        {
            string url = UrlHelper.GetDeleteWikirUrl(this, projectId, pageName);
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.DELETE, string.Empty, "DeleteWikiPage");
        }

        /// <summary>
        ///     Gets the redmine object based on id.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="id">The id of the object.</param>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>
        ///     Returns the object of type T.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        ///     An error occurred during deserialization. The original exception is available
        ///     using the System.Exception.InnerException property.
        /// </exception>
        /// <code>
        ///   <example>
        ///         string issueId = "927";
        ///         NameValueCollection parameters = null;
        ///         Issue issue = redmineManager.GetObject&lt;Issue&gt;(issueId, parameters);
        ///     </example>
        /// </code>
        public T GetObject<T>(string id, System.Collections.Specialized.NameValueCollection parameters) 
            where T : class, new()
        {
            string url = UrlHelper.GetGetUrl<T>(this, id);
            return WebApiHelper.ExecuteDownload<T>(this, url, "GetObject", parameters);
        }

        /// <summary>
        ///     Gets the paginated objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public PaginatedObjects<T> GetPaginatedObjects<T>(System.Collections.Specialized.NameValueCollection parameters) 
            where T : class, new()
        {
            string url = UrlHelper.GetListUrl<T>(this, parameters);
            return WebApiHelper.ExecuteDownloadList<T>(this, url, "GetObjectList", parameters);
        }

        /// <summary>
        ///     Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">The page size.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="include">Optional fetched data.</param>
        /// <remarks>
        /// Optional fetched data:
        ///     Project: trackers, issue_categories, enabled_modules (since 2.6.0)
        ///     Issue: children, attachments, relations, changesets, journals, watchers - Since 2.3.0
        ///     Users: memberships, groups (added in 2.1)
        ///     Groups: users, memberships
        /// </remarks>
        /// <returns>Returns the complete list of objects.</returns>
        public System.Collections.Generic.List<T> GetObjects<T>(int limit, int offset, params string[] include) 
            where T : class, new()
        {
            System.Collections.Specialized.NameValueCollection parameters = 
                new System.Collections.Specialized.NameValueCollection();

            parameters.Add(RedmineKeys.LIMIT, limit.ToString(System.Globalization.CultureInfo.InvariantCulture));
            parameters.Add(RedmineKeys.OFFSET, offset.ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (include != null)
            {
                parameters.Add(RedmineKeys.INCLUDE, string.Join(",", include));
            }

            return GetObjects<T>(parameters);
        }

        /// <summary>
        ///     Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="include">Optional fetched data.</param>
        /// <remarks>
        /// Optional fetched data:
        ///     Project: trackers, issue_categories, enabled_modules (since 2.6.0)
        ///     Issue: children, attachments, relations, changesets, journals, watchers - Since 2.3.0
        ///     Users: memberships, groups (added in 2.1)
        ///     Groups: users, memberships
        /// </remarks>
        /// <returns>Returns the complete list of objects.</returns>
        public System.Collections.Generic.List<T> GetObjects<T>(params string[] include) where T : class, new()
        {
            return GetObjects<T>(PageSize, 0, include);
        }

        /// <summary>
        ///     Returns the complete list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="parameters">Optional filters and/or optional fetched data.</param>
        /// <returns>
        ///     Returns a complete list of objects.
        /// </returns>
        public System.Collections.Generic.List<T> GetObjects<T>(System.Collections.Specialized.NameValueCollection parameters) 
            where T : class, new()
        {
            int totalCount = 0, pageSize = 0, offset = 0;
            bool isLimitSet = false;
            System.Collections.Generic.List<T> resultList = null;

            if (parameters == null)
            {
                parameters = new System.Collections.Specialized.NameValueCollection();
            }
            else
            {
                isLimitSet = int.TryParse(parameters[RedmineKeys.LIMIT], out pageSize);
                int.TryParse(parameters[RedmineKeys.OFFSET], out offset);
            }
            if (pageSize == default(int))
            {
                pageSize = PageSize > 0 ? PageSize : DEFAULT_PAGE_SIZE_VALUE;
                parameters.Set(RedmineKeys.LIMIT, pageSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            try
            {
                do
                {
                    parameters.Set(RedmineKeys.OFFSET, offset.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    PaginatedObjects<T> tempResult = GetPaginatedObjects<T>(parameters);
                    if (tempResult != null)
                    {
                        if (resultList == null)
                        {
                            resultList = tempResult.Objects;
                            totalCount = isLimitSet ? pageSize : tempResult.TotalCount;
                        }
                        else
                        {
                            resultList.AddRange(tempResult.Objects);
                        }
                    }
                    offset += pageSize;
                } while (offset < totalCount);
            }
            catch (System.Net.WebException wex)
            {
                wex.HandleWebException("GetObjectsAsync", MimeFormat);
            }
            return resultList;
        }

        /// <summary>
        ///     Creates a new Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="obj">The object to create.</param>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        /// <remarks>
        ///     When trying to create an object with invalid or missing attribute parameters, you will get a 422 Unprocessable
        ///     Entity response. That means that the object could not be created.
        /// </remarks>
        /// <code>
        ///   <example>
        ///         Project project = new Project();
        ///         project.Name = "test";
        ///         project.Identifier = "the project identifier";
        ///         project.Description = "the project description";
        ///         redmineManager.CreateObject(project);
        ///     </example>
        /// </code>
        public T CreateObject<T>(T obj, string ownerId) where T : class, new()
        {
            string url = UrlHelper.GetCreateUrl<T>(this, ownerId);
            string data = RedmineSerializer.Serialize(obj, MimeFormat);
            return WebApiHelper.ExecuteUpload<T>(this, url, HttpVerbs.POST, data, "CreateObject");
        }

        /// <summary>
        ///     Updates a Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to be update.</typeparam>
        /// <param name="id">The id of the object to be update.</param>
        /// <param name="obj">The object to be update.</param>
        /// <exception cref="RedmineException"></exception>
        /// <remarks>
        ///     When trying to update an object with invalid or missing attribute parameters, you will get a 422(RedmineException)
        ///     Unprocessable Entity response. That means that the object could not be updated.
        /// </remarks>
        /// <code></code>
        public void UpdateObject<T>(string id, T obj) where T : class, new()
        {
            UpdateObject(id, obj, null);
        }

        /// <summary>
        ///     Updates a Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to be update.</typeparam>
        /// <param name="id">The id of the object to be update.</param>
        /// <param name="obj">The object to be update.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <exception cref="RedmineException"></exception>
        /// <remarks>
        ///     When trying to update an object with invalid or missing attribute parameters, you will get a
        ///     422(RedmineException) Unprocessable Entity response. That means that the object could not be updated.
        /// </remarks>
        /// <code></code>
        public void UpdateObject<T>(string id, T obj, string projectId) where T : class, new()
        {
            string url = UrlHelper.GetUploadUrl(this, id, obj, projectId);
            string data = RedmineSerializer.Serialize(obj, MimeFormat);
            data = System.Text.RegularExpressions.Regex.Replace(data, @"\r\n|\r|\n", "\r\n");
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.PUT, data, "UpdateObject");
        }

        /// <summary>
        ///     Deletes the Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of objects to delete.</typeparam>
        /// <param name="id">The id of the object to delete</param>
        /// <exception cref="RedmineException"></exception>
        /// <code></code>
        public void DeleteObject<T>(string id) where T : class, new()
        {
            DeleteObject<T>(id, null);
        }

	    /// <summary>
	    /// Deletes the Redmine object.
	    /// </summary>
	    /// <typeparam name="T">The type of objects to delete.</typeparam>
	    /// <param name="id">The id of the object to delete</param>
	    /// <param name="parameters">The parameters</param>
	    /// <exception cref="RedmineException"></exception>
	    /// <code></code>
	    public void DeleteObject<T>(string id
            , System.Collections.Specialized.NameValueCollection parameters = null) 
            where T : class, new()
        {
            string url = UrlHelper.GetDeleteUrl<T>(this, id);
            WebApiHelper.ExecuteUpload(this, url, HttpVerbs.DELETE, string.Empty, "DeleteObject", parameters);
        }

        /// <summary>
        ///     Support for adding attachments through the REST API is added in Redmine 1.4.0.
        ///     Upload a file to server.
        /// </summary>
        /// <param name="data">The content of the file that will be uploaded on server.</param>
        /// <returns>
        ///     Returns the token for uploaded file.
        /// </returns>
        /// <exception cref="RedmineException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="ConflictException"></exception>
        /// <exception cref="NotAcceptableException"></exception>
        public Upload UploadFile(byte[] data, string fileName)
        {
            return UploadFile(data, fileName, null, null);
        }


        private static System.Collections.Generic.Dictionary<string, string> CreateMimeMap()
        {
            // SELECT 'mm.Add("' + MIME_FileExtension + '", "' + MIME_Type + '");' FROM T_SYS_Ref_MimeTypes 

            System.Collections.Generic.Dictionary<string, string> mm = 
                new System.Collections.Generic.Dictionary<string, string>(
                    System.StringComparer.InvariantCultureIgnoreCase
                    );

            mm.Add(".tex", "application/x-tex");
            mm.Add(".g2w", "application/vnd.geoplan");
            mm.Add(".cml", "chemical/x-cml");
            mm.Add(".wsdl", "application/wsdl+xml");
            mm.Add(".ecelp9600", "audio/vnd.nuera.ecelp9600");
            mm.Add(".c11amc", "application/vnd.cluetrust.cartomobile-config");
            mm.Add(".ice", "x-conference/x-cooltalk");
            mm.Add(".ots", "application/vnd.oasis.opendocument.spreadsheet-template");
            mm.Add(".efif", "application/vnd.picsel");
            mm.Add(".p7r", "application/x-pkcs7-certreqresp");
            mm.Add(".p7s", "application/pkcs7-signature");
            mm.Add(".xspf", "application/xspf+xml");
            mm.Add(".uu", "text/x-uuencode");
            mm.Add(".odf", "application/vnd.oasis.opendocument.formula");
            mm.Add(".mp4", "video/mp4");
            mm.Add(".odm", "application/vnd.oasis.opendocument.text-master");
            mm.Add(".sdp", "application/sdp");
            mm.Add(".ppsm", "application/vnd.ms-powerpoint.slideshow.macroenabled.12");
            mm.Add(".mpm", "application/vnd.blueice.multipass");
            mm.Add(".f", "text/x-fortran");
            mm.Add(".azw", "application/vnd.amazon.ebook");
            mm.Add(".atc", "application/vnd.acucorp");
            mm.Add(".vtu", "model/vnd.vtu");
            mm.Add(".bdm", "application/vnd.syncml.dm+wbxml");
            mm.Add(".wri", "application/x-mswrite");
            mm.Add(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            mm.Add(".pcx", "image/x-pcx");
            mm.Add(".nnd", "application/vnd.noblenet-directory");
            mm.Add(".cdkey", "application/vnd.mediastation.cdkey");
            mm.Add(".meta4", "application/metalink4+xml");
            mm.Add(".class", "application/java-vm");
            mm.Add(".cif", "chemical/x-cif");
            mm.Add(".yang", "application/yang");
            mm.Add(".chat", "application/x-chat");
            mm.Add(".lasxml", "application/vnd.las.las+xml");
            mm.Add(".hdf", "application/x-hdf");
            mm.Add(".h261", "video/h261");
            mm.Add(".sis", "application/vnd.symbian.install");
            mm.Add(".ftc", "application/vnd.fluxtime.clip");
            mm.Add(".yin", "application/yin+xml");
            mm.Add(".setreg", "application/set-registration-initiation");
            mm.Add(".wtb", "application/vnd.webturbo");
            mm.Add(".fzs", "application/vnd.fuzzysheet");
            mm.Add(".uvi", "image/vnd.dece.graphic");
            mm.Add(".sdw", "application/vnd.stardivision.writer");
            mm.Add(".chm", "application/vnd.ms-htmlhelp");
            mm.Add(".nbp", "application/vnd.wolfram.player");
            mm.Add(".cdbcmsg", "application/vnd.contact.cmsg");
            mm.Add(".ktx", "image/ktx");
            mm.Add(".xdm", "application/vnd.syncml.dm+xml");
            mm.Add(".apk", "application/vnd.android.package-archive");
            mm.Add(".potm", "application/vnd.ms-powerpoint.template.macroenabled.12");
            mm.Add(".nml", "application/vnd.enliven");
            mm.Add(".eot", "application/vnd.ms-fontobject");
            mm.Add(".atomcat", "application/atomcat+xml");
            mm.Add(".nlu", "application/vnd.neurolanguage.nlu");
            mm.Add(".psd", "image/vnd.adobe.photoshop");
            mm.Add(".xltm", "application/vnd.ms-excel.template.macroenabled.12");
            mm.Add(".sbml", "application/sbml+xml");
            mm.Add(".wrl", "model/vrml");
            mm.Add(".twd", "application/vnd.simtech-mindmapper");
            mm.Add(".mc1", "application/vnd.medcalcdata");
            mm.Add(".m4v", "video/x-m4v");
            mm.Add(".uvv", "video/vnd.dece.video");
            mm.Add(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            mm.Add(".n-gage", "application/vnd.nokia.n-gage.symbian.install");
            mm.Add(".pgpe", "application/pgp-encrypted");
            mm.Add(".hbci", "application/vnd.hbci");
            mm.Add(".kfo", "application/vnd.kde.kformula");
            mm.Add(".html", "text/html");
            mm.Add(".eml", "message/rfc822");
            mm.Add(".dts", "audio/vnd.dts");
            mm.Add(".qam", "application/vnd.epson.quickanime");
            mm.Add(".otp", "application/vnd.oasis.opendocument.presentation-template");
            mm.Add(".x3d", "application/vnd.hzn-3d-crossword");
            mm.Add(".ipk", "application/vnd.shana.informed.package");
            mm.Add(".opf", "application/oebps-package+xml");
            mm.Add(".vcs", "text/x-vcalendar");
            mm.Add(".kpr", "application/vnd.kde.kpresenter");
            mm.Add(".xslt", "application/xslt+xml");
            mm.Add(".igm", "application/vnd.insors.igm");
            mm.Add(".g3w", "application/vnd.geospace");
            mm.Add(".kmz", "application/vnd.google-earth.kmz");
            mm.Add(".aif", "audio/x-aiff");
            mm.Add(".shar", "application/x-shar");
            mm.Add(".png", "image/png");
            mm.Add(".trm", "application/x-msterminal");
            mm.Add(".deb", "application/x-debian-package");
            mm.Add(".gex", "application/vnd.geometry-explorer");
            mm.Add(".wm", "video/x-ms-wm");
            mm.Add(".xdf", "application/xcap-diff+xml");
            mm.Add(".ahead", "application/vnd.ahead.space");
            mm.Add(".sfs", "application/vnd.spotfire.sfs");
            mm.Add(".htm", "text/html");
            mm.Add(".ext", "application/vnd.novadigm.ext");
            mm.Add(".rlc", "image/vnd.fujixerox.edmics-rlc");
            mm.Add(".cmp", "application/vnd.yellowriver-custom-menu");
            mm.Add(".xul", "application/vnd.mozilla.xul+xml");
            mm.Add(".bz", "application/x-bzip");
            mm.Add(".sgl", "application/vnd.stardivision.writer-global");
            mm.Add(".esf", "application/vnd.epson.esf");
            mm.Add(".uvm", "video/vnd.dece.mobile");
            mm.Add(".svd", "application/vnd.svd");
            mm.Add(".clkt", "application/vnd.crick.clicker.template");
            mm.Add(".xfdl", "application/vnd.xfdl");
            mm.Add(".nsf", "application/vnd.lotus-notes");
            mm.Add(".aas", "application/x-authorware-seg");
            mm.Add(".pbd", "application/vnd.powerbuilder6");
            mm.Add(".shf", "application/shf+xml");
            mm.Add(".tei", "application/tei+xml");
            mm.Add(".rtf", "application/rtf");
            mm.Add(".karbon", "application/vnd.kde.karbon");
            mm.Add(".stc", "application/vnd.sun.xml.calc.template");
            mm.Add(".fig", "application/x-xfig");
            mm.Add(".es3", "application/vnd.eszigno3+xml");
            mm.Add(".jisp", "application/vnd.jisp");
            mm.Add(".ics", "text/calendar");
            mm.Add(".wg", "application/vnd.pmi.widget");
            mm.Add(".doc", "application/msword");
            mm.Add(".xo", "application/vnd.olpc-sugar");
            mm.Add(".cgm", "image/cgm");
            mm.Add(".sema", "application/vnd.sema");
            mm.Add(".3gp", "video/3gpp");
            mm.Add(".les", "application/vnd.hhe.lesson-player");
            mm.Add(".umj", "application/vnd.umajin");
            mm.Add(".dtb", "application/x-dtbook+xml");
            mm.Add(".odt", "application/vnd.oasis.opendocument.text");
            mm.Add(".lwp", "application/vnd.lotus-wordpro");
            mm.Add(".sxi", "application/vnd.sun.xml.impress");
            mm.Add(".mrcx", "application/marcxml+xml");
            mm.Add(".rcprofile", "application/vnd.ipunplugged.rcprofile");
            mm.Add(".srx", "application/sparql-results+xml");
            mm.Add(".oti", "application/vnd.oasis.opendocument.image-template");
            mm.Add(".application", "application/x-ms-application");
            mm.Add(".mpkg", "application/vnd.apple.installer+xml");
            mm.Add(".osfpvg", "application/vnd.yamaha.openscoreformat.osfpvg+xml");
            mm.Add(".mpc", "application/vnd.mophun.certificate");
            mm.Add(".c4g", "application/vnd.clonk.c4group");
            mm.Add(".acu", "application/vnd.acucobol");
            mm.Add(".ras", "image/x-cmu-raster");
            mm.Add(".wmx", "video/x-ms-wmx");
            mm.Add(".oxt", "application/vnd.openofficeorg.extension");
            mm.Add(".mpeg", "video/mpeg");
            mm.Add(".rar", "application/x-rar-compressed");
            mm.Add(".hvd", "application/vnd.yamaha.hv-dic");
            mm.Add(".pnm", "image/x-portable-anymap");
            mm.Add(".mqy", "application/vnd.mobius.mqy");
            mm.Add(".mpp", "application/vnd.ms-project");
            mm.Add(".iif", "application/vnd.shana.informed.interchange");
            mm.Add(".btif", "image/prs.btif");
            mm.Add(".ddd", "application/vnd.fujixerox.ddd");
            mm.Add(".odi", "application/vnd.oasis.opendocument.image");
            mm.Add(".webp", "image/webp");
            mm.Add(".rm", "application/vnd.rn-realmedia");
            mm.Add(".clkp", "application/vnd.crick.clicker.palette");
            mm.Add(".xlsm", "application/vnd.ms-excel.sheet.macroenabled.12");
            mm.Add(".cdy", "application/vnd.cinderella");
            mm.Add(".rnc", "application/relax-ng-compact-syntax");
            mm.Add(".npx", "image/vnd.net-fpx");
            mm.Add(".vcd", "application/x-cdlink");
            mm.Add(".3g2", "video/3gpp2");
            mm.Add(".tif", "image/tiff");
            mm.Add(".xlsb", "application/vnd.ms-excel.sheet.binary.macroenabled.12");
            mm.Add(".unityweb", "application/vnd.unity");
            mm.Add(".wpd", "application/vnd.wordperfect");
            mm.Add(".psb", "application/vnd.3gpp.pic-bw-small");
            mm.Add(".bdf", "application/x-font-bdf");
            mm.Add(".m3u8", "application/vnd.apple.mpegurl");
            mm.Add(".ufd", "application/vnd.ufdl");
            mm.Add(".zaz", "application/vnd.zzazz.deck+xml");
            mm.Add(".exe", "application/x-msdownload");
            mm.Add(".epub", "application/epub+zip");
            mm.Add(".skp", "application/vnd.koan");
            mm.Add(".qfx", "application/vnd.intu.qfx");
            mm.Add(".csp", "application/vnd.commonspace");
            mm.Add(".gqf", "application/vnd.grafeq");
            mm.Add(".csv", "text/csv");
            mm.Add(".sfd-hdstx", "application/vnd.hydrostatix.sof-data");
            mm.Add(".scurl", "text/vnd.curl.scurl");
            mm.Add(".txt", "text/plain");
            mm.Add(".xfdf", "application/vnd.adobe.xfdf");
            mm.Add(".tpl", "application/vnd.groove-tool-template");
            mm.Add(".stk", "application/hyperstudio");
            mm.Add(".par", "text/plain-bas");
            mm.Add(".mlp", "application/vnd.dolby.mlp");
            mm.Add(".wpl", "application/vnd.ms-wpl");
            mm.Add(".dwg", "image/vnd.dwg");
            mm.Add(".c11amz", "application/vnd.cluetrust.cartomobile-config-pkg");
            mm.Add(".aab", "application/x-authorware-bin");
            mm.Add(".air", "application/vnd.adobe.air-application-installer-package+zip");
            mm.Add(".apr", "application/vnd.lotus-approach");
            mm.Add("N/A", "application/andrew-inset");
            mm.Add(".mmf", "application/vnd.smaf");
            mm.Add(".org", "application/vnd.lotus-organizer");
            mm.Add(".edm", "application/vnd.novadigm.edm");
            mm.Add(".uvp", "video/vnd.dece.pd");
            mm.Add(".rl", "application/resource-lists+xml");
            mm.Add(".mmd", "application/vnd.chipnuts.karaoke-mmd");
            mm.Add(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            mm.Add(".ggb", "application/vnd.geogebra.file");
            mm.Add(".wax", "audio/x-ms-wax");
            mm.Add(".pfr", "application/font-tdpfr");
            mm.Add(".h263", "video/h263");
            mm.Add(".dxf", "image/vnd.dxf");
            mm.Add(".azf", "application/vnd.airzip.filesecure.azf");
            mm.Add(".mcd", "application/vnd.mcd");
            mm.Add(".cdx", "chemical/x-cdx");
            mm.Add(".xap", "application/x-silverlight-app");
            mm.Add(".sv4cpio", "application/x-sv4cpio");
            mm.Add(".grv", "application/vnd.groove-injector");
            mm.Add(".joda", "application/vnd.joost.joda-archive");
            mm.Add(".rdz", "application/vnd.data-vision.rdz");
            mm.Add(".thmx", "application/vnd.ms-officetheme");
            mm.Add(".wbxml", "application/vnd.wap.wbxml");
            mm.Add(".fh", "image/x-freehand");
            mm.Add(".rtx", "text/richtext");
            mm.Add(".saf", "application/vnd.yamaha.smaf-audio");
            mm.Add(".jpm", "video/jpm");
            mm.Add(".itp", "application/vnd.shana.informed.formtemplate");
            mm.Add(".gtw", "model/vnd.gtw");
            mm.Add(".gnumeric", "application/x-gnumeric");
            mm.Add(".dp", "application/vnd.osgi.dp");
            mm.Add(".pskcxml", "application/pskc+xml");
            mm.Add(".odc", "application/vnd.oasis.opendocument.chart");
            mm.Add(".msty", "application/vnd.muvee.style");
            mm.Add(".osf", "application/vnd.yamaha.openscoreformat");
            mm.Add(".ptid", "application/vnd.pvi.ptid1");
            mm.Add(".cmdf", "chemical/x-cmdf");
            mm.Add(".uoml", "application/vnd.uoml+xml");
            mm.Add(".dsc", "text/prs.lines.tag");
            mm.Add(".hvs", "application/vnd.yamaha.hv-script");
            mm.Add(".hpgl", "application/vnd.hp-hpgl");
            mm.Add(".tiff", "image/tiff");
            mm.Add(".ims", "application/vnd.ms-ims");
            mm.Add(".au", "audio/basic");
            mm.Add(".vis", "application/vnd.visionary");
            mm.Add(".gtm", "application/vnd.groove-tool-message");
            mm.Add(".jlt", "application/vnd.hp-jlyt");
            mm.Add(".kwd", "application/vnd.kde.kword");
            mm.Add(".oda", "application/oda");
            mm.Add(".mxl", "application/vnd.recordare.musicxml");
            mm.Add(".wspolicy", "application/wspolicy+xml");
            mm.Add(".odft", "application/vnd.oasis.opendocument.formula-template");
            mm.Add(".xbm", "image/x-xbitmap");
            mm.Add(".qt", "video/quicktime");
            mm.Add(".jad", "text/vnd.sun.j2me.app-descriptor");
            mm.Add(".sdkm", "application/vnd.solent.sdkm+xml");
            mm.Add(".dra", "audio/vnd.dra");
            mm.Add(".webm", "video/webm");
            mm.Add(".gac", "application/vnd.groove-account");
            mm.Add(".sxd", "application/vnd.sun.xml.draw");
            mm.Add(".pls", "application/pls+xml");
            mm.Add(".onetoc", "application/onenote");
            mm.Add(".sgml", "text/sgml");
            mm.Add(".oa3", "application/vnd.fujitsu.oasys3");
            mm.Add(".ggt", "application/vnd.geogebra.tool");
            mm.Add(".ghf", "application/vnd.groove-help");
            mm.Add(".tao", "application/vnd.tao.intent-module-archive");
            mm.Add(".es", "application/ecmascript");
            mm.Add(".pfa", "application/x-font-type1");
            mm.Add(".qbo", "application/vnd.intu.qbo");
            mm.Add(".gtar", "application/x-gtar");
            mm.Add(".rs", "application/rls-services+xml");
            mm.Add(".crd", "application/x-mscardfile");
            mm.Add(".sse", "application/vnd.kodak-descriptor");
            mm.Add(".xpm", "image/x-xpixmap");
            mm.Add(".xdssc", "application/dssc+xml");
            mm.Add(".cdxml", "application/vnd.chemdraw+xml");
            mm.Add(".rgb", "image/x-rgb");
            mm.Add(".java", "text/x-java-source,java");
            mm.Add(".g3", "image/g3fax");
            mm.Add(".gv", "text/vnd.graphviz");
            mm.Add(".jam", "application/vnd.jam");
            mm.Add(".fvt", "video/vnd.fvt");
            mm.Add(".tar", "application/x-tar");
            mm.Add(".prf", "application/pics-rules");
            mm.Add(".xsm", "application/vnd.syncml+xml");
            mm.Add(".pdb", "application/vnd.palm");
            mm.Add(".qxd", "application/vnd.quark.quarkxpress");
            mm.Add(".pml", "application/vnd.ctc-posml");
            mm.Add(".css", "text/css");
            mm.Add(".sm", "application/vnd.stepmania.stepchart");
            mm.Add(".ccxml", "application/ccxml+xml,");
            mm.Add(".tra", "application/vnd.trueapp");
            mm.Add(".xar", "application/vnd.xara");
            mm.Add(".ico", "image/x-icon");
            mm.Add(".ppd", "application/vnd.cups-ppd");
            mm.Add(".ogx", "application/ogg");
            mm.Add(".wmlc", "application/vnd.wap.wmlc");
            mm.Add(".setpay", "application/set-payment-initiation");
            mm.Add(".pgm", "image/x-portable-graymap");
            mm.Add(".uvs", "video/vnd.dece.sd");
            mm.Add(".cu", "application/cu-seeme");
            mm.Add(".cmx", "image/x-cmx");
            mm.Add(".jnlp", "application/x-java-jnlp-file");
            mm.Add(".sxw", "application/vnd.sun.xml.writer");
            mm.Add(".abw", "application/x-abiword");
            mm.Add(".mseq", "application/vnd.mseq");
            mm.Add(".mfm", "application/vnd.mfmp");
            mm.Add(".vcf", "text/x-vcard");
            mm.Add(".mpga", "audio/mpeg");
            mm.Add(".hqx", "application/mac-binhex40");
            mm.Add(".flv", "video/x-flv");
            mm.Add(".mxml", "application/xv+xml");
            mm.Add(".pcurl", "application/vnd.curl.pcurl");
            mm.Add(".mbox", "application/mbox");
            mm.Add(".etx", "text/x-setext");
            mm.Add(".txf", "application/vnd.mobius.txf");
            mm.Add(".fti", "application/vnd.anser-web-funds-transfer-initiation");
            mm.Add(".p10", "application/pkcs10");
            mm.Add(".mj2", "video/mj2");
            mm.Add(".cdmic", "application/cdmi-container");
            mm.Add(".rms", "application/vnd.jcp.javame.midlet-rms");
            mm.Add(".xif", "image/vnd.xiff");
            mm.Add(".vcg", "application/vnd.groove-vcard");
            mm.Add(".sh", "application/x-sh");
            mm.Add(".dae", "model/vnd.collada+xml");
            mm.Add(".spf", "application/vnd.yamaha.smaf-phrase");
            mm.Add(".xbd", "application/vnd.fujixerox.docuworks.binder");
            mm.Add(".wmls", "text/vnd.wap.wmlscript");
            mm.Add(".xps", "application/vnd.ms-xpsdocument");
            mm.Add(".gif", "image/gif");
            mm.Add(".mus", "application/vnd.musician");
            mm.Add(".wmz", "application/x-ms-wmz");
            mm.Add(".clkk", "application/vnd.crick.clicker.keyboard");
            mm.Add(".cer", "application/pkix-cert");
            mm.Add(".tsv", "text/tab-separated-values");
            mm.Add(".igx", "application/vnd.micrografx.igx");
            mm.Add(".ssml", "application/ssml+xml");
            mm.Add(".gsf", "application/x-font-ghostscript");
            mm.Add(".bh2", "application/vnd.fujitsu.oasysprs");
            mm.Add(".gph", "application/vnd.flographit");
            mm.Add(".oga", "audio/ogg");
            mm.Add(".ecelp4800", "audio/vnd.nuera.ecelp4800");
            mm.Add(".json", "application/json");
            mm.Add(".dotm", "application/vnd.ms-word.template.macroenabled.12");
            mm.Add(".gdl", "model/vnd.gdl");
            mm.Add(".asf", "video/x-ms-asf");
            mm.Add(".mmr", "image/vnd.fujixerox.edmics-mmr");
            mm.Add(".gxt", "application/vnd.geonext");
            mm.Add(".ncx", "application/x-dtbncx+xml");
            mm.Add(".fly", "text/vnd.fly");
            mm.Add(".seed", "application/vnd.fdsn.seed");
            mm.Add(".atx", "application/vnd.antix.game-component");
            mm.Add(".scs", "application/scvp-cv-response");
            mm.Add(".zmm", "application/vnd.handheld-entertainment+xml");
            mm.Add(".xpi", "application/x-xpinstall");
            mm.Add(".wml", "text/vnd.wap.wml");
            mm.Add(".gim", "application/vnd.groove-identity-message");
            mm.Add(".ifm", "application/vnd.shana.informed.formdata");
            mm.Add("0.123", "application/vnd.lotus-1-2-3");
            mm.Add(".fst", "image/vnd.fst");
            mm.Add(".ivu", "application/vnd.immervision-ivu");
            mm.Add(".afp", "application/vnd.ibm.modcap");
            mm.Add(".clkw", "application/vnd.crick.clicker.wordbank");
            mm.Add(".mads", "application/mads+xml");
            mm.Add(".src", "application/x-wais-source");
            mm.Add(".bcpio", "application/x-bcpio");
            mm.Add(".texinfo", "application/x-texinfo");
            mm.Add(".lbe", "application/vnd.llamagraphics.life-balance.exchange+xml");
            mm.Add(".rif", "application/reginfo+xml");
            mm.Add(".js", "application/javascript");
            mm.Add(".pclxl", "application/vnd.hp-pclxl");
            mm.Add(".spq", "application/scvp-vp-request");
            mm.Add(".p7m", "application/pkcs7-mime");
            mm.Add(".tcl", "application/x-tcl");
            mm.Add(".see", "application/vnd.seemail");
            mm.Add(".aep", "application/vnd.audiograph");
            mm.Add(".stl", "application/vnd.ms-pki.stl");
            mm.Add(".sit", "application/x-stuffit");
            mm.Add(".tsd", "application/timestamped-data");
            mm.Add(".swf", "application/x-shockwave-flash");
            mm.Add(".clkx", "application/vnd.crick.clicker");
            mm.Add(".bed", "application/vnd.realvnc.bed");
            mm.Add(".nns", "application/vnd.noblenet-sealer");
            mm.Add(".p8", "application/pkcs8");
            mm.Add(".scd", "application/x-msschedule");
            mm.Add(".fdf", "application/vnd.fdf");
            mm.Add(".zip", "application/zip");
            mm.Add(".cdmiq", "application/cdmi-queue");
            mm.Add(".rsd", "application/rsd+xml");
            mm.Add(".txd", "application/vnd.genomatix.tuxedo");
            mm.Add(".emma", "application/emma+xml");
            mm.Add(".fg5", "application/vnd.fujitsu.oasysgp");
            mm.Add(".irp", "application/vnd.irepository.package+xml");
            mm.Add(".snf", "application/x-font-snf");
            mm.Add(".mscml", "application/mediaservercontrol+xml");
            mm.Add(".3dml", "text/vnd.in3d.3dml");
            mm.Add(".xer", "application/patch-ops-error+xml");
            mm.Add(".rld", "application/resource-lists-diff+xml");
            mm.Add(".geo", "application/vnd.dynageo");
            mm.Add(".teacher", "application/vnd.smart.teacher");
            mm.Add(".adp", "audio/adpcm");
            mm.Add(".oas", "application/vnd.fujitsu.oasys");
            mm.Add(".flo", "application/vnd.micrografx.flo");
            mm.Add(".oth", "application/vnd.oasis.opendocument.text-web");
            mm.Add(".s", "text/x-asm");
            mm.Add(".p", "text/x-pascal");
            mm.Add(".rep", "application/vnd.businessobjects");
            mm.Add(".ppam", "application/vnd.ms-powerpoint.addin.macroenabled.12");
            mm.Add(".mag", "application/vnd.ecowin.chart");
            mm.Add(".ivp", "application/vnd.immervision-ivp");
            mm.Add(".semd", "application/vnd.semd");
            mm.Add(".musicxml", "application/vnd.recordare.musicxml+xml");
            mm.Add(".sus", "application/vnd.sus-calendar");
            mm.Add(".msh", "model/mesh");
            mm.Add(".ace", "application/x-ace-compressed");
            mm.Add(".zir", "application/vnd.zul");
            mm.Add(".h264", "video/h264");
            mm.Add(".qps", "application/vnd.publishare-delta-tree");
            mm.Add(".mpn", "application/vnd.mophun.application");
            mm.Add(".clp", "application/x-msclip");
            mm.Add(".sc", "application/vnd.ibm.secure-container");
            mm.Add(".mets", "application/mets+xml");
            mm.Add(".paw", "application/vnd.pawaafile");
            mm.Add(".ez2", "application/vnd.ezpix-album");
            mm.Add(".ppm", "image/x-portable-pixmap");
            mm.Add(".svc", "application/vnd.dvb.service");
            mm.Add(".mdb", "application/x-msaccess");
            mm.Add(".aso", "application/vnd.accpac.simply.aso");
            mm.Add(".mp4a", "audio/mp4");
            mm.Add(".pya", "audio/vnd.ms-playready.media.pya");
            mm.Add(".weba", "audio/webm");
            mm.Add(".ecelp7470", "audio/vnd.nuera.ecelp7470");
            mm.Add(".dtshd", "audio/vnd.dts.hd");
            mm.Add(".vxml", "application/voicexml+xml");
            mm.Add(".pwn", "application/vnd.3m.post-it-notes");
            mm.Add(".mgz", "application/vnd.proteus.magazine");
            mm.Add(".link66", "application/vnd.route66.link66+xml");
            mm.Add(".pic", "image/x-pict");
            mm.Add(".docm", "application/vnd.ms-word.document.macroenabled.12");
            mm.Add(".pvb", "application/vnd.3gpp.pic-bw-var");
            mm.Add(".wav", "audio/x-wav");
            mm.Add(".ram", "audio/x-pn-realaudio");
            mm.Add(".lrm", "application/vnd.ms-lrm");
            mm.Add(".atom", "application/atom+xml");
            mm.Add(".utz", "application/vnd.uiq.theme");
            mm.Add(".rmp", "audio/x-pn-realaudio-plugin");
            mm.Add(".xenc", "application/xenc+xml");
            mm.Add(".jpg", "image/jpeg");
            mm.Add(".cww", "application/prs.cww");
            mm.Add(".rss", "application/rss+xml");
            mm.Add(".mp4", "application/mp4");
            mm.Add(".t", "text/troff");
            mm.Add(".cab", "application/vnd.ms-cab-compressed");
            mm.Add(".res", "application/x-dtbresource+xml");
            mm.Add(".pcf", "application/x-font-pcf");
            mm.Add(".cii", "application/vnd.anser-web-certificate-issue-initiation");
            mm.Add(".sdc", "application/vnd.stardivision.calc");
            mm.Add(".chrt", "application/vnd.kde.kchart");
            mm.Add(".ott", "application/vnd.oasis.opendocument.text-template");
            mm.Add(".azs", "application/vnd.airzip.filesecure.azs");
            mm.Add(".hps", "application/vnd.hp-hps");
            mm.Add(".dvi", "application/x-dvi");
            mm.Add(".box", "application/vnd.previewsystems.box");
            mm.Add(".jpeg", "image/jpeg");
            mm.Add(".ustar", "application/x-ustar");
            mm.Add(".irm", "application/vnd.ibm.rights-management");
            mm.Add(".csml", "chemical/x-csml");
            mm.Add(".mxu", "video/vnd.mpegurl");
            mm.Add(".ngdat", "application/vnd.nokia.n-gage.data");
            mm.Add(".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow");
            mm.Add(".curl", "text/vnd.curl");
            mm.Add(".djvu", "image/vnd.djvu");
            mm.Add(".plb", "application/vnd.3gpp.pic-bw-large");
            mm.Add(".sv4crc", "application/x-sv4crc");
            mm.Add(".ei6", "application/vnd.pg.osasli");
            mm.Add(".pdf", "application/pdf");
            mm.Add(".cpt", "application/mac-compactpro");
            mm.Add(".ltf", "application/vnd.frogans.ltf");
            mm.Add(".ait", "application/vnd.dvb.ait");
            mm.Add(".ma", "application/mathematica");
            mm.Add(".icc", "application/vnd.iccprofile");
            mm.Add(".ssf", "application/vnd.epson.ssf");
            mm.Add(".sitx", "application/x-stuffitx");
            mm.Add(".smi", "application/smil+xml");
            mm.Add(".msl", "application/vnd.mobius.msl");
            mm.Add(".semf", "application/vnd.semf");
            mm.Add(".pgp", "application/pgp-signature");
            mm.Add(".psf", "application/x-font-linux-psf");
            mm.Add(".imp", "application/vnd.accpac.simply.imp");
            mm.Add(".atomsvc", "application/atomsvc+xml");
            mm.Add(".sda", "application/vnd.stardivision.draw");
            mm.Add(".dna", "application/vnd.dna");
            mm.Add(".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide");
            mm.Add(".c", "text/x-c");
            mm.Add(".der", "application/x-x509-ca-cert");
            mm.Add(".n3", "text/n3");
            mm.Add(".dssc", "application/dssc+der");
            mm.Add(".nnw", "application/vnd.noblenet-web");
            mm.Add(".crl", "application/pkix-crl");
            mm.Add(".swi", "application/vnd.aristanetworks.swi");
            mm.Add(".uvu", "video/vnd.uvvu.mp4");
            mm.Add(".wqd", "application/vnd.wqd");
            mm.Add(".cdmid", "application/cdmi-domain");
            mm.Add(".plf", "application/vnd.pocketlearn");
            mm.Add(".aam", "application/x-authorware-map");
            mm.Add(".fm", "application/vnd.framemaker");
            mm.Add(".mbk", "application/vnd.mobius.mbk");
            mm.Add(".cil", "application/vnd.ms-artgalry");
            mm.Add(".stw", "application/vnd.sun.xml.writer.template");
            mm.Add(".davmount", "application/davmount+xml");
            mm.Add(".lvp", "audio/vnd.lucent.voice");
            mm.Add(".uri", "text/uri-list");
            mm.Add(".tfm", "application/x-tex-tfm");
            mm.Add(".prc", "application/x-mobipocket-ebook");
            mm.Add(".cdmia", "application/cdmi-capability");
            mm.Add(".bin", "application/octet-stream");
            mm.Add(".sti", "application/vnd.sun.xml.impress.template");
            mm.Add(".dd2", "application/vnd.oma.dd2+xml");
            mm.Add(".ez3", "application/vnd.ezpix-package");
            mm.Add(".ttf", "application/x-font-ttf");
            mm.Add(".exi", "application/exi");
            mm.Add(".rq", "application/sparql-query");
            mm.Add(".mathml", "application/mathml+xml");
            mm.Add(".csh", "application/x-csh");
            mm.Add(".fnc", "application/vnd.frogans.fnc");
            mm.Add(".wmv", "video/x-ms-wmv");
            mm.Add(".wps", "application/vnd.ms-works");
            mm.Add(".otf", "application/x-font-otf");
            mm.Add(".sldm", "application/vnd.ms-powerpoint.slide.macroenabled.12");
            mm.Add(".xhtml", "application/xhtml+xml");
            mm.Add(".pcl", "application/vnd.hp-pcl");
            mm.Add(".p12", "application/x-pkcs12");
            mm.Add(".rpss", "application/vnd.nokia.radio-presets");
            mm.Add(".odb", "application/vnd.oasis.opendocument.database");
            mm.Add(".hpid", "application/vnd.hp-hpid");
            mm.Add(".p7b", "application/x-pkcs7-certificates");
            mm.Add(".ipfix", "application/ipfix");
            mm.Add(".otc", "application/vnd.oasis.opendocument.chart-template");
            mm.Add(".m21", "application/mp21");
            mm.Add(".edx", "application/vnd.novadigm.edx");
            mm.Add(".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template");
            mm.Add(".rp9", "application/vnd.cloanto.rp9");
            mm.Add(".f4v", "video/x-f4v");
            mm.Add(".dir", "application/x-director");
            mm.Add(".mrc", "application/marc");
            mm.Add(".lbd", "application/vnd.llamagraphics.life-balance.desktop");
            mm.Add(".wmf", "application/x-msmetafile");
            mm.Add(".nc", "application/x-netcdf");
            mm.Add(".eol", "audio/vnd.digital-winds");
            mm.Add(".latex", "application/x-latex");
            mm.Add(".spot", "text/vnd.in3d.spot");
            mm.Add(".sub", "image/vnd.dvb.subtitle");
            mm.Add(".dpg", "application/vnd.dpgraph");
            mm.Add(".oa2", "application/vnd.fujitsu.oasys2");
            mm.Add(".xop", "application/xop+xml");
            mm.Add(".dxp", "application/vnd.spotfire.dxp");
            mm.Add(".wbmp", "image/vnd.wap.wbmp");
            mm.Add(".fpx", "image/vnd.fpx");
            mm.Add(".msf", "application/vnd.epson.msf");
            mm.Add(".ief", "image/ief");
            mm.Add(".cat", "application/vnd.ms-pki.seccat");
            mm.Add(".rdf", "application/rdf+xml");
            mm.Add(".rip", "audio/vnd.rip");
            mm.Add(".potx", "application/vnd.openxmlformats-officedocument.presentationml.template");
            mm.Add(".pyv", "video/vnd.ms-playready.media.pyv");
            mm.Add(".smf", "application/vnd.stardivision.math");
            mm.Add(".pre", "application/vnd.lotus-freelance");
            mm.Add(".sxg", "application/vnd.sun.xml.writer.global");
            mm.Add(".bz2", "application/x-bzip2");
            mm.Add(".str", "application/vnd.pg.format");
            mm.Add(".sdd", "application/vnd.stardivision.impress");
            mm.Add(".acc", "application/vnd.americandynamics.acc");
            mm.Add(".rpst", "application/vnd.nokia.radio-preset");
            mm.Add(".wbs", "application/vnd.criticaltools.wbs+xml");
            mm.Add(".htke", "application/vnd.kenameaapp");
            mm.Add(".xls", "application/vnd.ms-excel");
            mm.Add(".mny", "application/x-msmoney");
            mm.Add(".kon", "application/vnd.kde.kontour");
            mm.Add(".xpw", "application/vnd.intercon.formnet");
            mm.Add(".xml", "application/xml");
            mm.Add(".mcurl", "text/vnd.curl.mcurl");
            mm.Add(".tmo", "application/vnd.tmobile-livetv");
            mm.Add(".ktz", "application/vnd.kahootz");
            mm.Add(".m3u", "audio/x-mpegurl");
            mm.Add(".wad", "application/x-doom");
            mm.Add(".7z", "application/x-7z-compressed");
            mm.Add(".fbs", "image/vnd.fastbidsheet");
            mm.Add(".cryptonote", "application/vnd.rig.cryptonote");
            mm.Add(".wvx", "video/x-ms-wvx");
            mm.Add(".xlam", "application/vnd.ms-excel.addin.macroenabled.12");
            mm.Add(".svg", "image/svg+xml");
            mm.Add(".std", "application/vnd.sun.xml.draw.template");
            mm.Add(".igs", "model/iges");
            mm.Add(".xdw", "application/vnd.fujixerox.docuworks");
            mm.Add(".jpgv", "video/jpeg");
            mm.Add(".vsd", "application/vnd.visio");
            mm.Add(".dis", "application/vnd.mobius.dis");
            mm.Add(".mxf", "application/mxf");
            mm.Add(".xpr", "application/vnd.is-xpr");
            mm.Add(".xbap", "application/x-ms-xbap");
            mm.Add(".st", "application/vnd.sailingtracker.track");
            mm.Add(".ogv", "video/ogg");
            mm.Add(".torrent", "application/x-bittorrent");
            mm.Add(".gmx", "application/vnd.gmx");
            mm.Add(".mgp", "application/vnd.osgeo.mapguide.package");
            mm.Add(".kia", "application/vnd.kidspiration");
            mm.Add(".mwf", "application/vnd.mfer");
            mm.Add(".bmp", "image/bmp");
            mm.Add(".portpkg", "application/vnd.macports.portpkg");
            mm.Add(".flx", "text/vnd.fmi.flexstor");
            mm.Add(".i2g", "application/vnd.intergeo");
            mm.Add(".spl", "application/x-futuresplash");
            mm.Add(".igl", "application/vnd.igloader");
            mm.Add(".fcs", "application/vnd.isac.fcs");
            mm.Add(".vsf", "application/vnd.vsf");
            mm.Add(".cod", "application/vnd.rim.cod");
            mm.Add(".sru", "application/sru+xml");
            mm.Add(".spp", "application/scvp-vp-response");
            mm.Add(".cmc", "application/vnd.cosmocaller");
            mm.Add(".ami", "application/vnd.amiga.ami");
            mm.Add(".fe_launch", "application/vnd.denovo.fcselayout-link");
            mm.Add(".ac", "application/pkix-attr-cert");
            mm.Add(".jar", "application/java-archive");
            mm.Add(".cpio", "application/x-cpio");
            mm.Add(".odp", "application/vnd.oasis.opendocument.presentation");
            mm.Add(".grxml", "application/srgs+xml");
            mm.Add(".kml", "application/vnd.google-earth.kml+xml");
            mm.Add(".ttl", "text/turtle");
            mm.Add(".fli", "video/x-fli");
            mm.Add(".plc", "application/vnd.mobius.plc");
            mm.Add(".stf", "application/vnd.wt.stf");
            mm.Add(".aac", "audio/x-aac");
            mm.Add(".movie", "video/x-sgi-movie");
            mm.Add(".uvh", "video/vnd.dece.hd");
            mm.Add(".mif", "application/vnd.mif");
            mm.Add(".woff", "application/x-font-woff");
            mm.Add(".tfi", "application/thraud+xml");
            mm.Add(".yaml", "text/yaml");
            mm.Add(".dtd", "application/xml-dtd");
            mm.Add(".pbm", "image/x-portable-bitmap");
            mm.Add(".dfac", "application/vnd.dreamfactory");
            mm.Add(".mpy", "application/vnd.ibm.minipay");
            mm.Add(".dwf", "model/vnd.dwf");
            mm.Add(".mts", "model/vnd.mts");
            mm.Add(".mxs", "application/vnd.triscape.mxs");
            mm.Add(".xyz", "chemical/x-xyz");
            mm.Add(".pgn", "application/x-chess-pgn");
            mm.Add(".slt", "application/vnd.epson.salt");
            mm.Add(".ai", "application/postscript");
            mm.Add(".fxp", "application/vnd.adobe.fxp");
            mm.Add(".pkipath", "application/pkix-pkipath");
            mm.Add(".xwd", "image/x-xwindowdump");
            mm.Add(".hal", "application/vnd.hal+xml");
            mm.Add(".tpt", "application/vnd.trid.tpt");
            mm.Add(".pptm", "application/vnd.ms-powerpoint.presentation.macroenabled.12");
            mm.Add(".viv", "video/vnd.vivo");
            mm.Add(".scm", "application/vnd.lotus-screencam");
            mm.Add(".pub", "application/x-mspublisher");
            mm.Add(".xdp", "application/vnd.adobe.xdp+xml");
            mm.Add(".wgt", "application/widget");
            mm.Add(".obd", "application/x-msbinder");
            mm.Add(".ods", "application/vnd.oasis.opendocument.spreadsheet");
            mm.Add(".vcx", "application/vnd.vcx");
            mm.Add(".car", "application/vnd.curl.car");
            mm.Add(".pki", "application/pkixcmp");
            mm.Add(".mods", "application/mods+xml");
            mm.Add(".cdmio", "application/cdmi-object");
            mm.Add(".fsc", "application/vnd.fsc.weblaunch");
            mm.Add(".bmi", "application/vnd.bmi");
            mm.Add(".uva", "audio/vnd.dece.audio");
            mm.Add(".ksp", "application/vnd.kde.kspread");
            mm.Add(".scq", "application/scvp-cv-request");
            mm.Add(".kne", "application/vnd.kinar");
            mm.Add(".odg", "application/vnd.oasis.opendocument.graphics");
            mm.Add(".avi", "video/x-msvideo");
            mm.Add(".flw", "application/vnd.kde.kivio");
            mm.Add(".wma", "audio/x-ms-wma");
            mm.Add(".tcap", "application/vnd.3gpp2.tcap");
            mm.Add(".otg", "application/vnd.oasis.opendocument.graphics-template");
            mm.Add(".mvb", "application/x-msmediaview");
            mm.Add(".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template");
            mm.Add(".sxm", "application/vnd.sun.xml.math");
            mm.Add(".daf", "application/vnd.mobius.daf");
            mm.Add(".mid", "audio/midi");
            mm.Add(".wmlsc", "application/vnd.wap.wmlscriptc");
            mm.Add(".dcurl", "text/vnd.curl.dcurl");
            mm.Add(".ppt", "application/vnd.ms-powerpoint");
            mm.Add(".wmd", "application/x-ms-wmd");
            mm.Add(".bak", "application/octet-stream");
            mm.Add(".gram", "application/srgs");
            mm.Add(".aw", "application/applixware");
            mm.Add(".mdi", "image/vnd.ms-modi");
            mm.Add(".hvp", "application/vnd.yamaha.hv-voice");
            mm.Add(".ser", "application/java-serialized-object");
            mm.Add(".hlp", "application/winhlp");
            mm.Add(".sxc", "application/vnd.sun.xml.calc");
            mm.Add(".cla", "application/vnd.claymore");

            return mm;
        }

        public Upload UploadFile(byte[] data, string fileName, string contentType, string description)
        {
            string url = UrlHelper.GetUploadFileUrl(this, fileName);
            Upload u = WebApiHelper.ExecuteUploadFile(this, url, data, "UploadFile");

            u.FileName = fileName;
            u.ContentType = contentType;
            u.Description = description;

            if (contentType == null)
            {
                string ext = System.IO.Path.GetExtension(fileName);
                System.Collections.Generic.Dictionary<string, string> mm = CreateMimeMap();
                if(mm.ContainsKey(ext))
                    u.ContentType = mm[ext];
            }

            return u;
        }


        public Upload UploadFile(string path, string contentType, string description)
        {
            string fileName = System.IO.Path.GetFileName(path);
            byte[] data = System.IO.File.ReadAllBytes(path);

            return UploadFile(data, fileName, contentType, description);
        }


        public Upload UploadFile(string path)
        {
            return UploadFile(path, null, null);
        }


        /// <summary>
        ///     Updates the attachment.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="attachment">The attachment.</param>
        public void UpdateAttachment(int issueId, Attachment attachment)
        {
            string address = UrlHelper.GetAttachmentUpdateUrl(this, issueId);
            Attachments attachments = new Attachments { { attachment.Id, attachment } };
            string data = RedmineSerializer.Serialize(attachments, MimeFormat);

            WebApiHelper.ExecuteUpload(this, address, HttpVerbs.PATCH, data, "UpdateAttachment");
        }

        /// <summary>
        ///     Downloads the file.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="ConflictException"></exception>
        /// <exception cref="NotAcceptableException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public byte[] DownloadFile(string address)
        {
            return WebApiHelper.ExecuteDownloadFile(this, address, "DownloadFile");
        }

        /// <summary>
        ///     Creates a new Redmine object.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="obj">The object to create.</param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="ConflictException"></exception>
        /// <exception cref="NotAcceptableException"></exception>
        /// <remarks>
        ///     When trying to create an object with invalid or missing attribute parameters, you will get a 422 Unprocessable
        ///     Entity response. That means that the object could not be created.
        /// </remarks>
        public T CreateObject<T>(T obj) where T : class, new()
        {
            return CreateObject(obj, null);
        }

        /// <summary>
        ///     Creates the Redmine web client.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="uploadFile">if set to <c>true</c> [upload file].</param>
        /// <returns></returns>
        /// <code></code>
        public virtual RedmineWebClient CreateWebClient(
              System.Collections.Specialized.NameValueCollection parameters
            , bool uploadFile = false)
        {
            RedmineWebClient webClient = new RedmineWebClient { Proxy = Proxy };
            if (!uploadFile)
            {
                webClient.Headers.Add(System.Net.HttpRequestHeader.ContentType, MimeFormat == MimeFormat.Xml
                    ? "application/xml"
                    : "application/json");
                webClient.Encoding = System.Text.Encoding.UTF8;
            }
            else
            {
                webClient.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/octet-stream");
            }

            if (parameters != null)
            {
                webClient.QueryString = parameters;
            }

            if (!string.IsNullOrEmpty(ApiKey))
            {
                webClient.QueryString[RedmineKeys.KEY] = ApiKey;
            }
            else
            {
                if (cache != null)
                {
                    webClient.PreAuthenticate = true;
                    webClient.Credentials = cache;
                    webClient.Headers[System.Net.HttpRequestHeader.Authorization] = basicAuthorization;
                }
                else
                {
                    webClient.UseDefaultCredentials = true;
                    webClient.Credentials = System.Net.CredentialCache.DefaultCredentials;
                }
            }

            if (!string.IsNullOrEmpty(ImpersonateUser))
            {
                webClient.Headers.Add("X-Redmine-Switch-User", ImpersonateUser);
            }

            return webClient;
        }

        /// <summary>
        ///     This is to take care of SSL certification validation which are not issued by Trusted Root CA.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The errors.</param>
        /// <returns></returns>
        /// <code></code>
        public virtual bool RemoteCertValidate(object sender
            , System.Security.Cryptography.X509Certificates.X509Certificate certificate
            , System.Security.Cryptography.X509Certificates.X509Chain chain
            , System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            Logger.Current.Error("X509Certificate [{0}] Policy Error: '{1}'", certificate.Subject, sslPolicyErrors);


            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                            continue;
                        }
                        else if (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotTimeValid)
                        {
                            // Ignore Expired certificates
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    } // Next status 

                } // End if (chain != null && chain.ChainStatus != null) 

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates (, or expired certificates). 
                // These certificates are valid for default Exchange server installations, so return true.
                return true;
            } // End if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0) 

            return false;
        }
    }
}