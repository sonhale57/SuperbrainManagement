using Hangfire;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SuperbrainManagement.Controllers
{
    public class Connect
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
        public Connect() { }
        public static List<T> Select<T>(string query) where T : new()
        {
            List<T> data = new List<T>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    T item = new T();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        object value = reader[i];

                        // Using reflection to set the property value of the object
                        var property = typeof(T).GetProperty(columnName);
                        if (property != null && value != DBNull.Value)
                        {
                            property.SetValue(item, value);
                        }
                    }

                    data.Add(item);
                }

                reader.Close();
            }

            return data;
        }
        public static DataTable SelectAll(string query)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dataTable.Load(reader); // Load data directly into DataTable

                reader.Close();
            }

            return dataTable;
        }

        public static T SelectSingle<T>(string query) where T : new()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                T item = default(T);

                if (reader.Read())
                {
                    item = new T();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        object value = reader[i];

                        // Using reflection to set the property value of the object
                        var property = typeof(T).GetProperty(columnName);
                        if (property != null && value != DBNull.Value)
                        {
                            property.SetValue(item, value);
                        }
                    }
                }
                reader.Close();
                return item;
            }
        }
    }
}