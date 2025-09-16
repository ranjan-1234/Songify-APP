using MySql.Data.MySqlClient;
using System.Data;

namespace Singer.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            var rawConnectionString = configuration.GetConnectionString("DefaultConnection")!;

            // Get password from Render environment
            var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
            if (string.IsNullOrEmpty(mysqlPassword))
                throw new Exception("MYSQL_PASSWORD environment variable is not set.");

            // Replace placeholder
            _connectionString = rawConnectionString.Replace("${MYSQL_PASSWORD}", mysqlPassword);
        }

        public DataTable ExecuteSelectQuery(string query, MySqlParameter[] parameters)
        {
            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(query, conn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            using var adapter = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt); // This was failing before
            return dt;
        }

        public int ExecuteNonQuery(string query, MySqlParameter[] parameters)
        {
            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(query, conn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}
