using FZ4P.Commons;
using FZ4P.Commons.Helper;
using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FZ4P.UI
{
    public partial class F_Manual : ModelChangedBase
    {
        private readonly IOISFunction _oISFunction = null;
        private readonly IAFunction _afFunction = null;
        private Task t1;
        private CancellationTokenSource cts;

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

        public F_Manual(IOISFunction oISFunction, IAFunction afFunction)
        {
            InitializeComponent();
            
            _oISFunction = oISFunction;
            _afFunction = afFunction;

            PropertyChanged += F_Manual_PropertyChanged;
            //cbb_Acturator_Model.DataSource = Enum.GetValues(typeof(ActuatorType));
            cbb_ADC_Select.DataSource = index;
            cbb_Aixs.DataSource = indexAxis;
            cbb_Channel.DataSource = indexCh;
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
                STATIC.Dln.PowerOnOff(0, true);
            else
                STATIC.Dln.PowerOnOff(0, false);
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
                cts = new CancellationTokenSource();
                Task.Run(() => ReadHold(cts.Token, ch));
            }
            else
                cts?.Cancel();
        }

        private void ReadHold(CancellationToken token,int iCh)
        {
            while (!token.IsCancellationRequested)
            {
                ReadHall = _oISFunction.ReadOISHall(0, 0, 0).ToString();
                ReadHall2 = _oISFunction.ReadOISHall(0, 1, 0).ToString();
                ReadHall3 = _afFunction.ReadAFHall(iCh).ToString();

                Thread.Sleep(5);
                //PeakCurrent = STATIC.DrvIC.GetPeakCurrent(iCh, iAixs).ToString();
                Thread.Sleep(5);
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
            var afMidCode = _afFunction.AF_MAX_CODE;
            var oisMidCode = _oISFunction.OIS_MAX_CODE;
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
    }
}
