
sudo apt-get install redmine redmine-pgsql ruby-dev freetds-dev
cd /usr/share/redmine
bundle install 

bundle exec rails server webrick -e production


Redmine-Version is here:
/usr/share/redmine/lib/redmine/version.rb

db-config:
/etc/redmine/default/database.yml


PG:
production:
  adapter: postgresql
  host: localhost
  port: 5432
  database: redmine_default
  username: redmine/instances/default
  password: TOP_SECRET
  encoding: utf8


MS:
production:
  adapter: sqlserver
  host: 127.0.0.1
  port: 1433
  database: Redmine
  username: redminer
  password: TOP_SECRET




wrong url: http://localhost:3000//uploads.xml
<errors type="array"><error>Attachment Erweiterung  ist nicht zugelassen</error></errors>

actual: http://localhost:3000//uploads.xml?filename=<filename>



webClient.Headers.Add(HttpRequestHeader.ContentType, "application/octet-stream");
webClient.PreAuthenticate = true;

private readonly CredentialCache cache;
cache = new CredentialCache { { new Uri(host), "Basic", new NetworkCredential(login, password) } };

var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", login, password)));
basicAuthorization = string.Format("Basic {0}", token);


webClient.Credentials = cache;
webClient.Headers[HttpRequestHeader.Authorization] = basicAuthorization; 



 │ The redmine/instances/default package must have a database installed and  │ 
 │ configured before it can be used. This can be optionally handled with     │ 
 │ dbconfig-common.                                                          │ 
 │                                                                           │ 
 │ If you are an advanced database administrator and know that you want to   │ 
 │ perform this configuration manually, or if your database has already      │ 
 │ been installed and configured, you should refuse this option. Details on  │ 
 │ what needs to be done should most likely be provided in                   │ 
 │ /usr/share/doc/redmine/instances/default.                                 │ 
 │                                                                           │ 
 │ Otherwise, you should probably choose this option.                        │ 
 │                                                         

The redmine/instances/default package can be configured to use one of       
 │ several database types. Below, you will be presented with the available     
 │ choices.                                                                    
 │                                                                             
 │ If other database types are supported by redmine/instances/default but      
 │ not shown here, the reason for their omission is that the corresponding     
 │ dbconfig-<database type> packages are not installed. If you know that       
 │ you want the package to use another supported database type, your best      
 │ option is to back out of the dbconfig-common questions and opt out of       
 │ dbconfig-common assistance for this package for now. Install your           
 │ preferred dbconfig-<database type> option from the list in the package      
 │ dependencies, and then "dpkg-reconfigure redmine/instances/default" to 



https://go.christiansteven.com/business-intelligence-solution-bi-strategy-roadmap
