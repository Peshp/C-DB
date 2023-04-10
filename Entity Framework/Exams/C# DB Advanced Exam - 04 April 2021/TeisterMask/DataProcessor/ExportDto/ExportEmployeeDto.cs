using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeisterMask.DataProcessor.ExportDto
{
    public class ExportEmployeeDto
    {
        public string Username { get; set; }
        public ExportTasksDto[] Tasks { get; set; }
    }
    public class ExportTasksDto
    {
        public string TaskName { get; set; }
        public string OpenDate { get; set; }
        public string DueDate { get; set; }
        public string LabelType { get; set; }
        public string ExecutionType { get; set; }
    }
}
