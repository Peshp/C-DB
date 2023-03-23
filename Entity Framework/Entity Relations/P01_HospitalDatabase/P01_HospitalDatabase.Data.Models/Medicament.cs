using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_HospitalDatabase.Data.Models
{
    public class Medicament
    {
        public Medicament() 
        { 
            this.PatientMedicaments= new HashSet<PatientMedicament>(); 
        }
        [Key]
        public int MedicamentId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public ICollection<PatientMedicament> PatientMedicaments { get; set; }
    }
}
