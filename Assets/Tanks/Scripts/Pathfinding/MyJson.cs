using System;
using System.Collections.Generic;
using System.IO;

namespace WrapperOnFilesAndJson
{
    public abstract class BaseFiles 
    {

        public static string ReturnPathToFolder(string pathToFolder, bool CreateDir = true)
        {
            string path = Directory.GetCurrentDirectory();
            path = System.IO.Path.Combine(path, pathToFolder);
            if (!Directory.Exists(path)) 
            {
                if (CreateDir) Directory.CreateDirectory(path);
                else
                {
                    UnityEngine.Debug.LogError("Directory doesn't exist");
                    return null;
                }
            }
            
            return path.Replace('/', @"\"[0]);
        }
        public static string ReturnPathToFolder(string pathToJsonsFolder, string subFolderName, string nameOfFile, bool CreateDir = true)
        {
            string path = Directory.GetCurrentDirectory();
            path = System.IO.Path.Combine(path, pathToJsonsFolder, subFolderName);

            if (!Directory.Exists(path))
            {
                if (CreateDir) Directory.CreateDirectory(path);
                else
                {
                    UnityEngine.Debug.LogError("Directory doesn't exist");
                    return null;
                }
            }
            path = System.IO.Path.Combine(path, nameOfFile);

            return path.Replace('/', @"\"[0]);
        }

    }
    public class MyFiles : BaseFiles
    {
        #region UnityColorSerializatio
        struct Colour
        {
            public Colour(float r, float g, float b, float a)
            {
                this.r = r; this.g = g; this.b = b; this.a = a;
            }

            public float r;
            public float g;
            public float b;
            public float a;
        }
        public static string SerializeUnityColor(List<UnityEngine.Color> oldColors)
        {
            if (oldColors == null) return null;

            List<Colour> newColors = new List<Colour>();
            foreach (UnityEngine.Color col in oldColors)
            {
                newColors.Add(new Colour(col.r, col.g, col.b, col.a));
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(newColors);
        }
        public static List<UnityEngine.Color> DeserializeUnityColor(string serialized)
        {
            if (serialized.Equals(null)) return null;

            List<UnityEngine.Color> newColors = new List<UnityEngine.Color>();
            List<Colour> oldColors = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Colour>>(serialized);
            foreach (Colour col in oldColors)
            {
                newColors.Add(new UnityEngine.Color(col.r, col.g, col.b, col.a));
            }
            return newColors;
        }
        #endregion

        public static string[] GetFiles(string pathToFolder, string searchPattern = null, SearchOption searchOption = SearchOption.AllDirectories)
        {
            string path = ReturnPathToFolder(pathToFolder, false);
            if (path == null) return null;            

            return Directory.GetFiles(path, searchPattern, searchOption);
        }
    }
    public class MyJson : BaseFiles
    {
        public static void WriteJsonAtJsonsFolder(object Object, string pathToJsonsFolder, string levelName, string nameOfFile)
        {
            WriteJsonAtJsonsFolder(Newtonsoft.Json.JsonConvert.SerializeObject(Object), pathToJsonsFolder, levelName, nameOfFile);
        }
        public static void WriteJsonAtJsonsFolder<T>(T Object, string pathToJsonsFolder, string levelName, string nameOfFile)
        {
            WriteJsonAtJsonsFolder(Newtonsoft.Json.JsonConvert.SerializeObject(Object), pathToJsonsFolder, levelName, nameOfFile);
        }
        public static void WriteJsonAtJsonsFolder(string serialized, string pathToJsonsFolder, string levelName, string nameOfFile)
        {
            string path = ReturnPathToFolder(pathToJsonsFolder, levelName, nameOfFile);

            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(serialized);
            }
        }
        
        public static string ReadFile(string pathToJsonsFolder, string subFolderName = "", string nameFile = "")
        {
            string path = ReturnPathToFolder(pathToJsonsFolder, subFolderName, nameFile);
            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        return sr.ReadToEnd();
                    }
                }
                catch (IOException e)
                {
                    UnityEngine.Debug.LogError("The file could not be read: " + e.Message);
                    return null;
                }
            }
            else return null;
        }
        public static T ReadAndDeserializeFile<T>(string pathToJsonsFolder, string subFolderName = "", string nameFile = "")
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(ReadFile(pathToJsonsFolder, subFolderName, nameFile));
        }
    }
}
