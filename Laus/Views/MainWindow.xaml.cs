﻿using System;
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
using Laus.Views;

namespace Laus
{
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyIcon = new NotifyIcon();

        private MainWindowViewModel _windowViewModel = new MainWindowViewModel();

        private Server _server = new Server(IPAddress.Any);

        private BackgroundWorker _selfSpecsWorker = new BackgroundWorker();
        private BackgroundWorker _lanDevicesWorker = new BackgroundWorker();
        private BackgroundWorker _checkConnectionWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _windowViewModel;

            notifyIcon.Visible = true;
            notifyIcon.Icon = Properties.Resources.NotifyIcon;
            notifyIcon.MouseClick += NotifyIconClicked;
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Выход", null, NotifyIconExitSelected);

            _selfSpecsWorker.DoWork += GetSelfSpecs;
            _selfSpecsWorker.RunWorkerCompleted += SelfSpecsCollected;

            _lanDevicesWorker.DoWork += GetLanDevices;
            _lanDevicesWorker.RunWorkerCompleted += LanDevicesCollected;

            _checkConnectionWorker.DoWork += CheckConnection;
            _checkConnectionWorker.RunWorkerCompleted += ConnectionChecked;

            _ = _server.ListenAsync();
        }

        #region Вспомогательные функции

        private void BlockControlPanel() => _windowViewModel.ControlPanelEnabled = false;
        private void UnblockControlPanel() => _windowViewModel.ControlPanelEnabled = true;

        #endregion

        #region Обработчики событий формы

        private void NotifyIconClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Visibility = Visibility.Visible;

            else if (e.Button == MouseButtons.Right)
                notifyIcon.ContextMenuStrip.Show();
        }

        void NotifyIconExitSelected(object sender, EventArgs e) => System.Windows.Application.Current.Shutdown();

        private void FormClosed(object sender, CancelEventArgs e)
        {
            if (_windowViewModel.ControlPanelEnabled)
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

        private void CheckConnectionButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_windowViewModel.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Устройство для проверки соединения не выбрано", "Отсутствие выбранного устройства", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BlockControlPanel();
            _windowViewModel.OperationStatus = "Проверка доступности устройства..";
            _checkConnectionWorker.RunWorkerAsync();
        }

        private void GetDevicesButtonClicked(object sender, RoutedEventArgs e)
        {
            BlockControlPanel();
            _windowViewModel.LanDevices.Clear();
            _windowViewModel.ResetSelectedIndex();

            _windowViewModel.OperationStatus = "Получение списка устройств...";
            _lanDevicesWorker.RunWorkerAsync();
        }

        private void GetSelfSpecsButtonClicked(object sender, RoutedEventArgs e)
        {
            BlockControlPanel();
            _windowViewModel.ResetSpecs();
            _windowViewModel.OperationStatus = "Получение сведений о системе...";
            
            _selfSpecsWorker.RunWorkerAsync();
        }

        private void GetForeignSpecsButtonClicked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OpenSettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            var settingsForm = new SettingsWindow();
            settingsForm.ShowDialog();
        }

        #endregion

        #region Компоненты фоновых потоков

        private void GetLanDevices(object sender, DoWorkEventArgs e)
        {
            var lanDevices = new List<IPAddress>();
            
            var selfAddresses = NetworkScanner.GetSelfAddresses();

            foreach (var selfAddress in selfAddresses)
                lanDevices.AddRange(NetworkScanner.GetLanDevices(selfAddress, timeout: Config.Get().TimeoutMs));

            e.Result = lanDevices;
        }

        private void LanDevicesCollected(object sender, RunWorkerCompletedEventArgs e)
        {
            var lanDevices = e.Result as List<IPAddress>;

            foreach (var device in lanDevices)
                _windowViewModel.LanDevices.Add(new DeviceViewModel { IpAddress = device.ToString(), Alias = "Не назначен" });

            _windowViewModel.ResetStatus();
            UnblockControlPanel();
        }

        private void CheckConnection(object sender, DoWorkEventArgs e)
        {
            try
            {
                string deviceAddress = _windowViewModel.GetSelectedItem().IpAddress;
                var client = new Client(deviceAddress);

                e.Result = client.CheckUser();
            }
            catch
            {
                e.Result = false;
            }
        }

        private void ConnectionChecked(object sender, RunWorkerCompletedEventArgs e)
        {
            bool checkResult = (bool)e.Result;

            if (checkResult)
                System.Windows.MessageBox.Show("Устройство доступно для получения характеристик", "Ответ получен", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                System.Windows.MessageBox.Show("Устройство недоступно для получения характеристик", "Ошибка соединения", MessageBoxButton.OK, MessageBoxImage.Error);

            UnblockControlPanel();
            _windowViewModel.ResetStatus();
        }

        private void GetSelfSpecs(object sender, DoWorkEventArgs e)
        {
            var specification = SpecMonitor.GetSpecification();
            e.Result = specification;
        }

        private void SelfSpecsCollected(object sender, RunWorkerCompletedEventArgs e)
        {
            _windowViewModel.Specs = (e.Result as Specification).ToString();
            _windowViewModel.ResetStatus();
            UnblockControlPanel();
        }

        #endregion
    }
}
