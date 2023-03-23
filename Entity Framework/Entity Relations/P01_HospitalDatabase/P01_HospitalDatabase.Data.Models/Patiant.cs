using System.ComponentModel.DataAnnotations;

namespace P01_HospitalDatabase.Data.Models
{
    public class Patient
    {
        public Patient() 
        { 
            this.PatientDoctors= new HashSet<PatientDoctor>();
            this.PatientMedicaments = new HashSet<PatientMedicament>();
        }
        [Key]
        public int PatiantId { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        [MaxLength(80)]
        public string Email { get; set; }

        public bool HasInsurance { get; set; }

        public ICollection<PatientDoctor> PatientDoctors { get; set; }
        public ICollection<PatientMedicament> PatientMedicaments { get; set; }
    }
}