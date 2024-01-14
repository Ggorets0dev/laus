using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Laus.Models
{
    /// <summary>
    /// Управление запуском приложения при загрузке Windows
    /// </summary>
    static internal class StartupLoader
    {
        static private readonly string _subKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        static private readonly string _appName = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

        /// <summary>
        /// Изменить состояние запуска при загрузке системы
        /// </summary>
        /// <param name="state">Включить / Выключить запуск при загрузке системы</param>
        static public void SetLoadOnStartup(bool state)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(_subKeyPath, true);

            string executablePath = '"' + System.Windows.Forms.Application.ExecutablePath.Replace("/", "\\") + '"' + " --hidden";

            if (state)
                rk.SetValue(_appName, executablePath);
            else
                rk.DeleteValue(_appName, false);

            rk.Close();
        }

        /// <summary>
        /// Проверить, включен ли запуск при загрузке системы
        /// </summary>
        /// <returns>Установлен ли запуск при загрузке системы</returns>
        static public bool IsLoadOnStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(_subKeyPath, true);
            var loadPath = rk.GetValue(_appName);
            rk.Close();

            return loadPath != null;
        }
    }
}
