using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; //Работа с аудио
using System.IO; //Библиотек для работы с файлами
using System.Runtime.Serialization.Formatters.Binary; //Библиотека для работы бинарной сериализацией



public class setting : MonoBehaviour
{
    //UIScript uiset = new UIScript();
    //float volumeS = uiset.volume;
    // Start is called before the first frame update
    private void Start()
    {

    }
    // Update is called once per frame
    private void Update()
    {

    }
    private void saveSetBtn()
  	{

  	}
    public float Chustvitelnost
    {
        set
        {
            SaveSetting.chustvitelnost = value;
        }
        get
        {
            return SaveSetting.chustvitelnost;
        }
    }
}

[System.Serializable] //Обязательно нужно указать, что класс должен сериализоваться
public class SaveSetting
{
    public static float chustvitelnost;
}

public static class SaveLoadSet
{
    private static string path = Application.persistentDataPath + "/settings.sndS"; //Путь к сохранению. Вы можете использовать любое расширение
    public static void Save()
    {

    }
    public static void Load()
    {

    }

}
