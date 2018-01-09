using System.IO;
using System.Web.Script.Serialization;

namespace CheckServerIsBusy.Model
{
    public class Settings<T> where T : new()
    {
        // TODO: Replace to end user's required path
        /// <summary>
        /// Path, where settings will be
        /// </summary>
        private const string DefaultFileName = @"C:\settings.json";

        public void Save(string fileName = DefaultFileName)
        {
            File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(this));
        }

        public static void Save(T whatToSerialize, string fileName = DefaultFileName)
        {
            File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(whatToSerialize));
        }

        public static T Load(string fileName = DefaultFileName)
        {
            T t = new T();
            if (File.Exists(fileName))
            {
                t = (new JavaScriptSerializer()).Deserialize<T>(File.ReadAllText(fileName));
            }
            return t;
        }
    }
}
