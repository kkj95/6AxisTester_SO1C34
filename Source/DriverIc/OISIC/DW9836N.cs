using Dln.I2cMaster;
using FZ4P.DriverIc.I2CBase;
using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace FZ4P.DriverIc.OISIC
{
    /// <summary>
    /// IC 동작에 구현체 
    /// </summary>
    //동윤보드 14bit Move 동작
    public class DW9836N : IOISFunction,IFRAFunction
    {
        private object _lock = new object();
        private readonly IOneTwoBytesDrivingIC _controls;

        public int FRA_Addr { get; set; } = 0x12;
        public int OISX_Addr { get; set; } = 0x0E;
        public int OISY_Addr { get; set; } = 0x4E;
        public int OIS_MIN_CODE { get; set; } = 0;
        public int OIS_MID_CODE { get; set; } = 8192;
        public int OIS_MAX_CODE { get; set; } = 16384;

        public IOneTwoBytesDrivingIC Controls => _controls;

        public DW9836N(IOneTwoBytesDrivingIC controls)
        {
            _controls = controls;
        }
        public void OISMove(int ch, int Xcode, int Ycode)
        {
            var moveX = Xcode << 2;
            var moveY = Ycode << 2;

            var targetBufferX1 = (moveX >> 8) & 0xFF;
            var targetBufferX2= (moveX) & 0xFF;

            var targetBufferY1 = (moveY >> 8) & 0xFF;
            var targetBufferY2 = (moveY) & 0xFF;

            Controls.WriteByte(OISX_Addr, (int)RegisterMapDW9836N.Target, 1, (byte)targetBufferX1);
            Controls.WriteByte(OISX_Addr, (int)RegisterMapDW9836N.Target1, 1, (byte)targetBufferX2);

            Controls.WriteByte(OISY_Addr, (int)RegisterMapDW9836N.Target, 1, (byte)targetBufferY1);
            Controls.WriteByte(OISY_Addr, (int)RegisterMapDW9836N.Target1, 1, (byte)targetBufferY2);
        }

        public void OISMoveOL(int ch, int axis, int code)
        {
            if (code < 0) code = 0;
            if (code > 0x1FFF) code = 0x1FFF; // 8191



            ushort raw = (ushort)(code << 3);

            byte data1 = (byte)((raw >> 8) & 0xFF);
            byte data2 = (byte)(raw & 0xFF);

            Controls.WriteByte(OISX_Addr, (int)RegisterMapDW9836N.Target, 1, data1);
            Controls.WriteByte(OISX_Addr, (int)RegisterMapDW9836N.Target1, 1, data2);

           
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
            short ReadData = 0x0000;

            int SlaveID = GetAxisTypeID((AxisTypeDW)axis);

            var Wrod = Controls.Read2Byte(SlaveID, (int)RegisterMapDW9836N.POSITION_READ_LOW, 1);

            ReadData = (short)(Wrod >> 2);

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
        public bool SetStore(int axis)
        {
            var slaveID = GetAxisTypeID((AxisTypeDW)axis);
            bool bResult = true;
            try
            {
                Controls.WriteByte(slaveID, (int)0x28, 1, (byte)0x39);
                Controls.WriteByte(slaveID, (int)0x28, 1, (byte)0xA0);
                Controls.WriteByte(slaveID, (int)RegisterMapDW9836N.Mode, 1, (byte)0x40);
                Controls.WriteByte(slaveID, (int)RegisterMapDW9836N.STORE_PROD_ID, 1, (byte)0x01);
                Thread.Sleep(640);
                Controls.WriteByte(slaveID, (int)0x28, 1, (byte)0x14);
                Controls.WriteByte(slaveID, (int)RegisterMapDW9836N.SWREST, 1, (byte)0x01);
            }
            catch
            {
                bResult = false;
            }

            return bResult;
        }

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
                case AxisTypeDW.AxisZ:
                    SlaveID = FRA_Addr;
                    break;
                default:
                    throw new Exception("Type Not Difined Error");
            }

            return SlaveID;
        }

        public void OISReset(int ch, int axis, bool OnOff)
        {
            byte data = 0x00;
            if (OnOff)
                data = 0x10;
            int slaveID = GetAxisTypeID((AxisTypeDW)axis);
            _controls.WriteByte(slaveID, (int)RegisterMapDW9836N.STORE_PROD_ID, 1, data);
        }
        
        public void Set_PT(int axis,bool OnOff)
        {
            var SlaveID = GetAxisTypeID((AxisTypeDW)axis);

            if (!OnOff) {
                _controls.WriteByte(SlaveID, 0x28, 1, 0x39);
                _controls.WriteByte(SlaveID, 0x28, 1, 0xA0);
            }
        }

        public void LiearCompWrite(int axis, List<int> CompValue)
        {
            var slaveID = GetAxisTypeID((AxisTypeDW)axis);
            Set_PT(axis, false);

            int startAddress = 0x55;
            _controls.WriteByte(slaveID, startAddress++, 1, 0x01);        // Linearity Enabled

            CompValue.ForEach(vlaue =>
            {
                _controls.WriteByte(slaveID, startAddress++, 1, (byte)vlaue);
            });
        }


        public void SetPCAL(int axis,int code)
        {
            var slaveID = GetAxisTypeID((AxisTypeDW)axis);
            _controls.WriteByte(slaveID, (int)RegisterMapPIDDW9836N.PCAL, 1, (byte)code);
        }
        public void SetNCAL(int axis, int code)
        {
            var slaveID = GetAxisTypeID((AxisTypeDW)axis);
            _controls.WriteByte(slaveID, (int)RegisterMapPIDDW9836N.NCAL, 1, (byte)code);
        }


        public void FRA_Echoboard_StartStop(int ch, StartStopType iStep)
        {
            if (iStep == StartStopType.Stop)
                _controls.WriteByte(ch, (int)RegisterMapFRA.FRA_START, 1, 0x00 ); //stop            
            else if (iStep == StartStopType.Start)
                _controls.WriteByte(ch, (int)RegisterMapFRA.FRA_START, 1, 0x03 ); //start
            else if (iStep == StartStopType.Ready)
                _controls.WriteByte(ch, (int)RegisterMapFRA.FRA_START, 1, 0x01); //Ready
        }

        public bool Echo_Board_WhoAmI(int ch)
        {
            byte[] board_info = new byte[1];
            var SlaveID =GetAxisTypeID(AxisTypeDW.AxisZ);

            board_info[0] = _controls.ReadByte(SlaveID, (int)RegisterMapFRA.BOARD_INFO, 1); 

            if (board_info[0] != 0xAE)
            {
                return false;
            }
            return true;
        }

        public void Echo_Board_Ready(int ch)
        {
            
        }

        public void Echo_Board_SetErrorCount(int ch)
        {
            throw new NotImplementedException();
        }

        public void Echo_Board_SetParameter(Echo_ParamBase param)
        {
            throw new NotImplementedException();
        }

        public void Echo_Board_Select_Ch(int ch)
        {
            if(ch == 1)
                _controls.WriteByte(FRA_Addr, (int)RegisterMapFRA.I2C_CH, 1, 0x01);
            else if(ch == 2)
                _controls.WriteByte(FRA_Addr, (int)RegisterMapFRA.I2C_CH, 1, 0x02);
            else
                _controls.WriteByte(FRA_Addr, (int)RegisterMapFRA.I2C_CH, 1, 0x01);
        }
    }
}
