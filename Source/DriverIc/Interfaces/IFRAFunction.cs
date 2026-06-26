using FZ4P.DriverIc.OISIC;
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
        bool Echo_Board_WhoAmI(int ch);
    }
}
