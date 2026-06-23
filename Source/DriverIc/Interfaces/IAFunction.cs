using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.Interfaces
{
    public interface IAFunction
    {
        //ReadOnly Properties
        int AF_MID_CODE { get; }
        int AF_MIN_CODE { get; }
        int AF_MAX_CODE { get; }

        (int, int) AF_IC_Data(int ch);
        bool AF_ICReset(int ch);
        void AFSleep(int ch);
        void AFOnOff(int ch, bool isOn);
        void AFMove(int ch, int code);
        void AFMoveOL(int ch, int code);
        int ReadAFHall(int ch);

        bool FRAModeEnable(int ch);
        bool FRAModeDisable(int ch);

        bool ChangeSlaveAddr(int ch);
        bool AF_Memory_Update(int ch, int mode);
    }
}
