using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.OISIC.Adapter
{
    public class FRAAdapterAKM : IFRAFunction
    {
        public readonly AK73XX _ak73xx;
        public FRAAdapterAKM(AK73XX ak73xx)
        {
            _ak73xx = ak73xx;
        }

        public int FRA_Addr => throw new NotImplementedException();

        public void AMA_Echoboard_StartStop(int ch, StartStopType type)
        {
            throw new NotImplementedException();
        }

        public void Echo_Board_Select_Ch(int ch)
        {
            throw new NotImplementedException();
        }

        public void Echo_Board_SetParameter(Echo_ParamBase param)
        {
            throw new NotImplementedException();
        }

        public bool Echo_Board_WhoAmI(int ch)
        {
            throw new NotImplementedException();
        }

        public void FRA_Echoboard_StartStop(int ch, StartStopType type)
        {
            throw new NotImplementedException();
        }
    }
}
