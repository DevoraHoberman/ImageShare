using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharePassword.Data
{
    public class ImageRepository
    {
        private string _connectionString;

        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddImage(string fileName, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Image(FileName, Password, Views)
                                VALUES (@fileName, @password, @views) SELECT SCOPE_IDENTITY();";
            cmd.Parameters.AddWithValue("@fileName", fileName);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@views", 0);
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }

        public Image GetImage(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Image WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var reader = cmd.ExecuteReader();
            reader.Read();
            var image = new Image
            {
                Id = (int)reader["id"],
                FileName = (string)reader["FileName"],
                Password = (string)reader["Password"],
                Views = (int)reader["Views"]
            };

            return image;
        }

        public string GetPassword(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Password FROM Image WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            return (string)cmd.ExecuteScalar();
        }
        public void UpdateView(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Image SET Views = Views + 1 WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        public Image VerifyPassword(int id, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Image WHERE Id = @id AND Password = @password";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@password", password);
            conn.Open();
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return (new Image
                {
                    Id = (int)reader["id"],
                    FileName = (string)reader["FileName"],
                    Password = (string)reader["Password"],
                    Views = (int)reader["Views"]
                });
            }
            return null;
        }
    }
}
