using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Host
{
    /// <summary>
    /// Interaction logic for HostLogs.xaml
    /// </summary>
    public partial class HostLogs : Window
    {
        public Hosts myHost { get; set; }

        public CPCC myCPCC { get; set; }

        public string HostName { get; set; }

        public string BitrateTxt { get; set; }

        public string Payload { get; set; }

        public HostLogs()
        {
            InitializeComponent();
        }

        public void FillDistantHosts()
        {
            foreach (KeyValuePair<string, string> entry in myCPCC.DistantHosts)
            {
                whereCombo.Items.Add(entry.Value);
            }
        }

        private void whereCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HostName = (string)whereCombo.SelectedItem;
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            BitrateTxt = bitrateText.Text;
            Task.Run(() => { myCPCC.SendCallRequest(myHost.HostName, HostName, BitrateTxt); });
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void NewLog(string info, HostLogs tmp2, string color)
        {
            HostLogs tmp = tmp2;
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

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            BitrateTxt = bitrateText.Text;
            Payload = messageText.Text;
            IPAddress ipaddress = new IPAddress(0);

            if(HostName=="host1")
            {
                ipaddress= IPAddress.Parse("127.0.0.10");

            }else if(HostName == "host2")
            {
                ipaddress = IPAddress.Parse("127.0.0.20");
            }
            else if(HostName=="host3")
            {
                ipaddress = IPAddress.Parse("127.0.0.30");
            }

            Task.Run(() => { myHost.SendMessage(ipaddress, BitrateTxt, Payload); });
        }
    }
}
