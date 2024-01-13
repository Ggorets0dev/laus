using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laus.Models
{
    /// <summary>
    /// Команды между клиентом и сервером
    /// </summary>
    internal enum LanCommandCodes
    {
        CheckUser,
        ApproveUser,
        RequestSpecs,
        ResponseSpecs
    }
}
