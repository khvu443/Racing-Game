using Assets.Script.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

namespace Assets.Script.Configuration
{
    public class FileDataHandle
    {
        private string dataDirPath = "";
        private string dataFileName = "";

        public FileDataHandle(string dataDirPath, string dataFileName)
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
        }

        public GameData Load()
        {
            //use Path.Combine for different OS's having different path seprators
            string path = Path.Combine(dataDirPath, dataFileName);

            GameData loadData = null;
            if(File.Exists(path))
            {
                try
                {
                    // Load data from the file data
                    string load = "";
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            load = reader.ReadToEnd();
                        }
                    }

                    //Serialize the data json to oject
                    loadData =  JsonConvert.DeserializeObject<GameData>(load);
                }
                catch(Exception ex)
                {
                    UnityEngine.Debug.LogError("Error occured when trying to save data to file " + path + "\n" + ex);
                }
            }
            return loadData;
        }

        public void Save(GameData data)
        {

            //use Path.Combine for different OS's having different path seprators
            string path =  Path.Combine(dataDirPath, dataFileName);
            try
            {
                //Create dir for the file will be written to if it doesn't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                //Serialize the data to json
                string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented);


                //Write data json to the file
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    using(StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
            }
            catch(Exception ex)
            {
                UnityEngine.Debug.LogError("Error occured when trying to save data to file " + path + "\n" + ex);
            }
        }
    }
}
