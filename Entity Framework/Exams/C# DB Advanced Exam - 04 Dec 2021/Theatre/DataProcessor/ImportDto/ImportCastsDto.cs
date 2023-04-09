using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Cast")]
    public class ImportCastsDto
    {
        [MinLength(4)]
        [MaxLength(30)]
        [Required]
        [XmlElement("FullName")]
        public string FullName { get; set; }

        [Required]
        public bool IsMainCharacter { get; set; }

        [RegularExpression(@"\+[0-9]{2}\-[0-9]{2}\-[0-9]{3}\-[0-9]{4}")]
        [Required]
        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Required]
        [XmlElement("PlayId")]
        public int PlayId { get; set; }
    }
}
