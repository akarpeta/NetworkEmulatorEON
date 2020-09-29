using Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace SubNetwork
{
    public class LRM
    {
        public List<SNP> SNPList { get; set; }

         //public SlotTable SlTable { get; set; }


        public string IPAddressLRM { get; set; }
       
        public string IPAddressCC { get; set; }
        public string IPAddressRC { get; set; }

        public SubnetworkLogs myWindow;
        public int IdLRM { get; set; }

        public LRM(int nr)
        {
            SNPList = new List<SNP>();
            //SlTable = new SlotTable();
            IdLRM = nr;

            //z xmla tez wczytuje SNP
            //int tmpNumber = Int32.Parse(ConfigurationManager.AppSettings[numberLRM + "SNPCount"]);
            //for (int i = 0; i < tmpNumber; i++)
            //{         wincyj gowien z configa
            //    string[] words = ConfigurationManager.AppSettings[numberLRM + "SNP" + (i + 1)].Split('#');
            //    SNPList.Add(new SNP(IPAddress.Parse(words[0]), Int32.Parse(words[1]), Int32.Parse(words[2])));
            //}
        }

        public LRM(IPAddress iPAddress, IPAddress rcip,SubnetworkLogs tmp)
        {
            myWindow = tmp;
            IPAddressCC = iPAddress.ToString();
            IPAddressRC = rcip.ToString();
        }

        public void ActionSelect(string act,string[] msg)
        {
           
            switch (act)
            {
                case MessageNames.LINK_CONNECTION_REQUEST:
                    //ResourcesAllocate(MessageNames.LINK_CONNECTION_REQUEST, msg);
                    break;
                case MessageNames.SNP_RELEASE:
                    //ResourcesAllocate(MessageNames.SNP_RELEASE + "RESPONSE", msg);
                    break;
                case MessageNames.LINK_CONNECTION_DEALLOCATION:
                    ResourcesDeallocate(MessageNames.LINK_CONNECTION_DEALLOCATION, msg);
                    break;
                case MessageNames.SNP_RELEASE + "DEALLOCATION":
                    ResourcesDeallocate(MessageNames.SNP_RELEASE + "DEALLOCATIONRESPONSE", msg);
                    break;
                case MessageNames.LINK_ALLOCATION_RESPONSE:
                    SendMessage(IPAddressCC,act);
                    break;
            }
        }

        public void ReceiveMessage()
        {
            
            byte[] bytes = new byte[64];
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPAddressLRM), 11000);
            UdpClient newsock = new UdpClient(ipep);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (true)
                {
                    bytes = newsock.Receive(ref sender);
                    var message = Encoding.ASCII.GetString(bytes);
                    var action = message.Split(null)[0];
                    var tmp = message.Replace($"{action} ", "");
                    var data = tmp.Split(' ');

                    Task.Run(() => ActionSelect(action, data));
                }
            }
            catch (Exception)
            {

            }
        }

        private void SendMessage(string adr, string msg)
        {
            byte[] data = new byte[64];
            IPEndPoint endPointSend = new IPEndPoint(IPAddress.Parse(adr), 11000);
            UdpClient SocketSend = new UdpClient();

            try
            {
                data = Encoding.ASCII.GetBytes(msg);
                SocketSend.Send(data, data.Length, endPointSend);
            }
            catch (Exception)
            {

            }
            SocketSend.Close();
        }

        public void KillLink(string text)
        {
            int numberOfLink;
            string[] words = text.Split(' ');
            try
            {
                numberOfLink = Int32.Parse(words[1]);

                //string msgKL = ConfigurationManager.AppSettings["LRM" + numberLRM] + "#" + MessageNames.KILL_LINK + "#" + numberOfLink + "#";
                //generateLogKillLink("RC", MessageNames.KILL_LINK, ConfigurationManager.AppSettings["RC" + numberLRM], numberOfLink);
                //SendMessage(ConfigurationManager.AppSettings["RC" + numberLRM], messageKillLink);

               // generateLogKillLink("CC", MessageNames.KILL_LINK, ConfigurationManager.AppSettings["CC" + numberLRM], numberOfLink);
                //SendMessage(ConfigurationManager.AppSettings["CC" + numberLRM], messageKillLink);

            }
            catch (Exception)
            {
                Console.WriteLine("Bad data format");

            }
        }


        //zaalokuj zasoby wyslij do RC
        public void ResourcesAllocate(string action, string[] msg)
        {
            string message = MessageNames.ALLOCATE;
            for(int a=0; a<msg.Length;a++)
            {
                message +=" "+msg[a];
            }
            message += " " + IdLRM+" "+IPAddressLRM;
            NewLog($"ResourcesAllocate message {action}{message}{IdLRM} adres{IPAddressLRM}", myWindow, "MediumVioletRed");

            SendMessage(IPAddressRC,message);
        }

        public void ResourcesDeallocate(string action, string[] msg)
        {

        }

        public void NewLog(string info, SubnetworkLogs tmp, string color)
        {
            myWindow.Dispatcher.Invoke(() => myWindow.NewLog(info, tmp, color));

        }
    }
}
