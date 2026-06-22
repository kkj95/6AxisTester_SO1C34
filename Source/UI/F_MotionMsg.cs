using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FZ4P
{
    public partial class F_MotionMsg : Form
    {
        Timer tmr = new Timer();
        Color before;
        public F_MotionMsg()
        {
            InitializeComponent();
            tmr.Tick += Tmr_Tick;
            tmr.Interval = 200;
        }

        private void Tmr_Tick(object sender, EventArgs e)
        {
            if (label1.ForeColor != Color.Red)
            {
                before = label1.ForeColor;
                label1.ForeColor = Color.Red;
            }
            else
                label1.ForeColor = before;
        }
        public void SetMessageCon(string message)
        {
            label1.Text = message;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            tmr.Enabled = false;
            this.DialogResult = DialogResult.Yes;
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            tmr.Enabled = false;
            this.DialogResult = DialogResult.No;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
