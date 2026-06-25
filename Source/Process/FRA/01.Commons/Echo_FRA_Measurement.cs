using FZ4P.Process.FRA._00.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FZ4P.Process.FRA._01.Commons
{
    public class Echo_FRA_Measurement
    {
        public Echo_FRA_Measurement()
        {
                
        }

        public int Echo_FRA_Single_Measurement(int ch, ref sFRA_Margin fra_result, ref sFRA_TestSetting fra_setting, ref double[] freq_buf, ref double[] gain_buf, ref double[] phase_buf, ref int SearchCnt, bool fullScanUse = true, bool gmtest = false)
        {
            byte[] u08_dat1 = new byte[1] { 0x00 };
            byte[] u08_dat2 = new byte[1] { 0x00 };
            byte[] board_info = new byte[1] { 0x00 };
            int cnt = 0;
            byte[] sts_check = new byte[1] { 0x00 };
            byte[] err_list = new byte[1] { 0x00 };
            //int freq;
            double gain, phase, freq = 0, gain10Hz = 0;
            double echo_resolution = 0.0;

            double[] echo_temp = new double[fra_setting.test_point];
            double prephase = 0;

            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x00, 1, board_info);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_Measurement] FRA Board Info = 0x{0:X2}", board_info[0]));

            if (board_info[0] != m__G.fGraph.DriverIC.FRA_BOARD_INFO)
            {
                m__G.fManage.AddOperatorLog(ch, "Echo_FRA_Measurement] FRA Board Info Error!!!");
                return (int)m__G.fGraph.DriverIC.FRA_BOARD_INFO_NG;
            }

            /* --------------------
            * Board version / power info
            * -------------------- */
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0xF2, 1, u08_dat1);
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0xF3, 1, u08_dat2);

            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_Measurement] FRA Board Version(REG 0xF2): 0x{0:X2} 0x{1:X2}", u08_dat1[0], u08_dat2[0]));

            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x32, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[echo_fra_single_measurement] VDD OUT(REG 0x32) = {0}", u08_dat1[0]));
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x33, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[echo_fra_single_measurement] CH1 AVDD(REG 0x33) = {0}", u08_dat1[0]));
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x34, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[echo_fra_single_measurement] CH1 IOVDD(REG 0x34) = {0}", u08_dat1[0]));
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x35, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[echo_fra_single_measurement] CH2 AVDD(REG 0x35) = {0}", u08_dat1[0]));
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x36, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[echo_fra_single_measurement] CH2 IOVDD(REG 0x36) = {0}", u08_dat1[0]));
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0xE1, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[echo_fra_single_measurement] AMP MODE(REG 0x1E) = {0}", u08_dat1[0]));

            /* ==================================
             * Calculate Frequency step (+ dedup)
             * - Protect test_point == 1
             * ================================== */
            if (fra_setting.test_point == 1)
            {
                freq_buf[0] = (double)fra_setting.start_freq;
            }
            else
            {
                for (int i = 0; i < fra_setting.test_point; i++)
                {
                    freq_buf[i] = Math.Floor(fra_setting.start_freq *
                                        Math.Exp(Math.Log(fra_setting.end_freq / (double)fra_setting.start_freq) /
                                            (double)(fra_setting.test_point - 1) * i));

                    echo_resolution = (freq_buf[i] < 1000.0) ? (6000.0 / 4096.0) : (12000.0 / 4096.0);

                    while (true)
                    {
                        /* Snap to echo resolution grid */
                        echo_temp[i] = Math.Floor(((freq_buf[i] / echo_resolution) + 0.5)) * echo_resolution;

                        if (i > 0)
                        {
                            /* Deduplication based on echo_buf[] */
                            if (SearchKeyInArr(echo_temp, i, echo_temp[i]) == 1)
                            {
                                freq_buf[i] = freq_buf[i] + 1.0;
                                continue;
                            }
                        }
                        break;
                    }
                }
            }

            //if (fra_setting.start_freq > fra_setting.end_freq)
            //    sort(freq_buf, fra_setting.test_point);

            /* --------------------
             * Board config
             * -------------------- */
            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x03, 1, new byte[1] { 0x01 });  // 0x01: Single 0x08: FullScan
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x31, 1, new byte[1] { 0x35 });  //0x31: DW9781C  0x35 DW9785
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x30, 1, new byte[1] { (byte)(fra_setting.ois_slave_id) });        //OIS Slave address
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x6E, 1, new byte[1] { fra_setting.ois_mode });             //0x00: plant X, 0x01: Open X, 0x10: plant Y, 0x11: Open Y
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x10, 1, new byte[1] { 0x00 });   //target position
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x11, 1, new byte[1] { 0x00 });
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x2F, 1, new byte[1] { fra_setting.ois_control_freq });    //0: 5KHz, 1: 10KHz
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x04, 1, new byte[1] { (byte)(fra_setting.amplitude >> 8) });     // Amplitude[mV] MSB 
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x05, 1, new byte[1] { (byte)(fra_setting.amplitude & 0xFF) });  // Amplitude[mV] LSB
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x04, 1, u08_dat1);
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x05, 1, u08_dat2);
            int ret = BitConverter.ToInt16(new byte[2] { u08_dat2[0], u08_dat1[0] }, 0);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_Measurement] FRA Amplitude(REG 0x04): {0}", ret));

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x06, 1, new byte[1] { (byte)(fra_setting.dc_bias_ofst >> 8) });     // DC Bias Offset[mV] MSB 
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x07, 1, new byte[1] { (byte)(fra_setting.dc_bias_ofst & 0xFF) });    // DC Bias Offset[mV] LSB
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x06, 1, u08_dat1);
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x07, 1, u08_dat2);
            ret = BitConverter.ToInt16(new byte[2] { u08_dat2[0], u08_dat1[0] }, 0);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_Measurement] FRA Offset(REG 0x06): {0}", ret));

            //m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x02, 1, sts_check);

            /* --------------------
             * FRA Mode ON + status check
             * -------------------- */

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x01, 1, new byte[1] { 0x01 });    /* FRA Mode ON 0x01?*/

            Thread.Sleep(200);
            //m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x02, 1, sts_check);

            for (cnt = 0; cnt <= m__G.fGraph.DriverIC.TEST_LIMIT_CNT; cnt++)
            {
                //Thread.Sleep(20);

                m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x02, 1, sts_check);

                Thread.Sleep(20);
                if (sts_check[0] == 0x01)
                {
                    m__G.fManage.AddOperatorLog(ch, string.Format($"[echo_fra_single_measurement] Mode ON OK (cnt={cnt})"));
                    break;
                }

                if (cnt == (m__G.fGraph.DriverIC.TEST_LIMIT_CNT))
                {
                    m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x09, 1, err_list);
                    m__G.fManage.AddOperatorLog(ch, string.Format("[echo_fra_single_measurement] Mode ON timeout (Status={0}, Err={1})",
                        sts_check[0], err_list[0]));
                    m__G.fGraph.DriverIC.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                    Thread.Sleep(100);
                    return 1;
                }
            }

            Array.Reverse(freq_buf);

            /* --------------------
            * FRA sweep (High -> Low)
            * -------------------- */

            bool isStart = false;

            m__G.fManage.AddOperatorLog(ch, string.Format("Freq\tGain\tPhase"));
            for (int i = 0; i < fra_setting.test_point; i++)
            {
                //  int idx = fra_setting.test_point - 1 - i;
                uint ifreq32;
                ushort ifreq;

                freq = freq_buf[i];

                /* round + clamp to 16-bit */
                ifreq32 = (uint)(freq + 0.5);
                if (ifreq32 > 0xFFFF) ifreq32 = 0xFFFF;
                ifreq = (ushort)ifreq32;

                m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x0C, 1, new byte[] { (byte)((ifreq >> 8) & 0xFF) });
                m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x0D, 1, new byte[] { (byte)((ifreq >> 0) & 0xFF) });


                m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x0C, 1, u08_dat1);
                m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x0D, 1, u08_dat2);

                Thread.Sleep(100);

                m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x01, 1, new byte[] { 0x03 });
                Thread.Sleep(20);

                for (cnt = 0; cnt < m__G.fGraph.DriverIC.TEST_LIMIT_CNT; cnt++)
                {
                    m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x02, 1, sts_check);

                    if (sts_check[0] == m__G.fGraph.DriverIC.FRA_TEST_COMPLETE)
                    {
                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x20, 1, u08_dat1);
                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x21, 1, u08_dat2);

                        freq = (ushort)((u08_dat1[0] << 8) | u08_dat2[0]) / 10.0;

                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x22, 1, u08_dat1);
                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x23, 1, u08_dat2);
                        gain = (short)((u08_dat1[0] << 8) | u08_dat2[0]) / 10.0;

                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x24, 1, u08_dat1);
                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x25, 1, u08_dat2);

                        phase = (short)((u08_dat1[0] << 8) | u08_dat2[0]) / 10.0;

                        freq_buf[i] = freq;
                        gain_buf[i] = gain;
                        phase_buf[i] = phase;
                        break;
                    }

                    Thread.Sleep(20);

                    if (cnt == (m__G.fGraph.DriverIC.TEST_LIMIT_CNT - 1))
                    {
                        m__G.fGraph.DriverIC.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                        Thread.Sleep(100);
                        return (int)m__G.fGraph.DriverIC.TEST_STS_CHK_ERR;
                    }

                }
                //TODO : KKJ 26.01.26 csv로 뽑기위해 임시 포멧 변경 테스트후 삭제 요청
                m__G.fManage.AddOperatorLog(ch, string.Format("{0},{1},{2}", freq_buf[i], gain_buf[i], phase_buf[i]));
                if (!fullScanUse)
                {
                    if (!gmtest)
                    {
                        if (gain_buf[i] < 0 && !isStart)
                        {
                            isStart = true;
                        }
                        if (isStart)
                        {
                            if (gain_buf[i] >= 0)
                            {
                                SearchCnt = i;
                                break;
                            }
                        }

                    }
                    else
                    {
                        if (i >= 1)
                        {
                            if (phase_buf[i - 1] > 165 && phase_buf[i] < -165)
                            {
                                SearchCnt = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    SearchCnt = fra_setting.test_point - 1;
                }
            }
            if (!gmtest)
            {
                //10Hz Gain
                m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x0C, 1, new byte[1] { (byte)((int)10 >> 8) });      // Test Frequency[Hz] MSB 
                Thread.Sleep(10);

                m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x0D, 1, new byte[1] { (byte)((int)10 & 0xFF) });         // Test Frequency[Hz] LSB
                Thread.Sleep(10);

                m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x0C, 1, u08_dat1);
                m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x0D, 1, u08_dat2);

                ret = BitConverter.ToInt16(new byte[2] { u08_dat2[0], u08_dat1[0] }, 0);
                m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_10Hz Gain Measurement] Test frequency(REG 0x0C): {0}", ret));

                m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x01, 1, new byte[1] { 0x03 });    // FRA Test Start
                Thread.Sleep(20);

                cnt = 0;
                sts_check[0] = 0x00;
                while (cnt < m__G.fGraph.DriverIC.TEST_LIMIT_CNT)
                {
                    m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x02, 1, sts_check);

                    if (sts_check[0] == m__G.fGraph.DriverIC.FRA_TEST_COMPLETE) // Test complete
                    {
                        m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_Measurement] cnt = {0} -- status check : {1:X2}", fra_setting.test_point, sts_check[0]));

                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x20, 1, u08_dat1);
                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x21, 1, u08_dat2);
                        freq = (ushort)(u08_dat1[0] * 256 + u08_dat2[0]) / (double)10;

                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x22, 1, u08_dat1);
                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x23, 1, u08_dat2);
                        gain10Hz = (short)(u08_dat1[0] * 256 + u08_dat2[0]) / (double)10;

                        m__G.fManage.AddOperatorLog(ch, string.Format("Freq = {0}, 10Hz Gain = {1}", freq, gain10Hz));

                        fra_result.freq10HzGain = gain10Hz;
                        break;
                    }
                    else
                    {
                        Thread.Sleep((int)m__G.fGraph.DriverIC.TEST_DELAY_TIME);
                        cnt++;
                    }
                }

            }

            m__G.fGraph.DriverIC.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
            Thread.Sleep(100);
            m__G.fManage.AddOperatorLog(ch, string.Format("=================================================="));

            return 0;
        }
        public int Echo_FRA_Fullscan_Measurement(int ch, ref sFRA_Margin fra_result, sFRA_TestSetting fra_setting, ref double[] freq_buf, ref double[] gain_buf, ref double[] phase_buf, ref int SearchCnt, bool gmtest = false)
        {
            byte[] u08_dat1 = new byte[1] { 0x00 };
            byte[] u08_dat2 = new byte[1] { 0x00 };
            byte[] board_info = new byte[1] { 0x00 };
            int cnt = 0;
            byte[] sts_check = new byte[1] { 0x00 };
            byte[] err_list = new byte[1] { 0x00 };
            //int freq;
            double gain, phase, freq = 0, gain10Hz = 0;

            double[] gain_temp = new double[fra_setting.test_point];
            double[] phase_temp = new double[fra_setting.test_point];

            double echo_resolution = 0.0;
            double[] echo_temp = new double[fra_setting.test_point];

            double prephase = 0;
            int lastcount = 0;

            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x00, 1, board_info);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] FRA Board Info = 0x{0:X2}", board_info[0]));

            if (board_info[0] != m__G.fGraph.DriverIC.FRA_BOARD_INFO)
            {
                m__G.fManage.AddOperatorLog(ch, "[Echo_FRA_FullScanMode_Measurement] FRA Board Info Error!!!");
                return (int)m__G.fGraph.DriverIC.FRA_BOARD_INFO_NG;
            }

            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0xF2, 1, u08_dat1);
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0xF3, 1, u08_dat2);

            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_Measurement] FRA Board Version(REG 0xF2): 0x{0:X2} 0x{1:X2}", u08_dat1[0], u08_dat2[0]));

            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x32, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] VDD OUT(REG 0x32) = {0}", u08_dat1[0]));
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x33, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] CH1 AVDD(REG 0x33) = {0}", u08_dat1[0]));
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x34, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] CH1 IOVDD(REG 0x34) = {0}", u08_dat1[0]));
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x35, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] CH2 AVDD(REG 0x35) = {0}", u08_dat1[0]));
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x36, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] CH2 IOVDD(REG 0x36) = {0}", u08_dat1[0]));
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0xE1, 1, u08_dat1);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] AMP MODE(REG 0x1E) = {0}", u08_dat1[0]));

            //==================================
            //		Calculate Frequency step
            //==================================
            for (int i = 0; i < fra_setting.test_point; i++)
            {
                if (gmtest)
                    freq_buf[i] = fra_setting.start_freq - (i * m__G.sRecipe.iFRAstep_GM);
                else
                    freq_buf[i] = fra_setting.start_freq - (i * m__G.sRecipe.iFRAstep);
            }


            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x03, 1, new byte[1] { 0x08 });
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x31, 1, new byte[1] { 0x35 }); //0x31: DW9781C  0x35 DW9785
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x30, 1, new byte[1] { (byte)(fra_setting.ois_slave_id) /* 0x78 */});        //OIS Slave address
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x6E, 1, new byte[1] { fra_setting.ois_mode /* 0x11 */});             //0x00: plant X, 0x11: Open X, 0x10: plant Y, 0x21: Open Y
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x10, 1, new byte[1] { 0x00 });   //target position
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x11, 1, new byte[1] { 0x00 });
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x2F, 1, new byte[1] { fra_setting.ois_control_freq /*0x01*/ });    //0: 5KHz, 1: 10KHz
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x04, 1, new byte[1] { (byte)(fra_setting.amplitude >> 8) /* 0x00 */ });     // Amplitude[mV] MSB 
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x05, 1, new byte[1] { (byte)(fra_setting.amplitude & 0xFF) /* 0x8D */ });  // Amplitude[mV] LSB
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x04, 1, u08_dat1);
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x05, 1, u08_dat2);
            int ret = BitConverter.ToInt16(new byte[2] { u08_dat2[0], u08_dat1[0] }, 0);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] FRA Amplitude(REG 0x04): {0}", ret));

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x06, 1, new byte[1] { (byte)(fra_setting.dc_bias_ofst >> 8) /* 0x00 */ });     // DC Bias Offset[mV] MSB 
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x07, 1, new byte[1] { (byte)(fra_setting.dc_bias_ofst & 0xFF) /* 0x00 */ });    // DC Bias Offset[mV] LSB
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x06, 1, u08_dat1);
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x07, 1, u08_dat2);
            ret = BitConverter.ToInt16(new byte[2] { u08_dat2[0], u08_dat1[0] }, 0);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] FRA Offset(REG 0x06): {0}", ret));

            //추가
            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x0A, 1, new byte[1] { (byte)(fra_setting.test_point >> 8) /* 0x00 */ });     /* FRA Points MSB */
            Thread.Sleep(10);
            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x0B, 1, new byte[1] { (byte)(fra_setting.test_point & 0xFF) /* 0x64 */ });   /* FRA Points LSB */
            Thread.Sleep(10);
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x0A, 1, u08_dat1);
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x0B, 1, u08_dat2);
            ret = BitConverter.ToInt16(new byte[2] { u08_dat2[0], u08_dat1[0] }, 0);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] FRA Point(REG 0x0A) = {0}", ret));

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x0C, 1, new byte[1] { (byte)(fra_setting.start_freq >> 8) /* 0x00 */ });     /* start_freq. MSB */
            Thread.Sleep(10);
            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x0D, 1, new byte[1] { (byte)(fra_setting.start_freq & 0xFF) /* 0x64 */ });   /* start_freq LSB */
            Thread.Sleep(10);
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x0C, 1, u08_dat1);
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x0D, 1, u08_dat2);
            ret = BitConverter.ToInt16(new byte[2] { u08_dat2[0], u08_dat1[0] }, 0);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] FRA start_freq(REG 0x0C) = {0}", ret));

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x0E, 1, new byte[1] { (byte)(fra_setting.end_freq >> 8) /* 0x03 */ });     /* end_freq. MSB */
            Thread.Sleep(10);
            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x0F, 1, new byte[1] { (byte)(fra_setting.end_freq & 0xFF) /* 0xe8 */ });   /* end_freq LSB */
            Thread.Sleep(10);

            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x0E, 1, u08_dat1);
            m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x0F, 1, u08_dat2);
            ret = BitConverter.ToInt16(new byte[2] { u08_dat2[0], u08_dat1[0] }, 0);
            m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] FRA end_freq(REG 0x0E) = {0}", ret));

            m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x01, 1, new byte[1] { 0x01 });     //FRA Mode ON
            Thread.Sleep(200);

            cnt = 0;

            while (cnt < m__G.fGraph.DriverIC.TEST_LIMIT_CNT)
            {
                m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x02, 1, sts_check);

                if (sts_check[0] == 0x01)
                {
                    m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_Measurement] Status check 0x{0:X2} success...cnt = {1}", sts_check[0], cnt));
                    break;
                }
                else
                {
                    Thread.Sleep((int)m__G.fGraph.DriverIC.TEST_DELAY_TIME);
                    cnt++;

                    if (cnt == m__G.fGraph.DriverIC.TEST_LIMIT_CNT)
                    {
                        m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_Measurement] Error!!! Status check over time. cnt = {0}", cnt));
                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x09, 1, err_list);

                        m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_Measurement] Error!!! Mode ON status check failed !!! -- Status = 0x{0:X2} -- Error List = 0x{1:X2}", sts_check[0], err_list[0]));
                        m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x01, 1, new byte[1] { 0x00 });        // FRA Test Stop
                        return (int)m__G.fGraph.DriverIC.MODE_ON_STS_CHK_ERR;
                    }
                }
            }

            //m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x01, 1, new byte[1] { 0x03 });    /* FRA Test Start */
            m__G.fGraph.DriverIC.FRA_Echoboard_StartStop(ch, StartStopType.Start);
            Thread.Sleep(500);      /* Waiting for FRA measurement */

            cnt = 0;
            while (cnt < m__G.fGraph.DriverIC.TEST_LIMIT_CNT)
            {
                m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x02, 1, sts_check);
                Thread.Sleep(30);
                if (sts_check[0] == m__G.fGraph.DriverIC.FRA_TEST_COMPLETE) // Test complete
                {
                    /* Test Finished */
                    //m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x01, 1, new byte[1] { 0x00 });    /* FRA Test Stop */
                    m__G.fGraph.DriverIC.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                    Thread.Sleep(100);

                    m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] cnt = {0} -- status check: 0x{1:%02X}", cnt, sts_check[0]));
                    m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] Measuring..."));

                    for (int i = 0; i < fra_setting.test_point; i++)
                    {  /* Read Result */
                        m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x40, 1, new byte[1] { (byte)(i >> 8) });
                        m__G.fGraph.DriverIC.WriteToDW9785_FRA(ch, 0x41, 1, new byte[1] { (byte)(i & 0xFF) });
                        Thread.Sleep(3);

                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x46, 1, u08_dat1);
                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x47, 1, u08_dat2);
                        freq = (ushort)((u08_dat1[0] << 8) + u08_dat2[0]);

                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x42, 1, u08_dat1);
                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x43, 1, u08_dat2);
                        gain = (short)((u08_dat1[0] << 8) + u08_dat2[0]) / (double)10;

                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x44, 1, u08_dat1);
                        m__G.fGraph.DriverIC.ReadFromDW9785_FRA(ch, 0x45, 1, u08_dat2);
                        phase = (short)((u08_dat1[0] << 8) + u08_dat2[0]) / (double)10;
                        freq_buf[i] = freq;
                        gain_buf[i] = gain;
                        phase_buf[i] = phase;
                    }

                    break;
                }
                else
                {
                    Thread.Sleep((int)m__G.fGraph.DriverIC.TEST_DELAY_TIME);
                    cnt++;
                }
            }

            if (sts_check[0] != m__G.fGraph.DriverIC.FRA_TEST_COMPLETE)
            {
                m__G.fGraph.DriverIC.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                m__G.fManage.AddOperatorLog(ch, string.Format("[Echo_FRA_FullScanMode_Measurement] Error!!! FRA Test Time's OUT !!! Test Freq. {0:%.2f} -- Measure not completed !!!", freq));
                return (int)m__G.fGraph.DriverIC.TEST_STS_CHK_ERR;
            }

            for (int i = 0; i < freq_buf.Length; i++)
            {
                if (freq_buf[i] == 10)
                {
                    fra_result.freq10HzGain = gain_buf[i];
                    break;
                }
            }

            bool isStart = false;

            Array.Reverse(freq_buf);
            Array.Reverse(gain_buf);
            Array.Reverse(phase_buf);
            for (int i = 0; i < fra_setting.test_point; i++)
            {
                if (!gmtest)
                {
                    if (gain_buf[i] < 0 && !isStart)
                    {
                        isStart = true;
                    }
                    if (isStart)
                    {
                        if (gain_buf[i] >= 0)
                        {
                            SearchCnt = i;
                            break;
                        }
                    }
                }
                else
                {
                    if (i >= 1)
                    {
                        if (phase_buf[i - 1] > 165 && phase_buf[i] < -165)
                        {
                            SearchCnt = i;
                            break;
                        }
                    }
                }
            }

            m__G.fGraph.DriverIC.FRA_Echoboard_StartStop(ch, StartStopType.Stop);

            // Print test results:
            m__G.fManage.AddOperatorLog(ch, string.Format("=================================================="));

            for (int i = 0; i < fra_setting.test_point; i++)
            {
                m__G.fManage.AddOperatorLog(ch, string.Format("{0},{1},{2}", freq_buf[i], gain_buf[i], phase_buf[i]));
            }

            m__G.fManage.AddOperatorLog(ch, string.Format("=================================================="));
            return (int)m__G.fGraph.DriverIC.TEST_OK;
        }
    }
}
