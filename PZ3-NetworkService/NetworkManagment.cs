using PZ3_NetworkService.Model;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PZ3_NetworkService
{
    public static class NetworkManagment
    {
        public static void CreateListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25565);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        var stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            byte[] data = Encoding.ASCII.GetBytes(StaticClass.Servers.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            try
                            {
                                //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                                //Console.WriteLine(incomming); //Na primer: "Objekat_1:272"

                                //################ IMPLEMENTACIJA ####################
                                // Obraditi poruku kako bi se dobile informacije o izmeni
                                string[] vals = incomming.Split('_', ':');
                                if (vals.Length > 0)
                                {
                                    int id = Convert.ToInt32(vals[1]);
                                    int value = Convert.ToInt32(vals[2]);

                                    // Azuriranje potrebnih stvari u aplikaciji
                                    StaticClass.Servers[id].Value = value;
                                    StaticClass.Rectangles[id].Height = value * 2;

                                    Console.WriteLine("Received value " + value + " For object " + id);

                                    using (var sw = new StreamWriter("Log.txt", true))
                                    {
                                        sw.WriteLine(DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString() + ": " + StaticClass.Servers[id].Name + ", " + value);
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                    }, null);
                }

            })
            {
                IsBackground = true
            };
            listeningThread.Start();
        }
    }
}
