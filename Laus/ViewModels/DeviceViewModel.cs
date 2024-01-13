using Laus.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laus
{
    /// <summary>
    /// Модель для описания устройства в локальной сети
    /// </summary>
    internal class DeviceViewModel : ViewModel
    {   
        private string _ipAddress;
        private string _alias;

        public string IpAddress 
        { 
            get => _ipAddress; 
            set => Set(ref _ipAddress, value); 
        }
        public string Alias
        {
            get => _alias;
            set => Set(ref _alias, value);
        }
    }
}
