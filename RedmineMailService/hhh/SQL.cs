
namespace RedmineMailService
{


    class SQL
    {

        protected static System.Data.Common.DbProviderFactory s_factory;

        static SQL()
        {
            s_factory = System.Data.SqlClient.SqlClientFactory.Instance;
        }


        public static string GetConnectionString()
        {
            var csb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            csb.DataSource = System.Environment.MachineName;
            csb.InitialCatalog = "COR_Basic_SwissLife_UAT";

            csb.IntegratedSecurity = true;
            return csb.ConnectionString;
        }



        public static System.Data.Common.DbConnection GetConnection(string connectionString)
        {
            System.Data.Common.DbConnection con = s_factory.CreateConnection();
            con.ConnectionString = connectionString;

            return con;
        }


        public static System.Data.Common.DbConnection GetConnection()
        {
            return GetConnection(GetConnectionString());
        }

        public static System.Data.IDbCommand CreateCommand(string strSQL)
        {
            System.Data.Common.DbCommand tCommand = s_factory.CreateCommand();
            tCommand.CommandText = strSQL;

            return tCommand;
        }


            public static System.Data.IDbDataParameter AddParameter(System.Data.IDbCommand command, string strParameterName, object objValue)
        {
            return AddParameter(command, strParameterName, objValue, System.Data.ParameterDirection.Input);
        }

        public static System.Data.IDbDataParameter AddParameter(System.Data.IDbCommand command, string strParameterName, object objValue, System.Data.ParameterDirection pad)
        {
            if (objValue == null)
                objValue = System.DBNull.Value;

            System.Type tDataType = objValue.GetType();
            System.Data.DbType dbType = GetDbType(tDataType);

            return AddParameter(command, strParameterName, objValue, pad, dbType);
        }

        public static void RemoveParameter(System.Data.IDbCommand command, string parameterName)
        {
            if (!parameterName.StartsWith("@"))
                parameterName = "@" + parameterName;

            if ((command.Parameters.Contains(parameterName)))
                command.Parameters.RemoveAt(parameterName);
        }

        public static System.Data.IDbDataParameter AddParameter(System.Data.IDbCommand command, string strParameterName, object objValue, System.Data.ParameterDirection pad, System.Data.DbType dbType)
        {
            System.Data.IDbDataParameter parameter = command.CreateParameter();

            if (!strParameterName.StartsWith("@"))
                strParameterName = "@" + strParameterName;

            if ((command.Parameters.Contains(strParameterName)))
                command.Parameters.RemoveAt(strParameterName);

            parameter.ParameterName = strParameterName;
            parameter.DbType = dbType;
            parameter.Direction = pad;

            if (objValue == null)
                parameter.Value = System.DBNull.Value;
            else
                parameter.Value = objValue;

            command.Parameters.Add(parameter);
            return parameter;
        }

        protected static System.Data.DbType GetDbType(System.Type type)
        {
            // http://social.msdn.microsoft.com/Forums/en/winforms/thread/c6f3ab91-2198-402a-9a18-66ce442333a6
            string strTypeName = type.Name;
            System.Data.DbType DBtype = System.Data.DbType.String; // default value

            if (object.ReferenceEquals(type, typeof(System.DBNull)))
                return DBtype;

            try
            {
                DBtype = (System.Data.DbType)System.Enum.Parse(typeof(System.Data.DbType), strTypeName, true);
            }
            catch
            {
            }

            return DBtype;
        }



        public static System.Data.DataTable GetDataTable(System.Data.IDbCommand cmd)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
           
            using (System.Data.IDbConnection idbConn = GetConnection())
            {
                cmd.Connection = idbConn;

                try
                {
                    using (System.Data.Common.DbDataAdapter da = s_factory.CreateDataAdapter())
                    {
                        da.SelectCommand = (System.Data.Common.DbCommand) cmd;
                        da.Fill(dt);
                    }

                }
                catch
                {
                    throw;
                }
            }

            return dt;
        }

        public static System.Data.DataTable GetDataTable(string sql)
        {
            System.Data.DataTable dt = null;
            
            using (System.Data.IDbCommand cmd = SQL.CreateCommand(sql))
            {
                dt = GetDataTable(cmd);
            }

            return dt;
        }


        public static void ExecuteNonQuery(System.Data.IDbCommand cmd)
        {
            using (System.Data.IDbConnection idbConn = GetConnection())
            {
                cmd.Connection = idbConn;

                try
                {
                    if (idbConn.State != System.Data.ConnectionState.Open)
                        idbConn.Open();

                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    throw;
                }
            }
        }


        protected static T CAnyType<T>(object UTO)
        {
            return (T)UTO;
        } // CAnyType


        public static T ExecuteScalar<T>(System.Data.IDbCommand cmd)
        {
            string strReturnValue = "";
            object objResult = null;
            System.Type tReturnType = null;

            // Create a connection
            using (System.Data.IDbConnection sqldbConnection = GetConnection())
            {
                try
                {
                    tReturnType = typeof(T);
                    cmd.Connection = sqldbConnection;

                    if (cmd.Connection.State != System.Data.ConnectionState.Open)
                        cmd.Connection.Open();

                    objResult = cmd.ExecuteScalar();

                    if (objResult != null)
                    {
                        if (!object.ReferenceEquals(tReturnType, typeof(byte[])))
                        {
                            strReturnValue = objResult.ToString();
                            objResult = null;
                        }
                    }
                    else
                        strReturnValue = null;
                }

                // MsgBox("Command completed successfully", MsgBoxStyle.OkOnly, "Success !")
                catch
                {
                    // 'Logme
                    throw;
                }
            } // sqldbConnection


            try
            {
                if (tReturnType == typeof(string))
                    return CAnyType<T>((object)strReturnValue);
                else if (tReturnType == typeof(bool))
                {
                    if (string.IsNullOrEmpty(strReturnValue))
                        return CAnyType<T>(false);

                    double n;
                    
                    if (double.TryParse(strReturnValue, out n))
                    {
                        if (n == 0.0d)
                            return CAnyType<T>(false);
                        else
                            return CAnyType<T>(true);
                    }

                    bool bReturnValue = bool.Parse(strReturnValue);
                    return CAnyType<T>((object)bReturnValue);
                }
                else if (tReturnType == typeof(int))
                {
                    int iReturnValue = int.Parse(strReturnValue);
                    return CAnyType<T>((object)iReturnValue);
                }
                else if (tReturnType == typeof(long))
                {
                    long lngReturnValue = long.Parse(strReturnValue);
                    return CAnyType<T>((object)lngReturnValue);
                }
                else if (tReturnType == typeof(System.Type))
                {
                    // Type.GetType() will only look in the calling assembly and then mscorlib.dll for the type. 
                    // Use Type.AssemblyQualifiedName for getting any type.
                    System.Type tReturnValue = System.Type.GetType(strReturnValue);
                    if (System.StringComparer.OrdinalIgnoreCase.Equals(strReturnValue, "System.Uri"))
                        tReturnValue = typeof(System.Uri);

                    return CAnyType<T>((object)tReturnValue);
                }
                else if (tReturnType == typeof(byte[]))
                {
                    if (objResult == System.DBNull.Value)
                        return CAnyType<T>(null);

                    return CAnyType<T>(objResult);
                }
                else
                    // COR.Debug.Output.MsgBox("ExecuteSQLstmtScalar(Of " + GetType(T).ToString() + "): This type is not yet defined.")
                    // System.Diagnostics.Trace.WriteLine("ExecuteSQLstmtScalar(Of T): This type is not yet defined.")
                    throw new System.NotImplementedException("ExecuteSQLstmtScalar(Of " + typeof(T).ToString() + "): This type is not yet defined.");
            }
            catch
            {
                // 'Logme
                throw;
            }

            return default(T);
        } // ExecuteScalar


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
