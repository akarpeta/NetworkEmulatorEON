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
 

namespace Node
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int NodeNumber { get; set; }

        public NetworkNodeWindow NodeWindow { get; set; }
        XmlReader reader = new XmlReader("ConfigurationNode.xml");


        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            for (int i = 1; i <= 8; i++)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    NodeWindow = new NetworkNodeWindow(i);
                    NodeWindow.Title = "Router " + i;
                    NodeWindow.Show();
                }));
            }
        }
/*
        private bool CheckNodeName(string number)
        {
            var isNumber = int.TryParse(number, out int x);
            return isNumber;
        }

        private bool CheckNodeNumber(int number)
        {
            int numberofnodes = reader.GetNumberOfItems("router");
            if (number > 0 && number <= numberofnodes)
            {
                return true;
            }
            else return false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (numberBox.Text.Length == 0)
            {
                return;
            }
            if (CheckNodeName(numberBox.Text) == false)
            {
                numberBox.Text = "To nie nr!";
                return;
            }
            var tmp = Convert.ToInt32(numberBox.Text);
            if (CheckNodeNumber(tmp) == false)
            {
                numberBox.Text = "Zly nr!";
                return;
            }
            NodeNumber = tmp;
            numberBox.Clear();

            Dispatcher.Invoke(new Action(() =>
            {
                NodeWindow = new NetworkNodeWindow(NodeNumber);
                NodeWindow.Title = "Router " + NodeWindow.ToString();
                NodeWindow.Show();
            }));
        }
        */
    }
}
