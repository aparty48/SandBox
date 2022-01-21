using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using NetEngie;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class Client : MonoBehaviour
{
    public long idPlayer = 12334567890123456;
    public string nickPlayer = "Player";
    private int conectEtap = 0;

    public GameObject playersGameObject;
    public GameObject prefabPlayer;

    private List<GameObject> playersGObj = new List<GameObject>();
    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    public void StartClient(IPAddress ip, int port)
    {
        NetEngie.Client.SetServer(ip, port);
        NetEngie.Client.Start();
        Task.Factory.StartNew(()=>{
            Byte[] bytef = new Byte[512];
            int bytes = 0;
            while(NetEngie.Client.isClient)
            {
                bytes = NetEngie.Client.Read(bytef);
                if(bytes == 0)
                {
                    continue;
                }
                WorkRead(Encoding.UTF8.GetString(bytef, 0, bytes));
            }
        });
    }
    private void WorkRead(string a)
    {
        switch(a)
        {
            case "s.":
            NetEngie.Client.Send(Encoding.UTF8.GetBytes(idPlayer.ToString() + ";" + nickPlayer));
            break;
            case "s.d":
            conectEtap++;
            break;
            case "s.pe":
            Debug.Log("Player esty");
            break;
        }
        string stringData = "pd.nick;1,1;0,2;1;2; nicks;1;11;2;1; ";
        List<string> playersData = new List<string>();

        string[] nicksPlayers ;
        Vector3[] posPlayers;
        float[] rotyPlayers;
        float[] xposPlayers;
        float[] yposPlayers;
        float[] zposPlayers;
        //pd.nick;1;2;3;4 nick1;1;2;3;4
        if (stringData.Substring(0,3) == "pd.")
        {
            stringData = stringData.Remove(0,3);
            for (int i = 0; i<stringData.Length; i++)
            {
                if (stringData.Substring(i, 1) == " ")
                {
                    playersData.Add(stringData.Substring(0, i));
                    stringData = stringData.Remove(0, i+1);
                    i=0;
                }
            }
            rotyPlayers = new float[playersData.Count];
            xposPlayers = new float[playersData.Count];
            yposPlayers = new float[playersData.Count];
            zposPlayers = new float[playersData.Count];
            nicksPlayers = new string[playersData.Count];
            posPlayers = new Vector3[playersData.Count];

            for(int j=0;j<playersData.Count;j++)
            {
                for(int k = 0;k<playersData[j].Length;k++)
                {
                    if (playersData[j].Substring(k,1) == ";")
                    {
                        nicksPlayers[j] = playersData[j].Substring(0,k);
                        playersData[j] = playersData[j].Remove(0,k+1);
                        break;
                    }
                }
                for(int k = 0;k<playersData[j].Length;k++)
                {
                    if (playersData[j].Substring(k,1) == ";")
                    {
                        xposPlayers[j] = float.Parse(playersData[j].Substring(0,k));
                        playersData[j] = playersData[j].Remove(0,k+1);
                        break;
                    }
                }
                for(int k = 0;k<playersData[j].Length;k++)
                {
                    if (playersData[j].Substring(k,1) == ";")
                    {
                        yposPlayers[j] = float.Parse(playersData[j].Substring(0,k));
                        playersData[j] = playersData[j].Remove(0,k+1);
                        break;
                    }
                }
                for(int k = 0;k<playersData[j].Length;k++)
                {
                    if (playersData[j].Substring(k,1) == ";")
                    {
                        zposPlayers[j] = float.Parse(playersData[j].Substring(0,k));
                        playersData[j] = playersData[j].Remove(0,k+1);
                        break;
                    }
                }
                for(int k = 0;k<playersData[j].Length;k++)
                {
                    if (playersData[j].Substring(k,1) == ";")
                    {
                        rotyPlayers[j] = float.Parse(playersData[j].Substring(0,k));
                        playersData[j] = playersData[j].Remove(0,k+1);
                        break;
                    }
                }
                posPlayers[j] = new Vector3(xposPlayers[j],yposPlayers[j],zposPlayers[j]);
            }
            if (nicksPlayers.Length > 0)
            {
                SetOrCreatPlayer(nicksPlayers,posPlayers,rotyPlayers);
            }
        }
    }










    private void SetOrCreatPlayer(string[] nicks, Vector3[] pos, float[] rotationY)
    {
        int trig = 0;
        int trig2 = 0;
        for (int i = 0; i < nicks.Length; i++)//берем каждого по очереди активного игрока
        {
            for (int j = 0; j < playersGObj.Count; j++)//берем каждого по оереди игрока из масива перемешаных игроков
            {
                if (nicks[j] == playersGObj[i].name)//сравниваем активных и смешаных
                {
                    trig++;
                    break;
                }
            }
            if (trig > trig2)
            {
                trig2 = trig;
                continue;
            }
            else if (trig == trig2)
            {
                GameObject g = Instantiate(prefabPlayer, pos[i], new Quaternion(0, rotationY[i], 0, 0), playersGameObject.transform);
                g.name = nicks[i];
                playersGObj.Add(g);
            }
            trig = 0;
            trig2 = 0;
        }
        if (playersGObj.Count > 0)//если активных игроков больше нуля
        {
            for (int i = 0; i < playersGObj.Count; i++)//берем каждого по очереди активного игрока
            {
                for (int j = 0; j < nicks.Length; j++)//берем каждого по оереди игрока из масива перемешаных игроков
                {
                    if (nicks[j] == playersGObj[i].name)//сравнение активных и смешаных
                    {// этот код если совпал игрок
                        playersGObj[i].transform.position = pos[j];//делаем чтото а имено устанавливаем позицию в мире
                        playersGObj[i].transform.rotation = new Quaternion(0, rotationY[j], 0, 0);
                        break;//заканчиваем цыкл перебора
                    }
                }
            }
        }
        for (int i = 0; i < playersGObj.Count; i++)//берем каждого по очереди активного игрока
        {
            for (int j = 0; j < nicks.Length; j++)//берем каждого по оереди игрока из масива перемешаных игроков
            {
                if (nicks[j] == playersGObj[i].name)//сравниваем активных и смешаных
                {
                    trig++;
                    break;
                }
            }
            if (trig > trig2)
            {
                trig2 = trig;
                continue;
            }
            else if (trig == trig2)
            {

                Destroy(playersGObj[i]);
                playersGObj.RemoveAt(i);
            }
        }
    }
}
