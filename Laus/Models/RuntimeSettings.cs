using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laus.Models
{
    /// <summary>
    /// Настройки, сбрасываемые при каждом перезапуске ПО
    /// </summary>
    static internal class RuntimeSettings
    {
        static private string _defaultSelfAddress = "127.0.0.1";
        static public string selfAddress {  get; set; }

        static RuntimeSettings() => selfAddress = _defaultSelfAddress;

        static public bool IsAddressSelected() => selfAddress != _defaultSelfAddress;
    }
}
