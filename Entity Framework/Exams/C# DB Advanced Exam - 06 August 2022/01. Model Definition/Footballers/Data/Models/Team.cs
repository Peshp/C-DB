using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Footballers.Data.Models
{
    public class Team
    {
        public Team()
        {
            this.TeamsFootballers = new HashSet<TeamFootballer>();
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(40)]
        [MinLength(3)]
        [Required]
        [RegularExpression(@"[a-zA-z.\-\d\s]+$"")")]
        public string Name { get; set; }

        [MaxLength(40)]
        [MinLength(2)]
        [Required]
        public string Nationality { get; set; }

        [Required]
        public int Trophies { get; set; }
        public virtual ICollection<TeamFootballer> TeamsFootballers { get; set; }
    }
}
