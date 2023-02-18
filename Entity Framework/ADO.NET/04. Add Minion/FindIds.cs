using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace _04._Add_Minion
{
    public class FindIds
    {
        private string townName;
        private int minionAge;
        private string minionName;
        private string villainName;
        public FindIds(string townName, int minionAge, string minionName, string villainName)
        {
            this.townName = townName;
            this.minionAge = minionAge;
            this.minionName = minionName;
            this.villainName = villainName;
        }
        public object TownIdFinder(string name, SqlConnection sql)
        {
            string nameQuery = @"SELECT Id FROM Towns WHERE Name = @name";
            SqlCommand nameCmd = new SqlCommand(nameQuery, sql);
            nameCmd.Parameters.AddWithValue("@name", name);
            return nameCmd.ExecuteScalar();
        }
        public object VillainIdFinder(string name, SqlConnection sql)
        {
            string nameQuery = @"SELECT Id FROM Villains WHERE Name = @Name";
            SqlCommand nameCmd = new SqlCommand(nameQuery, sql);
            nameCmd.Parameters.AddWithValue("@name", name);
            return nameCmd.ExecuteScalar();
        }
        public object MinionIdFinder(string name, SqlConnection sql)
        {
            string nameQuery = @"SELECT Id FROM Minions WHERE Name = @Name";
            SqlCommand nameCmd = new SqlCommand(nameQuery, sql);
            nameCmd.Parameters.AddWithValue("@name", name);
            return nameCmd.ExecuteScalar();
        }
    }
}
