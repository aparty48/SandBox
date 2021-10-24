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
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void saveSetBtn()
	{
        
	}
}
[System.Serializable] //Обязательно нужно указать, что класс должен сериализоваться
public class SaveSetting
{
    public SaveSetting(setting seting)
	{

	}
}
public static class SaveLoadSet
{
    private static string path = Application.persistentDataPath + "/settings.sndS"; //Путь к сохранению. Вы можете использовать любое расширение
    private static BinaryFormatter formatter = new BinaryFormatter(); //Создание сериализатора 
    public static void SaveSettings(setting seting)
	{
        FileStream fs = new FileStream(path, FileMode.Create); //Создание файлового потока

        SaveSetting data = new SaveSetting(seting); //Получение данных

        formatter.Serialize(fs, data); //Сериализация данных

        fs.Close(); //Закрытие потока
    }
    public static SaveSetting LoadSettings()
	{
        if (File.Exists(path))
        { //Проверка существования файла сохранения
            FileStream fs = new FileStream(path, FileMode.Open); //Открытие потока

            SaveSetting data = formatter.Deserialize(fs) as SaveSetting; //Получение данных

            fs.Close(); //Закрытие потока

            return data; //Возвращение данных
        }
        else
        {
            return null; //Если файл не существует, будет возвращено null
        }

    }

}