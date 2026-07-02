using Dln;
using FZ4P.DriverIc.Interfaces;
using MathNet.Numerics.Distributions;
using OpenCvSharp.Dnn;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace FZ4P
{

    public class DrvIC : IOISFunction,IAFunction
    { 
        public Process Process { get { return STATIC.Process; } }
        public DLN Dln { get { return STATIC.Dln; } }
        public int FRA_Addr { get; set; }
        public int AF_Addr { get; set; }
        public int OIS_Addr { get; set; }
        public int AF_MID_CODE { get; set; }
        public int AF_MIN_CODE { get; set; }
        public int AF_MAX_CODE { get; set; }
        public int OIS_MID_CODE { get; set; }
        public int OIS_MIN_CODE { get; set; }
        public int OIS_MAX_CODE { get; set; }

        public DrvIC()
        {
            AF_Addr = 0x0C;
            OIS_Addr = 0x3C;
            FRA_Addr = 0x14;
            AF_MID_CODE = 2048;
            AF_MAX_CODE = 4095;
            AF_MIN_CODE = 0;
            OIS_MID_CODE = 0;

        }

        #region AF Seq

        public (int, int) AF_IC_Data(int ch)
        {
            Dln.WriteByte(ch, AF_Addr, 0xAE, 1, 0x3B);
            AF_Memory_Update(ch, 5);
            Dln.WriteByte(ch, AF_Addr, 0xAE, 1, 0x00);
            int target_code = ((Dln.Read2Byte(ch, AF_Addr, 0x00, 1) >> 4) & 0x0FFF);
            int current_code = ReadAFHall(ch);
            int poscal = (Dln.Read2Byte_signed(ch, AF_Addr, 0x04, 1) >> 1) & 0x7FFF;
            int negcal = (Dln.Read2Byte_signed(ch, AF_Addr, 0x06, 1) >> 1) & 0x7FFF;
            int posvt = (Dln.Read2Byte_signed(ch, AF_Addr, 0xC0, 1) >> 6) & 0x03FF;
            int negvt = (Dln.Read2Byte_signed(ch, AF_Addr, 0xC2, 1) >> 6) & 0x03FF;

            Process.AddLog(ch, $"tag : {target_code}, cur : {current_code}");
            Process.AddLog(ch, $"pcal : {poscal}, ncal : {negcal}");
            Process.AddLog(ch, $"pvt : {posvt}, nvt : {negvt}");


            return (poscal, negcal);

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
        public void AFSleep(int ch)
        {
            Dln.WriteByte(ch, AF_Addr, 0x02, 1, 0x20);
            Process.AddLog(ch, $"AF Sleep Mode");

        }
        public void AFOnOff(int ch, bool isOn)
        {
            if (isOn)
            {
                Dln.WriteByte(ch, AF_Addr, 0x02, 1, 0x00);
                Process.AddLog(ch, $"AF Servo On");
            }
            else
            {
                Dln.WriteByte(ch, AF_Addr, 0x02, 1, 0x40);
                Process.AddLog(ch, $"AF Servo Off");
            }
            Process.Wait(10);
        }
        public void AFMove(int ch, int code)
        {
            if ((code & 0x8000) != 0) code = 0;
            code = code & 0x0FFF;
            Dln.Write2Byte(ch, AF_Addr, 0x00, 1, (ushort)(code << 4));
        }
        public void AFMoveOL(int ch, int code)
        {
            Dln.Write2Byte(ch, AF_Addr, 0x00, 1, (ushort)((code & 0x03FF) << 6));
        }
        public int ReadAFHall(int ch)
        {
            int a = (Dln.Read2Byte(ch, AF_Addr, 0x84, 1) >> 4) & 0x0FFF;
            return a;
        }
     
        public bool FRAModeEnable(int ch)
        {
            Process.AddLog(ch, $"FRA Mode Enable");
            if (!Dln.WriteByte(ch, FRA_Addr, 0x56, 1, 0x80)) return false;
            Process.AddLog(ch, string.Format("Write Mem : 0x{0:X2} Data : 0x{1:X2}", 0x56, 0x80));
            if (!Dln.WriteByte(ch, FRA_Addr, 0xAC, 1, 0x01)) return false;
            Process.AddLog(ch, string.Format("Write Mem : 0x{0:X2} Data : 0x{1:X2}", 0xAC, 0x01));
            Process.Wait(5);

            if (!Dln.WriteByte(ch, FRA_Addr, 0x54, 1, 0x0F)) return false;
            Process.AddLog(ch, string.Format("Write Mem : 0x{0:X2} Data : 0x{1:X2}", 0x54, 0x0F));
            if (!Dln.WriteByte(ch, FRA_Addr, 0x55, 1, 0x00)) return false;
            Process.AddLog(ch, string.Format("Write Mem : 0x{0:X2} Data : 0x{1:X2}", 0x55, 0x00));
            Process.Wait(5);

            if (!Dln.WriteByte(ch, FRA_Addr, 0xA8, 1, 0xC5)) return false;
            Process.AddLog(ch, string.Format("Write Mem : 0x{0:X2} Data : 0x{1:X2}", 0xA8, 0xC5));
            Process.Wait(1000);

            return true;
        }
        public bool FRAModeDisable(int ch)
        {

            Process.AddLog(ch, string.Format("FRA Mode Disable"));
            if (!Dln.WriteByte(ch, FRA_Addr, 0xA8, 1, 0x00)) return false;
            Process.AddLog(ch, string.Format("Write Mem : 0x{0:X2} Data : 0x{1:X2}", 0xA8, 0x00));
            if (!Dln.WriteByte(ch, FRA_Addr, 0xAF, 1, 0xEE)) return false;
            Process.AddLog(ch, string.Format("Write Mem : 0x{0:X2} Data : 0x{1:X2}", 0xAF, 0xEE));
            Process.Wait(5);
            if (!Dln.WriteByte(ch, FRA_Addr, 0xAC, 1, 0x00)) return false;
            Process.AddLog(ch, string.Format("Write Mem : 0x{0:X2} Data : 0x{1:X2}", 0xAC, 0x00));
            Process.Wait(15);


            return true;
        }
      
        public bool ChangeSlaveAddr(int ch)
        {
            byte[] addr = { 0x18, 0x1E, 0xE8, 0x98 }; byte icAddr_temp = 0xFF;
            byte temp;
            byte[] i2c_2nd = new byte[2];

            byte rbuf = Dln.ReadByte(ch, AF_Addr, 0x03, 1);
            if(rbuf == 0x1C)
            {
                Process.AddLog(ch, $"IC Address check OK");
                return true;
            }
            for (int i = 0; i < 4; i++)
            {
                rbuf = Dln.ReadByte(ch, addr[i] >> 1, 0x03, 1);
                if(rbuf == 0x1C)
                {
                    icAddr_temp = (byte)(addr[i] >> 1);
                    break;
                }
            }
            if (icAddr_temp != 0xFF)
            {
                Dln.WriteByte(ch, icAddr_temp, 0x02, 1, 0x40);
                Process.Wait(5);
                Dln.WriteByte(ch, icAddr_temp, 0xAE, 1, 0x3B);
                Dln.WriteByte(ch, icAddr_temp, 0x0A, 1, 0x00);
                AF_Memory_Update(ch, 1);
                AF_Memory_Update(ch, 5);
                Dln.WriteByte(ch, AF_Addr, 0xAE, 1, 0x00);
                temp = Dln.ReadByte(ch, AF_Addr, 0x03, 1);

                if (temp == 0x1C) { Process.AddLog(ch, $"I2C address change from 0x{icAddr_temp.ToString("X2")} to 0x{(AF_Addr << 1).ToString("X2")}"); return true; }
                else { Process.AddLog(ch, $"I2C address change NG(fpga error)"); return false; }

            }
            else { Process.AddLog(ch, $"I2C address change NG(check error)"); return false; }

        }
        public bool AF_Memory_Update(int ch, int mode)
        {
            byte val = 0;
            ushort time = 0;
            byte check = 0xFF;
            switch(mode)
            {
                case 0: val = 0x00; time = 0; break;
                case 1: val = 0x01; time = 150; break;
                case 2: val = 0x02; time = 230; break;
                case 3: val = 0x04; time = 200; break;
                case 4: val = 0x08; time = 210; break;
                case 5: val = 0x10; time = 50; break;
                default: break;
            }
            for (int i = 0; i < 5; i++)
            {
                Dln.WriteByte(ch, AF_Addr, 0x03, 1, val);
                Process.Wait(time);
                check = (byte)(Dln.ReadByte(ch, AF_Addr, 0x4B, 1) & 0x04);
                if (check == 0x00) break;
            }
            if (check != 0x00) { Process.AddLog(ch, $"AF Memory Update NG"); return false;}
            return true;
        }
      
        #endregion

        #region OIS Seq
        public void OISOnOff(int ch, bool isOn)
        {
            if (isOn)
            {
                Dln.WriteByte(ch, OIS_Addr, 0x6020, 2, 0x01);
                Process.AddLog(ch, $"OIS Servo On");
                Process.Wait(100);
            }
            else
            {
                Dln.WriteByte(ch, OIS_Addr, 0x6020, 2, 0x01);
                Process.Wait(10);
                Dln.WriteByte(ch, OIS_Addr, 0x6020, 2, 0x04);
                Process.AddLog(ch, $"OIS Servo Off");
                Process.Wait(10);
            }
        }
        public bool OIS_StausCheck(int ch, byte res1, byte res2)
        {
            Stopwatch st = new Stopwatch();
            byte? rdata = null;
            st.Start();
            do
            {
                rdata = Dln.ReadByte(ch, OIS_Addr, 0x6024, 2);
            } while ((rdata != res1 && rdata != res2) && st.ElapsedMilliseconds <= 5000);
            if (rdata != res1 && rdata != res2)
            {
                
                Process.AddLog(ch, $"OIS Status check NG. 0x{rdata?.ToString("X2")}");
                return false;
            }
            return true;
        }
        public bool OIS_StausCheck(int ch, int memAddr, byte res1, byte res2)
        {
            Stopwatch st = new Stopwatch();
            byte? rdata = null;
            st.Start();
            do
            {
                rdata = Dln.ReadByte(ch, OIS_Addr, memAddr, 2);
            } while ((rdata != res1 && rdata != res2) && st.ElapsedMilliseconds <= 10000);
            if (rdata != res1 && rdata != res2)
            {
                Process.AddLog(ch, $"OIS Status check NG. 0x{rdata?.ToString("X2")}");
                return false;
            }
            return true;
        }
        //public bool SetManualDrvModeX(int ch, int MidCode)
        //{
        //    bool flag = false;
        //    flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
        //    Dln.WriteByte(ch, OIS_Addr, 0x617A, 2, 0x01);
        //    flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
        //    Dln.WriteByte(ch, OIS_Addr, 0x6020, 2, 0x07);
        //    flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
        //    OISMove(ch, 0, MidCode);
        //    return true;
        //}
        //public bool SetManualDrvModeY(int ch, int MidCode)
        //{
        //    bool flag = false;
        //    flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
        //    Dln.WriteByte(ch, OIS_Addr, 0x617A, 2, 0x01);
        //    flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
        //    Dln.WriteByte(ch, OIS_Addr, 0x6020, 2, 0x07);
        //    flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
        //    OISMove(ch, 1, MidCode);
        //    return true;
        //}
        public bool SetManualDrvModeXY(int ch, int MidCodeX, int MidCodeY)
        {
            bool flag = false;
            flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
            Dln.WriteByte(ch, OIS_Addr, 0x617A, 2, 0x01);
            flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
            Dln.WriteByte(ch, OIS_Addr, 0x6020, 2, 0x07);
            flag = OIS_StausCheck(ch, 0x01, 0x02); if (!flag) return false;
            OISMove(ch, MidCodeX, MidCodeY);
        
            return true;
        }
        public void OISMove(int ch, int Xcode, int Ycode)
        {
            Dln.Write2Byte(ch, OIS_Addr, 0x60B0, 2, (short)Xcode);
            Dln.Write2Byte(ch, OIS_Addr, 0x60B2, 2, (short)Ycode);           
            OIS_StausCheck(ch, 0x01, 0x02);
        }

        public void OISMoveOL(int ch, int axis, int code)
        {
            int addr = axis == 0 ? 0x6064 : 0x6066;
            Dln.Write2Byte(ch, OIS_Addr, addr, 2, (short)code);
            OIS_StausCheck(ch, 0x01, 0x02);
        }

        public short ReadOISHall(int ch, int axis, int mode)
        {
            if(mode == 0)
            {
                byte[] data = new byte[3];
                if (axis == 0) Dln.ReadArray(ch, OIS_Addr, 0xB107, 2, data);              
                else Dln.ReadArray(ch, OIS_Addr, 0xB109, 2, data);

                short Readhall = (short)((data[1] << 8 | data[2]) / 16);
                if (Readhall >= 2048) Readhall = (short)(Readhall - 4096);
                return Readhall;

            }
            else
            {
                if(axis == 0)
                {
                    Dln.WriteByte(ch, OIS_Addr, 0x6060, 2, 0x00);
                    bool res = OIS_StausCheck(ch, 0x6060, 0x00, 0x00);
                    if (!res) return short.MaxValue;
                }
                else
                {
                    Dln.WriteByte(ch, OIS_Addr, 0x6060, 2, 0x01);
                    bool res = OIS_StausCheck(ch, 0x6060, 0x01, 0x01);
                    if(!res) return short.MaxValue;
                }
                return Dln.Read2Byte_signed(ch, OIS_Addr, 0x6062, 2);


                
            }
        }

        public void OISReset(int ch, int axis, bool OnOff)
        {
            throw new NotImplementedException();
        }

        public void OISICReset(int ch)
        {
            throw new NotImplementedException();
        }

        public bool SetStore(int axis)
        {
            throw new NotImplementedException();
        }

        public void LiearCompWrite(int axis, List<int> CompValue)
        {
            throw new NotImplementedException();
        }
        #endregion

    }


   
}
