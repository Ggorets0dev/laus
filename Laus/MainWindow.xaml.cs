using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinHardwareSpecs;

namespace Laus
{
    public partial class MainWindow : Window
    {
        private WindowsViewModel _windowsViewModel = new WindowsViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _windowsViewModel;

            //this.SpecsTextBox.Multiline = true;
            //this.SpecsTextBox.WordWrap = false;
            //this.SpecsTextBox.ScrollBars = ScrollBars.Horizontal;
        }

        private void GetDevicesButtonClicked(object sender, RoutedEventArgs e)
        {
            _windowsViewModel.LanDevices.Clear();

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return;

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    string ipAddr = ip.ToString();

                    _windowsViewModel.LanDevices.Add(new DeviceViewModel { ID = ipAddr, Alias = "Не назначен", IpAddress = ipAddr });
                }
            }
        }

        private void GetForeignSpecsButtonClicked(object sender, RoutedEventArgs e)
        {
        }

        private void GetSelfSpecsButtonClicked(object sender, RoutedEventArgs e)
        {
            var specification = SpecMonitor.GetSpecification();

            SpecsTextBox.Clear();
            SpecsTextBox.Text = specification.ToJson();
        }
    }
}
