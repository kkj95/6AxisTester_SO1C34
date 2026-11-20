using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.OISIC.H503s.AKM
{
    public class H503_AKM : MCUH503
    {
        public H503_AKM(IOISFunction oISFunction, IOneTwoBytesDrivingIC controls) : base(oISFunction, controls)
        {

        }

        public override void SetI3CByPaaMode(bool Onoff)
        {
        }
        public override void SetH503WakeUp()
        { 
        }
        public override short GetI3CCheckBuffer(AxisTypeDW axisTypeDW, int bufferCheckType)
        {
            return (short)0x00;
        }
        public override ushort GetVersionChecked(AxisTypeDW axisTypeDW)
        {
            return 0x00;
        }
        public override void SetSWReset(bool OnOff)
        {
        }
        public override byte DriveICConnctChecked()
        {
            return 0x00;
        }
        public override void I3CStop()
        {
        }
    }
}
