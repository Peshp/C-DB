using System;
using System.Data.SqlClient;
using System.Text;

namespace _03._Minion_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());
            SqlConnection sqlConn = new SqlConnection(Config.sqlConnect);
            sqlConn.Open();
            Console.WriteLine(LogicResult(sqlConn, id));
            sqlConn.Close();
        }
        public static string LogicResult(SqlConnection sqlConn, int villainId)
        {
            StringBuilder output = new StringBuilder();

            string villainNameQuery = @"SELECT [Name] FROM Villains                            
                            WHERE Id = @villainId";
            SqlCommand villainNameCmd = new SqlCommand(villainNameQuery, sqlConn);
            villainNameCmd.Parameters.AddWithValue("@villainId", villainId);
            string villainName = villainNameCmd.ExecuteScalar().ToString();

            if(villainName == null)
                return $"No villain with ID {villainId} exists in the database.";

            output.AppendLine($"Villain: {villainName}");

            string minnionsQuery = @"SELECT m.Name, m.Age
                                    FROM MinionsVillains AS mv 
                                    LEFT JOIN Minions AS m ON m.Id = mv.MinionId
                                    WHERE mv.VillainId = @villainId
                                    ORDER BY m.Name";
            SqlCommand minnionsCmd = new SqlCommand(minnionsQuery, sqlConn);
            minnionsCmd.Parameters.AddWithValue("@villainId", villainId);
            using SqlDataReader reader = minnionsCmd.ExecuteReader();

            if (!reader.HasRows)
                output.AppendLine("(no minions)");
            else
            {
                int pos = 0;
                while (reader.Read())
                {
                    output.AppendLine($"{pos}. {reader["Name"]} {reader["Age"]}");
                    pos++;
                }
            }
            
            return output.ToString().TrimEnd();
        }
    }
}
