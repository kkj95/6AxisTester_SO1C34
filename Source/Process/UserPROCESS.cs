using Dln;
using Dln.Exceptions;
using FZ4P.Extensions;
using MathNet.Numerics.Financial;
using MathNet.Numerics.Optimization.TrustRegion;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Flann;
using OpenCvSharp.Internal.Util;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using System.Xml.Schema;
using static alglib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace FZ4P
{
    public partial class Process
    {
       
        int SinewaveXMaxDiff = 0;
        int SinewaveYMaxDiff = 0;
        int RingingXStabilizer = 0;
        int RingingYStabilizer = 0;
        int[] g_IME = new int[2];
        double[] AFCurrentMinMax = new double[2];
        double[] OISXCurrentMinMax = new double[2];
        double[] OISYCurrentMinMax = new double[2];

        byte[] IC_SETTING_AF = new byte[1];
        byte[] IC_SETTING_AF_REG = new byte[1];
        byte[] IC_DATA_AF = new byte[1];
        byte[] IC_DATA_AF_REG = new byte[1];
        byte AFPIDVersion = 0xFF;

        void AddSequence()
        {
            ItemList.Add(new ActItems() { Name = "AF HallCalibration", Func = AF_HallCalibration, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "AF Only HallCalibration", Func = AF_OnlyHallCalibration, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS FW Donwload", Func = OIS_FWDownload, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS AutoBoot", Func = OIS_AutoBoot, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS HallCalibration", Func = OIS_HallCalibration, IsMulti = true });           
            ItemList.Add(new ActItems() { Name = "XYZ Aging", Func = Act_CloseLoopAging });
            ItemList.Add(new ActItems() { Name = "X/Y Servo Decenter", Func = ServoDecenter, IsMulti = true });      
            ItemList.Add(new ActItems() { Name = "AF Aging", Func = Act_AFScanAging });
            ItemList.Add(new ActItems() { Name = "AF Pre Driving", Func = Act_PreAFDriving });       
            ItemList.Add(new ActItems() { Name = "AF Gain Margin", Func = AFGM, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "AF Phase Margin", Func = AFPM, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "AF PID Verify", Func = AFPID_Verify, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "AF OIS XTalk Calibration", Func = AF_OIS_Xtalk_Calibration, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS Linear/Crosstalk Calibration", Func = OISLCCComp, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS Gain Margin", Func = OISGM, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS Phase Margin", Func = OISPM, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS Decenter Calibration", Func = OIS_Decenter_Calibration, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "AF Rated Stroke", Func = AFRatedStroke, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "AF HAT", Func = AF_HAT, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS HAT(Hall read)", Func = OIS_HAT, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS HAT(Rohm cmd)", Func = OIS_HAT2, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "AF RunUp Test", Func = AFRunUp, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS RunUp Test", Func = OISRunUp, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "AF LoopGain", Func = AFLoopGain, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS LoopGain", Func = OISLoopGain, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS Xtalk 2", Func = Xtalk2, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "AF Fluctuation", Func = AFFluctuation, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS X Fluctuation", Func = OISXFluctuation, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS Y Fluctuation", Func = OISYFluctuation, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "Verify AF_OIS Xtalk", Func = AF_OIS_Xtalk_Verify, IsMulti = true });
            
        }

        #region AddSeq

        void AFRunUp(int ch, string testItem, int InspCnt)
        {
            int[] fwd = new int[3];
            int[] bwd = new int[3];

            (fwd[0], fwd[1], fwd[2]) = AFRunUpFwd(ch, InspCnt, Condition.AFRunUpStartCode, Condition.AFRunUpEndCode, Condition.AFRunUpGain);
            (bwd[0], bwd[1], bwd[2]) = AFRunUpBwd(ch, InspCnt, Condition.AFRunUpStartCode2, Condition.AFRunUpEndCode2, Condition.AFRunUpGain);

            PassFails[0].Results[(int)SpecItem.AF_Diff_50ms].Val = Math.Max(fwd[0], bwd[0]);
            PassFails[0].Results[(int)SpecItem.AF_Diff_100ms].Val = Math.Max(fwd[1], bwd[1]);
            PassFails[0].Results[(int)SpecItem.AF_Diff_150ms].Val = Math.Max(fwd[2], bwd[2]);
            ShowDataResults(ch, (int)SpecItem.AF_Diff_50ms, (int)SpecItem.AF_Diff_150ms, InspType.OnlyMax, new double[] { });
        }
        void OISRunUp(int ch, string testItem, int InspCnt)
        {
            int XDiffPos = 0;
            int XDiffNeg = 0;
            int YDiffPos = 0;
            int YDiffNeg = 0;

            (XDiffNeg, XDiffPos) = OISRunUpProcess(ch, 0, InspCnt);
            (YDiffNeg, YDiffPos) = OISRunUpProcess(ch, 1, InspCnt);

            PassFails[0].Results[(int)SpecItem.OISX_Diff_ms].Val = Math.Max(XDiffNeg, XDiffPos);
            PassFails[0].Results[(int)SpecItem.OISY_Diff_ms].Val = Math.Max(YDiffNeg, YDiffPos);
            //PassFails[0].Results[(int)SpecItem.OISX_Diff_150ms].Val = Math.Max(XDiffNeg[2], XDiffPos[2]);
            //PassFails[0].Results[(int)SpecItem.OISY_Diff_50ms].Val = Math.Max(YDiffNeg[0], YDiffPos[0]);
            //PassFails[0].Results[(int)SpecItem.OISY_Diff_100ms].Val = Math.Max(YDiffNeg[1], YDiffPos[1]);
            //PassFails[0].Results[(int)SpecItem.OISY_Diff_150ms].Val = Math.Max(YDiffNeg[2], YDiffPos[2]);
            ShowDataResults(ch, (int)SpecItem.OISX_Diff_ms, (int)SpecItem.OISY_Diff_ms, InspType.OnlyMax, new double[] { });


        }
        (int, int) OISRunUpProcess(int ch, int axis, int InspCnt)
        {

            int RunUpDiffNeg =0;
            int RunUpDiffPos =0;
            int DiffTime = axis == 0 ? Condition.OISRunUpFindXDiffTimeX : Condition.OISRunUpFindXDiffTimeY;
            string axisStr = axis == 0 ? "X" : "Y";
            string dataDir = STATIC.CreateDateDir();
            dataDir += "OISRunUp\\";
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            string timeDir = $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s";
            List<string> arr = new List<string>();
            string path = string.Format(dataDir + "{0}_{1}_{2}_{3}.csv", $"OIS_RunUp {axisStr}", m_StrIndex[ch], InspCnt + 1, timeDir);


            List<RunUpData> Negdata = new List<RunUpData>();
            List<RunUpData> Posdata = new List<RunUpData>();
            DrvIC.OISOnOff(ch, true);
            Wait(100);
            DrvIC.OISOnOff(ch, false);
            Wait(100);
            if (axis == 0) Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6106, 2, 0x80);
            else Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6108, 2, 0x80);

            Task[] task = new Task[2];
            DrvIC.OISMoveOL(ch, axis, -8192);         
            Wait(2000);
          
           
          //  DrvIC.OISMove(ch, axis, 2048);
            Stopwatch st = new Stopwatch();
            int count = 0;
            task[0] = Task.Factory.StartNew(() =>
            {
                do
                {
                    Negdata.Add(new RunUpData
                    {
                        time = st.ElapsedMilliseconds,
                        ReadHall = DrvIC.ReadOISHall(ch, axis, 0),
                        current = Dln.GetCurrent(ch, 1),
                    });
                    count++;
                } while (count < 1000);
            });

            task[1] = Task.Factory.StartNew(() =>
            {
                st.Start();
                count = 0;
                DrvIC.OISOnOff(ch, true);

            });
            Task.WaitAll(task);
          
           
            task = new Task[2];
            DrvIC.OISOnOff(ch, false);
            Wait(100);
            DrvIC.OISMoveOL(ch, axis, 8191);              
            Wait(2000);
            st = new Stopwatch();
            count = 0;

            task[0] = Task.Factory.StartNew(() =>
            {
                do
                {
                    Posdata.Add(new RunUpData
                    {
                        time = st.ElapsedMilliseconds,
                        ReadHall = DrvIC.ReadOISHall(ch, axis, 0),
                        current = Dln.GetCurrent(ch, 1),
                    });
                    count++;
                } while (count < 1000);
            });
           
            task[1] = Task.Factory.StartNew(() =>
            {
                st.Start();
                count = 0;
                DrvIC.OISOnOff(ch, true);
            });
            Task.WaitAll(task);

            if (Option.SaveRawData)
            {
                arr.Add("Time(ms), ReadHall, Current, , , Time(ms), ReadHall, Current");
                for (int i = 0; i < Negdata.Count; i++)
                {
                    arr.Add($"{Negdata[i].time}, {Negdata[i].ReadHall}, {Negdata[i].current},,,{Posdata[i].time},{Posdata[i].ReadHall},{Posdata[i].current}");
                }
                if (path != "") STATIC.SetTextLine(path, arr);

            }

            if (axis == 0) Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6106, 2, 0x100);
            else Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6108, 2, 0x100);

            for (int i = 0; i < Negdata.Count - 1; i++)
            {
                if (Negdata[i].time <= DiffTime && Negdata[i + 1].time >= DiffTime)
                {
                    if (Math.Abs(Negdata[i].time - DiffTime) > Math.Abs(Negdata[i + 1].time - DiffTime))
                    {
                        RunUpDiffNeg = Math.Abs(Negdata[i + 1].ReadHall);
                    }
                    else RunUpDiffNeg = Math.Abs(Negdata[i].ReadHall);

                }
                //if (Negdata[i].time <= 100 && Negdata[i + 1].time >= 100)
                //{
                //    if (Math.Abs(Negdata[i].time - 100) > Math.Abs(Negdata[i + 1].time - 100))
                //    {
                //        RunUpDiffNeg[1] = Math.Abs(Negdata[i + 1].ReadHall);
                //    }
                //    else RunUpDiffNeg[1] = Math.Abs(Negdata[i].ReadHall);

                //}
                //if (Negdata[i].time <= 150 && Negdata[i + 1].time >= 150)
                //{
                //    if (Math.Abs(Negdata[i].time - 150) > Math.Abs(Negdata[i + 1].time - 150))
                //    {
                //        RunUpDiffNeg[2] = Math.Abs(Negdata[i + 1].ReadHall);
                //    }
                //    else RunUpDiffNeg[2] = Math.Abs(Negdata[i].ReadHall);

                //}

                if (Posdata[i].time <= DiffTime && Posdata[i + 1].time >= DiffTime)
                {
                    if (Math.Abs(Posdata[i].time - DiffTime) > Math.Abs(Posdata[i + 1].time - DiffTime))
                    {
                        RunUpDiffPos = Math.Abs(Posdata[i + 1].ReadHall);
                    }
                    else RunUpDiffPos = Math.Abs(Posdata[i].ReadHall);

                }
                //if (Posdata[i].time <= 100 && Posdata[i + 1].time >= 100)
                //{
                //    if (Math.Abs(Posdata[i].time - 100) > Math.Abs(Posdata[i + 1].time - 100))
                //    {
                //        RunUpDiffPos[1] = Math.Abs(Posdata[i + 1].ReadHall);
                //    }
                //    else RunUpDiffPos[1] = Math.Abs(Posdata[i].ReadHall);

                //}
                //if (Posdata[i].time <= 150 && Posdata[i + 1].time >= 150)
                //{
                //    if (Math.Abs(Posdata[i].time - 150) > Math.Abs(Posdata[i + 1].time - 150))
                //    {
                //        RunUpDiffPos[2] = Math.Abs(Posdata[i + 1].ReadHall);
                //    }
                //    else RunUpDiffPos[2] = Math.Abs(Posdata[i].ReadHall);

                //}

                if (Negdata[i].time > DiffTime + 50 && Posdata[i].time > DiffTime + 50) break;
            }



            return (RunUpDiffNeg, RunUpDiffPos);

        }
        (int, int, int) AFRunUpFwd(int ch, int InspCnt, int start, int end, int gainMode)
        {


            int[] RunUpDiff = new int[3];

            string dataDir = STATIC.CreateDateDir();
            dataDir += "AFRunUp\\";
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            string timeDir = $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s";
            List<string> arr = new List<string>();
            string path = string.Format(dataDir + "{0}_{1}_{2}_{3}.csv", $"AF_RunUp({start}~{end})", m_StrIndex[ch], InspCnt + 1, timeDir);


            byte cur_gain, new_gain;
            List<RunUpData> data = new List<RunUpData>();

            DrvIC.AFOnOff(ch, false);
            Wait(5);
            cur_gain = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x10, 1);
            AddLog(ch, $"AF current gain = 0x{cur_gain.ToString("X2")}");

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            byte backData = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x0B, 1);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x1A, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, (byte)(backData & 0x7F));
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x7B);
            
            if(gainMode == 1)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0x10, 1, (byte)(cur_gain / 2));
                Wait(22);
                new_gain = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x10, 1);
                AddLog(ch, $"AF -6dB gain = 0x{new_gain.ToString("X2")}");

            }


            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);

            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMoveOL(ch, 0);
            Wait(2000);
            DrvIC.AFOnOff(ch, false);
            Wait(5);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, backData);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMove(ch, start);
            Wait(100);
            Task[] task = new Task[2];
           
            Stopwatch st = new Stopwatch();
            int count = 0;
            task[0] = Task.Factory.StartNew(() =>
            {
                do
                {
                    data.Add(new RunUpData
                    {
                        time = st.ElapsedMilliseconds,
                        ReadHall = DrvIC.ReadAFHall(ch),
                        current = Dln.GetCurrent(ch, 0),
                    });
                    count++;
                } while (count < 1000);
            });
           
            task[1] = Task.Factory.StartNew(() =>
            {
                st.Start();
                count = 0;
                DrvIC.AFMove(ch, end);
                
            });
            Task.WaitAll(task);


            DrvIC.AFOnOff(ch, false);
            if(gainMode == 1)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0x10, 1, cur_gain);
                Wait(22);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
                DrvIC.AFOnOff(ch, true);
            }
            Wait(5);
          

            if(Option.SaveRawData)
            {
                arr.Add("Time(ms), ReadHall, Current");
                for (int i = 0; i < data.Count; i++)
                {
                    arr.Add($"{data[i].time}, {data[i].ReadHall}, {data[i].current}");
                }
                if (path != "") STATIC.SetTextLine(path, arr);

            }

            for (int i = 0; i < data.Count - 1; i++)
            {
                if (data[i].time <= 50 && data[i + 1].time >= 50)
                {
                    if (Math.Abs(data[i].time - 50) > Math.Abs(data[i + 1].time - 50))
                    {
                        RunUpDiff[0] = Math.Abs(data[i + 1].ReadHall - 2100);
                    }
                    else RunUpDiff[0] = Math.Abs(data[i].ReadHall - 2100);

                }
                if (data[i].time <= 100 && data[i + 1].time >= 100)
                {
                    if (Math.Abs(data[i].time - 100) > Math.Abs(data[i + 1].time - 100))
                    {
                        RunUpDiff[1] = Math.Abs(data[i + 1].ReadHall - 2100);
                    }
                    else RunUpDiff[1] = Math.Abs(data[i].ReadHall - 2100);

                }
                if (data[i].time <= 150 && data[i + 1].time >= 150)
                {
                    if (Math.Abs(data[i].time - 150) > Math.Abs(data[i + 1].time - 150))
                    {
                        RunUpDiff[2] = Math.Abs(data[i + 1].ReadHall - 2100);
                    }
                    else RunUpDiff[2] = Math.Abs(data[i].ReadHall - 2100);

                }

                if (data[i].time > 200) break;
            }


            return (RunUpDiff[0], RunUpDiff[1], RunUpDiff[2]);

       

        }
        (int, int, int) AFRunUpBwd(int ch, int InspCnt, int start, int end, int gainMode)
        {


            int[] RunUpDiff = new int[3];

            string dataDir = STATIC.CreateDateDir();
            dataDir += "AFRunUp\\";
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            string timeDir = $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s";
            List<string> arr = new List<string>();
            string path = string.Format(dataDir + "{0}_{1}_{2}_{3}.csv", $"AF_RunUp({start}~{end})", m_StrIndex[ch], InspCnt + 1, timeDir);


            byte cur_gain, new_gain;
            List<RunUpData> data = new List<RunUpData>();

            DrvIC.AFOnOff(ch, false);
            Wait(5);
            cur_gain = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x10, 1);
            AddLog(ch, $"AF current gain = 0x{cur_gain.ToString("X2")}");

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            byte backData = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x0B, 1);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x1A, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, (byte)(backData & 0x7F));
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x7B);
            if(gainMode == 1)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0x10, 1, (byte)(cur_gain / 2));
                Wait(22);
                new_gain = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x10, 1);
                AddLog(ch, $"AF 6dB gain = 0x{new_gain.ToString("X2")}");
            }
            
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);

            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMoveOL(ch, 4095);
            Wait(2000);
            DrvIC.AFOnOff(ch, false);
            Wait(5);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, backData);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMove(ch, start);
            Wait(100);
            Task[] task = new Task[2];

            Stopwatch st = new Stopwatch();
            int count = 0;
            task[0] = Task.Factory.StartNew(() =>
            {
                do
                {
                    data.Add(new RunUpData
                    {
                        time = st.ElapsedMilliseconds,
                        ReadHall = DrvIC.ReadAFHall(ch),
                        current = Dln.GetCurrent(ch, 0),
                    });
                    count++;
                } while (count < 1000);
            });

            task[1] = Task.Factory.StartNew(() =>
            {
                st.Start();
                count = 0;
                DrvIC.AFMove(ch, end);

            });
            Task.WaitAll(task);


            DrvIC.AFOnOff(ch, false);
            if(gainMode == 1)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0x10, 1, cur_gain);
                Wait(22);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
                DrvIC.AFOnOff(ch, true);

            }
            Wait(5);
        

            if (Option.SaveRawData)
            {
                arr.Add("Time(ms), ReadHall, Current");
                for (int i = 0; i < data.Count; i++)
                {
                    arr.Add($"{data[i].time}, {data[i].ReadHall}, {data[i].current}");
                }
                if (path != "") STATIC.SetTextLine(path, arr);

            }

            for (int i = 0; i < data.Count - 1; i++)
            {
                if (data[i].time <= 50 && data[i + 1].time >= 50)
                {
                    if (Math.Abs(data[i].time - 50) > Math.Abs(data[i + 1].time - 50))
                    {
                        RunUpDiff[0] = Math.Abs(data[i + 1].ReadHall - 2000);
                    }
                    else RunUpDiff[0] = Math.Abs(data[i].ReadHall - 2000);

                }
                if (data[i].time <= 100 && data[i + 1].time >= 100)
                {
                    if (Math.Abs(data[i].time - 100) > Math.Abs(data[i + 1].time - 100))
                    {
                        RunUpDiff[1] = Math.Abs(data[i + 1].ReadHall - 2000);
                    }
                    else RunUpDiff[1] = Math.Abs(data[i].ReadHall - 2000);

                }
                if (data[i].time <= 150 && data[i + 1].time >= 150)
                {
                    if (Math.Abs(data[i].time - 150) > Math.Abs(data[i + 1].time - 150))
                    {
                        RunUpDiff[2] = Math.Abs(data[i + 1].ReadHall - 2000);
                    }
                    else RunUpDiff[2] = Math.Abs(data[i].ReadHall - 2000);

                }

                if (data[i].time > 200) break;
            }

            return (RunUpDiff[0], RunUpDiff[1], RunUpDiff[2]);
         

        }

        void OIS_HAT(int ch, string testItem, int InspCnt)
        {
            int[] res = new int[2];
            int[] res2 = new int[2];
            (res[0], res2[0]) = OIS_HallAccuracyTest(ch, 0, InspCnt);
            (res[1], res2[1]) = OIS_HallAccuracyTest(ch, 1, InspCnt);
            PassFails[0].Results[(int)SpecItem.OISXHAT_Diff].Val = res[0];
            PassFails[0].Results[(int)SpecItem.OISXHAT_Diff_MaxError].Val = res2[0];
            PassFails[0].Results[(int)SpecItem.OISYHAT_Diff].Val = res[1];
            PassFails[0].Results[(int)SpecItem.OISYHAT_Diff_MaxError].Val = res2[1];
            ShowDataResults(ch, (int)SpecItem.OISXHAT_Diff, (int)SpecItem.OISYHAT_Diff_MaxError, InspType.OnlyMax, new double[] { });

        }
        void OIS_HAT2(int ch, string testItem, int InspCnt)
        {
            int res = 0;
            int res1 = 0;
            int res2 = 0;
            int res3 = 0;
            (res, res1, res2, res3) = OIS_HallAccuracyTest2(ch, InspCnt);

            PassFails[0].Results[(int)SpecItem.OISXHAT_Diff].Val = res;
            PassFails[0].Results[(int)SpecItem.OISXHAT_Diff_MaxError].Val = res1;
            PassFails[0].Results[(int)SpecItem.OISYHAT_Diff].Val = res2;
            PassFails[0].Results[(int)SpecItem.OISYHAT_Diff_MaxError].Val = res3;
            ShowDataResults(ch, (int)SpecItem.OISXHAT_Diff, (int)SpecItem.OISYHAT_Diff_MaxError, InspType.OnlyMax, new double[] { });

        }
        (int, int) OIS_HallAccuracyTest(int ch, int axis, int inspcnt)
        {
            bool status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if (!status)
            {
                return (99999, 99999);
            }
            string axisName = axis == 0 ? "X" : "Y";
            string OtheraxisName = axis == 0 ? "Y" : "X";
            int OtherAxis = axis == 0 ? 1 : 0;
            int[] OISMovePointX = new int[9] { XHATPOSOIS[0], (int)(XHATPOSOIS[0] / Math.Sqrt(2)), 0, (int)(XHATPOSOIS[1] / Math.Sqrt(2)), XHATPOSOIS[1], (int)(XHATPOSOIS[1] / Math.Sqrt(2)), 0, (int)(XHATPOSOIS[0] / Math.Sqrt(2)), 0 };
            int[] OISMovePointY = new int[9] { 0, (int)(YHATPOSOIS[1] / Math.Sqrt(2)), YHATPOSOIS[1], (int)(YHATPOSOIS[1] / Math.Sqrt(2)), 0, (int)(YHATPOSOIS[0] / Math.Sqrt(2)), YHATPOSOIS[0], (int)(YHATPOSOIS[0] / Math.Sqrt(2)), 0 };
            int MaxPtoP = int.MinValue;
            int MaxError = int.MinValue;
            string dataDir = STATIC.CreateDateDir();
            dataDir += "HATData\\";
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            string timeDir = $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s";

            List<string> arr = new List<string>();
            string path = string.Format(dataDir + "{0} {1}_{2}_{3}_{4}.csv", "OIS_HAT", axisName, m_StrIndex[ch], inspcnt + 1, timeDir);

            List<List<int>> AxisReadHall = new List<List<int>>();
            List<List<long>> AxisReadHallST = new List<List<long>>();
            List<int> AFReadHall = new List<int>();
            List<int> OtherAxisReadHall = new List<int>();
            List<List<int>> AxisMaxError = new List<List<int>>();

            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x614A, 2, 0x01);
            DrvIC.OISOnOff(ch, true);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x617A, 2, 0x01);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6020, 2, 0x07);

            //set 6db
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6106, 2, 0x200);
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6108, 2, 0x200);

            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMove(ch, 100);
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x60F0, 2, 100);
            Wait(Condition.OISHATMoveDelay);

            for (int i = 0; i < 9; i++)
            {
                Stopwatch st = new Stopwatch();
                int MaxPtoPOnCode = int.MinValue;
                AxisReadHall.Add(new List<int>());
                AxisReadHallST.Add(new List<long>());
                AxisMaxError.Add(new List<int>());
                DrvIC.OISMove(ch, OISMovePointX[i], OISMovePointY[i]);
                Wait(Condition.OISHATMoveDelay);
                AFReadHall.Add(DrvIC.ReadAFHall(ch));
                OtherAxisReadHall.Add(DrvIC.ReadOISHall(ch, OtherAxis, 0));
                st.Start();
                for (int j = 0; j < 1000; j++)
                {
                    int a = DrvIC.ReadOISHall(ch, axis, 0);
                    AxisReadHallST[i].Add(st.ElapsedMilliseconds);
                    AxisReadHall[i].Add(a);
                    if (axis == 0) AxisMaxError[i].Add(Math.Abs(OISMovePointX[i] - a));
                    else AxisMaxError[i].Add(Math.Abs(OISMovePointY[i] - a));

                }
                MaxPtoPOnCode = Math.Abs(AxisReadHall[i].Max() - AxisReadHall[i].Min());
                if (MaxPtoPOnCode > MaxPtoP)
                    MaxPtoP = MaxPtoPOnCode;

                if (AxisMaxError[i].Max() > MaxError) MaxError = AxisMaxError[i].Max();

            }
            
            if(Option.SaveRawData)
            {
                arr.Add($"AF Hall 1, {OtheraxisName} Hall 1, AF Hall 2, {OtheraxisName} Hall 2, AF Hall 3, {OtheraxisName} Hall 3, AF Hall 4, {OtheraxisName} Hall 4, AF Hall 5, {OtheraxisName} Hall 5, " +
                    $"AF Hall 6, {OtheraxisName} Hall 6, AF Hall 7, {OtheraxisName} Hall 7, AF Hall 8, {OtheraxisName} Hall 8, AF Hall 9, {OtheraxisName} Hall 9");

                if (axis == 0)
                {
                    arr.Add($"{100}, {OISMovePointY[0]},{100}, {OISMovePointY[1]},{100}, {OISMovePointY[2]},{100}, {OISMovePointY[3]},{100}, {OISMovePointY[4]}," +
                    $"{100}, {OISMovePointY[5]},{100}, {OISMovePointY[6]},{100}, {OISMovePointY[7]},{100}, {OISMovePointY[8]}");
                }
                else 
                {
                    arr.Add($"{100}, {OISMovePointX[0]},{100}, {OISMovePointX[1]},{100}, {OISMovePointX[2]},{100}, {OISMovePointX[3]},{100}, {OISMovePointX[4]}," +
                    $"{100}, {OISMovePointX[5]},{100}, {OISMovePointX[6]},{100}, {OISMovePointX[7]},{100}, {OISMovePointX[8]}");
                }


                    arr.Add($"Time, {axisName} Hall 1, Time, {axisName} Hall 2, Time, {axisName} Hall 3, Time, {axisName} Hall 4, Time, {axisName} Hall 5," +
                            $"Time, {axisName} Hall 6, Time, {axisName} Hall 7, Time, {axisName} Hall 8, Time, {axisName} Hall 9");

                for (int i = 0; i < 1000; i++)
                {
                    arr.Add($"{AxisReadHallST[0][i]}, {AxisReadHall[0][i]}, {AxisReadHallST[1][i]}, {AxisReadHall[1][i]}, {AxisReadHallST[2][i]}, {AxisReadHall[2][i]}, {AxisReadHallST[3][i]}, {AxisReadHall[3][i]}," +
                        $"{AxisReadHallST[4][i]}, {AxisReadHall[4][i]}, {AxisReadHallST[5][i]}, {AxisReadHall[5][i]}, {AxisReadHallST[6][i]}, {AxisReadHall[6][i]}, {AxisReadHallST[7][i]}, {AxisReadHall[7][i]}, {AxisReadHallST[8][i]}, {AxisReadHall[8][i]}");
                }

            }

            AFReadHall.Clear();
            OtherAxisReadHall.Clear();
            AxisReadHall.Clear();
            AxisReadHallST.Clear();
            AxisMaxError.Clear();
            DrvIC.AFMove(ch, 3995);
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x60F0, 2, 3995);
            Wait(Condition.OISHATMoveDelay);

            for (int i = 0; i < 9; i++)
            {
                Stopwatch st = new Stopwatch();
                int MaxPtoPOnCode = int.MinValue;
                AxisReadHall.Add(new List<int>());
                AxisReadHallST.Add(new List<long>());
                AxisMaxError.Add(new List<int>());
                DrvIC.OISMove(ch, OISMovePointX[i], OISMovePointY[i]);
                Wait(Condition.OISHATMoveDelay);
                AFReadHall.Add(DrvIC.ReadAFHall(ch));
                OtherAxisReadHall.Add(DrvIC.ReadOISHall(ch, OtherAxis, 0));
                st.Start();
                for (int j = 0; j < 1000; j++)
                {
                    int a = DrvIC.ReadOISHall(ch, axis, 0);
                    AxisReadHallST[i].Add(st.ElapsedMilliseconds);
                    AxisReadHall[i].Add(a);
                    if (axis == 0) AxisMaxError[i].Add(Math.Abs(OISMovePointX[i] - a));
                    else AxisMaxError[i].Add(Math.Abs(OISMovePointY[i] - a));
                }
                MaxPtoPOnCode = Math.Abs(AxisReadHall[i].Max() - AxisReadHall[i].Min());
                if (MaxPtoPOnCode > MaxPtoP)
                    MaxPtoP = MaxPtoPOnCode;
                if (AxisMaxError[i].Max() > MaxError) MaxError = AxisMaxError[i].Max();

            }
            if (Option.SaveRawData)
            {
                arr.Add("\r\n");

                arr.Add($"AF Hall 1, {OtheraxisName} Hall 1, AF Hall 2, {OtheraxisName} Hall 2, AF Hall 3, {OtheraxisName} Hall 3, AF Hall 4, {OtheraxisName} Hall 4, AF Hall 5, {OtheraxisName} Hall 5, " +
                    $"AF Hall 6, {OtheraxisName} Hall 6, AF Hall 7, {OtheraxisName} Hall 7, AF Hall 8, {OtheraxisName} Hall 8, AF Hall 9, {OtheraxisName} Hall 9");
                if (axis == 0)
                {
                    arr.Add($"{3995}, {OISMovePointY[0]},{3995}, {OISMovePointY[1]},{3995}, {OISMovePointY[2]},{3995}, {OISMovePointY[3]},{3995}, {OISMovePointY[4]}," +
                    $"{3995}, {OISMovePointY[5]},{3995}, {OISMovePointY[6]},{3995}, {OISMovePointY[7]},{3995}, {OISMovePointY[8]}");
                }
                else
                {
                    arr.Add($"{3995}, {OISMovePointX[0]},{3995}, {OISMovePointX[1]},{3995}, {OISMovePointX[2]},{3995}, {OISMovePointX[3]},{3995}, {OISMovePointX[4]}," +
                    $"{3995}, {OISMovePointX[5]},{3995}, {OISMovePointX[6]},{3995}, {OISMovePointX[7]},{3995}, {OISMovePointX[8]}");
                }

                arr.Add($"Time, {axisName} Hall 1, Time, {axisName} Hall 2, Time, {axisName} Hall 3, Time, {axisName} Hall 4, Time, {axisName} Hall 5," +
                        $"Time, {axisName} Hall 6, Time, {axisName} Hall 7, Time, {axisName} Hall 8, Time, {axisName} Hall 9");

                for (int i = 0; i < 1000; i++)
                {
                    arr.Add($"{AxisReadHallST[0][i]}, {AxisReadHall[0][i]}, {AxisReadHallST[1][i]}, {AxisReadHall[1][i]}, {AxisReadHallST[2][i]}, {AxisReadHall[2][i]}, {AxisReadHallST[3][i]}, {AxisReadHall[3][i]}," +
                        $"{AxisReadHallST[4][i]}, {AxisReadHall[4][i]}, {AxisReadHallST[5][i]}, {AxisReadHall[5][i]}, {AxisReadHallST[6][i]}, {AxisReadHall[6][i]}, {AxisReadHallST[7][i]}, {AxisReadHall[7][i]}, {AxisReadHallST[8][i]}, {AxisReadHall[8][i]}");
                }
                if (path != "") STATIC.SetTextLine(path, arr);

            }



            //set 0db
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6106, 2, 0x100);
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6108, 2, 0x100);

            return (MaxPtoP, MaxError);
        }
        (int, int, int, int) OIS_HallAccuracyTest2(int ch, int inspcnt)
        {
            bool status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if (!status)
            {
                return (99999, 99999, 99999, 99999);
            }
            //string axisName = axis == 0 ? "X" : "Y";
            //string OtheraxisName = axis == 0 ? "Y" : "X";
            //int OtherAxis = axis == 0 ? 1 : 0;
            int[] OISMovePointX = new int[9] { XHATPOSOIS[0], (int)(XHATPOSOIS[0] / Math.Sqrt(2)), 0, (int)(XHATPOSOIS[1] / Math.Sqrt(2)), XHATPOSOIS[1], (int)(XHATPOSOIS[1] / Math.Sqrt(2)), 0, (int)(XHATPOSOIS[0] / Math.Sqrt(2)), 0 };
            int[] OISMovePointY = new int[9] { 0, (int)(YHATPOSOIS[1] / Math.Sqrt(2)), YHATPOSOIS[1], (int)(YHATPOSOIS[1] / Math.Sqrt(2)), 0, (int)(YHATPOSOIS[0] / Math.Sqrt(2)), YHATPOSOIS[0], (int)(YHATPOSOIS[0] / Math.Sqrt(2)), 0 };
            int MaxPtoPX = int.MinValue;
            int MaxErrorX = int.MinValue;
            int MaxPtoPY = int.MinValue;
            int MaxErrorY = int.MinValue;
            //string dataDir = STATIC.CreateDateDir();
            //dataDir += "HATData\\";
            //if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            //string timeDir = $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s";

            //List<string> arr = new List<string>();
            //string path = string.Format(dataDir + "{0} {1}_{2}_{3}_{4}.csv", "OIS_HAT", axisName, m_StrIndex[ch], inspcnt + 1, timeDir);

            int HallMinX = 0;
            int HallMaxX = 0;
            int HallMinY = 0;
            int HallMaxY = 0;
            int MinErrX = 0;
            int MaxErrX = 0;
            int MinErrY = 0;
            int MaxErrY = 0;
         

            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x614A, 2, 0x01);
            DrvIC.OISOnOff(ch, true);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x617A, 2, 0x01);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6020, 2, 0x07);

            //set 6db
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6106, 2, 0x200);
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6108, 2, 0x200);

            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMove(ch, 100);
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x60F0, 2, 100);
            Wait(Condition.OISHATMoveDelay);
         

            for (int i = 0; i < 9; i++)
            {

                int MaxPtoPOnCodeX = int.MinValue;
                int MaxErrorOnCodeX = int.MinValue;
                int MaxPtoPOnCodeY = int.MinValue;
                int MaxErrorOnCodeY = int.MinValue;

                AddLog(ch, $"AF Position 1, OIS Position {i + 1} (AF : 100, X:{OISMovePointX[i]}, Y:{OISMovePointY[i]}) >>>>>>>>>>>>>>>");
                DrvIC.OISMove(ch, OISMovePointX[i], OISMovePointY[i]);
                Wait(Condition.OISHATMoveDelay);
              

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61EC, 2, 0x01);
                status = DrvIC.OIS_StausCheck(ch, 0x61EC, 0x00, 0x00);
                if (!status) return (99999, 99999, 99999, 99999);

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x00);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x00, 0x00);
                if (!status) return (99999, 99999, 99999, 99999);
                HallMinX = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, XHallMin:{HallMinX}");

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x01);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x01, 0x01);
                if (!status) return (99999, 99999, 99999, 99999);
                HallMaxX = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, XHallMax:{HallMaxX}");

              
                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x04);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x04, 0x04);
                if (!status) return (99999, 99999, 99999, 99999);
                HallMinY = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, YHallMin:{HallMinY}");

              
                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x05);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x05, 0x05);
                if (!status) return (99999, 99999, 99999, 99999);
                HallMaxY = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, YHallMax:{HallMaxY}");



                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x10);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x10, 0x10);
                if (!status) return (99999, 99999, 99999, 99999);
                MinErrX = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, XErrorMin:{MinErrX}");

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x11);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x11, 0x11);
                if (!status) return (99999, 99999, 99999, 99999);
                MaxErrX = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, XErrorMax:{MaxErrX}");

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x14);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x14, 0x14);
                if (!status) return (99999, 99999, 99999, 99999);
                MinErrY = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, YErrorMin:{MinErrY}");

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x15);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x15, 0x15);
                if (!status) return (99999, 99999, 99999, 99999);
                MaxErrY = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, YErrorMax:{MaxErrY}");

                MaxPtoPOnCodeX = Math.Abs(HallMaxX - HallMinX);
                MaxErrorOnCodeX = Math.Abs(MaxErrX - MinErrX);
                MaxPtoPOnCodeY = Math.Abs(HallMaxY - HallMinY);
                MaxErrorOnCodeY = Math.Abs(MaxErrY - MinErrY);

                if (MaxPtoPOnCodeX > MaxPtoPX)
                    MaxPtoPX = MaxPtoPOnCodeX;
                if (MaxErrorOnCodeX > MaxErrorX)
                    MaxErrorX = MaxErrorOnCodeX;
                if (MaxPtoPOnCodeY > MaxPtoPY)
                    MaxPtoPY = MaxPtoPOnCodeY;
                if (MaxErrorOnCodeY > MaxErrorY)
                    MaxErrorY = MaxErrorOnCodeY;

            }



            DrvIC.AFMove(ch, 3995);
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x60F0, 2, 3995);
            Wait(Condition.OISHATMoveDelay);

            for (int i = 0; i < 9; i++)
            {

                int MaxPtoPOnCodeX = int.MinValue;
                int MaxErrorOnCodeX = int.MinValue;
                int MaxPtoPOnCodeY = int.MinValue;
                int MaxErrorOnCodeY = int.MinValue;

                AddLog(ch, $"AF Position 1, OIS Position {i + 1} (AF : 3995, X:{OISMovePointX[i]}, Y:{OISMovePointY[i]}) >>>>>>>>>>>>>>>");

                DrvIC.OISMove(ch, OISMovePointX[i], OISMovePointY[i]);
                Wait(Condition.OISHATMoveDelay);

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61EC, 2, 0x01);
                status = DrvIC.OIS_StausCheck(ch, 0x61EC, 0x00, 0x00);
                if (!status) return (99999, 99999, 99999, 99999);

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x00);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x00, 0x00);
                if (!status) return (99999, 99999, 99999, 99999);
                HallMinX = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, XHallMin:{HallMinX}");

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x01);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x01, 0x01);
                if (!status) return (99999, 99999, 99999, 99999);
                HallMaxX = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, XHallMax:{HallMaxX}");


                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x04);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x04, 0x04);
                if (!status) return (99999, 99999, 99999, 99999);
                HallMinY = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, YHallMin:{HallMinY}");


                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x05);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x05, 0x05);
                if (!status) return (99999, 99999, 99999, 99999);
                HallMaxY = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, YHallMax:{HallMaxY}");



                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x10);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x10, 0x10);
                if (!status) return (99999, 99999, 99999, 99999);
                MinErrX = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, XErrorMin:{MinErrX}");

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x11);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x11, 0x11);
                if (!status) return (99999, 99999, 99999, 99999);
                MaxErrX = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, XErrorMax:{MaxErrX}");

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x14);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x14, 0x14);
                if (!status) return (99999, 99999, 99999, 99999);
                MinErrY = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, YErrorMin:{MinErrY}");

                Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61ED, 2, 0x15);
                status = DrvIC.OIS_StausCheck(ch, 0x61ED, 0x15, 0x15);
                if (!status) return (99999, 99999, 99999, 99999);
                MaxErrY = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x61EE, 2);
                AddLog(ch, $"afpos:{DrvIC.ReadAFHall(ch)}, YErrorMax:{MaxErrY}");

                MaxPtoPOnCodeX = Math.Abs(HallMaxX - HallMinX);
                MaxErrorOnCodeX = Math.Abs(MaxErrX - MinErrX);
                MaxPtoPOnCodeY = Math.Abs(HallMaxY - HallMinY);
                MaxErrorOnCodeY = Math.Abs(MaxErrY - MinErrY);
                if (MaxPtoPOnCodeX > MaxPtoPX)
                    MaxPtoPX = MaxPtoPOnCodeX;
                if (MaxErrorOnCodeX > MaxErrorX)
                    MaxErrorX = MaxErrorOnCodeX;

                if (MaxPtoPOnCodeY > MaxPtoPY)
                    MaxPtoPY = MaxPtoPOnCodeY;
                if (MaxErrorOnCodeY > MaxErrorY)
                    MaxErrorY = MaxErrorOnCodeY;

            }

            //set 0db
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6106, 2, 0x100);
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6108, 2, 0x100);

            return (MaxPtoPX, MaxErrorX, MaxPtoPY, MaxErrorY);
        }

        void AF_HAT(int ch, string testItem, int InspCnt)
        {
            bool status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if(!status)
            {
                PassFails[0].Results[(int)SpecItem.AFHAT_Diff].Val = 99999;
                ShowDataResults(ch, (int)SpecItem.AFHAT_Diff, (int)SpecItem.AFHAT_Diff, InspType.Normal, new double[] { });
            }

            int[] OISMovePointX = new int[9] { XHATPOSAF[0], (int)(XHATPOSAF[0] / Math.Sqrt(2)), 0, (int)(XHATPOSAF[1] / Math.Sqrt(2)), XHATPOSAF[1], (int)(XHATPOSAF[1] / Math.Sqrt(2)), 0, (int)(XHATPOSAF[0] / Math.Sqrt(2)), 0 };
            int[] OISMovePointY = new int[9] { 0, (int)(YHATPOSAF[1] / Math.Sqrt(2)), YHATPOSAF[1], (int)(YHATPOSAF[1] / Math.Sqrt(2)), 0, (int)(YHATPOSAF[0] / Math.Sqrt(2)), YHATPOSAF[0], (int)(YHATPOSAF[0] / Math.Sqrt(2)), 0 };


            string dataDir = STATIC.CreateDateDir();
            dataDir += "HATData\\";
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            string timeDir = $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s";

            List<string> arr = new List<string>();
            string path = string.Format(dataDir + "{0}_{1}_{2}_{3}.csv", "AF_HAT", m_StrIndex[ch], InspCnt + 1, timeDir);

            List<List<int>> AFReadHall = new List<List<int>>();
            List<List<long>> AFReadHallST = new List<List<long>>();
            List<int> XReadHall = new List<int>();
            List<int> YReadHall = new List<int>();
            List<List<int>> AFMaxError = new List<List<int>>();

            int MaxPtoP = int.MinValue;
            int MaxError = int.MinValue;
            byte cur_gain, new_gain;
            DrvIC.AFOnOff(ch, false);
            Wait(5);
            cur_gain = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x10, 1);
            AddLog(ch, $"AF current gain = 0x{cur_gain.ToString("X2")}");
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x10, 1, (byte)(cur_gain * 2));
            Wait(22);
            new_gain = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x10, 1);
            AddLog(ch, $"AF 6dB gain = 0x{new_gain.ToString("X2")}");
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            DrvIC.AFOnOff(ch, true);
            DrvIC.OISOnOff(ch, true);
            DrvIC.SetManualDrvModeXY(ch, 0, 0);
            DrvIC.AFMove(ch, 100);
            Wait(Condition.AFHATMoveDelay);

            for (int i = 0; i < 9; i++)
            {
                Stopwatch st = new Stopwatch();
                int MaxPtoPOnCode = int.MinValue;
              
                AFReadHall.Add(new List<int>());
                AFReadHallST.Add(new List<long>());
                AFMaxError.Add(new List<int>());
                         
                DrvIC.OISMove(ch, OISMovePointX[i], OISMovePointY[i]);
                Wait(Condition.AFHATMoveDelay);
                XReadHall.Add(DrvIC.ReadOISHall(ch, 0, 0));
                YReadHall.Add(DrvIC.ReadOISHall(ch, 1, 0));
                st.Start();
                for (int j = 0; j < 1000; j++)
                {
                    int a = DrvIC.ReadAFHall(ch);
                    AFReadHallST[i].Add(st.ElapsedMilliseconds);
                    AFReadHall[i].Add(a);
                    AFMaxError[i].Add(Math.Abs(100 - a));
                }
                MaxPtoPOnCode = Math.Abs(AFReadHall[i].Max() - AFReadHall[i].Min());
               
                if (MaxPtoPOnCode > MaxPtoP)
                    MaxPtoP = MaxPtoPOnCode;
                if (AFMaxError[i].Max() > MaxError) MaxError = AFMaxError[i].Max();
            }

            if(Option.SaveRawData)
            {
                arr.Add("X Hall 1, Y Hall 1, X Hall 2, Y Hall 2, X Hall 3, Y Hall 3, X Hall 4, Y Hall 4, X Hall 5, Y Hall 5, " +
              "X Hall 6, Y Hall 6, X Hall 7, Y Hall 7, X Hall 8, Y Hall 8, X Hall 9, Y Hall 9");
                arr.Add($"{OISMovePointX[0]}, {OISMovePointY[0]},{OISMovePointX[1]}, {OISMovePointY[1]},{OISMovePointX[2]}, {OISMovePointY[2]},{OISMovePointX[3]}, {OISMovePointY[3]},{OISMovePointX[4]}, {OISMovePointY[4]}," +
                    $"{OISMovePointX[5]}, {OISMovePointY[5]},{OISMovePointX[6]}, {OISMovePointY[6]},{OISMovePointX[7]}, {OISMovePointY[7]},{OISMovePointX[8]}, {OISMovePointY[8]}");
                arr.Add("Time, AF Hall 1, Time, AF Hall 2, Time, AF Hall 3, Time, AF Hall 4, Time, AF Hall 5," +
                    "Time, AF Hall 6, Time, AF Hall 7, Time, AF Hall 8, Time, AF Hall 9");

                for (int i = 0; i < 1000; i++)
                {
                    arr.Add($"{AFReadHallST[0][i]}, {AFReadHall[0][i]}, {AFReadHallST[1][i]}, {AFReadHall[1][i]}, {AFReadHallST[2][i]}, {AFReadHall[2][i]}, {AFReadHallST[3][i]}, {AFReadHall[3][i]}," +
                        $"{AFReadHallST[4][i]}, {AFReadHall[4][i]}, {AFReadHallST[5][i]}, {AFReadHall[5][i]}, {AFReadHallST[6][i]}, {AFReadHall[6][i]}, {AFReadHallST[7][i]}, {AFReadHall[7][i]}, {AFReadHallST[8][i]}, {AFReadHall[8][i]}");
                }

            }


            AFReadHall.Clear();
            AFReadHallST.Clear();
            XReadHall.Clear();
            YReadHall.Clear();
            AFMaxError.Clear();
            DrvIC.AFMove(ch, 3995);
            Wait(Condition.AFHATMoveDelay);
            for (int i = 0; i < 9; i++)
            {
                Stopwatch st = new Stopwatch();
                int MaxPtoPOnCode = int.MinValue;
                AFReadHall.Add(new List<int>());
                AFReadHallST.Add(new List<long>());
                AFMaxError.Add(new List<int>());

                DrvIC.OISMove(ch, OISMovePointX[i], OISMovePointY[i]);
                Wait(Condition.AFHATMoveDelay);
                XReadHall.Add(DrvIC.ReadOISHall(ch, 0, 0));
                YReadHall.Add(DrvIC.ReadOISHall(ch, 1, 0));
                st.Start();
                for (int j = 0; j < 1000; j++)
                {
                    int a = DrvIC.ReadAFHall(ch);
                    AFReadHallST[i].Add(st.ElapsedMilliseconds);
                    AFReadHall[i].Add(a);
                    AFMaxError[i].Add(Math.Abs(3995 - a));
                }
                MaxPtoPOnCode = Math.Abs(AFReadHall[i].Max() - AFReadHall[i].Min());
                if (MaxPtoPOnCode > MaxPtoP)
                    MaxPtoP = MaxPtoPOnCode;
                if (AFMaxError[i].Max() > MaxError) MaxError = AFMaxError[i].Max();
            }

            if(Option.SaveRawData)
            {
                arr.Add("\r\n");
                arr.Add("X Hall 1, Y Hall 1, X Hall 2, Y Hall 2, X Hall 3, Y Hall 3, X Hall 4, Y Hall 4, X Hall 5, Y Hall 5, " +
                  "X Hall 6, Y Hall 6, X Hall 7, Y Hall 7, X Hall 8, Y Hall 8, X Hall 9, Y Hall 9");
                arr.Add($"{OISMovePointX[0]}, {OISMovePointY[0]},{OISMovePointX[1]}, {OISMovePointY[1]},{OISMovePointX[2]}, {OISMovePointY[2]},{OISMovePointX[3]}, {OISMovePointY[3]},{OISMovePointX[4]}, {OISMovePointY[4]}," +
                    $"{OISMovePointX[5]}, {OISMovePointY[5]},{OISMovePointX[6]}, {OISMovePointY[6]},{OISMovePointX[7]}, {OISMovePointY[7]},{OISMovePointX[8]}, {OISMovePointY[8]}");
                arr.Add("Time, AF Hall 1, Time, AF Hall 2, Time, AF Hall 3, Time, AF Hall 4, Time, AF Hall 5," +
                    "Time, AF Hall 6, Time, AF Hall 7, Time, AF Hall 8, Time, AF Hall 9");

                for (int i = 0; i < 1000; i++)
                {
                    arr.Add($"{AFReadHallST[0][i]}, {AFReadHall[0][i]}, {AFReadHallST[1][i]}, {AFReadHall[1][i]}, {AFReadHallST[2][i]}, {AFReadHall[2][i]}, {AFReadHallST[3][i]}, {AFReadHall[3][i]}," +
                        $"{AFReadHallST[4][i]}, {AFReadHall[4][i]}, {AFReadHallST[5][i]}, {AFReadHall[5][i]}, {AFReadHallST[6][i]}, {AFReadHall[6][i]}, {AFReadHallST[7][i]}, {AFReadHall[7][i]}, {AFReadHallST[8][i]}, {AFReadHall[8][i]}");
                }
                if (path != "") STATIC.SetTextLine(path, arr);
            }


          

            PassFails[0].Results[(int)SpecItem.AFHAT_Diff].Val = MaxPtoP;
            PassFails[0].Results[(int)SpecItem.AFHAT_Diff_MaxError].Val = MaxError;
            ShowDataResults(ch, (int)SpecItem.AFHAT_Diff, (int)SpecItem.AFHAT_Diff_MaxError, InspType.Normal, new double[] { });
            //dB원복
            DrvIC.AFOnOff(ch, false);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x10, 1, cur_gain);
            Wait(22);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            DrvIC.AFOnOff(ch, true);
        }

        void AFRatedStroke(int ch, string testItem, int inspCnt)
        {
            FindResult res = new FindResult();
            AFRatedMinMax = new double[3];
            double Stroke = 0;
            LEDs_All_On(ch, true);

            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMove(ch, 0);
            Wait(Condition.AFRatedStrokeDelay);

            res = Measure();
            AFRatedMinMax[0] = res.cz[0];
            AddLog(ch, $"AF Zero Pos = {AFRatedMinMax[0].ToString("F3")}");

            DrvIC.AFMove(ch, Condition.AFRatedStrokeMinCode);
            Wait(Condition.AFRatedStrokeDelay);
            res = Measure();
            AFRatedMinMax[1] = res.cz[0];
            AddLog(ch, $"AF Inf Pos = {(AFRatedMinMax[1] - AFRatedMinMax[0]).ToString("F3")}");

            DrvIC.AFMove(ch, Condition.AFRatedStrokeMaxCode);
            Wait(Condition.AFRatedStrokeDelay);
            res = Measure();
            AFRatedMinMax[2] = res.cz[0];
            AddLog(ch, $"AF Mac Pos = {(AFRatedMinMax[2] - AFRatedMinMax[0]).ToString("F3")}");

            Stroke = AFRatedMinMax[2] - AFRatedMinMax[1];
            AddLog(ch, $"AF Rated Stroke = {Stroke.ToString("F3")}");


            double AfSensitivity = Stroke / (Condition.AFRatedStrokeMaxCode - Condition.AFRatedStrokeMinCode);
            PassFails[0].Results[(int)SpecItem.AF_Ratedstroke2].Val = Stroke;
            PassFails[0].Results[(int)SpecItem.AFSensitivity].Val = AfSensitivity;
            ShowDataResults(ch, (int)SpecItem.AF_Ratedstroke2, (int)SpecItem.AFSensitivity, InspType.Normal, new double[] { });

            LEDs_All_On(ch, false);
        }


        void OISPM(int ch, string testItem, int inspCnt)
        {

            double[] res = new double[2];

            res[0] = OISPMGM(ch, 0, 0, Condition.iXChirpFrom, Condition.iXChirpTo, Condition.XPMInspCnt, Condition.iXAmplitude);
            res[1] = OISPMGM(ch, 1, 0, Condition.iYChirpFrom, Condition.iYChirpTo, Condition.YPMInspCnt, Condition.iYAmplitude);

            PassFails[0].Results[(int)SpecItem.FRAX_PhaseMargin].Val = res[0];
            ShowDataResults(ch, (int)SpecItem.FRAX_PhaseMargin, (int)SpecItem.FRAX_PhaseMargin, InspType.Normal, new double[] { });
            PassFails[0].Results[(int)SpecItem.FRAY_PhaseMargin].Val = res[1];
            ShowDataResults(ch, (int)SpecItem.FRAY_PhaseMargin, (int)SpecItem.FRAY_PhaseMargin, InspType.Normal, new double[] { });
        }
        void OISGM(int ch, string testItem, int inspCnt)
        {
            double[] res = new double[2];

            res[0] = OISPMGM(ch, 0, 1, Condition.iXChirpFromGM, Condition.iXChirpToGM, Condition.XGMInspCnt, Condition.iXAmplitudeGM);
            res[1] = OISPMGM(ch, 1, 1, Condition.iYChirpFromGM, Condition.iYChirpToGM, Condition.YGMInspCnt, Condition.iYAmplitudeGM);

            PassFails[0].Results[(int)SpecItem.FRAX_GainMargin].Val = res[0];
            ShowDataResults(ch, (int)SpecItem.FRAX_GainMargin, (int)SpecItem.FRAX_GainMargin, InspType.Normal, new double[] { });
            PassFails[0].Results[(int)SpecItem.FRAY_GainMargin].Val = res[1];
            ShowDataResults(ch, (int)SpecItem.FRAY_GainMargin, (int)SpecItem.FRAY_GainMargin, InspType.Normal, new double[] { });
        }

        void OISLoopGain(int ch, string testItem, int inspCnt)
        {
            double[] res = new double[2];

            res[0] = OISLG(ch, 0, Condition.OISXLoopGainAmp, Condition.OISXLoopGainFreq);
            res[1] = OISLG(ch, 1, Condition.OISYLoopGainAmp, Condition.OISYLoopGainFreq);

            PassFails[0].Results[(int)SpecItem.FRAX_LoopGain].Val = res[0];
            ShowDataResults(ch, (int)SpecItem.FRAX_LoopGain, (int)SpecItem.FRAX_LoopGain, InspType.Normal, new double[] { });
            PassFails[0].Results[(int)SpecItem.FRAY_LoopGain].Val = res[1];
            ShowDataResults(ch, (int)SpecItem.FRAY_LoopGain, (int)SpecItem.FRAY_LoopGain, InspType.Normal, new double[] { });
        }

        void Act_AFScanAging(int ch, string testItem, int InspCnt)
        {
            AddLog(ch, "<<<  AF Scan aging Start  >>>");
            int target = 0, readhall = 0;
            int stepSize = Condition.AFScanAgingStep;
            int stepDelay = Condition.AFScanAgingDelay;
            stepSize = 256;
            stepDelay = 30;

            AddLog(ch, $"Start aging {Condition.AFSCanAgingCount} cycle for AF Driving");
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x00);
            DrvIC.AFMove(ch, DrvIC.AF_MID_CODE);
            Wait(100);


            for (int i = 0; i < Condition.AFSCanAgingCount; i++)
            {
                for (target = 2047; target >= 0; target -= stepSize)
                {
                    if (target <= 0) target = 0;
                    DrvIC.AFMove(ch, target); Wait(stepDelay);

                }
                for (target = 0; target <= 4095; target += stepSize)
                {
                    if (target >= 4095) target = 4095;
                    DrvIC.AFMove(ch, target); Wait(stepDelay);
                }
                for (target = 4095; target >= 2047; target -= stepSize)
                {
                    if (target <= 2047) target = 2047;
                    DrvIC.AFMove(ch, target); Wait(stepDelay);
                }
            }
            AddLog(ch, "<<<  AF Scan aging End  >>>");
            PassFails[0].Results[(int)SpecItem.AFScanAging].Val = 1;
            ShowDataResults(ch, (int)SpecItem.AFScanAging, (int)SpecItem.AFScanAging, InspType.Normal, new double[] { });
        }
        void Act_PreAFDriving(int ch, string testItem, int InspCnt)
        {
            LEDs_All_On(0, true);
            AddLog(ch, "AF Pre Driving");
            DrvIC.OISOnOff(ch, false);
            DrvIC.AFOnOff(ch, true);
         
            FindResult res = new FindResult();

            for (int i = 0; i < 5; i++)
            {
                double[] MtoM = new double[2];
                DrvIC.AFMove(ch, 2048); Wait(50);

                DrvIC.AFMove(ch, 100); Wait(20);
                DrvIC.AFMove(ch, 20); Wait(20);
                DrvIC.AFMove(ch, 10); Wait(20);
                DrvIC.AFMove(ch, 0); Wait(50);
                res = Measure();
                MtoM[0] = res.cz[0];
                DrvIC.AFMove(ch, 4095 - 100); Wait(20);
                DrvIC.AFMove(ch, 4095 - 20); Wait(20);
                DrvIC.AFMove(ch, 4095 - 10); Wait(20);
                DrvIC.AFMove(ch, 4095); Wait(50);
                res = Measure();
                MtoM[1] = res.cz[0];

                AddLog(ch, $"{i + 1} scan stroke : {Math.Abs(MtoM[1] - MtoM[0]).ToString("F3")}");
            }
            LEDs_All_On(0, false);
        }


        int AFPOSVT, AFNEGVT;
       
        public bool Load_AFPID(string path)
        {
            try
            {
                string textVal = File.ReadAllText(path);
                string[] t = textVal.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                IC_DATA_AF = new byte[(t.Length - 2)];
                IC_DATA_AF_REG = new byte[(t.Length - 2)];
                AFPIDVersion = 0xFF;
                for (int i = 0; i < t.Length; i++)
                {
                    if(i == 0)
                    {
                        string[] b = t[i].Split(new string[] { ",", " ", "\t", "//", "Reg", "AF", "PID", "Version" }, StringSplitOptions.RemoveEmptyEntries);
                        AFPIDVersion = Convert.ToByte(b[0], 16);
                    }
                    else if (i == 1)
                    {
                        string[] b = t[i].Split(new string[] { ",", " ", "\t", "//", "Reg" }, StringSplitOptions.RemoveEmptyEntries);
                        IC_SETTING_AF = new byte[b.Length / 2];
                        IC_SETTING_AF_REG = new byte[b.Length / 2];
                        for (int j = 0; j < b.Length; j++)
                        {
                            if (j < b.Length / 2) IC_SETTING_AF[j] = Convert.ToByte(b[j], 16);
                            else IC_SETTING_AF_REG[j - b.Length / 2] = Convert.ToByte(b[j], 16);
                        }
                    }
                    else
                    {
                        string[] b = t[i].Split(new string[] { ",", " ", "\t"}, StringSplitOptions.RemoveEmptyEntries);

                        IC_DATA_AF_REG[(i - 2)] = Convert.ToByte(b[0], 16);
                        IC_DATA_AF[(i - 2)] = Convert.ToByte(b[1], 16);
                     
                    }

                }
                return true;
            }
            catch { return false; }
           
        }

        void AF_HallCalibration(int ch, string testItem, int InspCnt)
        {
            bool res = false;
            res = DrvIC.ChangeSlaveAddr(ch);
            if(!res)
            {
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
                return;
            }

            //Dln.PowerSequence(0);
            //Wait(100);

          
            int agingCount;
            double OldStroke = 0, NewStroke = 0;
            FindResult tmpres = new FindResult();
            double[] zVal = new double[2];
            double AvgOLStroke = 0;
            DrvIC.AFOnOff(ch, false);
            Wait(5);
            byte backdata = 0xff;

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            if (Condition.AFMechaOnOff == 1)
            {
                backdata = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x0B, 1);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0x1A, 1, 0x00);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, (byte)(backdata & 0x7F));
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x7B);
                DrvIC.AFOnOff(ch, true);
                AddLog(ch, $"AF Openloop Aging");
                LEDs_All_On(0, true);



                for (int i = 0; i < Condition.AFOLLoopCount; i++)
                {

                    DrvIC.AFMoveOL(ch, Condition.AFOLMax);
                    Wait(Condition.AFOLDelay);

                    DrvIC.AFMoveOL(ch, Condition.AFOLMin);
                    Wait(Condition.AFOLDelay);
                }

                DrvIC.AFOnOff(ch, false);
                Wait(5);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, backdata);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x00);
            }

            AddLog(ch, $"\r\nAuto calibration\r\n");
            //Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[1], 1, IC_SETTING_AF[1]);
            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[0], 1, IC_SETTING_AF[0]);

            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[3], 1, IC_SETTING_AF[3]);
            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[4], 1, IC_SETTING_AF[4]);
            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[5], 1, IC_SETTING_AF[5]);
            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[6], 1, IC_SETTING_AF[6]);

            for (int i = 0xC0; i <= 0xC3; i++)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, i, 1, 0x00);
               
            }
            AddLog(ch, $"Reset EPA Data.");
            for (int i = 0xC5; i <= 0xDF; i++)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, i, 1, 0x00);
              
            }
            AddLog(ch, $"Reset Linearity comp coeff data.");
            for (int i = 0; i < IC_DATA_AF.Length; i++)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, IC_DATA_AF_REG[i], 1, IC_DATA_AF[i]);
            }
            AddLog(ch, $"PID Parameter setting.");

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x5D, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x80);
            Wait(10);
            byte rbuf = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x70, 1);
            byte cBackup;
            byte cTemp;
            cBackup = cTemp = rbuf;
            AddLog(ch, $"1 Reg 0x70 : 0x{cBackup.ToString("X2")}");
            cBackup &= 0x80;
            AddLog(ch, $"2 Reg 0x70 : 0x{cBackup.ToString("X2")}");
            cTemp = (byte)((cTemp << 1) & 0x7E);
            cTemp |= cBackup;
            AddLog(ch, $"3 Reg 0x70 : 0x{cTemp.ToString("X2")}");
            rbuf = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x71, 1);
            cBackup = rbuf;
            cBackup &= 0x80;
            AddLog(ch, $"4 Reg 0x71 : 0x{cBackup.ToString("X2")}");
            cTemp |= (byte)(cBackup >> 7);
            AddLog(ch, $"4 Reg 0x5D : 0x{cTemp.ToString("X2")}");

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x5D, 1, cTemp);
            
            
            int index = 0;
            cTemp = 0xff;
            while(cTemp > 0x10 && index < 5)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[2], 1, IC_SETTING_AF[2]);
                for (int i = 0; i < 2; i++)
                {
                    Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x18);
                    Wait(300);
                }
               
                cTemp = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x19, 1);
                int iTemp = cTemp;
             //   AddLog(ch, $"Reg : 0x19 = 0x{iTemp.ToString("X2")}");
                iTemp = iTemp * 3 / 4;
                if (iTemp < 0) iTemp = 0;
                if (iTemp > 255) iTemp = 255;
                cTemp = (byte)iTemp;
                AddLog(ch, $"Reg : 0x19 = 0x{iTemp.ToString("X2")}");
                if(cTemp > 0x10)
                {
                    //Error처리
                    AddLog(ch, $"AF calibration 2 (Reg 19) error[over 0x10]");
                }
                index++;
            }

            if(index >= 5)
            {
                AddLog(ch, $"AF Hall Calibration NG");
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
                return;
            }
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x19, 1, cTemp);
            
            res = DrvIC.AF_Memory_Update(ch, 1);
            res &= DrvIC.AF_Memory_Update(ch, 2);
            res &= DrvIC.AF_Memory_Update(ch, 3);
            res &= DrvIC.AF_Memory_Update(ch, 4);
            res &= DrvIC.AF_Memory_Update(ch, 5);
            if(!res)
            {
                AddLog(ch, $"AF Memory update NG");
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
                return;
            }

            backdata = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x0B, 1);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x1A, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, (byte)(backdata & 0x7F));
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x7B);
            DrvIC.AFOnOff(ch, true);
            AddLog(ch, $"AF Openloop Stroke Check");
            LEDs_All_On(0, true);

            List<double> OLStroke = new List<double>();


            for (int i = 0; i < Condition.AFOLLoopCount; i++)
            {
                OldStroke = NewStroke;
                DrvIC.AFMoveOL(ch, Condition.AFOLMax);
                Wait(Condition.AFOLDelay);
                tmpres = Measure();
                zVal[0] = tmpres.cz[0];
                DrvIC.AFMoveOL(ch, Condition.AFOLMin);
                Wait(Condition.AFOLDelay);
                tmpres = Measure();
                zVal[1] = tmpres.cz[0];
                NewStroke = Math.Abs(zVal[1] - zVal[0]);
                AddLog(ch, $"{i + 1} : {NewStroke.ToString("F3")}");

                OLStroke.Add(NewStroke);

            }
            if (Condition.AFOLLoopCount >= 3)
            {
                int MaxIndex = OLStroke.Select((value, idx) => new { value, idx }).OrderByDescending(x => x.value).First().idx;
                OLStroke.RemoveAt(MaxIndex);
                int MinIndex = OLStroke.Select((value, idx) => new { value, idx }).OrderBy(x => x.value).First().idx;
                OLStroke.RemoveAt(MinIndex);
            }


            for (int i = 0; i < OLStroke.Count; i++)
            {
                AvgOLStroke += OLStroke[i];
            }


            AvgOLStroke = AvgOLStroke / OLStroke.Count;
            PassFails[0].Results[(int)SpecItem.AFMechaStroke].Val = AvgOLStroke;
            ShowDataResults(ch, (int)SpecItem.AFMechaStroke, (int)SpecItem.AFMechaStroke, InspType.Normal, new double[] { });
            //    LEDs_All_On(0, false);

            DrvIC.AFOnOff(ch, false);
            Wait(5);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, backdata);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x00);



            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            Dln.PowerSequence(0);

            DrvIC.AFOnOff(ch, false);

            int poscal = 0;
            int negcal = 0;
            (poscal, negcal) =  DrvIC.AF_IC_Data(ch);


            if (Option.SaveRawData)
            {
                StreamWriter sw = null;
                string dateDir = STATIC.CreateDateDir();
                if (!Directory.Exists(dateDir)) Directory.CreateDirectory(dateDir);
                string path = dateDir + $"AF_HallCalData.csv";

                if (!File.Exists(path))
                {
                    sw = File.AppendText(path);
                    string s = $"SPL No, Date, Time, PCAL, NCAL";
                    sw.WriteLine(s);
                    sw.Close();
                }
                sw = File.AppendText(path);
                string data = $"{m_StrIndex[ch]},{STATIC.LogDate.ToString("yyyy-MM-dd")},{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s," +
                    $"{poscal},{negcal}";
                sw.WriteLine(data);
                sw.Close();
            }

            (bool EPARes, int stroke) = AF_EPA(ch);
            if (!EPARes)
            {
                AddLog(ch, $"AF EPA NG");
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = stroke;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
                return;
            }
            AddLog(ch, "<<<  AF Lin. Comp Start  >>>");
            bool LinRes = AFLinComp(ch, 8, 4088, 34, 0, 0, 0, 0, 0);
            AddLog(ch, "<<<  AF Lin. Comp End  >>>");
            if (!LinRes)
            {
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
                return;
            }
            PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = stroke;
            ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });

        }
        void AF_OnlyHallCalibration(int ch, string testItem, int InspCnt)
        {
           
            bool res = false;
            res = DrvIC.ChangeSlaveAddr(ch);
            if (!res)
            {
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
                return;
            }

            //Dln.PowerSequence(0);
            //Wait(100);


            int agingCount;
            double OldStroke = 0, NewStroke = 0;
            FindResult tmpres = new FindResult();
            double[] zVal = new double[2];
            double AvgOLStroke = 0;
            DrvIC.AFOnOff(ch, false);
            Wait(5);

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            byte backdata = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x0B, 1);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x1A, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, (byte)(backdata & 0x7F));
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x7B);
            DrvIC.AFOnOff(ch, true);
            AddLog(ch, $"AF Openloop Stroke Check");
            LEDs_All_On(0, true);

            for (int i = 0; i < Condition.AFOLLoopCount; i++)
            {
                OldStroke = NewStroke;
                DrvIC.AFMove(ch, Condition.AFOLMax);
                Wait(Condition.AFOLDelay);
                tmpres = Measure();
                zVal[0] = tmpres.cz[0];
                DrvIC.AFMove(ch, Condition.AFOLMin);
                Wait(Condition.AFOLDelay);
                tmpres = Measure();
                zVal[1] = tmpres.cz[0];
                NewStroke = Math.Abs(zVal[1] - zVal[0]);
                AddLog(ch, $"{i + 1} : {NewStroke.ToString("F3")}");

                if (i != 0) AvgOLStroke += NewStroke;

            }
            AvgOLStroke = AvgOLStroke / (Condition.AFOLLoopCount - 1);
            PassFails[0].Results[(int)SpecItem.AFMechaStroke].Val = AvgOLStroke;
            ShowDataResults(ch, (int)SpecItem.AFMechaStroke, (int)SpecItem.AFMechaStroke, InspType.Normal, new double[] { });
            LEDs_All_On(0, false);

            //AddLog(ch, $"{Current.AFPidPath}");
            //res = Load_AFPID(Current.AFPidPath);
            //if (!res)
            //{
            //    AddLog(ch, $"Load PID NG");
            //    PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
            //    ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
            //    return;
            //}
            DrvIC.AFOnOff(ch, false);
            Wait(5);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x00);


            AddLog(ch, $"\r\nAuto calibration\r\n");
            //Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[1], 1, IC_SETTING_AF[1]);
            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[0], 1, IC_SETTING_AF[0]);

            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[3], 1, IC_SETTING_AF[3]);
            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[4], 1, IC_SETTING_AF[4]);
            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[5], 1, IC_SETTING_AF[5]);
            Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[6], 1, IC_SETTING_AF[6]);

            for (int i = 0xC0; i <= 0xC3; i++)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, i, 1, 0x00);

            }
            AddLog(ch, $"Reset EPA Data.");
            for (int i = 0xC5; i <= 0xDF; i++)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, i, 1, 0x00);

            }
            AddLog(ch, $"Reset Linearity comp coeff data.");
            for (int i = 0; i < IC_DATA_AF.Length; i++)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, IC_DATA_AF_REG[i], 1, IC_DATA_AF[i]);
            }
            AddLog(ch, $"PID Parameter setting.");

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x5D, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x80);
            Wait(10);
            byte rbuf = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x70, 1);
            byte cBackup;
            byte cTemp;
            cBackup = cTemp = rbuf;
            AddLog(ch, $"1 Reg 0x70 : 0x{cBackup.ToString("X2")}");
            cBackup &= 0x80;
            AddLog(ch, $"2 Reg 0x70 : 0x{cBackup.ToString("X2")}");
            cTemp = (byte)((cTemp << 1) & 0x7E);
            cTemp |= cBackup;
            AddLog(ch, $"3 Reg 0x70 : 0x{cTemp.ToString("X2")}");
            rbuf = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x71, 1);
            cBackup = rbuf;
            cBackup &= 0x80;
            AddLog(ch, $"4 Reg 0x71 : 0x{cBackup.ToString("X2")}");
            cTemp |= (byte)(cBackup >> 7);
            AddLog(ch, $"4 Reg 0x5D : 0x{cTemp.ToString("X2")}");

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x5D, 1, cTemp);


            int index = 0;
            cTemp = 0xff;
            while (cTemp > 0x10 && index < 5)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[2], 1, IC_SETTING_AF[2]);
                for (int i = 0; i < 2; i++)
                {
                    Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x18);
                    Wait(300);
                }

                cTemp = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x19, 1);
                int iTemp = cTemp;
                //   AddLog(ch, $"Reg : 0x19 = 0x{iTemp.ToString("X2")}");
                iTemp = iTemp * 3 / 4;
                if (iTemp < 0) iTemp = 0;
                if (iTemp > 255) iTemp = 255;
                cTemp = (byte)iTemp;
                AddLog(ch, $"Reg : 0x19 = 0x{iTemp.ToString("X2")}");
                if (cTemp > 0x10)
                {
                    //Error처리
                    AddLog(ch, $"AF calibration 2 (Reg 19) error[over 0x10]");
                }
                index++;
            }

            if (index >= 5)
            {
                AddLog(ch, $"AF Hall Calibration NG");
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
                return;
            }
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x19, 1, cTemp);

            res = DrvIC.AF_Memory_Update(ch, 1);
            res &= DrvIC.AF_Memory_Update(ch, 2);
            res &= DrvIC.AF_Memory_Update(ch, 3);
            res &= DrvIC.AF_Memory_Update(ch, 4);
            res &= DrvIC.AF_Memory_Update(ch, 5);
            if (!res)
            {
                AddLog(ch, $"AF Memory update NG");
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
                return;
            }

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            Dln.PowerSequence(0);

            DrvIC.AFOnOff(ch, false);
            int poscal = 0;
            int negcal = 0;
            (poscal, negcal) = DrvIC.AF_IC_Data(ch);


            if (Option.SaveRawData)
            {
                StreamWriter sw = null;
                string dateDir = STATIC.CreateDateDir();
                if (!Directory.Exists(dateDir)) Directory.CreateDirectory(dateDir);
                string path = dateDir + $"AF_HallCalData.csv";

                if (!File.Exists(path))
                {
                    sw = File.AppendText(path);
                    string s = $"SPL No, Date, Time, PCAL, NCAL";
                    sw.WriteLine(s);
                    sw.Close();
                }
                sw = File.AppendText(path);
                string data = $"{m_StrIndex[ch]},{STATIC.LogDate.ToString("yyyy-MM-dd")},{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s," +
                    $"{poscal},{negcal}";
                sw.WriteLine(data);
                sw.Close();
            }

            //(bool EPARes, int stroke) = AF_EPA(ch);
            //if (!EPARes)
            //{
            //    AddLog(ch, $"AF EPA NG");
            //    PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = stroke;
            //    ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
            //    return;
            //}
            //AddLog(ch, "<<<  AF Lin. Comp Start  >>>");
            //bool LinRes = AFLinComp(ch, 8, 4088, 34, 0, 0, 6, 6, 0);
            //AddLog(ch, "<<<  AF Lin. Comp End  >>>");
            //if (!LinRes)
            //{
            //    PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
            //    ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
            //    return;
            //}
            //PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = stroke;
            //ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });

        }
        (bool, int) AF_EPA(int ch)
        {
            int BTM_POS = 20;
            //int BTM_POS = 30;
            //int TTL_RNG = 400;
            int TTL_RNG = 530;
            int TOP_POS = BTM_POS + TTL_RNG;
            int TOP_MARGIN = 10;
            LEDs_All_On(0, true);
            //AF EPA
            AddLog(ch, "<<<  AF EPA Start  >>>");
            int btm_position, tmp_position, top_position, ctr_position;
            int step, inf_cut, mac_cut;
            uint posvt, negvt, target_code;
            int stroke;
            int loop = 0, mac_loop = 0;
            int new_con = 0, old_con = 0, cond = 0;
            int mac_loop_max = 50;
            uint inf_tag_code, mac_tag_code;	// save code value
            int imgIndex = 0;
            FindResult res = new FindResult();
            DrvIC.AFOnOff(ch, true); Wait(100);
            DrvIC.AFMove(ch, DrvIC.AF_MID_CODE); Wait(50);
            res = Measure().SaveImage(m__G, 0, m__G.m_RootDirectory + "\\Result\\RawData\\ImgAna\\Epa_" + (imgIndex++).ToString());
            ctr_position = (int)res.cz[0];
            DrvIC.AFMove(ch, DrvIC.AF_MIN_CODE + 100); Wait(20);
            DrvIC.AFMove(ch, DrvIC.AF_MIN_CODE + 20); Wait(20);
            DrvIC.AFMove(ch, DrvIC.AF_MIN_CODE + 10); Wait(20);
            DrvIC.AFMove(ch, DrvIC.AF_MIN_CODE); Wait(150);
            res = Measure().SaveImage(m__G, 0, m__G.m_RootDirectory + "\\Result\\RawData\\ImgAna\\Epa_" + (imgIndex++).ToString());
            int refPos = btm_position = (int)res.cz[0];
            tmp_position = 0;
            AddLog(ch, $"Inf Cut Start");

            for (target_code = 0, step = 0x200; step > 0; step >>= 1)
            {

                AddLog(ch, $"tmp_pos:{tmp_position}, tar_code:{target_code}, step:{step}");
                if (tmp_position < BTM_POS - 1) target_code += (ushort)step;
                else if (tmp_position > BTM_POS + 1) target_code -= (ushort)step;
                else break;
                DrvIC.AFMove(ch, (int)target_code);
                Wait(200);
                res = Measure().SaveImage(m__G, 0, m__G.m_RootDirectory + "\\Result\\RawData\\ImgAna\\Epa_" + (imgIndex++).ToString());
                tmp_position = btm_position = (short)(res.cz[0] - refPos);
                loop++;
            }
            inf_tag_code = target_code;
            AddLog(ch, $"Inf_loop : {loop}");
            negvt = target_code;
            AddLog(ch, $"negvt = {negvt}");
            inf_cut = tmp_position;
            if ((inf_cut < (BTM_POS - 1)) || (inf_cut > (BTM_POS + 1)))
            {
                AddLog(ch, $"EPA Error");

                LEDs_All_On(0, false);
                return (false, 0);
            }
            AddLog(ch, $"");
            DrvIC.AFMove(ch, DrvIC.AF_MAX_CODE - 100); Wait(20);
            DrvIC.AFMove(ch, DrvIC.AF_MAX_CODE - 20); Wait(20);
            DrvIC.AFMove(ch, DrvIC.AF_MAX_CODE - 10); Wait(20);
            DrvIC.AFMove(ch, DrvIC.AF_MAX_CODE); Wait(150);
            res = Measure().SaveImage(m__G, 0, m__G.m_RootDirectory + "\\Result\\RawData\\ImgAna\\Epa_" + (imgIndex++).ToString());
            top_position = (int)res.cz[0];
            tmp_position = 0;
            stroke = Math.Abs(refPos - top_position);

            if (stroke > TOP_POS + TOP_MARGIN)
            {
                mac_cut = (short)(stroke - (TOP_POS));
                step = 0x300;
            }
            else
            {
                mac_cut = TOP_MARGIN;
                step = 0x200;
            }
            AddLog(ch, "Mac Cut Start");
            AddLog(ch, $"Mac_Cut:{mac_cut}, Mac_Step:{step}");

            for (target_code = 4095; step > 0; step >>= 1)
            {
                string s = string.Empty;
                s += $"tmp_pos:{tmp_position}, tar_code:{target_code},";

                if (tmp_position < -1 - mac_cut)
                {
                    if (cond == 2)
                    {
                        step = (short)(step << 1);
                    }
                    target_code += (ushort)step;
                    cond = 2;
                    s += $"step:{step}, cond:{cond}";
                    AddLog(ch, s);
                }
                else if (tmp_position > 1 - mac_cut)
                {
                    if (cond == 3)
                    {
                        step = (short)(step << 1);
                    }
                    target_code -= (ushort)step;
                    cond = 3;
                    s += $"step:{step}, cond:{cond}";
                    AddLog(ch, s);
                }
                else break;
                DrvIC.AFMove(ch, (int)target_code);
                Wait(200);
                res = Measure().SaveImage(m__G, 0, m__G.m_RootDirectory + "\\Result\\RawData\\ImgAna\\Epa_" + (imgIndex++).ToString());
                tmp_position = (int)(res.cz[0] - top_position);
                mac_loop++;

                if (mac_loop > mac_loop_max) break;
            }
            mac_tag_code = target_code;

            if (mac_loop > mac_loop_max)
            {
                AddLog(ch, "Mac Cut Error");
                AddLog(ch, $"EPA Error");
                LEDs_All_On(0, false);
                return (false, 3);
            }
            AddLog(ch, $"tmp_pos:{tmp_position}, tar_code:{target_code}, mac_loop:{mac_loop}");
            posvt = target_code;
            AddLog(ch, $"posvt = {posvt}");
            AddLog(ch, "---------------------------------");
            AddLog(ch, $"Target stroke : {TTL_RNG}um");
            AddLog(ch, $"Target btm_top MG : {BTM_POS}_{TOP_MARGIN} um");
            AddLog(ch, $"Measured stroke : {stroke}um");
            AddLog(ch, $"Measured Mac_cut : {mac_cut}um");
            AddLog(ch, $"Inf cut-off size : {inf_cut}um");
            AddLog(ch, $"Mac cut-off size : {Math.Abs(tmp_position)}um");
            AddLog(ch, "---------------------------------");

            DrvIC.AFMove(ch, DrvIC.AF_MID_CODE); Wait(50);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, IC_SETTING_AF[1]);

            AFPOSVT = (int)((posvt - 4095) << 4);
            AFNEGVT = (int)(negvt << 4);

            AddLog(ch, $"AFPOSVT = {AFPOSVT}");
            AddLog(ch, $"AFNEGVT = {AFNEGVT}");

            Dln.Write2Byte(ch, DrvIC.AF_Addr, 0xC0, 1, (ushort)AFPOSVT);
            Dln.Write2Byte(ch, DrvIC.AF_Addr, 0xC2, 1, (ushort)AFNEGVT);

            bool WriteCheck = DrvIC.AF_Memory_Update(ch, 1);
            WriteCheck &= DrvIC.AF_Memory_Update(ch, 5);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            DrvIC.AFOnOff(ch, true);
            DrvIC.AF_IC_Data(ch);
            LEDs_All_On(0, false);

            if (Option.SaveRawData)
            {
                StreamWriter sw = null;
                string dateDir = STATIC.CreateDateDir();
                if (!Directory.Exists(dateDir)) Directory.CreateDirectory(dateDir);
                string path = dateDir + $"AF_EPA_CODE.csv";

                if (!File.Exists(path))
                {
                    sw = File.AppendText(path);
                    string s = $"SPL No, Date, Time, INF Code, MAC Code";
                    sw.WriteLine(s);
                    sw.Close();
                }
                sw = File.AppendText(path);
                string data = $"{m_StrIndex[ch]},{STATIC.LogDate.ToString("yyyy-MM-dd")},{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s," +
                    $"{inf_tag_code},{mac_tag_code}";
                sw.WriteLine(data);
                sw.Close();
            }
            AddLog(ch, "<<<  AF EPA End  >>>");


            return (true, stroke);
        }
        void OIS_FWDownload(int ch, string testItem, int InspCnt)
        {
            List<ushort> FWaddr = new List<ushort>();
            List<byte[]> FWdata = new List<byte[]>();
            List<ushort> Caladdr = new List<ushort>();
            List<byte[]> Caldata = new List<byte[]>();


            if (!File.Exists(Current.OISFWPath))
            {
                AddLog(ch, $"OIS FW file not exist");
                PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                return;
            }


            if (!File.Exists(Current.OISBaseCalPath))
            {
                AddLog(ch, $"OIS Cal file not exist");
                PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                return;
            }

            StreamReader sr = new StreamReader(Current.OISFWPath);
            string tmpdata = sr.ReadToEnd();
            sr.Close();

            string[] splitData = tmpdata.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitData.Length; i++)
            {
                if (splitData[i].Length < 11) continue;
                string[] DataArr = splitData[i].Split('_');
                FWaddr.Add(Convert.ToUInt16(DataArr[1].Substring(2), 16));
                FWdata.Add(new byte[DataArr[2].Length / 2]);
                for (int j = 0; j < DataArr[2].Length / 2; j++)
                    FWdata[FWdata.Count - 1][j] = Convert.ToByte(DataArr[2].Substring(j * 2, 2), 16);
            }

            sr = new StreamReader(Current.OISBaseCalPath);
            tmpdata = sr.ReadToEnd();
            sr.Close();
            splitData = tmpdata.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < splitData.Length; i++)
            {
                if (splitData[i].Length < 11) continue;
                string[] DataArr = splitData[i].Split('_');
                Caladdr.Add(Convert.ToUInt16(DataArr[1].Substring(2), 16));
                Caldata.Add(new byte[DataArr[2].Length / 2]);
                for (int j = 0; j < DataArr[2].Length / 2; j++)
                    Caldata[Caldata.Count - 1][j] = Convert.ToByte(DataArr[2].Substring(j * 2, 2), 16);
            }

           

            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6017, 2, 0x04);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6018, 2, 0xAA);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6019, 2, 0x2A);

            bool statusCheck = DrvIC.OIS_StausCheck(ch, 0xD1, 0xD1);
            if (!statusCheck)
            {
                PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                return;
            }
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0xF010, 2, 0x00);


            for (int i = 0; i < FWaddr.Count; i++)
            {
                bool res = Dln.WriteArray(ch, DrvIC.OIS_Addr, FWaddr[i], 2, FWdata[i]);
                if (!res)
                {
                    AddLog(ch, $"FW Data Write Fail");
                    PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                    ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                    return;
                }
            }

            uint checkSum = Dln.Read4Byte(ch, DrvIC.OIS_Addr, 0xF008, 2);
            AddLog(ch, $"Checksum = 0x{checkSum.ToString("X8")}");
            if(checkSum != Convert.ToUInt32(Condition.OISFWChecksum, 16))
            {
                AddLog(ch, $"FW Download Checksum NG");
                PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                return;
            }


            for (int i = 0; i < Caladdr.Count; i++)
            {
                bool res = Dln.WriteArray(ch, DrvIC.OIS_Addr, Caladdr[i], 2, Caldata[i]);
                if (!res)
                {
                    AddLog(ch, $"Cal Data Write Fail");
                    PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                    ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                    return;
                }
            }

            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0xF006, 2, 0x00);
            Wait(1);

            statusCheck = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if (!statusCheck)
            {
                PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                return;
            }
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x7000, 2, 0x00);
            Wait(1);
            uint ProgID = Dln.Read4Byte(ch, DrvIC.OIS_Addr, 0x6010, 2);
            AddLog(ch, $"ProgramID = 0x{ProgID.ToString("X8")}");
            if (ProgID != Convert.ToUInt32(Condition.OISFWProgID, 16))
            {
                AddLog(ch, $"FW Download Program ID NG");
                PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                return;
            }

            PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 0;
            ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });

        }
        void OIS_FWDownload_OnFlash(int ch, string testItem, int InspCnt)
        {
            List<ushort> FWaddr = new List<ushort>();
            List<byte[]> FWdata = new List<byte[]>();
            List<ushort> Caladdr = new List<ushort>();
            List<byte[]> Caldata = new List<byte[]>();


            if (!File.Exists(Current.OISFWPath))
            {
                AddLog(ch, $"OIS FW file not exist");
                PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                return;
            }


            if (!File.Exists(Current.OISBaseCalPath))
            {
                AddLog(ch, $"OIS Cal file not exist");
                PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                return;
            }

            StreamReader sr = new StreamReader(Current.OISFWPath);
            string tmpdata = sr.ReadToEnd();
            sr.Close();

            string[] splitData = tmpdata.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitData.Length; i++)
            {
                if (splitData[i].Length < 11) continue;
                string[] DataArr = splitData[i].Split('_');
                FWaddr.Add(Convert.ToUInt16(DataArr[1].Substring(2), 16));
                FWdata.Add(new byte[DataArr[2].Length / 2]);
                for (int j = 0; j < DataArr[2].Length / 2; j++)
                    FWdata[FWdata.Count - 1][j] = Convert.ToByte(DataArr[2].Substring(j * 2, 2), 16);
            }

            sr = new StreamReader(Current.OISBaseCalPath);
            tmpdata = sr.ReadToEnd();
            sr.Close();
            splitData = tmpdata.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < splitData.Length; i++)
            {
                if (splitData[i].Length < 11) continue;
                string[] DataArr = splitData[i].Split('_');
                FWaddr.Add(Convert.ToUInt16(DataArr[1].Substring(2), 16));
                FWdata.Add(new byte[DataArr[2].Length / 2]);
                for (int j = 0; j < DataArr[2].Length / 2; j++)
                    FWdata[FWdata.Count - 1][j] = Convert.ToByte(DataArr[2].Substring(j * 2, 2), 16);
            }

            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0xF0C0, 2, 0x55);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x7010, 2, 0x00);
            Wait(5);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x7000, 2, 0x00);
            Wait(1);
            byte tmpByte = Dln.ReadByte(ch, DrvIC.OIS_Addr, 0x6024, 2);
            if((tmpByte & 0x07) != 0x01)
            {
                AddLog(ch, $"Status Check Fail");
                PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                return;
            }
            Wait(1);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6007, 2, 0xA2);
            Wait(1);
            Dln.WriteByte(ch, 0x51, 0xF000, 2, 0x01);
            Dln.WriteByte(ch, 0x51, 0xFF0f, 2, 0xA5);
            tmpByte = Dln.ReadByte(ch, 0x51, 0xFFFF, 2);
            if ((tmpByte & 0x07) != 0x01)
            {
                AddLog(ch, $"Status Check Fail");
                PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                return;
            }
            Dln.WriteByte(ch, 0x51, 0x8000, 2, 0xEA);
            Wait(10);
            for (int i = 0; i < FWaddr.Count; i++)
            {
                bool res = Dln.WriteArray(ch, 0x51, FWaddr[i], 2, FWdata[i]);
                if (!res)
                {
                    AddLog(ch, $"FW Data Write Fail");
                    PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                    ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                    return;
                }
            }

            for (int i = 0; i < Caladdr.Count; i++)
            {
                bool res = Dln.WriteArray(ch, 0x51, Caladdr[i], 2, Caldata[i]);
                if (!res)
                {
                    AddLog(ch, $"Cal Data Write Fail");
                    PassFails[0].Results[(int)SpecItem.OISFWDownload].Val = 1;
                    ShowDataResults(ch, (int)SpecItem.OISFWDownload, (int)SpecItem.OISFWDownload, InspType.OKNG, new double[] { });
                    return;
                }
            }
            Dln.WriteByte(ch, 0x51, 0xFF0F, 2, 0x5A);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0xF0C0, 2, 0x00);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x7010, 2, 0x00);


        }
        void OIS_AutoBoot(int ch, string testItem, int InspCnt)
        {

            bool status = DrvIC.OIS_StausCheck(ch, 0x01, 0x01);
            if (!status)
            {
                PassFails[0].Results[(int)SpecItem.OISAutoBoot].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISAutoBoot, (int)SpecItem.OISAutoBoot, InspType.OKNG, new double[] { });
                return;
            }
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x7000, 2, 0x00);
            Wait(3);
            uint ProgID = Dln.Read4Byte(ch, DrvIC.OIS_Addr, 0x6010, 2);
            AddLog(ch, $"ProgramID = 0x{ProgID.ToString("X8")}");
            if (ProgID != Convert.ToUInt32( Condition.OISFWProgID, 16))
            {
                AddLog(ch, $"OIS AutoBoot Program ID NG");
                PassFails[0].Results[(int)SpecItem.OISAutoBoot].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISAutoBoot, (int)SpecItem.OISAutoBoot, InspType.OKNG, new double[] { });
                return;
            }
        }
        void OIS_HallCalibration(int ch, string testItem, int InspCnt)
        {

            OISCalData = new OISCalParameter();
            //AF BestPos Move
            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMove(ch, Condition.OISCalAFPos);
            AddLog(ch, $"Move AF Position :  {Condition.OISCalAFPos}");
            DWDrvIC.OISOnOff(ch, false);

            //DWDrvIC.Controls.WriteByte()
            
            
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x60A0, 2, Convert.ToByte(Condition.OISCal60A0, 16));
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61D2, 2, Convert.ToByte(Condition.OISCal61D2, 16));
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61D3, 2, Convert.ToByte(Condition.OISCal61D3, 16));
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61D4, 2, Convert.ToByte(Condition.OISCal61D4, 16));
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x61D5, 2, Convert.ToByte(Condition.OISCal61D5, 16));
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x60A1, 2, 0x11);
            Wait(100);
            //res = DrvIC.OIS_StausCheck(ch, 0x60A1, 0x01, 0x01);
            //if (!res)
            //{
            //    //error
            //    PassFails[0].Results[(int)SpecItem.XYHallCalibration].Val = 1;
            //    ShowDataResults(ch, (int)SpecItem.XYHallCalibration, (int)SpecItem.XYHallCalibration, InspType.OKNG, new double[] { });
            //    return;
            //}
            //res = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            //if (!res)
            //{
            //    //error
            //    PassFails[0].Results[(int)SpecItem.XYHallCalibration].Val = 1;
            //    ShowDataResults(ch, (int)SpecItem.XYHallCalibration, (int)SpecItem.XYHallCalibration, InspType.OKNG, new double[] { });
            //    return;
            //}
            byte XhallCurrent = Dln.ReadByte(ch, DrvIC.OIS_Addr, 0x60A2, 2);
            byte YhallCurrent = Dln.ReadByte(ch, DrvIC.OIS_Addr, 0x60A3, 2);
            ushort XhallOfsDAC = Dln.Read2Byte(ch, DrvIC.OIS_Addr, 0x60A4, 2);
            ushort YhallOfsDAC = Dln.Read2Byte(ch, DrvIC.OIS_Addr, 0x60A6, 2);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x60CC, 2, 0x00);
            short XhallMin = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x60CE, 2);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x60CC, 2, 0x01);
            short XhallMax = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x60CE, 2);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x60CC, 2, 0x02);
            short YhallMin = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x60CE, 2);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x60CC, 2, 0x03);
            short YhallMax = Dln.Read2Byte_signed(ch, DrvIC.OIS_Addr, 0x60CE, 2);

            string s = $"X Hall current = {XhallCurrent}\r\n" +
                       $"Y Hall current = {YhallCurrent}\r\n" +
                       $"X Hall offset DAC = {XhallOfsDAC}\r\n" +
                       $"Y Hall offset DAC = {YhallOfsDAC}\r\n" +
                       $"X Hall Min = {XhallMin}\r\n" +
                       $"X Hall Max = {XhallMax}\r\n" +
                       $"X Hall Mid = {(XhallMax + XhallMin) / 2}\r\n" +
                       $"Y Hall Min = {YhallMin}\r\n" +
                       $"Y Hall Max = {YhallMax}\r\n" +
                       $"Y Hall Mid = {(YhallMax + YhallMin) / 2}\r\n";
            AddLog(ch, s);

            OISCalData.XHallCurrent = XhallCurrent;
            OISCalData.YHallCurrent = YhallCurrent;
            OISCalData.XHallOfsDAC = XhallOfsDAC;
            OISCalData.YHallOfsDAC = YhallOfsDAC;
            OISCalData.XhallMin = XhallMin;
            OISCalData.YhallMin = YhallMin;
            OISCalData.XhallMax = XhallMax;
            OISCalData.YhallMax = YhallMax;
            OISCalData.XhallMid = (XhallMax + XhallMin) / 2;
            OISCalData.YhallMid = (YhallMax + YhallMin) / 2;

            if (Option.SaveRawData)
            {
                StreamWriter sw = null;
                string dateDir = STATIC.CreateDateDir();
                if (!Directory.Exists(dateDir)) Directory.CreateDirectory(dateDir);
                string path = dateDir + $"OIS_HallCalData.csv";

                if (!File.Exists(path))
                {
                    sw = File.AppendText(path);
                    s = $"SPL No, Date, Time, X Hall Current(Bias), X Hall Offset, X Hall Min, X Hall Max, X Hall Mid, Y Hall Current(Bias), Y Hall Offset, Y Hall Min, Y Hall Max, Y Hall Mid";
                    sw.WriteLine(s);
                    sw.Close();
                }
                sw = File.AppendText(path);
                string data = $"{m_StrIndex[ch]},{STATIC.LogDate.ToString("yyyy-MM-dd")},{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s," +
                    $"{XhallCurrent},{XhallOfsDAC},{XhallMin},{XhallMax},{(XhallMax + XhallMin) / 2},{YhallCurrent},{YhallOfsDAC},{YhallMin},{YhallMax},{(YhallMax + YhallMin) / 2}";
                sw.WriteLine(data);
                sw.Close();
            }
            if (XhallMin > Condition.XCalMinSpec || XhallMax < Condition.XCalMaxSpec || YhallMin > Condition.YCalMinSpec || YhallMax < Condition.YCalMaxSpec
                || XhallMin > 0 || XhallMax < 0 || YhallMin > 0 || YhallMax < 0)
            {
                PassFails[0].Results[(int)SpecItem.XYHallCalibration].Val = 1;
                ShowDataResults(ch, (int)SpecItem.XYHallCalibration, (int)SpecItem.XYHallCalibration, InspType.OKNG, new double[] { });
                return;
            }
            PassFails[0].Results[(int)SpecItem.XYHallCalibration].Val = 0;
            ShowDataResults(ch, (int)SpecItem.XYHallCalibration, (int)SpecItem.XYHallCalibration, InspType.OKNG, new double[] { });
            return;

        }

        void OIS_Decenter_Calibration(int ch, string testItem, int InspCnt)
        {
            bool[] SkipFlag = new bool[2] { false, false };
            int[] Offset = new int[2];
            (SkipFlag[0], Offset[0]) = Decenter_Calibration(ch, 0);
            (SkipFlag[1], Offset[1]) = Decenter_Calibration(ch, 1);

            if (!SkipFlag[0])
            {
                PassFails[0].Results[(int)SpecItem.OISXDecenterCal].Val = Offset[0];
                ShowDataResults(ch, (int)SpecItem.OISXDecenterCal, (int)SpecItem.OISXDecenterCal, InspType.Normal, new double[] { });
            }
            else
            {
                PassFails[0].Results[(int)SpecItem.OISXDecenterCal].Val = 0;
                ShowDataResults(ch, (int)SpecItem.OISXDecenterCal, (int)SpecItem.OISXDecenterCal, InspType.Normal, new double[] { });
            }
            if (!SkipFlag[1])
            {
                PassFails[0].Results[(int)SpecItem.OISYDecenterCal].Val = Offset[1];
                ShowDataResults(ch, (int)SpecItem.OISYDecenterCal, (int)SpecItem.OISYDecenterCal, InspType.Normal, new double[] { });

            }
            else
            {
                PassFails[0].Results[(int)SpecItem.OISYDecenterCal].Val = 0;
                ShowDataResults(ch, (int)SpecItem.OISYDecenterCal, (int)SpecItem.OISYDecenterCal, InspType.Normal, new double[] { });
            }

        }
        (bool b, int a) Decenter_Calibration(int ch, int axis)
        {
            LEDs_All_On(0, true);
            int X1, X2, X3, X4, X5, X6;
            int Y1, Y2, Y3, Y4, Y5, Y6;
            int Initial_X_Decenter, X_Decenter1, X_Decenter2;
            int Initial_Y_Decenter, Y_Decenter1, Y_Decenter2;
            int Final_MG_Offset = -999999;
            float b, slope = 0;
            double xCenter, yCenter;

            FindResult res = new FindResult();
            bool checkStatus = false;

            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMove(ch, Condition.DecenterCalAFPos);
            AddLog(ch, $"Move AF Position = {Condition.DecenterCalAFPos}");
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x60F0, 2, (ushort)Condition.DecenterCalAFPos);
            Wait(100);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6020, 2, 0x01);
            checkStatus = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if(!checkStatus)
            {
                return (false, Final_MG_Offset);
            }
            Wait(200);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x617A, 2, 0x01);
            checkStatus = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if (!checkStatus)
            {
                return (false, Final_MG_Offset);
            }
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6020, 2, 0x07);
            checkStatus = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if (!checkStatus)
            {
                return  (false, Final_MG_Offset);
            }
            Wait(200);

            DrvIC.AFMove(ch, Condition.DecenterCalAFPos);
            Wait(200);
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x60F0, 2, (ushort)Condition.DecenterCalAFPos);
            DrvIC.OISMove(ch, 0, 0);
            Wait(100);
            res = Measure();
            xCenter = res.cx[0];
            yCenter = res.cy[0];
          //  AddLog(ch, $"X Center : {(res.cx[0]).ToString("F2")}, Y Center : {(res.cy[0]).ToString("F2")}");
            DrvIC.OISMove(ch, 0, 0);
            Wait(100);

            if(axis == 0)
            {
                AddLog(ch, $"X Decenter Calibration start >>>>>>>>>>>>>>>>>>>>>>");
                DrvIC.OISMove(ch, OISCalData.XhallMax, 0);
                Wait(200);
                res = Measure();
                X1 = (int)(res.cx[0] - xCenter);
                AddLog(ch, $"X_max_dis = {X1}");

                DrvIC.OISMove(ch, OISCalData.XhallMin, 0);
                Wait(200);
                res = Measure();
                X2 = (int)(res.cx[0] - xCenter);
                AddLog(ch, $"X_min_dis = {X2}");

                Initial_X_Decenter = (X1 + X2) / 2;
                AddLog(ch, $"Initial_X_Decenter = {Initial_X_Decenter}");

                if(Math.Abs(Initial_X_Decenter) < Condition.DecenterThr)
                {

                    AddLog(ch, $"Skip X Decenter Cal");
                    return (true, 0);
                }


                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6174, 2, (ushort)Condition.Decenter_X_MGOffset_Pos);
                Wait(20);
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6176, 2, 0);
                Wait(20);
                DrvIC.OISMove(ch, 0, 0);
                Wait(100);

                res = Measure();
                xCenter = res.cx[0];
                yCenter = res.cy[0];

                DrvIC.OISMove(ch, OISCalData.XhallMax, 0);
                Wait(200);
                res = Measure();
                X3 = (int)(res.cx[0] - xCenter);
                AddLog(ch, $"X_max_dis with {Condition.Decenter_X_MGOffset_Pos} = {X3}");

                DrvIC.OISMove(ch, OISCalData.XhallMin, 0);
                Wait(200);
                res = Measure();
                X4 = (int)(res.cx[0] - xCenter);
                AddLog(ch, $"X_min_dis with {Condition.Decenter_X_MGOffset_Pos} = {X4}");

                X_Decenter1 = (X3 + X4) / 2;
                AddLog(ch, $"MG offset {Condition.Decenter_X_MGOffset_Pos} Decenter = {X_Decenter1}");

                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6174, 2, (ushort)Condition.Decenter_X_MGOffset_Neg);
                Wait(20);
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6176, 2, 0);
                Wait(20);
                DrvIC.OISMove(ch, 0, 0);
                Wait(100);

                res = Measure();
                xCenter = res.cx[0];
                yCenter = res.cy[0];


                DrvIC.OISMove(ch, OISCalData.XhallMax, 0);
                Wait(200);
                res = Measure();
                X5 = (int)(res.cx[0] - xCenter);
                AddLog(ch, $"X_max_dis with {Condition.Decenter_X_MGOffset_Neg} = {X5}");

                DrvIC.OISMove(ch, OISCalData.XhallMin, 0);
                Wait(200);
                res = Measure();
                X6 = (int)(res.cx[0] - xCenter);
                AddLog(ch, $"X_min_dis with {Condition.Decenter_X_MGOffset_Neg} = {X6}");

                X_Decenter2 = (X5 + X6) / 2;
                AddLog(ch, $"MG offset {Condition.Decenter_X_MGOffset_Neg} Decenter = {X_Decenter2}");

                slope = (float)((X_Decenter2 - X_Decenter1) * ( 1.0 / ((double)Condition.Decenter_X_MGOffset_Pos - (double)Condition.Decenter_X_MGOffset_Neg) ) * -1);
                AddLog(ch, $"slope = {slope.ToString("F3")}");
                b = X_Decenter2 - (slope * Condition.Decenter_X_MGOffset_Neg);
                AddLog(ch, $"b = {b.ToString("F3")}");
                Final_MG_Offset = (int)(-b / slope);
                AddLog(ch, $"Calibrated MG offset = {Final_MG_Offset.ToString("F3")}");

                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6174, 2, (ushort)Final_MG_Offset);
                Wait(20);
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6176, 2, 0);
                Wait(20);

                AddLog(ch, $"X Decenter Calibration end >>>>>>>>>>>>>>>>>>>>>>");
            }
            else
            {
                AddLog(ch, $"Y Decenter Calibration start >>>>>>>>>>>>>>>>>>>>>>");

                DrvIC.OISMove(ch, 0, OISCalData.YhallMax);
                Wait(200);
                res = Measure();
                Y1 = (int)(res.cy[0] - yCenter);
                AddLog(ch, $"Y_max_dis = {Y1}");

                DrvIC.OISMove(ch, 0, OISCalData.YhallMin);
                Wait(200);
                res = Measure();
                Y2 = (int)(res.cy[0] - yCenter);
                AddLog(ch, $"Y_min_dis = {Y2}");

                Initial_Y_Decenter = (Y1 + Y2) / 2;
                AddLog(ch, $"Initial_Y_Decenter = {Initial_Y_Decenter}");

                if (Math.Abs(Initial_Y_Decenter) < Condition.DecenterThr)
                {

                    AddLog(ch, $"Skip Y Decenter Cal");
                    return (true, 0);
                }


                //Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6174, 2, 500);
                //Wait(20);
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6176, 2, (ushort)Condition.Decenter_Y_MGOffset_Pos);
                Wait(20);
                DrvIC.OISMove(ch, 0, 0);
                Wait(100);

                res = Measure();
                xCenter = res.cx[0];
                yCenter = res.cy[0];

                DrvIC.OISMove(ch, 0, OISCalData.YhallMax);
                Wait(200);
                res = Measure();
                Y3 = (int)(res.cy[0] - yCenter);
                AddLog(ch, $"Y_max_dis with {Condition.Decenter_Y_MGOffset_Pos} = {Y3}");

                DrvIC.OISMove(ch, 0, OISCalData.YhallMin);
                Wait(200);
                res = Measure();
                Y4 = (int)(res.cy[0] - yCenter);
                AddLog(ch, $"Y_min_dis with {Condition.Decenter_Y_MGOffset_Pos} = {Y4}");

                Y_Decenter1 = (Y3 + Y4) / 2;
                AddLog(ch, $"MG offset {Condition.Decenter_Y_MGOffset_Pos} Decenter = {Y_Decenter1}");

                //Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6174, 2, -500);
                //Wait(20);
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6176, 2, (ushort)Condition.Decenter_Y_MGOffset_Neg);
                Wait(20);
                DrvIC.OISMove(ch, 0, 0);
                Wait(100);

                res = Measure();
                xCenter = res.cx[0];
                yCenter = res.cy[0];

                DrvIC.OISMove(ch, 0, OISCalData.YhallMax);
                Wait(200);
                res = Measure();
                Y5 = (int)(res.cy[0] - yCenter);
                AddLog(ch, $"Y_max_dis with {Condition.Decenter_Y_MGOffset_Neg} = {Y5}");

                DrvIC.OISMove(ch, 0, OISCalData.YhallMin);
                Wait(200);
                res = Measure();
                Y6 = (int)(res.cy[0] - yCenter);
                AddLog(ch, $"Y_min_dis with {Condition.Decenter_Y_MGOffset_Neg} = {Y6}");

                Y_Decenter2 = (Y5 + Y6) / 2;
                AddLog(ch, $"MG offset {Condition.Decenter_Y_MGOffset_Neg} Decenter = {Y_Decenter2}");

                slope = (float)((Y_Decenter2 - Y_Decenter1) * (1.0 / ((double)Condition.Decenter_Y_MGOffset_Pos - (double)Condition.Decenter_Y_MGOffset_Neg)) * -1);
                AddLog(ch, $"slope = {slope.ToString("F3")}");
                b = Y_Decenter2 - (slope * Condition.Decenter_Y_MGOffset_Neg);
                AddLog(ch, $"b = {b.ToString("F3")}");
                Final_MG_Offset = (int)(-b / slope);
                AddLog(ch, $"Calibrated MG offset = {Final_MG_Offset.ToString("F3")}");

                //Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6174, 2, (ushort)Final_MG_Offset);
                //Wait(20);
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x6176, 2, (ushort)Final_MG_Offset);
                Wait(20);

                AddLog(ch, $"Y Decenter Calibration end >>>>>>>>>>>>>>>>>>>>>>");
            }

            LEDs_All_On(0, false);
            return (false, Final_MG_Offset);
        }



        bool AFLinComp(int ch, int startpos, int endpos, int step, int margin_start, int margin_end, int s_value, int e_value, int linear_spec)
        {
            LEDs_All_On(0, true);
            int NUM_COEF = 27;
            FindResult tmpres = new FindResult();
            float[] targPosi = new float[step + 1]; // Array for storing target position data
            float[] lensPosi = new float[step + 1]; // Array for storing lens position data
            int[] readHall = new int[step + 1];
            float[] refLensPosi = new float[step + 1];
            int valueStepsize = step - s_value - e_value;
            float[] valueLensPosi = new float[valueStepsize + 1];
            float refStepsize = 0, gap = 0, valueStep = 0, valuegap = 0;
            float max_gap = 0, max_valuegap = 0;
            int temp_table = startpos;
            int ignInf = 0;
            int ignMac = 0;
            int numLinCompData;
            int pVt, nVt;
            pVt = (Dln.Read2Byte(ch, DrvIC.AF_Addr, 0xC0, 1) >> 6) & 0x03FF;
            nVt = (Dln.Read2Byte(ch, DrvIC.AF_Addr, 0xC2, 1) >> 6) & 0x03FF;

            int[] linCoef = new int[NUM_COEF]; // Array for storing line compensation coefficients
            int pVtNew = 0;    // Recalculation "POSVT" after linearity compensation
            int nVtNew = 0;    // Recalculation "NEGVT" after linearity compensation
            float resError = 0;   // Variable for storing residual error after linearity compensation

            int result;
            int step_size = (endpos - startpos) / step;
            int LZValue, HallValue;
            float RefData = 0;
            AddLog(ch, $"AK7316 Linearity Compensation start");
            AddLog(ch, $"pVt : {pVt}");
            AddLog(ch, $"nVt : {nVt}");
            AddLog(ch, $"step size : {step_size}");

            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMove(ch, startpos); Wait(100);
            DrvIC.OISOnOff(ch, false); Wait(200);

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            for (int i = 0; i < NUM_COEF; i++) Dln.WriteByte(ch, DrvIC.AF_Addr, 0xC5 + i, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);

            AddLog(ch, $"Target\tReadHall\tPos");
            for (int i = 0; i <= step; i++)
            {
                targPosi[i] = temp_table;
                DrvIC.AFMove(ch, (int)targPosi[i]); Wait(50);
                readHall[i] = DrvIC.ReadAFHall(ch);
                tmpres = Measure();
                if (i != 0) lensPosi[i] = (float)tmpres.cz[0] - RefData;
                else { lensPosi[i] = 0; RefData = (float)tmpres.cz[0]; }

                temp_table += step_size;
                AddLog(ch, $"{targPosi[i]}\t{readHall[i]}\t{lensPosi[i].ToString("F2")}");
            }

            valueStep = (lensPosi[step - e_value] - lensPosi[s_value]) / (valueStepsize);
            valueLensPosi[0] = lensPosi[s_value];
            valueLensPosi[valueStepsize] = lensPosi[s_value + valueStepsize];

            AddLog(ch, "");
            AddLog(ch, "=== Linearity check ===");
            AddLog(ch, $"ValueStepSize = {valueStepsize}");
            AddLog(ch, $"ValueStep = {valueStep}");
            AddLog(ch, "=======================");
            AddLog(ch, $"{lensPosi[s_value].ToString("F3")}, {valueLensPosi[0].ToString("F3")}");

            for (int i = 1; i < valueStepsize; i++)
            {
                valueLensPosi[i] = valueLensPosi[i - 1] + valueStep;
                valuegap = valueLensPosi[i] - lensPosi[i + s_value];
                if (valuegap >= 0) { }
                else { valuegap *= -1; }
                AddLog(ch, $"{lensPosi[i + s_value].ToString("F3")}, {valueLensPosi[i].ToString("F3")}, {valuegap.ToString("F3")}");

                if (max_valuegap < valuegap) max_valuegap = valuegap;

            }
            AddLog(ch, $"{lensPosi[valueStepsize + s_value].ToString("F3")}, {valueLensPosi[valueStepsize].ToString("F3")}");
            AddLog(ch, $"max valuegap= {max_valuegap.ToString("F3")}");
            if (max_valuegap > linear_spec)
            {

                if (targPosi.Length == lensPosi.Length)
                {
                    AFLinCompCoef coef = new AFLinCompCoef();
                    numLinCompData = targPosi.Length;
                    AddLog(ch, $"numLinCompData = {numLinCompData}");

                    result = coef.LinCompMain(targPosi, lensPosi, numLinCompData, pVt, nVt, ignInf, ignMac, linCoef, ref resError);
                    if (result != 0)
                    {
                        AddLog(ch, $"Linearity Comp Fail");
                        LEDs_All_On(0, false);
                        return false;  // Error: please check return value of LinCompMain()
                    }

                }
                else
                {
                    AddLog(ch, $"Number of targetd ata and lens data is different");
                    LEDs_All_On(0, false);
                    return false;   // Error: Number of targetd ata and lens data is different.
                                    //return 1;	// Error: Number of targetd ata and lens data is different.
                }

                //// Result display example.
                //AddLog(ch, $"0Eh:{pVtNew.ToString("X2")}, 0Fh:{nVtNew.ToString("X2")}");
                //AddLog(ch, " 30h:%x, 31h:%x, 32h:%x, 33h:%x, 34h:%x, 35h:%x, 36h:%x, 37h:%x, 38h:%x, 39h:%x, 3Ah:%x, 3Bh:%x, 3Ch:%x, ResidualError:%x\n",
                // linCoef[0], linCoef[1], linCoef[2], linCoef[3], linCoef[4], linCoef[5], linCoef[6], linCoef[7], linCoef[8], linCoef[9], linCoef[10], linCoef[11], linCoef[12], resError);

                DrvIC.AFMove(ch, DrvIC.AF_MID_CODE); Wait(50);              // Lens Move Mid Code : 0x03 update -> positon store			

                // save coefficient
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
                //			AK7316_Write_byte(0x0E, (Byte)pVtNew); 
                //			AK7316_Write_byte(0x0F, (Byte)nVtNew); 
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xC5, 1, (byte)linCoef[0]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xC6, 1, (byte)linCoef[1]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xC7, 1, (byte)linCoef[2]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xC8, 1, (byte)linCoef[3]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xC9, 1, (byte)linCoef[4]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xCA, 1, (byte)linCoef[5]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xCB, 1, (byte)linCoef[6]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xCC, 1, (byte)linCoef[7]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xCD, 1, (byte)linCoef[8]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xCE, 1, (byte)linCoef[9]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xCF, 1, (byte)linCoef[10]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xD0, 1, (byte)linCoef[11]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xD1, 1, (byte)linCoef[12]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xD2, 1, (byte)linCoef[13]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xD3, 1, (byte)linCoef[14]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xD4, 1, (byte)linCoef[15]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xD5, 1, (byte)linCoef[16]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xD6, 1, (byte)linCoef[17]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xD7, 1, (byte)linCoef[18]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xD8, 1, (byte)linCoef[19]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xD9, 1, (byte)linCoef[20]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xDA, 1, (byte)linCoef[21]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xDB, 1, (byte)linCoef[22]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xDC, 1, (byte)linCoef[23]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xDD, 1, (byte)linCoef[24]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xDE, 1, (byte)linCoef[25]);
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xDF, 1, (byte)linCoef[26]);
                //----------------------------------------------------------------------------------------------

                DrvIC.AF_Memory_Update(ch, 4);    // 0xC4~DF store AK7316
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);  // normal mode
            }

            else
            {
                // Laser disable

                AddLog(ch, " Skip Linearity Compensation");
                LEDs_All_On(0, false);
                return false;
            }


            LEDs_All_On(0, false);
            DrvIC.OISOnOff(ch, true);
            DrvIC.OISOnOff(ch, false);
            Wait(50);
            return true;
        }
        void Act_CloseLoopAging(int ch, string testitem, int InspCnt)
        {
            CloseLoopAging(ch);
        }
        void AFPM(int ch, string testItem, int inspCnt)
        {
            int freq_val, freq_temp = 0, gain_temp, freq_PM = 0, old_freq;
            int[] before_after_zero_freq = new int[2];
            double gain_val = 0, phasemargin_val = 0, phase_temp, pre_pm = 0;
            double[] before_after_zero_gain = new double[2];
            double phase_min = 5000;
            double PM_2nd = 0;
            byte backup, flag_2nd = 0;
            byte freq_en;



            byte[] rbuf3 = new byte[3];

            int zero_range = 3;
            int StartFreq = Condition.iAFChirpFrom;
            int EndFreq = Condition.iAFChirpTo;
            int Step = Condition.iAFFRAstep;
            double amp = Condition.iAFAmplitude;
            int GainTh = Condition.PMAFGainTH;


            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x00);
            DrvIC.AFMove(ch, Condition.AfPosPM); // 가이드 받으면 적용 
            Wait(100);

            Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x00, 1, 0x01);
            Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x00, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x6F, 1, (byte)(DrvIC.AF_Addr << 1));

            AddLog(ch, $"[AF Phase Margin test(High Freq Start)]");
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x40);
            Wait(1);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);

            DrvIC.FRAModeEnable(ch);

            Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x52, 1, (ushort)((int)amp << 6));

            AddLog(ch, "--------------------------------------------");
            AddLog(ch, " Amp	Freq	Gain	P/M ");
            AddLog(ch, " [Dec]	[Hz]	[dB]	[deg] ");
            AddLog(ch, "--------------------------------------------");

            for (old_freq = freq_val = StartFreq; freq_val >= EndFreq; freq_val -= freq_temp)
            {
                Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x50, 1, (ushort)(freq_val << 1));
                Wait((int)(1000.0 / old_freq + 5000.0 / freq_val + 10));
                old_freq = freq_val;

                Dln.ReadArray(ch, DrvIC.FRA_Addr, 0x94, 1, rbuf3);
                gain_temp = (rbuf3[0] << 16) + (rbuf3[1] << 8) + rbuf3[2];
                gain_val = Math.Log10(((double)gain_temp / 65536)) * 20;

                phase_temp = (double)Dln.Read2Byte(ch, DrvIC.FRA_Addr, 0x98, 1);
                phase_temp /= 128;
                if (phase_temp > 256) phase_temp -= 512;
                phasemargin_val = 180 + phase_temp;
                if (phasemargin_val > 180) phasemargin_val -= 360;
                if (phasemargin_val < -180) phasemargin_val += 360;

                AddLog(ch, $"{amp}, {freq_val}, {gain_val.ToString("F2")}, {phasemargin_val.ToString("F2")}");
                if (phasemargin_val < phase_min) phase_min = phasemargin_val;

                if(gain_val > 0)
                {
                    phasemargin_val = ((gain_val * pre_pm) - (before_after_zero_gain[0] * phasemargin_val)) /
                       (gain_val - before_after_zero_gain[0]);
                    freq_PM = (int)(((gain_val * before_after_zero_freq[0]) - (before_after_zero_gain[0] *
                                     freq_val)) / (gain_val - before_after_zero_gain[0]));

                    before_after_zero_freq[1] = freq_val;
                    before_after_zero_gain[1] = gain_val;
                    break;
                }
                else
                {
                    before_after_zero_freq[0] = freq_val;
                    before_after_zero_gain[0] = gain_val;
                }

                if((Math.Abs(gain_val) < zero_range) && (flag_2nd !=1))
                {
                    PM_2nd = phasemargin_val;
                    flag_2nd = 1;
                }
                pre_pm = phasemargin_val;
                freq_temp = freq_val * Step / 100;

                if (freq_temp < 1) freq_temp = 1;
            }

            AddLog(ch, "--------------------------------------------");
            AddLog(ch, $"Zero Freq Before = {before_after_zero_freq[0]} Hz,  {before_after_zero_gain[0].ToString("F2")} dB");
            AddLog(ch, $"Zero Freq After  = {before_after_zero_freq[1]} Hz,  {before_after_zero_gain[1].ToString("F2")} dB");

            if(freq_val == StartFreq)
            {
                AddLog(ch, $"Error type1 : Gain over zero at 1st cycle");
                PassFails[ch].Results[(int)SpecItem.FRAAF_PhaseMargin].Val = 1; //plus Gain
                ShowDataResults(ch, (int)SpecItem.FRAAF_PhaseMargin, (int)SpecItem.FRAAF_PhaseMargin, InspType.Normal, new double[] { });
                DrvIC.FRAModeDisable(ch);
                return;
            }
            if((freq_val <= EndFreq) && (gain_val <= 0))
            {
                AddLog(ch, $"Error type4 : No cross over point during period");
                PassFails[ch].Results[(int)SpecItem.FRAAF_PhaseMargin].Val = 4; //No cross
                ShowDataResults(ch, (int)SpecItem.FRAAF_PhaseMargin, (int)SpecItem.FRAAF_PhaseMargin, InspType.Normal, new double[] { });
                DrvIC.FRAModeDisable(ch);
                return;
            }
            if(Math.Abs(gain_val - before_after_zero_gain[1]) > GainTh)
            {
                AddLog(ch, $"Error type2 : gain is changed drastically over {GainTh}");
                PassFails[ch].Results[(int)SpecItem.FRAAF_PhaseMargin].Val = 2; //No cross
                ShowDataResults(ch, (int)SpecItem.FRAAF_PhaseMargin, (int)SpecItem.FRAAF_PhaseMargin, InspType.Normal, new double[] { });
                DrvIC.FRAModeDisable(ch);
                return;

            }
            AddLog(ch, "\nUse Linear Interpolation");
            AddLog(ch, "--------------------------------------------------");
            AddLog(ch, $" {amp} amp, {freq_PM} Hz, {gain_val.ToString("F2")} dB, {phasemargin_val.ToString("F0")} deg");
            AddLog(ch, "--------------------------------------------------");
            AddLog(ch, $"Phase at -3dB : {PM_2nd.ToString("F0")} deg");

            DrvIC.FRAModeDisable(ch);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            DrvIC.AF_ICReset(ch);

            PassFails[ch].Results[(int)SpecItem.FRAAF_PhaseMargin].Val = phasemargin_val;
            ShowDataResults(ch, (int)SpecItem.FRAAF_PhaseMargin, (int)SpecItem.FRAAF_PhaseMargin, InspType.Normal, new double[] { });
         

        }
        void AFGM(int ch, string testItem, int inspCnt)
        {
            int freq_val, freq_temp = 0, gain_temp, freq_GM = 0, old_freq;
            int[] before_after_zero_freq = new int[2];
            double gain_val = 0, phasemargin_val = 0, gainmargin_val = 0, phase_temp, pre_gm = 0;
            double[] before_after_zero_phase = new double[2];

            byte backup;
            float result;
            bool test_continue = true;

            byte[] rbuf3 = new byte[3];

            
            int StartFreq = Condition.AFGMStartFreq;
            int EndFreq = Condition.AFGMEndFreq;
            int Step = Condition.AFGMStep;
            double amp = Condition.AFGMamp;
            int GainTh = Condition.PMAFGainTH;


            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x00);
            DrvIC.AFMove(ch, Condition.AFPosGM); // 가이드 받으면 적용 
            Wait(100);

            Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x00, 1, 0x01);
            Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x00, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x6F, 1, (byte)(DrvIC.AF_Addr << 1));

            AddLog(ch, $"[AF Gain Margin test]");
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x40);
            Wait(1);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);

            DrvIC.FRAModeEnable(ch);

            Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x52, 1, (ushort)((int)amp << 6));


            freq_val = StartFreq;
            freq_temp = freq_val * Step / 100;
            freq_val += freq_temp;
            Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x50, 1, (ushort)(freq_val << 1));
            Wait(30000 / freq_val + 10);
            Wait(100);
            Dln.ReadArray(ch, DrvIC.FRA_Addr, 0x94, 1, rbuf3);
            gain_temp = (rbuf3[0] << 16) + (rbuf3[1] << 8) + rbuf3[2];
            gain_val = Math.Log10(((double)gain_temp / 65536)) * 20;


            phase_temp = (double)Dln.Read2Byte(ch, DrvIC.FRA_Addr, 0x98, 1);
            phase_temp /= 128;
            if (phase_temp > 256) phase_temp -= 512;
            phasemargin_val = 180 + phase_temp;
            if (phasemargin_val > 180) phasemargin_val -= 360;
            if (phasemargin_val < -180) phasemargin_val += 360;

            AddLog(ch, "skip high freq for aging\n");
            AddLog(ch, $"{amp},{freq_val} Hz, {gain_val.ToString("F2")} dB, {phasemargin_val.ToString("F0")} deg\n\n");


            AddLog(ch, "--------------------------------------------");
            AddLog(ch, " Amp	Freq	Gain	P/M ");
            AddLog(ch, " [Dec]	[Hz]	[dB]	[deg] ");
            AddLog(ch, "--------------------------------------------");

            for (old_freq = freq_val = StartFreq; freq_val >= EndFreq; freq_val -= freq_temp)
            {
                Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x50, 1, (ushort)(freq_val << 1));
                Wait((int)(1000.0 / old_freq + 5000.0 / freq_val + 10));
                old_freq = freq_val;

                Dln.ReadArray(ch, DrvIC.FRA_Addr, 0x94, 1, rbuf3);
                gain_temp = (rbuf3[0] << 16) + (rbuf3[1] << 8) + rbuf3[2];
                gain_val = Math.Log10(((double)gain_temp / 65536)) * 20;

                phase_temp = (double)Dln.Read2Byte(ch, DrvIC.FRA_Addr, 0x98, 1);
                phase_temp /= 128;
                if (phase_temp > 256) phase_temp -= 512;
                phasemargin_val = 180 + phase_temp;
                if (phasemargin_val > 180) phasemargin_val -= 360;
                if (phasemargin_val < -180) phasemargin_val += 360;

                AddLog(ch, $"{amp}, {freq_val}, {gain_val.ToString("F2")}, {phasemargin_val.ToString("F2")}");
               
                if (phasemargin_val > 0)
                {
                    gain_val = ((phasemargin_val * pre_gm) - (before_after_zero_phase[0] * gain_val))  / (phasemargin_val - before_after_zero_phase[0]);
                    freq_GM = (int)(((phasemargin_val * before_after_zero_freq[0]) - (before_after_zero_phase[0] *
                                     freq_val)) / (phasemargin_val - before_after_zero_phase[0]));

                    before_after_zero_freq[1] = freq_val;
                    before_after_zero_phase[1] = phasemargin_val;
                    break;
                }
                else
                {
                    before_after_zero_freq[0] = freq_val;
                    before_after_zero_phase[0] = phasemargin_val;
                }

             
                pre_gm = gain_val;
                freq_temp = freq_val * Step / 100;

                if (freq_temp < 1) freq_temp = 1;
            }

            AddLog(ch, "--------------------------------------------");
            AddLog(ch, $"Zero Freq Before = {before_after_zero_freq[0]} Hz,  {before_after_zero_phase[0].ToString("F2")} dB");
            AddLog(ch, $"Zero Freq After  = {before_after_zero_freq[1]} Hz,  {before_after_zero_phase[1].ToString("F2")} dB");

            if (test_continue && (freq_val == StartFreq))
            {
                AddLog(ch, $"Error type1 : Phase over zero at 1st cycle");
                PassFails[ch].Results[(int)SpecItem.FRAAF_GainMargin].Val = 1; //plus Gain
                ShowDataResults(ch, (int)SpecItem.FRAAF_GainMargin, (int)SpecItem.FRAAF_GainMargin, InspType.Normal, new double[] { });
                DrvIC.FRAModeDisable(ch);
                test_continue = false;
                return;
            }
            if(test_continue && (freq_val <= EndFreq) && (phasemargin_val <= 0))
            {
                AddLog(ch, $"Error type4 : No cross over point during period");
                PassFails[ch].Results[(int)SpecItem.FRAAF_GainMargin].Val = 4; //plus Gain
                ShowDataResults(ch, (int)SpecItem.FRAAF_GainMargin, (int)SpecItem.FRAAF_GainMargin, InspType.Normal, new double[] { });
                DrvIC.FRAModeDisable(ch);
                test_continue = false;
                return;
            }
          
            AddLog(ch, "\nUse Linear Interpolation");
            AddLog(ch, "--------------------------------------------------");
            AddLog(ch, $" {amp} amp, {freq_GM} Hz, {gain_val.ToString("F2")} dB, {phasemargin_val.ToString("F0")} deg");
            AddLog(ch, "--------------------------------------------------");

            PassFails[ch].Results[(int)SpecItem.FRAAF_GainMargin].Val = (float)(-1 * gain_val);
            ShowDataResults(ch, (int)SpecItem.FRAAF_GainMargin, (int)SpecItem.FRAAF_GainMargin, InspType.Normal, new double[] { });
           
            DrvIC.FRAModeDisable(ch);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            DrvIC.AF_ICReset(ch);

        

        }

        void AFLoopGain(int ch, string testItem, int inspCnt)
        {

            int Freq = Condition.AFLoopGainFreq;
            double amp = Condition.AFLoopGainAmp;
            byte[] rbuf3 = new byte[3];
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x00);
            DrvIC.AFMove(ch, Condition.AFLoopGainPos); // 가이드 받으면 적용 
            Wait(100);
            AddLog(ch, $"AF Position : {DrvIC.ReadAFHall(ch)}");
            Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x00, 1, 0x01);
            Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x00, 1, 0x00);
            Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x6F, 1, (byte)(DrvIC.AF_Addr << 1));

            AddLog(ch, $"[AF LoopGain test]");
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x40);
            Wait(1);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);

            DrvIC.FRAModeEnable(ch);

            Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x52, 1, (ushort)((int)amp << 6));


         
            Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x50, 1, (ushort)(Freq << 1));
            Wait(30000 / Freq + 10);
            Wait(100);
            Dln.ReadArray(ch, DrvIC.FRA_Addr, 0x94, 1, rbuf3);
            double gain_temp = (rbuf3[0] << 16) + (rbuf3[1] << 8) + rbuf3[2];
         
            double gain_val = Math.Log10(((double)gain_temp / 65536)) * 20;
            AddLog(ch, $"{amp},{Freq} Hz, {gain_val.ToString("F2")} dB\n\n");

            PassFails[ch].Results[(int)SpecItem.AF_LoopGain].Val = (float)(gain_val);
            ShowDataResults(ch, (int)SpecItem.AF_LoopGain, (int)SpecItem.AF_LoopGain, InspType.Normal, new double[] { });
         

            DrvIC.FRAModeDisable(ch);
            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            DrvIC.AF_ICReset(ch);



        }

        

        void CloseLoopAging(int ch)
        {
            int AFMin = Condition.CLAgingAFMin, AFMax = Condition.CLAgingAFMax, OISMin = Condition.CLAgingOISMin, OISMax = Condition.CLAgingOISMax, count = Condition.CLAgingCount;
            int delay = 1000 / Condition.CLAgingFreq / 2;
            int[] check_hall = new int[3];

            AddLog(ch, "<<<  XYZ Aging Start  >>>");
            AddLog(ch, $"Frequency : {Condition.CLAgingFreq}");
            AddLog(ch, $"Aging Count : {count}");
            AddLog(ch, $"AF Range : {AFMin} - {AFMax}");
            AddLog(ch, $"OIS Range : {OISMin} - {OISMax}");

            DrvIC.AFOnOff(ch, true);
            DrvIC.OISOnOff(ch, true);
            DrvIC.SetManualDrvModeXY(ch, 0, 0);
            DrvIC.AFMove(ch, AFCenter);
          //
         

            for (int i = 0; i < count; i++)
            {
                DrvIC.AFMove(ch, AFMin);
                DrvIC.OISMove(ch, OISMin, OISMin);
             
                Wait(delay);
                DrvIC.AFMove(ch, AFMax);
                DrvIC.OISMove(ch, OISMax, OISMax);
         
            }


            DrvIC.AFMove(ch, AFCenter);
            DrvIC.OISMove(ch, OISXCenter, OISYCenter);
        
            Wait(delay);
            DrvIC.AFOnOff(ch, false);
            DrvIC.OISOnOff(ch, false);
            AddLog(ch, "<<<  XYZ Aging End  >>>");

            PassFails[0].Results[(int)SpecItem.XYZAging].Val = 1;
            ShowDataResults(ch, (int)SpecItem.XYZAging, (int)SpecItem.XYZAging, InspType.Normal, new double[] { });

        }       
        void Act_FindBestAFPosition(int ch, string testitem, int InspCnt, bool IsTwice)
        {

            //int[] step = new int[9] { 0, 511, 1023, 1535, 2047, 2559, 3071, 3585, 4095 };
            //int[] hallX = new int[9];
            //int[] hallY = new int[9];

            //Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x02, new byte[] { 0x00 });
            //DrvIC.Move(ch, "AF", 200);
            //Wait(50);
            //DrvIC.Move(ch, "AF", 0);

            //Dln.WriteArray(ch, DrvIC.XSlaveAddr, 0xAE, new byte[] { 0x3B });
            //Dln.WriteArray(ch, DrvIC.Y1SlaveAddr, 0xAE, new byte[] { 0x3B });

            ////중간 셋팅값 확인 

            ////
            //Dln.WriteArray(ch, DrvIC.XSlaveAddr, 0x02, new byte[] { 0x40 });
            //Dln.WriteArray(ch, DrvIC.Y1SlaveAddr, 0x02, new byte[] { 0x40 });

            //Wait(100);

            //for (int i = 0; i < 9; i++)
            //{
            //    int[] tmphallX = new int[6];
            //    int[] tmphallY = new int[6];
            //    DrvIC.Move(ch, "AF", step[i]);
            //    Wait(100);
            //    for (int j = 0; j < 6; j++)
            //    {
            //        tmphallX[j] = DrvIC.ReadHall(ch, "X");
            //        tmphallY[j] = DrvIC.ReadHall(ch, "Y");
            //        hallX[i] += tmphallX[j];
            //        hallY[i] += tmphallY[j];
            //    }
            //    hallX[i] /= 6;
            //    hallY[i] /= 6;

            //    AddLog(ch, $"Pos = {step[i]}, DataX[{i}] = {hallX[i]}, DataY[{i}] = {hallY[i]}");
            //}
            //int xMin = hallX.Min(); int xMax = hallX.Max();
            //int yMin = hallY.Min(); int yMax = hallY.Max();
            //int xCenter = (xMin + xMax) / 2;
            //int yCenter = (yMin + yMax) / 2;
            //int xMinIndex = 0; int yMinIndex = 0;
            //int xMaxIndex = 0; int yMaxIndex = 0;
            //bool XMinFind = false; bool YMinFind = false;
            //bool XMaxFind = false; bool YMaxFind = false;
            //int xBestPos = 0; int yBestPos = 0;
            //for (int i = 0; i < 9; i++)
            //{
            //    if (xMin == hallX[i] && !XMinFind) { XMinFind = true; xMinIndex = i; }
            //    if (xMax == hallX[i] && !XMaxFind) { XMaxFind = true; xMaxIndex = i; }
            //    if (yMin == hallY[i] && !YMinFind) { YMinFind = true; yMinIndex = i; }
            //    if (yMax == hallY[i] && !YMaxFind) { YMaxFind = true; yMaxIndex = i; }
            //}
            //int startXIndex = 0; int endXIndex = 0; int startYIndex = 0; int endYIndex = 0;
            //if (xMinIndex > xMaxIndex)
            //{
            //    startXIndex = xMaxIndex;
            //    endXIndex = xMinIndex;
            //}
            //else
            //{
            //    startXIndex = xMinIndex;
            //    endXIndex = xMaxIndex;
            //}
            //if (yMinIndex > yMaxIndex)
            //{
            //    startYIndex = yMaxIndex;
            //    endYIndex = yMinIndex;
            //}
            //else
            //{
            //    startYIndex = yMinIndex;
            //    endYIndex = yMaxIndex;
            //}
            //string s = $"[MAX/MIN Index] 0, start:{startXIndex}, end:{endXIndex}\r\n" +
            //           $"[MAX/MIN Index] 1, start:{startYIndex}, end:{endYIndex}\r\n" +
            //           $"X Min : {xMin}, X Max : {xMax} ({xMax - xMin})\r\n" +
            //           $"Y Min : {yMin}, Y Max : {yMax} ({yMax - yMin})\r\n" +
            //           $"X Center :{xCenter}, Y Center : {yCenter}\r\n";
            //AddLog(ch, s);

            //for (int i = startXIndex; i <= endXIndex; i++)
            //{
            //    if (i == 0) continue;
            //    if (hallX[i - 1] <= xCenter && hallX[i] >= xCenter || hallX[i - 1] >= xCenter && hallX[i] <= xCenter)
            //    {

            //        xBestPos = (int)(step[i - 1] + (step[i] - step[i - 1]) * (xCenter - hallX[i - 1]) / (hallX[i] - hallX[i - 1]));


            //        break;
            //    }
            //}
            //for (int i = startYIndex; i <= endYIndex; i++)
            //{
            //    if (i == 0) continue;
            //    if (hallY[i - 1] <= yCenter && hallY[i] >= yCenter || hallY[i - 1] >= yCenter && hallY[i] <= yCenter)
            //    {
            //        yBestPos = (int)(step[i - 1] + (step[i] - step[i - 1]) * (yCenter - hallY[i - 1]) / (hallY[i] - hallY[i - 1]));

            //        break;
            //    }
            //}
            //AddLog(ch, $"X_AF : {xBestPos}, Y_AF : {yBestPos}");
            //if (xMax - xMin > yMax - yMin)
            //    BestAFPos = xBestPos;
            //else BestAFPos = yBestPos;
            //AddLog(ch, $"Chosen Best AF : {BestAFPos}");
        }
        public void ServoDecenter(int ch, string name, int InspCnt)
        {
            AddLog(ch, "<<<  OIS XY Servo Decenter Start  >>>");


            FindResult[] fXY = new FindResult[2] { new FindResult(), new FindResult() };
          

            LEDs_All_On(0, true);


            DrvIC.AFMove(ch, Condition.ServoDecenterAFPos);
            Wait(300);
            AddLog(ch, $"AF Position : {DrvIC.ReadAFHall(ch)}");

            DrvIC.OISOnOff(ch, true);

            DrvIC.SetManualDrvModeXY(ch, 0, 0);
       
            Wait(Condition.ServoDecenterDelay);
            fXY[0] = Measure();

            DrvIC.OISOnOff(ch, false);
            Wait(Condition.ServoDecenterDelay);

            fXY[1] = Measure();
            PassFails[0].Results[(int)SpecItem.x_ServoDecenter].Val = fXY[1].cx[0] - fXY[0].cx[0];
            PassFails[0].Results[(int)SpecItem.y_ServoDecenter].Val = fXY[0].cy[0] - fXY[1].cy[0];
            ShowDataResults(0, (int)SpecItem.x_ServoDecenter, (int)SpecItem.y_ServoDecenter, InspType.Normal, new double[] { });
            AddLog(ch, $"Decenter X = {(fXY[1].cx[0] - fXY[0].cx[0]).ToString("F2")}");
            AddLog(ch, $"Decenter Y = {(fXY[0].cy[0] - fXY[1].cy[0]).ToString("F2")}");
            AddLog(ch, "<<<  OIS XY Servo Decenter End  >>>");
            AddLog(ch, "");
            
            LEDs_All_On(0, false);
            

        }
        void AFPID_Verify(int ch, string testItem, int InspCnt)
        {
          
            bool res = true;

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
            DrvIC.AF_Memory_Update(ch, 5);

            //Dln.ReadArray(ch, DrvIC.AFSlaveAddr, 0x03, rbuf);
            //int afid = rbuf[0];
            //if (afid != 0x1E)
            //{
            //    AddLog(ch, $"Error, AF IC is not AK7314, 0x{afid.ToString("X2")}");
            //    res = false;
            //    PassFails[ch].Results[(int)SpecItem.AFPIDVerifyRes].Val = 1;
            //    ShowDataResults(ch, (int)SpecItem.AFPIDVerifyRes, (int)SpecItem.AFPIDVerifyRes, InspType.Normal, new double[] { });
            //    return;
            //}
            for (int i = 0; i < IC_SETTING_AF.Length; i++)
            {
                if (IC_SETTING_AF_REG[i] == 0x0C) continue;
                byte rbuf = Dln.ReadByte(ch, DrvIC.AF_Addr, IC_SETTING_AF_REG[i], 1);
                if (IC_SETTING_AF[i] != rbuf)
                {
                    AddLog(ch, $"Addr. : 0x{IC_SETTING_AF_REG[i].ToString("X2")}, rdata : 0x{rbuf.ToString("X2")}, wdata : 0x{IC_SETTING_AF[i].ToString("X2")}");
                    AddLog(ch, "AF PID Verify Fail");
                    PassFails[ch].Results[(int)SpecItem.AFPIDVerifyRes].Val = 2;
                    ShowDataResults(ch, (int)SpecItem.AFPIDVerifyRes, (int)SpecItem.AFPIDVerifyRes, InspType.Normal, new double[] { });
                    return;
                }
            }
            for (int i = 0; i < IC_DATA_AF.Length; i++)
            {
                if (IC_DATA_AF_REG[i] == 0x19) continue;
             
                byte rbuf = Dln.ReadByte(ch, DrvIC.AF_Addr, IC_DATA_AF_REG[i], 1);
                if (IC_DATA_AF[i] != rbuf)
                {
                    AddLog(ch, $"Addr. : 0x{IC_DATA_AF_REG[i].ToString("X2")}, rdata : 0x{rbuf.ToString("X2")}, wdata : 0x{IC_DATA_AF[i].ToString("X2")}");
                    AddLog(ch, "AF PID Verify Fail");
                    PassFails[ch].Results[(int)SpecItem.AFPIDVerifyRes].Val = 3;
                    ShowDataResults(ch, (int)SpecItem.AFPIDVerifyRes, (int)SpecItem.AFPIDVerifyRes, InspType.Normal, new double[] { });
                    return;
                }
            }
            PassFails[ch].Results[(int)SpecItem.AFPIDVerifyRes].Val = 0;
            ShowDataResults(ch, (int)SpecItem.AFPIDVerifyRes, (int)SpecItem.AFPIDVerifyRes, InspType.Normal, new double[] { });
        }
        void WriteUserMem(int ch, int res)
        {
            try
            {

                if(!Option.BarcodeUse)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        STATIC.ActID_Memory[i] = Dln.ReadByte(ch, DrvIC.AF_Addr, 0xF5 + i, 1);
                    }
                }
                var now = STATIC.LogDate;
                var year = now.Year - 2000;
                var month = now.Month;
                var day = now.Day;
                var hour = now.Hour;
                var minute = now.Minute;
                var second = now.Second;
         
                //AF Mem
                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);

                byte[] AFWriteData = new byte[16];
                AFWriteData[0] = (byte)res;
                AFWriteData[1] = (byte)Math.Abs((AFRatedMinMax[1] - AFRatedMinMax[0]) / 4);
                AFWriteData[2] = (byte)Math.Abs((AFRatedMinMax[2] - AFRatedMinMax[0]) / 4);
                AFWriteData[3] = (byte)((int)PassFails[ch].Results[(int)SpecItem.AF_Ratedstroke].Val >> 8);
                AFWriteData[4] = (byte)((int)PassFails[ch].Results[(int)SpecItem.AF_Ratedstroke].Val);
                AFWriteData[10] = (byte)(PassFails[ch].Results[(int)SpecItem.AF_Tilt].Val * 10);
                AFWriteData[11] = AFPIDVersion;
                AFWriteData[12] = (byte)(PassFails[ch].Results[(int)SpecItem.OISX_Ratedstroke].Val / 4);
                AFWriteData[13] = (byte)(PassFails[ch].Results[(int)SpecItem.OISY_Ratedstroke].Val / 4);
                AFWriteData[15] = (byte)(PassFails[ch].Results[(int)SpecItem.AF_Linearity].Val * 10);

                for (int i = 0; i < AFWriteData.Length; i++)
                {
                    if (i == 5 || i == 6 || i == 7 || i == 8 || i == 9 || i == 14) continue;
                    Dln.WriteByte(ch, DrvIC.AF_Addr, 0xF0 + i, 1, AFWriteData[i]);
                    Wait(30);
                }

             
                for (int i = 0; i < STATIC.ActID_Memory.Length; i++)
                {
                    Dln.WriteByte(ch, DrvIC.AF_Addr, 0xF5 + i, 1, STATIC.ActID_Memory[i]);
                    Wait(30);
                }

                Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);

                Dln.PowerSequence(ch);
                DrvIC.AF_ICReset(ch);
              
              
             
                byte[] afCheckData = new byte[AFWriteData.Length];

                AddLog(ch, "AF Nvm Data Check");
                for (int i = 0; i < afCheckData.Length; i++)
                {
                    if (i == 5 || i == 6 || i == 7 || i == 8 || i == 9 || i == 14) continue;
                    byte rdata = Dln.ReadByte(ch, DrvIC.AF_Addr, 0xF0 + i, 1);
                    AddLog(ch, $"Addr : 0x{(0xF0 + i).ToString("X2")}, WData : 0x{AFWriteData[i].ToString("X2")}, RData : 0x{rdata.ToString("X2")}");
                    if (AFWriteData[i] != rdata)
                    {
                        if (PassFails[ch].FirstFailIndex == 0)
                        {
                            AddLog(ch, "NVM Verify NG");
                            PassFails[ch].Results[(int)SpecItem.AFPIDVerifyRes].Val = 1;
                            ShowDataResults(ch, (int)SpecItem.AFPIDVerifyRes, (int)SpecItem.AFPIDVerifyRes, InspType.Normal, new double[] { });

                        }

                    }
                }

                for (int i = 0; i < STATIC.ActID_Memory.Length; i++)
                {
                  
                    byte rdata = Dln.ReadByte(ch, DrvIC.AF_Addr, 0xF5 + i, 1);
                    AddLog(ch, $"Addr : 0x{(0xF5 + i).ToString("X2")}, WData : 0x{STATIC.ActID_Memory[i].ToString("X2")}, RData : 0x{rdata.ToString("X2")}");
                    if (STATIC.ActID_Memory[i] != rdata)
                    {
                        if (PassFails[ch].FirstFailIndex == 0)
                        {
                            AddLog(ch, "NVM Verify NG");
                            PassFails[ch].Results[(int)SpecItem.AFPIDVerifyRes].Val = 1;
                            ShowDataResults(ch, (int)SpecItem.AFPIDVerifyRes, (int)SpecItem.AFPIDVerifyRes, InspType.Normal, new double[] { });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Form f = Application.OpenForms["F_Main"];
                if (f != null)
                {
                    if (f.InvokeRequired)
                    {
                        f.BeginInvoke(new Action(() =>
                            MessageBox.Show(f, ex.ToString(), "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }
                    else
                    {
                        MessageBox.Show(f, ex.ToString(), "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // 메인폼을 못 찾았을 때 (owner 없이 표시)
                    MessageBox.Show(ex.ToString(), "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }



                if (m_ChannelOn[ch] && PassFails[0].FirstFailIndex == 0)
                {
                    m_ChannelOn[ch] = false;
                    PassFails[0].FirstFailIndex = -999;
                    PassFails[0].FirstFail = "Check UserMem Setting";
                }

            }
        }
        void AF_OIS_Xtalk_Verify(int ch, string testItem, int inspCnt)
        {
            LEDs_All_On(0, true);
            FindResult res = new FindResult();

            string dataDir = STATIC.CreateDateDir();
            dataDir += $"OIS XtalkVerifyData\\";
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            string timeDir = $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s";

            List<string> arr = new List<string>();
            string path = string.Format(dataDir + "{0}_{1}_{2}_{3}.csv", $"XtalkVerifyData", m_StrIndex[ch], inspCnt + 1, timeDir);


            //type A
            short[] AFVerifyTarget = new short[9]
            {
                425, 828, 1231, 1634, 2037, 2440, 2843, 3246, 3648
            };

            short[] AFVerifyTargetTypeB = new short[3]
            {
                (short)Condition.Verify_AFOIS_Xtalk_inf, 2048, (short)Condition.Verify_AFOIS_Xtalk_mac
            };

            double[] VerifyOnDataX = new double[9];
            double[] VerifyOnDataY = new double[9];

            List<int> InspPointX = new List<int>();
            List<int> InspPointY = new List<int>();

            List<int> codetypeB = new List<int>();
            List<double> XvalTypeB = new List<double>();
            List<double> YvalTypeB = new List<double>();

            DrvIC.AFOnOff(ch, true);
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x614A, 2, 0x01);
            DrvIC.OISOnOff(ch, true);
            DrvIC.SetManualDrvModeXY(ch, 0, 0);

            for (int i = 0; i < 9; i++)
            {
                DrvIC.AFMove(ch, AFVerifyTarget[i]);
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x60F0, 2, AFVerifyTarget[i]);
                Wait(100);
                res = Measure();
                VerifyOnDataX[i] = res.cx[0];
                VerifyOnDataY[i] = res.cy[0];
            }

            double topX = 0;
            double topY = AFOISVerifyYPOSOIS[0];
            double btmX = 0;
            double btmY = AFOISVerifyYPOSOIS[1];
            double leftX = AFOISVerifyXPOSOIS[1];
            double leftY = 0;
            double rightX = AFOISVerifyXPOSOIS[0];
            double rightY = 0;

            double centerX = (leftX + rightX) / 2.0;
            double centerY = (topY + btmY) / 2.0;
            double radius = (rightX - leftX) / 2.0;

            for (int i = 0; i < 360; i += 15)
            {
                double rad = i * Math.PI / 180.0;
                double x = centerX + radius * Math.Cos(rad);
                double y = centerY + radius * Math.Sin(rad);
                InspPointX.Add((int)Math.Round(x));
                InspPointY.Add((int)Math.Round(y));
            }

            for (int i = 0; i < 3; i++)
            {
                DrvIC.AFMove(ch, AFVerifyTargetTypeB[i]);
                for (int j = 0; j < InspPointX.Count; j++)
                {
                    DrvIC.OISMove(ch, InspPointX[j], InspPointY[j]);
                    Wait(120);
                    res = Measure();
                    codetypeB.Add(AFVerifyTargetTypeB[i]);
                    XvalTypeB.Add(res.cx[0]);
                    YvalTypeB.Add(res.cy[0]);
                }
                
            }


            LEDs_All_On(0, false);
            if (Option.SaveRawData)
            {
                arr.Add($"Type A");
                arr.Add($"AF Code, Dis_X, Dis_Y");
                for (int i = 0; i < 9; i++)
                {
                    if (i == 0) arr.Add($"{AFVerifyTarget[i]}, {0}, {0}");
                    else arr.Add($"{AFVerifyTarget[i]}, {(VerifyOnDataX[i] - VerifyOnDataX[0]).ToString("F3")}, {(VerifyOnDataY[i] - VerifyOnDataY[0]).ToString("F3")}");
                }

                arr.Add("\r\n");
                arr.Add($"Type B");
                arr.Add($"AF Code, Dis_X, Dis_Y");
                for (int i = 0; i < codetypeB.Count; i++)
                {
                    arr.Add($"{codetypeB[i]}, {XvalTypeB[i].ToString("F3")}, {YvalTypeB[i].ToString("F3")}");
                }

           
                if (path != "") STATIC.SetTextLine(path, arr);
            }

        }
        void AF_OIS_Xtalk_Calibration(int ch, string testItem, int inspCnt)
        {
            LEDs_All_On(0, true);
            FindResult MeasRes = new FindResult();
            short AFTargetRef = 425;
            short[] hallX = new short[9];
            short[] hallY = new short[9];

            short[] xTalkDataX = new short[10];
            short[] xTalkDataY = new short[10];

            short[] AFTarget = new short[9] 
            {
                425, 828, 1231, 1634, 2037, 2440, 2843, 3246, 3648
            };
            short[] AFVerifyTarget = new short[9]
            {
               425, 828, 1231, 1634, 2037, 2440, 2843, 3246, 3648
            };

            double[] VerifyOffDataX = new double[9];
            double[] VerifyOffDataY = new double[9];
            double[] VerifyOnDataX = new double[9];
            double[] VerifyOnDataY = new double[9];
            double[] VerifyDiffX = new double[9];
            double[] VerifyDiffY = new double[9];
            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMove(ch, AFTargetRef); // Guide
            DrvIC.OISOnOff(ch, true);
            Wait(100);
            bool status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if(!status)
            {
                LEDs_All_On(0, false);
                PassFails[ch].Results[(int)SpecItem.xTaklMaxDiff].Val = 99999;
                ShowDataResults(ch, (int)SpecItem.xTaklMaxDiff, (int)SpecItem.xTaklMaxDiff, InspType.OnlyMax, new double[] { });
                return;
            }
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x614A, 2, 0x00);
            DrvIC.AFMove(ch, AFTargetRef); // guide

            DrvIC.OISOnOff(ch, false);
            Wait(20);
            short hall_x_ref = DrvIC.ReadOISHall(ch, 0, 1);
            short hall_y_ref = DrvIC.ReadOISHall(ch, 1, 1);
            for (int i = 0; i < 9; i++)
            {
                DrvIC.AFMove(ch, AFTarget[i]);
                Wait(200);
                hallX[i] = DrvIC.ReadOISHall(ch, 0, 1);
                hallY[i] = DrvIC.ReadOISHall(ch, 1, 1);

                xTalkDataX[i] = (short)(hall_x_ref - hallX[i]);
                xTalkDataY[i] = (short)(hall_y_ref - hallY[i]);
                AddLog(ch, $"{i + 1}({AFTarget[i]}), ReadhallX : {hallX[i]}, ReadhallY : {hallY[i]}");
            }
            xTalkDataX[9] = 0; xTalkDataY[9] = 0;
            AddLog(ch, "");
            for (int i = 0; i < 10; i++)
            {
                AddLog(ch, $"{i + 1}, XtalkData X : {xTalkDataX[i]}, XtalkData Y : {xTalkDataY[i]}");
            }

            int RefAddr = 0x10250CE1;
            for (int i = 0; i < 5; i++)
            {
                Dln.Write4Byte(ch, DrvIC.OIS_Addr, 0x6080, 2, RefAddr + (i * 0x400));

                uint wData = (uint)((xTalkDataX[(i * 2) + 1] << 16) + xTalkDataX[i * 2]);
                Dln.Write4Byte(ch, DrvIC.OIS_Addr, 0x6084, 2, wData);
                status = DrvIC.OIS_StausCheck(ch, 0x01, 0x01);
                if(!status)
                {
                    LEDs_All_On(0, false);
                    PassFails[ch].Results[(int)SpecItem.xTaklMaxDiff].Val = 99999;
                    ShowDataResults(ch, (int)SpecItem.xTaklMaxDiff, (int)SpecItem.xTaklMaxDiff, InspType.OnlyMax, new double[] { });
                    return;
                }
            }
            RefAddr = 0x102520E1;
            for (int i = 0; i < 5; i++)
            {
                Dln.Write4Byte(ch, DrvIC.OIS_Addr, 0x6080, 2, RefAddr + (i * 0x400));

                uint wData = (uint)((xTalkDataY[(i * 2) + 1] << 16) + xTalkDataY[i * 2]);
                Dln.Write4Byte(ch, DrvIC.OIS_Addr, 0x6084, 2, wData);
                status = DrvIC.OIS_StausCheck(ch, 0x01, 0x01);
                if (!status)
                {
                    LEDs_All_On(0, false);
                    PassFails[ch].Results[(int)SpecItem.xTaklMaxDiff].Val = 99999;
                    ShowDataResults(ch, (int)SpecItem.xTaklMaxDiff, (int)SpecItem.xTaklMaxDiff, InspType.OnlyMax, new double[] { });
                    return;
                }
            }
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x614A, 2, 0x01);

    
            //Verify
            AddLog(ch, "AF OIS Xtalk Verify");
            DrvIC.OISOnOff(ch, false);

            for (int i = 0; i < 9; i++)
            {
                DrvIC.AFMove(ch, AFVerifyTarget[i]);
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x60F0, 2, AFVerifyTarget[i]);
                Wait(100);
                MeasRes = Measure();
                VerifyOffDataX[i] = MeasRes.cx[0];
                VerifyOffDataY[i] = MeasRes.cy[0];
            }
            DrvIC.OISOnOff(ch, true);
            for (int i = 0; i < 9; i++)
            {
                DrvIC.AFMove(ch, AFVerifyTarget[i]);
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x60F0, 2, AFVerifyTarget[i]);
                Wait(100);
                MeasRes = Measure();
                VerifyOnDataX[i] = MeasRes.cx[0];
                VerifyOnDataY[i] = MeasRes.cy[0];
            }
            string[] LogStr = new string[2] { "Verify Diff X\tVerify Off DataX\tVerify On DataX\r\n", "Verify Diff Y\tVerify Off DataY\tVerify On DataY\r\n" };


            for (int i = 0; i < 9; i++)
            {
                VerifyDiffX[i] = VerifyOffDataX[i] - VerifyOnDataX[i];
                VerifyDiffY[i] = VerifyOffDataY[i] - VerifyOnDataY[i];

                LogStr[0] += $"{VerifyDiffX[i].ToString("F2")}\t{VerifyOffDataX[i].ToString("F2")}\t{VerifyOnDataX[i].ToString("F2")}\r\n";
                LogStr[1] += $"{VerifyDiffY[i].ToString("F2")}\t{VerifyOffDataY[i].ToString("F2")}\t{VerifyOnDataY[i].ToString("F2")}\r\n";
            }

            AddLog(ch, LogStr[0]);
            AddLog(ch, LogStr[1]);

            double VerifyDiffMaxX = VerifyDiffX.Max();
            double VerifyDiffMinX = VerifyDiffX.Min();
            double VerifyDiffMaxY = VerifyDiffY.Max();
            double VerifyDiffMinY = VerifyDiffY.Min();

            AddLog(ch, $"VerifyDiffMaxX : {VerifyDiffMaxX.ToString("F2")}, VerifyDiffMinX : {VerifyDiffMinX.ToString("F2")}");
            AddLog(ch, $"VerifyDiffMaxY : {VerifyDiffMaxY.ToString("F2")}, VerifyDiffMinY : {VerifyDiffMinY.ToString("F2")}");

            double d = Math.Max(Math.Abs(VerifyDiffMaxX - VerifyDiffMinX), Math.Abs(VerifyDiffMaxY - VerifyDiffMinY));


            PassFails[ch].Results[(int)SpecItem.xTaklMaxDiff].Val = d * 2.4;
            ShowDataResults(ch, (int)SpecItem.xTaklMaxDiff, (int)SpecItem.xTaklMaxDiff, InspType.OnlyMax, new double[] { });

            LEDs_All_On(0, false);
        }

        void OISLCCComp(int ch, string testItem, int inspCnt)
        {
            const int SIZE_OFS_TBL = 7;

            FindResult res = new FindResult();
            short[] TargetX = new short[SIZE_OFS_TBL] { -1024, -683, -341, 0, 341, 683, 1024 }; //TBD
            short[] TargetY = new short[SIZE_OFS_TBL] { -1024, -683, -341, 0, 341, 683, 1024 }; //TBD

            List<double> MeasX = new List<double>();
            List<double> MeasY = new List<double>();

            List<int> adjMatrixX = new List<int>();
            List<int> adjMatrixY = new List<int>();
            LEDs_All_On(0, true);
          
            DrvIC.OISOnOff(ch, true);
            Wait(100);
            bool Status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if(!Status)
            {
                LEDs_All_On(0, false);
                PassFails[ch].Results[(int)SpecItem.OISLCCComp].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISLCCComp, (int)SpecItem.OISLCCComp, InspType.OKNG, new double[] { });
                return;
            }
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x617A, 2, 0x00);
            Status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if(!Status)
            {
                LEDs_All_On(0, false);
                PassFails[ch].Results[(int)SpecItem.OISLCCComp].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISLCCComp, (int)SpecItem.OISLCCComp, InspType.OKNG, new double[] { });
                return;
            }
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6020, 2, 0x07);
            Status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if(!Status)
            {
                LEDs_All_On(0, false);
                PassFails[ch].Results[(int)SpecItem.OISLCCComp].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISLCCComp, (int)SpecItem.OISLCCComp, InspType.OKNG, new double[] { });
                return;
            }
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x60B0, 2, 0);
            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x60B2, 2, 0);
            Wait(10);


            for (int i = 0; i < SIZE_OFS_TBL * SIZE_OFS_TBL; i++)
            {
                DrvIC.OISMove(ch, TargetX[i % SIZE_OFS_TBL], TargetY[i / 7]);
                Wait(100);
                res = Measure();
                MeasX.Add(res.cx[0]);
                MeasY.Add(res.cy[0]);
            }

       
            AddLog(ch, $"MoveX\tMoveY");
            for (int i = 0; i < MeasX.Count; i++)
            {
                AddLog(ch, $"{MeasX[i].ToString("F2")}\t{MeasY[i].ToString("F2")}");
            }
            int normParam = 2048;
            int center = (SIZE_OFS_TBL * SIZE_OFS_TBL) / 2;

            int idx_x = SIZE_OFS_TBL * (SIZE_OFS_TBL / 2);
            int idx_x_e = idx_x + (SIZE_OFS_TBL - 1);
            int idx_y = (SIZE_OFS_TBL / 2);
            int idx_y_e = idx_y + SIZE_OFS_TBL * (SIZE_OFS_TBL - 1);

            double sense_px = ((double)(normParam * (SIZE_OFS_TBL - 1)) / (MeasX[idx_x_e] - MeasX[idx_x]));
            double sense_py = ((double)(normParam * (SIZE_OFS_TBL - 1)) / (MeasY[idx_y_e] - MeasY[idx_y]));

            for (int i = 0; i < SIZE_OFS_TBL * SIZE_OFS_TBL; i++)
            {
                int tmpX = (int)((MeasX[i] - MeasX[center]) * sense_px);
                int tmpY = (int)((MeasY[i] - MeasY[center]) * sense_py);

                adjMatrixX.Add(tmpX);
                adjMatrixY.Add(tmpY);
            }
            adjMatrixX.Add(0);
            adjMatrixY.Add(0);



            AddLog(ch, "Updata cal data of matrix y");
            int startAddr = 0x102408E1;
            for (int i = 0; i < 25; i++)
            {
                int addr = startAddr + (i * 0x400);
                uint data = (uint)((adjMatrixY[i * 2 + 1] << 16) + adjMatrixY[i * 2]);
                Dln.Write4Byte(ch, DrvIC.OIS_Addr, 0x6080, 2, addr);
                Dln.Write4Byte(ch, DrvIC.OIS_Addr, 0x6084, 2, data);
                AddLog(ch, $"Addr : 0x{addr.ToString("X8")}, data : 0x{data.ToString("X8")}");
                Status = DrvIC.OIS_StausCheck(ch, 0x01, 0x01);
                if(!Status)
                {
                    LEDs_All_On(0, false);
                    PassFails[ch].Results[(int)SpecItem.OISLCCComp].Val = 1;
                    ShowDataResults(ch, (int)SpecItem.OISLCCComp, (int)SpecItem.OISLCCComp, InspType.OKNG, new double[] { });
                    return;
                }
                
            }
            AddLog(ch, "Updata cal data of matrix x");
            startAddr = 0x10246CE1;
            for (int i = 0; i < 25; i++)
            {
                int addr = startAddr + (i * 0x400);
                uint data = (uint)((adjMatrixX[i * 2 + 1] << 16) + adjMatrixX[i * 2]);
                Dln.Write4Byte(ch, DrvIC.OIS_Addr, 0x6080, 2, addr);
                Dln.Write4Byte(ch, DrvIC.OIS_Addr, 0x6084, 2, data);
                AddLog(ch, $"Addr : 0x{addr.ToString("X8")}, data : 0x{data.ToString("X8")}");
                Status = DrvIC.OIS_StausCheck(ch, 0x01, 0x01);
                if (!Status)
                {
                    LEDs_All_On(0, false);
                    PassFails[ch].Results[(int)SpecItem.OISLCCComp].Val = 1;
                    ShowDataResults(ch, (int)SpecItem.OISLCCComp, (int)SpecItem.OISLCCComp, InspType.OKNG, new double[] { });
                    return;
                }
            }

            DrvIC.OISOnOff(ch, true);
            Wait(100);
            Status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if (!Status)
            {
                LEDs_All_On(0, false);
                PassFails[ch].Results[(int)SpecItem.OISLCCComp].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISLCCComp, (int)SpecItem.OISLCCComp, InspType.OKNG, new double[] { });
                return;
            }
            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x617A, 2, 0x01);
            Status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if (!Status)
            {
                LEDs_All_On(0, false);
                PassFails[ch].Results[(int)SpecItem.OISLCCComp].Val = 1;
                ShowDataResults(ch, (int)SpecItem.OISLCCComp, (int)SpecItem.OISLCCComp, InspType.OKNG, new double[] { });
                return;
            }


            PassFails[ch].Results[(int)SpecItem.OISLCCComp].Val = 0;
            ShowDataResults(ch, (int)SpecItem.OISLCCComp, (int)SpecItem.OISLCCComp, InspType.OKNG, new double[] { });

            //MeasX.Clear();
            //MeasY.Clear();
            //Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6020, 2, 0x07);
            //DrvIC.OISOnOff(ch, true);
            //for (int i = 0; i < SIZE_OFS_TBL * SIZE_OFS_TBL; i++)
            //{
            //    DrvIC.OISMove(ch, TargetX[i % SIZE_OFS_TBL], TargetY[i / 7]);
            //    Wait(100);
            //    res = Measure();
            //    MeasX.Add(res.cx[0]);
            //    MeasY.Add(res.cy[0]);
            //}

            LEDs_All_On(0, false);
        }

        double OISPMGM(int ch, int axis, int type, int start, int end, int points, int amp)
        {
            int inspcnt = points;
            
            bool flag = false;
            int fs = 56250;
            int n = 32;
            int flim = 0;
            double minwait = 100;
            double maxwait = 0;
            ushort fset = 0;

            List<double> freqList = new List<double>();
            List<double> gainList = new List<double>();
            List<double> phaseList = new List<double>();

            string AxisStr = axis == 0 ? "X" : "Y";
            string TypeStr = type == 0 ? "PM" : "GM";
            int AFPos = type == 0 ? Condition.AFPosOISPM : Condition.AFPosOISGM;

            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6020, 2, 0x01);
            Wait(100);
            flag = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if (!flag) return -99999;

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x00);
            DrvIC.AFMove(ch, AFPos); // guide

            AddLog(ch, $"\r\n");
            AddLog(ch, $"{AxisStr} {TypeStr} >>>>>>>>>>>>>>>>>>>>>>");
            AddLog(ch, $"Freq[Hz]\tGain[dB]\tPhase[deg]");

            for (int i = 0; i < points; i++)
            {
                double fsig = Math.Pow(10, Math.Log10(start) + (Math.Log10(end) - Math.Log10(start)) / (points - 1) * i);
                double S1I = 0, S1Q = 0, S2I = 0, S2Q = 0;
                if (Math.Abs(fsig - fs / 16) / fsig < 0.03)
                    fset = (ushort)Math.Floor(32768 * Math.Pow(2 - 2 * Math.Cos(2 * Math.PI * fsig * 0.97 / fs), 0.5));
                else if (Math.Abs(fsig - fs / 8) / fsig < 0.03)
                    fset = (ushort)Math.Floor(32768 * Math.Pow(2 - 2 * Math.Cos(2 * Math.PI * fsig * 0.97 / fs), 0.5));
                else if (Math.Abs(fsig - fs / 4) / fsig < 0.03)
                    fset = (ushort)Math.Floor(32768 * Math.Pow(2 - 2 * Math.Cos(2 * Math.PI * fsig * 0.97 / fs), 0.5));
                else
                    fset = (ushort)Math.Floor(32768 * Math.Pow(2 - 2 * Math.Cos(2 * Math.PI * fsig * 1 / fs), 0.5));

                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DC, 2, 0x0000);
                if (axis == 0) Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61D8, 2, (ushort)(0x0000 | amp));
                else Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61D8, 2, (ushort)(0x8000 | amp));
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DA, 2, 0x0010);
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DC, 2, fset);
                double b0 = Math.Floor(34.8 / 25 * fsig / n * 2 * fs / 56250) + 1;

                if (flim == 0)
                {
                    if (b0 == 1)
                    {
                        flim = 1;
                        maxwait = 1 / fsig * n * 1000;
                    }

                }
                if (i == 0) Wait(1000);
                if (flim == 1)
                {
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(b0 * 100));
                    Wait((int)Math.Max(maxwait * 0.1, minwait * 0.1));
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(0x8000 | (int)(b0 * 10)));

                }
                else if (b0 * 100 < 32767)
                {
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(b0 * 100));
                    Wait((int)Math.Max(1 / fsig * n * 1000 * 0.1, minwait * 0.1));
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(0x8000 | (int)(b0 * 10)));
                }
                else
                {
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, 32767);
                    Wait((int)Math.Max(1 / fsig * n * 1000 * 0.1, minwait * 0.1));
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(0x8000 | (int)(b0 * 10)));
                }

                if (flim == 1)
                {
                    Wait((int)Math.Max(maxwait * 0.2, minwait * 0.2));
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(0x8000 | (int)b0));
                    Wait((int)Math.Max(maxwait * 0.7, minwait * 0.7));
                }
                else
                {
                    Wait((int)Math.Max(1 / fsig * n * 1000 * 0.2, minwait * 0.2));
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(0x8000 | (int)b0));
                    Wait((int)Math.Max(1 / fsig * n * 1000 * 0.7, minwait * 0.7));
                }

                if(axis == 0)
                {
                    S1I = I2C_BU24_RTA_read(ch, 0x5044B0D2);
                    S1Q = I2C_BU24_RTA_read(ch, 0x5044B8D2);
                    S2I = I2C_BU24_RTA_read(ch, 0x5044C0D2);
                    S2Q = I2C_BU24_RTA_read(ch, 0x5044C8D2);
                }
                else
                {
                    S1I = I2C_BU24_RTA_read(ch, 0x5054B0D2);
                    S1Q = I2C_BU24_RTA_read(ch, 0x5054B8D2);
                    S2I = I2C_BU24_RTA_read(ch, 0x5054C0D2);
                    S2Q = I2C_BU24_RTA_read(ch, 0x5054C8D2);
                }
                double gainS1 = Math.Pow((Math.Pow(S1I, 2) + Math.Pow(S1Q, 2)), 0.5);
                double gainS2 = Math.Pow((Math.Pow(S2I, 2) + Math.Pow(S2Q, 2)), 0.5);
                double PhaseS1 = -1 * Math.Atan2(S1Q, S1I);
                double PhaseS2 = -1 * Math.Atan2(-1 * S2Q, -1 * S2I);
                double gain = 20 * Math.Log10(gainS1 / gainS2);
                double phase = 0;
                if ((PhaseS1 - PhaseS2) / Math.PI * 180 > 180)
                    phase = (PhaseS1 - PhaseS2) / Math.PI * 180 - 360;
                else if ((PhaseS1 - PhaseS2) / Math.PI * 180 < -180)
                    phase = (PhaseS1 - PhaseS2) / Math.PI * 180 + 360;
                else phase = (PhaseS1 - PhaseS2) / Math.PI * 180;

                freqList.Add(fsig);
                gainList.Add(gain);
                phaseList.Add(phase);

                AddLog(ch, string.Format("{0:0.000}\t{1:0.000}\t{2:0.000}", fsig, gain, phase));
                if (i > 0)
                {
                    if (type == 0)
                    {
                        if (gainList[i] * gainList[i - 1] <= 0)
                        {
                            inspcnt = gainList.Count;
                            AddLog(ch, "Zero Cross detected");
                            break;
                        }
                    }
                    else if (type == 1)
                    {
                        if (phaseList[i] * phaseList[i - 1] <= 0)
                        {
                            inspcnt = gainList.Count;
                            AddLog(ch, "Zero Cross detected");
                            break;
                        }
                    }
                }
                if (fsig == end)
                {
                    AddLog(ch, "The end of measurement");
                    break;
                }
            }

            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DC, 2, 0x0000);
            double[] resArr = new double[2]; 
            if(type == 0)
            {
                resArr = getPhase(ch, freqList.ToList(), gainList.ToList(), phaseList.ToList(), inspcnt);
                AddLog(ch, $"Freq[Hz] : {resArr[0].ToString("F3")}, Phase[deg] : {resArr[1].ToString("F3")}");
            }
            else
            {
                resArr = getGain(ch, freqList.ToList(), gainList.ToList(), phaseList.ToList(), inspcnt);
                AddLog(ch, $"Freq[Hz] : {resArr[0].ToString("F3")}, Gain[dB] : {resArr[1].ToString("F3")}");
            }
            return resArr[1];
           
        }

        void Xtalk2(int ch, string testItem, int inspCnt)
        {
            bool status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if (!status)
            {
                PassFails[0].Results[(int)SpecItem.XyCrosstalk2].Val = 99999;
                PassFails[0].Results[(int)SpecItem.YxCrosstalk2].Val = 99999;
                ShowDataResults(ch, (int)SpecItem.XyCrosstalk2, (int)SpecItem.YxCrosstalk2, InspType.OnlyMax, new double[] { });
                return;
            }



            LEDs_All_On(0, true);
            DrvIC.AFOnOff(ch, true);
            DrvIC.OISOnOff(ch, true);
            DrvIC.SetManualDrvModeXY(ch, 0, 0);
            DrvIC.AFMove(ch, Condition.Xtalk2AFPos);
            Wait(Condition.Xtalk2Delay);
            AddLog(ch, $"AF Position : {DrvIC.ReadAFHall(ch)}");
            FindResult res = new FindResult();

            double X_Xmin = 0;
            double X_Xmax = 0;
            double X_Ymin = 0;
            double X_Ymax = 0;

            double Y_Xmin = 0;
            double Y_Xmax = 0;
            double Y_Ymin = 0;
            double Y_Ymax = 0;


            DrvIC.OISMove(ch, Condition.xTalk2MinCodeX, 0);
            Wait(Condition.Xtalk2Delay);
            res = Measure();
            X_Xmin = res.cx[0]; X_Ymin = res.cy[0];

            DrvIC.OISMove(ch, Condition.xTalk2MaxCodeX, 0);
            Wait(Condition.Xtalk2Delay);
            res = Measure();
            X_Xmax = res.cx[0]; X_Ymax = res.cy[0];


            DrvIC.OISMove(ch, 0, Condition.xTalk2MinCodeY);
            Wait(Condition.Xtalk2Delay);
            res = Measure();
            Y_Xmin = res.cx[0]; Y_Ymin = res.cy[0];

            DrvIC.OISMove(ch, 0, Condition.xTalk2MaxCodeY);
            Wait(Condition.Xtalk2Delay);
            res = Measure();
            Y_Xmax = res.cx[0]; Y_Ymax = res.cy[0];

            AddLog(ch, $"X code Xmin : {X_Xmin.ToString("F3")}, X code Xmax : {X_Xmax.ToString("F3")}");
            AddLog(ch, $"X code Ymin : {X_Ymin.ToString("F3")}, X code Ymax : {X_Ymax.ToString("F3")}");
            AddLog(ch, $"Y code Xmin : {Y_Xmin.ToString("F3")}, Y code Xmax : {Y_Xmax.ToString("F3")}");
            AddLog(ch, $"Y code Ymin : {Y_Ymin.ToString("F3")}, Y code Ymax : {Y_Ymax.ToString("F3")}");

            AddLog(ch, "\r\n");

            double xyTalk = (Math.Abs(X_Ymax - X_Ymin) / Math.Abs(Y_Ymax - Y_Ymin)) * 100f;
            double yxTalk = (Math.Abs(Y_Xmax - Y_Xmin) / Math.Abs(X_Xmax - X_Xmin)) * 100f;

            AddLog(ch, $"Xy xtalk : {xyTalk.ToString("F3")}, Yx xtalk : {yxTalk.ToString("F3")}");


            PassFails[ch].Results[(int)SpecItem.XyCrosstalk2].Val = xyTalk;
            PassFails[ch].Results[(int)SpecItem.YxCrosstalk2].Val = yxTalk;
            ShowDataResults(ch, (int)SpecItem.XyCrosstalk2, (int)SpecItem.YxCrosstalk2, InspType.OnlyMax, new double[] { });


            LEDs_All_On(0, false);

        }

        double OISLG(int ch, int axis, int amp, int Freq)
        {
          
            bool flag = false;
            int fs = 56250;
            int n = 32;
            int flim = 0;
            double minwait = 100;
            double maxwait = 0;
            ushort fset = 0;

            List<double> freqList = new List<double>();
            List<double> gainList = new List<double>();
            List<double> phaseList = new List<double>();

            string AxisStr = axis == 0 ? "X" : "Y";

            int AFPos = Condition.OISLoopGainAFPos;

            Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x6020, 2, 0x01);
            Wait(100);
            flag = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if (!flag) return -99999;

            Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x00);
            DrvIC.AFMove(ch, AFPos); // guide
            Wait(100);
            AddLog(ch, $"AF Position : {DrvIC.ReadAFHall(ch)}");
            AddLog(ch, $"\r\n");
            AddLog(ch, $"{AxisStr} >>>>>>>>>>>>>>>>>>>>>>");
            AddLog(ch, $"Freq[Hz]\tGain[dB]");

            for (int i = 0; i < 1; i++)
            {
                double fsig = Freq;
                double S1I = 0, S1Q = 0, S2I = 0, S2Q = 0;
                if (Math.Abs(fsig - fs / 16) / fsig < 0.03)
                    fset = (ushort)Math.Floor(32768 * Math.Pow(2 - 2 * Math.Cos(2 * Math.PI * fsig * 0.97 / fs), 0.5));
                else if (Math.Abs(fsig - fs / 8) / fsig < 0.03)
                    fset = (ushort)Math.Floor(32768 * Math.Pow(2 - 2 * Math.Cos(2 * Math.PI * fsig * 0.97 / fs), 0.5));
                else if (Math.Abs(fsig - fs / 4) / fsig < 0.03)
                    fset = (ushort)Math.Floor(32768 * Math.Pow(2 - 2 * Math.Cos(2 * Math.PI * fsig * 0.97 / fs), 0.5));
                else
                    fset = (ushort)Math.Floor(32768 * Math.Pow(2 - 2 * Math.Cos(2 * Math.PI * fsig * 1 / fs), 0.5));

                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DC, 2, 0x0000);
                if (axis == 0) Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61D8, 2, (ushort)(0x0000 | amp));
                else Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61D8, 2, (ushort)(0x8000 | amp));
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DA, 2, 0x0010);
                Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DC, 2, fset);
                double b0 = Math.Floor(34.8 / 25 * fsig / n * 2 * fs / 56250) + 1;

                if (flim == 0)
                {
                    if (b0 == 1)
                    {
                        flim = 1;
                        maxwait = 1 / fsig * n * 1000;
                    }

                }
                if (i == 0) Wait(1000);
                if (flim == 1)
                {
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(b0 * 100));
                    Wait((int)Math.Max(maxwait * 0.1, minwait * 0.1));
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(0x8000 | (int)(b0 * 10)));

                }
                else if (b0 * 100 < 32767)
                {
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(b0 * 100));
                    Wait((int)Math.Max(1 / fsig * n * 1000 * 0.1, minwait * 0.1));
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(0x8000 | (int)(b0 * 10)));
                }
                else
                {
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, 32767);
                    Wait((int)Math.Max(1 / fsig * n * 1000 * 0.1, minwait * 0.1));
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(0x8000 | (int)(b0 * 10)));
                }

                if (flim == 1)
                {
                    Wait((int)Math.Max(maxwait * 0.2, minwait * 0.2));
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(0x8000 | (int)b0));
                    Wait((int)Math.Max(maxwait * 0.7, minwait * 0.7));
                }
                else
                {
                    Wait((int)Math.Max(1 / fsig * n * 1000 * 0.2, minwait * 0.2));
                    Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DE, 2, (ushort)(0x8000 | (int)b0));
                    Wait((int)Math.Max(1 / fsig * n * 1000 * 0.7, minwait * 0.7));
                }

                if (axis == 0)
                {
                    S1I = I2C_BU24_RTA_read(ch, 0x5044B0D2);
                    S1Q = I2C_BU24_RTA_read(ch, 0x5044B8D2);
                    S2I = I2C_BU24_RTA_read(ch, 0x5044C0D2);
                    S2Q = I2C_BU24_RTA_read(ch, 0x5044C8D2);
                }
                else
                {
                    S1I = I2C_BU24_RTA_read(ch, 0x5054B0D2);
                    S1Q = I2C_BU24_RTA_read(ch, 0x5054B8D2);
                    S2I = I2C_BU24_RTA_read(ch, 0x5054C0D2);
                    S2Q = I2C_BU24_RTA_read(ch, 0x5054C8D2);
                }
                double gainS1 = Math.Pow((Math.Pow(S1I, 2) + Math.Pow(S1Q, 2)), 0.5);
                double gainS2 = Math.Pow((Math.Pow(S2I, 2) + Math.Pow(S2Q, 2)), 0.5);
            
                double gain = 20 * Math.Log10(gainS1 / gainS2);
                double phase = 0;
             

                freqList.Add(fsig);
                gainList.Add(gain);
              
                AddLog(ch, string.Format("{0:0.000}\t{1:0.000}\t{2:0.000}", fsig, gain, phase));
             
            }

            Dln.Write2Byte(ch, DrvIC.OIS_Addr, 0x61DC, 2, 0x0000);
            double[] resArr = new double[2];
           
            AddLog(ch, $"Freq[Hz] : {freqList[0].ToString("F3")}, Gain[dB] : {gainList[0].ToString("F3")}");
            return gainList[0];

        }

        double[] getPhase(int ch, List<double> freq, List<double> gain, List<double> phase, int cnt)
        {

            double PM = 360;
            double freq_Pm = 0;

            if (cnt > 1)
            {
                for (int i = 1; i < cnt; i++)
                {
                    if (gain[i - 1] * gain[i] <= 0)
                    {
                        double g_lin_a = (gain[i - 1] - gain[i]) / (freq[i - 1] - freq[i]);
                        double g_lin_b = gain[i] - freq[i] * g_lin_a;
                        double freqCorss = -1 * g_lin_b / g_lin_a;
                        double p_lin_a = (phase[i - 1] - phase[i]) / (freq[i - 1] - freq[i]);
                        double p_lin_b = phase[i] - freq[i] * p_lin_a;
                        double phaseTmp = Math.Abs(p_lin_a * freqCorss + p_lin_b);
                        if (phaseTmp < PM)
                        {
                            PM = phaseTmp;
                            freq_Pm = freqCorss;
                        }
                    }
                }
                if (PM == 360)
                {
                    AddLog(ch, "It can't get phase margin because there is no zero-cross point!!!");
                    return new double[2] { -999, -999 };

                }
            }
            else
            {
                AddLog(ch, "It can't get phase margin because cnt_point is 1");
                return new double[2] { -999, -999 };
            }

            AddLog(ch, string.Format("Freq : {0: 0.000}, Phase : {1 : 0.000}", freq_Pm, PM));
            return new double[2] { freq_Pm, PM };
        }
        double[] getGain(int ch, List<double> freq, List<double> gain, List<double> phase, int cnt)
        {

            double GM = 100;
            double freq_GM = 0;

            if (cnt > 1)
            {
                for (int i = 1; i < cnt; i++)
                {
                    if (phase[i - 1] * phase[i] <= 0)
                    {
                        double g_lin_a = (phase[i - 1] - phase[i]) / (freq[i - 1] - freq[i]);
                        double g_lin_b = phase[i] - freq[i] * g_lin_a;
                        double freqCorss = -1 * g_lin_b / g_lin_a;
                        double p_lin_a = (gain[i - 1] - gain[i]) / (freq[i - 1] - freq[i]);
                        double p_lin_b = gain[i] - freq[i] * p_lin_a;
                        double gainTmp = Math.Abs(p_lin_a * freqCorss + p_lin_b);
                        if (gainTmp < GM)
                        {
                            GM = gainTmp;
                            freq_GM = freqCorss;
                        }
                    }
                }
                if (GM == 100)
                {
                    AddLog(ch, "It can't get gain margin because there is no zero-cross point!!!");
                    return new double[2] { -999, -999 };

                }
            }
            else
            {
                AddLog(ch, "It can't get gain margin because cnt_point is 1");
                return new double[2] { -999, -999 };
            }

            AddLog(ch, string.Format("Freq : {0: 0.000}, Gain : {1 : 0.000}", freq_GM, GM));
            return new double[2] { freq_GM, GM };
        }
        int I2C_BU24_RTA_read(int ch, uint data)
        {
            bool flag = false;
            
            try
            {
                Dln.Write4Byte(ch, DrvIC.OIS_Addr, 0x6080, 2, data);
                Dln.Write4Byte(ch, DrvIC.OIS_Addr, 0x6084, 2, 0x00000000);
                flag = DrvIC.OIS_StausCheck(ch, 0x01, 0x01);
                if (!flag) return int.MaxValue;

                return Dln.Read4Byte_signed(ch, DrvIC.OIS_Addr, 0x6084, 2);

            }
            catch
            {
                return int.MaxValue;
            }
        }
        public static void Wait(int ms)
        {
            //       Thread.Sleep(ms);
            ms = ms * 1000;
            Stopwatch startNew = Stopwatch.StartNew();

            long usDelayTick = (ms * Stopwatch.Frequency) / 1000000;

            while (startNew.ElapsedTicks < usDelayTick) ;



            //if (ms <= 0)
            //    return;

            //var sw = Stopwatch.StartNew();

            //// 목표 tick (ms → tick)
            //double targetTicks = ms * (double)Stopwatch.Frequency / 1000.0;

            //while (true)
            //{
            //    double elapsedTicks = sw.ElapsedTicks;
            //    double remainingTicks = targetTicks - elapsedTicks;

            //    if (remainingTicks <= 0)
            //        break;

            //    // 남은 tick → ms로 환산
            //    double remainingMs = remainingTicks * 1000.0 / Stopwatch.Frequency;

            //    if (remainingMs > 5.0)
            //    {
            //        // 아직 여유가 많으면 1ms씩 Sleep하면서 CPU 양보
            //        Thread.Sleep(1);
            //    }
            //    else if (remainingMs > 1.0)
            //    {
            //        // 1~5ms 남은 구간: 가벼운 SpinWait로 세밀히 접근
            //        Thread.SpinWait(500); // 값은 환경에 맞게 조절 가능
            //    }
            //    else
            //    {
            //        // 1ms 이하 남은 구간: 매우 짧게 busy-wait로 마무리
            //        // (Stopwatch 해상도에 가까운 정밀도)
            //        // 여기서는 불필요한 연산 없이 루프만 돎
            //        while (sw.ElapsedTicks < targetTicks)
            //        {
            //            // tight spin
            //        }
            //        break;
            //    }
            //}
        }


        void AFFluctuation(int ch, string testItem, int inspCnt)
        {
            LEDs_All_On(0, true);
            DrvIC.AFOnOff(ch, true);
         
            DrvIC.AFMove(ch, Condition.AFFluctuationMinCode);
            Wait(Condition.AFFluctuationDelay);

            string dataDir = STATIC.CreateDateDir();
            dataDir += "AFFluctuaionData\\";
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            string timeDir = $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s";

            List<string> arr = new List<string>();
            string path = string.Format(dataDir + "{0}_{1}_{2}_{3}.csv", "AF_Fluctuaion", m_StrIndex[ch], inspCnt + 1, timeDir);
            FindResult res = new FindResult();

            List<int> Pos = new List<int>();
            List<double> Min = new List<double>();
            List<double> Max = new List<double>();
            List<double> P2P = new List<double>();
        
            for (int i = Condition.AFFluctuationMinCode; i <= Condition.AFFluctuationMaxCode; i += Condition.AFFluctuationStepCode)
            {
                double MinVal = double.MaxValue;
                double MaxVal = double.MinValue;
                double tmpP2P = double.MinValue;
                DrvIC.AFMove(ch, i);
                Wait(Condition.AFFluctuationDelay);
                AddLog(ch, $"AF Fluctuation Test Code : {i} >>>>>>>>>>>>>>>>");
                for (int j = 0; j < Condition.AFFluctuationCount; j++)
                {
                    res = Measure();
                    if (res.cz[0] > MaxVal) MaxVal = res.cz[0];
                    if (res.cz[0] < MinVal) MinVal = res.cz[0];
                }
                tmpP2P = Math.Abs(MaxVal - MinVal);

                Pos.Add(i);
                Min.Add(MinVal);
                Max.Add(MaxVal);
                P2P.Add(tmpP2P);

                AddLog(ch, $"Pos:{i}, Min:{MinVal.ToString("F3")}, Max:{MaxVal.ToString("F3")}, P2P:{tmpP2P.ToString("F3")}");
            }

            if(Option.SaveRawData)
            {
                arr.Add($"AF Position, Min, Max, P2P");
                for (int i = 0; i < P2P.Count; i++)
                {
                    arr.Add($"{Pos[i]}, {Min[i].ToString("F3")}, {Max[i].ToString("F3")}, {P2P[i].ToString("F3")}");
                }
                if (path != "") STATIC.SetTextLine(path, arr);
            }

            PassFails[ch].Results[(int)SpecItem.AFfluctuation].Val = P2P.Max();
            ShowDataResults(ch, (int)SpecItem.AFfluctuation, (int)SpecItem.AFfluctuation, InspType.Normal, new double[] { });
            LEDs_All_On(0, false);

        }

        void OISXFluctuation(int ch, string testItem, int inspCnt)
        {
            double d = OISFluctuation(ch, 0, Condition.XFluctuationMinCode, Condition.XFluctuationMaxCode, Condition.XFluctuationStepCode, Condition.XFluctuationAFPos,
                Condition.XFluctuationDelay, Condition.XFluctuationCount, inspCnt);

            PassFails[ch].Results[(int)SpecItem.Xfluctuation].Val = d;
            ShowDataResults(ch, (int)SpecItem.Xfluctuation, (int)SpecItem.Xfluctuation, InspType.Normal, new double[] { });
        }

        void OISYFluctuation(int ch, string testItem, int inspCnt)
        {
            double d = OISFluctuation(ch, 1, Condition.YFluctuationMinCode, Condition.YFluctuationMaxCode, Condition.YFluctuationStepCode, Condition.YFluctuationAFPos,
                Condition.YFluctuationDelay, Condition.YFluctuationCount, inspCnt);

            PassFails[ch].Results[(int)SpecItem.Yfluctuation].Val = d;
            ShowDataResults(ch, (int)SpecItem.Yfluctuation, (int)SpecItem.Yfluctuation, InspType.Normal, new double[] { });
        }

        double OISFluctuation(int ch, int axis, int MinCode, int MaxCode, int Step, int AFPos, int Delay, int count, int inspCnt)
        {

            bool status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            if (!status)
            {
              
                return 99999;
            }

            string AxisName = axis == 0 ? "X" : "Y";


            LEDs_All_On(0, true);
            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMove(ch, AFPos);
            DrvIC.OISOnOff(ch, true);
            DrvIC.SetManualDrvModeXY(ch, 0, 0);
            Wait(Delay);
            AddLog(ch, $"AF Position : {DrvIC.ReadAFHall(ch)}");
            string dataDir = STATIC.CreateDateDir();
            dataDir += $"OIS {AxisName} FluctuaionData\\";
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            string timeDir = $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s";

            List<string> arr = new List<string>();
            string path = string.Format(dataDir + "{0}_{1}_{2}_{3}.csv", $"{AxisName}_Fluctuaion", m_StrIndex[ch], inspCnt + 1, timeDir);
            FindResult res = new FindResult();

            List<int> Pos = new List<int>();
            List<double> Min = new List<double>();
            List<double> Max = new List<double>();
            List<double> P2P = new List<double>();

            for (int i = MinCode; i <= MaxCode; i += Step)
            {
                double MinVal = double.MaxValue;
                double MaxVal = double.MinValue;
                double tmpP2P = double.MinValue;

                if (axis == 0) DrvIC.OISMove(ch, i, 0);
                else DrvIC.OISMove(ch, 0, i);
                Wait(Delay);
                AddLog(ch, $"OIS {AxisName} Fluctuation Test Code : {i} >>>>>>>>>>>>>>>>");
                for (int j = 0; j < count; j++)
                {
                    res = Measure();
                    if(axis == 0)
                    {
                        if (res.cx[0] > MaxVal) MaxVal = res.cx[0];
                        if (res.cx[0] < MinVal) MinVal = res.cx[0];
                    }
                    else
                    {
                        if (res.cy[0] > MaxVal) MaxVal = res.cy[0];
                        if (res.cy[0] < MinVal) MinVal = res.cy[0];
                    }
                   
                }
                tmpP2P = Math.Abs(MaxVal - MinVal);

                Pos.Add(i);
                Min.Add(MinVal);
                Max.Add(MaxVal);
                P2P.Add(tmpP2P);

                AddLog(ch, $"Pos:{i}, Min:{MinVal.ToString("F3")}, Max:{MaxVal.ToString("F3")}, P2P:{tmpP2P.ToString("F3")}");
            }

            if (Option.SaveRawData)
            {
                arr.Add($"{AxisName} Position, Min, Max, P2P");
                for (int i = 0; i < P2P.Count; i++)
                {
                    arr.Add($"{Pos[i]}, {Min[i].ToString("F3")}, {Max[i].ToString("F3")}, {P2P[i].ToString("F3")}");
                }
                if (path != "") STATIC.SetTextLine(path, arr);
            }

            LEDs_All_On(0, false);
            return P2P.Max();
        }

        #endregion
    }
}
