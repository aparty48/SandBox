using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainmenu : MonoBehaviour
{
    public List<worldInfo> wf = new List<worldInfo>();

    public UIScript uisc;
    public Generatorworlda gn;

    public GameObject btnminmenu;
    public GameObject worldMenu;
    public GameObject worldMenuSeting;
    public GameObject shablonWorld;
    public GameObject GmObjShow;

    public Sprite defaulltIconWorld;

    public int seed;
    public string nameWorld = @"";


    string [] d;
    // Start is called before the first frame update
    void Start()
    {
        if(!Directory.Exists(Application.persistentDataPath + "/worlds"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/worlds");
        }
        d = Directory.GetDirectories(Application.persistentDataPath + "/worlds");
        for(int pi=0;pi<d.Length;pi++)
        {
            wf.Add(new worldInfo(){name="",date="",seed=""});
        }
        //if(!File.Exists(Application.persistentDataPath + "/miniGames.txt"))
        //{
        //  using (FileStream fs = File.Create(Application.persistentDataPath + "/miniGames.txt")){}
        //  continue;
        //}
    }

    // Update is called once per frame

    public void PlayButtonTanks()
    {
        SceneManager.LoadScene("Tanks");
    }
    public void playBtnOn()
    {

      uisc.inoe = !uisc.inoe;
        worldMenu.gameObject.SetActive(uisc.inoe);
        btnminmenu.SetActive(!uisc.inoe);
        LoadListWorld();
    }
    public void RandomSeed(InputField inp)
    {
        System.Random rd = new System.Random();
        inp.text = rd.Next(0, 9).ToString()+ rd.Next(0, 9).ToString()+ rd.Next(0, 9).ToString()+ rd.Next(0, 9).ToString()+ rd.Next(0, 9).ToString()+ rd.Next(0, 9).ToString()+ rd.Next(0, 9).ToString()+ rd.Next(0, 9).ToString()+ rd.Next(0, 9).ToString()+ rd.Next(0, 9).ToString();
        seed = (int)long.Parse(inp.text);
    }
    public void OnEnterInput(InputField inp)
    {
        seed = (int)long.Parse(inp.text);
    }
    public void NameWorld(InputField inp)
	  {
        nameWorld = inp.text;
	  }
    public void CreateWorldData()
	  {
        if (!Directory.Exists(Application.persistentDataPath + "/worlds"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/worlds");
        }
        if(nameWorld=="")
		    {
            System.Random rd = new System.Random();
            nameWorld = rd.Next(1, 10000).ToString()+ rd.Next(1, 10000).ToString()+ rd.Next(1, 10000).ToString();
        }
        if (!File.Exists(Application.persistentDataPath + "/worlds/" + nameWorld + "/confw.txt"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/worlds/" + nameWorld);
            using (FileStream fs = new FileStream(Application.persistentDataPath + "/worlds/" + nameWorld + "/confw.txt",FileMode.Create)) {}
            using(StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/worlds/" + nameWorld + "/confw.txt", false))
			      {
                DateTime da = new DateTime();
                sw.WriteLine(nameWorld);
                sw.WriteLine(da);
                sw.WriteLine(seed.ToString());
			      }
        }
        gn.seed = seed;
        gn.Generate();
        uisc.inoe = false;
    }
    public void LoadListWorld()
    {
      if(true)
      {
        if(!Directory.Exists(Application.persistentDataPath + "/worlds"))
        {
          System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/worlds");
        }



        for(int i = 0;i<d.Length;i++)
        {
          string ww = d[i]+"/confw.txt";
          if(!File.Exists(d[i]+"/confw.txt")){
            using (FileStream fs = File.Create(d[i]+"/confw.txt")){}
            continue;
          }

          string[] readText ;
          readText = File.ReadAllLines(d[i]+"/confw.txt");
          if(readText.Length >= 2)
          {

            wf[i].name = readText[0];
            wf[i].date = readText[1];
            wf[i].seed = readText[2];
            //Debug.Log(" "+readText[0]+" "+readText[1]+" "+readText[2]);
            for(int q=0;q<readText.Length;q++)
            {
              //Debug.Log(readText[q]);
            }
          }

        }
        for(int i = 0;i<wf.Count;i++)
        {
          GameObject obj2 = Instantiate(shablonWorld, new Vector3(0,0,0), new Quaternion(0, 0, 0, 0), GmObjShow.transform);
          obj2.name = i.ToString();
          GameObject t = obj2.transform.GetChild(1).gameObject;
          GameObject p = obj2.transform.GetChild(2).gameObject;
          t.GetComponent<Text>().text = wf[i].name;
          p.GetComponent<Text>().text = wf[i].date;
          Button tempButton = obj2.GetComponent<Button>();//каждий предмет это кнопка

          tempButton.onClick.AddListener(delegate { StartGenSeed((int)Convert.ToInt64(obj2.name)); });
        }
      }
      if(uisc.MobileD)
      {

      }
      //Debug.Log();
    }
    private void StartGenSeed(int numWo)
    {

      gn.seed = (int)Convert.ToInt64(wf[numWo].seed);
      gn.Generate();
    }
    public void SortListWorld()
    {
      for(int k = 0;k<wf.Count;k++)
      {
        for(int i = wf.Count;i>wf.Count;i--)
        {
          var a = Convert.ToDateTime(wf[i].date);
          var b = Convert.ToDateTime(wf[i-1].date);
          if(a<b)
          {
            //wf[i].date = b.ToString("yyyy/MM/dd");
            //wf[i-1].date = b.ToString("yyyy/MM/dd");
          }
        }
      }
    }
    //DateTime date1 = new DateTime();

}
public class worldInfo
{
  public string name{get;set;}
  public string date{get;set;}
  public string seed{get;set;}

}
