using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Singer.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        
        // ✅ For INSERT, UPDATE, DELETE
        public int ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, con);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            con.Open();
            return cmd.ExecuteNonQuery();
        }



        // ✅ For SELECT queries
        public DataTable ExecuteSelectQuery(string query, SqlParameter[] parameters)
        {
            var dt = new DataTable();
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, con);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }
    }
}
