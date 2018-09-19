
namespace RedmineMailService
{


    class SQL
    {



        public static string GetConnectionString()
        {
            var csb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            csb.DataSource = System.Environment.MachineName;
            csb.InitialCatalog = "COR_Basic_SwissLife_UAT";

            csb.IntegratedSecurity = true;
            return csb.ConnectionString;
        }


        public static string ExecuteScalar(string sql)
        {
            string json = "";
            using (var con = new System.Data.SqlClient.SqlConnection(GetConnectionString()))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    json = System.Convert.ToString(cmd.ExecuteScalar());
                    if (con.State != System.Data.ConnectionState.Closed)
                        con.Close();
                }
            }

            return json;
        }


    }


}
