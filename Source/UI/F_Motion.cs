using Basler.Pylon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;

namespace FZ4P
{
    public partial class F_Motion : Form
    {
        public MotionData motData = new MotionData();
     
        public F_Main MyOwner = null;
        double[] AxisStroke = new double[2] { 0, 0 };
        public double[] AxisCurrrent = new double[2] { 0, 0 };
        bool JogMode = false;
        public bool isFirstHomeSearchingAxis0 = true;
        public bool isFirstHomeSearchingAxis1 = true;
        F_MotionMsg mesCon = new F_MotionMsg();
        int axisCnt = 0;
        public const double DegreeToPulse = 0.1;
        public F_Motion()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            STATIC.State = (int)STATIC.STATE.Manage;
        }
        public void Init()
        {
            if (STATIC.Rcp.Model.MCType != "Posture_M")
                return;
            if (File.Exists(STATIC.MotionDir)) motData = DataIO.DeserializeXMLFileToObject<MotionData>(STATIC.MotionDir);

            AxisStroke = new double[2] { 4.1625, 4.1625 };
            if (AjinLibrary.AxlOpenNoReset(7) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                InitGrid();
                InitPosList();
                AddAxisInfo();
                checkBox1.Checked = motData.HWLimitUse;
            }

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(10);
                 
                    if (InvokeRequired)
                    {
                        BeginInvoke((MethodInvoker)delegate
                        {
                            Display();
                        });
                    }
                }
            });
        }
        void InitGrid()
        {
            Type dgv = ParameterGrid.GetType();
            PropertyInfo pi = dgv.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(ParameterGrid, true, null);

            Type dgv2 = PosGrid.GetType();
            PropertyInfo pi2 = dgv.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(PosGrid, true, null);

            ParameterGrid.ColumnCount = 5;
            ParameterGrid.Font = new Font("Arial", 9, FontStyle.Bold);
            for (int i = 0; i < ParameterGrid.ColumnCount; i++)
                ParameterGrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            ParameterGrid.RowHeadersVisible = false;
            ParameterGrid.BackgroundColor = Color.LightGray;
            ParameterGrid.Columns[0].Name = "Category";
            ParameterGrid.Columns[1].Name = "Item";
            ParameterGrid.Columns[2].Name = "Block θ";
            ParameterGrid.Columns[3].Name = "Socket θ";
            ParameterGrid.Columns[4].Name = "unit";

            ParameterGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ParameterGrid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ParameterGrid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            ParameterGrid.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            ParameterGrid.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            ParameterGrid.Columns[0].Width = 100;
            ParameterGrid.Columns[1].Width = 105;
            ParameterGrid.Columns[2].Width = 85;
            ParameterGrid.Columns[3].Width = 85;
            ParameterGrid.Columns[4].Width = 50;
            ParameterGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ParameterGrid.ColumnHeadersHeight = 22;

            int effRowNum = 0;
            bool bColorChange = true;
            ParameterGrid.Rows.Clear();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(motData);
            for (int i = 0; i < props.Count; i++)
            {
                string Category = DataIO.GetCustomAttribute<MotionAttribute>(props[i])?.Category;
                string Name = DataIO.GetCustomAttribute<MotionAttribute>(props[i])?.DisplayName;
                string Unit = DataIO.GetCustomAttribute<MotionAttribute>(props[i])?.Unit;

                if (Category != "NotUse")
                {
                    double[] d = (double[])props[i]?.GetValue(motData);

                    ParameterGrid.Rows.Add(Category, Name, d[0].ToString(), d[1].ToString(), Unit);
                    if (i != 0)
                    {
                        string beforeCate = DataIO.GetCustomAttribute<MotionAttribute>(props[i - 1])?.Category;
                        if (beforeCate != Category) bColorChange = !bColorChange;
                        if (bColorChange)
                        {
                            ParameterGrid[0, effRowNum].Style.BackColor = Color.Lavender;
                            ParameterGrid[1, effRowNum].Style.BackColor = Color.Lavender;
                            ParameterGrid[4, effRowNum].Style.BackColor = Color.Lavender;
                        }
                        else
                        {
                            ParameterGrid[0, effRowNum].Style.BackColor = Color.White;
                            ParameterGrid[1, effRowNum].Style.BackColor = Color.White;
                            ParameterGrid[4, effRowNum].Style.BackColor = Color.White;
                        }
                        if (beforeCate == Category) ParameterGrid.Rows[effRowNum].Cells[0].Style.ForeColor = ParameterGrid.Rows[effRowNum].Cells[0].Style.BackColor;
                        effRowNum++;
                    }

                }
            }
            for (int i = 0; i < effRowNum; i++)
            {
                ParameterGrid.Rows[i].Height = 18;
                ParameterGrid.Rows[i].Resizable = DataGridViewTriState.False;
                ParameterGrid.Rows[i].DefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
                ParameterGrid[1, i].Style.Font = new Font("Arial", 9, FontStyle.Bold);
                ParameterGrid[2, i].Style.Font = new Font("Arial", 9, FontStyle.Bold);
                ParameterGrid[4, i].Style.Font = new Font("Arial", 9, FontStyle.Italic);
            }

            for (int colum = 2; colum < 4; colum++)
            {
                for (int row = 0; row < effRowNum; row++)
                {
                    ParameterGrid[colum, row].Style.BackColor = Color.LightGray;

                }
            }
            ParameterGrid.ReadOnly = true;
            cb_Edit.Checked = false;

            //////////////////////////// Position Setting 
            PosGrid.ColumnCount = 4;
            PosGrid.Font = new Font("Arial", 9, FontStyle.Bold);
            for (int i = 0; i < PosGrid.ColumnCount; i++)
                PosGrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            PosGrid.RowHeadersVisible = false;
            PosGrid.BackgroundColor = Color.LightGray;

            PosGrid.Columns[0].Name = "Position";
            PosGrid.Columns[1].Name = "Block θ";
            PosGrid.Columns[2].Name = "Socket θ";
            PosGrid.Columns[3].Name = "unit";

            DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn();
            chkCol.ValueType = typeof(bool);
            chkCol.HeaderText = "On/Off";
            PosGrid.Columns.Add(chkCol);
            DataGridViewTextBoxColumn tmpcol = new DataGridViewTextBoxColumn();
            tmpcol.ValueType = typeof(string);
            tmpcol.HeaderText = "Name";
            PosGrid.Columns.Add(tmpcol);


            for (int i = 0; i < 6; i++)
                PosGrid.Columns[i].DefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);

            PosGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            PosGrid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            PosGrid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            PosGrid.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            PosGrid.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            PosGrid.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            PosGrid.Columns[0].Width = 110;
            PosGrid.Columns[1].Width = 75;
            PosGrid.Columns[2].Width = 75;
            PosGrid.Columns[3].Width = 65;
            PosGrid.Columns[4].Width = 50;
            PosGrid.Columns[5].Width = 70;

            PosGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            PosGrid.ColumnHeadersHeight = 22;

            PosGrid.Rows.Clear();

            for (int i = 0; i < props.Count; i++)
            {
                string Category = DataIO.GetCustomAttribute<MotionAttribute>(props[i])?.Category;
                string Name = DataIO.GetCustomAttribute<MotionAttribute>(props[i])?.DisplayName;

                if (Category == "NotUse" && Name == "PosList")
                {
                    List<DrvPosInfo> d = (List<DrvPosInfo>)props[i]?.GetValue(motData);
                    for (int j = 0; j < d.Count; j++)
                    {
                        PosGrid.Rows.Add("Position" + (j + 1), d[j].Deg[0].ToString(), d[j].Deg[1].ToString(), "deg", d[j].Enable, d[j].PositionName);
                        PosGrid[0, j].Style.BackColor = Color.White;
                        PosGrid[1, j].Style.BackColor = Color.White;
                        PosGrid.Rows[j].Height = 20;
                        PosGrid.Rows[j].Resizable = DataGridViewTriState.False;
                        PosGrid.Rows[j].DefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
                        PosGrid[0, j].Style.Font = new Font("Arial", 9, FontStyle.Bold);
                        PosGrid[1, j].Style.Font = new Font("Arial", 9, FontStyle.Bold);
                        PosGrid[3, j].Style.Font = new Font("Arial", 9, FontStyle.Italic);
                    }

                    for (int colum = 1; colum < 3; colum++)
                    {
                        for (int row = 0; row < d.Count; row++)
                        {
                            PosGrid[colum, row].Style.BackColor = Color.LightGray;

                        }
                    }
                    for (int row = 0; row < d.Count; row++)
                    {
                        PosGrid[4, row].Style.BackColor = Color.LightGray;
                        PosGrid[5, row].Style.BackColor = Color.LightGray;

                    }
                    break;
                }
            }


            PosGrid.ReadOnly = true;
            cbInspPosEdit.Checked = false;

            for (int i = 0; i < PosGrid.RowCount; i++)
            {
                if (!motData.MotionPosList[i].Enable)
                {
                    PosGrid[1, i].Style.BackColor = Color.OrangeRed;
                    PosGrid[2, i].Style.BackColor = Color.OrangeRed;

                }
            }

            cbAxis0SWLimitUse.Checked = motData.SWLimitUse[0];
            cbAxis1SWLimitUse.Checked = motData.SWLimitUse[1];


        }
        void UpdateGrid()
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(motData);
            for (int i = 0; i < ParameterGrid.RowCount; i++)
            {
                double[] val = (double[])props.Cast<PropertyDescriptor>().FirstOrDefault(p =>
                p.Attributes.OfType<MotionAttribute>().Any(att => att.Category == ParameterGrid[0, i].Value.ToString()) &&
                p.Attributes.OfType<MotionAttribute>().Any(att => att.DisplayName == ParameterGrid[1, i].Value.ToString())).GetValue(motData);
                ParameterGrid[2, i].Value = val[0];
                ParameterGrid[3, i].Value = val[1];

            }
            List<double[]> PosList = new List<double[]>();
            for (int i = 0; i < props.Count; i++)
            {
                string Category = DataIO.GetCustomAttribute<MotionAttribute>(props[i])?.Category;
                string Name = DataIO.GetCustomAttribute<MotionAttribute>(props[i])?.DisplayName;

                if (Category == "NotUse" && Name == "PosList")
                {
                    PosList = (List<double[]>)props[i]?.GetValue(motData);
                    break;
                }
            }

            for (int i = 0; i < PosGrid.RowCount; i++)
            {
                PosGrid[1, i].Value = PosList[i][0];
                PosGrid[2, i].Value = PosList[i][1];

            }
        }
        void UpdataeData(string path)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(motData);
            for (int i = 0; i < ParameterGrid.RowCount; i++)
            {
                var found = props.Cast<PropertyDescriptor>().FirstOrDefault(prop =>
                ((MotionAttribute)prop.Attributes[typeof(MotionAttribute)])?.DisplayName == ParameterGrid[1, i].Value.ToString() &&
                ((MotionAttribute)prop.Attributes[typeof(MotionAttribute)])?.Category == ParameterGrid[0, i].Value.ToString());
                double[] valArr = new double[2] { Convert.ToDouble(ParameterGrid[2, i].Value), Convert.ToDouble(ParameterGrid[3, i].Value) };
                found.SetValue(motData, valArr);
            }
            for (int i = 0; i < PosGrid.RowCount; i++)
            {
                if (i == 0)
                {
                    motData.MotionPosList[i].Deg[0] = Convert.ToDouble(PosGrid[1, i].Value);
                    motData.MotionPosList[i].Deg[1] = Convert.ToDouble(PosGrid[2, i].Value);
                    motData.MotionPosList[i].Enable = true;
                    motData.MotionPosList[i].PositionName = Convert.ToString(PosGrid[5, i].Value);

                }
                else
                {
                    motData.MotionPosList[i].Deg[0] = Convert.ToDouble(PosGrid[1, i].Value);
                    motData.MotionPosList[i].Deg[1] = Convert.ToDouble(PosGrid[2, i].Value);
                    motData.MotionPosList[i].Enable = Convert.ToBoolean(PosGrid[4, i].Value);
                    motData.MotionPosList[i].PositionName = Convert.ToString(PosGrid[5, i].Value);
                }
            }
            DataIO.SerializeToXMLFile(motData, path);
            if (motData.SWLimitUse[0])
                AjinLibrary.AxmSignalSetSoftLimit(0, 1, 1, 0, motData.SWLimitP[0] * AxisStroke[0] / DegreeToPulse, motData.SWLimitN[0] * AxisStroke[0] / DegreeToPulse);
            if (motData.SWLimitUse[1])
                AjinLibrary.AxmSignalSetSoftLimit(1, 1, 1, 0, motData.SWLimitP[1] * AxisStroke[1] / DegreeToPulse, motData.SWLimitN[1] * AxisStroke[1] / DegreeToPulse);


        }
        void AddAxisInfo()
        {
            AjinLibrary.AxmHomeSetSignalLevel(0, 1);
            AjinLibrary.AxmHomeSetSignalLevel(1, 1);
            AjinLibrary.AxmInfoGetAxisCount(ref axisCnt);
            AjinLibrary.AxmSignalSetStop(0, 0, 2);
            AjinLibrary.AxmSignalSetStop(1, 0, 2);

            AjinLibrary.AxmMotSetPulseOutMethod(0, 4);
            AjinLibrary.AxmMotSetPulseOutMethod(1, 4);
            AjinLibrary.AxmSignalSetLimit(0, 0, 2, 2);
            AjinLibrary.AxmSignalSetLimit(1, 0, 2, 2);
            for (int i = 0; i < 2; i++)
            {
                if (motData.SWLimitUse[i])
                    AjinLibrary.AxmSignalSetSoftLimit(i, 1, 1, 0, motData.SWLimitP[i] * AxisStroke[i] / DegreeToPulse, motData.SWLimitN[i] * AxisStroke[i] / DegreeToPulse);
                else AjinLibrary.AxmSignalSetSoftLimit(i, 0, 1, 0, motData.SWLimitP[i] * AxisStroke[i] / DegreeToPulse, motData.SWLimitN[i] * AxisStroke[i] / DegreeToPulse);
            }

        }
        void InitPosList()
        {
            lstPosition.Items.Clear();
            for (int i = 0; i < motData.MotionPosList.Count; i++)
            {
                lstPosition.Items.Add("position " + (i + 1));
            }
        }
        public bool EMGOn = false;
        private void Display()
        {
            int HomeStatus = 7;
            int EmgStatus = 6;
            int[] iDigitReadMotion = { 0, 1, 2, 3, 11 };
            double dcmdPos = 0.0;
            double dVelocity = 0.0;
            int iCheck = 0;
            uint duRetcode = 0;
            uint duStatus = 0;
            uint duHome = 0;

            uint duHomeSearch = 0;

            for (int i = 0; i < axisCnt; i++)
            {
                AjinLibrary.AxmStatusGetCmdPos(i, ref dcmdPos);
                AjinLibrary.AxmStatusReadVel(i, ref dVelocity);
                AjinLibrary.AxmStatusReadMechanical(i, ref duHome);
                AjinLibrary.AxmHomeGetResult(i, ref duHomeSearch);

                duRetcode = AjinLibrary.AxmStatusReadMotion(i, ref duStatus);

                if (i == 0)
                {
                    lblCommandPosAxis0.Text = String.Format("{0:0.000}", AxisCurrrent[0] = dcmdPos * DegreeToPulse / AxisStroke[0]);
                    lblCommandVelocityAxis0.Text = String.Format("{0:0.000}", dVelocity * DegreeToPulse / AxisStroke[0]);


                    if (duHomeSearch == (int)AXT_MOTION_HOME_RESULT.HOME_SUCCESS || duHomeSearch == (int)AXT_MOTION_HOME_RESULT.HOME_ERR_UNKNOWN)
                    {
                        lblHomeSearchAxis0.BackColor = Color.Transparent;
                        AjinLibrary.AxmSignalSetLimit(0, 0, 2, 2);

                    }
                    else if (duHomeSearch == (int)AXT_MOTION_HOME_RESULT.HOME_SEARCHING)
                    {
                        lblHomeSearchAxis0.BackColor = Color.YellowGreen;
                    }
                    else
                    {
                        lblHomeSearchAxis0.BackColor = Color.Red;

                    }

                    iCheck = ((int)duHome >> HomeStatus & 0x1);
                    if (iCheck == 0)
                    {
                        lblHomeAxis0.BackColor = Color.Transparent;
                    }
                    else
                    {
                        lblHomeAxis0.BackColor = Color.YellowGreen;
                    }

                    //iCheck = ((int)duHome >> EmgStatus & 0x1);
                    //if (iCheck == 0)
                    //{
                    //    EMGOn = false;
                    //    lblEmgStopAxis0.BackColor = Color.Transparent;
                    //}
                    //else
                    //{

                    //    EMGOn = true;
                    //    lblEmgStopAxis0.BackColor = Color.Red;
                    //}


                    if (motData.SWLimitUse[0])
                    {
                        if (Convert.ToDouble(lblCommandPosAxis0.Text) >= motData.SWLimitP[0])
                            lblMotionLimitAxis0.BackColor = Color.YellowGreen;
                        else if (Convert.ToDouble(lblCommandPosAxis0.Text) <= motData.SWLimitN[0])
                            lblMotionLimitAxis0.BackColor = Color.Red;
                        else
                            lblMotionLimitAxis0.BackColor = Color.Transparent;
                    }
                    else
                        lblMotionLimitAxis0.BackColor = Color.Transparent;


                    if (duRetcode == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            iCheck = ((int)duStatus >> iDigitReadMotion[j] & 0x1);

                            switch (j)
                            {
                                case 0:
                                    if (iCheck == 1)
                                        lblMotionBusyAxis0.BackColor = Color.YellowGreen;
                                    else
                                        lblMotionBusyAxis0.BackColor = Color.Transparent;
                                    break;
                                case 1:
                                    if (iCheck == 1)
                                        lblMotionDecelAxis0.BackColor = Color.YellowGreen;
                                    else
                                        lblMotionDecelAxis0.BackColor = Color.Transparent;
                                    break;
                                case 2:
                                    if (iCheck == 1)
                                        lblMotionConstantAxis0.BackColor = Color.YellowGreen;
                                    else
                                        lblMotionConstantAxis0.BackColor = Color.Transparent;
                                    break;
                                case 3:
                                    if (iCheck == 1)
                                        lblMotionAccAxis0.BackColor = Color.YellowGreen;
                                    else
                                        lblMotionAccAxis0.BackColor = Color.Transparent;
                                    break;
                                case 4:
                                    if (lblMotionBusyAxis0.BackColor == Color.YellowGreen)
                                    {
                                        if (iCheck == 1)
                                            lblMotionDirectionAxis0.BackColor = Color.Red;
                                        else
                                            lblMotionDirectionAxis0.BackColor = Color.YellowGreen;
                                        break;
                                    }
                                    else
                                        lblMotionDirectionAxis0.BackColor = Color.Transparent;
                                    break;

                                default:
                                    break;
                            }


                        }
                    }
                }
                else if (i == 1)
                {
                    lblCommandPosAxis1.Text = String.Format("{0:0.000}", AxisCurrrent[1] = dcmdPos * DegreeToPulse / AxisStroke[1]);
                    lblCommandVelocityAxis1.Text = String.Format("{0:0.000}", dVelocity * DegreeToPulse / AxisStroke[1]);


                    if (duHomeSearch == (int)AXT_MOTION_HOME_RESULT.HOME_SUCCESS || duHomeSearch == (int)AXT_MOTION_HOME_RESULT.HOME_ERR_UNKNOWN)
                    { lblHomeSearchAxis1.BackColor = Color.Transparent; AjinLibrary.AxmSignalSetLimit(1, 0, 2, 2); }
                    else if (duHomeSearch == (int)AXT_MOTION_HOME_RESULT.HOME_SEARCHING)
                        lblHomeSearchAxis1.BackColor = Color.YellowGreen;
                    else
                    { lblHomeSearchAxis1.BackColor = Color.Red; }

                    iCheck = ((int)duHome >> HomeStatus & 0x1);
                    if (iCheck == 0)
                        lblHomeAxis1.BackColor = Color.Transparent;
                    else
                        lblHomeAxis1.BackColor = Color.YellowGreen;

                    if (motData.SWLimitUse[1])
                    {
                        if (Convert.ToDouble(lblCommandPosAxis1.Text) >= motData.SWLimitP[1])
                            lblMotionLimitAxis1.BackColor = Color.YellowGreen;
                        else if (Convert.ToDouble(lblCommandPosAxis1.Text) <= motData.SWLimitN[1])
                            lblMotionLimitAxis1.BackColor = Color.Red;
                        else
                            lblMotionLimitAxis1.BackColor = Color.Transparent;
                    }
                    else
                        lblMotionLimitAxis1.BackColor = Color.Transparent;

                    if (duRetcode == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            iCheck = ((int)duStatus >> iDigitReadMotion[j] & 0x1);

                            switch (j)
                            {
                                case 0:
                                    if (iCheck == 1)
                                        lblMotionBusyAxis1.BackColor = Color.YellowGreen;

                                    else
                                        lblMotionBusyAxis1.BackColor = Color.Transparent;
                                    break;
                                case 1:
                                    if (iCheck == 1)
                                        lblMotionDecelAxis1.BackColor = Color.YellowGreen;
                                    else
                                        lblMotionDecelAxis1.BackColor = Color.Transparent;
                                    break;
                                case 2:
                                    if (iCheck == 1)
                                        lblMotionConstantAxis1.BackColor = Color.YellowGreen;
                                    else
                                        lblMotionConstantAxis1.BackColor = Color.Transparent;
                                    break;
                                case 3:
                                    if (iCheck == 1)
                                        lblMotionAccAxis1.BackColor = Color.YellowGreen;
                                    else
                                        lblMotionAccAxis1.BackColor = Color.Transparent;
                                    break;
                                case 4:
                                    if (lblMotionBusyAxis1.BackColor == Color.YellowGreen)
                                    {
                                        if (iCheck == 1)
                                            lblMotionDirectionAxis1.BackColor = Color.Red;
                                        else
                                            lblMotionDirectionAxis1.BackColor = Color.YellowGreen;
                                        break;
                                    }
                                    else
                                        lblMotionDirectionAxis1.BackColor = Color.Transparent;
                                    break;
                                default:
                                    break;
                            }
                        }


                    }
                }
            }

        }
        private void F_Motion_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (STATIC.Rcp.Model.MCType == "Posture_M")
                    return;
                AjinLibrary.AxmMoveEStop(0);
                AjinLibrary.AxmMoveEStop(1);
                AjinLibrary.AxlClose();
            }
            catch { }
        }
        void AddPosition()
        {
            if (motData.MotionPosList.Count > 0)
            {
                motData.MotionPosList.Add(new DrvPosInfo
                {
                    Deg = new double[2] { 0, 0 },
                    Enable = false,
                    PositionName = string.Empty,
                });
            }
            else
            {
                motData.MotionPosList.Add(new DrvPosInfo
                {
                    Deg = new double[2] { 0, 0 },
                    Enable = true,
                    PositionName = string.Empty,
                });
            }

            lstPosition.Items.Add("Position " + motData.MotionPosList.Count);
            if (motData.MotionPosList.Count > 1)
                PosGrid.Rows.Add("Position " + motData.MotionPosList.Count, "0.0", "0.0", "deg", false, "");
            else PosGrid.Rows.Add("Position " + motData.MotionPosList.Count, "0.0", "0.0", "deg", true, "");

            cbInspPosEdit.Checked = false;
            PosGrid.ReadOnly = true;

            PosGrid.ReadOnly = true;
            for (int i = 0; i < PosGrid.Rows.Count; i++)
            {
                PosGrid[1, i].Style.BackColor = Color.LightGray;
                PosGrid[2, i].Style.BackColor = Color.LightGray;
                PosGrid[4, i].Style.BackColor = Color.LightGray;
                PosGrid[5, i].Style.BackColor = Color.LightGray;
            }

            for (int i = 0; i < PosGrid.RowCount; i++)
            {
                if (!motData.MotionPosList[i].Enable)
                {
                    PosGrid[1, i].Style.BackColor = Color.OrangeRed;
                    PosGrid[2, i].Style.BackColor = Color.OrangeRed;

                }
            }
            DataIO.SerializeToXMLFile(motData, STATIC.MotionDir);
        }
        void RemovePosition()
        {
            if (PosGrid.RowCount == 1) return;
            if (motData.MotionPosList.Count > 0)
            {
                motData.MotionPosList.RemoveAt(motData.MotionPosList.Count - 1);
                lstPosition.Items.RemoveAt(lstPosition.Items.Count - 1);
                PosGrid.Rows.Remove(PosGrid.Rows[PosGrid.RowCount - 1]);
                for (int i = 0; i < PosGrid.RowCount; i++)
                {
                    if (!motData.MotionPosList[i].Enable)
                    {
                        PosGrid[1, i].Style.BackColor = Color.OrangeRed;
                        PosGrid[2, i].Style.BackColor = Color.OrangeRed;

                    }
                }
            }
            DataIO.SerializeToXMLFile(motData, STATIC.MotionDir);
        }

        public (bool isUse, double[] d, string PosName) MoveSelectedPosition(int PosIndex)
        {

            
            if (EMGOn) return (false, new double[2] { 0, 0 }, string.Empty);

            if (PosIndex != -1 && !motData.MotionPosList[PosIndex].Enable) return (false, new double[2] { 0, 0 }, string.Empty);
          
            double[] degree = new double[2];
            uint ret = 0;
            int[] AxisNo = { 0, 1 };
            double[] dMultiPos = { 0.0, 0.0 }, dMultiVel = { 0.0, 0.0 },
                    dMultiAcc = { 0.0, 0.0 }, dMultiDec = { 0.0, 0.0 };
            double[] dCmdPos = { 0.0, 0.0 };
            string PosName = string.Empty;
            if(PosIndex == -1)
            {
                dMultiPos[0] = 0;/*(degree[0] = motData.MotionPosList[PosIndex].Deg[0]) * AxisStroke[0] / DegreeToPulse;*/
                dMultiPos[1] = 0;/*(degree[1] = motData.MotionPosList[PosIndex].Deg[1]) * AxisStroke[1] / DegreeToPulse;*/

            }
            else
            {
                dMultiPos[0] = (degree[0] = motData.MotionPosList[PosIndex].Deg[0]) * AxisStroke[0] / DegreeToPulse;
                dMultiPos[1] = (degree[1] = motData.MotionPosList[PosIndex].Deg[1]) * AxisStroke[1] / DegreeToPulse;
                PosName = motData.MotionPosList[PosIndex].PositionName;
            }
     

            dMultiVel[0] = motData.Velocity[0] * AxisStroke[0] / DegreeToPulse;
            dMultiVel[1] = motData.Velocity[1] * AxisStroke[1] / DegreeToPulse;

            dMultiAcc[0] = motData.Acc[0] * AxisStroke[0] / DegreeToPulse;
            dMultiAcc[1] = motData.Acc[1] * AxisStroke[1] / DegreeToPulse;

            dMultiDec[0] = motData.Dec[0] * AxisStroke[0] / DegreeToPulse;
            dMultiDec[1] = motData.Dec[1] * AxisStroke[1] / DegreeToPulse;
            ret = AjinLibrary.AxmMoveStartMultiPos(2, AxisNo, dMultiPos, dMultiVel, dMultiAcc, dMultiDec);
            return (true, degree, PosName);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            UpdataeData(STATIC.MotionDir);
        }
        private void btnJogNAxis0_MouseDown(object sender, MouseEventArgs e)
        {
            if (EMGOn) return;
            uint duRetCode = 0;
            double dVelocity = 0.0;
            double dAccel = 0.0;
            double dDecel = 0.0;
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "btnJogPAxis0":
                    dVelocity = motData.Velocity[0] * AxisStroke[0] / DegreeToPulse;
                    dAccel = motData.Acc[0] * AxisStroke[0] / DegreeToPulse;
                    dDecel = motData.Dec[0] * AxisStroke[0] / DegreeToPulse;
                    duRetCode = AjinLibrary.AxmMoveVel(0, dVelocity, dAccel, dDecel);
                    break;
                case "btnJogNAxis0":
                    dVelocity = motData.Velocity[0] * AxisStroke[0] / DegreeToPulse;
                    dAccel = motData.Acc[0] * AxisStroke[0] / DegreeToPulse;
                    dDecel = motData.Dec[0] * AxisStroke[0] / DegreeToPulse;
                    duRetCode = AjinLibrary.AxmMoveVel(0, -dVelocity, dAccel, dDecel);
                    break;
                case "btnJogPAxis1":
                    dVelocity = motData.Velocity[1] * AxisStroke[1] / DegreeToPulse;
                    dAccel = motData.Acc[1] * AxisStroke[1] / DegreeToPulse;
                    dDecel = motData.Dec[1] * AxisStroke[1] / DegreeToPulse;
                    duRetCode = AjinLibrary.AxmMoveVel(1, dVelocity, dAccel, dDecel);
                    break;
                case "btnJogNAxis1":
                    dVelocity = motData.Velocity[1] * AxisStroke[1] / DegreeToPulse;
                    dAccel = motData.Acc[1] * AxisStroke[1] / DegreeToPulse;
                    dDecel = motData.Dec[1] * AxisStroke[1] / DegreeToPulse;
                    duRetCode = AjinLibrary.AxmMoveVel(1, -dVelocity, dAccel, dDecel);
                    break;
                default:
                    break;
            }
            if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                JogMode = false;

            else
                JogMode = true;
        }
        private void btnJogNAxis0_MouseUp(object sender, MouseEventArgs e)
        {
            if (EMGOn) return;
            uint duRetCode = 0;
            Button btn = (Button)sender;
            if (JogMode)
            {
                switch (btn.Name)
                {
                    case "btnJogPAxis0":
                        duRetCode = AjinLibrary.AxmMoveSStop(0);
                        break;
                    case "btnJogNAxis0":
                        duRetCode = AjinLibrary.AxmMoveSStop(0);
                        break;
                    case "btnJogPAxis1":
                        duRetCode = AjinLibrary.AxmMoveSStop(1);
                        break;
                    case "btnJogNAxis1":
                        duRetCode = AjinLibrary.AxmMoveSStop(1);
                        break;
                    default:
                        break;
                }

                JogMode = false;
            }
        }
        private void cb_Edit_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_Edit.Checked)
            {
                ParameterGrid.ReadOnly = false;
                for (int i = 0; i < ParameterGrid.RowCount; i++)
                {
                    ParameterGrid[0, i].ReadOnly = true;
                    ParameterGrid[1, i].ReadOnly = true;
                    ParameterGrid[4, i].ReadOnly = true;
                    ParameterGrid[2, i].Style.BackColor = Color.White;
                    ParameterGrid[3, i].Style.BackColor = Color.White;

                }
            }
            else
            {
                ParameterGrid.ReadOnly = true;
                for (int i = 0; i < ParameterGrid.RowCount; i++)
                {

                    ParameterGrid[2, i].Style.BackColor = Color.LightGray;
                    ParameterGrid[3, i].Style.BackColor = Color.LightGray;

                }
            }
        }
        private void cbInspPosEdit_CheckedChanged(object sender, EventArgs e)
        {
            if (cbInspPosEdit.Checked)
            {
                PosGrid.ReadOnly = false;
                for (int i = 0; i < PosGrid.RowCount; i++)
                {
                    //if (i == 0)
                    //{
                    //    PosGrid[0, i].ReadOnly = true;
                    //    PosGrid[1, i].ReadOnly = true;
                    //    PosGrid[2, i].ReadOnly = true;
                    //    PosGrid[3, i].ReadOnly = true;
                    //    PosGrid[4, i].ReadOnly = true;

                    //}

                    PosGrid[0, i].ReadOnly = true;
                    PosGrid[3, i].ReadOnly = true;
                    PosGrid[1, i].Style.BackColor = Color.White;
                    PosGrid[2, i].Style.BackColor = Color.White;
                    PosGrid[4, i].Style.BackColor = Color.White;
                    PosGrid[5, i].Style.BackColor = Color.White;

                }
            }
            else
            {
                PosGrid.ReadOnly = true;
                for (int i = 0; i < PosGrid.Rows.Count; i++)
                {
                    PosGrid[1, i].Style.BackColor = Color.LightGray;
                    PosGrid[2, i].Style.BackColor = Color.LightGray;
                    PosGrid[4, i].Style.BackColor = Color.LightGray;
                    PosGrid[5, i].Style.BackColor = Color.LightGray;
                }
            }

            for (int i = 0; i < PosGrid.Rows.Count; i++)
            {
                if (!Convert.ToBoolean(PosGrid[4, i].Value))
                {
                    PosGrid[1, i].Style.BackColor = Color.OrangeRed;
                    PosGrid[2, i].Style.BackColor = Color.OrangeRed;
                }
            }
        }
        private void cbAxis0SWLimitUse_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
            {
                switch (chk.Name)
                {
                    case "cbAxis0SWLimitUse":
                        cbAxis0SWLimitUse.BackColor = Color.YellowGreen;
                        motData.SWLimitUse[0] = true;
                        AjinLibrary.AxmSignalSetSoftLimit(0, 1, 1, 0, motData.SWLimitP[0] * AxisStroke[0] / DegreeToPulse, motData.SWLimitN[0] * AxisStroke[0] / DegreeToPulse);

                        break;
                    case "cbAxis1SWLimitUse":
                        cbAxis1SWLimitUse.BackColor = Color.YellowGreen;
                        motData.SWLimitUse[1] = true;
                        AjinLibrary.AxmSignalSetSoftLimit(1, 1, 1, 0, motData.SWLimitP[1] * AxisStroke[1] / DegreeToPulse, motData.SWLimitN[1] * AxisStroke[1] / DegreeToPulse);
                        break;
                    default:
                        break;

                }
            }
            else
            {
                switch (chk.Name)
                {
                    case "cbAxis0SWLimitUse":
                        cbAxis0SWLimitUse.BackColor = Color.Transparent;
                        motData.SWLimitUse[0] = false;
                        AjinLibrary.AxmSignalSetSoftLimit(0, 0, 1, 0, motData.SWLimitP[0] * AxisStroke[0] / DegreeToPulse, motData.SWLimitN[0] * AxisStroke[0] / DegreeToPulse);

                        break;
                    case "cbAxis1SWLimitUse":
                        cbAxis1SWLimitUse.BackColor = Color.Transparent;
                        motData.SWLimitUse[1] = false;
                        AjinLibrary.AxmSignalSetSoftLimit(1, 0, 1, 0, motData.SWLimitP[1] * AxisStroke[1] / DegreeToPulse, motData.SWLimitN[1] * AxisStroke[1] / DegreeToPulse);
                        break;
                    default:
                        break;

                }

            }
            DataIO.SerializeToXMLFile(motData, STATIC.MotionDir);
        }
        private void btnMoveManualPosAxis0_Click(object sender, EventArgs e)
        {
            if (EMGOn) return;
            uint duRetCode = 0;
            double dPosition = 0.0;
            double dVelocity = 0.0;
            double dAccel = 0.0;
            double dDecel = 0.0;
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "btnMoveManualPosAxis0":
                    dPosition = motData.TestPosition[0] * AxisStroke[0] / DegreeToPulse;
                    dVelocity = motData.Velocity[0] * AxisStroke[0] / DegreeToPulse;
                    dAccel = motData.Acc[0] * AxisStroke[0] / DegreeToPulse;
                    dDecel = motData.Dec[0] * AxisStroke[0] / DegreeToPulse;

                    duRetCode = AjinLibrary.AxmMoveStartPos(0, dPosition, dVelocity, dAccel, dDecel);

                    break;
                case "btnMoveManualPosAxis1":
                    dPosition = motData.TestPosition[1] * AxisStroke[1] / DegreeToPulse;
                    dVelocity = motData.Velocity[1] * AxisStroke[1] / DegreeToPulse;
                    dAccel = motData.Acc[1] * AxisStroke[1] / DegreeToPulse;
                    dDecel = motData.Dec[1] * AxisStroke[1] / DegreeToPulse;

                    duRetCode = AjinLibrary.AxmMoveStartPos(1, dPosition, dVelocity, dAccel, dDecel);
                    break;
                default:
                    break;
            }

        }
        private void btnMoveStopAxis0_Click(object sender, EventArgs e)
        {
            uint duRetCode = 0;

            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "btnMoveStopAxis0":
                    duRetCode = AjinLibrary.AxmMoveSStop(0);
                    AjinLibrary.AxmSignalSetLimit(0, 0, 2, 2);
                    break;
                case "btnMoveStopAxis1":
                    duRetCode = AjinLibrary.AxmMoveSStop(1);
                    AjinLibrary.AxmSignalSetLimit(1, 0, 2, 2);
                    break;
                default:
                    break;

            }
        }


        void MoveHomeDefault(Button btn)
        {
            if (EMGOn) return;
            bool isValueExist = false;
            uint duRetCode = 999;
            uint duHomeInfo = 0;

            switch (btn.Name)
            {
                case "btnMoveHomeAxis0":

                    int tmpint = (int)Math.Round(Convert.ToDouble(lblCommandPosAxis0.Text));
                    for (int i = 0; i < motData.MotionPosList.Count; i++)
                    {
                        if (tmpint == (int)Math.Round(motData.MotionPosList[i].Deg[0]))
                        {
                            isValueExist = true;
                            break;
                        }
                    }
                    if (isValueExist && !isFirstHomeSearchingAxis0)
                    {
                        AjinLibrary.AxmHomeSetVel(0, 7.2 * AxisStroke[0] / DegreeToPulse, 7.2 * AxisStroke[0] / DegreeToPulse, 1.44 * AxisStroke[0] / DegreeToPulse, 0.072 * AxisStroke[0] / DegreeToPulse, 28.8 * AxisStroke[0] / DegreeToPulse, 57.6 * AxisStroke[0] / DegreeToPulse);
                        if (motData.HomeOffsetN[0] > 40 && motData.HomeOffsetP[0] > 40)
                        {
                            if (Convert.ToDouble(lblCommandPosAxis0.Text) < -45)
                                AjinLibrary.AxmHomeSetMethod(0, 0, 4, 0, motData.ClearTime[0], motData.HomeOffsetN[0] * AxisStroke[0] / DegreeToPulse);
                            else
                                AjinLibrary.AxmHomeSetMethod(0, 1, 4, 0, motData.ClearTime[0], motData.HomeOffsetP[0] * AxisStroke[0] / DegreeToPulse);
                        }
                        else if (motData.HomeOffsetN[0] < -40 && motData.HomeOffsetP[0] < -40)
                        {
                            if (Convert.ToDouble(lblCommandPosAxis0.Text) < 45)
                                AjinLibrary.AxmHomeSetMethod(0, 0, 4, 0, motData.ClearTime[0], motData.HomeOffsetN[0] * AxisStroke[0] / DegreeToPulse);
                            else
                                AjinLibrary.AxmHomeSetMethod(0, 1, 4, 0, motData.ClearTime[0], motData.HomeOffsetP[0] * AxisStroke[0] / DegreeToPulse);
                        }
                        else
                        {
                            if (Convert.ToDouble(lblCommandPosAxis0.Text) < 0)
                                AjinLibrary.AxmHomeSetMethod(0, 0, 4, 0, motData.ClearTime[0], motData.HomeOffsetN[0] * AxisStroke[0] / DegreeToPulse);
                            else
                                AjinLibrary.AxmHomeSetMethod(0, 1, 4, 0, motData.ClearTime[0], motData.HomeOffsetP[0] * AxisStroke[0] / DegreeToPulse);
                        }

                        duRetCode = AjinLibrary.AxmHomeSetStart(0);
                        AjinLibrary.AxmHomeGetResult(0, ref duHomeInfo);
                    }
                    else
                    {
                        DialogResult dr = ShowMessageCon("Block의 현재위치를 0도 기준으로 방향 확인 후 구동해주세요.");
                        AjinLibrary.AxmHomeSetVel(0, 7.2 * AxisStroke[0] / DegreeToPulse, 7.2 * AxisStroke[0] / DegreeToPulse, 1.44 * AxisStroke[0] / DegreeToPulse, 0.072 * AxisStroke[0], 28.8 * AxisStroke[0] / DegreeToPulse, 57.6 * AxisStroke[0] / DegreeToPulse);
                        if (dr == DialogResult.Yes)
                        {
                            AjinLibrary.AxmHomeSetMethod(0, 0, 4, 0, motData.ClearTime[0], motData.HomeOffsetN[0] * AxisStroke[0] / DegreeToPulse);
                            duRetCode = AjinLibrary.AxmHomeSetStart(0);
                            AjinLibrary.AxmHomeGetResult(0, ref duHomeInfo);

                        }
                        else if (dr == DialogResult.No)
                        {
                            AjinLibrary.AxmHomeSetMethod(0, 1, 4, 0, motData.ClearTime[0], motData.HomeOffsetP[0] * AxisStroke[0] / DegreeToPulse);
                            duRetCode = AjinLibrary.AxmHomeSetStart(0);
                            AjinLibrary.AxmHomeGetResult(0, ref duHomeInfo);

                        }

                    }

                    break;
                case "btnMoveHomeAxis1":
                    int tmpint1 = (int)Math.Round(Convert.ToDouble(lblCommandPosAxis1.Text));
                    for (int i = 0; i < motData.MotionPosList.Count; i++)
                    {
                        if (tmpint1 == (int)Math.Round(motData.MotionPosList[i].Deg[1]))
                        {
                            isValueExist = true;
                            break;
                        }
                    }
                    if (isValueExist && !isFirstHomeSearchingAxis1)
                    {
                        AjinLibrary.AxmHomeSetVel(1, 7.2 * AxisStroke[1] / DegreeToPulse, 7.2 * AxisStroke[1] / DegreeToPulse, 1.44 * AxisStroke[1] / DegreeToPulse, 0.072 * AxisStroke[1] / DegreeToPulse, 28.8 * AxisStroke[1] / DegreeToPulse, 57.6 * AxisStroke[1] / DegreeToPulse);
                        if (motData.HomeOffsetN[1] > 40 && motData.HomeOffsetP[1] > 40)
                        {
                            if (Convert.ToDouble(lblCommandPosAxis1.Text) < -45)
                                AjinLibrary.AxmHomeSetMethod(1, 0, 4, 0, motData.ClearTime[1], motData.HomeOffsetN[1] * AxisStroke[1] / DegreeToPulse);
                            else
                                AjinLibrary.AxmHomeSetMethod(1, 1, 4, 0, motData.ClearTime[1], motData.HomeOffsetP[1] * AxisStroke[1] / DegreeToPulse);
                        }
                        else if (motData.HomeOffsetN[1] < -40 && motData.HomeOffsetP[1] < -40)
                        {
                            if (Convert.ToDouble(lblCommandPosAxis1.Text) < 45)
                                AjinLibrary.AxmHomeSetMethod(1, 0, 4, 0, motData.ClearTime[1], motData.HomeOffsetN[1] * AxisStroke[1] / DegreeToPulse);
                            else
                                AjinLibrary.AxmHomeSetMethod(1, 1, 4, 0, motData.ClearTime[1], motData.HomeOffsetP[1] * AxisStroke[1] / DegreeToPulse);
                        }
                        else
                        {
                            if (Convert.ToDouble(lblCommandPosAxis1.Text) < 0)
                                AjinLibrary.AxmHomeSetMethod(1, 0, 4, 0, motData.ClearTime[1], motData.HomeOffsetN[1] * AxisStroke[1] / DegreeToPulse);
                            else
                                AjinLibrary.AxmHomeSetMethod(1, 1, 4, 0, motData.ClearTime[1], motData.HomeOffsetP[1] * AxisStroke[1] / DegreeToPulse);

                        }
                        duRetCode = AjinLibrary.AxmHomeSetStart(1);
                        AjinLibrary.AxmHomeGetResult(1, ref duHomeInfo);
                    }
                    else
                    {
                        DialogResult dr = DialogResult.None;
                        dr = ShowMessageCon("Socket의 현재위치를 0도 기준으로 방향 확인 후 구동해주세요");


                        AjinLibrary.AxmHomeSetVel(1, 7.2 * AxisStroke[1] / DegreeToPulse, 7.2 * AxisStroke[1] / DegreeToPulse, 1.44 * AxisStroke[1] / DegreeToPulse, 0.072 * AxisStroke[1] / DegreeToPulse, 28.8 * AxisStroke[1] / DegreeToPulse, 57.6 * AxisStroke[1] / DegreeToPulse);
                        if (dr == DialogResult.Yes)
                        {
                            AjinLibrary.AxmHomeSetMethod(1, 0, 4, 0, motData.ClearTime[1], motData.HomeOffsetN[1] * AxisStroke[1] / DegreeToPulse);
                            duRetCode = AjinLibrary.AxmHomeSetStart(1);
                            AjinLibrary.AxmHomeGetResult(1, ref duHomeInfo);

                        }
                        else if (dr == DialogResult.No)
                        {
                            AjinLibrary.AxmHomeSetMethod(1, 1, 4, 0, motData.ClearTime[1], motData.HomeOffsetP[1] * AxisStroke[1] / DegreeToPulse);
                            duRetCode = AjinLibrary.AxmHomeSetStart(1);
                            AjinLibrary.AxmHomeGetResult(1, ref duHomeInfo);

                        }


                    }
                    break;
                default:
                    break;
            }
            if (duRetCode == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                switch (btn.Name)
                {
                    case "btnMoveHomeAxis0":
                        isFirstHomeSearchingAxis0 = false;
                        break;
                    case "btnMoveHomeAxis1":
                        isFirstHomeSearchingAxis1 = false;
                        break;
                }

            }
        }
        void MoveHomeLimitSensor(Button btn)
        {
            if (EMGOn) return;
            uint duRetCode = 999;
            uint duHomeInfo = 0;

            switch (btn.Name)
            {
                case "btnMoveHomeAxis0":
                    AjinLibrary.AxmHomeSetVel(0, 10 * AxisStroke[0] / DegreeToPulse, 7.2 * AxisStroke[0] / DegreeToPulse, 1.44 * AxisStroke[0] / DegreeToPulse, 0.072 * AxisStroke[0] / DegreeToPulse, 28.8 * AxisStroke[0] / DegreeToPulse, 57.6 * AxisStroke[0] / DegreeToPulse);
                    if (Convert.ToDouble(lblCommandPosAxis0.Text) < 0)
                        AjinLibrary.AxmHomeSetMethod(0, 1, 4, 0, motData.ClearTime[0], motData.HomeOffsetN[0] * AxisStroke[0] / DegreeToPulse);
                    else
                        AjinLibrary.AxmHomeSetMethod(0, 0, 4, 0, motData.ClearTime[0], motData.HomeOffsetP[0] * AxisStroke[0] / DegreeToPulse);
                    AjinLibrary.AxmSignalSetLimit(0, 0, 1, 1);
                    duRetCode = AjinLibrary.AxmHomeSetStart(0);
                    AjinLibrary.AxmHomeGetResult(0, ref duHomeInfo);

                    break;
                case "btnMoveHomeAxis1":
                    AjinLibrary.AxmHomeSetVel(1, 10 * AxisStroke[1] / DegreeToPulse, 7.2 * AxisStroke[1] / DegreeToPulse, 1.44 * AxisStroke[1] / DegreeToPulse, 0.072 * AxisStroke[1] / DegreeToPulse, 28.8 * AxisStroke[1] / DegreeToPulse, 57.6 * AxisStroke[1] / DegreeToPulse);
                    if (Convert.ToDouble(lblCommandPosAxis1.Text) < 0)
                        AjinLibrary.AxmHomeSetMethod(1, 1, 4, 0, motData.ClearTime[1], motData.HomeOffsetN[1] * AxisStroke[1] / DegreeToPulse);
                    else
                        AjinLibrary.AxmHomeSetMethod(1, 0, 4, 0, motData.ClearTime[1], motData.HomeOffsetP[1] * AxisStroke[1] / DegreeToPulse);
                    AjinLibrary.AxmSignalSetLimit(1, 0, 1, 1);
                    duRetCode = AjinLibrary.AxmHomeSetStart(1);
                    AjinLibrary.AxmHomeGetResult(1, ref duHomeInfo);
                    break;
                default:
                    break;
            }
            if (duRetCode == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                switch (btn.Name)
                {
                    case "btnMoveHomeAxis0":
                        isFirstHomeSearchingAxis0 = false;
                        break;
                    case "btnMoveHomeAxis1":
                        isFirstHomeSearchingAxis1 = false;
                        break;
                }

            }
        }


        private void btnMoveHomeAxis0_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (!motData.HWLimitUse) MoveHomeDefault(btn);
            else MoveHomeLimitSensor(btn);

        }
        private void btnAddInspPos_Click(object sender, EventArgs e)
        {
            AddPosition();
        }
        private void btnMinusInspPos_Click(object sender, EventArgs e)
        {
            RemovePosition();
        }
        private void btnPositionClearAxis0_Click(object sender, EventArgs e)
        {
            AjinLibrary.AxmStatusSetPosMatch(0, 0.0);
        }
        private void btnPositionClearAxis1_Click(object sender, EventArgs e)
        {
            AjinLibrary.AxmStatusSetPosMatch(1, 0.0);
        }
        DialogResult ShowMessageCon(string message)
        {
            DialogResult dr1 = DialogResult.None;

            mesCon = new F_MotionMsg();
            mesCon.SetMessageCon(message);

            dr1 = mesCon.ShowDialog();
            mesCon.TopMost = true;
            return dr1;
        }
        private void btnMoveSetPos_Click(object sender, EventArgs e)
        {
            if (EMGOn) return;
            if (lstPosition.SelectedIndex == -1) return;
            MoveSelectedPosition(lstPosition.SelectedIndex);

        }
        private void btnEmgStop_Click(object sender, EventArgs e)
        {
            EMGStop();
            AjinLibrary.AxmSignalSetLimit(0, 0, 2, 2);
            AjinLibrary.AxmSignalSetLimit(1, 0, 2, 2);

        }
        public void EMGStop()
        {
            try
            {
                uint duRetCode = 0;
                duRetCode = AjinLibrary.AxmMoveEStop(0);
                duRetCode = AjinLibrary.AxmMoveEStop(1);

            }
            catch
            { }
        }
        private void PosGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 1 && e.ColumnIndex == 4)
            {
                DataGridViewCell cell = PosGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (Convert.ToBoolean(cell.Value))
                {
                    PosGrid[1, e.RowIndex].Style.BackColor = Color.White;
                    PosGrid[2, e.RowIndex].Style.BackColor = Color.White;
                }
                else
                {
                    PosGrid[1, e.RowIndex].Style.BackColor = Color.OrangeRed;
                    PosGrid[2, e.RowIndex].Style.BackColor = Color.OrangeRed;
                }

                for (int i = 0; i < PosGrid.RowCount; i++)
                {
                    motData.MotionPosList[i].Deg[0] = Convert.ToDouble(PosGrid[1, i].Value);
                    motData.MotionPosList[i].Deg[1] = Convert.ToDouble(PosGrid[2, i].Value);
                    motData.MotionPosList[i].Enable = Convert.ToBoolean(PosGrid[4, i].Value);
                    motData.MotionPosList[i].PositionName = Convert.ToString(PosGrid[5, i].Value);

                }
                DataIO.SerializeToXMLFile(motData, STATIC.MotionDir);
            }
        }
        private void PosGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (PosGrid.IsCurrentCellDirty)
                PosGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            motData.HWLimitUse = checkBox1.Checked;
            DataIO.SerializeToXMLFile(motData, STATIC.MotionDir);
        }

       
    }
    public class MotionData
    {
        [Motion("NotUse", "PosList", "")] public List<DrvPosInfo> MotionPosList { get; set; } = new List<DrvPosInfo>();
        [Motion("NotUse", "", "")] public bool[] SWLimitUse { get; set; } = new bool[2] { false, false };
        [Motion("NotUse", "", "")] public bool HWLimitUse { get; set; } = false;
        [Motion("Common", "Velocity", "deg/s")] public double[] Velocity { get; set; } = new double[2] { 14.4, 14.4 };
        [Motion("Common", "Acceleration", "deg/s\xB2")] public double[] Acc { get; set; } = new double[2] { 28.8, 28.8 };
        [Motion("Common", "Deceleration", "deg/s\xB2")] public double[] Dec { get; set; } = new double[2] { 28.8, 28.8 };
        [Motion("Home Setting", "Set Offset P", "deg")] public double[] HomeOffsetP { get; set; } = new double[2] { 0.0, 0.0 };
        [Motion("Home Setting", "Set Offset N", "deg")] public double[] HomeOffsetN { get; set; } = new double[2] { 0.0, 0.0 };
        [Motion("Home Setting", "Clear Time", "msec")] public double[] ClearTime { get; set; } = new double[2] { 1000.0, 1000.0 };
        [Motion("Limit Setting", "SW Limit P", "deg")] public double[] SWLimitP { get; set; } = new double[2] { 0.0, 0.0 };
        [Motion("Limit Setting", "SW Limit N", "deg")] public double[] SWLimitN { get; set; } = new double[2] { 0.0, 0.0 };
        [Motion("Position Setting", "Test Position", "deg")] public double[] TestPosition { get; set; } = new double[2] { 0.0, 0.0 };
    }
    public class DrvPosInfo
    {
        public string PositionName { get; set; } = string.Empty;
        public double[] Deg { get; set; } = new double[2];
        public bool Enable { get; set; } = false;
    }
    public sealed class MotionAttribute : Attribute
    {
        public string Category { get; set; }
        public string DisplayName { get; set; }
        public string Unit { get; set; }
        public MotionAttribute(string des, string des2, string des3)
        {
            Category = des;
            DisplayName = des2;
            Unit = des3;
        }
    }

}
