using Dln.I2cMaster;
using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.OISIC
{
    /// <summary>
    /// IC 동작에 구현체 
    /// </summary>
    //동윤보드 14bit Move 동작
    public class DW9836N : IOISFunction
    {
        private readonly IOneTwoBytesDrivingIC _controls;

        public int FRA_Addr { get; set; }
        public int OISX_Addr { get; set; }
        public int OISY_Addr { get; set; }
        public int OIS_MIN_CODE { get; set; }
        public int OIS_MID_CODE { get; set; }
        public int OIS_MAX_CODE { get; set; }

        public DW9836N(IOneTwoBytesDrivingIC controls)
        {
            _controls = controls;
        }
        public void OISMove(int ch, int Xcode, int Ycode)
        {
            _controls.Write2Byte(OISX_Addr, (int)eRegisterMapDW9836N.Target1, 2, (ushort)Xcode);
            _controls.Write2Byte(OISY_Addr, (int)eRegisterMapDW9836N.Target1, 2, (ushort)Ycode);
        }

        public void OISMoveOL(int ch, int axis, int code)
        {
            throw new NotImplementedException();
        }

        public void OISOnOff(int ch, bool isOn)
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
                default:
                    writeData = 0x00;
                    break;
            }

            _controls.WriteByte(SlaveID, (int)eRegisterMapDW9836N.Mode, 1, writeData);
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

            return (int)axisType;
        }
    }
}
