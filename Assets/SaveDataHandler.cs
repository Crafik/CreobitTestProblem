using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public SaveDataHandler(string dataDirPath, string dataFileName){
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public string Load(){
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        string loadedData = "";
        if (File.Exists(fullPath)){
            try{
                using (FileStream stream = new FileStream(fullPath, FileMode.Open)){
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadedData = reader.ReadToEnd();
                    }
                }
            }
            catch(Exception e){
                Debug.LogError("Error has occured while loading data from file: " + fullPath + '\n' + e);
            }
        }
        return loadedData;
    }

    public void Save(string saveData){
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try{
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (FileStream stream = new FileStream(fullPath, FileMode.Create)){
                using (StreamWriter writer = new StreamWriter(stream)){
                    writer.Write(saveData);
                }
            }
        }
        catch(Exception e){
            Debug.LogError("Error has occured while saving data to file: " + fullPath + '\n' + e);
        }
    }
}
