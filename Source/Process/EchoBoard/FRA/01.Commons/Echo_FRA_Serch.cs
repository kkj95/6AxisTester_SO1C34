using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{
    public class Echo_FRA_Serch
    {
        private Action<int, string> LogAction;
        public Echo_FRA_Serch(Action<int, string> collback)
        {
            LogAction = collback;
        }

        public int Search_PM(int ch, ref sFRA_Margin fra_result, sFRA_TestSetting fra_setting, double[] freq_buf, double[] gain_buf, double[] phase_buf, int Searchcnt)
        {
            int i = 0;
            int msg = 0;
            int PM_flag = 0;
            double phase_margin = 0, gain = 0, freq_PM = 0, before_Freq = 0, before_gain = 0, before_pm = 0;
            double final_freq = 0, final_pm = 0;

            LogAction(ch, string.Format("=================================================="));

            for (i = 0; i < Searchcnt; i++)
            {
                //Search Phase Margin
                if (gain_buf[i] * gain_buf[i + 1] <= 0)
                {
                    if (PM_flag == 0)
                    {
                        phase_margin = phase_buf[i + 1] + 180;
                        freq_PM = freq_buf[i + 1];
                        gain = gain_buf[i + 1];
                        PM_flag = 1;
                    }
                    else if (phase_buf[i + 1] + 180 < phase_margin)
                    {
                        phase_margin = phase_buf[i + 1] + 180;
                        freq_PM = freq_buf[i + 1];
                        gain = gain_buf[i + 1];
                    }
                }
            }

            final_freq = freq_PM;
            final_pm = phase_margin;

            fra_result.phase_margin_flag = (byte)PM_flag;
            fra_result.phase_margin = final_pm;
            fra_result.phase_margin_freq = final_freq;

            LogAction(ch, string.Format("[Search_PM] PM flag = {0}, PM[deg] = {1:0.0}, PM Freq[Hz] = {2:0.0}", PM_flag, final_pm, final_freq));
            LogAction(ch, string.Format("=================================================="));

            return msg;
        }
        public int Search_GM(int ch, ref sFRA_Margin fra_result, sFRA_TestSetting fra_setting, double[] freq_buf, double[] gain_buf, double[] phase_buf, int SearchCnt)
        {
            int msg = 0;
            int i = 0;
            int GM_flag = 0;
            double gain_margin = 0, freq_GM = 0, before_Freq = 0, phase = 0, before_phase = 0, before_gm = 0;
            double final_freq = 0, final_gm = 0;

            LogAction(ch, string.Format("=================================================="));

            for (i = 0; i < SearchCnt; i++)
            {
                //Search Gain Margin
                if (phase_buf[i] > 165 && phase_buf[i + 1] < -165)
                {
                    if (GM_flag == 0)
                    {
                        if (Math.Abs(phase_buf[i]) > Math.Abs(phase_buf[i + 1]))
                        {
                            gain_margin = gain_buf[i];
                            freq_GM = freq_buf[i];
                        }
                        else
                        {
                            gain_margin = gain_buf[i + 1];
                            freq_GM = freq_buf[i + 1];
                        }
                        GM_flag = 1;
                        LogAction(ch, string.Format($"gain = %6.2f GM Freq[Hz] = %6.1f", gain_margin, freq_GM));
                    }
                    else
                    {
                        if (Math.Abs(phase_buf[i]) > Math.Abs(phase_buf[i + 1]))
                        {
                            if (gain_buf[i] > gain_margin)
                            {
                                gain_margin = gain_buf[i];
                                freq_GM = freq_buf[i];
                            }
                        }
                        else
                        {
                            if (gain_buf[i + 1] > gain_margin)
                            {
                                gain_margin = gain_buf[i + 1];
                                freq_GM = freq_buf[i + 1];
                            }
                        }
                        LogAction(ch, string.Format("gain = {0:0.00}GM Freq[Hz] = {1:0.00}", gain_margin, freq_GM));
                    }
                }
            }

            final_freq = freq_GM;
            final_gm = gain_margin;

            fra_result.gain_margin_flag = (byte)GM_flag;
            fra_result.gain_margin = final_gm;
            fra_result.gain_margin_freq = final_freq;
            LogAction(ch, string.Format("[Search_GM] GM flag = {0}, GM[dB] = {1:0.0}, GM Freq[Hz] = {2:0.0}", GM_flag, gain_margin * (-1), freq_GM));
            LogAction(ch, string.Format("=================================================="));

            return msg;
        }
    }
}
