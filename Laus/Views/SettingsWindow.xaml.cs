using Laus.ViewModels;
using Laus.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Laus.Views
{
    public partial class SettingsWindow : System.Windows.Window
    {
        private SettingsViewModel _windowViewModel = new SettingsViewModel();

        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = _windowViewModel;

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            _windowViewModel.Version = fileVersionInfo.FileVersion;

            var config = Config.Get();
            AliasTextBox.Text = config.Alias;
            TimeoutTextBox.Text = config.TimeoutMs.ToString();

            AddressComboBox.ItemsSource = NetworkScanner.GetSelfAddresses();

            foreach (var item in AddressComboBox.Items)
            {
                if (item.ToString() == RuntimeSettings.selfAddress)
                {
                    AddressComboBox.SelectedValue = item;
                    break;
                }
            }
        }

        #region Обработчики событий формы
        private void SaveButtonClicked(object sender, RoutedEventArgs e)
        {
            string alias = AliasTextBox.Text;

            ushort msTimeout;
            bool timeoutCheck = ushort.TryParse(TimeoutTextBox.Text, out msTimeout);

            if (!timeoutCheck || msTimeout > Config.MaxTimeoutMs)
            {
                System.Windows.MessageBox.Show($"Значение максимального времени ожидания отклика не может превышать {Config.MaxTimeoutMs} мс", "Значение не может быть использовано", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            else if (alias.Length > Config.MaxAliasLength)
            {
                System.Windows.MessageBox.Show($"Длина псевдонима не может превышать {Config.MaxAliasLength} символов", "Значение не может быть использовано", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var config = Config.Get();

            config.TimeoutMs = msTimeout;
            config.Alias = alias;

            config.Save();

            RuntimeSettings.selfAddress = AddressComboBox.Text;

            System.Windows.MessageBox.Show("Файл конфигурации успешно сохранен", "Сохранение завершено", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        void TimeoutTextBoxPreview(object sender, TextCompositionEventArgs e)
        {
            foreach (char symbol in e.Text)
            {
                if (!char.IsDigit(symbol))
                {
                    e.Handled = true;
                    break;
                }
            }
        }
        #endregion
    }
}
