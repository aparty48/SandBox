using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Build : MonoBehaviour
{
    public GameObject worldBuild;
    public GameObject blockBuild;
    public Camera manCam;
    public GameObject plas;
    RaycastHit hit;
    public Inventar inventr;
    public DataBase Dbb;
    public UIScript uisc;
    int blockCount;
    bool moznoStroit = false;
    int tci = 0;

    int idselectblock = 0;


    // Start is called before the first frame update
    void Start()
    {
        blockCount=Dbb.blocks.Count;
        //GeneratorWorldPlock(-50,0,-50,100,100,Dbb.blocks[13].prefabBlock);
        //InvokeRepeating("time",0.0f,0.01f);
    }

    // Update is called once per frame
    void Update()
  	{

		    plas = uisc.player;
        BuildLogic();
    }
    Vector3 posBlockPos(Vector3 a , bool s)
	{
        Ray ray = rayB();
        Ray rayB()
        {
            Vector3 pointSc = new Vector3(manCam.pixelWidth / 2,manCam.pixelHeight / 2, 0);
            return manCam.ScreenPointToRay(pointSc);
            //return new Ray(manCam.transform.position, plas.transform.forward);
        }
        //Vector3 fwd = transform.TransformDirection(Vector3.forward);
        //Debug.Log(ray + " " + hit.point + " " + Physics.Raycast(ray, out hit));
        if (s)
        {
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.transform.position.x - hit.point.x >= 0.49f)
                {
                    Vector3 pos = new Vector3(hit.collider.transform.position.x - 1.0f, hit.collider.transform.position.y, hit.collider.transform.position.z);
                    return pos + a;
                }
                else if (hit.collider.transform.position.x - hit.point.x <= -0.49f)
                {
                    Vector3 pos = new Vector3(hit.collider.transform.position.x + 1.0f, hit.collider.transform.position.y, hit.collider.transform.position.z);
                    return pos + a;
                }
                else if (hit.collider.transform.position.y - hit.point.y >= 0.49f)
                {
                    Vector3 pos = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y - 1.0f, hit.collider.transform.position.z);
                    return pos + a;
                }
                else if (hit.collider.transform.position.y - hit.point.y <= -0.49f)
                {
                    Vector3 pos = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 1.0f, hit.collider.transform.position.z);
                    return pos + a;
                }
                else if (hit.collider.transform.position.z - hit.point.z >= 0.49f)
                {
                    Vector3 pos = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z - 1.0f);
                    return pos + a;
                }
                else if (hit.collider.transform.position.z - hit.point.z <= -0.49f)
                {
                    Vector3 pos = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z + 1.0f);
                    return pos + a;
                }
            }
            return new Vector3(0, 0, 0);
        }
        else
		{
            return new Vector3(0, 0, 0);
		}
    }
    void BuildLogic()
    {
        Debug.DrawLine(plas.transform.position, plas.transform.forward, Color.red);
        if (uisc.nonemenu == true){//если меню не открито
			  if (uisc.PcD)//если пк
            {

                if(Input.GetMouseButtonDown(0))//если левый клик миши
				        {
                //Vector3 ds = posBlockPos(new Vector3(0, 0, 0), false);
                Ray ray = rayB();//полуаем путь луча
                Ray rayB()//метод пути луча
                {
                    Vector3 pointSc = new Vector3(manCam.pixelWidth / 2,manCam.pixelHeight / 2, 0);//получаем точку посередине екрана
                    return manCam.ScreenPointToRay(pointSc);//возращяем путь
                    //return new Ray(manCam.transform.position, plas.transform.forward);
                }
                if (Physics.Raycast(ray, out hit,10f))//пускаем луч и пверям на столкновение
                {
                    if (hit.collider.gameObject != null && hit.collider.gameObject.layer != 9 )//проверка на обьекти
                    {
                      if(inventr.items[inventr.si].id == 14){//если айди выбраного предмета 14
                      string blokname = hit.collider.gameObject.name;//получаем имя обьекта в который попал луч
                      /*for(int i = 0;i<10;i++)//цыкл
                      {
                          if(blokname.Substring(0,1)=="0")//если выбраный символ ноль
                          {
                            blokname=blokname.Remove(0,1);//уаляем его
                          }
                          else
                          {
                            break;//нет заканчиваем стерать нули
                          }
*/                        //Debug.Log(blokname);
                        inventr.SearchForSameItem(Dbb.items[int.Parse(blokname)],1);//добавляем игроку блок
                        Destroy(hit.collider.gameObject);//уничтожаем обьект
                    }
                    else
                    {
                      if (SelectBlockForInventory() != null)//проверяем наличи выбраного блока
                      {
                          inventr.items[inventr.si].count-=1;//забераем блок из инвентаря
                          //устанавливаем выыбраный блок

                          GameObject obj2 = Instantiate(SelectBlockForInventory(), convertVector(posBlockPos(new Vector3(0, 0, 0),true)), new Quaternion(0, 0, 0, 0), worldBuild.transform);
                          obj2.name = idselectblock.ToString();
                      }
                    }
                  }
				       }
             }
          }
        }
    }
    public void time()
	  {

      if (uisc.nonemenu)
      {
          if (uisc.MobileD)
          {
              Ray ray = rayB();
              Ray rayB()
              {
                  Vector3 pointSc = new Vector3(manCam.pixelWidth / 2,manCam.pixelHeight / 2, 0);
                  return manCam.ScreenPointToRay(pointSc);
                  //return new Ray(manCam.transform.position, plas.transform.forward);
              }
              if (Physics.Raycast(ray, out hit,10f))
              {
                  if (hit.collider.gameObject != null && hit.collider.gameObject.layer != 9)
                  {
                      if(inventr.items[inventr.si].id == 14)
                      {
                          string blokname = hit.collider.gameObject.name;
                          /*for(int i = 0;i<10;i++)
                          {
                              if(blokname.Substring(0,1)=="0")
                              {
                                  blokname=blokname.Remove(0,1);
                              }
                              else
                              {
                                  break;
                              }
                          }*/
                          //Debug.Log(blokname);
                          inventr.SearchForSameItem(Dbb.items[int.Parse(blokname)],1);
                          Destroy(hit.collider.gameObject);
                      }
                      else
                      {
                          if (SelectBlockForInventory() != null)
                          {
                              inventr.items[inventr.si].count-=1;
                              GameObject obj2 = Instantiate(SelectBlockForInventory(), convertVector(posBlockPos(new Vector3(0, 0, 0),true)), new Quaternion(0, 0, 0, 0), worldBuild.transform);
                              obj2.name = idselectblock.ToString();
                          }
                      }
                  }
              }
          }
      }
	}

    void GeneratorWorldPlock(int x, int y, int z, int SizeX, int Sizez, GameObject block)
    {
      for(int i = 0; i < SizeX; i++)
      {
        for(int i1 = 0; i1 < Sizez; i1++)
        {
          GameObject obj2= Instantiate(block,new Vector3(i + x, y, i1 + z),new Quaternion(0,0,0,0),worldBuild.transform);
        }
      }
    }

    Vector3 convertVector(Vector3 a)//окугляем позицию
    {
      return new Vector3((int)a.x, (int)a.y, (int)a.z);
    }

    GameObject SelectBlockForInventory()
    {
        for(int i=0;i<blockCount;i++)//перебираем все блоки из базы
        {
            if(Dbb.blocks[i].idItemBlock == inventr.items[inventr.selectItem].id)//если выбрный блок и блок из базы совпали
            {
                idselectblock = Dbb.blocks[i].idItemBlock;
                return Dbb.blocks[i].prefabBlock;//возращяем обьект блока
            }
        }
        return null;//если ни чего не совпало ничего не возрвщяем
    }
}
