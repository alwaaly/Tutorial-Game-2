using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {
    public static void Save() {
        Data data = Load();
        if (data != null) {
            if(LevelManager.Instance != null) {
                if (data.MaxProgress < LevelManager.Instance.mainCharctere.wallPassed)
                    data.MaxProgress = LevelManager.Instance.mainCharctere.wallPassed;
            }
        }else data =new(0,0,0);

        string path = Application.persistentDataPath + "/Data.bi";
        FileStream stream = new FileStream(path, FileMode.Create);
        BinaryFormatter formatter = new();
        formatter.Serialize(stream, new Data(data.MaxProgress,UIManager.Instance.GetSoundEffectVolum(), UIManager.Instance.GetMusicVolum()));
        stream.Close();
    }
    public static Data Load() {
        string path = Application.persistentDataPath + "/Data.bi";
        if (File.Exists(path)) {
            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new();
            Data data = (Data)formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        else {
            return null;
        }
    }
}
[System.Serializable]
public class Data {
    public int MaxProgress;
    public float SoundEffectVolum;
    public float MusicVolum;
    public Data(int maxProgress, float soundEffectVolum, float musicVolum) {
        MaxProgress = maxProgress;
        SoundEffectVolum = soundEffectVolum;
        MusicVolum = musicVolum;
    }
}
