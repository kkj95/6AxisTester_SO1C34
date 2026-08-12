using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using FZ4P.DriverIc.OISIC.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.OISIC
{
    public class MCUH503 : DriveICBase,I2CTOI3C_Function
    {
        private readonly IOneTwoBytesDrivingIC _controls = null;
        public IOISFunction OIS { get; }

        public MCUH503(IOISFunction oISFunction, IOneTwoBytesDrivingIC controls)
        {
            OIS = oISFunction;
            _controls = controls;
        }

        #region public
        public void SetI3CByPaaMode(bool Onoff)
        {
            int slaveId = GetAxisTypeID(AxisTypeDW.AxisY);
            if (Onoff)
                _controls.WriteByte(slaveId, 0xE6, 1, 0x01);
            else
                _controls.WriteByte(slaveId, 0xE6, 1, 0x00);
        }
        public void SetH503WakeUp()
        {
            int slaveId = GetAxisTypeID(AxisTypeDW.AxisY);
            _controls.WriteByte(slaveId, 0x00, 1, 0x00);
        }
        public short GetI3CCheckBuffer(AxisTypeDW axisTypeDW,int bufferCheckType)
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
        #endregion

        #region Private
        private int GetAxisTypeID(AxisTypeDW axisType)
        {
            int SlaveID = -1;
            switch (axisType)
            {
                case AxisTypeDW.AxisX:
                    SlaveID = OIS.OISX_Addr;
                    break;
                case AxisTypeDW.AxisY:
                    SlaveID = OIS.OISY_Addr;
                    break;
                default:
                    throw new Exception("Type Not Difined Error");
            }

            return SlaveID;
        }

        public ushort GetVersionChecked(AxisTypeDW axisTypeDW)
        {
            int slaveId = GetAxisTypeID(axisTypeDW);
            var readWord = _controls.Read2Byte(slaveId, 0x22, 1);
            return readWord;
        }
        #endregion
    }
}
