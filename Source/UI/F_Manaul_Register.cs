using FZ4P.Commons.Helper;
using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FZ4P.UI
{
    public partial class F_Manaul_Register : Form
    {
        private readonly IOISFunction _oISFunction = null;
        private readonly IFRAFunction _fraFunction = null;
        private readonly IAFunction _afFunction = null;
        private readonly I2CTOI3C_Function _i2CToI3C = null;
        private readonly IOneTwoBytesDrivingIC _i2cMasterControl = null; 
        private readonly Action<int, string> _actionLog;
        private readonly Action<int, bool> _powerOnOff;

        public F_Manaul_Register(   IOISFunction oISFunction, 
                                    IAFunction afFunction,
                                    IOneTwoBytesDrivingIC i2cMasterControl,
                                    Action<int, string> actionLog, 
                                    Action<int, bool> PowerOnOff, 
                                    I2CTOI3C_Function i2cFunction = null)
        {
            InitializeComponent();
            _oISFunction = oISFunction;
            _afFunction = afFunction;
            _actionLog = actionLog;
            _i2CToI3C = i2cFunction;
            _powerOnOff = PowerOnOff;
            _i2cMasterControl = i2cMasterControl;

            cbb_ReadWriteState.SelectedIndex = 0;
            cbb_SlaveIDState.SelectedIndex = 0;
            toolStripButton4.Image = imageList1.Images["Power"];
            toolStripButton5.Image = imageList1.Images["I2CWrite"];
        }

        private void btn_WindowState_Max_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void btn_WindowState_Min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btn_WindowState_Close_Click(object sender, EventArgs e)
        {
        }

        private void toolStripButton4_CheckStateChanged(object sender, EventArgs e)
        {
            var onoff = ((ToolStripButton)sender).Checked;
            _powerOnOff(0, onoff);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            string CRLF = "\r\n";
            var selectedIndex = cbb_SlaveIDState.SelectedIndex;
            var SlaveID = GetSlaveID(selectedIndex);
            var byteConvertFlg = byte.TryParse(tlst_Register_Value.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte byteData);
            var byteConvertFlg1 = byte.TryParse(tlst_Register.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte byteRegister);

            if (cbb_ReadWriteState.SelectedIndex == 1)
            {
                _i2cMasterControl.WriteByte(SlaveID, byteRegister, 1, byteData);
                string tmpFormat = string.Format($"SlaveID : 0x{SlaveID.ToString("X2")}, Register : 0x{byteRegister.ToString("X2")} , Value : 0x{byteData.ToString("X2")}") + CRLF;
                rchtxtbx_WriteLog.AppendText(tmpFormat);
            }
            else
            {
                string tmpFormat = string.Format($"SlaveID : 0x{SlaveID.ToString("X2")}, Register : 0x{byteRegister.ToString("X2")} , Value : 0x{_i2cMasterControl.ReadByte(SlaveID, byteRegister, 1).ToString("X2")}") + CRLF;
                rchtxtbx_ReadLog.AppendText(tmpFormat);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            rchtxtbx_WriteLog.Clear();
        }

        private void cbb_SlaveIDState_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            rchtxtbx_ReadLog.Clear();
        }

        private int GetSlaveID(int AxisType)
        {
            if (AxisType == 0)
                return _oISFunction.OISX_Addr;
            else
                return _oISFunction.OISY_Addr;
        }

        private void F_Manaul_Register_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            var result = MessageBox.Show("화면을 종료하시겠습니까?", "종료 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                this.Hide();
        }
    }
}
