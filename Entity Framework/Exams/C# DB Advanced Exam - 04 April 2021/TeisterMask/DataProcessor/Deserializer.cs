// ReSharper disable InconsistentNaming

namespace TeisterMask.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Text;
    using System.Xml.Serialization;
    using TeisterMask.DataProcessor.ImportDto;
    using System.Globalization;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProjectDto[]),
                new XmlRootAttribute("Projects"));
            var projects = (ImportProjectDto[])xmlSerializer.Deserialize(new StringReader(xmlString))!;

            foreach (var xmlProject in projects)
            {
                if(!IsValid(xmlProject))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                bool isValidOpenDate = DateTime.TryParseExact(xmlProject.OpenDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validOpenDate);
                bool isValidDueDate = DateTime.TryParseExact(xmlProject.DueDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validDueDate);

                if(!String.IsNullOrEmpty(xmlProject.DueDate))
                {
                    if(!isValidOpenDate || !isValidDueDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }                   
                }
                
                Project project = new Project
                {
                    Name = xmlProject.Name,
                    OpenDate = validOpenDate,
                    DueDate = validDueDate,
                };

                foreach (var xmlTask in xmlProject.Tasks)
                {
                    if (!IsValid(xmlTask))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isValidTaskOpenDate = DateTime.TryParseExact(xmlTask.OpenDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validTaskOpenDate);
                    bool isValidTaskDueDate = DateTime.TryParseExact(xmlTask.DueDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validTaskDueDate);

                    if (!String.IsNullOrEmpty(xmlTask.DueDate))
                    {
                        if(!isValidTaskOpenDate || !isValidTaskDueDate)
                        {
                            output.AppendLine(ErrorMessage);
                            continue;
                        }                       
                    }
                    
                    Task task = new Task
                    {
                        Name = xmlTask.Name,
                        OpenDate = validTaskOpenDate,
                        DueDate = validTaskDueDate,
                        ExecutionType = (ExecutionType)xmlTask.ExecutionType,
                        LabelType = (LabelType)xmlTask.LabelType,
                        Project = project,
                    };

                    project.Tasks.Add(task);
                }
                context.Projects.Add(project);
                output.AppendLine(String.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
            }
            context.SaveChanges();
            return output.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder output = new StringBuilder();
            var employees = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);

            foreach (var jsonEmployee in employees)
            {
                if(!IsValid(jsonEmployee))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Employee employee = new Employee
                {
                    Username = jsonEmployee.Username,
                    Email = jsonEmployee.Email,
                    Phone = jsonEmployee.Phone,
                };

                int[] validTaskIds = context.Tasks.Select(t => t.Id).ToArray();

                foreach (var jsonTask in jsonEmployee.Tasks.Distinct())
                {
                    if(!validTaskIds.Contains(jsonTask))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    employee.EmployeesTasks.Add(new EmployeeTask
                    {
                        TaskId = jsonTask,
                        EmployeeId = employee.Id,
                    });
                }

                context.Employees.Add(employee);
                output.AppendLine(String.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count()));
            }
            context.SaveChanges();
            return output.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}