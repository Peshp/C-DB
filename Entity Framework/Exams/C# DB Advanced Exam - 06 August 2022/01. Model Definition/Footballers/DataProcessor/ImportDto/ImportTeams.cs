using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Footballers.DataProcessor.ImportDto
{
    public class ImportTeams
    {
        [MinLength(3)]
        [MaxLength(40)]      
        [Required]
        [RegularExpression(@"[a-zA-z.\-\d\s]+$")]
        public string Name { get; set; }

        [MinLength(2)]
        [MaxLength(40)]      
        [Required]
        public string Nationality { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Trophies { get; set; }
        public int[] Footballers { get; set; }
    }   
}
