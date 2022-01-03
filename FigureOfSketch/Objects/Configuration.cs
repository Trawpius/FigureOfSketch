using System;
using System.IO;
using System.Xml.Serialization;

namespace FigureOfSketch.Objects
{
    [Serializable]
    public class Configuration
    {

        static Configuration()
        {
            Current = XmlSerializeExtension.DeserializeObject<Configuration>("Config.xml");
        }

        public static Configuration Current { get; private set; }

        [XmlElement]
        public string Directory { get; set; }

        public static void Save()
        {
            XmlSerializeExtension.SerializeObject<Configuration>(Current, "Config.xml");
        }
    }

    public static class XmlSerializeExtension
    {
        public static T DeserializeObject<T>(string config)
        {
            T configuration;

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(config);

            configuration = (T)serializer.Deserialize(reader);

            reader.Close();

            return configuration;
        }

        public static void SerializeObject<T>(this T value, string config)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamWriter writer = new StreamWriter(config);
            serializer.Serialize(writer, value);
            writer.Close();
        }
    }
}
