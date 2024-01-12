using Laus.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laus
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly string _defaultOperationStatus = "Отсутствует";
        private readonly string _specsPlaceholder = "Не определены";

        private ObservableCollection<DeviceViewModel> _lanDevices;
        private string _operationStatus;
        private string _specs;
        private bool _controlPanelEnabled;
        private bool _lanDevicesListEnabled;

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

        public bool ControlPanelEnabled
        {
            get => _controlPanelEnabled;
            set => Set(ref _controlPanelEnabled, value);
        }
        public bool LanDevicesListEnabled
        {
            get => _lanDevicesListEnabled;
            set => Set(ref _lanDevicesListEnabled, value);
        }

        public int SelectedAddressIndex { get; set; }

        public MainWindowViewModel()
        {
            _lanDevices = new ObservableCollection<DeviceViewModel>();
            _controlPanelEnabled = true;
            _lanDevicesListEnabled = true;

            ResetSelectedAddressIndex();
            ResetStatus();
            ResetSpecification();
        }

        public void ResetSelectedAddressIndex() => SelectedAddressIndex = -1;
        public DeviceViewModel GetSelectedAddress() => LanDevices[SelectedAddressIndex];
        public void ResetStatus() => OperationStatus = _defaultOperationStatus;
        public void ResetSpecification() => Specs = _specsPlaceholder;

    }
}
