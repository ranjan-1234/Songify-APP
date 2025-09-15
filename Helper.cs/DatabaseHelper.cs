using MySql.Data.MySqlClient;
using System.Data;

namespace Singer.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public DataTable ExecuteSelectQuery(string query, MySqlParameter[] parameters)
        {
            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(query, conn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            using var adapter = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
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
