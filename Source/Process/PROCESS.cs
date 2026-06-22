using Basler.Pylon;
using FZ4P.Properties;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Flann;
using OpenCvSharp.XImgProc;
using S2System.Vision;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace FZ4P
{
    public partial class Process
    {
        public DLN Dln { get { return STATIC.Dln; } }
        public DrvIC DrvIC { get { return STATIC.DrvIC; } }
        public Recipe Rcp { get { return STATIC.Rcp; } }
        public Condition Condition { get { return STATIC.Rcp.Condition; } }
        public Spec Spec { get { return STATIC.Rcp.Spec; } }
        public Option Option { get { return STATIC.Rcp.Option; } }
        public Model Model { get { return STATIC.Rcp.Model; } }
        public CurrentPath Current { get { return STATIC.Rcp.Current; } }
        public List<PassFail> PassFails { get { return STATIC.Rcp.PassFails; } }
        public TotalYield yield { get { return STATIC.Rcp.yield; } }
     
        Global m__G = null; 


        public ObservableCollection<ActItems> ItemList = new ObservableCollection<ActItems>();
        public List<NVMHallParam> HallParam = new List<NVMHallParam>();
        public List<Task> RunTasks = new List<Task>();
        public int RunTaskId1 = 0;
        public int RunTaskId2 = 0;

        public bool m_bAllLEDOn = false;
        public bool IsVirtual = false;
        public bool SuddenStop = false;
        public int RepeatRun = 0;
        public int CurrentRun = 0;
        public bool IsHallComplete = false;
        public int PortCnt { get; set; }
        public int ChannelCnt { get; set; }

     
        public List<string> errMsg = new List<string>();
        public List<bool> m_ChannelOn = new List<bool>();
        public List<string> m_StrIndex = new List<string>();
        public List<bool> IsScan = new List<bool>();
        public List<int> framCnt = new List<int>();

        public List<byte[]> FWCode = new List<byte[]>();

        public event EventHandler<int> RunStart = null;
        public event EventHandler<int> RunEnd = null;

        public List<LogText> ViewLog = new List<LogText>();

        public List<InfoButton> InfoBtn = new List<InfoButton>();

        public BarcodeInfoButton BarcodeInfoBtn = new BarcodeInfoButton();

        public List<DrvParam> DrvValue = new List<DrvParam>();

        public List<List<CalResult>> CalList = new List<List<CalResult>>();

        public DataGridView ResultDataGrid = new DataGridView()
        { Size = new System.Drawing.Size(780, 828) };
        public Label lblFailList = new Label();
        public List<ChartList> ChartTop = new List<ChartList>();
        public List<ChartList> ChartBtm = new List<ChartList>();
        public List<TiltGraph> tiltChart = new List<TiltGraph>();

        //    public List<ChartList> ChartBtm = new List<ChartList>();

        public OISCalParameter OISCalData = new OISCalParameter();
        public double[] AFRatedMinMax = new double[3];
        public int[] XHATPOSAF = new int[2];
        public int[] YHATPOSAF = new int[2];
        public int[] XHATPOSOIS = new int[2];
        public int[] YHATPOSOIS = new int[2];
        public int[] AFOISVerifyXPOSOIS = new int[2];
        public int[] AFOISVerifyYPOSOIS = new int[2];



        public int OISXCenter = 0;
        public int OISYCenter = 0;
        public int AFCenter = 2048;
        double SlopeX = 0;
        double SlopeY = 0;
        public bool I2CMonitorStartFlag = false;
        bool isI2cMonitoring = false;

        public bool SafetyState = false;
        public bool EMGState = false;
        public bool StopState = false;
        public Process()
        {
            PortCnt = 1;
            ChannelCnt = 1;

            for (int i = 0; i < PortCnt; i++)
            {
              
                IsScan.Add(false);
                framCnt.Add(0);
            }
            for (int i = 0; i < ChannelCnt; i++)
            {
                errMsg.Add("");
                m_ChannelOn.Add(false);
                m_StrIndex.Add("");
                HallParam.Add(new NVMHallParam());
                DrvValue.Add(new DrvParam());

                CalList.Add(new List<CalResult>());
                CalList[i].Add(new CalResult("AF Scan"));              
                CalList[i].Add(new CalResult("AF Settling"));
                CalList[i].Add(new CalResult("OIS X Scan"));              
                CalList[i].Add(new CalResult("OIS Y Scan"));

                ChartTop.Add(new ChartList("Stroke", i));
                ChartBtm.Add(new ChartList("Settling", i));
                tiltChart.Add(new TiltGraph
                {
                    title = "AF Tilt",
                    range = 15,
                });
                tiltChart[i].SetRings(new double[] { tiltChart[i].range / 2, tiltChart[i].range });
                

                InfoBtn.Add(new InfoButton()); //test
                InfoBtn.Add(new InfoButton());
                ViewLog.Add(new LogText());
            }
            ItemList.Add(new ActItems() { Name = "AF Scan", Func = Act_ScanCode });
            ItemList.Add(new ActItems() { Name = "OIS X Scan", Func = Act_ScanCode });
            ItemList.Add(new ActItems() { Name = "OIS Y Scan", Func = Act_ScanCode });
            ItemList.Add(new ActItems() { Name = "AF Settling", Func = Act_ScanTimeCode });
           
            AddSequence();

            Rcp.RetryCnt = new RetryCount();
            Rcp.RetryCnt.RetryOption.Add(new Retry { InspName = "All", Count = 0 });
          
            for (int i = 0; i < ItemList.Count; i++)
                Rcp.RetryCnt.RetryOption.Add(new Retry { InspName = ItemList[i].Name, Count = 0 });
            if (File.Exists(STATIC.RetryCountDir))
            {
                RetryCount compare = new RetryCount();
                compare = DataIO.DeserializeXMLFileToObject<RetryCount>(STATIC.RetryCountDir);
                for (int i = 0; i < compare.RetryOption.Count; i++)
                {
                    int index = Rcp.RetryCnt.RetryOption.FindIndex(x => x.InspName == compare.RetryOption[i].InspName);
                    if(index != -1) Rcp.RetryCnt.RetryOption[index].Count = compare.RetryOption[i].Count;
                }
            }

            m__G = Global.GetInstance();
        }



        #region Default

        public void StartI2CMonitor()
        {
            if (I2CMonitorStartFlag) return;
            I2CMonitorStartFlag = true;
            isI2cMonitoring = true;
            Task monitorI2C = new Task(() => MonitorI2C());
            monitorI2C.Start();
        }
        void MonitorI2C()
        {
            while (true)
            {
                if (!I2CMonitorStartFlag) { m__G.mIDLEcount = 0; break; }
                Thread.Sleep(5000);
                if (!I2CMonitorStartFlag) { m__G.mIDLEcount = 0; break; }
                if (!Dln.IsRun)
                {
                    m__G.mIDLEcount++;
                    if (m__G.mIDLEcount > 7)
                    {
                        List<double> led = new List<double>() { 0.5, 0.5 };
                        LEDs_All_On(0, true, led);
                        Thread.Sleep(1);
                        if (m__G.mIDLEcount > 7)
                        {
                            if (!Dln.IsRun)
                            {
                                LEDs_All_On(0, false);
                                m__G.mIDLEcount = 0;
                            }
                        }

                    }
                }
                else
                {
                    m__G.mIDLEcount = 0;
                }
                if (!I2CMonitorStartFlag) { m__G.mIDLEcount = 0; break; }
            }
            isI2cMonitoring = false;
        }
        public bool CheckFail(int ch, string Item)
        {
            for (int i = 0; i < Spec.specList.Count; i++)
            {
                if (Spec.specList[i].Category == Item)
                {
                    if (!PassFails[ch].Results[i].bPass) return false;
                }
            }
            return true;
        }
        public void SetFailList(int ch)
        {
            if (lblFailList.InvokeRequired)
            {
                lblFailList.BeginInvoke((MethodInvoker)delegate
                {
                    for (int i = 0; i < Spec.specList.Count; i++)
                    {
                        if (!PassFails[ch].Results[i].bPass) { STATIC.FailNumber += $"{i + 1},"; lblFailList.Text = STATIC.FailNumber; }

                    }

                });
            }
            else
            {
                for (int i = 0; i < Spec.specList.Count; i++)
                {
                    if (!PassFails[ch].Results[i].bPass) { STATIC.FailNumber += $"{i + 1},"; lblFailList.Text = STATIC.FailNumber; }
                }
            }
        }
        public void ShowDataResults(int ch, int start, int end, InspType type, double[] MtoMRes )
        {
            for (int i = start; i < end + 1; i++)
            {
                if (!Spec.specList[i].OnOff) continue;

                double lmin, lmax;
                lmin = Convert.ToDouble(Spec.specList[i].MinSpec);
                lmax = Convert.ToDouble(Spec.specList[i].MaxSpec);

                switch(type)
                {
                    case InspType.Normal:
                        if (PassFails[ch].Results[i].Val < lmin || PassFails[ch].Results[i].Val > lmax || double.IsNaN(PassFails[ch].Results[i].Val))
                        {
                            PassFails[ch].Results[i].msg = Spec.specList[i].DisplayName;
                            PassFails[ch].Results[i].bPass = false;
                          
                        }
                        else
                        {
                            PassFails[ch].Results[i].msg = "";
                            PassFails[ch].Results[i].bPass = true;

                        }
                        break;
                    case InspType.OKNG:
                        if (PassFails[ch].Results[i].Val != 0)
                        {
                            PassFails[ch].Results[i].msg = Spec.specList[i].DisplayName;
                            PassFails[ch].Results[i].bPass = false;
                          
                        }
                        else
                        {
                            PassFails[ch].Results[i].msg = "";
                            PassFails[ch].Results[i].bPass = true;

                        }
                        break;
                    case InspType.OnlyMax:
                        if (PassFails[ch].Results[i].Val > lmax || double.IsNaN(PassFails[ch].Results[i].Val))
                        {
                            PassFails[ch].Results[i].msg = Spec.specList[i].DisplayName;
                            PassFails[ch].Results[i].bPass = false;
                           
                        }
                        else
                        {
                            PassFails[ch].Results[i].msg = "";
                            PassFails[ch].Results[i].bPass = true;

                        }
                        break;
                    case InspType.OnlyMin:
                        if (PassFails[ch].Results[i].Val < lmin ||double.IsNaN(PassFails[ch].Results[i].Val))
                        {
                            PassFails[ch].Results[i].msg = Spec.specList[i].DisplayName;
                            PassFails[ch].Results[i].bPass = false;
                            
                        }
                        else
                        {
                            PassFails[ch].Results[i].msg = "";
                            PassFails[ch].Results[i].bPass = true;

                        }
                        break;
                    case InspType.MintoMax:

                        if (MtoMRes[1] < lmin || MtoMRes[0] > lmax || double.IsNaN(MtoMRes[0]) || double.IsNaN(MtoMRes[1]))
                        {
                            PassFails[ch].Results[i].msg = Spec.specList[i].DisplayName;
                            PassFails[ch].Results[i].bPass = false;
                          
                        }
                        else
                        {
                            PassFails[ch].Results[i].msg = "";
                            PassFails[ch].Results[i].bPass = true;

                        }
                        break;
                }
                

            
            }
            for (int i = start; i < end + 1; i++)
            {
                if (!PassFails[ch].Results[i].bPass)
                {
                    if (PassFails[ch].FirstFailIndex == 0)
                    {
                        PassFails[ch].FirstFailIndex = (i + 1);
                        PassFails[ch].FirstFail = PassFails[ch].Results[i].msg;

                        int failCnt = Convert.ToInt32(Spec.specList[i].FailCnt); failCnt++;
                        Spec.specList[i].FailCnt = failCnt;
                    }

               
                }
            }

            if (ResultDataGrid.InvokeRequired)
            {
                ResultDataGrid.BeginInvoke((MethodInvoker)delegate
                {
                    for (int i = start; i <= end; i++)
                    {
                        if (type == InspType.MintoMax)
                            ResultDataGrid[ch + 4, i].Value = $"{MtoMRes[1]} ~ {MtoMRes[0]}";
                        else if (type == InspType.OKNG)
                        {
                            if (PassFails[ch].Results[i].Val == 0)
                                ResultDataGrid[ch + 4, i].Value = "OK";
                            else ResultDataGrid[ch + 4, i].Value = "NG";
                        }
                        else
                        {
                            ResultDataGrid[ch + 4, i].Value = PassFails[ch].Results[i].Val.ToString("F3");
                        }
                        if (PassFails[ch].Results[i].bPass) { ResultDataGrid[ch + 4, i].Style.BackColor = Color.White; ResultDataGrid[ch, i].Style.BackColor = Color.White; }
                        else { ResultDataGrid[ch + 4, i].Style.BackColor = Color.Orange; ResultDataGrid[ch, i].Style.BackColor = Color.Orange; }
                        

                    }

                });
            }
            else
            {
                for (int i = start; i <= end; i++)
                {
                    if (type == InspType.MintoMax)
                        ResultDataGrid[ch + 4, i].Value = $"{MtoMRes[1]} ~ {MtoMRes[0]}";
                    else if (type == InspType.OKNG)
                    {
                        if (PassFails[ch].Results[i].Val == 0)
                            ResultDataGrid[ch + 4, i].Value = "OK";
                        else ResultDataGrid[ch + 4, i].Value = "NG";
                    }
                    else
                    {
                        ResultDataGrid[ch + 4, i].Value = PassFails[ch].Results[i].Val.ToString("F3");
                    }
                    if (PassFails[ch].Results[i].bPass) { ResultDataGrid[ch + 4, i].Style.BackColor = Color.White; ResultDataGrid[ch, i].Style.BackColor = Color.White; }
                    else { ResultDataGrid[ch + 4, i].Style.BackColor = Color.Orange; ResultDataGrid[ch, i].Style.BackColor = Color.Orange; }

                }
            }

          
            for (int i = start; i <= end; i++)
            {
                if (!PassFails[ch].Results[i].bPass)
                {
                    if (!Option.ContinueTestingOnFail) m_ChannelOn[ch] = false;
                }


            }

        }     
        public void InitResultData()
        {
            Type dgvType = ResultDataGrid.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(ResultDataGrid, true, null);

            ResultDataGrid.AllowUserToAddRows = false;
            ResultDataGrid.AllowUserToDeleteRows = false;
            ResultDataGrid.AllowUserToResizeColumns = false;
            ResultDataGrid.AllowUserToResizeRows = false;
            ResultDataGrid.Tag = "S";
            ResultDataGrid.ColumnCount = 6; //  Group, Item, min, max, r0, r1, r2, r3, unit, Fratio
            ResultDataGrid.Font = new Font("Calibri", 10, FontStyle.Bold);
            for (int i = 0; i < ResultDataGrid.ColumnCount; i++)
            {
                ResultDataGrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            ResultDataGrid.RowHeadersVisible = false;
            ResultDataGrid.BackgroundColor = Color.LightGray;

            //// Column
        //    ResultDataGrid.Columns[0].Name = "Axis";
            ResultDataGrid.Columns[0].Name = "Item No.";
            ResultDataGrid.Columns[1].Name = "Item Name";
            ResultDataGrid.Columns[2].Name = "Min";
            ResultDataGrid.Columns[3].Name = "Max";
            ResultDataGrid.Columns[4].Name = "Result";
          //  ResultDataGrid.Columns[5].Name = "#2 Result";
            ResultDataGrid.Columns[5].Name = "unit";

         //   ResultDataGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            ResultDataGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            ResultDataGrid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
            ResultDataGrid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopRight;
            ResultDataGrid.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopRight;
            ResultDataGrid.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopRight;
            ResultDataGrid.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

        //    ResultDataGrid.Columns[0].Width = 150;
            ResultDataGrid.Columns[0].Width = 70;
            ResultDataGrid.Columns[1].Width = 215;
            ResultDataGrid.Columns[2].Width = 70;
            ResultDataGrid.Columns[3].Width = 70;
            ResultDataGrid.Columns[4].Width = 100;
            ResultDataGrid.Columns[5].Width = 65;

            ResultDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ResultDataGrid.ColumnHeadersHeight = 28;

          
            ResultDataGrid.Rows.Clear();
            for (int i = 0; i < Spec.specList.Count; i++)
            {
                switch(Spec.specList[i].InspectionType)
                {
                    case InspType.Normal:
                    case InspType.MintoMax:
                        ResultDataGrid.Rows.Add(i + 1, Spec.specList[i].DisplayName, Spec.specList[i].MinSpec, Spec.specList[i].MaxSpec, "", Spec.specList[i].Unit);
                        break;
                    case InspType.OnlyMax:
                        ResultDataGrid.Rows.Add(i + 1, Spec.specList[i].DisplayName, "", Spec.specList[i].MaxSpec, "", Spec.specList[i].Unit);
                        break;
                    case InspType.OnlyMin:
                        ResultDataGrid.Rows.Add(i + 1, Spec.specList[i].DisplayName, Spec.specList[i].MinSpec, "", "", Spec.specList[i].Unit);
                        break;
                    case InspType.OKNG:
                        ResultDataGrid.Rows.Add(i + 1, Spec.specList[i].DisplayName, "", "", "", Spec.specList[i].Unit);
                        break;
                   
                      
                }

                ResultDataGrid.Rows[i].Visible = Convert.ToBoolean(Spec.specList[i].OnOff);
                for (int k = 0; k < ResultDataGrid.ColumnCount; k++) ResultDataGrid[k, i].Style.BackColor = Color.White;

                ResultDataGrid.Rows[i].Height = 22;
                ResultDataGrid.Rows[i].Resizable = DataGridViewTriState.False;
                ResultDataGrid.Rows[i].DefaultCellStyle.Font = new Font("Calibri", 10, FontStyle.Bold);
                ResultDataGrid[0, i].Style.Font = new Font("Calibri", 10, FontStyle.Bold);
                ResultDataGrid[2, i].Style.Font = new Font("Calibri", 10, FontStyle.Bold);
                ResultDataGrid[5, i].Style.Font = new Font("Calibri", 10, FontStyle.Italic);

                ResultDataGrid.ReadOnly = true;
            }

            //string old = string.Empty;/*ResultGrid.Rows[0].Cells[0].Value.ToString();*/
            //for (int i = 0; i < Spec.specList.Count; i++)
            //{
            //    if (ResultDataGrid.Rows[i].Visible)
            //    {
            //        string newKey = ResultDataGrid.Rows[i].Cells[0].Value.ToString();

            //        if (old != newKey)
            //            bColorChange = !bColorChange;
            //        if (bColorChange) for (int k = 0; k < ResultDataGrid.ColumnCount; k++) ResultDataGrid[k, i].Style.BackColor = Color.Lavender;
            //        else for (int k = 0; k < ResultDataGrid.ColumnCount; k++) ResultDataGrid[k, i].Style.BackColor = Color.White;

            //        if (old == newKey)
            //            ResultDataGrid.Rows[i].Cells[0].Style.ForeColor = ResultDataGrid.Rows[i].Cells[0].Style.BackColor;
            //        old = newKey;
            //    }
            //}
        }
        public void InitResult(int ch, string Item)
        {
            if (ResultDataGrid.InvokeRequired)
            {
                ResultDataGrid.BeginInvoke((MethodInvoker)delegate
                {
                    for (int i = 0; i < Spec.specList.Count; i++)
                    {
                        if (Spec.specList[i].Category == Item)
                        {
                            if (PassFails[ch].FirstFailIndex == i + 1)
                            {
                                PassFails[ch].FirstFail = "";
                                PassFails[ch].FirstFailIndex = 0;
                            }
                            PassFails[ch].Results[i].Val = double.MaxValue;
                            PassFails[ch].Results[i].msg = ""; PassFails[ch].Results[i].bPass = true;

                            ResultDataGrid[ch + 4, i].Value = "";
                            ResultDataGrid[ch + 4, i].Style.BackColor = Color.White;
                            ResultDataGrid[ch, i].Style.BackColor = Color.White;

                        }
                     
                    }
                  
                });
            }
            else
            {

                for (int i = 0; i < Spec.specList.Count; i++)
                {
                    if (Spec.specList[i].Category == Item)
                    {
                        if (PassFails[ch].FirstFailIndex == i + 1)
                        {
                            PassFails[ch].FirstFail = "";
                            PassFails[ch].FirstFailIndex = 0;
                        }
                        PassFails[ch].Results[i].Val = double.MaxValue;
                        PassFails[ch].Results[i].msg = ""; PassFails[ch].Results[i].bPass = true;

                        ResultDataGrid[ch + 4, i].Value = "";
                        ResultDataGrid[ch + 4, i].Style.BackColor = Color.White;
                        ResultDataGrid[ch, i].Style.BackColor = Color.White;
                    }
                  
                }
            }
            
            m_ChannelOn[ch] = true;
        }
        public void InitResult(int ch)
        {
          
            PassFails[ch].FirstFail = "";
            PassFails[ch].FirstFailIndex = 0;
            for (int i = 0; i < (int)SpecItem.Length; i++)
            {              
                PassFails[ch].Results[i].Val = double.MaxValue;
                PassFails[ch].Results[i].msg = ""; PassFails[ch].Results[i].bPass = true;
            }
        }

        public void ShowDataResultsInit(int ch)
        {
            if (ResultDataGrid.InvokeRequired)
            {
                ResultDataGrid.BeginInvoke((MethodInvoker)delegate
                {
                    InitResult(ch);
                    for (int i = 0; i < Spec.specList.Count; i++)
                    {
                        ResultDataGrid[ch + 4, i].Value = "";
                        ResultDataGrid[ch + 4, i].Style.BackColor = Color.White;
                        ResultDataGrid[ch, i].Style.BackColor = Color.White;
                    }
                });
            }
            else
            {
                InitResult(ch);
                for (int i = 0; i < Spec.specList.Count; i++)
                {
                    ResultDataGrid[ch + 4, i].Value = "";
                    ResultDataGrid[ch + 4, i].Style.BackColor = Color.White;
                    ResultDataGrid[ch, i].Style.BackColor = Color.White;
                }
            }

            if (lblFailList.InvokeRequired)
            {
                lblFailList.BeginInvoke((MethodInvoker)delegate
                {
                    lblFailList.Text = "";
                });
            }
            else lblFailList.Text = "";
            STATIC.FailNumber = "Fail No. : ";
        }
        public void AddLog(int ch, string msg)
        {
            STATIC.SaveLogData += msg + "\r\n";
            ViewLog[ch].Log(msg);
        }
        public void AddChart(int ch, string name, List<double> time = null, List<double> Stroke = null, double MaxtiltX = 0, double MaxtiltY = 0, double[] refArr = null)
        {
            while (ChartTop[ch].IsFalg)
                Process.Wait(10);

            int CodeRange = 0;

            foreach (var Cal in CalList[ch])
            {
                if (Cal.Name == name)
                {
                    switch (name)
                    {
                        case "OIS X Scan":


                            CalResult tmpRes = new CalResult(name);
                            for (int i = 0; i < Cal.CodeX.Count; i++)
                            {
                                tmpRes.CodeX.Add(Cal.CodeX[i]);
                                tmpRes.Current.Add(Cal.Current[i]);
                                tmpRes.StrokeX.Add(Cal.StrokeX[i]);
                                tmpRes.HallX.Add(Cal.HallX[i]);
                            }


                            CodeRange = Condition.iXPlotRange;
                            //Stroke
                            if (ChartTop[ch].C.InvokeRequired)
                            {
                                ChartTop[ch].C.BeginInvoke((MethodInvoker)delegate
                                {
                                    ChartTop[ch].C.Series[0].Points.Clear();

                                    for (int i = 0; i < tmpRes.CodeX.Count; i++)
                                    {
                                        if (tmpRes.CodeX[i] >= OISXCenter - CodeRange && tmpRes.CodeX[i] <= OISXCenter + CodeRange)
                                        {
                                            ChartTop[ch].C.Series[0].Points.AddXY(tmpRes.CodeX[i] + 2048, tmpRes.StrokeX[i]); //  stroke
                                            ChartTop[ch].C.Series[3].Points.AddXY(tmpRes.CodeX[i] + 2048, tmpRes.Current[i]); //  current
                                            ChartTop[ch].C.Series[6].Points.AddXY(tmpRes.CodeX[i] + 2048, (tmpRes.HallX[i] + 2048) / 10); //  hall
                                        }
                                    }
                                });
                            }
                            //Tilt
                            //if (ChartBtm[ch].C.InvokeRequired)
                            //{
                            //    ChartBtm[ch].C.BeginInvoke((MethodInvoker)delegate
                            //    {
                            //        for (int i = 2; i < Cal.CodeX.Count; i++)
                            //        {
                            //            if (Cal.CodeX[i] >= OISCenter - CodeRange && Cal.CodeX[i] <= OISCenter + CodeRange)
                            //            {
                            //                //ChartBtm[ch].C.Series[0].Points.AddXY(Cal.CodeX[i], Cal.TiltX[i]); //  Tilt 
                            //                //ChartBtm[ch].C.Series[1].Points.AddXY(Cal.CodeX[i], Cal.TiltY[i]); //  Tilt 
                            //                //ChartBtm[ch].C.Series[2].Points.AddXY(Cal.CodeX[i], Cal.TiltZ[i]); //  Tilt 
                            //            }
                            //        }
                            //    });
                            //}
                            break;
                        case "OIS Y Scan":


                            tmpRes = new CalResult(name);
                            for (int i = 0; i < Cal.CodeY.Count; i++)
                            {
                                tmpRes.CodeY.Add(Cal.CodeY[i]);
                                tmpRes.Current.Add(Cal.Current[i]);
                                tmpRes.StrokeY.Add(Cal.StrokeY[i]);
                                tmpRes.HallY.Add(Cal.HallY[i]);
                            }

                            CodeRange = Condition.iYPlotRange;
                            //Stroke
                            if (ChartTop[ch].C.InvokeRequired)
                            {
                                ChartTop[ch].C.BeginInvoke((MethodInvoker)delegate
                                {
                                    for (int i = 0; i < tmpRes.CodeY.Count; i++)
                                    {
                                        if (tmpRes.CodeY[i] >= OISYCenter - CodeRange && tmpRes.CodeY[i] <= OISYCenter + CodeRange)
                                        {
                                            ChartTop[ch].C.Series[1].Points.AddXY(tmpRes.CodeY[i] + 2048, tmpRes.StrokeY[i]); //  stroke
                                                                                                                  //   ChartTop[ch].C.Series[9].Points.AddXY(Cal.CodeY1[i], Cal.StrokeY1[i]); //  stroke 1
                                                                                                                  // ChartTop[ch].C.Series[10].Points.AddXY(Cal.CodeY2[i], Cal.StrokeY2[i]); //  stroke 2
                                            ChartTop[ch].C.Series[4].Points.AddXY(tmpRes.CodeY[i] + 2048, tmpRes.Current[i]); //  current
                                            ChartTop[ch].C.Series[7].Points.AddXY(tmpRes.CodeY[i] + 2048, (tmpRes.HallY[i] + 2048) / 10); //  hall
                                        }
                                    }
                                });
                            }
                            //Tilt
                            //if (ChartBtm[ch].C.InvokeRequired)
                            //{
                            //    ChartBtm[ch].C.BeginInvoke((MethodInvoker)delegate
                            //    {
                            //        for (int i = 2; i < Cal.CodeY.Count; i++)
                            //        {
                            //            if (Cal.CodeY[i] >= OISCenter - CodeRange && Cal.CodeY[i] <= OISCenter + CodeRange)
                            //            {
                            //                //ChartBtm[ch].C.Series[3].Points.AddXY(Cal.CodeY1[i], Cal.TiltX[i]); //  Tilt 
                            //                //ChartBtm[ch].C.Series[4].Points.AddXY(Cal.CodeY1[i], Cal.TiltY[i]); //  Tilt 
                            //                //ChartBtm[ch].C.Series[5].Points.AddXY(Cal.CodeY1[i], Cal.TiltZ[i]); //  Tilt 
                            //            }
                            //        }
                            //    });
                            //}
                            break;
                        case "AF Scan":


                            tmpRes = new CalResult(name);
                            for (int i = 0; i < Cal.CodeZ.Count; i++)
                            {
                                tmpRes.CodeZ.Add(Cal.CodeZ[i]);
                                tmpRes.Current.Add(Cal.Current[i]);
                                tmpRes.StrokeZ.Add(Cal.StrokeZ[i]);
                                tmpRes.HallZ.Add(Cal.HallZ[i]);
                                tmpRes.TiltX.Add(Cal.TiltX[i]);
                                tmpRes.TiltY.Add(Cal.TiltY[i]);
                            }

                            CodeRange = Condition.iAFPlotRange;
                            //Stroke
                            if (ChartTop[ch].C.InvokeRequired)
                            {
                                ChartTop[ch].C.BeginInvoke((MethodInvoker)delegate
                                {
                                    for (int i = 0; i < tmpRes.CodeZ.Count; i++)
                                    {
                                        if (tmpRes.CodeZ[i] >= AFCenter - CodeRange && tmpRes.CodeZ[i] <= AFCenter + CodeRange)
                                        {
                                            ChartTop[ch].C.Series[2].Points.AddXY(tmpRes.CodeZ[i], tmpRes.StrokeZ[i]); //  stroke
                                            ChartTop[ch].C.Series[5].Points.AddXY(tmpRes.CodeZ[i], tmpRes.Current[i]); //  current
                                            ChartTop[ch].C.Series[8].Points.AddXY(tmpRes.CodeZ[i], tmpRes.HallZ[i] / 10); //  hall
                                        }
                                    }
                                });
                            }
                            //Tilt
                            if (tiltChart[ch].InvokeRequired)
                            {
                                tiltChart[ch].BeginInvoke((MethodInvoker)delegate
                                {
                                    List<double> xs_List = new List<double>();
                                    List<double> ys_List = new List<double>();

                                  

                                    for (int i = 2; i < tmpRes.CodeZ.Count; i++)
                                    {
                                        if (tmpRes.CodeZ[i] >= Condition.TiltMinCode && tmpRes.CodeZ[i] <= Condition.TiltMaxCode)
                                        {
                                            xs_List.Add(tmpRes.TiltX[i]);
                                            ys_List.Add(tmpRes.TiltY[i]);
                                            //xs[i] = Cal.TiltX[i];
                                            //ys[i] = Cal.TiltY[i];
                                             
                                        }
                                    }
                                    double[] xs = xs_List.ToArray();
                                    double[] ys = ys_List.ToArray();
                                    tiltChart[ch].SetPoints(xs, ys, Color.Lime);
                                    tiltChart[ch].SetPoint(xs[0], ys[0], Color.LightGray);
                                    tiltChart[ch].SetPoint(xs[xs.Length - 1], ys[ys.Length - 1], Color.LightGray);
                                    tiltChart[ch].SetPoint(MaxtiltX, MaxtiltY, Color.Red);
                                    tiltChart[ch].SetPoint(refArr[0], refArr[1], Color.Orange);
                                  
                                });
                            }
                            break;
                        case "AF Settling":
                         

                            //Stroke
                            if (ChartBtm[ch].C.InvokeRequired)
                            {
                                ChartBtm[ch].C.BeginInvoke((MethodInvoker)delegate
                                {
                                    for (int i = 0; i < time.Count; i++)
                                    {
                                        ChartBtm[ch].C.Series[0].Points.AddXY(time[i], Stroke[i]); //  stroke
                                    }
                                });
                            }
                            //Tilt
                            //if (ChartBtm[ch].C.InvokeRequired)
                            //{
                            //    ChartBtm[ch].C.BeginInvoke((MethodInvoker)delegate
                            //    {
                            //        for (int i = 2; i < Cal.Time.Count; i++)
                            //        {
                            //            ChartBtm[ch].C.Series[6].Points.AddXY(Cal.Time[i] * 1000, Cal.TiltX[i]); //  Tilt 
                            //            ChartBtm[ch].C.Series[7].Points.AddXY(Cal.Time[i] * 1000, Cal.TiltY[i]); //  Tilt 
                            //            ChartBtm[ch].C.Series[8].Points.AddXY(Cal.Time[i] * 1000, Cal.TiltZ[i]); //  Tilt 
                            //        }
                            //    });
                            //}
                            break;

                    }
                    ChartSet(ch, name);
                }
            }
        }
        private void ChartSet(int ch, string name)
        {
            //StrokeChart
            if (ChartTop[ch].C.InvokeRequired)
            {
                ChartTop[ch].C.BeginInvoke((MethodInvoker)delegate
                {
                    ChartTop[ch].C.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
                    ChartTop[ch].C.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
                    ChartTop[ch].C.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
                    ChartTop[ch].C.ChartAreas[0].AxisY2.MinorGrid.Enabled = false;

                    if (name.Contains("Settling"))
                    {
                        //ChartTop[ch].C.Titles[0].Text = "Stroke vs Time";
                        //ChartTop[ch].C.ChartAreas[0].AxisX.Minimum = 0;
                        //ChartTop[ch].C.ChartAreas[0].AxisX.Maximum = 600;
                        //ChartTop[ch].C.ChartAreas[0].AxisX.Interval = 100;
                        //ChartTop[ch].C.ChartAreas[0].AxisX.MajorGrid.Interval = 100;
                    }
                    else
                    {
                        ChartTop[ch].C.Titles[0].Text = "Stroke vs Code";
                        ChartTop[ch].C.ChartAreas[0].AxisX.Minimum = 0;
                        ChartTop[ch].C.ChartAreas[0].AxisX.Maximum = 4100;
                        ChartTop[ch].C.ChartAreas[0].AxisX.Interval = 512;
                        ChartTop[ch].C.ChartAreas[0].AxisX.MajorGrid.Interval = 512;
                    }


                    ChartTop[ch].C.ChartAreas[0].AxisY.Minimum = -500;
                    ChartTop[ch].C.ChartAreas[0].AxisY.Maximum = 500;
                    ChartTop[ch].C.ChartAreas[0].AxisY.Interval = 100;
                    ChartTop[ch].C.ChartAreas[0].AxisY.MajorGrid.Interval = 100;

                    ChartTop[ch].C.ChartAreas[0].AxisY2.Minimum = -50;
                    ChartTop[ch].C.ChartAreas[0].AxisY2.Maximum = 410;
                    ChartTop[ch].C.ChartAreas[0].AxisY2.Interval = 45;

                    ChartTop[ch].C.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
                    ChartTop[ch].C.ChartAreas[0].AxisY2.LabelStyle.ForeColor = Color.DarkGreen;
                    ChartTop[ch].C.ChartAreas[0].AxisY2.LabelStyle.Font = new Font("Calibri", 9, FontStyle.Bold);

                    ChartTop[ch].IsFalg = false;
                });
            }
            //settle Chart
            if (ChartBtm[ch].C.InvokeRequired)
            {
                ChartBtm[ch].C.BeginInvoke((MethodInvoker)delegate
                {
                    ChartBtm[ch].C.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
                    ChartBtm[ch].C.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
                    //ChartBtm[ch].C.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
                    //ChartBtm[ch].C.ChartAreas[0].AxisY2.MinorGrid.Enabled = false;


                    if (name.Contains("Settling"))
                    {
                        ChartBtm[ch].C.Titles[0].Text = "Time to Settle";
                        ChartBtm[ch].C.ChartAreas[0].AxisX.Minimum = 0;
                        ChartBtm[ch].C.ChartAreas[0].AxisX.Maximum = 110;
                        ChartBtm[ch].C.ChartAreas[0].AxisX.Interval = 10;
                        ChartBtm[ch].C.ChartAreas[0].AxisX.MajorGrid.Interval = 10;
                    }
                    else
                    {
                        //ChartBtm[ch].C.Titles[0].Text = "Tilt vs Code";
                        //ChartBtm[ch].C.ChartAreas[0].AxisX.Minimum = 0;
                        //ChartBtm[ch].C.ChartAreas[0].AxisX.Maximum = 4100;
                        //ChartBtm[ch].C.ChartAreas[0].AxisX.Interval = 512;
                        //ChartBtm[ch].C.ChartAreas[0].AxisX.MajorGrid.Interval = 512;
                    }

                    ChartBtm[ch].C.ChartAreas[0].AxisY.Minimum = -10;
                    ChartBtm[ch].C.ChartAreas[0].AxisY.Maximum = 300;
                    ChartBtm[ch].C.ChartAreas[0].AxisY.Interval = 10;
                    ChartBtm[ch].C.ChartAreas[0].AxisY.MajorGrid.Interval = 10;

                    //ChartBtm[ch].C.ChartAreas[0].AxisY2.Minimum = -200;
                    //ChartBtm[ch].C.ChartAreas[0].AxisY2.Maximum = 200;
                    //ChartBtm[ch].C.ChartAreas[0].AxisY2.Interval = 40;
                    //ChartBtm[ch].C.ChartAreas[0].AxisY2.MajorGrid.Interval = 40;

                    //ChartBtm[ch].C.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
                    //ChartBtm[ch].C.ChartAreas[0].AxisY2.LabelStyle.ForeColor = Color.DarkGreen;
                    //ChartBtm[ch].C.ChartAreas[0].AxisY2.LabelStyle.Font = new Font("Calibri", 9, FontStyle.Bold);

                    ChartBtm[ch].IsFalg = false;
                });
            }
        }
      
        public void ClearChart()
        {
            if (ChartTop[0].C.InvokeRequired)
            {
                ChartTop[0].C.BeginInvoke((MethodInvoker)delegate
                {
                    for (int i = 0; i < ChartTop[0].C.Series.Count; i++)
                    {
                        ChartTop[0].C.Series[i].Points.Clear();
                    }
                    ChartTop[0].C.Series[0].Points.AddXY(0, 0);
                });
            }
            else
            {
                for (int i = 0; i < ChartTop[0].C.Series.Count; i++)
                {
                    ChartTop[0].C.Series[i].Points.Clear();
                }
                ChartTop[0].C.Series[0].Points.AddXY(0, 0);
            }
            if (ChartBtm[0].C.InvokeRequired)
            {
                ChartBtm[0].C.BeginInvoke((MethodInvoker)delegate
                {
                    for (int i = 0; i < ChartBtm[0].C.Series.Count; i++)
                    {
                        ChartBtm[0].C.Series[i].Points.Clear();
                    }
                    ChartBtm[0].C.Series[0].Points.AddXY(0, 0);
                });
            }
            else
            {
                for (int i = 0; i < ChartBtm[0].C.Series.Count; i++)
                {
                    ChartBtm[0].C.Series[i].Points.Clear();
                }
                ChartBtm[0].C.Series[0].Points.AddXY(0, 0);
            }

            if (tiltChart[0].InvokeRequired)
            {
                tiltChart[0].BeginInvoke((MethodInvoker)delegate
                {
                    tiltChart[0].ClearPoint();
                });
            }
            else
            {
                tiltChart[0].ClearPoint();
            }
        }
        public void RunTest(int InspType) // 0:btn 1:switch 2:handler
        {
            I2CMonitorStartFlag = false;
            if (Model.MCType != "Posture_S") STATIC.PosturePos = string.Empty;
            if (Option.BarcodeUse)
            {
                if(STATIC.ActID == string.Empty || STATIC.ActID.Contains("Error"))
                {
                    errMsg[0] = "Barcode Error";
                    if (Model.MCType == "Slave" || Model.MCType == "Posture_S")
                        STATIC.TcpConn.SendMessage($"Res.{errMsg[0]}", STATIC.TCPCOnState);
                    STATIC.fManage.SetInforViewOnComm(errMsg[0], 0);
                    STATIC.BarcodeConn.SendMessage2("LON\r", STATIC.BarcodeConState);

                    return;
                }
            }

            if (RepeatRun == 1 || InspType != 0)
            {
                CurrentRun = 1;
                if (Dln.IsRun) return;

                if (!Dln.IsRun)
                {
                    Dln.IsRun = true;
                    Task.Factory.StartNew(() => LoadTestUnload(0, InspType));
                }
            }
            else
            {
                CurrentRun = 1;
                if (Dln.IsRun) return;
                Dln.IsRun = true;
                while (true)
                {
                    //   ClearChart();
                    I2CMonitorStartFlag = false;
                    foreach (var l in ViewLog) l.Clear();

                    Task tasks = null;
                    tasks = Task.Factory.StartNew(() => LoadTestUnload(0, InspType));
                    Task.WaitAll(tasks);

                    if (CurrentRun >= RepeatRun || SuddenStop) break;
                    CurrentRun++;
                    Process.Wait(1500);
                }
            }
        }


        public void LoadSeq()
        {
            try
            {
                Stopwatch st = new Stopwatch();
               
                Dln.CoverUp();
                Thread.Sleep(700);
                Dln.LoadSocket();
                if (Option.SocketSensorUse)
                {
                    st.Start();
                    while (!Dln.GetGpioStatus(12) || Dln.GetGpioStatus(13))
                    {
                        if (st.ElapsedMilliseconds > 3000) { MessageBox.Show("Check Socket Sensor Status"); return; }
                        Thread.Sleep(10);
                    }
                    st.Stop();
                    Thread.Sleep(300);
                }
                else Thread.Sleep(2000);
                Dln.CoverDn();

                Thread.Sleep(500);
                Dln.PowerOnOff(0, true);
                Wait(200);
            }
            catch
            { }
        }
        public void UnloadSeq()
        {
            try
            {
                Dln.PowerOnOff(0, false);
                Wait(200);
                Stopwatch st = new Stopwatch();             
                Dln.CoverUp();
                Thread.Sleep(700);
                Dln.UnloadSocket();
                if (Option.SocketSensorUse)
                {
                    st.Start();
                    while (Dln.GetGpioStatus(12) || !Dln.GetGpioStatus(13))
                    {
                        if (st.ElapsedMilliseconds > 3000) { MessageBox.Show("Check Socket Sensor Status"); return; }
                        Thread.Sleep(10);
                    }
                    st.Stop();
                }
                else Thread.Sleep(500);
            }
            catch
            { }
        }


        void RunPosture(int InspType)
        {
            try
            {

                bool b = false;
                double[] deg = new double[2];
                Task wait = null;
                int TotalPosCnt = STATIC.fMotion.motData.MotionPosList.Count;
                int Curpos = 1;
                while (true)
                {
                    while (!STATIC.IsPostureS_End) Thread.Sleep(10);
                    if (STATIC.TCPCOnState) STATIC.IsPostureS_End = false;
                    (b, deg, STATIC.PosturePos) = STATIC.fMotion.MoveSelectedPosition(Curpos - 1);

                    if (!b)
                    {
                        STATIC.IsPostureS_End = true;
                        if (Curpos >= TotalPosCnt) break;
                        Curpos++; continue;
                    }


                    wait = Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            Thread.Sleep(10);
                            if (((int)STATIC.fMotion.AxisCurrrent[0] + 1 >= deg[0] && (int)STATIC.fMotion.AxisCurrrent[1] + 1 >= deg[1] && (int)STATIC.fMotion.AxisCurrrent[0] - 1 <= deg[0] && (int)STATIC.fMotion.AxisCurrrent[1] - 1 <= deg[1]))
                            {
                                Thread.Sleep(1000);
                                break;
                            }
                            if (Dln.IsEMG || /*Dln.IsStop || Dln.IsSafety ||*/ EMGState || StopState || SafetyState) break;
                        }
                    });

                    Task.WaitAll(wait);
                    if (Dln.IsEMG || /*Dln.IsStop || Dln.IsSafety ||*/ EMGState || StopState || SafetyState)
                    {
                        STATIC.fMotion.EMGStop();
                        if (Model.MCType == "Posture_M")
                        {
                            Dln.IOOnOff(PostureIO.RED_L, true);
                            Dln.IOOnOff(PostureIO.ORANGE_L, false);
                            Dln.IOOnOff(PostureIO.GREEN_L, false);
                            if (EMGState || SafetyState) Dln.IOOnOff(PostureIO.BUZZER, true);

                        }
                        SuddenStop = true;
                       
                        return;
                    }
                   
                    STATIC.TcpConn.SendMessage($"Start_P,{STATIC.PosturePos}", STATIC.TCPCOnState);

                    foreach (var l in ViewLog) l.Clear();
                    RunStart?.Invoke(null, 0);
                    Process_Start(0, Curpos);
                    RunEnd?.Invoke(null, InspType);
                    if (Curpos >= TotalPosCnt || SuddenStop) break;
                    Curpos++;
                }
                while (!STATIC.IsPostureS_End) Thread.Sleep(10);
                (b, deg, STATIC.PosturePos) = STATIC.fMotion.MoveSelectedPosition(-1);
                wait = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(10);
                        if (((int)STATIC.fMotion.AxisCurrrent[0] + 1 >= deg[0] && (int)STATIC.fMotion.AxisCurrrent[1] + 1 >= deg[1] && (int)STATIC.fMotion.AxisCurrrent[0] - 1 <= deg[0] && (int)STATIC.fMotion.AxisCurrrent[1] - 1 <= deg[1]))
                        {
                            Thread.Sleep(1000);
                            break;
                        }

                        if (Dln.IsEMG /*|| Dln.IsStop || Dln.IsSafety */|| EMGState || StopState || SafetyState) break;

                    }
                });

                Task.WaitAll(wait);
                if (Dln.IsEMG || /*Dln.IsStop || Dln.IsSafety || */EMGState || StopState || SafetyState)
                {
                    STATIC.fMotion.EMGStop();
                    if (Model.MCType == "Posture_M")
                    {
                        Dln.IOOnOff(PostureIO.RED_L, true);
                        Dln.IOOnOff(PostureIO.ORANGE_L, false);
                        Dln.IOOnOff(PostureIO.GREEN_L, false);
                        if (EMGState || SafetyState) Dln.IOOnOff(PostureIO.BUZZER, true);

                    }
                    SuddenStop = true;
                   
                    return;
                }
                if (Model.MCType == "Posture_M")
                {
                    Dln.IOOnOff(PostureIO.RED_L, false);
                    Dln.IOOnOff(PostureIO.ORANGE_L, true);
                    Dln.IOOnOff(PostureIO.GREEN_L, false);
                    Dln.IOOnOff(PostureIO.BUZZER, false);

                }
              
            }
            catch
            {
            //    m__G.fGraph.mDriverIC.IsRunning = false;
            }

        }

        public void LoadTestUnload(int port, int InspType) //inspType 0:btn 1:switch 2:handler
        {
            try
            {
                int ch = port * 2;

                if(Model.MCType == "Posture_M")
                {
                    if (Dln.IsEMG ||/* Dln.IsStop || Dln.IsSafety ||*/ EMGState || StopState || SafetyState)
                    { Dln.IsRun = false; return; }
                    if (Model.MCType == "Posture_M")
                    {
                        Dln.IOOnOff(PostureIO.RED_L, false);
                        Dln.IOOnOff(PostureIO.ORANGE_L, false);
                        Dln.IOOnOff(PostureIO.GREEN_L, true);
                        Dln.IOOnOff(PostureIO.BUZZER, false);

                    }


                    (bool b, double[] deg, string s) = STATIC.fMotion.MoveSelectedPosition(-1);

                    Task wait = Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            Thread.Sleep(10);
                            if (((int)STATIC.fMotion.AxisCurrrent[0] == deg[0] && (int)STATIC.fMotion.AxisCurrrent[1] == deg[1]))

                            {
                                break;
                            }
                            if (Dln.IsEMG ||/* Dln.IsStop || Dln.IsSafety ||*/ EMGState || StopState || SafetyState)
                            {
                                break;
                            }

                        }
                    });
                    Task.WaitAll(wait);
                    if (Dln.IsEMG || /*Dln.IsStop || Dln.IsSafety ||*/ EMGState || StopState || SafetyState)
                    {
                        STATIC.fMotion.EMGStop();
                        if (Model.MCType == "Posture_M")
                        {
                            Dln.IOOnOff(PostureIO.RED_L, true);
                            Dln.IOOnOff(PostureIO.ORANGE_L, false);
                            Dln.IOOnOff(PostureIO.GREEN_L, false);
                            if (EMGState || SafetyState) Dln.IOOnOff(PostureIO.BUZZER, true);

                        }

                        SuddenStop = true;
                     
                        Dln.IsRun = false;
                        return;
                    }
                }

                if (Model.MCType != "Posture_S")
                    LoadSeq();
                Process.Wait(100);
                
               

                if(Model.MCType == "Posture_M" && InspType == 1)
                {
                    Task t = Task.Factory.StartNew(() =>
                    {
                        RunPosture(InspType);

                    });
                    Task.WaitAll(t);

                }
                else
                {
                    RunStart?.Invoke(null, port);
                    Process_Start(port, 1);
                    RunEnd?.Invoke(null, InspType);
                }
                if (Model.MCType == "Posture_M")
                {
                    if (!Dln.IsEMG && !Dln.IsStop && !Dln.IsSafety && !EMGState && !StopState && !SafetyState)
                    { if (InspType != 2) UnloadSeq(); }
                }
                else
                {
                    if (InspType != 2 && Model.MCType != "Posture_S") UnloadSeq();
                }
                
                Dln.IsRun = false;
                StartI2CMonitor();
                if (Option.BarcodeUse)
                    STATIC.BarcodeConn.SendMessage2("LON\r", STATIC.BarcodeConState);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Dln.IsRun = false;
            }
        }
        void SaveLogData()
        {
            string dateDir = STATIC.CreateDateDir();
            dateDir += "LogData\\";
            if (!Directory.Exists(dateDir))
                Directory.CreateDirectory(dateDir);
            for (int j = 0; j < ChannelCnt; j++)
            {

                string path = string.Format("{0}{1}_{2}.txt", dateDir, m_StrIndex[0], $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s");

                if (path != "")
                {
                    string FilePath = path;
                    //if (!File.Exists(FilePath)) return;
                    StreamWriter sw = new StreamWriter(FilePath);
                    sw.WriteLine(STATIC.SaveLogData);
                    sw.Close();
                }
            }
        }

        public void Process_Start(int port, int PostureInspIndex)
        {
            while (isI2cMonitoring) Thread.Sleep(10);
            STATIC.SaveLogData = string.Empty;
            bool isI2cFail = false;
            if (Option.DryRunMode) { Thread.Sleep(40000); return; }
            int index = Rcp.RetryCnt.RetryOption.FindIndex(x => x.InspName == "All");
            int LoopCnt = 1 + Rcp.RetryCnt.RetryOption[index].Count;
            STATIC.Rcp.tt.CurrentST = 0;
            for (int Loop = 0; Loop < LoopCnt; Loop++)
            {
                try
                {
                    STATIC.I2CFailcnt = 0;
                    STATIC.LogDate = DateTime.Now;
                    AFRatedMinMax = new double[3];
                    XHATPOSAF = new int[2];
                    YHATPOSAF = new int[2];
                    XHATPOSOIS = new int[2];
                    YHATPOSOIS = new int[2];
                    AFOISVerifyXPOSOIS = new int[2];
                    AFOISVerifyYPOSOIS = new int[2];
                    ShowDataResultsInit(0);
                  
                //    Dln.PowerOnOff(port, true);
                    Thread.Sleep(500);
                    m__G.oCam[port].ResetmCpXY();
                    int ch = port * 2;
                    DrvIC.FRAModeDisable(ch);
                    DrvIC.AF_ICReset(ch);
                    SinewaveXMaxDiff = 0;
                    SinewaveYMaxDiff = 0;
                    RingingXStabilizer = 0;
                    RingingYStabilizer = 0;

                    AFCurrentMinMax = new double[2];
                    OISXCurrentMinMax = new double[2];
                    OISYCurrentMinMax = new double[2];
                    g_IME = new int[2];
                    

                  
                    //      Dln.ReadArray(0, DrvIC.XSlaveAddr, 0xE5, b);
                  //  AddLog(ch, $"AF Best Pos = {Condition.AFBestPos}");
                    //  BestAFPos = b[0] << 4;
                    //  if (BestAFPos == 0) BestAFPos = 2048;
                    int count = Condition.ToDoList.Count;
                    if (count == 0)
                    {
                        for (int i = ch; i < ch + ChannelCnt; i++)
                            errMsg[i] = "Test Item is Empty";
                        return;
                    }
                    for (int k = ch; k < ch + ChannelCnt; k++)
                    {
                        m_ChannelOn[k] = true;
                        errMsg[k] = "";
                        PassFails[k].FirstFailIndex = 0;
                    }


                    if (Dln.ReadByteNull(ch, DrvIC.AF_Addr, 0x03, 1) == null) m_ChannelOn[ch] = false;
                    if (Dln.ReadByteNull(ch, DrvIC.OIS_Addr, 0x6024, 2) == null) m_ChannelOn[ch] = false;


                    for (int k = ch; k < ch + ChannelCnt; k++)
                    {
                        if (!m_ChannelOn[k])
                        {
                            errMsg[k] = "I2C Fail";
                            AddLog(k, "I2C Fail");
                            isI2cFail = true;
                        }
                    }

                    if (!isI2cFail)
                    {
                        LEDs_All_On(0, true);
                        Thread.Sleep(100);
                        FindResult res = Measure();

                        if (res.cx[0] == 0 || res.cy[0] == 0 || res.cz[0] == 0)
                            m_ChannelOn[ch] = false;

                        //OpenLoopMoveSeq
                        Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);
                        byte backData = Dln.ReadByte(ch, DrvIC.AF_Addr, 0x0B, 1);
                        Dln.WriteByte(ch, DrvIC.AF_Addr, 0x1A, 1, 0x00);
                        Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, (byte)(backData & 0x7F));
                        Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x7B);
                        DrvIC.AFOnOff(ch, true);
                        DrvIC.AFMoveOL(ch, 0);
                        Wait(100);
                        res = Measure();
                        if (res.cx[0] == 0 || res.cy[0] == 0 || res.cz[0] == 0)
                            m_ChannelOn[ch] = false;
                        DrvIC.AFMoveOL(ch, 4095);
                        Wait(100);
                        res = Measure();
                        if (res.cx[0] == 0 || res.cy[0] == 0 || res.cz[0] == 0)
                            m_ChannelOn[ch] = false;

                        DrvIC.AFOnOff(ch, false);
                        Wait(5);
                        Dln.WriteByte(ch, DrvIC.AF_Addr, 0x0B, 1, backData);
                        Dln.WriteByte(ch, DrvIC.AF_Addr, 0xA6, 1, 0x00);

                        //DrvIC.OISOnOff(ch, true);
                        //DrvIC.OISOnOff(ch, false);
                        //Wait(10);
                        //DrvIC.OISMoveOL(ch, 1, 0);
                        //DrvIC.OISMoveOL(ch, 0, -8192);
                        //Wait(100);
                        //res = Measure();
                        //if (res.cx[0] == 0 || res.cy[0] == 0 || res.cz[0] == 0)
                        //    m_ChannelOn[ch] = false;
                        //DrvIC.OISMoveOL(ch, 0, 8191);
                        //Wait(100);
                        //res = Measure();
                        //if (res.cx[0] == 0 || res.cy[0] == 0 || res.cz[0] == 0)
                        //    m_ChannelOn[ch] = false;
                        //DrvIC.OISMoveOL(ch, 0, 0);
                        //DrvIC.OISMoveOL(ch, 1, -8192);
                        //Wait(100);
                        //res = Measure();
                        //if (res.cx[0] == 0 || res.cy[0] == 0 || res.cz[0] == 0)
                        //    m_ChannelOn[ch] = false;
                        //DrvIC.OISMoveOL(ch, 1, 8191);
                        //Wait(100);
                        //res = Measure();
                        //if (res.cx[0] == 0 || res.cy[0] == 0 || res.cz[0] == 0)
                        //    m_ChannelOn[ch] = false;

                        LEDs_All_On(0, false);

                        for (int k = ch; k < ch + ChannelCnt; k++)
                        {
                            if (!m_ChannelOn[k])
                            {
                                errMsg[k] = "Socket Empty\r\nVision Check";
                                AddLog(k, "Socket Empty\r\nVision Check");
                            }
                        }
                    }

  

                    if (errMsg[ch] != "")
                    {
                        return;
                    }

                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    bool loopContinue = true;

                    int todoCnt = 0;
                    SuddenStop = false;

                    while (todoCnt < count)
                    {
                        string testItem = Condition.ToDoList[todoCnt];
                        if(Model.MCType == "Posture_M")
                        {
                            if (PostureInspIndex > 1 && (testItem == "AF HallCalibration" || testItem == "OIS HallCalibration" || testItem == "OIS FW Donwload" 
                                || testItem == "AF OIS XTalk Calibration" || testItem == "OIS Linear/Crosstalk Calibration" || testItem == "OIS Decenter Calibration"))
                            { todoCnt++; continue; }

                        }
                            
                        Process_Function(port, testItem);

                        if (errMsg[ch] != "")
                        {
                            loopContinue = false;
                            AddLog(ch, errMsg[ch]);

                        }
                        if (SuddenStop)
                        {
                            loopContinue = false;
                            errMsg[ch] = "User Stop !";
                            AddLog(ch, errMsg[ch]);

                        }

                        if (!loopContinue) break;
                        else todoCnt++;
                        Process.Wait(100);
                    }
                    LEDs_All_On(port, false);

                    double ellipse = (double)sw.ElapsedMilliseconds / 1000;
                    sw.Stop();

                    yield.LastSampleNum++;

                    for (int k = ch; k < ch + ChannelCnt; k++)
                    {
                        AddLog(k, string.Format("Total Test Time\t{0:0.000} sec", ellipse));
                        PassFails[k].TotalTime = ellipse.ToString("F3");
                        STATIC.Rcp.tt.St += ellipse;
                        STATIC.Rcp.tt.CurrentST += ellipse;
                    }

                    if (!SuddenStop)
                    {
                        if(LoopCnt > 1)
                        {
                            if (errMsg[0] == "" && PassFails[0].FirstFailIndex == 0)
                            {

                                if (Option.WriteResultToDriverIC)
                                {
                                    if (errMsg[0] == "" && PassFails[0].FirstFailIndex == 0)
                                        WriteUserMem(ch, 0x02);
                                    else WriteUserMem(ch, 0x09);
                                }
                                else
                                {
                                    if (!Option.BarcodeUse)
                                    {
                                        for (int i = 0; i < 5; i++)
                                            STATIC.ActID_Memory[i] = Dln.ReadByte(ch, DrvIC.AF_Addr, 0xF5 + i, 1);
                                    }

                                }
                                WriteResult(port);
                                SaveLogData();
                                SetFailList(ch);

                            }
                            else
                            {
                                if(Loop == LoopCnt - 1)
                                {
                                    if (Option.WriteResultToDriverIC)
                                    {
                                        if (errMsg[0] == "" && PassFails[0].FirstFailIndex == 0)
                                            WriteUserMem(ch, 0x02);
                                        else WriteUserMem(ch, 0x09);
                                    }
                                    else
                                    {
                                        if (!Option.BarcodeUse)
                                        {
                                            for (int i = 0; i < 5; i++)
                                                STATIC.ActID_Memory[i] = Dln.ReadByte(ch, DrvIC.AF_Addr, 0xF5 + i, 1);
                                        }

                                    }
                                    WriteResult(port);
                                    SaveLogData();
                                    SetFailList(ch);

                                }
                                else
                                {
                                    AddLog(ch, $"Fail Retry =  {errMsg[0]}");
                                    yield.LastSampleNum--;
                                }
                            }
                        }
                        else
                        {
                            
                            if (Option.WriteResultToDriverIC)
                            {
                                if (errMsg[0] == "" && PassFails[0].FirstFailIndex == 0)
                                    WriteUserMem(ch, 0x02);
                                else WriteUserMem(ch, 0x09);
                            }
                            else
                            {
                                if (!Option.BarcodeUse)
                                {
                                    for (int i = 0; i < 5; i++)
                                        STATIC.ActID_Memory[i] = Dln.ReadByte(ch, DrvIC.AF_Addr, 0xF5 + i, 1);
                                }

                            }
                            WriteResult(port);
                            SaveLogData();
                            SetFailList(ch);
                        }
                    }
                    else { /*Dln.PowerOnOff(port, false);*/  STATIC.Rcp.tt.Count++; return; }
                   // Dln.PowerOnOff(port, false);
                }
                catch
                {
                   // Dln.PowerOnOff(port, false);
                }
                if (errMsg[0] == "" && PassFails[0].FirstFailIndex == 0) { STATIC.Rcp.tt.Count++; return; }
            }
            STATIC.Rcp.tt.Count++;
            return;

        }
        
        public void Process_Function(int port, string testItem)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int index = 0;
            int RetryIndex = Rcp.RetryCnt.RetryOption.FindIndex(x => x.InspName == testItem);
            int RetryCnt = Rcp.RetryCnt.RetryOption[RetryIndex].Count + 1;
           
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (testItem == ItemList[i].Name)
                {
                    index = i; break;
                }
            }

            int ch = port * 2;
            if (!m_ChannelOn[ch])
                return;

            for (int k = ch; k < ch + ChannelCnt; k++)
            {
                if (m_ChannelOn[k])
                {
                    m_StrIndex[k] = (yield.LastSampleNum + k + 1).ToString();
                    AddLog(k, "\r\n");
                    AddLog(k, m_StrIndex[k] + ">> " + testItem + " Start");
                }
            }

            try
            {
                for (int i = 0; i < RetryCnt; i++)
                {
                   
                    Task Func1 = null, Func2 = null;

                    if (!ItemList[index].IsMulti)
                    {
                        Func1 = new Task(() => ItemList[index].Func(port, testItem, i));
                        Func1.Start();

                        if (Func1 != null) Task.WaitAll(Func1);
                    }
                    else
                    {
                        if (m_ChannelOn[ch])
                        {
                            Func1 = new Task(() => ItemList[index].Func(ch, testItem, i));
                            Func1.Start();
                            AddLog(ch, testItem + " Start");
                        }
                        if (ChannelCnt > 1)
                        {
                            if (m_ChannelOn[ch + 1])
                            {
                                Func2 = new Task(() => ItemList[index].Func(ch + 1, testItem, i));
                                Func2.Start();
                                AddLog(ch + 1, testItem + " Start");
                            }
                        }

                        if (Func1 != null && Func2 != null) Task.WaitAll(Func1, Func2);
                        else
                        {
                            if (Func1 != null) Task.WaitAll(Func1);
                            if (Func2 != null) Task.WaitAll(Func2);
                        }
                    }
                    if(i < RetryCnt - 1)
                    {
                        bool res = CheckFail(ch, testItem);
                        if (res) break;
                        else InitResult(ch, testItem);
                    }
                }

            }
            catch (Exception e)
            {
                for (int k = ch; k < ch + ChannelCnt; k++)
                {
                    AddLog(k, testItem + " Exception : " + e.ToString() + " ch : " + k.ToString());
                    errMsg[k] = testItem + " Error";
                    m_ChannelOn[k] = false;
                }
            }

            for (int k = ch; k < ch + ChannelCnt; k++)
            {
                double ellipse = (double)sw.ElapsedMilliseconds / 1000;
                AddLog(k, string.Format("{0}\t{1:0.000} sec", testItem, ellipse));
                ItemList[index].Time = ellipse.ToString("F3");
            }
            sw.Stop();
        }
        public void LEDs_All_On(int port, bool isOn, List<double> volt = null)
        {
            int ch = port * 2;

            if (volt == null)
            {
                volt = new List<double>
                {
                    STATIC.Rcp.vsFile.LEDCurrentR,
                    STATIC.Rcp.vsFile.LEDCurrentL
                };
            }

            if (m_bAllLEDOn = isOn)
            {
                //  CSH035 적용 시 
                Dln.SetLEDpower(1, (int)(volt[0] * 500));
                Dln.SetLEDpower(2, (int)(volt[1] * 500));
            }
            else
                for (int k = ch; k < ch + ChannelCnt; k++)
                {
                    Dln.SetLEDpower(1, 0);
                    Dln.SetLEDpower(2, 0);
                }
        }

        public void MakeWaveform(string name)
        {
            for (int k = 0; k < ChannelCnt; k++)
            {
                foreach (var Cal in CalList[k])
                {
                    if (Cal.Name == name)
                    {
                        Cal.Clear();

                        int min = 0;
                        int max = 0;
                        int step = 0;
                        int curPos = 0;

                        switch (name)
                        {
                            case "AF Scan":
                                //AF ========
                                MakeWaveformCodeAF(ref Cal.CodeZ, Condition.iAFDrvCodeMin, Condition.iAFDrvCodeMax, AFCenter, Condition.iDrvAFStep);
                                break;
                            case "OIS X Scan":
                                //X =========
                                MakeWaveformCodeOIS(ref Cal.CodeX, Condition.iXDrvCodeMin, Condition.iXDrvCodeMax, OISXCenter, Condition.iDrvXStep);
                                break;
                            case "OIS Y Scan":
                                //Y1 ===========================
                                MakeWaveformCodeOIS(ref Cal.CodeY, Condition.iYDrvCodeMin, Condition.iYDrvCodeMax, OISYCenter, Condition.iDrvYStep);                               
                                break;

                            case "AF Settling":
                                //min = Condition.iAFStandbyCode;
                                //max = Condition.iAFJumpStepCode;
                                //Cal.CodeZ.Add(min);
                                //Cal.CodeZ.Add(min);
                                //Cal.CodeZ.Add(min);
                                //Cal.CodeZ.Add(max);
                                break;
                        }
                    }
                }

            }
        }
        private void MakeWaveformCodeAF(ref List<int> code, int min, int max, int mid, int step)
        {
            int curPos = 0;
            curPos = mid;
            do
            {
                code.Add(curPos);
                curPos += step;
            } while (curPos < max);
            //  if (max >= 4095) max = 4095;
            code.Add(max);
            curPos -= step;
            do
            {
                code.Add(curPos);
                curPos -= step;
            } while (curPos > min);
            code.Add(min);
            curPos += step;

            do
            {
                code.Add(curPos);
                curPos += step;
            } while (curPos < mid);
            code.Add(mid);

        }
        void MakeWaveformCodeOIS(ref List<int> code, int min, int max, int mid, int step)
        {
            int curPos = 0;
            curPos = mid;
            do
            {
                code.Add(curPos);
                curPos += step;
            } while (curPos < max);
          //  if (max >= 4095) max = 4095;
            code.Add(max);
            curPos -= step;
            do
            {
                code.Add(curPos);
                curPos -= step;
            } while (curPos > min);
            code.Add(min);
            curPos += step;

            do
            {
                code.Add(curPos);
                curPos += step;
            } while (curPos < mid);
            code.Add(mid);
        }

        private void CrossOffsetMove(int port, string name)
        {
            int ch = port * 2;
            //Cross Offset Pos Move 
            for (int j = ch; j < ch + ChannelCnt; j++)
            {
                if (!m_ChannelOn[j]) continue;
              

                switch (name)
                {
                    case "AF Scan":
                        if (Condition.AFMoveOISServoStatus == 0)
                            DrvIC.OISOnOff(j, false);
                        else DrvIC.OISOnOff(j, true);
                        DrvIC.AFOnOff(j, true);
                        break;
                    case "OIS X Scan":
                        DrvIC.OISOnOff(j, true);
                        DrvIC.SetManualDrvModeXY(j, OISXCenter, OISYCenter);                      
                        DrvIC.AFOnOff(j, true);
                        DrvIC.AFMove(j, Condition.OISDrvAFPos);
                        AddLog(j, $"Move AF Best Position : {Condition.OISDrvAFPos}");
                        break;
                    case "OIS Y Scan":
                        DrvIC.OISOnOff(j, true);
                        DrvIC.SetManualDrvModeXY(j, OISXCenter, OISYCenter);                  
                        DrvIC.AFOnOff(j, true);
                        DrvIC.AFMove(j, Condition.OISDrvAFPos);
                        AddLog(j, $"Move AF Best Position : {Condition.OISDrvAFPos}");
                        break;

                }
            }
            Process.Wait(100);
            //Initial Pos Move 

            for (int k = 0; k < 2; k++)
            {
                switch (name)
                {
                    case "AF Scan":
                        for (int j = ch; j < ch + ChannelCnt; j++)
                        {
                            if (!m_ChannelOn[j]) continue;
                            foreach (var Cal in CalList[j])
                            {
                                if (Cal.Name == name) DrvIC.AFMove(j, Cal.CodeZ[0]);
                            }
                        }
                        Process.Wait(Condition.iDrvStepIntervalZ);
                        break;
                    case "OIS X Scan":
                        for (int j = ch; j < ch + ChannelCnt; j++)
                        {
                            if (!m_ChannelOn[j]) continue;
                            foreach (var Cal in CalList[j])
                            {
                                if (Cal.Name == name) DrvIC.OISMove(j, Cal.CodeX[0], OISYCenter);
                            }
                        }
                        Process.Wait(Condition.iDrvStepIntervalX);
                        break;
                    case "OIS Y Scan":
                        for (int j = ch; j < ch + ChannelCnt; j++)
                        {
                            if (!m_ChannelOn[j]) continue;
                            foreach (var Cal in CalList[j])
                            {
                                if (Cal.Name == name) DrvIC.OISMove(j, OISXCenter, Cal.CodeY[0]);
                            }
                        }
                        Process.Wait(Condition.iDrvStepIntervalY);
                        break;

                }
            }
        }
        private void Process_ScanCodeTest(int port, string name, int InspCnt)
        {
            int ch = port * 2;

            Wait(100);

            CrossOffsetMove(port, name);

            IsScan[port] = true;
            framCnt[port] = 0;
            int curPos = 0;

            Stopwatch sw = new Stopwatch();
            sw.Reset(); sw.Start();

            while (IsScan[port])
            {
                for (int j = ch; j < ch + ChannelCnt; j++)
                {
                    if (!m_ChannelOn[j]) continue;
                    
                    foreach (var Cal in CalList[j])
                        if (Cal.Name == name)
                        {
                            if (name.Contains("X"))
                            {
                                DrvIC.OISMove(j, Cal.CodeX[framCnt[port]], OISYCenter);
                            }
                            else if (name.Contains("Y"))
                            {
                                DrvIC.OISMove(j, OISXCenter, Cal.CodeY[framCnt[port]]);
                            }
                            else if (name.Contains("AF"))
                            {
                                DrvIC.AFMove(j, Cal.CodeZ[framCnt[port]]);
                            }

                            Cal.StrokeX.Add(0);
                            Cal.StrokeY.Add(0);
                            Cal.StrokeZ.Add(0);
                            Cal.StrokeY1.Add(0);
                            Cal.StrokeY2.Add(0);
                            Cal.HallX.Add(0);
                            Cal.HallY.Add(0);
                            Cal.HallZ.Add(0);
                            Cal.HallY1.Add(0);
                            Cal.HallY2.Add(0);
                            Cal.Current.Add(0);
                            Cal.TiltX.Add(0);
                            Cal.TiltY.Add(0);
                            Cal.TiltZ.Add(0);
                        }
                }
                if (name.Contains("X"))
                {
                    Thread.Sleep(Condition.iDrvStepIntervalX);
                }
                else if (name.Contains("Y"))
                {
                    Thread.Sleep(Condition.iDrvStepIntervalY);
                }
                else if (name.Contains("AF"))
                {
                    Thread.Sleep(Condition.iDrvStepIntervalZ);
                }

                for (int j = ch; j < ch + ChannelCnt; j++)
                {
                    if (!m_ChannelOn[j]) continue;
                    foreach (var Cal in CalList[j])
                        if (Cal.Name == name)
                        {
                            Cal.HallX[framCnt[port]] = DrvIC.ReadOISHall(j, 0, 0);
                            Cal.HallY[framCnt[port]] = DrvIC.ReadOISHall(j, 1, 0);
                            Cal.HallZ[framCnt[port]] = DrvIC.ReadAFHall(j);
                            //Get Hall
                            if (name.Contains("X"))
                            {
                                Cal.Current[framCnt[port]] = Dln.GetCurrent(j, 1);
                                AddLog(j, string.Format("{0} == Code : {1}, Hall : {2}", name, Cal.CodeX[framCnt[port]], Cal.HallX[framCnt[port]]));
                            }
                            else if (name.Contains("Y"))
                            {
                                Cal.Current[framCnt[port]] = Dln.GetCurrent(j, 1);
                                AddLog(j, string.Format("{0} == Code : {1}, Hall : {2}", name, Cal.CodeY[framCnt[port]], Cal.HallY[framCnt[port]]));
                            }
                            else if (name.Contains("AF"))
                            {
                                Cal.Current[framCnt[port]] = Dln.GetCurrent(j, 0) + Condition.AFCurrentOffset;
                                AddLog(j, string.Format("{0} == Code : {1}, Hall : {2}", name, Cal.CodeZ[framCnt[port]], Cal.HallZ[framCnt[port]]));
                            }

                        }
                }
                STATIC.fVision.m__G.oCam[port].GrabA(framCnt[port]);

                for (int j = ch; j < ch + ChannelCnt; j++)
                {
                    if (!m_ChannelOn[j]) continue;
                    foreach (var Cal in CalList[j])
                        if (Cal.Name == name)
                        {
                            if (name.Contains("X"))
                            {
                                if (Cal.CodeX.Count - 1 == framCnt[port]) IsScan[port] = false;
                            }
                            else if (name.Contains("Y"))
                            {
                                if (Cal.CodeY.Count - 1 == framCnt[port]) IsScan[port] = false;
                            }
                            else if (name.Contains("AF"))
                            {
                                if (Cal.CodeZ.Count - 1 == framCnt[port]) IsScan[port] = false;
                            }

                        }
                }
                framCnt[port]++;
            }
            long esec = sw.ElapsedMilliseconds;
            sw.Stop();

            double fps = 0;
            if (name.Contains("X"))
            {
                fps = esec - Condition.iDrvStepIntervalX * framCnt[port];
            }
            else if (name.Contains("Y"))
            {
                fps = esec - Condition.iDrvStepIntervalY * framCnt[port];
            }
            else if (name.Contains("AF"))
            {
                fps = esec - Condition.iDrvStepIntervalZ * framCnt[port];
            }

            fps = fps / 1000;
            fps = framCnt[port] / fps * 2.4;

            for (int j = ch; j < ch + ChannelCnt; j++)
            {
                DrvIC.AFOnOff(j, false);
                DrvIC.OISOnOff(j, false);
              
            }
            for (int j = ch; j < ch + ChannelCnt; j++)
                AddLog(j, string.Format("framCnt {0}", framCnt[port]));

            STATIC.fVision.m__G.oCam[port].CommonToReplayBuf(name, framCnt[port]);
        }
        public double settleRigingTime = 0;
        private void Process_ScanTimeTest(int port, string name, int startcode, int endcode)
        {
            try
            {
                settleRigingTime = 0;
                framCnt[port] = 0;
                int ch = port * 2;

                MakeWaveform(name);
                DrvIC.AFOnOff(ch, true);
             

                for (int j = ch; j < ch + ChannelCnt; j++)
                {
                    DrvIC.OISOnOff(ch, false);
                }
                Thread.Sleep(100);
                Stopwatch sw = new Stopwatch();
                sw.Reset(); sw.Start();
                //Time Grab ===============
                Task[] task = new Task[2];

                long startTime = 0;
                long endTime = 0;
                long lTimerFrequency = 0;
                SupremeTimer.QueryPerformanceCounter(ref startTime);
                SupremeTimer.QueryPerformanceCounter(ref endTime);
                SupremeTimer.QueryPerformanceFrequency(ref lTimerFrequency);

                double Ellapsed = 1000000 * (endTime - startTime) / (double)(lTimerFrequency);
                task[0] = Task.Factory.StartNew(() =>
                {
                    IsScan[port] = true;
                    //         SupremeTimer.QueryPerformanceCounter(ref startTime);
                    while (IsScan[port])
                    {
                        STATIC.fVision.m__G.oCam[port].GrabD(framCnt[port]);
                        for (int j = ch; j < ch + ChannelCnt; j++)
                        {
                            foreach (var Cal in CalList[j])
                                if (Cal.Name == name)
                                {
                                    SupremeTimer.QueryPerformanceCounter(ref endTime);
                                    SupremeTimer.QueryPerformanceFrequency(ref lTimerFrequency);
                                    Ellapsed = 1000 * (endTime - startTime) / (double)(lTimerFrequency); //  msec

                                    Cal.Time.Add(Ellapsed);
                                    Cal.StrokeX.Add(0);
                                    Cal.StrokeY.Add(0);
                                    Cal.StrokeZ.Add(0);
                                    Cal.StrokeY1.Add(0);
                                    Cal.StrokeY2.Add(0);
                                    Cal.TiltX.Add(0);
                                    Cal.TiltY.Add(0);
                                    Cal.TiltZ.Add(0);

                                }
                        }
                        framCnt[port]++;
                    }

                });

                task[1] = Task.Factory.StartNew(() =>
                {
                    foreach (var Cal in CalList[port])
                        if (Cal.Name == name)
                        {

                            for (int j = ch; j < ch + ChannelCnt; j++)
                            {
                                if (Cal.Name == name)
                                {
                                    DrvIC.AFMove(j, startcode);
                                }
                            }

                            Wait(100);
                            //Thread.Sleep(100);
                            for (int j = ch; j < ch + ChannelCnt; j++)
                            {
                                if (Cal.Name == name)
                                {
                                    SupremeTimer.QueryPerformanceCounter(ref startTime);
                                    DrvIC.AFMove(j, endcode);
                                }
                            }
                            settleRigingTime = (double)sw.ElapsedMilliseconds / 1000;

                            Wait(100);
                            //Thread.Sleep(100);
                        }
                    IsScan[port] = false;
                });

                Task t = Task.WhenAll(task);
                try
                {
                    t.Wait();
                }
                catch { }
                sw.Stop();

                // FrmRate 표시 === 
                double frameRate = framCnt[port] / (double)sw.ElapsedMilliseconds * 1000;
                for (int j = ch; j < ch + ChannelCnt; j++)
                {
                    AddLog(j, string.Format("FrmRate == {0:F2} frame/sec", frameRate));
                }
                STATIC.fVision.m__G.oCam[port].CommonToReplayBuf(name, framCnt[port]);


            }
            catch (Exception ex)
            {
                AddLog(0, ex.ToString());
            }


        }



        public void Process_CalcCodeTest(int port, string name, int InspCnt)
        {
          
            int ch = port * 2;

            for (int j = ch; j < ch + ChannelCnt; j++)
            {
                AddLog(j, string.Format("{0} Driving Data>>", name));
            }
            List<FindResult> result = new List<FindResult>();
            int fCount = 0;
            foreach (var Cal in CalList[port])
                if (Cal.Name == name)
                {
                    if (name.Contains("X"))
                    {
                        fCount = Cal.CodeX.Count;
                    }
                    else if (name.Contains("Y"))
                    {
                        fCount = Cal.CodeY.Count;
                    }
                    else if (name.Contains("AF"))
                    {
                        fCount = Cal.CodeZ.Count;
                    }

                }

            for (int i = 0; i < fCount; i++)
            {
                result.Add(new FindResult());

                result[i] = STATIC.fVision.MeasureTxTyTz(i, name, true, false);

            }

            //////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////


            for (int j = ch; j < ch + ChannelCnt; j++)
            {
                if (!m_ChannelOn[j]) continue;
                foreach (var Cal in CalList[j])
                    if (Cal.Name == name)
                    {
                        double centerX = 0;
                        double centerY = 0;
                        double centerY1 = 0;
                        double centerY2 = 0;
                        double centerZ = 0;
                        double centertX = 0;
                        double centertY = 0;
                        double centertZ = 0;

                        bool isCentered = false;
                        for (int i = 2; i < fCount; i++)
                        {
                            if (name.Contains("X"))
                            {
                                if (Cal.CodeX[i] == OISXCenter)
                                {
                                    centerX = result[i].cx[j];
                                    centerY = result[i].cy[j];
                                    centerZ = result[i].cz[j];
                                    centertX = result[i].tx[j];
                                    centertY = result[i].ty[j];
                                    centertZ = result[i].tz[j];
                                    centerY1 = result[i].cy1[j];
                                    centerY2 = result[i].cy2[j];
                                    isCentered = true;
                                    break;
                                }
                            }
                            else if (name.Contains("Y"))
                            {
                                if (Cal.CodeY[i] == OISYCenter)
                                {
                                    centerX = result[i].cx[j];
                                    centerY = result[i].cy[j];
                                    centerZ = result[i].cz[j];
                                    centertX = result[i].tx[j];
                                    centertY = result[i].ty[j];
                                    centertZ = result[i].tz[j];
                                    centerY1 = result[i].cy1[j];
                                    centerY2 = result[i].cy2[j];
                                    isCentered = true;
                                    break;
                                }
                            }
                            else if (name.Contains("AF"))
                            { 
                                if (Cal.CodeZ[i] == AFCenter)
                                {
                                    centerX = result[i].cx[j];
                                    centerY = result[i].cy[j];
                                    centerZ = result[i].cz[j];
                                    centertX = result[i].tx[j];
                                    centertY = result[i].ty[j];
                                    centertZ = result[i].tz[j];
                                    centerY1 = result[i].cy1[j];
                                    centerY2 = result[i].cy2[j];
                                    isCentered = true;
                                    break;
                                }
                            }


                        }
                        if (!isCentered)
                        {
                            AddLog(j, string.Format("Center Code Data Failed"));
                        }

                        //if (Option.FixedCenter)
                        //{
                        //    int centerPoint = 0;
                        //    for (int i = 0; i < fCount; i++)
                        //    {
                        //        if (name.Contains("X"))
                        //        {
                        //            centerPoint = HallParam[j].XHmid;
                        //        }
                        //        else if (name.Contains("Y"))
                        //        {
                        //            centerPoint = HallParam[j].YHmid;
                        //        }

                        //    }
                        //    centerX = result[centerPoint].cx[j]; centerY = result[centerPoint].cy[j];
                        //    centerY1 = result[centerPoint].cy1[j]; centerY2 = result[centerPoint].cy2[j];
                        //}
                        //else
                        //{
                        //}
                        for (int i = 0; i < fCount; i++)
                        {
                            Cal.StrokeX[i] = result[i].cx[j] - centerX;
                            Cal.StrokeY[i] = result[i].cy[j] - centerY;
                            Cal.StrokeZ[i] = result[i].cz[j] - centerZ;
                            Cal.StrokeY1[i] = result[i].cy1[j] - centerY1;
                            Cal.StrokeY2[i] = result[i].cy2[j] - centerY2;
                            Cal.TiltX[i] = result[i].tx[j] - centertX;
                            Cal.TiltY[i] = result[i].ty[j] - centertY;
                            Cal.TiltZ[i] = result[i].tz[j] - centertZ;
                        }
                    }
            }
            if (Option.SaveRawData)
            {
                string str = Convert.ToString(yield.LastSampleNum + 1);
                string dateDir = STATIC.CreateDateDir();
                dateDir += "DrivingData\\";
                if (!Directory.Exists(dateDir))
                    Directory.CreateDirectory(dateDir);


                //string timeDir = string.Format("{0}{1}{2}", dt.Hour, dt.Minute, dt.Second);
                string timeDir = $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s";
          

                for (int j = ch; j < ch + ChannelCnt; j++)
                {
                    if (!m_ChannelOn[j]) continue;
                    foreach (var Cal in CalList[j])
                        if (Cal.Name == name)
                        {
                            List<string> arry = new List<string>();
                            
                            string path = "";
                            switch (name)
                            {
                                case "AF Scan":

                                    arry.Add("i,AF Code,X Code,Y Code,X,Y,Z,TX,TY,TZ,Hall X,Hall Y,Hall AF,Current");
                                    for (int i = 0; i < fCount; i++)
                                    {
                                        path = string.Format(dateDir + "{0}_{1}_{2}_{3}.csv", name, m_StrIndex[j], InspCnt + 1,timeDir);
                                        string data = $"{i},{Cal.CodeZ[i]},{OISXCenter},{OISYCenter},{Cal.StrokeX[i].ToString("F3")},{Cal.StrokeY[i].ToString("F3")},{Cal.StrokeZ[i].ToString("F3")}," +
                                                      $"{Cal.TiltX[i].ToString("F3")},{Cal.TiltY[i].ToString("F3")},{Cal.TiltZ[i].ToString("F3")},{Cal.HallX[i]},{Cal.HallY[i]},{Cal.HallZ[i]},{Cal.Current[i].ToString("F3")}";
                                        arry.Add(data);

                                        if (i == 0)
                                            AddLog(j, string.Format("Code AF\tStroke AF\tTx\tTy\tTz"));
                                        AddLog(j, string.Format("{0}\t{1:0.000}\t{2:0.000}\t{3:0.000}\t{4:0.000}", Cal.CodeZ[i], Cal.StrokeZ[i], Cal.TiltX[i], Cal.TiltY[i], Cal.TiltZ[i]));
                                    }
                                    break;
                                case "OIS X Scan":
                                    arry.Add("i,AF Code,X Code,Y Code,X,Y,Z,TX,TY,TZ,Hall X,Hall Y,Hall AF,Current");
                                 
                                    for (int i = 0; i < fCount; i++)
                                    {
                                        path = string.Format(dateDir + "{0}_{1}_{2}_{3}.csv", name, m_StrIndex[j], InspCnt + 1,  timeDir);
                                        string data = $"{i},{Condition.OISDrvAFPos},{Cal.CodeX[i]},{OISYCenter},{Cal.StrokeX[i].ToString("F3")},{Cal.StrokeY[i].ToString("F3")},{Cal.StrokeZ[i].ToString("F3")}," +
                                                      $"{Cal.TiltX[i].ToString("F3")},{Cal.TiltY[i].ToString("F3")},{Cal.TiltZ[i].ToString("F3")},{Cal.HallX[i]},{Cal.HallY[i]},{Cal.HallZ[i]},{Cal.Current[i].ToString("F3")}";
                                        arry.Add(data);
                                      
                                        if (i == 0)
                                            AddLog(j, string.Format("Code X\tStroke X\tTx\tTy\tTz"));
                                        AddLog(j, string.Format("{0}\t{1:0.000}\t{2:0.000}\t{3:0.000}\t{4:0.000}", Cal.CodeX[i], Cal.StrokeX[i], Cal.TiltX[i], Cal.TiltY[i], Cal.TiltZ[i]));
                                    }

                                    //AddLog(j, string.Format("Cross Y1 Hall Max {0:00} Y1 Hall Min {1:00}", Cal.HallY1.Max(), Cal.HallY1.Min()));
                                    //AddLog(j, string.Format("Cross Y1 Hall Diff {0:00}", Math.Abs(Cal.HallY1.Max() - Cal.HallY1.Min())));
                                    //AddLog(j, string.Format("Cross Y2 Hall Max {0:00} Y2 Hall Min {1:00}", Cal.HallY2.Max(), Cal.HallY2.Min()));
                                    //AddLog(j, string.Format("Cross Y2 Hall Diff {0:00}", Math.Abs(Cal.HallY2.Max() - Cal.HallY2.Min())));

                                    //AddLog(j, string.Format("Rotation Max {0:00} Min {1:00}", Cal.TiltZ.Max(), Cal.TiltZ.Min()));
                                    //AddLog(j, string.Format("Rotation Diff {0:00}", Math.Abs(Cal.TiltZ.Max() - Cal.TiltZ.Min())));

                                    break;
                                case "OIS Y Scan":

                                    arry.Add("i,AF Code,X Code,Y Code,X,Y,Z,TX,TY,TZ,Hall X,Hall Y,Hall AF,Current");
                                    for (int i = 0; i < fCount; i++)
                                    {
                                        path = string.Format(dateDir + "{0}_{1}_{2}_{3}.csv", name, m_StrIndex[j], InspCnt + 1, timeDir);
                                        string data = $"{i},{Condition.OISDrvAFPos},{OISXCenter},{Cal.CodeY[i]},{Cal.StrokeX[i].ToString("F3")},{Cal.StrokeY[i].ToString("F3")},{Cal.StrokeZ[i].ToString("F3")}," +
                                                     $"{Cal.TiltX[i].ToString("F3")},{Cal.TiltY[i].ToString("F3")},{Cal.TiltZ[i].ToString("F3")},{Cal.HallX[i]},{Cal.HallY[i]},{Cal.HallZ[i]},{Cal.Current[i].ToString("F3")}";
                                        arry.Add(data);

                                        if (i == 0)
                                            AddLog(j, string.Format("Code Y1\tCode Y2\tStroke Y1\tStroke Y2\t\tTx\tTy\tTz"));

                                        AddLog(j, string.Format("{0}\t{1}\t{2:0.000}\t{3:0.000}\t{4:0.000}\t{5:0.000}\t{6:0.000}", Cal.CodeY[i], Cal.CodeY[i], Cal.StrokeY1[i], Cal.StrokeY2[i], Cal.TiltX[i], Cal.TiltY[i], Cal.TiltZ[i]));
                                    }

                                    //AddLog(j, string.Format("Cross X Hall Max {0:00} X Hall Min {1:00}", Cal.HallY2.Max(), Cal.HallY2.Min()));
                                    //AddLog(j, string.Format("Cross X Hall Diff {0:00}", Math.Abs(Cal.HallY2.Max() - Cal.HallY2.Min())));

                                    //AddLog(j, string.Format("Rotation Max {0:00} Min {1:00}", Cal.TiltZ.Max(), Cal.TiltZ.Min()));
                                    //AddLog(j, string.Format("Rotation Diff {0:00}", Math.Abs(Cal.TiltZ.Max() - Cal.TiltZ.Min())));

                                    break;

                            }
                            if (path != "") STATIC.SetTextLine(path, arry);
                        }
                }
            }
            
            for (int j = ch; j < ch + ChannelCnt; j++)
            {
                if (!m_ChannelOn[j]) continue;
                double maxtiltX = 0, maxtiltY = 0;
                double[] refArray = null;
                foreach (var Cal in CalList[j])
                    if (Cal.Name == name)
                    {
                        double forword = 0, backword = 0;
                        if (name.Contains("Linearity")) return;
                        if (name.Contains("AF"))
                        {
                            forword = PassFails[j].Results[(int)SpecItem.AF_Forwardstroke].Val = Cal.StrokeZ.Max(); //Cal.CalFwdStoke(Cal.CodeZ, Cal.StrokeZ);
                            backword = PassFails[j].Results[(int)SpecItem.AF_Backwardstroke].Val = Cal.StrokeZ.Min(); //Cal.CalBwdStoke(Cal.CodeZ, Cal.StrokeZ);
                            PassFails[j].Results[(int)SpecItem.AF_Ratedstroke].Val = Math.Abs(forword - backword);
                      //      PassFails[j].Results[(int)SpecItem.AF_Sensitivity].Val = Cal.CalSensitivity(Cal.CodeZ, Cal.StrokeZ, Condition.iAFCodeRange, Condition.iAFStrokeRange, AFCenter);
                            PassFails[j].Results[(int)SpecItem.AF_Linearity].Val = Cal.CalLinearity(Cal.CodeZ, Cal.StrokeZ, Condition.AFLinMinRange, Condition.AFLinMaxRange, Condition.AFLinMinStep,
                                Condition.AFLinMaxStep, Condition.AFLinMinStroke, Condition.AFLinMaxStroke, Condition.AFLinMode, Condition.iAFDrvCodeMin, Condition.iAFDrvCodeMax);
                            PassFails[j].Results[(int)SpecItem.AF_Hysteresis].Val = Cal.CalHysteresis(Cal.CodeZ, Cal.StrokeZ, Condition.AFHysMinRange, Condition.AFHysMaxRange, Condition.AFHysMinStep,
                                Condition.AFhysMaxStep, Condition.AFHysMinStroke, Condition.AFHysMaxStroke, Condition.AFHysMode, Condition.iAFDrvCodeMin, Condition.iAFDrvCodeMax);


                            double[] MtoM = Cal.CalCurrent(Cal.CodeZ, Cal.StrokeZ, Cal.Current, Condition.AFCurrMinRange, Condition.AFCurrMaxRange, Condition.AFCurrMinStep, Condition.AFCurrMaxStep,
                                Condition.AFCurrMinStroke, Condition.AFCurrMaxStroke, Condition.AFCurrMode, Condition.iAFDrvCodeMin, Condition.iAFDrvCodeMax);

                        //    PassFails[j].Results[(int)SpecItem.AF_MaxCurrent].Val = MtoM[0]; //Cal.CalMaxCurrent(Cal.CodeZ, Cal.StrokeZ, Condition.iAFCodeRange, Condition.iAFStrokeRange);
                          //  PassFails[j].Results[(int)SpecItem.AF_MinCurrent].Val = MtoM[1];
                            //     PassFails[j].Results[(int)SpecItem.AF_HoldingCurrent].Val = Cal.CalHoldingCurrent(Cal.CodeZ, Cal.StrokeZ, Condition.iAFCodeRange, Condition.iAFStrokeRange);
                        //    PassFails[j].Results[(int)SpecItem.AF_CrosstalkX].Val = Cal.CalCrosstalkAF(Cal.CodeZ, Cal.StrokeZ, Cal.StrokeX, Condition.iAFCodeRange, Condition.iAFStrokeRange, AFCenter);
                       //     PassFails[j].Results[(int)SpecItem.AF_CrosstalkY].Val = Cal.CalCrosstalkAF(Cal.CodeZ, Cal.StrokeZ, Cal.StrokeY, Condition.iAFCodeRange, Condition.iAFStrokeRange, AFCenter);
                          //  PassFails[j].Results[(int)SpecItem.AF_CrosstalkR].Val = Cal.CalCrosstalkR(Cal.CodeZ, Cal.StrokeX, Cal.StrokeY, Condition.iAFCodeRange, Condition.iAFStrokeRange, AFCenter);
                        //    PassFails[j].Results[(int)SpecItem.AF_Rolling].Val = Cal.CalRolling(Cal.CodeZ, Cal.StrokeZ, Condition.iAFCodeRange, Condition.iAFStrokeRange, AFCenter);
                            
                            (double sqrT, double maxX, double maxY, double[] refArr) = Cal.CalTilt(Cal.CodeZ, Cal.TiltX, Cal.TiltY, Condition.TiltMinCode, Condition.TiltMaxCode, Condition.TiltRefCode, Condition.iAFDrvCodeMin, Condition.iAFDrvCodeMax);
                            maxtiltX = maxX; maxtiltY = maxY;
                            refArray = refArr;
                            PassFails[j].Results[(int)SpecItem.AF_Tilt].Val = sqrT;

                            PassFails[j].Results[(int)SpecItem.AF_DynamicTilt_Regional].Val = Cal.CalAFDynamicTilt(Cal.CodeZ, Cal.StrokeZ, Cal.TiltX, Cal.TiltY, 
                                Condition.DynamicTiltRange, Condition.DynamincTiltRefCode, Condition.iAFDrvCodeMin, Condition.iAFDrvCodeMax);

                            ShowDataResults(j, (int)SpecItem.AF_Ratedstroke, (int)SpecItem.AF_Ratedstroke, InspType.Normal, new double[] { });
                            ShowDataResults(j, (int)SpecItem.AF_Backwardstroke, (int)SpecItem.AF_Backwardstroke, InspType.OnlyMax, new double[] { });
                            ShowDataResults(j, (int)SpecItem.AF_Forwardstroke, (int)SpecItem.AF_Forwardstroke, InspType.OnlyMin, new double[] { });
                            ShowDataResults(j, (int)SpecItem.AF_Hysteresis, (int)SpecItem.AF_Hysteresis, InspType.OnlyMax, new double[] { });
                            ShowDataResults(j, (int)SpecItem.AF_Linearity, (int)SpecItem.AF_Linearity, InspType.OnlyMax, new double[] { });
                            ShowDataResults(j, (int)SpecItem.AF_Current, (int)SpecItem.AF_Current, InspType.MintoMax, MtoM);
                            ShowDataResults(j, (int)SpecItem.AF_Tilt, (int)SpecItem.AF_Tilt, InspType.Normal, new double[] { });
                            ShowDataResults(j, (int)SpecItem.AF_DynamicTilt_Regional, (int)SpecItem.AF_DynamicTilt_Regional, InspType.Normal, new double[] { });
                            AFCurrentMinMax = MtoM.ToArray();

                        }
                        else if (name.Contains("OIS X"))
                        {
                            forword = PassFails[j].Results[(int)SpecItem.OISX_Forwardstroke].Val = Cal.StrokeX.Max();// Cal.CalFwdStoke(Cal.CodeX, Cal.StrokeX);
                            backword = PassFails[j].Results[(int)SpecItem.OISX_Backwardstroke].Val = Cal.StrokeX.Min();//Cal.CalBwdStoke(Cal.CodeX, Cal.StrokeX);
                            PassFails[j].Results[(int)SpecItem.OISX_Ratedstroke].Val = Math.Abs(forword - backword);
                            PassFails[j].Results[(int)SpecItem.x_Sensitivity].Val = Cal.CalSensitivity(Cal.CodeX, Cal.StrokeX, Condition.iXDrvCodeMin, Condition.iXDrvCodeMin, Condition.XSensitivityMinStroke, Condition.XSensitivityMaxStroke);
                            PassFails[j].Results[(int)SpecItem.OISX_Linearity].Val = Cal.CalLinearity(Cal.CodeX, Cal.StrokeX, Condition.XLinMinRange, Condition.XLinMaxRange, Condition.XLinMinStep,
                                Condition.XLinMaxStep, Condition.XLinMinStroke, Condition.XLinMaxStroke, Condition.XLinMode, Condition.iXDrvCodeMin, Condition.iXDrvCodeMax);
                            PassFails[j].Results[(int)SpecItem.OISX_Hysteresis].Val = Cal.CalHysteresis(Cal.CodeX, Cal.StrokeX, Condition.XHysMinRange, Condition.XHysMaxRange, Condition.XHysMinStep,
                                Condition.XHysMaxStep, Condition.XHysMinStroke, Condition.XHysMaxStroke, Condition.XHysMode, Condition.iXDrvCodeMin, Condition.iXDrvCodeMax);

                            double[] MtoM = Cal.CalCurrent(Cal.CodeX, Cal.StrokeX, Cal.Current, Condition.XCurrMinRange, Condition.XCurrMaxRange, Condition.XCurrMinStep, Condition.XCurrMaxStep,
                              Condition.XCurrMinStroke, Condition.XCurrMaxStroke, Condition.XCurrMode, Condition.iXDrvCodeMin, Condition.iXDrvCodeMax);

                            PassFails[j].Results[(int)SpecItem.OISX_xTalk].Val = Cal.CalCrossTalk(Cal.CodeX, Cal.StrokeX, Cal.StrokeY, Condition.X_XtalkMinRange, Condition.X_XtalkMaxRange, Condition.X_XtalkMinStep, Condition.X_XtalkMaxStep,
                                Condition.X_XtalkMinStroke, Condition.X_XtalkMaxStroke, Condition.X_XtalkMode, Condition.iXDrvCodeMin, Condition.iXDrvCodeMax);


                            PassFails[j].Results[(int)SpecItem.xDynamicTilt].Val = Cal.CalTiltOIS(Cal.CodeX, Cal.StrokeX, Cal.TiltX, Cal.TiltY, Condition.XLinMinRange, Condition.XLinMaxRange, Condition.XLinMinStep,
                               Condition.XLinMaxStep, Condition.XLinMinStroke, Condition.XLinMaxStroke, Condition.XLinMode, Condition.iXDrvCodeMin, Condition.iXDrvCodeMax);
                            //          PassFails[j].Results[(int)SpecItem.OISX_MaxCurrent].Val = MtoM[0]; //Cal.CalMaxCurrent(Cal.CodeX, Cal.StrokeX, Condition.iXCodeRange, Condition.iXStrokeRange);
                            //     PassFails[j].Results[(int)SpecItem.OISX_MinCurrent].Val = MtoM[1];
                            // PassFails[j].Results[(int)SpecItem.OISX_CenteringCurrent].Val = Cal.CalCenterCurrent(Cal.CodeX, Cal.StrokeX, Condition.iXCodeRange, Condition.iXCodeRange);

                            //    double[] xtalkRes = Cal.CalCrosstalk(Cal.CodeX, Cal.StrokeX, Cal.StrokeY, Condition.iXCodeRange, Condition.iXStrokeRange, OISCenter);

                            // PassFails[j].Results[(int)SpecItem.OISX_CrosstalkY].Val = xtalkRes[0];
                            //  PassFails[j].Results[(int)SpecItem.OISX_CrosstalkY_dB].Val = xtalkRes[1];
                            //   PassFails[j].Results[(int)SpecItem.OISX_CrosstalkY_P2P].Val = xtalkRes[2];
                            //  PassFails[j].Results[(int)SpecItem.OISX_CrosstalkYP2P_dB].Val = xtalkRes[3];
                            //PassFails[j].Results[(int)SpecItem.OISX_CrosstalkZ].Val = Cal.CalCrosstalk(Cal.CodeX, Cal.StrokeZ, Condition.iXCodeRange, Condition.iXCodeRange);
                            //PassFails[j].Results[(int)SpecItem.OISX_CrosstalkR].Val = Cal.CalCrosstalkR(Cal.CodeX, Cal.StrokeY, Cal.StrokeZ, Condition.iXCodeRange, Condition.iXCodeRange);
                            // PassFails[j].Results[(int)SpecItem.OISX_Rolling].Val = Cal.CalRolling(Cal.CodeX, Cal.StrokeX, Condition.iXCodeRange, Condition.iXStrokeRange, OISCenter);


                            SlopeX = Cal.CalSlopeForOISShift(Cal.CodeX, Cal.StrokeX);
                            (XHATPOSAF[0], XHATPOSAF[1], XHATPOSOIS[0], XHATPOSOIS[1]) = Cal.GetCodeByStroke(Cal.CodeX, Cal.StrokeX, Condition.iXDrvCodeMin, Condition.iXDrvCodeMax, Condition.AFHATStroke, Condition.OISHATStroke);
                            (AFOISVerifyXPOSOIS[0], AFOISVerifyXPOSOIS[1]) = Cal.GetCodeByStroke(Cal.CodeX, Cal.StrokeX, Condition.iXDrvCodeMin, Condition.iXDrvCodeMax, Condition.Verify_AFOIS_Xtalk_Stroke);
                            PassFails[j].Results[(int)SpecItem.x_HallDecenter].Val = (Math.Abs(forword) - Math.Abs(backword)) / 2.0;

                            ShowDataResults(j, (int)SpecItem.OISX_Ratedstroke, (int)SpecItem.OISX_Ratedstroke, InspType.Normal, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISX_Backwardstroke, (int)SpecItem.OISX_Backwardstroke, InspType.OnlyMax, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISX_Forwardstroke, (int)SpecItem.OISX_Forwardstroke, InspType.OnlyMin, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISX_Hysteresis, (int)SpecItem.OISX_Hysteresis, InspType.OnlyMax, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISX_Linearity, (int)SpecItem.OISX_Linearity, InspType.OnlyMax, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISX_Current, (int)SpecItem.OISX_Current, InspType.MintoMax, MtoM);
                            ShowDataResults(j, (int)SpecItem.x_HallDecenter, (int)SpecItem.x_HallDecenter, InspType.Normal, new double[] { });
                            ShowDataResults(j, (int)SpecItem.x_Sensitivity, (int)SpecItem.x_Sensitivity, InspType.Normal, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISX_xTalk, (int)SpecItem.OISX_xTalk, InspType.OnlyMax, new double[] { });
                            ShowDataResults(j, (int)SpecItem.xDynamicTilt, (int)SpecItem.xDynamicTilt, InspType.Normal, new double[] { });
                            OISXCurrentMinMax = MtoM.ToArray();

                        }
                        else if (name.Contains("OIS Y"))
                        {
                            forword = PassFails[j].Results[(int)SpecItem.OISY_Forwardstroke].Val = Cal.StrokeY.Max();// Cal.CalFwdStoke(Cal.CodeY1, Cal.StrokeY);
                            backword = PassFails[j].Results[(int)SpecItem.OISY_Backwardstroke].Val = Cal.StrokeY.Min(); // Cal.CalBwdStoke(Cal.CodeY1, Cal.StrokeY);
                            PassFails[j].Results[(int)SpecItem.OISY_Ratedstroke].Val = Math.Abs(forword - backword);

                            PassFails[j].Results[(int)SpecItem.y_Sensitivity].Val = Cal.CalSensitivity(Cal.CodeY, Cal.StrokeY, Condition.iYDrvCodeMin, Condition.iYDrvCodeMin, Condition.YSensitivityMinStroke, Condition.YSensitivityMaxStroke);
                            PassFails[j].Results[(int)SpecItem.OISY_Linearity].Val = Cal.CalLinearity(Cal.CodeY, Cal.StrokeY, Condition.YLinMinRange, Condition.YLinMaxRange, Condition.YLinMinStep,
                                Condition.YLinMaxStep, Condition.YLinMinStroke, Condition.YLinMaxStroke, Condition.YLinMode, Condition.iYDrvCodeMin, Condition.iYDrvCodeMax);
                            PassFails[j].Results[(int)SpecItem.OISY_Hysteresis].Val = Cal.CalHysteresis(Cal.CodeY, Cal.StrokeY, Condition.YHysMinRange, Condition.YHysMaxRange, Condition.YHysMinStep,
                                Condition.YHysMaxStep, Condition.YHysMinStroke, Condition.YHysMaxStroke, Condition.YHysMode, Condition.iYDrvCodeMin, Condition.iYDrvCodeMax);

                            double[] MtoM = Cal.CalCurrent(Cal.CodeY, Cal.StrokeY, Cal.Current, Condition.YCurrMinRange, Condition.YCurrMaxRange, Condition.YCurrMinStep, Condition.YCurrMaxStep,
                            Condition.YCurrMinStroke, Condition.YCurrMaxStroke, Condition.YCurrMode, Condition.iYDrvCodeMin, Condition.iYDrvCodeMax);
                          
                            PassFails[j].Results[(int)SpecItem.OISY_xTalk].Val = Cal.CalCrossTalk(Cal.CodeY, Cal.StrokeY, Cal.StrokeX, Condition.Y_XtalkMinRange, Condition.Y_XtalkMaxRange, Condition.Y_XtalkMinStep, Condition.Y_XtalkMaxStep,
                                Condition.Y_XtalkMinStroke, Condition.Y_XtalkMaxStroke, Condition.Y_XtalkMode, Condition.iYDrvCodeMin, Condition.iYDrvCodeMax);

                            PassFails[j].Results[(int)SpecItem.yDynamicTilt].Val = Cal.CalTiltOIS(Cal.CodeY, Cal.StrokeY, Cal.TiltX, Cal.TiltY, Condition.YLinMinRange, Condition.YLinMaxRange, Condition.YLinMinStep,
                              Condition.YLinMaxStep, Condition.YLinMinStroke, Condition.YLinMaxStroke, Condition.YLinMode, Condition.iYDrvCodeMin, Condition.iYDrvCodeMax);


                          
                            SlopeY = Cal.CalSlopeForOISShift(Cal.CodeY, Cal.StrokeY);
                            (YHATPOSAF[0], YHATPOSAF[1], YHATPOSOIS[0], YHATPOSOIS[1]) = Cal.GetCodeByStroke(Cal.CodeY, Cal.StrokeY, Condition.iYDrvCodeMin, Condition.iYDrvCodeMax, Condition.AFHATStroke, Condition.OISHATStroke);
                            (AFOISVerifyYPOSOIS[0], AFOISVerifyYPOSOIS[1]) = Cal.GetCodeByStroke(Cal.CodeY, Cal.StrokeY, Condition.iYDrvCodeMin, Condition.iYDrvCodeMax, Condition.Verify_AFOIS_Xtalk_Stroke);
                            PassFails[j].Results[(int)SpecItem.y_HallDecenter].Val = (Math.Abs(forword) - Math.Abs(backword)) / 2.0;
                            ShowDataResults(j, (int)SpecItem.OISY_Ratedstroke, (int)SpecItem.OISY_Ratedstroke, InspType.Normal, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISY_Backwardstroke, (int)SpecItem.OISY_Backwardstroke, InspType.OnlyMax, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISY_Forwardstroke, (int)SpecItem.OISY_Forwardstroke, InspType.OnlyMin, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISY_Hysteresis, (int)SpecItem.OISY_Hysteresis, InspType.OnlyMax, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISY_Linearity, (int)SpecItem.OISY_Linearity, InspType.OnlyMax, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISY_Current, (int)SpecItem.OISY_Current, InspType.MintoMax, MtoM);
                            ShowDataResults(j, (int)SpecItem.y_HallDecenter, (int)SpecItem.y_HallDecenter, InspType.Normal, new double[] { });
                            ShowDataResults(j, (int)SpecItem.y_Sensitivity, (int)SpecItem.y_Sensitivity, InspType.Normal, new double[] { });
                            ShowDataResults(j, (int)SpecItem.OISY_xTalk, (int)SpecItem.OISY_xTalk, InspType.OnlyMax, new double[] { });
                            ShowDataResults(j, (int)SpecItem.yDynamicTilt, (int)SpecItem.yDynamicTilt, InspType.Normal, new double[] { });
                            OISYCurrentMinMax = MtoM.ToArray();

                        }
                        AddChart(j, name, null, null, maxtiltX, maxtiltY, refArray);
                    }
            }
            framCnt[port] = 0;
        }
        private double Process_CalcTimeTest(int port, string name, int startcode, int endcode, double criteria, int trycnt, int testIndex)
        {
            try
            {
                int ch = port * 2;
                int timeIndex = 99999;
                for (int j = ch; j < ch + ChannelCnt; j++)
                {
                    AddLog(j, string.Format("{0} Driving Data>>", name));
                }
                List<FindResult> result = new List<FindResult>();

                for (int i = 0; i < framCnt[port]; i++)
                {
                    result.Add(new FindResult());
                    result[i] = STATIC.fVision.MeasureTxTyTz(i, name, true, false);
                }

                for (int j = ch; j < ch + ChannelCnt; j++)
                {
                    if (!m_ChannelOn[j]) continue;
                    foreach (var Cal in CalList[j])
                        if (Cal.Name == name)
                        {
                            double centerX = 0;
                            double centerY = 0;
                            double centerY1 = 0;
                            double centerY2 = 0;
                            double centerZ = 0;
                            double centertX = 0;
                            double centertY = 0;
                            double centertZ = 0;

                            centerX = result[2].cx[j];
                            centerY = result[2].cy[j];
                            centerZ = result[2].cz[j];
                            centertX = result[2].tx[j];
                            centertY = result[2].ty[j];
                            centertZ = result[2].tz[j];
                            centerY1 = result[2].cy1[j];
                            centerY2 = result[2].cy2[j];


                            for (int i = 0; i < framCnt[port]; i++)
                            {
                                Cal.StrokeX[i] = result[i].cx[j] - centerX;
                                Cal.StrokeX[i] = result[i].cy[j] - centerY;
                                Cal.StrokeZ[i] = result[i].cz[j] - centerZ;
                                Cal.StrokeY1[i] = result[i].cy1[j] - centerY1;
                                Cal.StrokeY2[i] = result[i].cy2[j] - centerY2;
                                Cal.TiltX[i] = result[i].tx[j] - centertX;
                                Cal.TiltY[i] = result[i].ty[j] - centertY;
                                Cal.TiltZ[i] = result[i].tz[j] - centertZ;
                            }
                        }
                }
                List<double> Time = new List<double>();
                List<double> Stroke = new List<double>();
                bool isStart = false;
                bool isFirstStart = false;
                double RefTime = 0;
                double RefStroke = 0;


                foreach (var Cal in CalList[ch])
                {
                    if (Cal.Name == name)
                    {
                        switch (name)
                        {
                            case "AF Settling":

                                for (int i = 0; i < framCnt[port]; i++)
                                {
                                    if(i > 0)
                                    {
                                        if (Cal.Time[i - 1] > Cal.Time[i]) isStart = true;
                                    }
                                
                                    if (isStart)
                                    {
                                        if (!isFirstStart)
                                        {
                                            RefTime = Cal.Time[i];
                                            RefStroke = Cal.StrokeZ[i];
                                            isFirstStart = true;
                                        }
                                        Time.Add(Cal.Time[i]);
                                        Stroke.Add(Cal.StrokeZ[i]);

                                    }
                                }

                                break;
                        }
                    }
                }

                for (int i = 0; i < Time.Count; i++)
                {
                    Time[i] = Time[i] - RefTime;
                    Stroke[i] = Stroke[i] - RefStroke;
                }




                for (int j = ch; j < ch + ChannelCnt; j++)
                {
                    foreach (var Cal in CalList[j])
                        if (Cal.Name == name)
                        {
                            switch (name)
                            {
                                case "AF Settling":

                                    double FinalStroke = 0;
                                    double InitStroke = Stroke[0];

                                    double SettlingDev;
                                    double FinalTime1, FinalTime2;
                                    FinalTime1 = 100;
                                    FinalTime2 = 90;
                                    int index = 0;

                                    for (int i = 0; i < Time.Count; i++)
                                    {
                                        if (Time[i] < FinalTime1 && Time[i] > FinalTime2)
                                        {
                                            FinalStroke = Stroke[i];
                                            index++;

                                            break;
                                        }
                                    }
                                    double StepStroke = Math.Abs(FinalStroke - InitStroke);

                                    SettlingDev = StepStroke * criteria / 100.0;
                                    if (index == 0) FinalStroke = Stroke[Stroke.Count - 1];
                                    for (int i = Stroke.Count - 1; i > -1; i--)
                                    {
                                        if (Stroke[i] - SettlingDev > FinalStroke || Stroke[i] + SettlingDev < FinalStroke)
                                        {
                                            timeIndex = i;
                                            break;
                                        }
                                    }
                                    break;
                            }
                        }
                }

                if (Option.SaveRawData)
                {
                    string str = Convert.ToString(yield.LastSampleNum + 1);
                    string dateDir = STATIC.CreateDateDir();
                    dateDir += "DrivingData\\";
                    if (!Directory.Exists(dateDir))
                        Directory.CreateDirectory(dateDir);

                    DateTime dt = DateTime.Now;
                    string timeDir = $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s";

                    for (int j = ch; j < ch + ChannelCnt; j++)
                    {
                        foreach (var Cal in CalList[j])
                            if (Cal.Name == name)
                            {
                                List<string> arry = new List<string>();

                                string path = "";
                                switch (name)
                                {
                                    case "AF Settling":
                                        path = string.Format(dateDir + $"{name} {(testIndex)}_{m_StrIndex[j]}_{startcode}_{endcode}_{trycnt}_{timeDir}.csv");
                                        arry.Add("i,AF Time,Z");

                                        for (int i = 0; i < Time.Count; i++)
                                        {
                                            string data = string.Format("{0},{1:0.000},{2:0.000}", i, Time[i], Stroke[i]);
                                            arry.Add(data);

                                        }
                                        //AddLog(j, lstr);
                                        break;
                                }
                                if (path != "") STATIC.SetTextLine(path, arry);
                            }
                    }
                }
                if (Option.settlingGraphVisible) AddChart(0, name, Time.ToList(), Stroke.ToList());
                framCnt[port] = 0;
                return Time[timeIndex];
            }
            catch (Exception ex)
            {
                //PassFails[0].Results[(int)SpecItem.AF_SettillingTime].Val = 11.999;
                //ShowDataResults(0, (int)SpecItem.AF_SettillingTime, (int)SpecItem.AF_SettillingTime, InspType.Normal, new double[] { });
                framCnt[port] = 0;
                AddLog(0, ex.ToString());
                return 99999;
            }

        }




        public void AddHeadResult(string sFilePath)
        {
            StreamWriter writer;
            writer = File.AppendText(sFilePath);

            string sHeader;
            //"Time,Index,PlateBCode,LotID,ACTID,Channel,PM Index,PassFail,"
            sHeader = "Date,Time,Index,PlateBCode,LotID,ACTID,ACTID_M,McNum,PortNum,PassFail,1st Fail Item,";

            string sParam = "";
            for (int i = 0; i < (int)SpecItem.Length; i++)
            {
                if (Spec.specList[i].InspectionType == InspType.MintoMax)
                {
                    sParam += string.Format("{0},", $"{Spec.specList[i].DisplayName} Min");
                    sParam += string.Format("{0},", $"{Spec.specList[i].DisplayName} Max");
                }
                else
                {
                    sParam += string.Format("{0},", Spec.specList[i].DisplayName);
                }

                    
            }
            sHeader += sParam;
            sHeader += "Posture Pos,";


            //Time
            sParam = "";
            for (int i = 0; i < ItemList.Count; i++)
            {
                sParam += string.Format("{0} Time ,", ItemList[i].Name);
            }
            sParam += "Total Test Time";

            sHeader += sParam;

            writer.WriteLine(sHeader);

            //"Time,Index,PlateBCode,LotID,ACTID,Channel,PM Index,PassFail,1st Fail Item,";

            sHeader = "uint,,,,,,,,,,,";

            sParam = "";
            for (int i = 0; i < (int)SpecItem.Length; i++)
            {
                if (Spec.specList[i].InspectionType == InspType.MintoMax)
                {
                    sParam += string.Format("({0}),", Spec.specList[i].Unit);
                    sParam += string.Format("({0}),", Spec.specList[i].Unit);
                }
                else
                {
                    sParam += string.Format("({0}),", Spec.specList[i].Unit);
                }


                   
            }
            sHeader += sParam;

            writer.WriteLine(sHeader);

            sHeader = "Spec Min,,,,,,,,,,,";
            sParam = "";
            for (int i = 0; i < (int)SpecItem.Length; i++)
            {
                if (Spec.specList[i].InspectionType == InspType.MintoMax)
                {
                    sParam += string.Format("{0},", Spec.specList[i].MinSpec);
                    sParam += string.Format("{0},", "");
                }
                else
                {
                    sParam += string.Format("{0},", Spec.specList[i].MinSpec);
                }
                   
            }
            sHeader += sParam;

            writer.WriteLine(sHeader);

            sHeader = "Spec Max,,,,,,,,,,,";
            sParam = "";
            for (int i = 0; i < (int)SpecItem.Length; i++)
            {
                if (Spec.specList[i].InspectionType == InspType.MintoMax)
                {
                    sParam += string.Format("{0},", "");
                    sParam += string.Format("{0},", Spec.specList[i].MaxSpec);
                }
                else
                {
                    sParam += string.Format("{0},", Spec.specList[i].MaxSpec);
                }
                
            }
            sHeader += sParam;

            writer.WriteLine(sHeader);

            writer.Close();
        }
        public void WriteResult(int port)
        {
            try
            {
                string dateDir = STATIC.CreateDateDir();
                if (!Directory.Exists(dateDir))
                    Directory.CreateDirectory(dateDir);

                string path = string.Format("{0}res_{1}.csv", dateDir, DateTime.Now.ToString("yyMMdd"));

                if (!File.Exists(path))
                {
                    AddHeadResult(path);
                }

                int ch = port * 2;

                StreamWriter sw = File.AppendText(path);

                for (int j = ch; j < ch + ChannelCnt; j++)
                {
                    string log = "";
                    if (errMsg[j] == "I2C Fail" || errMsg[j] == "Socket Empty\r\nVision Check") { yield.TotlaTested--; continue; }

                    
                    AddLog(j, string.Format("ch : {0}, msg : {1}, PassFail : {2}", j, errMsg[j], PassFails[j].FirstFailIndex));
                    string ActID_M = string.Empty;
                    for (int i = 0; i < STATIC.ActID_Memory.Length; i++) ActID_M += STATIC.ActID_Memory[i].ToString("X2");
                   
                    log += string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},",
                        STATIC.LogDate.ToString("yyyy-MM-dd"), $"{STATIC.LogDate.Hour}h{STATIC.LogDate.Minute}m{STATIC.LogDate.Second}s", m_StrIndex[j], "", Model.LotID, STATIC.ActID, $"\t{ActID_M}", Model.MCNum,Model.TesterNo, PassFails[j].FirstFailIndex);

                    yield.TotlaTested++;
                    //1st Fail Item
                    if (PassFails[j].FirstFailIndex > 0)
                    {
                        errMsg[j] = PassFails[j].FirstFail;
                        yield.TotlaFailed++;
                        AddLog(j, "Fail : " + errMsg[j]);
                        log += errMsg[j] + ",";
                        var Item = STATIC.Rcp.YieldItem.FirstOrDefault(x => x.ItemName == errMsg[j]);
                        if (Item != null) Item.FailCnt++;
                        else STATIC.Rcp.YieldItem.Add(new NewYield { ItemName = errMsg[j], FailCnt = 1 });
                    }
                    else if (PassFails[j].FirstFailIndex < 0)
                    {
                        errMsg[j] = PassFails[j].FirstFail;
                        yield.TotlaFailed++;
                        log += errMsg[j] + ",";
                        var Item = STATIC.Rcp.YieldItem.FirstOrDefault(x => x.ItemName == errMsg[j]);
                        if (Item != null) Item.FailCnt++;
                        else STATIC.Rcp.YieldItem.Add(new NewYield { ItemName = errMsg[j], FailCnt = 1 });
                    }
                    else
                    {
                        if (m_ChannelOn[j])
                        {
                            yield.TotlaPassed++;
                            log += "PASS" + ",";
                        }
                        else
                        {
                            log += "NONE" + ",";
                        }
                    }

                    //  X Results

                    for (int i = 0; i < (int)SpecItem.Length; i++)
                    {


                        switch (Rcp.Spec.specList[i].InspectionType)
                        {
                            case InspType.Normal:
                            case InspType.OnlyMax:
                            case InspType.OnlyMin:

                                if (PassFails[j].Results[i].Val == double.MaxValue) log += " ,";
                                else log += string.Format("{0:0.000},", PassFails[j].Results[i].Val);


                                break;
                            case InspType.OKNG:
                                if (PassFails[j].Results[i].Val == 0) log += string.Format("OK") + ",";
                                else if (PassFails[j].Results[i].Val == double.MaxValue) log += string.Format(" ") + ",";
                                else log += string.Format("NG") + ",";
                                break;

                            case InspType.MintoMax:
                                if (i == (int)SpecItem.AF_Current)
                                { 
                                    log += $"{AFCurrentMinMax[1].ToString("F3")},";
                                    log += $"{AFCurrentMinMax[0].ToString("F3")},";
                                }
                                else if (i == (int)SpecItem.OISX_Current)
                                {
                                    log += $"{OISXCurrentMinMax[1].ToString("F3")},";
                                    log += $"{OISXCurrentMinMax[0].ToString("F3")},";
                                }
                               
                                else if (i == (int)SpecItem.OISY_Current)
                                {
                                    log += $"{OISYCurrentMinMax[1].ToString("F3")},";
                                    log += $"{OISYCurrentMinMax[0].ToString("F3")},";

                                }
                                break;
                        }




                    }

                    log += $"{STATIC.PosturePos},";

                    //Time
                    for (int i = 0; i < ItemList.Count; i++)
                    {

                        log += string.Format("{0:0.000},", ItemList[i].Time);
                    }

                    log += string.Format("{0:0.000},", PassFails[ch].TotalTime);

                    sw.WriteLine(log);
                }
                sw.Close();

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
                m_ChannelOn[0] = false;
                errMsg[0] = "Check the Result File Open!!!";
            }
        }
        private void Act_ScanCode(int port, string testItem, int InspCnt)
        {
            MakeWaveform(testItem);
            LEDs_All_On(port, true);
            Process_ScanCodeTest(port, testItem, InspCnt);
            LEDs_All_On(port, false);
            Process_CalcCodeTest(port, testItem, InspCnt);

        }
        private void Act_ScanTimeCode(int port, string testItem, int InspCnt)
        {
            for (int i = 0; i < 50; i++)
            {
                if (Condition.AFSettling[i].UseFlag)
                {
                    int Startcode = Condition.AFSettling[i].StartCode;
                    int Endcode = Condition.AFSettling[i].EndCode;
                    double criteria = Condition.AFSettling[i].Criteria;
                    int TryCnt = Condition.AFSettling[i].TryCount;
                    double res = 0;
                    Wait(100);
                    for (int j = 0; j < TryCnt; j++)
                    {
                        LEDs_All_On(port, true);
                        Process_ScanTimeTest(port, testItem, Startcode, Endcode);
                        LEDs_All_On(port, false);
                       res =  Process_CalcTimeTest(port, testItem, Startcode, Endcode, criteria, TryCnt, i + 1);
                    }

                    PassFails[0].Results[(int)(SpecItem.AF_SettillingTime1 + i)].Val = res;
                    ShowDataResults(0, (int)(SpecItem.AF_SettillingTime1 + i), (int)(SpecItem.AF_SettillingTime1 + i), InspType.OnlyMax, new double[] { });


                }
            }
         

        }
        FindResult Measure()
        {
            FindResult res = new FindResult();

            STATIC.fVision.m__G.oCam[0].Grab(0);
            res = STATIC.fVision.MeasureTxTyTz(0);
            return res; 
        }

 

        #endregion


    }
    public class OISCalParameter
    {

        public int XHallCurrent { get; set; }
        public int YHallCurrent { get; set; }
        public int XHallOfsDAC { get; set; }
        public int YHallOfsDAC { get; set; }
        public int XhallMin { get; set; }
        public int XhallMax { get; set; }
        public int XhallMid { get; set; }

        public int YhallMin { get; set; }
        public int YhallMax { get; set; }
        public int YhallMid { get; set; }


    }
}
