using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Utility;

namespace Host
{
    public class CPCC
    {
        //adres IP host
        public IPAddress HostIP { get; set; }
        //nazwa hosta - klienta
        public string HostName { get; set; }
        //domena w ktorej znajudje sie host 
        public int DomainID { get; set; }

        //Zmienna przechowująca IP na którym będzie nasłuchiwał CPCC
        public IPAddress CPCCListenerIP { get; set; }
        
        //ip adres NCC z ktorym bedziemy sie laczyc
        public IPAddress NCCIpAddress { get; set; }

        //ip portu na ktorym laczymy sie z NCC
        public int NCCPort { get; set; }

        //Adres docelowy polaczenia
        public string DestinationIP;

        public Dictionary<string, string> DistantHosts;

        public bool FlagListening;

        public bool FlagAcceptCall = true;

        public CPCC _cpcc;

        public HostLogs myWindow { get; set; }

        //public Window window;
        public void CPCCStart(int numberofhost)
        {
            
            try
            {
               XmlReader reader = new XmlReader("Configurationhost" + numberofhost + ".xml");
               HostName = reader.GetAttributeValue("host", "NAME");
               HostIP = IPAddress.Parse(reader.GetAttributeValue("host", "IP_ADDRESS"));
               CPCCListenerIP = IPAddress.Parse(reader.GetAttributeValue("host", "CPCC_IP_ADDRESS"));
               NCCIpAddress = IPAddress.Parse(reader.GetAttributeValue("host", "NCC_IP_ADDRESS"));
               NCCPort = Convert.ToUInt16(reader.GetAttributeValue("host", "NCC_PORT"));
               DomainID = Convert.ToUInt16(reader.GetAttributeValue("host", "DOMAIN"));

               DistantHosts = reader.GetHostItems("host", "IP_ADDRESS", "HOST_NAME"); 
               Listen(CPCCListenerIP, 11000);
            }
            catch (Exception)
            {
                //NewLog("catch", myWindow, "LightSalmon");
            }
        }

        public CPCC(HostLogs tmp)
        {
            myWindow = tmp;
           
            _cpcc = this;
        }


        //CPCC IP Address 
        private void Listen(IPAddress address, int port)
        {
            NewLog($"{CPCCListenerIP} Listen", myWindow, "LightSalmon");

            Task.Run(() =>
             {
                 try
                 {
                    
                     byte[] messagebyte = new byte[64];


                     Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                     IPEndPoint ipep = new IPEndPoint(address, port);
                     UdpClient newsock = new UdpClient(ipep);
                     IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                  
                     FlagListening = true;
                    
                     while (FlagListening == true)
                        {
                    
                            messagebyte = newsock.Receive(ref sender);
                            if (messagebyte.Length > 0)
                            {
                                var message = Encoding.ASCII.GetString(messagebyte);
                                var action = message.Split(null)[0];
                                var tmp = message.Replace($"{action} ", "");
                                var data = tmp.Split(' ');
                                switch (action)
                                {
                                    case MessageNames.CALL_INDICATION:
                                            CallAccept(data);
                                        break;
                                    case MessageNames.CALL_CONFIRMED:
                                        break;
                                 case MessageNames.ADD_CONNECTION:
                                     HandleAddedConnection(data);
                                     break;
                             }
                                messagebyte = null;
                            }
                        }
                    }
                    catch
                    {
                     

                 }
             });
   
        }

        public void HandleAddedConnection(string[] msg)
        {
            Connections newConnection = XmlSerialization.DeserializeObject<Connections>(XmlSerialization.GetStringToNormal(msg));
            myWindow.myHost.connections.Add(newConnection);
            NewLog($"New connection handled", myWindow, "Blue");
        }

        //odpowiedz na CallIndication NCC akceptuje lub zrywa polaczenie -przyjmujemy zawsze Accept
        public void CallAccept(string[] data)
        {
           
            string Acceptation;
            string sourcehost = data[0];
            string destinationhost = data[1];
            string demanded = data[2];
            //ncc skad bedzie szla wiadomosc
            string ncc_source = data[3];
            //IPAddress ncc_ip = IPAddress.Parse(ncc_source);

            NewLog($"{CPCCListenerIP} Get CallIndication from NCC [{sourcehost} {destinationhost} {demanded} {ncc_source}]", myWindow, "Red");

            if (FlagAcceptCall)
            {
                Acceptation = "Accept";
                
            }else
            {
                Acceptation = "Break";   
            }
            SendCallConfirmed(Acceptation, sourcehost, destinationhost, demanded,ncc_source);
        }


        //Funkcja do wysyłania wiadomości po nacisnięciu
        public void SendCallRequest(string sourcename, string destname, string demandedCapacity)
        {
            try
            {
                NewLog($"{CPCCListenerIP} Send Call Request to NCC: {sourcename} {destname} {demandedCapacity}",myWindow, "LimeGreen");
                string message = null;
                message = MessageNames.CALL_REQUEST + " " + sourcename + " " + destname + " " + demandedCapacity+" ";
                byte[] messagebyte = new byte[message.Length];
                messagebyte = Encoding.ASCII.GetBytes(message);

                //Wyslłanie wiadomości UDP
                UdpClient client = new UdpClient(NCCIpAddress.ToString(), 11000); 
                client.Send(messagebyte, messagebyte.Length);
            }
            catch(Exception e)
            {
                e.ToString();
                
            }

        }

        //send message udp client
        public void SendMessage(string ipaddress, string message)
        {

            try
            {
                NewLog($"SEEEND MESSAGE TO CLOUD", myWindow, "LimeGreen");
                //byte[] messagebyte = new byte[message.Length];
                byte[] messagebyte = Encoding.ASCII.GetBytes(message);
                //Wyslłanie wiadomości UDP
                UdpClient client = new UdpClient(ipaddress, 11000);
                client.Send(messagebyte, messagebyte.Length);
            }
            catch (Exception e)
            {
                e.ToString();

            }

        }

        //public void SendMessage()
        //{

        //    NewLog($"sacfewdc", window, "Pink");
        //    MyPacket packet = new MyPacket();
        //    packet.Port = 111;
        //    //packet.Port = 111;
        //    //packet.DestinationAddress = IPAddress.Parse("127.0.0.30");
        //    NewLog($"iudhkiuagf", window, "Pink");

        //    mySocket.Send(packet);

        //    NewLog($"2378273", window, "Pink");
        //    NewLog($"Wysłano pakiet nr {packet.ID} wiadomość {packet.Payload}  z portu {packet.Port}", window, "Pink");


        //    //await Task.Delay(TimeSpan.FromMilliseconds(10000));
        //}

        public void SendCallConfirmed(string accept, string source, string destination, string demanded, string ncc_source)
        {
            try
            {
               
                string message = null;
                message = accept + " " + source + " " + destination + " " + demanded+" "+ ncc_source;

                NewLog($"{CPCCListenerIP} Send CallConfirmed message", myWindow, "LightSalmon");
                string message2 = null;
                message2 = MessageNames.CALL_CONFIRMED_FROM_CPCC + " " + message;
              
                byte[] messagebyte = new byte[message2.Length];
                messagebyte = Encoding.ASCII.GetBytes(message2);
                //Wyslłanie wiadomości UDP
                UdpClient client = new UdpClient();
                client.Send(messagebyte, messagebyte.Length, NCCIpAddress.ToString(), 11000);
            }
            catch
            {

            }
        }

        public void NewLog(string info, HostLogs tmp, string color)
        {

            myWindow.Dispatcher.Invoke(() => myWindow.NewLog(info, tmp,color));
        
        }
       
        
    }
}
