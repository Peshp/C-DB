using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ExportDto
{
    [XmlType("User")]
    public class ExportPlaysDto
    {
        [XmlElement("Title")]
        public string Title { get; set; }

        [XmlElement("Duration")]
        public string Duration { get; set; }

        [XmlElement("Rating")]
        public string Rating { get; set; }

        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlArray("Actors")]
        public ExportActorsDto[] Actors { get; set; }
    }
    [XmlType("Actor")]
    public class ExportActorsDto
    {
        [XmlElement("FullName")]
        public string FullName { get; set; }

        [XmlElement("MainCharacter")]
        public string MainCharacter { get; set; }
    }
}
