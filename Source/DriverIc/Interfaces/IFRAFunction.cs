using FZ4P.DriverIc.OISIC;
using FZ4P.Process.AMA._00.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.Interfaces
{
    public interface IFRAFunction
    {
        int FRA_Addr { get; }

        void FRA_Echoboard_StartStop(int ch, StartStopType type);
        bool Echo_Board_WhoAmI(int ch,int AxisType);
        void Echo_Board_Ready(int ch,int AxisType);
        void Echo_Board_SetErrorCount(int ch, int AxisType);
        void Echo_Board_SetParameter(Echo_ParamBase param);
        void Echo_Board_Select_Ch(int ch);
    }
}
