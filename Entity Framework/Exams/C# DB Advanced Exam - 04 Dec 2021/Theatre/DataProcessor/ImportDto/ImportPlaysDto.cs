using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Play")]
    public class ImportPlaysDto
    {
        [MinLength(4)]
        [MaxLength(50)]
        [Required]
        [XmlElement("Title")]
        public string Title { get; set; }

        [Required]
        [XmlElement("Duration")]
        public string Duration { get; set; }

        [Range(0.0, 10.0)]
        [Required]
        [XmlElement("Raiting")]
        public float Rating { get; set; }

        [Required]     
        [XmlElement("Genre")]
        public string Genre { get; set; }

        [MaxLength(700)]
        [Required]
        [XmlElement("Description")]
        public string Description { get; set; }

        [MinLength(4)]
        [MaxLength(30)]
        [Required]
        public string Screenwriter { get; set; }
    }
}
