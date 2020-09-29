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


namespace Host
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HostLogs HostLog1 { get; set; }

        public HostLogs HostLog2 { get; set; }

        public HostLogs HostLog3 { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            logBox.Document.Blocks.Remove(logBox.Document.Blocks.FirstBlock);
            this.Hide();
            HostLog1 = new HostLogs();
            HostLog1.Title = "Host 1";
            HostLog2 = new HostLogs();
            HostLog2.Title = "Host 2";
            HostLog3 = new HostLogs();
            HostLog3.Title = "Host 3";
            HostLog1.Show();
            HostLog2.Show();
            HostLog3.Show();

            Hosts hosts1 = new Hosts(1, HostLog1);
            Hosts hosts2 = new Hosts(2, HostLog2);
            Hosts hosts3 = new Hosts(3, HostLog3);

            CPCC cpcc1 = new CPCC(HostLog1);
            cpcc1.CPCCStart(1);
            CPCC cpcc2 = new CPCC(HostLog2);
            cpcc2.CPCCStart(2);
            CPCC cpcc3 = new CPCC(HostLog3);
            cpcc3.CPCCStart(3);

            

            HostLog1.myHost = hosts1;
            HostLog1.myCPCC = cpcc1;
            HostLog2.myHost = hosts2;
            HostLog2.myCPCC = cpcc2;
            HostLog3.myHost = hosts3;
            HostLog3.myCPCC = cpcc3;
            HostLog1.FillDistantHosts();
            HostLog2.FillDistantHosts();
            HostLog3.FillDistantHosts();

            //Task.Run(() => { hosts1.SendMessage(); });
            //Task.Run(() => { cpcc1.SendMessage("127.0.0.1", "dzien dobry"); });
            //Task.Run(() => { cpcc1.SendMessage("127.0.0.1", "dzien dobry"); });
            //Task.Run(() => { hosts1.SendMessage("127.0.0.10", "127.0.0.30", "13"); });

            //Task.Run(() => { cpcc1.SendCallRequest("host1", "host3", "600"); });
            //Task.Run(() => { cpcc1.SendMessage("127.0.0.2", "dzien dobry "); });
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
