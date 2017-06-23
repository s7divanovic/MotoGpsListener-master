using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections;
using MotoTcpListener.Controllers;
using System.Text;

namespace MotoTcpListener
{
    class Program
    {
        public static Hashtable clientsList = new Hashtable();

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 9000);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;
            try
            {
                serverSocket.Start();
                Console.WriteLine("GPS Tracker Server Started ....");
                while (true)
                {
                    counter += 1;
                    clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Connected");
                    byte[] bytesFrom = new byte[10025];
                    string dataFromClient = null;

                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, 10025);
                    dataFromClient = Encoding.ASCII.GetString(bytesFrom);
                    //Console.WriteLine("GPS data is: " + dataFromClient);
                    //System.IO.File.WriteAllText(@"C:\_djivanovic\trackerdata.txt", dataFromClient);
                    int contentParseLength = dataFromClient.Split(',').Length;
                    String[] contentParse = dataFromClient.Split(',');
                    String prepare = contentParse.ElementAtOrDefault(1);
                    String imei = null;

                    if (prepare != null)
                    {
                        String[] prepareImei = prepare.Split(':');
                        imei = prepareImei.ElementAtOrDefault(1);
                    }

                    if (imei != null && !clientsList.Contains(imei))
                    {
                        clientsList.Add(imei, clientSocket);
                    }
                    else
                    {
                        Console.WriteLine("Assign new socket for client");
                        clientsList[imei] = clientSocket;
                        continue;
                    }

                    if (contentParseLength == 3 && imei != null)
                    {
                        Broadcast.BroadcastClientData("LOAD", imei, false, clientsList);
                    }
                    //else
                    //{
                    //    continue;
                    //}
                    //broadcast(dataFromClient + " Joined ", dataFromClient, false);

                    Console.WriteLine(imei + " Joined GPS tracker ");
                    HandleClient client = new HandleClient();
                    client.StartClient(clientSocket, imei, clientsList,dataFromClient);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception is: " + e.Message);
            }

            finally
            {
                clientSocket.Close();
                serverSocket.Stop();
                Console.WriteLine("exit");
                Console.ReadLine();
            }
        }
    }
}