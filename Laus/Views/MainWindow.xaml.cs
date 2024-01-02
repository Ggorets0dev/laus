using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinHardwareSpecs;
using Laus.Models;
using Laus;
using System.Threading;
using System.ComponentModel;
using Laus.ViewModels.Base;

namespace Laus
{
    public partial class MainWindow : Window
    {
        private WindowsViewModel _windowsViewModel = new WindowsViewModel();

        private Server _server = new Server(IPAddress.Any, 8888);

        private BackgroundWorker _selfSpecsWorker = new BackgroundWorker();
        private BackgroundWorker _lanDevicesWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _windowsViewModel;

            _selfSpecsWorker.DoWork += GetSelfSpecs;
            _selfSpecsWorker.RunWorkerCompleted += SelfSpecsCollected;

            _lanDevicesWorker.DoWork += GetLanDevices;
            _lanDevicesWorker.RunWorkerCompleted += LanDevicesCollected;

            _ = _server.ListenAsync();
        }

        private void BlockControlPanel() => _windowsViewModel.ControlPanelEnabled = false;
        private void UnblockControlPanel() => _windowsViewModel.ControlPanelEnabled = true;

        private void GetDevicesButtonClicked(object sender, RoutedEventArgs e)
        {
            BlockControlPanel();
            _windowsViewModel.LanDevices.Clear();
            _windowsViewModel.ResetSelectedIndex();

            _windowsViewModel.OperationStatus = "Получение списка устройств...";
            _lanDevicesWorker.RunWorkerAsync();
        }

        private void GetSelfSpecsButtonClicked(object sender, RoutedEventArgs e)
        {
            BlockControlPanel();
            _windowsViewModel.ResetSpecs();
            _windowsViewModel.OperationStatus = "Получение сведений о системе...";
            
            _selfSpecsWorker.RunWorkerAsync();
        }

        private void LanDevicesListViewSelectionChanged(object sender, SelectionChangedEventArgs e) => GetForeignSpecsButton.IsEnabled = _windowsViewModel.SelectedIndex != -1;
    
        private void GetLanDevices(object sender, DoWorkEventArgs e)
        {
            var lanDevices = new List<IPAddress>();
            
            var selfAddresses = NetworkScanner.GetSelfAddresses();

            foreach (var selfAddress in selfAddresses)
                lanDevices.AddRange(NetworkScanner.GetLanDevices(selfAddress, timeout: 100));

            e.Result = lanDevices;
        }

        private void LanDevicesCollected(object sender, RunWorkerCompletedEventArgs e)
        {
            var lanDevices = e.Result as List<IPAddress>;

            foreach (var device in lanDevices)
                _windowsViewModel.LanDevices.Add(new DeviceViewModel { IpAddress = device.ToString(), Alias = "Не назначен" });
            
            _windowsViewModel.ResetStatus();
            UnblockControlPanel();
        }

        private void GetSelfSpecs(object sender, DoWorkEventArgs e)
        {
            var specification = SpecMonitor.GetSpecification();
            e.Result = specification;
        }

        private void SelfSpecsCollected(object sender, RunWorkerCompletedEventArgs e)
        {
            _windowsViewModel.Specs = (e.Result as Specification).ToString();
            _windowsViewModel.ResetStatus();
            UnblockControlPanel();
        }
    }
}
