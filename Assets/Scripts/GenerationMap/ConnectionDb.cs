using System.Collections.Generic;
using Npgsql;
using UnityEngine;

namespace GenerationMap
{
    public class ConnectionDb : MonoBehaviour
    {
        private readonly string _connectionString = "Port = 5432;" +
                                                   "Server= ec2-54-77-40-202.eu-west-1.compute.amazonaws.com;" +
                                                   "Database= dp3oh4vja8l35;" +
                                                   "User ID= eudqcffpovolpi;" +
                                                   "Password= 65f254f251471be22f035c26958c8cfad49fc31c9e8134febf4f4c165bd47665;" +
                                                   "sslmode=Prefer;" +
                                                   "Trust Server Certificate=true";

        private NpgsqlConnection _dbConnection;

        void Start()
        {
            _dbConnection = new NpgsqlConnection(_connectionString);
            _dbConnection.Open();
        }

        public List<AdminNewMode.HallOptions> GetAllOptions()
        {
            var options = new List<AdminNewMode.HallOptions>();

            var command = _dbConnection.CreateCommand();
            var sqlRequest =
                "SELECT * FROM " +
                "options_view";
            command.CommandText = sqlRequest;
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var newOption = new AdminNewMode.HallOptions
                {
                    onum = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    name = reader.IsDBNull(1) ? "NULL" : reader.GetString(1),
                    sizex = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    sizez = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    is_date_b = reader.GetBoolean(4),
                    is_date_e = reader.GetBoolean(5),
                    date_begin = reader.IsDBNull(6) ? "NULL" : reader.GetDateTime(6).ToShortDateString(),
                    date_end = reader.IsDBNull(7) ? "NULL" : reader.GetDateTime(7).ToShortDateString(),
                    is_maintained = reader.GetBoolean(8),
                    is_hidden = reader.GetBoolean(9),
                };
                options.Add(newOption);
            }

            reader.Close();
            return options;
        }
    
        public AdminEditMode.HallContent GetContentByOnum(int num)
        {
            var command = _dbConnection.CreateCommand();
            var sql =
                "SELECT c.cnum, c.title, c.image_desc, c.image_url, c.pos_x, c.pos_z, c.combined_pos, c.type " +
                "FROM public.options AS o " +
                $"JOIN public.contents AS c ON {num} = c.onum";
            command.CommandText = sql;
            var reader = command.ExecuteReader();
            var content = new AdminEditMode.HallContent();
            while (reader.Read())
            {
                content = new AdminEditMode.HallContent()
                {
                    onum = num,
                    cnum = (reader.IsDBNull(0)) ? 0 : reader.GetInt32(0),
                    title = (reader.IsDBNull(1)) ? "NULL" : reader.GetString(1),
                    image_desc = (reader.IsDBNull(2)) ? "NULL" : reader.GetString(2),
                    image_url = (reader.IsDBNull(3)) ? "NULL" : reader.GetString(3),
                    pos_x = (reader.IsDBNull(4)) ? 0 : reader.GetInt32(4),
                    pos_z = (reader.IsDBNull(5)) ? 0 : reader.GetInt32(5),
                    combined_pos = (reader.IsDBNull(6)) ? "NULL" : reader.GetString(6),
                    type = (reader.IsDBNull(7)) ? 0 : reader.GetInt32(7),
                };
            }
        
            reader.Close();
            if (content.cnum == default)
            {
                Debug.Log($"Empty content by onum {num}");
            }
            return content;
        }
    }
}