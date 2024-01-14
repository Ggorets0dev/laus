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
using Laus.Views;
using System.Windows.Input;
using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace Laus
{
    /// <summary>
    /// Главное окно
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyIcon = new NotifyIcon();

        private MainWindowViewModel _windowViewModel = new MainWindowViewModel();

        private string _selectedDeviceAddress;
        private Regex _addressRegex = new Regex(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$"); // формат IPv4 

        private Server _server = new Server(IPAddress.Any);

        private BackgroundWorker _selfSpecsWorker = new BackgroundWorker();
        private BackgroundWorker _lanDevicesWorker = new BackgroundWorker();
        private BackgroundWorker _checkConnectionWorker = new BackgroundWorker();
        private BackgroundWorker _foreignSpecsWorker = new BackgroundWorker();

        /// <summary>Создание и настройка главного окна</summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _windowViewModel;

            notifyIcon.Visible = true;
            notifyIcon.Icon = Properties.Resources.NotifyIcon;
            notifyIcon.MouseClick += NotifyIconClicked;
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Выход", null, NotifyIconExitSelected);

            DeviceAddressTextBox.MaxLength = 15; // Максимальная длина адреса в формате IPv4

            _selfSpecsWorker.DoWork += GetSelfSpecs;
            _selfSpecsWorker.RunWorkerCompleted += SpecsCollected;

            _lanDevicesWorker.DoWork += GetLanDevices;
            _lanDevicesWorker.RunWorkerCompleted += LanDevicesCollected;

            _checkConnectionWorker.DoWork += CheckConnection;
            _checkConnectionWorker.RunWorkerCompleted += ConnectionChecked;

            _foreignSpecsWorker.DoWork += GetForeignSpecs;
            _foreignSpecsWorker.RunWorkerCompleted += SpecsCollected;

            _ = _server.ListenAsync();
        }

        #region Вспомогательные функции
        private bool SetSelectedDeviceAddress()
        {
            bool isDeviceSelected = _windowViewModel.SelectedAddressIndex != -1;
            bool isAddressEntered = _addressRegex.IsMatch(DeviceAddressTextBox.Text);

           if (isAddressEntered)
                _selectedDeviceAddress = DeviceAddressTextBox.Text;

            else if (isDeviceSelected)
                _selectedDeviceAddress = _windowViewModel.GetSelectedAddress().IpAddress;

            return isDeviceSelected || isAddressEntered;
        }
        #endregion

        #region Обработчики событий формы
        private void NotifyIconClicked(object sender, System.Windows.Forms.MouseEventArgs e)
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
            if (!SetSelectedDeviceAddress())
            {
                System.Windows.MessageBox.Show("Устройство для проверки соединения не выбрано", "Отсутствие адреса устройства", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _windowViewModel.ControlPanelEnabled = false;
            _windowViewModel.OperationStatus = "Проверка доступности устройства...";
            _checkConnectionWorker.RunWorkerAsync();
        }

        private void GetDevicesButtonClicked(object sender, RoutedEventArgs e)
        {
            _windowViewModel.ControlPanelEnabled = false;
            _windowViewModel.LanDevices.Clear();
            _windowViewModel.ResetSelectedAddressIndex();

            _windowViewModel.OperationStatus = "Получение списка устройств...";
            _lanDevicesWorker.RunWorkerAsync();
        }

        private void GetSelfSpecsButtonClicked(object sender, RoutedEventArgs e)
        {
            _windowViewModel.ControlPanelEnabled = false;
            _windowViewModel.ResetSpecification();
            _windowViewModel.OperationStatus = "Получение сведений о системе...";
            
            _selfSpecsWorker.RunWorkerAsync();
        }

        private void GetForeignSpecsButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!SetSelectedDeviceAddress())
            {
                System.Windows.MessageBox.Show("Устройство для получения характеристик не выбрано", "Отсутствие адреса устройства", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _windowViewModel.ControlPanelEnabled = false;
            _windowViewModel.OperationStatus = "Получение характеристик выбранного устройства...";
            _foreignSpecsWorker.RunWorkerAsync();
        }

        private void OpenSettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            var settingsForm = new SettingsWindow();
            settingsForm.ShowDialog();
        }

        private void DeviceAddressTextBoxKeyPreview(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void DeviceAddressTextBoxTextPreview(object sender, TextCompositionEventArgs e) => e.Handled = "0123456789.".IndexOf(e.Text) < 0;
        #endregion

        #region Компоненты фоновых потоков
        private void GetLanDevices(object sender, DoWorkEventArgs e)
        {
            var lanDevices = new List<DeviceViewModel>();

            if (RuntimeSettings.IsAddressSelected())
            {
                var selectedAddress = IPAddress.Parse(RuntimeSettings.selfAddress);
                ushort pingTimoutMs = Config.Get().TimeoutMs;

                lanDevices = NetworkScanner.GetLanDevices(selectedAddress, pingTimoutMs);
            }

            else
            {
                System.Windows.MessageBox.Show("Сеть для поиска устройств не выбрана, определите ее в настройках приложения", "Сеть не выбрана", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            e.Result = lanDevices;
        }

        private void LanDevicesCollected(object sender, RunWorkerCompletedEventArgs e)
        {
            var lanDevices = e.Result as List<DeviceViewModel>;

            foreach (var device in lanDevices)
                _windowViewModel.LanDevices.Add(device);

            _windowViewModel.ResetStatus();
            _windowViewModel.ControlPanelEnabled = true;
        }

        private void CheckConnection(object sender, DoWorkEventArgs e)
        {
            try
            {
                var client = new Client(_selectedDeviceAddress);
                e.Result = client.CheckUser().isApproved;
            }
            catch (SocketException ex)
            {
                System.Windows.MessageBox.Show("Не удалось получить характеристики из-за ошибки в соединении с устройством", "Соединение не установлено", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (e.Result == null)
                    e.Result = false;
            }
        }

        private void ConnectionChecked(object sender, RunWorkerCompletedEventArgs e)
        {
            bool checkResult = (bool)e.Result;

            if (checkResult)
                System.Windows.MessageBox.Show("Устройство доступно для получения характеристик", "Соединение установлено", MessageBoxButton.OK, MessageBoxImage.Information);

            _windowViewModel.ResetStatus();
            _windowViewModel.ControlPanelEnabled = true;
        }

        private void GetSelfSpecs(object sender, DoWorkEventArgs e)
        {
            var specification = SpecMonitor.GetSpecification();
            e.Result = specification;
        }

        private void SpecsCollected(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                var specification = e.Result as Specification;
                _windowViewModel.Specs = specification.ToString();
            }

            _windowViewModel.ResetStatus();
            _windowViewModel.ControlPanelEnabled = true;
        }
        
        private void GetForeignSpecs(object sender, DoWorkEventArgs e)
        {
            try
            {
                var client = new Client(_selectedDeviceAddress);
                e.Result = client.GetSpecification();
            }
            catch (WebException ex)
            {
                System.Windows.MessageBox.Show("Не удалось получить характеристики из-за ошибки в соединении с устройством. Ошибка протокола", "Соединение не установлено", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SocketException ex)
            {
                System.Windows.MessageBox.Show("Не удалось получить характеристики из-за ошибки в соединении с устройством. Ошибка сокета", "Соединение не установлено", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
