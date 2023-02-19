using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _05.__Change_Town_Names_Casing
{
    class Program
    {
        static void Main(string[] args)
        {
            using SqlConnection sql = new SqlConnection(Config.Sqlconnection);
            sql.Open();
            UpdateTownName(Console.ReadLine(), sql);
            sql.Close();                   
        }

        public static void UpdateTownName(string v, SqlConnection sql)
        {
            string updateTownsQuery = @"UPDATE Towns
                                        SET Name = UPPER(Name)
                                        WHERE CountryCode = (
                                        SELECT c.Id FROM Countries AS c 
                                        WHERE c.Name = @countryName)";
            SqlCommand updateTownsCmd = new SqlCommand(updateTownsQuery, sql);
            updateTownsCmd.Parameters.AddWithValue("@countryName", v);

            string townNamesQuery = @" SELECT t.Name 
                                       FROM Towns as t
                                       JOIN Countries AS c ON c.Id = t.CountryCode
                                       WHERE c.Name = @countryName";
            SqlCommand townNamesCmd = new SqlCommand(townNamesQuery, sql);
            townNamesCmd.Parameters.AddWithValue("@countryName", v);
            SqlDataReader reader = townNamesCmd.ExecuteReader();

            if(!reader.HasRows)
                Console.WriteLine("No town names were affected.");
            else
            {
                var affectedTowns = new List<string>();
                while (reader.Read())
                    affectedTowns.Add(reader["Name"].ToString());

                Console.WriteLine($"{affectedTowns.Count} town names were affected.");
                Console.WriteLine($"[{string.Join(", ", affectedTowns)}]");
            }
        }
    }
}
