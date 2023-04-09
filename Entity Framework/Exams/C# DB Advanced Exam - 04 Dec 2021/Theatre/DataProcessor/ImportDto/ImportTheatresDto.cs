using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theatre.DataProcessor.ImportDto
{
    public class ImportTheatresDto
    {
        [MinLength(4)]
        [MaxLength(30)]
        [Required]
        public string Name { get; set; }

        [Range(1, 10)]
        [Required]
        public sbyte NumberOfHalls { get; set; }

        [MinLength(4)]
        [MaxLength(30)]
        [Required]
        public string Director { get; set; }
        public ImportZTicketsDto[] Tickets { get; set; }
    }
    public class ImportZTicketsDto
    {
        [Range(1.00, 100.00)]
        [Required]
        public decimal Price { get; set; }

        [Range(1, 10)]
        [Required]
        public sbyte RowNumber { get; set; }

        [Required]
        public int PlayId { get; set; }
    }
}
