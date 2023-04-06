using Footballers.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Footballers.Data.Models
{
    public class Footballer
    {
        public Footballer() 
        {
            this.TeamsFootballers = new HashSet<TeamFootballer>();
        }

        [Key]
        [Required]
        public int Id { get; set; }

        [MaxLength(40)]
        [MinLength(2)]
        [Required]
        public string Name { get; set; }

        [Required] 
        public DateTime ContractStartDate { get; set; }

        [Required]
        public DateTime ContractEndDate { get; set; }

        [Required]
        public PositionType PositionType { get; set; }

        [Required]
        public BestSkillType BestSkillType { get; set; }

        [Required]
        [ForeignKey(nameof(Coach))]
        public int CoachId { get; set; }
        public virtual Coach Coach { get; set; }
        public virtual ICollection<TeamFootballer> TeamsFootballers { get; set; }
    }
}
