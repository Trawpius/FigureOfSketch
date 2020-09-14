using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FigureOfSketch.Objects
{
    [Serializable]
    public class Configuration
    {
        [XmlElementAttribute]
        public string Directory { get; set; }
    }
}
