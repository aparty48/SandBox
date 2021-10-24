using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventar : MonoBehaviour
{
    public DataBase data;//связь с дата базой предметов
    public UIScript uics;
    public List<ItemInventory> items=new List<ItemInventory>();//список предметов

    public GameObject gameObjShow;//образец иконуи предмета
    //public GameObject gameObjShowHB;

    public GameObject InventoryMainObject;//площядка для предметов
    public GameObject InventoryMainObjectHB;
    public int maxCount;//количество слотов инвентаря

    public Camera cam;//камера
    public EventSystem es;

    public int CurentID;//
    public ItemInventory currentItem;//

    public RectTransform movingObject;//предмет которий будет следить за мишкой
    public Vector3 offset;//отступ от курсора
    public GameObject selectItm;

    public int selectItem;//айди выбраного предмета хотбара
    //public GameObject[] qwe = new GameObject[10];
    public GameObject dropCenter;


    public void Start()
    {
      if(items.Count == 0)//количество предметов == 0
      {
        AddGraphics();//вызов функции при условии
            hotbarupdate();
            //AddGraphicsHotBar();//вызов функции при условии
        }

    for(int i=0; i< maxCount; i++)//цикл по заполнению инвентаря рандомними предметами
    {
      int asd = Random.Range(0, data.items.Count);
      AddItem(i, data.items[asd], Random.Range(1,data.items[asd].stak));//взываем фунцию добавь предмет
    }
    UpdateInventory();//вызываем функцию обновления инвентаря

    }

    public void Update()
    {
        if(CurentID != -1)
        {
        MoveObject();//вызываем фунцию при условии
        }
        itemcount();
        UpdateInventory();//вызываем фунцию обновления инвентаря
        updateHB();
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if (items[selectItem].count > 0)
            {
                items[si].count -= 1;
                for (int i = 0; i < data.lengthListBlock; i++)
                {
                    if (data.blocks[i].idItemBlock == selectItem)
                    {
                        itemTr(data.items[i], 1, uics.player.transform.position + uics.player.transform.forward * 0.7f, true);
                        Debug.Log(data.items[i].id);
                    }
                }
            }
        }
//UpdateHB();
    }
    public void itemcount()
	{

    }
    int contr=0;
    public void dropItem(Item item, int count, GameObject obj)
  	{
        bool tryu = SearchForSameItem(item,count);
        if(tryu)
        {
          Destroy(obj);
        }
        else
        {
          itemTr(item,contr,uics.player.transform.position + new Vector3(0,1,0)+ uics.player.transform.forward*0.7f,true);
        }
  	}
    public void itemTr(Item item,int count, Vector3 pos,bool bros)
    {
        GameObject drop1 = Instantiate(data.blocks[item.id].prefabBlock, pos, new Quaternion(0, 0, 0, 0), dropCenter.transform);
        drop1.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
        Drop op = drop1.AddComponent<Drop>();
        Rigidbody _rb = drop1.AddComponent<Rigidbody>();
        BoxCollider col = drop1.AddComponent<BoxCollider>();
        col.isTrigger = true;
        op.invntr = cam.GetComponent<Inventar>();
        op.itm = data;
        op.idI = data.blocks[item.id].idItemBlock;
        op.countI = count;
        if(bros)
        {
          _rb.AddForce(cam.transform.forward*150f,ForceMode.Force);
        }
        Debug.Log(item.id);
    }
    public bool SearchForSameItem(Item item, int count)//фунция поиска доступной ячеики для предмета
    {
      for(int i = 0; i < maxCount; i++)//цыкл проверяет каждую ячеику
      {
        if(items[i].id == item.id)//если совпадают выбраный предемет и предмет со списка
        {
          if(items[i].count < item.stak)//если ячеика меньше 200 предметов
          {
            items[i].count += count;//выполняем сложение предметов

            if(items[i].count > item.stak)//если предмети в ячеике превышают 200 предметов
            {
              count = items[i].count - item.stak;//количество равняеиться количеству предметов и минус 200
              items[i].count = item.stak;//количество предметов равняеться 100
              //return true;
            }
            else
            {
              count = 0;//количество равно 0
              i = maxCount;//к переменой цикла  присваиваеться максю слотов инвенаря
              return true;
            }
          }
        }
      }

     if(count>0)//количество больше 0
     {
       for(int i = 0; i < maxCount; i ++)//цыкл
       {
         if(items[i].id == 0)//если в ячеике айди предмета равно 0 или пустая
         {
           AddItem(i, item, count);//добавляем предмет в пустую ячеику
           i= maxCount;//к переменой цикла  присваиваеться максю слотов инвенаря
           return true;
         }
       }
       contr = count;
       return false;
     }
     return false;
    }


    public void AddItem(int id, Item item, int count)//функция добавления предмета
    {

              items[id].id = item.id;//присваиваем к айди в списке айди переданого предмета
              items[id].count = count;//присваиваем к количеству в списке количество переданого предмета
              items[id].itemGameObj.GetComponent<Image>().sprite = item.img;//иконка предмета


          if(count > 1 && item.id != 0)//если количество больше 1 и айди предмета не равняеться 0
          {
            items[id].itemGameObj.GetComponentInChildren<Text>().text = count.ToString();//вместо имени предмета присваиваем количество
          }
          else
          {
            items[id].itemGameObj.GetComponentInChildren<Text>().text = "";//вместо имени предмета присваиваем ничего
          }
    }
    public void AddInventoryItem(int id, ItemInventory invItem)//функция добавления предмета в инвентарь
    {
          items[id].id = invItem.id;//присваиваем к айди в списке айди переданого предмета
          items[id].count = invItem.count;//присваиваем к количеству в списке количество переданого предмета
          items[id].itemGameObj.GetComponent<Image>().sprite = data.items[invItem.id].img;//иконка предмета

          if(invItem.count > 1 && invItem.id != 0)//если количество больше 1 и айди преданого предмета не равняеться 0
          {
            items[id].itemGameObj.GetComponentInChildren<Text>().text = invItem.count.ToString();//вместо имени предмета присваиваем количество
          }
          else
          {
            items[id].itemGameObj.GetComponentInChildren<Text>().text = "";//вместо имени предмета присваиваем ничего
          }
    }

    public void AddGraphics()//функция отрисовки предметов
    {
        for(int i = 0; i < maxCount; i++)//цикл для отрисовки каждого предмет
        {
            GameObject newItem = Instantiate(gameObjShow, InventoryMainObject.transform) as GameObject;//вставляем предмет в слот инвентаря

            newItem.name = i.ToString();//вместо имени предмета присваиваем количество

            ItemInventory ii = new ItemInventory();
            ii.itemGameObj = newItem;//присваиваем отрисований предмет

            RectTransform rt = newItem.GetComponent<RectTransform>();//получаем графические своиства предмета
            rt.localPosition = new Vector3(0, 0, 0);//локальная позиция предмета по нулям
            rt.localScale = new Vector3(1, 1, 1);//размер стандартный
            newItem.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);//размер стандартный

            Button tempButton = newItem.GetComponent<Button>();//каждий предмет это кнопка

            tempButton.onClick.AddListener(delegate { SelectObject();});//если нажали на предмет вызываем функцию выбора предмета

            items.Add(ii);//добавляем предмет
        }
    }


    public void UpdateInventory()//фунция обновления инвентаря
    {
      for(int i = 0; i<maxCount; i++)//цыкл для обновления каждой ячеики
      {
        if(items[i].id != 0 && items[i].count >=1)//айди предмета в ячеике не равняеться 0 и количество предметов в ячеике больше 1
        {
          items[i].itemGameObj.GetComponentInChildren<Text>().text = items[i].count.ToString();//вместо текста пишем количество
        }
        else
        {
          items[i].itemGameObj.GetComponentInChildren<Text>().text = "";//вместо текста пишем ничего
          items[i].id=0;
          items[i].itemGameObj.GetComponent<Image>().sprite = data.items[0].img;
        }
        items[i].itemGameObj.GetComponent<Image>().sprite = data.items[items[i].id].img;//устанавливаем картинку пердмета
      }

      //if(movingObject.gameObject.activeSelf==true){}else{selectItem=0;}
    }



    public void SelectObject()//функция выбраного предмета
    {

        ItemInventory IIa = items[int.Parse(es.currentSelectedGameObject.name)];//получаем предмет из списка под номером имени кликнутого предмета
        //selectItem=IIa.id;

        if(CurentID == -1)//айди текущего  предмета равняеться -1
        {
            CurentID = int.Parse(es.currentSelectedGameObject.name);//текущей предмет равняеться имени текущего предмета
            currentItem = CopyInventoryItem(items[CurentID]);//текущий предмет равняеться предмету из списка под текущим айди
            movingObject.gameObject.SetActive(true);//активируем предмет следующий за курсором
            movingObject.GetComponent<Image>().sprite = data.items[currentItem.id].img;//присваиваем  предмету следуещего за мишкой картинкувибраного предмета

            AddItem(CurentID, data.items[0], 0);//очищяем ячеику на которую нажали
        }
        else
        {

            ItemInventory II = items[int.Parse(es.currentSelectedGameObject.name)];//получаем предмет из списка под номером имени кликнутого предмета

            if(currentItem.id != II.id)//айди текущего предмета не равняеться айди кликнутого предмета
            {
                AddInventoryItem(CurentID, II);//добавляем предмет в инвентарь кликнутий предмет
                AddInventoryItem(int.Parse(es.currentSelectedGameObject.name), currentItem);//добавляем предмет в инвентарь текущий предмет с айди кликнутого предмета
            }
            else
            {
                if(II.count + currentItem.count <=data.items[II.id].stak)//сума количества пелметов кликнутого предмета и количество выбраного предмета меньше или навно 200
                {
                    II.count +=currentItem.count;//сумируем количества пелметов кликнутого предмета и количество выбраного предмета
                }
                else
                {
                    int lcount = 0;
                    AddItem(CurentID, data.items[II.id], II.count + currentItem.count - data.items[II.id].stak);//добавляем предмет в новую ячеику суму количества пелметов кликнутого предмета и количество выбраного предмета минус 200
                    II.count = data.items[II.id].stak;//количество кликнутого предмета равно 200
                }
                II.itemGameObj.GetComponentInChildren<Text>().text = II.count.ToString();//обновляем текст количества предметов
            }
            CurentID= -1;//текущий айди равняеться -1
            movingObject.gameObject.SetActive(false);//выключаем обект следующий за курсором
        }
    }


    public void MoveObject()//предмет следуещий за крсором
    {
      Vector3 pos = Input.mousePosition + offset;//получаем позицию курсора плюс смещение предмета
      pos.z = InventoryMainObject.GetComponent<RectTransform>().position.z;//позиция пердмета следуещего за курсосром равняеться суме позиции курсора и смещения
      movingObject.position = cam.ScreenToWorldPoint(pos);//позиция пердмета следуещего за курсосром равняеться суме позиции курсора и смещения
    }

    public ItemInventory CopyInventoryItem(ItemInventory old)//копирование предмета
    {
      ItemInventory New =new ItemInventory();//новый предмет

      New.id = old.id;
      New.itemGameObj = old.itemGameObj;
      New.count = old.count;

      return New;
    }



    public GameObject[] hbi = new GameObject[10];
    public int si = 0;
    public void hotbarupdate()
	{
        for(int i = 0; i<10; i++)
		{
            //GameObject newItem = Instantiate(gameObjShow, InventoryMainObjectHB.transform) as GameObject;//вставляем предмет в слот инвентаря
            hbi[i]= Instantiate(gameObjShow, InventoryMainObjectHB.transform) as GameObject;

            hbi[i].name = i.ToString();//вместо имени предмета присваиваем количество

            ItemInventory ii = new ItemInventory();
            ii.itemGameObj = hbi[i];//присваиваем отрисований предмет

            RectTransform rt = hbi[i].GetComponent<RectTransform>();//получаем графические своиства предмета
            rt.localPosition = new Vector3(0, 0, 0);//локальная позиция предмета по нулям
            rt.localScale = new Vector3(1, 1, 1);//размер стандартный
            hbi[i].GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);//размер стандартный

            Button tempButton = hbi[i].GetComponent<Button>();//каждий предмет это кнопка

            tempButton.onClick.AddListener(delegate { SelectObjectHB(); });
        }
    }
    public void SelectObjectHB()
	{
        if(true)
		{
           selectItem = int.Parse(es.currentSelectedGameObject.name);
		}




    }
    public void updateHB()
	{
        for (int i = 0; i < 10; i++)//цыкл для обновления каждой ячеики
        {
            if (items[i].id != 0 && items[i].count > 1)//айди предмета в ячеике не равняеться 0 и количество предметов в ячеике больше 1
            {
                hbi[i].GetComponentInChildren<Text>().text = items[i].count.ToString();//вместо текста пишем количество
            }
            else
            {
                hbi[i].GetComponentInChildren<Text>().text = "";//вместо текста пишем ничего
            }
            hbi[i].GetComponent<Image>().sprite = data.items[items[i].id].img;//устанавливаем картинку пердмета
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            si++;
            selectItem = si;
            //Debug.Log(si);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            si--;
            selectItem = si;
            ///Debug.Log(si);
        }
        if (si > 9)
        {
            si = 0;
        }
        if (si < 0)
        {
            si = 9;
        }

        //Debug.Log(si);
    }
}
[System.Serializable]

public class ItemInventory
{
    public int id;//ади предмета
    public GameObject itemGameObj;//

    public int count;//количество
}
