using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class load : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        SaveSetting data = SaveLoadSet.LoadSettings(); //Получение данных
        if (!data.Equals(null)) //Если данные есть
		{

		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
