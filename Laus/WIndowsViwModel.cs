using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laus
{
    internal class WindowsViewModel
    {
        private ObservableCollection<DeviceViewModel> _lanDevices;

        public ObservableCollection<DeviceViewModel> LanDevices
        {
            get => _lanDevices;
            set { _lanDevices = value; }
        }

        public WindowsViewModel()
        {
            LanDevices = new ObservableCollection<DeviceViewModel>();
        }
    }
}
