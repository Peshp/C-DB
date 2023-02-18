using System;
using System.Data.SqlClient;
using System.Text;

namespace _04._Add_Minion
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection sql = new SqlConnection(Config.SqlConnection);

            string[] input = Console.ReadLine().Split();
            string[] input2 = Console.ReadLine().Split();

            sql.Open();
            AddMinionToVillain(input, input2, sql);
            sql.Close();
        }  
        public static void AddMinionToVillain(string[] input, string[] input2, 
            SqlConnection sql)
        {
            FindIds ids = new FindIds(input[1], int.Parse(input[2]), input[3], input2[1]);
            object townId = ids.TownIdFinder(input[3], sql);
            object villainId = ids.VillainIdFinder(input2[1], sql);
            object minionId = ids.MinionIdFinder(input[1], sql);
            IsIdsValid isvalid = new IsIdsValid(townId);

            if (isvalid.isIdValid(townId) == true)
            {
                string addMinionQuery = $"INSERT INTO Minions VALUES ('{input[1]}', {input2}, {townId})";
                SqlCommand addMinionCmd = new SqlCommand(addMinionQuery, sql);
            }
            else
            {
                string addTownQuery = @"INSERT INTO Towns (Name) VALUES (@townName)";
                SqlCommand addTownCmd = new SqlCommand(addTownQuery, sql);
                addTownCmd.Parameters.AddWithValue("@townName", input[3]);
                Console.WriteLine($"Town {input[3]} was added to the database.");
            }

            if(isvalid.isIdValid(villainId) == false)
            {
                string addVillainQuery = $"INSERT INTO Villains (Name, EvilnessFactorId) VALUES ({input2[1]}, 4)";
                SqlCommand addVillainCmd = new SqlCommand(addVillainQuery, sql);
                Console.WriteLine($"Villain {input2[1]} was added to the database.");
            }

            string villainToMinion = $"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES ({minionId}, {villainId})";
            SqlCommand villainToMinionCmd = new SqlCommand(villainToMinion, sql);
            Console.WriteLine($"Successfully added {input[1]} to be minion of {input2[1]}.");
        }
    }
}
