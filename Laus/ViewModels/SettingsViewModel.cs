using Laus.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laus.ViewModels
{
    internal class SettingsViewModel : ViewModel
    {
        private string _version;

        public string Version
        {
            get => _version;
            set => Set(ref _version, value);
        }
    }
}
