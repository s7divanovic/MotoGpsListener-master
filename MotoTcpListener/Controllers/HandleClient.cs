using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using MotoTcpListener.Model;
using Newtonsoft.Json;
using System.Linq;
using MotoTcpListener.Lib;

namespace MotoTcpListener.Controllers
{
    public class HandleClient
    {
        TcpClient clientSocket;
        string clNo;
        Hashtable clientsList;

        int requestCount = 0;
        byte[] bytesFrom = new byte[10025];
        string dataFromClient = null;
        Byte[] sendBytes = null;
        string serverResponse = null;
        string rCount = null;

        public void StartClient(TcpClient inClientSocket, string clineNo, Hashtable cList,string dataFromClient)
        {
            this.dataFromClient = dataFromClient;
            clientSocket = inClientSocket;
            clNo = clineNo;
            clientsList = cList;
            Thread ctThread = new Thread(DoChat);
            ctThread.Start();
        }

        private void DoChat()
        {
            try
            {
                while (true)
                {
                    requestCount++;
                    //NetworkStream networkStream = clientSocket.GetStream();
                    //networkStream.Read(bytesFrom, 0, 10025);
                    //dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    
                    Console.WriteLine("From GPS - " + clNo + " : " + dataFromClient);
                    rCount = requestCount.ToString();
                    Console.WriteLine("GPS data: " + dataFromClient);
                    var separate = dataFromClient.Split(':');
                    var parseData = separate.Last().Split(',').Length;

                    if (parseData == 1)
                    {
                        Broadcast.BroadcastClientData("ON", clNo, false, clientsList);
                    }

                    if (parseData > 3 && dataFromClient.Contains("tracker"))
                    {
                        Dictionary<string, InsertionData> dataForInsertion = new Dictionary<string, InsertionData>();
                        List<string> parseGpsData = new List<string>(separate.Last().Split(','));
                        InsertionData insertionData = new InsertionData();

                        insertionData.mileage = (float.Parse(parseGpsData[5]) * 1.60934).ToString();
                        insertionData.latitude = CalculateData.DegreeToDecimal(Decimal.Parse(parseGpsData[7]), parseGpsData[8]).ToString();
                        insertionData.longitude = CalculateData.DegreeToDecimal(Decimal.Parse(parseGpsData[9]), parseGpsData[10]).ToString();
                        insertionData.speed = (float.Parse(parseGpsData[11]) * (float)1.852).ToString();
                        insertionData.bearing = float.Parse(parseGpsData[12]).ToString();
                        insertionData.fuelConsumption = float.Parse(parseGpsData[14]).ToString();

                        dataForInsertion.Add(clNo, insertionData);

                        Tracking data = new Tracking()
                        {
                            imei = clNo,
                            type = "tracker",
                            journey_data = JsonConvert.SerializeObject(dataForInsertion.Values),
                            timestamp = DateTime.Now
                        };

                        using (VehicleContext _db = new VehicleContext())
                        {
                            _db.Tracking.Add(data);
                            _db.SaveChangesAsync();
                            //##,imei:359710048989827,A; : imei:359710048989827,tracker,161101201234,,F,191235.000,A,4515.6358,N,01949.5184,E,0.14,181.92,,0,,,,;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}