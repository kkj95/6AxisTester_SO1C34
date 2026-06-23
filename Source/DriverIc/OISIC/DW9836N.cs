using Dln.I2cMaster;
using FZ4P.DriverIc.I2CBase;
using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace FZ4P.DriverIc.OISIC
{
    /// <summary>
    /// IC 동작에 구현체 
    /// </summary>
    //동윤보드 14bit Move 동작
    public class DW9836N : IOISFunction
    {
        private readonly IOneTwoBytesDrivingIC _controls;

        public int FRA_Addr { get; set; } = 0x12;
        public int OISX_Addr { get; set; } = 0x0E;
        public int OISY_Addr { get; set; } = 0x4E;
        public int OIS_MIN_CODE { get; set; } = 0;
        public int OIS_MID_CODE { get; set; } = 2048;
        public int OIS_MAX_CODE { get; set; } = 4095;

        public IOneTwoBytesDrivingIC Controls => _controls;

        public DW9836N(IOneTwoBytesDrivingIC controls)
        {
            _controls = controls;
        }
        public void OISMove(int ch, int Xcode, int Ycode)
        {
            var positionX = Xcode;// << 3;
            var positionY = Ycode;// << 3;

            byte[] tmp = new byte[] { (byte)((positionX >> 6) & 0xff), (byte)(positionX & 0x3f) };
            tmp[1] = (byte)(tmp[1] << 2);

            Controls.Write2Byte(OISX_Addr, (int)RegisterMapDW9836N.Target, 2, (ushort)positionX);
            Controls.Write2Byte(OISY_Addr, (int)RegisterMapDW9836N.Target, 2, (ushort)positionY);
        }

        public void OISMoveOL(int ch, int axis, int code)
        {
            throw new NotImplementedException();
        }

        public void OISOnOff(int ch, bool isOn)
        {
            //임시
            var axisTypeX = AxisTypeDW.AxisX;
            var axisTypeY = AxisTypeDW.AxisY;

            if (isOn)
            {
                SetOperationMode(axisTypeX, OperationTypeDW.StandbyMode);
                SetOperationMode(axisTypeX, OperationTypeDW.ClosedMode);

                SetOperationMode(axisTypeY, OperationTypeDW.StandbyMode);
                SetOperationMode(axisTypeY, OperationTypeDW.ClosedMode);
            }
            else
            {
                SetOperationMode(axisTypeX, OperationTypeDW.StandbyMode);
                SetOperationMode(axisTypeY, OperationTypeDW.StandbyMode);
            }
        }

        public void OISICReset(int ch)
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
            int SlaveID = GetAxisTypeID((AxisTypeDW)axis);

            var ReadData = Controls.Read2Byte(SlaveID, (int)RegisterMapDW9836N.POSITION_READ_LOW, 2);
            if (mode == 0)
            {
                //short Readhall = (short)((data[1] << 8 | data[2]) / 16);
                //if (Readhall >= 2048) Readhall = (short)(Readhall - 4096);
                //return Readhall;

            }
            else
            {
                //if (axis == 0)
                //{
                //    Dln.WriteByte(ch, OIS_Addr, 0x6060, 2, 0x00);
                //    bool res = OIS_StausCheck(ch, 0x6060, 0x00, 0x00);
                //    if (!res) return short.MaxValue;
                //}
                //else
                //{
                //    Dln.WriteByte(ch, OIS_Addr, 0x6060, 2, 0x01);
                //    bool res = OIS_StausCheck(ch, 0x6060, 0x01, 0x01);
                //    if (!res) return short.MaxValue;
                //}
                //return Dln.Read2Byte_signed(ch, OIS_Addr, 0x6062, 2);
            }

            return (short)ReadData;
        }

        public bool SetManualDrvModeXY(int ch, int MidCodeX, int MidCodeY)
        {
            bool flag = false;
            //flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
            //_controls.WriteByte(ch, OIS_Addr, 0x617A, 2, 0x01);
            //flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
            //_controls.WriteByte(ch, OIS_Addr, 0x6020, 2, 0x07);
            //flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
            OISMove(ch, MidCodeX, MidCodeY);

            return true;
        }

        //Store 명령
        //public bool SetStore()
        //{
        //    byte byteData = 0x40;
        //    return _controls.WriteByte(OISX_Addr, 0X28, 1, byteData);
        //}

        //public bool PT_OFF()
        //{
        //    byte byteData = 0x40;
        //    _controls.WriteByte(OISX_Addr, 0X28, 1, byteData);
        //}

        /// <summary>
        /// 기본 close 모드
        /// </summary>
        /// <param name="axisType"></param>
        /// <param name="ModeType"></param>
        public void SetOperationMode(AxisTypeDW axisType, OperationTypeDW ModeType)
        {
            int SlaveID = GetAxisTypeID(axisType);
            byte writeData = 0x00;
            switch (ModeType)
            {
                case OperationTypeDW.StandbyMode:
                    writeData = 0x40;
                    break;
                case OperationTypeDW.SleepMode:
                    writeData = 0x20;
                    break;
                case OperationTypeDW.ClosedMode:
                    writeData = 0x00;
                    break;
                case OperationTypeDW.OpenMode:
                    writeData = 0x03;
                    break;
                default:
                    writeData = 0x00;
                    break;
            }

            Controls.WriteByte(SlaveID, (int)RegisterMapDW9836N.Mode, 1, writeData);
        }
        private int GetAxisTypeID(AxisTypeDW axisType)
        {
            int SlaveID = -1;
            switch (axisType)
            {
                case AxisTypeDW.AxisX:
                    SlaveID = OISX_Addr;
                    break;
                case AxisTypeDW.AxisY:
                    SlaveID = OISY_Addr;
                    break;
                default:
                    throw new Exception("Type Not Difined Error");
            }

            return SlaveID;
        }
    }
}
