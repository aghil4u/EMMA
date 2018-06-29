using System;
using System.IO;
using System.Reflection;

namespace EMMA.Helper_Classes
{
    internal class StorageSystem
    {
        public static readonly string ApplicationPath =
            new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase))
                .LocalPath;


        public static bool Store(DB dataDb)
        {
            // XmlSerializer serializer = new XmlSerializer(typeof(Db));
            // TextWriter textWriter = new StreamWriter(ApplicationPath + "\\libdata.xml");
            // serializer.Serialize(textWriter, songs);
            // textWriter.Close();
            //Debug.Print("Media Library Created successfully");
            using (var file = File.Create(ApplicationPath + "\\libdata.xml"))
            {
               // Serializer.Serialize<DB>(file, dataDb);
            }
            return true;
        }

        public static DB Read()
        {
            //XmlSerializer deserializer = new XmlSerializer(typeof(Db));
            //TextReader textReader = new StreamReader(ApplicationPath + "\\libdata.xml");
            //Db db = (Db)deserializer.Deserialize(textReader);
            // textReader.Close();
            DB db = null;
            if (File.Exists(ApplicationPath + "\\libdata.xml"))
            {
                using (var file = File.OpenRead(ApplicationPath + "\\libdata.xml"))
                {
                   // db = Serializer.Deserialize<DB>(file);
                }
            }
            return db;
        }
    }
}