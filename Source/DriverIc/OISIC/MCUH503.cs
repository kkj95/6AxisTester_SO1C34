using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using FZ4P.DriverIc.OISIC.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace FZ4P.DriverIc.OISIC
{
    public class MCUH503 : DriveICBase,I2CTOI3C_Function
    {
        protected readonly IOneTwoBytesDrivingIC _controls = null;
        public IOISFunction OIS { get; }

        public MCUH503(IOISFunction oISFunction, IOneTwoBytesDrivingIC controls)
        {
            OIS = oISFunction;
            _controls = controls;
        }

        //TODO : 테스트 가능시 메서드 처리 제거함.
        #region public DW 기능들...
        public virtual void SetI3CByPaaMode(bool Onoff)
        {
            int slaveId = GetAxisTypeID(AxisTypeDW.AxisX);
            if (Onoff)
                _controls.WriteByte(slaveId, 0xE6, 1, 0x01);
            else
                _controls.WriteByte(slaveId, 0xE6, 1, 0x00);
        }
        public virtual void SetH503WakeUp()
        {
            //int slaveId = GetAxisTypeID(AxisTypeDW.AxisY);
            //_controls.ReadByte(slaveId, 0x00, 1);

            OIS.ReadOISHall(0, 0, 0).ToString();
        }
        public virtual short GetI3CCheckBuffer(AxisTypeDW axisTypeDW,int bufferCheckType)
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
        public virtual ushort GetVersionChecked(AxisTypeDW axisTypeDW)
        {
            int slaveId = GetAxisTypeID(axisTypeDW);
            var readWord = _controls.Read2Byte(slaveId, 0x22, 1);
            return readWord;
        }
        public virtual void SetSWReset(bool OnOff)
        { 
            //H503은 X축 및 Y축 아이디...
            int slaveId = GetAxisTypeID(AxisTypeDW.AxisX);
            if (OnOff)
                _controls.WriteByte(slaveId, 0x04, 1, 0x01);
            else
                _controls.WriteByte(slaveId, 0x04, 1, 0x03);
        }
        public virtual byte DriveICConnctChecked()
        {
            int slaveId = GetAxisTypeID(AxisTypeDW.AxisX);
            return _controls.ReadByte(slaveId, 0x1F, 1);
        }
        public virtual void I3CStop()
        {
            int slaveId = GetAxisTypeID(AxisTypeDW.AxisX);
            _controls.WriteByte(slaveId, 0x02, 1, 0x60);
        }
        #endregion

        #region Private
        protected int GetAxisTypeID(AxisTypeDW axisType)
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
        #endregion
    }
}
