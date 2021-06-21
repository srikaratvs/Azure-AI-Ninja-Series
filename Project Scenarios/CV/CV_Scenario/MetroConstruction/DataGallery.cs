using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;



namespace MetroConstruction
{
    public class DataGallery
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
        }

        public static void GetLogData()
        {
            
            MySqlCommand cmd = new MySqlCommand("SELECT t1.id, employee_id, register_image1, first_name, log_type, log_date_time from employees as t1 LEFT JOIN (SELECT id,user_id, log_date_time, log_type from attendance_log where id in (SELECT max(id) as id from attendance_log WHERE log_date_time>'2021-03-09' GROUP by user_id)) as logtable on t1.id=logtable.user_id LIMIT 15", new MySqlConnection(GetConnectionString()));

            // cmd.Parameters.AddWithValue("@todaydatet", "2021-03-09");
            try
            {
                cmd.Connection.Open();
                MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                List<string> flag = new List<string>();
                while (dr.Read())
                {
                    flag.Add(Convert.ToString(dr["employee_id"]));
                }
                var s = flag;
                dr.Close();
            }
            catch(Exception ex)
            {

            }
            

            return;
        }


    }

}
