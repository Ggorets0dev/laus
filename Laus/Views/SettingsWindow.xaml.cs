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
using System.IO;

namespace Laus.Views
{
    /// <summary>
    /// Окно с настройками приложения
    /// </summary>
    public partial class SettingsWindow : System.Windows.Window
    {
        private SettingsViewModel _windowViewModel = new SettingsViewModel();

        /// <summary>Создание и настройка окна с настройками приложения</summary>
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = _windowViewModel;

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            _windowViewModel.Version = fileVersionInfo.FileVersion;

            TimeoutTextBox.MaxLength = Config.MaxTimeoutMs.ToString().Length;
            AliasTextBox.MaxLength = Config.MaxAliasLength;

            try
            {
                var config = Config.Get();
                AliasTextBox.Text = config.Alias;
                TimeoutTextBox.Text = config.TimeoutMs.ToString();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Не удалось отобразить значения из файла конфигурации по причине его отсутствия", "Файл конфигурации отсутствует", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            LoadOnStartupCheckBox.IsChecked = StartupLoader.IsLoadOnStartup();

            _windowViewModel.SelfAddresses = NetworkScanner.GetSelfAddresses();
            _windowViewModel.SetSelectedAddress(RuntimeSettings.selfAddress);
        }

        #region Обработчики событий формы
        private void SaveButtonClicked(object sender, RoutedEventArgs e)
        {
            string alias = AliasTextBox.Text;

            ushort msTimeout;
            bool timeoutCheck = ushort.TryParse(TimeoutTextBox.Text, out msTimeout);

            if (!timeoutCheck || msTimeout > Config.MaxTimeoutMs)
            {
                MessageBox.Show($"Значение максимального времени ожидания отклика не может превышать {Config.MaxTimeoutMs} мс", "Значение не может быть использовано", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            else if (alias.Length == 0)
            {
                MessageBox.Show($"Длина псевдонима может быть от 1 до {Config.MaxAliasLength} символов", "Значение не может быть использовано", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var config = Config.Get();

                config.TimeoutMs = msTimeout;
                config.Alias = alias;

                config.Save();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Не удалось сохранить значения в файл конфигурации по причине его отсутствия", "Файл конфигурации отсутствует", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            RuntimeSettings.selfAddress = _windowViewModel.GetSelectedAddress().ToString();

            StartupLoader.SetLoadOnStartup((bool)LoadOnStartupCheckBox.IsChecked);

            MessageBox.Show("Файл конфигурации успешно сохранен", "Сохранение завершено", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TimeoutTextBoxKeyPreview(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void TimeoutTextBoxTextPreview(object sender, TextCompositionEventArgs e) => e.Handled = "0123456789".IndexOf(e.Text) < 0;

        private void FormKeyPreview(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();

            else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
                SaveButtonClicked(sender, e);
        }
        #endregion
    }
}
