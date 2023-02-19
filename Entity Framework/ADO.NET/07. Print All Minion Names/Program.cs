using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace _07._Print_All_Minion_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            using SqlConnection sql = new SqlConnection(Config.Sqlconnection);

            sql.Open();
            Console.WriteLine(PrintMinionNames(sql)); 
            sql.Close();
        }

        public static string PrintMinionNames(SqlConnection sql)
        {
            string minionNamesQuery = @"SELECT Name FROM Minions";
            SqlCommand minionNamesCmd = new SqlCommand(minionNamesQuery, sql);
            SqlDataReader reader = minionNamesCmd.ExecuteReader();

            var queue = new Queue<string>();
            var stack = new Stack<string>();

            while (reader.Read())
            {
                queue.Enqueue(reader["Name"].ToString());
                stack.Push(reader["Name"].ToString());
            }

            return PrintName(queue, stack);
        }

        private static string PrintName(Queue<string> queue, Stack<string> stack)
        {
            StringBuilder sb = new StringBuilder();
            while(queue.Count > 6 && stack.Count > 5)
            {
                sb.AppendLine(queue.Dequeue());
                sb.AppendLine(stack.Pop());
            }
            return sb.ToString().TrimEnd();
        }
    }
}
