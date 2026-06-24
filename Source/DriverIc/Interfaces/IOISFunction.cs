using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.Interfaces
{
    public interface IOISFunction
    {
        int OIS_MIN_CODE { get; }
        int OIS_MID_CODE { get; }
        int OIS_MAX_CODE { get; }


        void OISICReset(int ch);
        void OISOnOff(int ch, bool isOn);
        bool OIS_StausCheck(int ch, byte res1, byte res2);

        bool OIS_StausCheck(int ch, int memAddr, byte res1, byte res2);

        bool SetManualDrvModeXY(int ch, int MidCodeX, int MidCodeY);
        void OISMove(int ch, int Xcode, int Ycode);

        void OISMoveOL(int ch, int axis, int code);
        short ReadOISHall(int ch, int axis, int mode);

        void OISReset(int ch, int axis, bool OnOff);

        bool SetStore(int axis);

        void LiearCompWrite(int axis, List<int> CompValue);
    }
}
