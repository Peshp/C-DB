using SoftUni.Data;
using SoftUni.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(GetEmployeesWithSalaryOver50000(new SoftUniContext()));
        }       
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employess = context.Employees.ToList();
            foreach (var e in employess.Where(x => x.Salary > 50000)
                .OrderBy(e => e.FirstName))
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
