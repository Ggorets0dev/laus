using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using WinHardwareSpecs;
using System.Threading;
using System.ComponentModel;
using System.Drawing;
using Laus.ViewModels.Base;
using Laus.Models;
using Laus;

namespace Laus
{
    public partial class MainWindow : Window
    {
        private NotifyIcon nIcon = new NotifyIcon();

        private WindowsViewModel _windowsViewModel = new WindowsViewModel();

        private Server _server = new Server(IPAddress.Any, 8888);

        private BackgroundWorker _selfSpecsWorker = new BackgroundWorker();
        private BackgroundWorker _lanDevicesWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _windowsViewModel;

            nIcon.Visible = true;
            nIcon.Icon = Properties.Resources.NotifyIcon;
            nIcon.MouseClick += NotifyIconClicked;
            nIcon.ContextMenuStrip = new ContextMenuStrip();
            nIcon.ContextMenuStrip.Items.Add("Выход", null, NotifyIconExitSelected);

            _selfSpecsWorker.DoWork += GetSelfSpecs;
            _selfSpecsWorker.RunWorkerCompleted += SelfSpecsCollected;

            _lanDevicesWorker.DoWork += GetLanDevices;
            _lanDevicesWorker.RunWorkerCompleted += LanDevicesCollected;

            _ = _server.ListenAsync();
        }

        private void NotifyIconClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Visibility = Visibility.Visible;

            else if (e.Button == MouseButtons.Right)
                nIcon.ContextMenuStrip.Show();
        }

        void NotifyIconExitSelected(object sender, EventArgs e) => System.Windows.Application.Current.Shutdown();

        private void FormClosed(object sender, CancelEventArgs e)
        {
            if (_windowsViewModel.ControlPanelEnabled)
            {
                Visibility = Visibility.Hidden;
                e.Cancel = true;
            }

            else
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Вы уверены, что хотите закрыть программу? Существует активная операция, приложение останется в работе", "Подтверждение закрытия", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    Visibility = Visibility.Hidden;

                e.Cancel = true;
            }
        }

        private void BlockControlPanel() => _windowsViewModel.ControlPanelEnabled = false;
        private void UnblockControlPanel() => _windowsViewModel.ControlPanelEnabled = true;

        private void GetForeignSpecsButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_windowsViewModel.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Устройство для получения характеристик не выбрано", "Отсутствие выбранного устройства", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

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
    
        private void GetLanDevices(object sender, DoWorkEventArgs e)
        {
            var lanDevices = new List<IPAddress>();
            
            var selfAddresses = NetworkScanner.GetSelfAddresses();

            foreach (var selfAddress in selfAddresses)
                lanDevices.AddRange(NetworkScanner.GetLanDevices(selfAddress, timeout: 300));

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
