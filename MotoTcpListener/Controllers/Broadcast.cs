using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;

namespace MotoTcpListener.Controllers
{
    public class Broadcast
    {
        public static void BroadcastClientData(string msg, string uName, bool flag, Hashtable clientsList)
        {
            TcpClient broadcastSocket;
            broadcastSocket = (TcpClient)clientsList[uName];
            NetworkStream broadcastStream = broadcastSocket.GetStream();
            Byte[] broadcastBytes = null;

            if (flag == true)
            {
                broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + msg);
            }
            else
            {
                broadcastBytes = Encoding.ASCII.GetBytes(msg);
            }

            broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
            broadcastStream.Flush();
        }
    }
}