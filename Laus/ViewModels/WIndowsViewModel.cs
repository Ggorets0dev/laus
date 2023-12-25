using Laus.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laus
{
    internal class WindowsViewModel : ViewModel
    {
        private const string _defaultOperationStatus = "Отсутствует";
        private const string _specsPlaceholder = "Не определены";

        private ObservableCollection<DeviceViewModel> _lanDevices;
        private string _operationStatus;
        private string _specs;

        public ObservableCollection<DeviceViewModel> LanDevices
        {
            get => _lanDevices;
            set => Set(ref _lanDevices, value);
        }
        public string OperationStatus 
        { 
            get => _operationStatus;
            set => Set(ref _operationStatus, value);
        }
        public string Specs
        {
            get => _specs;
            set => Set(ref _specs, value);
        }

        public int SelectedIndex { get; set; }

        public WindowsViewModel()
        {
            _lanDevices = new ObservableCollection<DeviceViewModel>();
            ResetSelectedIndex();
            ResetStatus();
            ResetSpecs();
        }

        public void ResetSelectedIndex() => SelectedIndex = -1;
        public DeviceViewModel GetSelectedItem() => LanDevices[SelectedIndex];
        public void ResetStatus() => OperationStatus = _defaultOperationStatus;
        public void ResetSpecs() => Specs = _specsPlaceholder;

    }
}
