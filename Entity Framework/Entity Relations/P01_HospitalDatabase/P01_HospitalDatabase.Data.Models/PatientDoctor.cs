using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_HospitalDatabase.Data.Models
{
    public class PatientDoctor
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public virtual Patient Patiant { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}
