using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.OISIC
{
    public class AK73XX_Ext : AK73XX , IAFunction , IOISFunction
    {
        public AK73XX_Ext()
        {
                
        }

        #region AF Function
        public int AF_Addr => throw new NotImplementedException();

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
                base.AK7314_Mode(ch, 0);
            else
                base.AK7314_Mode(ch, 1);
        }

        public void AFSleep(int ch)
        {
            throw new NotImplementedException();
        }

        public bool AF_ICReset(int ch)
        {
            AFOnOff(ch, false);
            Process.Wait(10);
            AF_Memory_Update(ch, 5);
            AFMove(ch, AF_MID_CODE);
            AFOnOff(ch, true);
            Process.AddLog(ch, $"AF was reset, 0x03 = 0x{Dln.ReadByte(ch, AF_Addr, 0x03, 1).ToString("x2")}");
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
        #endregion

        #region OIS Function
        public int OIS_Addr => throw new NotImplementedException();         ///기존 DLN은 OIS Slave ID 가 1개였다... Register로 구분하는 방식.... 추가 삭제 예정...
        public int OISX_Addr { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int OISY_Addr { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int OIS_MIN_CODE => throw new NotImplementedException();

        public int OIS_MID_CODE => throw new NotImplementedException();

        public int OIS_MAX_CODE => throw new NotImplementedException();

        public void LiearCompWrite(int axis, List<int> CompValue)
        {
            throw new NotImplementedException();
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
            
            if (isOn)
            {
                Dln.WriteArray(ch, this.XSlaveAddr, 0x02, 1, new byte[] { 0x40 });
                Dln.WriteArray(ch, this.Y1SlaveAddr, 0x02, 1, new byte[] { 0x40 });
                if (this.Y2SlaveAddr != 0x00)
                    Dln.WriteArray(ch, this.Y2SlaveAddr, 0x02, 1, new byte[] { 0x40 });
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
        #endregion
    }
}
