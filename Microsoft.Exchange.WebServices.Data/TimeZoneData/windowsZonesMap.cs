
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0

using System.Xml.Serialization;
using System.Collections.Generic;


// namespace RedmineMailService.Code
namespace Microsoft.Exchange.WebServices.Data.TimeZoneData
{

    [XmlRoot(ElementName = "version")]
    public class Version
    {
        [XmlAttribute(AttributeName = "number")]
        public string Number { get; set; }
    }


    [XmlRoot(ElementName = "mapZone")]
    public class MapZone
    {
        [XmlAttribute(AttributeName = "other")]
        public string Other { get; set; }

        [XmlAttribute(AttributeName = "territory")]
        public string Territory { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }


    [XmlRoot(ElementName = "mapTimezones")]
    public class MapTimezones
    {
        [XmlElement(ElementName = "mapZone")]
        public List<MapZone> MapZone { get; set; }

        [XmlAttribute(AttributeName = "otherVersion")]
        public string OtherVersion { get; set; }

        [XmlAttribute(AttributeName = "typeVersion")]
        public string TypeVersion { get; set; }
    }


    [XmlRoot(ElementName = "windowsZones")]
    public class WindowsZones
    {
        [XmlElement(ElementName = "mapTimezones")]
        public MapTimezones MapTimezones { get; set; }
    }


    [XmlRoot(ElementName = "supplementalData")]
    public class WindowsZonesMap
    {
        [XmlElement(ElementName = "version")]
        public Version Version { get; set; }

        [XmlElement(ElementName = "windowsZones")]
        public WindowsZones WindowsZones { get; set; }


        public static WindowsZonesMap FromEmbeddedRessource(string fileName)
        {
            WindowsZonesMap sd = Serialization.DeserializeXmlFromEmbeddedRessource<WindowsZonesMap>(fileName);
            return sd;
        }

    }

}
