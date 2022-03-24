using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Net;
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
    public GameObject canvasMultiplayer;
    public GameObject shablonServer;
    public GameObject objectShowServer;

    public Sprite defaulltIconWorld;

    public int seed;
    public string nameWorld = @"";
    public bool loadServerlist = false;
    private bool loadWorldList = false;
    private List<ServerInfo> servers = new List<ServerInfo>();
    public Client clientss;


    string [] d;
    // Start is called before the first frame update
    void Start()
    {
        if(!Directory.Exists(Application.persistentDataPath + "/worlds"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/worlds");
        }
        d = Directory.GetDirectories(Application.persistentDataPath + "/worlds");
        using (FileStream fs = new FileStream(Application.persistentDataPath + "/servers", FileMode.OpenOrCreate, FileAccess.Read))
        {}
        //if(!File.Exists(Application.persistentDataPath + "/miniGames.txt"))
        //{
        //  using (FileStream fs = File.Create(Application.persistentDataPath + "/miniGames.txt")){}
        //  continue;
        //}
    }

    // Update is called once per frame
    private void Update()
    {
        canvasMultiplayer.SetActive(uisc.gsMenuMultiplayer);
        btnminmenu.SetActive(!uisc.gsMenuMultiplayer);
        worldMenu.gameObject.SetActive(uisc.gsMenuWorld);
        btnminmenu.SetActive(!uisc.gsMenuWorld);
    }

    public void PlayButtonTanks()
    {
        SceneManager.LoadScene("Scene/Tanks");
    }
    public void playBtnOn(bool a)
    {
        if(uisc.nonemenu||a)
        {
            uisc.WorldMenu();
            LoadListWorld();
        }
    }
    public void MultiplayerMenu(bool a)
    {
        if(uisc.nonemenu ||a)
        {
            uisc.MultyplayerMenu();
            LoadListSever();
        }
    }
    List<ServerInfo> servinf = new List<ServerInfo>();
    public void LoadListSever()
    {
        ///Debug.Log("a");
        if(!loadServerlist)
        {
            loadServerlist = true;


            string[] readText ;
            readText = File.ReadAllLines(Application.persistentDataPath + "/servers");
            IPAddress ipserver = null;
            int port = 0;
            string nameserver = "";
            if(servers.Count>0)
            {
                servers.Clear();
            }
            for (int i = 0;i < readText.Length; i++)
            {
                for (int j = 0; j < readText[i].Length; j++)
                {
                    if (readText[i].Substring(j,1) == ";")
                    {
                      ipserver = IPAddress.Parse(readText[i].Substring(0,j));
                      readText[i] = readText[i].Remove(0,j + 1);
                      break;
                    }
                    else
                    {
                        continue;
                    }
                }

                for (int j = 0; j < readText[i].Length; j++)
                {
                    if (readText[i].Substring(j,1) != ";")
                    {
                        continue;
                    }
                    else
                    {
                        port = int.Parse(readText[i].Substring(0,j));
                        readText[i] = readText[i].Remove(0,j + 1);
                        nameserver = readText[i];
                        break;
                    }
                }
                servers.Add(new ServerInfo(){name = nameserver, port = port, ip = ipserver});
                //Debug.Log(servers.Count);
            }
            if(objectShowServer.transform.childCount>0)
            {
                for (int i = 0; i < objectShowServer.transform.childCount; i++)
                {
                    Destroy(objectShowServer.transform.GetChild(i).gameObject);
                }
            }
            for (int i = 0; i < servers.Count; i++)
            {
                GameObject o = Instantiate(shablonServer, new Vector3(0,0,0), new Quaternion(0, 0, 0, 0), objectShowServer.transform);
                o.name = i.ToString();
                GameObject n = o.transform.GetChild(0).gameObject;
                n.GetComponent<Text>().text = servers[i].name;
                Button tempButton = o.GetComponent<Button>();
                tempButton.onClick.AddListener(delegate {ConnectToServer(i);});
            }

        }
    }

    private void ConnectToServer(int a)
    {
        MultiplayerMenu(true);
        clientss.StartClient(servers[a].ip, servers[a].port);
    }

    public void CreatServerInList(IPAddress ip, int port, string name)
    {
        using (System.IO.StreamWriter swa = new StreamWriter(Application.persistentDataPath + "/servers", true))
        {
            swa.WriteLine(ip.ToString() + ";" + port.ToString() + ";" + name);
        }
        loadServerlist = false;
    }
    public GameObject menuAddServer;
    public void PressButtonAdd()
    {
        menuAddServer.SetActive(!menuAddServer.activeSelf);
    }
    public InputField iip;
    public InputField iport;
    public InputField iname;

    public void addcreate()
    {
        if(iip.text != null && iport != null && iname != null){
            CreatServerInList(IPAddress.Parse(iip.text),int.Parse(iport.text),iname.text);
            PressButtonAdd();LoadListSever();}
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
        playBtnOn(true);
        loadWorldList = false;
    }
    public void LoadListWorld()
    {
      if(!loadWorldList)
      {
          loadWorldList=true;
          if(GmObjShow.transform.childCount>0)
          {
              for (int i = 0; i < GmObjShow.transform.childCount; i++)
              {
                  DestroyImmediate(GmObjShow.transform.GetChild(i).gameObject);
              }
          }
          if(wf.Count>0)
          {
              wf.Clear();
          }
          if(!Directory.Exists(Application.persistentDataPath + "/worlds"))
          {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/worlds");
          }



          for(int i = 0;i<d.Length;i++)
          {
              string ww = d[i]+"/confw.txt";
              if(!File.Exists(d[i]+"/confw.txt"))
              {
                  using (FileStream fs = File.Create(d[i]+"/confw.txt")){}
                  continue;
              }

              string[] readText ;
              readText = File.ReadAllLines(d[i]+"/confw.txt");
              if(readText.Length >= 2)
              {
                  //wf[i].name = readText[0];
                  //wf[i].date = readText[1];
                  //wf[i].seed = readText[2];
                  wf.Add(new worldInfo(){name = readText[0], date = readText[1], seed = readText[2]});
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
}
public class worldInfo
{
  public string name{get;set;}
  public string date{get;set;}
  public string seed{get;set;}

}
public class ServerInfo
{
    public string name{get;set;}
    public IPAddress ip{get;set;}
    public int port{get;set;}
}
