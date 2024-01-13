using Laus.Models;
using Laus.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Laus.ViewModels
{
    /// <summary>
    /// Модель для описания окна настроек
    /// </summary>
    internal class SettingsViewModel : ViewModel
    {
        private string _version;
        private List<IPAddress> _selfAddresses;
        private int _selectedAddressIndex;

        public string Version
        {
            get => _version;
            set => Set(ref _version, value);
        }
        public List<IPAddress> SelfAddresses
        {
            get => _selfAddresses;
            set => Set(ref _selfAddresses, value);
        }
        public int SelectedAddressIndex
        {
            get => _selectedAddressIndex;
            set => Set(ref _selectedAddressIndex, value);
        }

        public void SetSelectedAddress(string address)
        {
            foreach (var item in SelfAddresses)
            {
                if (item.ToString() == address)
                {
                    SelectedAddressIndex = SelfAddresses.IndexOf(item);
                    break;
                }
            }
        }
        public IPAddress GetSelectedAddress() => SelfAddresses[SelectedAddressIndex];
    }
}
