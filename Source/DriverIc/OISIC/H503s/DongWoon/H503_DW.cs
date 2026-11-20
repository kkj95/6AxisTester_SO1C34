using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.OISIC.H503s.DongWoon
{
    public class H503_DW : MCUH503
    {
        public H503_DW(IOISFunction oISFunction, IOneTwoBytesDrivingIC controls) : base(oISFunction, controls)
        {
        }

        public override void SetI3CByPaaMode(bool Onoff)
        {
            int slaveId = GetAxisTypeID(AxisTypeDW.AxisX);
            if (Onoff)
                _controls.WriteByte(slaveId, 0xE6, 1, 0x01);
            else
                _controls.WriteByte(slaveId, 0xE6, 1, 0x00);
        }
        public override void SetH503WakeUp()
        {
            //int slaveId = GetAxisTypeID(AxisTypeDW.AxisY);
            //_controls.ReadByte(slaveId, 0x00, 1);

            OIS.ReadOISHall(0, 0, 0).ToString();
        }
        public override short GetI3CCheckBuffer(AxisTypeDW axisTypeDW, int bufferCheckType)
        {
            int slaveId = GetAxisTypeID(axisTypeDW);

            ushort readWord = 0x0000;
            if (bufferCheckType == 0)
                readWord = _controls.Read2Byte(slaveId, 0x40, 1);
            else
                readWord = _controls.Read2Byte(slaveId, 0x42, 1);

            var ReadData = (short)(readWord);

            return (short)ReadData;
        }
        public override ushort GetVersionChecked(AxisTypeDW axisTypeDW)
        {
            int slaveId = GetAxisTypeID(axisTypeDW);
            var readWord = _controls.Read2Byte(slaveId, 0x22, 1);
            return readWord;
        }
        public override void SetSWReset(bool OnOff)
        {
            //H503은 X축 및 Y축 아이디...
            int slaveId = GetAxisTypeID(AxisTypeDW.AxisX);
            if (OnOff)
                _controls.WriteByte(slaveId, 0x04, 1, 0x01);
            else
                _controls.WriteByte(slaveId, 0x04, 1, 0x03);
        }
        public override byte DriveICConnctChecked()
        {
            int slaveId = GetAxisTypeID(AxisTypeDW.AxisX);
            return _controls.ReadByte(slaveId, 0x1F, 1);
        }
        public override void I3CStop()
        {
            int slaveId = GetAxisTypeID(AxisTypeDW.AxisX);
            _controls.WriteByte(slaveId, 0x02, 1, 0x60);
        }
    }
}
