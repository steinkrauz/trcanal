using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vksanal
{
    class DBAdapter:IDisposable
    {
        SqlConnection conn;
        

        public DBAdapter()
        {
            string connString= $"Data Source={ConfigurationManager.AppSettings["Server"]};Initial Catalog={ConfigurationManager.AppSettings["Database"]};User ID={ConfigurationManager.AppSettings["UserID"]};Password={ConfigurationManager.AppSettings["Password"]}";
            conn = new SqlConnection(connString);
            conn.Open();
        }

        public void Dispose()
        {
            conn.Close();
        }

        public void Store(VksInfo info)
        {
            string sqlSess = $"INSERT INTO VKSSESSIONS (id, topic, start_time, end_time) " +
                $"VALUES('{info.Id}','{info.Topic}','{info.StartTime}','{info.EndTime}')";
            using (SqlCommand cmd = new SqlCommand(sqlSess, conn))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand usersCmd = conn.CreateCommand())
            {
                usersCmd.CommandType = System.Data.CommandType.Text;
                usersCmd.CommandText = "INSERT INTO VKSUSERS (id, name) VALUES (@id, @name);";
                usersCmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NChar));
                usersCmd.Parameters.Add(new SqlParameter("@name", SqlDbType.NChar));
                try
                {
                    foreach (var name in info.Participants)
                    {
                        usersCmd.Parameters[0].Value = info.Id;
                        usersCmd.Parameters[1].Value = name;
                        usersCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
