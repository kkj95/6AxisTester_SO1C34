using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.OISIC
{
    public class OISAdapterAKM : IOISFunction
    {
        public int OIS_MIN_CODE => throw new NotImplementedException();
        public int OIS_MID_CODE => throw new NotImplementedException();
        public int OIS_MAX_CODE => throw new NotImplementedException();

        public readonly AK73XX _ak73xx;

        public OISAdapterAKM(AK73XX ak73xx)
        {
            _ak73xx = ak73xx;
        }

        public void LiearCompWrite(int axis, List<int> CompValue)
        {
            
        }
        public void OISICReset(int ch)
        {
            throw new NotImplementedException();
        }
        public void OISMove(int ch, int Xcode, int Ycode)
        {
            throw new NotImplementedException();
        }
        public void OISMoveOL(int ch, int axis, int code)
        {
            throw new NotImplementedException();
        }
        public void OISOnOff(int ch, bool isOn)
        {
            throw new NotImplementedException();
        }
        public void OISReset(int ch, int axis, bool OnOff)
        {
            throw new NotImplementedException();
        }
        public bool OIS_StausCheck(int ch, byte res1, byte res2)
        {
            throw new NotImplementedException();
        }
        public bool OIS_StausCheck(int ch, int memAddr, byte res1, byte res2)
        {
            throw new NotImplementedException();
        }
        public short ReadOISHall(int ch, int axis, int mode)
        {
            throw new NotImplementedException();
        }
        public bool SetManualDrvModeXY(int ch, int MidCodeX, int MidCodeY)
        {
            throw new NotImplementedException();
        }
        public bool SetStore(int axis)
        {
            throw new NotImplementedException();
        }
    }
}
