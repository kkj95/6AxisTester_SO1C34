using FZ4P.DriverIc.OISIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FZ4P.Extensions
{
    public static class DLNExtension
    {
        public static DLN I3CStop(this DLN dLN, MCUH503 mcu503)
        {
            mcu503.I3CStop();
            Thread.Sleep(200);
            return dLN;
        }

        public static DLN HWReset(this DLN dLN, MCUH503 mcu503)
        {
            mcu503.SetSWReset(false);
            Thread.Sleep(500);
            return dLN;
        }

        public static DLN Connected(this DLN dLN, MCUH503 mcu503)
        {
            mcu503.DriveICConnctChecked();
            Thread.Sleep(30);
            return dLN;
        }
    }
}
