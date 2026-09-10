using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{
    public class PIDResult
    {
        public byte[] SettingRegister { get; set; } = new byte[1];
        public byte[] SettingRegisterValue { get; set; } = new byte[1];

        public byte[] Register { get; set; } = new byte[1];
        public byte[] RegisterValue { get; set; } = new byte[1];
        public byte Version { get; set; } = 0x00;
    }
}
