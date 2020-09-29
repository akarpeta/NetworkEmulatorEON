using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utility;
using System.Net;

namespace SubNetwork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SubnetworkLogs SubnetworkLog1 { get; set; }
        public SubnetworkLogs SubnetworkLog2 { get; set; }

        public KillLinkWindow KillWindow { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            SubnetworkLog1 = new SubnetworkLogs();
            SubnetworkLog1.Title = "Domena 1";
            SubnetworkLog2 = new SubnetworkLogs();
            SubnetworkLog2.Title = "Domena 2";
            SubnetworkLog1.Show();
            SubnetworkLog2.Show();
            ConnectionController cc = new ConnectionController(101, SubnetworkLog1);
            ConnectionController cc2 = new ConnectionController(202, SubnetworkLog2);
            RC rc = new RC(101, SubnetworkLog1);
            RC rc2 = new RC(202, SubnetworkLog2);
            
            Task.Run(() => { cc.ReceivedMessage(); });
            Task.Run(() => { rc.Run(); });
            Task.Run(() => { cc2.ReceivedMessage(); });
            Task.Run(() => { rc2.Run(); });
            //var task1 = Task.Run(() => cc.ReceivedMessage());
            //var task2 = Task.Run(() => rc.Run());
            //Task.WhenAll(task1, task2,task3,task4);


            //cc.ConnectionRequest();
            //Task.Run(() => {
            //rc.SendMessage("127.0.42.42", "czesc to rc do node");
            //});
            //testy
            // string source = "127.0.0.10";
            // string destination = "127.0.0.30";
            // string demanded = "34";
            // int domainflag = 0;
            // // var message = source + " " + destination + " " + demanded + " " + domainflag;
            //var message = source + " " + destination + " " + demanded + " " + "END";
            // var data = message.Split(' ');

            // rc.RouteTableQueryReq(data);

            // CreatePath(int startNodeid, int endNodeid, string dCapacity, int distance, int IdRC)
            //rc.CreatePath(1, 5, "100", 0, 101);
            //rc2.CreatePath(6, 8, "100", 0, 202);

            //string dat = "127.0.0.10 127.0.0.30 34 120";
            //var con = dat.Split(' ');
            //rc2.NetworkTopologyReq(con);
            //source + " "+destin+ " "+dCapacity+" "+distance;
            //"NETWORKTOPOLOGY 127.0.0.10 127.0.0.30 34 127.0.10.31"
            //rc.NetworkTopologyRsp(con);
            //Connection request do CC o zestawienie połaczenia action source detination demanded czywtejsamejdomenie
            //testy
            //string source="127.0.0.10";
            //string destination="127.0.0.30";
            //string demanded="34";
            //int domainflag = 0;
            //var message = source + " " + destination + " " + demanded + " " + domainflag;
            //var data = message.Split(' ');
            //cc.ConnectionRequest(data);       
        }


        public void NewLog(string info, MainWindow tmp2, string color)
        {
            MainWindow tmp = tmp2;
            string timeNow = DateTime.Now.ToString("h:mm:ss") + "." + DateTime.Now.Millisecond.ToString();
            string restLog = info;
            string fullLog = timeNow + " " + restLog;
            tmp.logBox.AppendText(Environment.NewLine);
            ColorLog(tmp.logBox, fullLog, color);
            tmp.logBox.ScrollToEnd();
        }

        public static void ColorLog(RichTextBox box, string text, string color)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = text;
            try
            {
                tr.ApplyPropertyValue(TextElement.BackgroundProperty,
                    bc.ConvertFromString(color));
            }
            catch (FormatException) { }
        }
    }
}
