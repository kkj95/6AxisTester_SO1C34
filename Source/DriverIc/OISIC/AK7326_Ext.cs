using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace FZ4P.DriverIc.OISIC
{
    public class AK7326_Ext : AK73XX , IAFunction , IOISFunction, IFRAFunction
    {
        private Action<int, string> _logAction;
        private readonly IOneTwoBytesDrivingIC _controls;
        public AK7326_Ext(Action<int, string> logAction)
        {
            _logAction = logAction;
        }

        public override bool WriteArray(int ch, int slaveAddr, int memAddr, int memCnt, byte[] data, bool logAction = true)
        {
            bool result = base.WriteArray(ch, slaveAddr, memAddr, memCnt, data);
            string text = string.Join(",", data.Select(x => $"0x{x:X2}"));

            var fulldata = data;
            if (_logAction != null && logAction)
                _logAction(ch, $"Write : 0x{memAddr.ToString("X2")},0x{text}");

            return result;
        }

        public override bool ReadArray(int ch, int slaveAddr, int memAddr, int memCnt, byte[] data, bool logAction = true)
        {
            bool result = base.ReadArray(ch, slaveAddr, memAddr, memCnt, data);
            string text = string.Join(",", data.Select(x => $"0x{x:X2}"));

            if (_logAction != null && logAction)
                _logAction(ch, $"Read : 0x{memAddr.ToString("X2")},0x{text}");

            return result;
        }


        #region AF Function
        public int AF_Addr => base.AFSlaveAddr;

        public int AF_MID_CODE => 2048;

        public int AF_MIN_CODE => 0;

        public int AF_MAX_CODE => 4096;

        public void AFMove(int ch, int code)
        {
            int data = code << 4;
            byte[] buff = new byte[2] { (byte)(data >> 8), (byte)(data % 256) };

            Dln.WriteArray(ch, AFSlaveAddr, 0x00, 1, buff);
        }

        public void AFMoveOL(int ch, int code)
        {
            throw new NotImplementedException();
        }

        public void AFOnOff(int ch, bool isOn)
        {
            if(isOn)
                base.AK7314_Mode(ch, 1);
            else
                base.AK7314_Mode(ch, 0);
        }

        public void AFSleep(int ch)
        {
            throw new NotImplementedException();
        }

        public bool AF_ICReset(int ch)
        {
            //AFOnOff(ch, false);
            //Process.Wait(10);
            //AF_Memory_Update(ch, 5);
            //AFMove(ch, AF_MID_CODE);
            //AFOnOff(ch, true);
            //Process.AddLog(ch, $"AF was reset, 0x03 = 0x{Dln.ReadByte(ch, AF_Addr, 0x03, 1).ToString("x2")}");
            //return true;

            byte[] rbuf = new byte[1];
            Dln.WriteArray(ch, AFSlaveAddr, 0x02, 1, new byte[] { 0x40 });
            Thread.Sleep(100);
            Dln.WriteArray(ch, AFSlaveAddr, 0x03, 1, new byte[] { 0x10 });
            Thread.Sleep(150);
            Dln.ReadArray(ch, AFSlaveAddr, 0x4B, 1, rbuf);
            if ((byte)(rbuf[0] & 0x04) != 0x00)
            {
                _logAction(ch, "Store fail");
                return false;
            }
            Dln.WriteArray(ch, AFSlaveAddr, 0x02, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, AFSlaveAddr, 0x00, 1, new byte[] { 0x80, 0x00 });
            Thread.Sleep(100);
            return true;
        }

        public (int, int) AF_IC_Data(int ch)
        {
            throw new NotImplementedException();
        }

        public bool AF_Memory_Update(int ch, int mode)
        {
            return base.AK7314_memory_update(ch, (byte)mode);
        }

        public bool ChangeSlaveAddr(int ch)
        {
            throw new NotImplementedException();
        }

        public int ReadAFHall(int ch)
        {
            return base.ReadHall(ch, "AF");
        }

        public void AF_LinearityComp_Reset(int ch)
        {
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x30, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x31, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x32, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x33, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x34, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x35, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x36, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x37, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x38, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x39, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x3A, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x3B, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, this.AFSlaveAddr, 0x3C, 1, new byte[] { 0x00 });
        }
        #endregion

        #region OIS Function
        public int OIS_Addr => throw new NotImplementedException();         ///기존 DLN은 OIS Slave ID 가 1개였다... Register로 구분하는 방식.... 추가 삭제 예정...

        public int OISX_Addr { get; set; } = 0x0E;
        public int OISY_Addr { get; set; } = 0x4E;

        public int OIS_MIN_CODE { get; set; } = 0;
        public int OIS_MID_CODE { get; set; } = 2048;
        public int OIS_MAX_CODE { get; set; } = 4096;

        public void LiearCompWrite(int axis, List<int> CompValue)
        {
            throw new NotImplementedException();
        }
        public byte LiearCompEnable(int axis, bool enable)
        {
            throw new NotImplementedException();
        }

        public void OISICReset(int ch)
        {
            throw new NotImplementedException();
        }

        public void OISMove(int ch, int Xcode, int Ycode)
        {
            var moveX = Xcode << 3;
            var moveY = Ycode << 3;

            var targetBufferX1 = (moveX >> 8) & 0xFF;
            var targetBufferX2 = (moveX) & 0xFF;

            var targetBufferY1 = (moveY >> 8) & 0xFF;
            var targetBufferY2 = (moveY) & 0xFF;

            Dln.WriteByte(ch, OISX_Addr, (int)RegisterMap7326.Target, 1, (byte)targetBufferX1);
            Dln.WriteByte(ch, OISX_Addr, (int)RegisterMap7326.Target1, 1, (byte)targetBufferX2);

            Dln.WriteByte(ch, OISY_Addr, (int)RegisterMap7326.Target, 1, (byte)targetBufferY1);
            Dln.WriteByte(ch, OISY_Addr, (int)RegisterMap7326.Target1, 1, (byte)targetBufferY2);
        }

        public void OISMoveOL(int ch, int axis, int code)
        {
            throw new NotImplementedException();
        }

        public void OISOnOff(int ch, bool isOn)
        {
            if (isOn)
            {
                Dln.WriteArray(ch, OISX_Addr, (int)RegisterMap7326.Mode, 1, new byte[] { 0x00 });
                Dln.WriteArray(ch, OISY_Addr, (int)RegisterMap7326.Mode, 1, new byte[] { 0x00 });
            }
            else
            {
                Dln.WriteArray(ch, OISX_Addr, (int)RegisterMap7326.Mode, 1, new byte[] { 0x40 });
                Dln.WriteArray(ch, OISY_Addr, (int)RegisterMap7326.Mode, 1, new byte[] { 0x40 });
            }
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
            short ReadData = 0x0000;

            int SlaveID = GetAxisTypeID((AxisTypeDW)axis);
            var Wrod = Dln.Read2Byte(ch, SlaveID, (int)RegisterMap7326.POSITION_READ_LOW, 1);
            ReadData = (short)(Wrod >> 3);

            return (short)ReadData;
        }

        public bool SetManualDrvModeXY(int ch, int MidCodeX, int MidCodeY)
        {
            bool flag = false;
            OISMove(ch, MidCodeX, MidCodeY);
            return true;
        }
        private void SettingMode(int ch, int axis, bool OnOff)
        {
            var slaveID = GetAxisTypeID((AxisTypeDW)axis);
            if (OnOff)
            {
                if (!Dln.WriteArray(ch, slaveID, 0xAE, 1, new byte[] { 0x3B })) return;
                    _logAction(ch, string.Format("Setting Mode = Write Mem : 0x{0:X2} XData : 0x{1:X2}", 0xAE, 0x3B));
            }
            else
            {
                if (!Dln.WriteArray(ch, slaveID, 0xAE, 1, new byte[] { 0x00 })) return;
                    _logAction(ch, string.Format("Setting Mode = Write Mem : 0x{0:X2} XData : 0x{1:X2}", 0xAE, 0x3B));
            }
        }
        public bool SetStore(int axis)
        {
            var slaveID = GetAxisTypeID((AxisTypeDW)axis);
            bool bResult = true;
            try
            {
                SettingMode(0, axis, true);
                Thread.Sleep(50);
                Dln.WriteByte(0, slaveID, (int)RegisterMap7326.STORE_PROD_ID, 1, (byte)0x01);
                Thread.Sleep(200);
                _logAction(0, string.Format("Store Memory = Write Mem : 0x{0:X2} Data : 0x{1:X2}", 0x03, 0x01));
            }
            catch
            {
                bResult = false;
            }

            return bResult;
        }
        #endregion

        #region FRA Function

        public void FRA_Echoboard_StartStop(int ch, StartStopType type)
        {
            throw new NotImplementedException();
        }

        public void AMA_Echoboard_StartStop(int ch, StartStopType type)
        {
            throw new NotImplementedException();
        }

        public bool Echo_Board_WhoAmI(int ch)
        {
            throw new NotImplementedException();
        }

        public void Echo_Board_SetParameter(Echo_ParamBase param)
        {
            throw new NotImplementedException();
        }

        public void Echo_Board_Select_Ch(int ch)
        {
            throw new NotImplementedException();
        }

        public double GetCurrent(int axis)
        {
            throw new NotImplementedException();
        }

        public void Set_FRA_I2C_Speed()
        {
            throw new NotImplementedException();
        }

        public void Set_Control_Freq(ControlFreqMode controlFreqMode)
        {
            throw new NotImplementedException();
        }

        #endregion

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
                    SlaveID = AF_Addr;
                    break;
                default:
                    throw new Exception("Type Not Difined Error");
            }

            return SlaveID;
        }
    }
}
