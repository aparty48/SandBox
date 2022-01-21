using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using NetEngie;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class HostGame : MonoBehaviour
{
    private List<TcpClient> clients = new List<TcpClient>();
    private List<SClient> sclients = new List<SClient>();
    private TcpClient client;
    public bool ConsoleApp = false;
    public float distancePlayer = 15f;
    public static bool isHost = false;
    // Start is called before the first frame update
    private void Start()
    {

    }

    public void StartHostOnButton()
    {
        string a = "127.10.11.11";
        StartHost(IPAddress.Parse(a),1234);
    }
    // Update is called once per frame
    private void Update()
    {
        GetLocalPlayerData();
    }

    public void StartHost(IPAddress ip, int port)
    {
        NetEngie.Host.SetData(ip,port);
        NetEngie.Host.Start();
        Task.Factory.StartNew(()=>
        {
            while(NetEngie.Host.ishost)
            {
                client = NetEngie.Host.server.AcceptTcpClient();
                clients.Add(client);
                GetInfoClients(client);
                Task.Delay(10).Wait();
            }
        });
        isHost = NetEngie.Host.ishost;
        Task.Factory.StartNew(()=>
        {
            while(NetEngie.Host.ishost)
            {
                for(int i = 0; i<clients.Count;i++)
                {
                    if(!clients[i].Connected)
                    {
                        for(int j = 0; j < sclients.Count; j++)
                        {
                            if(sclients[j].tcpclent == clients[i])
                            {
                                sclients.RemoveAt(j);
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if(clients[i] != null)
                        {
                            clients[i].Close();
                            clients.RemoveAt(i);
                        }

                    }
                }

                int[] countPlayerOfRange = new int[sclients.Count];
                int counterArray = 0;
                Vector3[] pospl = new Vector3[sclients.Count];
                string outputdata = "pd.";

                for (int i = 0; i < sclients.Count; i++)
                {
                    for (int j = 0; j < sclients.Count; j++)
                    {
                        if (Vector3.Distance(sclients[i].globalPosition , sclients[j].globalPosition) < distancePlayer)
                        {
                            countPlayerOfRange[counterArray] = i;
                            counterArray++;
                            break;
                        }
                    }

                    //wirezaty elsi eto budet console app
                    if (Vector3.Distance(uisc.player.transform.position , sclients[i].globalPosition) < distancePlayer)
                    {
                        outputdata += plc.nickPlayer + ";" + uisc.player.transform.position.x + ";" + uisc.player.transform.position.y + ";" + uisc.player.transform.position.z + ";" + uisc.player.transform.rotation.y + "; ";
                    }
                    //do etogo comentariya virezaty

                    for(int j = 0; j < counterArray; j++)
                    {
                        outputdata += sclients[i].nick + ";" + sclients[i].globalPosition.x.ToString() + ";" + sclients[i].globalPosition.y.ToString() + ";" + sclients[i].globalPosition.z.ToString() + ";" + sclients[i].rotationY.ToString() + "; ";
                    }
                    NetworkStream stream = sclients[i].tcpclent.GetStream();
                    stream.Write(Encoding.UTF8.GetBytes(outputdata), 0, Encoding.UTF8.GetBytes(outputdata).Length);
                    stream.Close();
                }

                Task.Delay(10).Wait();
            }
        });
    }


    public void Stop()
    {
        NetEngie.Host.Stop();
        isHost = false;
        for (int i = 0; i<clients.Count; i++)
        {
            if(clients[i] != null)
                clients[i].Close();
            clients.Clear();
        }
    }


    public void GetInfoClients(TcpClient c)
    {
        string a;
        a = "s.";
        string h = "s.d";
        string g = "s.pe";
        int b =0;
        bool sd = false;
        Byte[] d = new Byte[2];
        NetworkStream stream = c.GetStream();
        stream.Write(Encoding.UTF8.GetBytes(a), 0, Encoding.UTF8.GetBytes(a).Length);
        b = stream.Read(d,0,d.Length);
        sd = AddToList(Encoding.UTF8.GetString(d, 0, b),c);
        if (sd)
        {
            stream.Write(Encoding.UTF8.GetBytes(h), 0, Encoding.UTF8.GetBytes(h).Length);
        }
        else
        {
            stream.Write(Encoding.UTF8.GetBytes(g), 0, Encoding.UTF8.GetBytes(g).Length);
            c.Close();
            clients.Remove(c);
        }
        stream.Close();
    }


    private bool AddToList(string a,TcpClient cln)
    {
        string b ="";//player nick
        long s=0;//id
        for (int j = 0; j < a.Length; j++)
        {
            if (a.Substring(j,1) == ";")
            {
              s = long.Parse(a.Substring(0,j));
              b = a.Remove(0,j + 1);
              break;
            }
            else
            {
                continue;
            }
        }
        for (int i = 0; i<sclients.Count; i++)
        {
            if (sclients[i].nick != b && sclients[i].id == s)
            {
                return false;
            }
            sclients.Add(new SClient(){tcpclent = cln, nick = b, id = s, inventary = null});
            return true;
        }
        return false;
    }













    [SerializeField]
    private Client plc;
    [SerializeField]
    private UIScript uisc;
    private bool inListClient = false;
    private void GetLocalPlayerData()
    {
        if (!ConsoleApp)
        {
            if (isHost)
            {

            }
        }
    }



}
public class SClient
{
    public TcpClient tcpclent;
    public string nick;
    public long id;
    public ItemInventory inventary;
    public Vector3 globalPosition;
    public float rotationY;
}
