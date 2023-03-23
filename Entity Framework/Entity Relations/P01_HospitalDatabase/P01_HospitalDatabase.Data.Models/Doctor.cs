using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_HospitalDatabase.Data.Models
{
    public class Doctor
    {
        public Doctor() 
        {
            this.PatientDoctors = new HashSet<PatientDoctor>();
        }
        [Key]
        public int DoctorId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Specialty { get; set; }

        public ICollection<PatientDoctor> PatientDoctors { get; set; }
    }
}
