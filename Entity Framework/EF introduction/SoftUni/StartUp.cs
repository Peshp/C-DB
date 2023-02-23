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
            Console.WriteLine(GetEmployeesFullInformation(new SoftUniContext()));
        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees.ToList();
            foreach (var e in employees.OrderBy(x => x.EmployeeId))
            {
                sb.AppendLine($"{e.FirstName} {e.MiddleName} {e.LastName} {e.JobTitle}" +
                    $"{e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
