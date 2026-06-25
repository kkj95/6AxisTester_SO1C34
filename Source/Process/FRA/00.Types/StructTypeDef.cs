using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{
    public struct sFRA_TestSetting
    {
        public byte ois_slave_id;
        public byte ois_mode;
        public byte ois_control_freq;
        public int amplitude;
        public int dc_bias_ofst;
        public int test_point;
        public int start_freq;
        public int end_freq;

        // 241120 추가됨. API v6.0.1.5
        public short x_target;
        public short y_target;
        public short af_target;
    }

    public struct sFRA_Margin
    {
        public byte gain_margin_flag;
        public double gain_margin;
        public double gain_margin_freq;
        public byte phase_margin_flag;
        public double phase_margin;
        public double phase_margin_freq;
        public double freq10HzGain;
    }
}
