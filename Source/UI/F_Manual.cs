using FZ4P.Commons;
using FZ4P.Commons.Helper;
using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using FZ4P.DriverIc.OISIC;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FZ4P.UI
{
    public partial class F_Manual : ModelChangedBase
    {
        private readonly IOISFunction _oISFunction = null;
        private readonly IFRAFunction _fraFunction = null;
        private readonly IAFunction _afFunction = null;
        private readonly I2CTOI3C_Function _i2CToI3C= null;
        private readonly Action<int, string> _actionLog;
        private Task t1;
        private CancellationTokenSource[] cts = new CancellationTokenSource[2];

        string[] index = { "OIS", "AF" };
        string[] indexCh = { "0", "1" };
        string[] indexAxis = { "OIS", "AF" };

        private string _readHall;
        public string ReadHall
        {
            get => _readHall;
            set
            {
                if (_readHall != value)
                {
                    _readHall = value;
                    OnPropertyChanged(nameof(ReadHall), value);
                }
            }
        }
        private string _readHall2;
        public string ReadHall2
        {
            get => _readHall2;
            set
            {
                if (_readHall2 != value)
                {
                    _readHall2 = value;
                    OnPropertyChanged(nameof(ReadHall2), value);
                }
            }
        }

        private string _readHall3;
        public string ReadHall3
        {
            get => _readHall3;
            set
            {
                if (_readHall3 != value)
                {
                    _readHall3 = value;
                    OnPropertyChanged(nameof(ReadHall3), value);
                }
            }
        }

        private string _peakCurrent;
        public string PeakCurrent
        {
            get => _peakCurrent;
            set
            {
                if (_peakCurrent != value)
                {
                    _peakCurrent = value;
                    OnPropertyChanged(nameof(PeakCurrent), value);
                }
            }
        }

        private string _current2;
        public string Current2
        {
            get => _current2;
            set
            {
                if (_current2 != value)
                {
                    _current2 = value;
                    OnPropertyChanged(nameof(Current2), value);
                }
            }
        }

        private string _checkBuffer_I3C_X;
        public string CheckBuffer_I3C_X
        {
            get => _checkBuffer_I3C_X;
            set
            {
                if (_checkBuffer_I3C_X != value)
                {
                    _checkBuffer_I3C_X = value;
                    OnPropertyChanged(nameof(CheckBuffer_I3C_X), value);
                }
            }
        }

        private string _checkBuffer_I3C_X_2;
        public string CheckBuffer_I3C_X_2
        {
            get => _checkBuffer_I3C_X_2;
            set
            {
                if (_checkBuffer_I3C_X_2 != value)
                {
                    _checkBuffer_I3C_X_2 = value;
                    OnPropertyChanged(nameof(CheckBuffer_I3C_X_2), value);
                }
            }
        }

        private string _checkBuffer_I3C_Y;
        public string CheckBuffer_I3C_Y
        {
            get => _checkBuffer_I3C_Y;
            set
            {
                if (_checkBuffer_I3C_Y != value)
                {
                    _checkBuffer_I3C_Y = value;
                    OnPropertyChanged(nameof(CheckBuffer_I3C_Y), value);
                }
            }
        }

        private string _checkBuffer_I3C_Y_2;
        public string CheckBuffer_I3C_Y_2
        {
            get => _checkBuffer_I3C_Y_2;
            set
            {
                if (_checkBuffer_I3C_Y_2 != value)
                {
                    _checkBuffer_I3C_Y_2 = value;
                    OnPropertyChanged(nameof(CheckBuffer_I3C_Y_2), value);
                }
            }
        }

        private string fw_Version;
        public string FW_Version
        {
            get => fw_Version;
            set
            {
                if (fw_Version != value)
                {
                    fw_Version = value;
                    OnPropertyChanged(nameof(FW_Version), value);
                }
            }
        }

        private ObservableCollection<SlaveId> _scanSlaveID = new ObservableCollection<SlaveId>();
        public ObservableCollection<SlaveId> ScanSlaveID { 
            get => _scanSlaveID;
            set
            {
                if (_scanSlaveID == value)
                    return;

                if (_scanSlaveID != null)
                    _scanSlaveID.CollectionChanged -= ScanSlaveID_CollectionChanged;

                _scanSlaveID = value;

                if (_scanSlaveID != null)
                    _scanSlaveID.CollectionChanged += ScanSlaveID_CollectionChanged;

                OnPropertyChanged(nameof(ScanSlaveID));
            }
        }

        public F_Manual(IOISFunction oISFunction, IAFunction afFunction,Action<int, string> actionLog, I2CTOI3C_Function i2cFunction = null)
        {
            InitializeComponent();
            
            _oISFunction = oISFunction;
            _i2CToI3C = i2cFunction;
            _afFunction = afFunction;
            _fraFunction = oISFunction as IFRAFunction;
            PropertyChanged += F_Manual_PropertyChanged;
            _actionLog = actionLog;
            //cbb_Acturator_Model.DataSource = Enum.GetValues(typeof(ActuatorType));
            cbb_ADC_Select.DataSource = index;
            cbb_Aixs.DataSource = indexAxis;
            cbb_Channel.DataSource = indexCh;

            _scanSlaveID.CollectionChanged += ScanSlaveID_CollectionChanged;

            var _ = this.Handle;
        }

        private void F_Manual_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReadHall))
            {
                this.InvokeOnUIThread(() => { 
                    lbl_ReadHall.Text = PropertiesHelper.GetValue<string>(e);
                });
            }
            else if (e.PropertyName == nameof(ReadHall2))
            {
                this.InvokeOnUIThread(() => {
                    lbl_ReadHall2.Text = PropertiesHelper.GetValue<string>(e);
                });
            }
            else if (e.PropertyName == nameof(ReadHall3))
            {
                this.InvokeOnUIThread(() => {
                    lbl_ReadHall3.Text = PropertiesHelper.GetValue<string>(e);
                });
            }
            else if (e.PropertyName == nameof(PeakCurrent))
            {
                this.InvokeOnUIThread(() => {
                    lbl_ADC.Text = PropertiesHelper.GetValue<string>(e);
                });
            }
            else if (e.PropertyName == nameof(Current2))
            {
                this.InvokeOnUIThread(() => {
                    lbl_ADC_2.Text = PropertiesHelper.GetValue<string>(e);
                });
            }
            else if (e.PropertyName == nameof(CheckBuffer_I3C_X))
            {
                this.InvokeOnUIThread(() => {
                    lbl_X_40.Text = PropertiesHelper.GetValue<string>(e);
                });
            }
            else if (e.PropertyName == nameof(CheckBuffer_I3C_X_2))
            {
                this.InvokeOnUIThread(() => {
                    lbl_X_42.Text = PropertiesHelper.GetValue<string>(e);
                });
            }
            else if (e.PropertyName == nameof(CheckBuffer_I3C_Y))
            {
                this.InvokeOnUIThread(() => {
                    lbl_Y_40.Text = PropertiesHelper.GetValue<string>(e);
                });
            }
            else if (e.PropertyName == nameof(CheckBuffer_I3C_Y_2))
            {
                this.InvokeOnUIThread(() => {
                    lbl_Y_42.Text = PropertiesHelper.GetValue<string>(e);
                });
            }
            else if (e.PropertyName == nameof(FW_Version))
            {
                this.InvokeOnUIThread(() => {
                    lbl_Version.Text = PropertiesHelper.GetValue<string>(e);
                });
            }
            else if (e.PropertyName == nameof(SlaveId))
            {
                this.InvokeOnUIThread(() => {
                    var rowid = PropertiesHelper.GetValue<SlaveId>(e);
                    var tab = tbcntl.TabPages.Cast<TabPage>().FirstOrDefault(x => (int)x.Tag == rowid.IndexKey);
                    var bindList = new ListBox() { Dock = DockStyle.Fill };
                    bindList.Items.AddRange(rowid.Values);
                    tab.Controls.Add(bindList);
                });
            }
        }

        private void ScanSlaveID_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (SlaveId item in e.NewItems)
                    {
                        var text = "DLN_Series_" + item.IndexKey;
                        tbcntl.TabPages.Add(new TabPage() 
                        {
                            Text = text,
                            Tag = item.IndexKey,
                        });
                        item.PropertyChanged += F_Manual_PropertyChanged;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (SlaveId item in e.OldItems)
                    {
                        item.PropertyChanged -= F_Manual_PropertyChanged;
                        var tabpage= tbcntl.TabPages.Cast<TabPage>().FirstOrDefault(x => (int)x.Tag == item.IndexKey);
                        if(tabpage != null)
                            tbcntl.TabPages.Remove(tabpage);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void F_Manual_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            bool State= ((CheckBox)sender).Checked;

            if (State)
            {
                STATIC.Dln.PowerOnOff(0, true);
                STATIC.Dln.PowerOnOff(1, true);
            }

            else
            {
                STATIC.Dln.PowerOnOff(0, false);
                STATIC.Dln.PowerOnOff(1, false);
            }
                
        }

        private void btn_PositionMove_Click(object sender, EventArgs e)
        {
            try
            {
                var positionX = Convert.ToInt32(txt_PositionCode_AxisX.Text);
                var positionY = Convert.ToInt32(txt_PositionCode_AxisY.Text);
                var positionZ = Convert.ToInt32(txt_PositionCode_AxisZ.Text);

                _afFunction.AFMove(0, positionZ);
                _oISFunction.OISMove(0, positionX, positionY);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void EditCondition_CheckStateChanged(object sender, EventArgs e)
        {
            bool State = ((CheckBox)sender).Checked;
            int ch = Convert.ToInt32(cbb_Channel.Text);
            if (State)
            {
                cts[0] = new CancellationTokenSource();
                Task.Run(() => ReadHold(cts[0].Token, ch));
            }
            else
                cts[0]?.Cancel();
        }

        private void ReadHold(CancellationToken token,int iCh) 
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    //ReadHall = _oISFunction.GetI3CData(AxisTypeDW.AxisX).ToString();
                    ReadHall = _oISFunction.ReadOISHall(0, 0, 0).ToString();
                    Thread.Sleep(5);
                    ReadHall2 = _oISFunction.ReadOISHall(0, 1, 0).ToString();
                    Thread.Sleep(5);
                    ReadHall3 = _afFunction.ReadAFHall(iCh).ToString();
                    Thread.Sleep(5);

                    PeakCurrent = _oISFunction.GetCurrent((int)AxisTypeDW.AxisX).ToString("00.00");
                    Thread.Sleep(5);
                    Current2 = _oISFunction.GetCurrent((int)AxisTypeDW.AxisY).ToString("00.00");
                    Thread.Sleep(5);
                }
                catch(Exception ex)
                {
                    _actionLog(iCh, ex.Message);
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int ch = Convert.ToInt32(cbb_Channel.Text);
            int iAixs = cbb_Aixs.SelectedIndex;

            //STATIC.DrvIC.CurrentSetRegister(ch, iAixs);
        }

        private void rb_DetectPin_Click(object sender, EventArgs e)
        {
            var stateIndex = Convert.ToInt32(((RadioButton)sender).Tag);
            int adcNumber = cbb_ADC_Select.SelectedIndex;
            switch (stateIndex)
            {
                case 0:
                    //STATIC.Dln.PeakDetector(adcNumber, PeakDetectState.Hold);
                    break;
                case 1:
                    //STATIC.Dln.PeakDetector(adcNumber, PeakDetectState.Detect);
                    break;
                case 2:
                    //STATIC.Dln.PeakDetector(adcNumber, PeakDetectState.Reset);
                    break;
            }
            
        }

        private void EditCondition_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn_ServoOn_Click(object sender, EventArgs e)
        {
            _oISFunction.OISOnOff(0, true);
            _afFunction.AFOnOff(0, true);
        }

        private void btn_Move_Min_Click(object sender, EventArgs e)
        {
            var afMinCode = _afFunction.AF_MIN_CODE;
            var oisMinCode = _oISFunction.OIS_MIN_CODE;
            _afFunction.AFMove(0, afMinCode);
            _oISFunction.OISMove(0, oisMinCode, oisMinCode);
        }

        private void btn_Move_Mid_Click(object sender, EventArgs e)
        {
            var afMidCode = _afFunction.AF_MID_CODE;
            var oisMidCode = _oISFunction.OIS_MID_CODE;
            _afFunction.AFMove(0, afMidCode);
            _oISFunction.OISMove(0, oisMidCode, oisMidCode);
        }

        private void btn_Move_Max_Click(object sender, EventArgs e)
        {
            var afMidCode = _afFunction.AF_MAX_CODE-1;
            var oisMidCode = _oISFunction.OIS_MAX_CODE-1;
            _afFunction.AFMove(0, afMidCode);
            _oISFunction.OISMove(0, oisMidCode, oisMidCode);
        }

        private void checkBox3_CheckStateChanged(object sender, EventArgs e)
        {
            bool State = ((CheckBox)sender).Checked;
            int ch = Convert.ToInt32(cbb_Channel.Text);
            _oISFunction.OISReset(0, 0, State);
            _oISFunction.OISReset(0, 1, State);
            _afFunction.AF_ICReset(0);
        }

        private void checkBox4_CheckStateChanged(object sender, EventArgs e)
        {
            bool State = ((CheckBox)sender).Checked;
            //int ch = Convert.ToInt32(cbb_Channel.Text);
            if (State)
                _fraFunction.Echo_Board_Select_Ch(1);
            else if (!State)
                _fraFunction.Echo_Board_Select_Ch(2);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckStateChanged(object sender, EventArgs e)
        {
            bool State = ((CheckBox)sender).Checked;
            _i2CToI3C.SetI3CByPaaMode(State);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _i2CToI3C.SetH503WakeUp();
        }

        private void ReadI2CBuffer(int iCh)
        {
            try
            {
                CheckBuffer_I3C_X = ((ushort)_i2CToI3C.GetI3CCheckBuffer(AxisTypeDW.AxisX,0)).ToString();
                Thread.Sleep(5);
                CheckBuffer_I3C_X_2 = ((ushort)_i2CToI3C.GetI3CCheckBuffer(AxisTypeDW.AxisX, 1)).ToString();
                Thread.Sleep(5);
                CheckBuffer_I3C_Y = ((ushort)_i2CToI3C.GetI3CCheckBuffer(AxisTypeDW.AxisY, 0)).ToString();
                Thread.Sleep(5);
                CheckBuffer_I3C_Y_2 = ((ushort)_i2CToI3C.GetI3CCheckBuffer(AxisTypeDW.AxisY, 1)).ToString();
                Thread.Sleep(5);
            }
            catch (Exception ex)
            {
                _actionLog(iCh, ex.Message);
            }
        }

        private void btn_VerserChecked_Click(object sender, EventArgs e)
        {
            FW_Version = "0x" + _i2CToI3C.GetVersionChecked(AxisTypeDW.AxisX).ToString("X4");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int[] slaveIDCollection = null;
            removeClear();

            for (int i = 0; i < STATIC.Dln.DLNi2c.Length; i++)
            {
                if (STATIC.Dln.DLNi2c[i] == null) continue;

                ScanSlaveID.Add(new SlaveId() { IndexKey = i });
                var lastElement= ScanSlaveID.Last();
                try
                {
                    slaveIDCollection = STATIC.Dln.DLNi2c[i].ScanDevices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"error{ex.Message}");
                    slaveIDCollection = new int[1] { -1, };
                }
                var v = slaveIDCollection.Select(x=>"0x" + x.ToString("X2")).ToArray();
                lastElement.Values = v;
            }
        }

        private void removeClear()
        {
            if (ScanSlaveID.Count == 0) return;

            var count = ScanSlaveID.Count;
            for (int i = 0; i < count; i++)
                ScanSlaveID.RemoveAt(0);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            ReadI2CBuffer(0);
        }
    }

    public class SlaveId : ModelChangedBase
    {
        private int _IndexKey = 0;
        private string[] _values;

        public int IndexKey { get => _IndexKey; set => _IndexKey = value; }
        public string[] Values
        {
            get => _values;
            set
            {
                if (_values == value)
                    return;

                _values = value;
                OnPropertyChanged(nameof(SlaveId),this);
            }
        }
    }
}
