using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace NetEngie
{
    public class NetEngie : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
    public class Server
    {
        public void Start()
        {

        }
        public void Stop()
        {

        }
    }
    public class Host
    {
        private static IPAddress localAddress;
        private static int port;
        public static TcpListener server;
        public static bool ishost;

        public static void Start()
        {
            server = new TcpListener(localAddress, port);
            server.Start();
            ishost = true;
        }
        public static void Stop()
        {
            server.Stop();
        }
        public static void SetData(IPAddress ip, int port)
        {
            localAddress = ip;
            port = port;
        }
    }







    public class Client
    {
        private static int port;
        private static TcpClient client;
        private static NetworkStream stream;
        private static IPAddress ipserver;
        public static bool isClient;

        public static void Start()
        {
            client = new TcpClient();
            client.Connect(ipserver, port);
            stream = client.GetStream();
            isClient = true;
        }
        public static void Stop()
        {
            client.Close();
            stream.Close();
            isClient = false;
        }
        public static void SetServer(IPAddress address, int port)
        {
          ipserver = address;
          port = port;
        }
        public static void Send(Byte[] message)
        {
            stream.Write(message, 0,message.Length);
        }
        public static int Read(Byte[] data)
        {
            if (stream.CanRead) return stream.Read(data, 0, data.Length);
            else return 0;
        }
    }
}
