using Microsoft.Data.SqlClient;
using System;

namespace _02._Villain_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(Config.SqlConn);

            sqlConnection.Open();

            string query = @"SELECT v.Name,
	                                COUNT(mv.MinionId) AS [MinionsCount]
                            FROM Villains AS v
                            LEFT JOIN MinionsVillains AS mv ON mv.VillainId = v.Id
                            JOIN Minions AS m ON m.Id = mv.MinionId
                            GROUP BY v.Name
                            HAVING COUNT(mv.MinionId) > 3";

            SqlCommand cmd = new SqlCommand(query, sqlConnection);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]} - {reader["MinionsCount"]}");
            }

            sqlConnection.Close();
        }
    }
}
