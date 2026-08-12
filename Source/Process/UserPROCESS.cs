using FZ4P.DriverIc.OISIC;
using FZ4P.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace FZ4P
{
    public partial class Process
    {
        int[] g_IME = new int[2];
        double[] AFCurrentMinMax = new double[2];
        double[] OISXCurrentMinMax = new double[2];
        double[] OISYCurrentMinMax = new double[2];

        byte[] IC_SETTING_AF = new byte[1];
        byte[] IC_SETTING_AF_REG = new byte[1];
        byte[] IC_SETTING_OIS_X = new byte[1];
        byte[] IC_SETTING_OIS_X_REG = new byte[1];
        byte[] IC_SETTING_OIS_Y = new byte[1];
        byte[] IC_SETTING_OIS_Y_REG = new byte[1];
        byte[] IC_DATA_AF = new byte[1];
        byte[] IC_DATA_AF_REG = new byte[1];
        byte[] IC_DATA_OIS_X = new byte[1];
        byte[] IC_DATA_OIS_X_REG = new byte[1];
        byte[] IC_DATA_OIS_Y = new byte[1];
        byte[] IC_DATA_OIS_Y_REG = new byte[1];
        byte AFPIDVersion = 0xFF;
        byte OISPIDVersion = 0xFF;

        void AddSequence()
        {
            ItemList.Add(new ActItems() { Name = "AF HallCalibration", Func = AF_HallCalibration, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS HallCalibration", Func = OIS_HallCalibration, IsMulti = true });      
            ItemList.Add(new ActItems() { Name = "AF Gain Margin", Func = AFGM, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "AF Phase Margin", Func = AFPM, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS LinearityCompensation", Func = OISLCCComp, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS Gain Margin", Func = OISGM, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS Phase Margin", Func = OISPM, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS Gain Margin Low", Func = OISGM_LOW, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "OIS Phase Margin Low", Func = OISPM_LOW, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "SineWave Test", Func = OISSineWave, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "Ringing Test", Func = OISRinging, IsMulti = true });

            ItemList.Add(new ActItems() { Name = "Changed I3C Mode", Func = OIS_ChangedI3C, IsMulti = true });
            ItemList.Add(new ActItems() { Name = "Changed I2C Mode", Func = OIS_ChangedI2C, IsMulti = true });
        }

        #region AddSeq

        void OISPM(int ch, string testItem, int inspCnt)
        {
            double[] res = new double[2];

            sFRA_Margin fra_result = new sFRA_Margin();
            sFRA_TestSetting fra_setting = new sFRA_TestSetting()
            {
                ois_slave_id = (byte)(DWDrvIC.OISX_Addr << 1),
                ois_mode = 0x00,
                test_point = Condition.iFRAStep,
                amplitude = Condition.iXAmplitude,
                dc_bias_ofst = Condition.iXOffset,
                start_freq = Condition.iXChirpTo,
                end_freq = Condition.iXChirpFrom,
            };

            OISPM(ch, (int)AxisTypeDW.AxisX, fra_setting,ref fra_result);

            PassFails[0].Results[(int)SpecItem.FRAX_PhaseMargin].Val = fra_result.phase_margin;
            ShowDataResults(ch, (int)SpecItem.FRAX_PhaseMargin, (int)SpecItem.FRAX_PhaseMargin, InspType.Normal, new double[] { });

            fra_result = new sFRA_Margin();
            fra_setting = new sFRA_TestSetting()
            {
                ois_slave_id = (byte)(DWDrvIC.OISY_Addr << 1),
                ois_mode = 0x00,
                test_point = Condition.iFRAStep,
                amplitude = Condition.iYAmplitude,
                dc_bias_ofst = Condition.iYOffset,
                start_freq = Condition.iYChirpTo,
                end_freq = Condition.iYChirpFrom,
            };
            
            OISPM(ch, (int)AxisTypeDW.AxisY, fra_setting, ref fra_result);

            PassFails[0].Results[(int)SpecItem.FRAY_PhaseMargin].Val = fra_result.phase_margin;
            ShowDataResults(ch, (int)SpecItem.FRAY_PhaseMargin, (int)SpecItem.FRAY_PhaseMargin, InspType.Normal, new double[] { });
        }
        void OISGM(int ch, string testItem, int inspCnt)
        {
            double[] res = new double[2];

            sFRA_Margin fra_result = new sFRA_Margin();
            sFRA_TestSetting fra_setting = new sFRA_TestSetting()
            {
                ois_slave_id = (byte)(DWDrvIC.OISX_Addr << 1),
                ois_mode = 0x00,
                test_point = Condition.iFRAStep_GM,
                amplitude = (int)Condition.iXAmplitude_GM,
                dc_bias_ofst = (int)Condition.iXOffset_GM,
                start_freq = Condition.iXChirpTo_GM,
                end_freq = Condition.iXChirpFrom_GM
            };
            
            OISGM(ch, (int)AxisTypeDW.AxisX, fra_setting,ref fra_result);

            PassFails[0].Results[(int)SpecItem.FRAX_GainMargin].Val = fra_result.gain_margin_freq;
            ShowDataResults(ch, (int)SpecItem.FRAX_GainMargin, (int)SpecItem.FRAX_GainMargin, InspType.Normal, new double[] { });

            fra_result = new sFRA_Margin();
            fra_setting = new sFRA_TestSetting()
            {
                ois_slave_id = (byte)(DWDrvIC.OISY_Addr << 1),
                ois_mode = 0x00,
                test_point = Condition.iFRAStep_GM,
                amplitude = (int)Condition.iYAmplitude_GM,
                dc_bias_ofst = (int)Condition.iYOffset_GM,
                start_freq = Condition.iYChirpTo_GM,
                end_freq = Condition.iYChirpFrom_GM,
            };
            
            OISGM(ch, (int)AxisTypeDW.AxisY, fra_setting, ref fra_result);

            PassFails[0].Results[(int)SpecItem.FRAY_GainMargin].Val = fra_result.gain_margin_freq;
            ShowDataResults(ch, (int)SpecItem.FRAY_GainMargin, (int)SpecItem.FRAY_GainMargin, InspType.Normal, new double[] { });
        }

        void OISPM_LOW(int ch, string testItem, int inspCnt)
        {
            double[] res = new double[2];

            sFRA_Margin fra_result = new sFRA_Margin();
            sFRA_TestSetting fra_setting = new sFRA_TestSetting()
            {
                ois_slave_id = (byte)(DWDrvIC.OISX_Addr << 1),
                ois_mode = 0x00,
                test_point = Condition.iFRAStep_LOW,
                amplitude = Condition.iXAmplitude_LOW,
                dc_bias_ofst = Condition.iXOffset_LOW,
                start_freq = Condition.iXChirpTo_LOW,
                end_freq = Condition.iXChirpFrom_LOW,
            };

            OISPM(ch, (int)AxisTypeDW.AxisX, fra_setting, ref fra_result);

            PassFails[0].Results[(int)SpecItem.FRAX_PhaseMarginLow].Val = fra_result.phase_margin;
            ShowDataResults(ch, (int)SpecItem.FRAX_PhaseMarginLow, (int)SpecItem.FRAX_PhaseMarginLow, InspType.Normal, new double[] { });

            fra_result = new sFRA_Margin();
            fra_setting = new sFRA_TestSetting()
            {
                ois_slave_id = (byte)(DWDrvIC.OISY_Addr << 1),
                ois_mode = 0x00,
                test_point = Condition.iFRAStep_LOW,
                amplitude = Condition.iYAmplitude_LOW,
                dc_bias_ofst = Condition.iYOffset_LOW,
                start_freq = Condition.iYChirpTo_LOW,
                end_freq = Condition.iYChirpFrom_LOW,
            };

            OISPM(ch, (int)AxisTypeDW.AxisY, fra_setting, ref fra_result);

            PassFails[0].Results[(int)SpecItem.FRAY_PhaseMarginLow].Val = fra_result.phase_margin;
            ShowDataResults(ch, (int)SpecItem.FRAY_PhaseMarginLow, (int)SpecItem.FRAY_PhaseMarginLow, InspType.Normal, new double[] { });
        }
        void OISGM_LOW(int ch, string testItem, int inspCnt)
        {
            double[] res = new double[2];

            sFRA_Margin fra_result = new sFRA_Margin();
            sFRA_TestSetting fra_setting = new sFRA_TestSetting()
            {
                ois_slave_id = (byte)(DWDrvIC.OISX_Addr << 1),
                ois_mode = 0x00,
                test_point = Condition.iFRAStep_GM_Low,
                amplitude = (int)Condition.iXAmplitude_GM_Low,
                dc_bias_ofst = (int)Condition.iXOffset_GM_Low,
                start_freq = Condition.iXChirpTo_GM_Low,
                end_freq = Condition.iXChirpFrom_GM_Low
            };

            OISGM(ch, (int)AxisTypeDW.AxisX, fra_setting,ref fra_result);

            PassFails[0].Results[(int)SpecItem.FRAX_GainMarginLow].Val = fra_result.gain_margin_freq;
            ShowDataResults(ch, (int)SpecItem.FRAX_GainMarginLow, (int)SpecItem.FRAX_GainMarginLow, InspType.Normal, new double[] { });

            fra_result = new sFRA_Margin();
            fra_setting = new sFRA_TestSetting()
            {
                ois_slave_id = (byte)(DWDrvIC.OISY_Addr << 1),
                ois_mode = 0x00,
                test_point = Condition.iFRAStep_GM_Low,
                amplitude = (int)Condition.iYAmplitude_GM_Low,
                dc_bias_ofst = (int)Condition.iYOffset_GM_Low,
                start_freq = Condition.iYChirpTo_GM_Low,
                end_freq = Condition.iYChirpFrom_GM_Low,
            };

            OISGM(ch, (int)AxisTypeDW.AxisY, fra_setting,ref fra_result);

            PassFails[0].Results[(int)SpecItem.FRAY_GainMarginLow].Val = fra_result.gain_margin_freq;
            ShowDataResults(ch, (int)SpecItem.FRAY_GainMarginLow, (int)SpecItem.FRAY_GainMarginLow, InspType.Normal, new double[] { });
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
        public bool Load_OISXPID(string path)
        {
            try
            {
                string textVal = File.ReadAllText(path);
                string[] t = textVal.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                IC_DATA_OIS_X = new byte[(t.Length - 2)];
                IC_DATA_OIS_X_REG = new byte[(t.Length - 2)];
                OISPIDVersion = 0xFF;
                for (int i = 0; i < t.Length; i++)
                {
                    if (i == 0)
                    {
                        string[] b = t[i].Split(new string[] { ",", " ", "\t", "//", "Reg", "AF", "PID", "Version" }, StringSplitOptions.RemoveEmptyEntries);
                        OISPIDVersion = Convert.ToByte(b[0], 16);
                    }
                    else if (i == 1)
                    {
                        //string[] b = t[i].Split(new string[] { ",", " ", "\t", "//", "Reg" }, StringSplitOptions.RemoveEmptyEntries);
                        //IC_SETTING_OIS_X = new byte[b.Length / 2];
                        //IC_SETTING_OIS_X_REG = new byte[b.Length / 2];
                        //for (int j = 0; j < b.Length; j++)
                        //{
                        //    if (j < b.Length / 2) IC_SETTING_OIS_X[j] = Convert.ToByte(b[j], 16);
                        //    else IC_SETTING_OIS_X_REG[j - b.Length / 2] = Convert.ToByte(b[j], 16);
                        //}
                    }
                    else
                    {
                        string[] b = t[i].Split(new string[] { ",", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

                        IC_DATA_OIS_X_REG[(i - 2)] = Convert.ToByte(b[0], 16);
                        IC_DATA_OIS_X[(i - 2)] = Convert.ToByte(b[1], 16);

                    }

                }
                return true;
            }
            catch { return false; }

        }
        public bool Load_OISYPID(string path)
        {
            try
            {
                string textVal = File.ReadAllText(path);
                string[] t = textVal.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                IC_DATA_OIS_Y = new byte[(t.Length - 2)];
                IC_DATA_OIS_Y_REG = new byte[(t.Length - 2)];
                OISPIDVersion = 0xFF;
                for (int i = 0; i < t.Length; i++)
                {
                    if (i == 0)
                    {
                        string[] b = t[i].Split(new string[] { ",", " ", "\t", "//", "Reg", "AF", "PID", "Version" }, StringSplitOptions.RemoveEmptyEntries);
                        OISPIDVersion = Convert.ToByte(b[0], 16);
                    }
                    else if (i == 1)
                    {
                        //string[] b = t[i].Split(new string[] { ",", " ", "\t", "//", "Reg" }, StringSplitOptions.RemoveEmptyEntries);
                        //IC_SETTING_OIS_Y = new byte[b.Length / 2];
                        //IC_SETTING_OIS_Y_REG = new byte[b.Length / 2];
                        //for (int j = 0; j < b.Length; j++)
                        //{
                        //    if (j < b.Length / 2) IC_SETTING_OIS_Y[j] = Convert.ToByte(b[j], 16);
                        //    else IC_SETTING_OIS_Y_REG[j - b.Length / 2] = Convert.ToByte(b[j], 16);
                        //}
                    }
                    else
                    {
                        string[] b = t[i].Split(new string[] { ",", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

                        IC_DATA_OIS_Y_REG[(i - 2)] = Convert.ToByte(b[0], 16);
                        IC_DATA_OIS_Y[(i - 2)] = Convert.ToByte(b[1], 16);

                    }

                }
                return true;
            }
            catch { return false; }

        }

        void AF_HallCalibration(int ch, string testItem, int InspCnt)
        {
           
            Dln.PowerSequence(0);
            Wait(100);

            int BTM_POS = 10;
            int TOP_POS = 820;
            int TOP_MARGIN = 10;

            byte[] rbuf = new byte[1];
            int agingCount;
            double OldStroke = 0, NewStroke = 0;
            FindResult res = new FindResult();
            double[] zVal = new double[2];

            DrvIC.AK7314_Mode(ch, 0);
            Wait(5);
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x3B });
            DrvIC.ReadArray(ch, DrvIC.AFSlaveAddr, 0x0B, 1, rbuf);
            byte backdata = rbuf[0];
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0x0B, 1, new byte[] { (byte)(rbuf[0] & 0x7F) });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xA6, 1, new byte[] { 0x7B });
            DrvIC.AK7314_Mode(ch, 1);
            AddLog(ch, $"AF Openloop Stroke Check");

            LEDs_All_On(0, true);
            for (agingCount = 0, NewStroke = 0; (agingCount < 10) || ((agingCount < 20) && (NewStroke > OldStroke)); agingCount++)
            {
                OldStroke = NewStroke;
                DrvIC.Move(ch, "AF", 4095);
                Wait(50);
                res = Measure();
                zVal[0] = res.cz[0];
                DrvIC.Move(ch, "AF", 0);
                Wait(50);
                res = Measure();
                zVal[1] = res.cz[0];
                NewStroke = Math.Abs(zVal[1] - zVal[0]);
                AddLog(ch, $"{agingCount + 1} : {NewStroke.ToString("F3")}");
            }

            //DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0x0B, 1, new byte[] { backdata });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xA6, 1, new byte[] { 0x00 });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x00 });
            DrvIC.AK7314_Mode(ch, 0);
            Wait(5);

            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x3B });

            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, IC_SETTING_AF_REG[0], 1, new byte[] { IC_SETTING_AF[0] });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, IC_SETTING_AF_REG[1], 1, new byte[] { IC_SETTING_AF[1] });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, IC_SETTING_AF_REG[3], 1, new byte[] { IC_SETTING_AF[3] });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, IC_SETTING_AF_REG[4], 1, new byte[] { IC_SETTING_AF[4] });

            //EPA Reset
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0x0E, 1, new byte[] { 0x00 });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0x0F, 1, new byte[] { 0x00 });
            AddLog(ch, "Reset EPA Data.");
            //Linearity Reset

            DrvIC.AF_LinearityComp_Reset(ch);
            AddLog(ch, "AF Linearity Comp Reset");
            for (int i = 0; i < IC_DATA_AF.Length; i++)
            {
                Dln.WriteByte(ch, DrvIC.AF_Addr, IC_DATA_AF_REG[i], 1, IC_DATA_AF[i]);
                AddLog(ch, $"PID Parameter 0x{IC_DATA_AF_REG[i].ToString("X2")},0x{IC_DATA_AF[i].ToString("X2")} ");
            }
            AddLog(ch, $"PID Parameter setting.");

            /*임의 추가*/
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xCA, 1, new byte[] { 0x46 });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xCB, 1, new byte[] { 0xD8 });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xCC, 1, new byte[] { 0x40 });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xCD, 1, new byte[] { 0x32 });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xCE, 1, new byte[] { 0x00 });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0x3D, 1, new byte[] { 0x00 });
            AddLog(ch, "Function Register Setting");


            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xC9, 1, new byte[] { 0x00 });
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0x02, 1, new byte[] { 0x80 });
            Wait(10);
            DrvIC.ReadArray(ch, DrvIC.AFSlaveAddr, 0x70, 1, rbuf);
            AddLog(ch, $"Read 0x70 : 0x{rbuf[0].ToString("X")}");
            DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xC9, 1, rbuf);
            AddLog(ch, "Temp register setting");
            for (int i = 0; i < 5; i++)
            {
                DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, IC_SETTING_AF_REG[2], 1, new byte[] { IC_SETTING_AF[2] });
                for (int j = 0; j < 2; j++)
                {
                    DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0x02, 1, new byte[] { 0x18 });
                    Wait(300);
                }
                Dln.ReadArray(ch, DrvIC.AFSlaveAddr, 0x19, 1, rbuf);
                byte tmpData = (byte)Math.Floor(rbuf[0] * 0.75);
                //임시 주석
                if (tmpData >= 0x00 && tmpData <= 0x30)
                {
                    DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0x19, 1, new byte[] { tmpData });
                    AddLog(ch, "AF Calibration OK!");
                    break;
                }
                else
                {
                    // SetError(ch, NonSpecItem.AF_HallCalibration);
                    AddLog(ch, "AF Calibration (Reg 19) error[over 0x90]");
                }
            }
       //     DrvIC.WriteArray(ch, DrvIC.AFSlaveAddr, 0xF3, 1, new byte[] { 0x1E });
          //  Wait(25);
            bool WriteRes = DrvIC.AK7314_memory_update(ch, 1);
            WriteRes &= DrvIC.AK7314_memory_update(ch, 2);
            WriteRes &= DrvIC.AK7314_memory_update(ch, 3);
            WriteRes &= DrvIC.AK7314_memory_update(ch, 4);
            WriteRes &= DrvIC.AK7314_memory_update(ch, 5);

            if (!WriteRes)
            {
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });

                AddLog(ch, "AF Calibration Memory Update Fail");
                return;
            }
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x00 });
            DrvIC.Move(ch, "AF", 2048);
            DrvIC.AK7314_Mode(ch, 1);
            DrvIC.AK7314_IC_Data(ch);

            DWDrvIC.OISOnOff(ch, false);
            Wait(100);

            //  AF EPA
            AddLog(ch, "<<<  AF EPA Start  >>>");
            short btm_position, tmp_position, top_position, ctr_position;
            short step, inf_cut, mac_cut;
            ushort posvt, negvt, target_code;
            short stroke;
            int loop = 0, mac_loop = 0;
            int new_con = 0, old_con = 0, cond = 0;
            int mac_loop_max = 50;
            ushort inf_tag_code, mac_tag_code;	// save code value

            DrvIC.AK7314_IC_Data(ch);
            DrvIC.Move(ch, "AF", 2048);
            Wait(50);
            res = Measure();
            ctr_position = (short)res.cz[0];
            DrvIC.Ak7314_soft_move(ch, 0, 10);
            res = Measure();
            short refPos = btm_position = (short)res.cz[0];
            tmp_position = 0;
            AddLog(ch, "Inf Cut Start");
            for (target_code = 0, step = 0x200; step > 0; step >>= 1)
            {
                AddLog(ch, $"tmp_pos:{tmp_position}, tar_code:{target_code}, step:{step}");
                if (tmp_position < BTM_POS - 1) target_code += (ushort)step;
                else if (tmp_position > BTM_POS + 1) target_code -= (ushort)step;
                else break;
                DrvIC.Move(ch, "AF", target_code);
                Wait(50);
                res = Measure();
                tmp_position = btm_position = (short)(res.cz[0] - refPos);
                loop++;
            }
            inf_tag_code = target_code;
            AddLog(ch, $"Inf_loop:{loop}");
            negvt = target_code; inf_cut = tmp_position;
            AddLog(ch, $"Inf_cut:{inf_cut}");
            if ((inf_cut < (BTM_POS - 1)) || (inf_cut > (BTM_POS + 1)))
            {
                AddLog(ch, $"EPA Error");
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });

                LEDs_All_On(0, false);
                return;
            }
            AddLog(ch, $"");

            DrvIC.Ak7314_soft_move(ch, 4095, 10);
            res = Measure();
            top_position = (short)res.cz[0];
            tmp_position = 0;
            stroke = (short)Math.Abs(refPos - top_position);

            if (stroke > TOP_POS + TOP_MARGIN)
            {
                mac_cut = (short)(stroke - (TOP_POS));
                step = 0x300;
                //step = 0xC0;
            }
            else
            {
                mac_cut = (short)TOP_MARGIN;
                step = 0x200;
                //step = 0x80;
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
                DrvIC.Move(ch, "AF", target_code);
                Wait(50);
                res = Measure();
                tmp_position = (short)(res.cz[0] - top_position);
                mac_loop++;

                if (mac_loop > mac_loop_max) break;
            }
            mac_tag_code = target_code;

            if (mac_loop > mac_loop_max)
            {
                AddLog(ch, $"EPA Error");
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });

                LEDs_All_On(0, false);
                return;
            }
            AddLog(ch, $"tmp_pos:{tmp_position}, tar_code:{target_code}, mac_loop:{mac_loop}");
            posvt = target_code;
            AddLog(ch, "");
            AddLog(ch, "---------------------------------");
            AddLog(ch, $"Target stroke : {810}um");
            AddLog(ch, $"Target btm_top MG : {BTM_POS}_{TOP_MARGIN} um");
            AddLog(ch, $"Measured stroke : {stroke}um");
            AddLog(ch, $"Measured Mac_cut : {mac_cut}um");
            AddLog(ch, $"Inf cut-off size : {inf_cut}um");
            AddLog(ch, $"Mac cut-off size : {Math.Abs(tmp_position)}um");
            AddLog(ch, "---------------------------------");
            AddLog(ch, $"Inf/Mac target_code : {inf_tag_code}, {mac_tag_code}um");
            AddLog(ch, "---------------------------------");

            DrvIC.Move(ch, "AF", 2048);
            Wait(50);
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x3B });
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, IC_SETTING_AF_REG[1], 1, new byte[] { IC_SETTING_AF[1] });

            AFPOSVT = (byte)((4095 - posvt + 2) >> 4);      // for SU2810
            AFNEGVT = (byte)((negvt + 2) >> 4);

            AddLog(ch, $"posvt({posvt}) negvt({negvt}) POSVT({AFPOSVT}) NEGVT({AFNEGVT})");

            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x0E, 1, new byte[] { (byte)AFPOSVT });
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x0F, 1, new byte[] { (byte)AFNEGVT });
            DrvIC.AK7314_memory_update(ch, 1);
            DrvIC.AK7314_memory_update(ch, 5);
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x00 });
            DrvIC.AK7314_Mode(ch, 1);
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


            DWDrvIC.OISOnOff(ch, false);
            Thread.Sleep(100);

          //  AF LinCompDWDrvIC
            AddLog(ch, "<<<  AF Lin. Comp Start  >>>");
            bool LinRes = AFLinComp(ch, 8, 4088, 34, 0, 0, 6, 6, 0, (int)stroke);
            //bool LinRes = false;
            AddLog(ch, "<<<  AF Lin. Comp End  >>>");
            DWDrvIC.OISOnOff(ch, true);
            DWDrvIC.OISOnOff(ch, false);
            Wait(100);

            LEDs_All_On(0, false);
            if (!LinRes)
            {
                PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = 0;
                ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
                return;
            }
            PassFails[0].Results[(int)SpecItem.AF_NonEPAStroke].Val = stroke;
            ShowDataResults(ch, (int)SpecItem.AF_NonEPAStroke, (int)SpecItem.AF_NonEPAStroke, InspType.Normal, new double[] { });
        }
        void OIS_HallCalibration(int ch, string testItem, int InspCnt)
        {
            LEDs_All_On(ch, true);
            DWDrvIC.SetOperationMode(AxisTypeDW.AxisX,OperationTypeDW.OpenMode);

            for (int i = 0; i< 10 ; i++)
            {  
                DWDrvIC.OISMove(ch, DWDrvIC.OIS_MIN_CODE, DWDrvIC.OIS_MID_CODE);
                Thread.Sleep(200);
                var position = Measure();
                double[] cx = new double[2];
                cx[0] = position.cx[0];
                Thread.Sleep(50);

                DWDrvIC.OISMove(ch, DWDrvIC.OIS_MAX_CODE-1, DWDrvIC.OIS_MID_CODE);
                Thread.Sleep(200);
                position = Measure();
                cx[1] = position.cx[0];
                var stroke = cx[1] - cx[0];
                var d= Math.Round(stroke, 2);
                AddLog(ch, $"Open Loop Stroke : {d} um");
                Thread.Sleep(50);
            }
            LEDs_All_On(ch, false);

            //AF BestPos Move
            DrvIC.AFOnOff(ch, true);
            DrvIC.AFMove(ch, Condition.OISCalAFPos);
            
            AddLog(ch, $"Move AF Position :  {Condition.OISCalAFPos}");

            #region OIS Hall Calibration
            AddLog(ch, "OIS X PID Write Start");
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x28, 1, 0x39);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x39, 1, 0xA0);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x7D, 1, 0x00);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x03, 1, 0x01);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x55, 1, 0x00);

            Wait(55);

            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x02, 1, 0x40);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x28, 1, 0x39);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x28, 1, 0xA0);

            for (int i = 0; i < IC_DATA_OIS_X.Length; i++)
            {
               DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, IC_DATA_OIS_X_REG[i], 1, IC_DATA_OIS_X[i]);
            }

            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x02, 1, 0x40);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x02, 1, 0x04);
            Wait(800);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x02, 1, 0x40);

            byte data = DWDrvIC.Controls.ReadByte(DWDrvIC.OISX_Addr, 0x44, 1);
            if (data == 0x01)
            {
                AddLog(ch, $"OIS X Hall Calibration Success");
                DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x28, 1, 0x39);
                DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x28, 1, 0xA0);
                DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x02, 1, 0x40);
                DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x03, 1, 0x01);
                Wait(20);
                DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x28, 1, 0x14);
            }
            else
            {
                
                AddLog(ch, $"OIS X Hall Calibration Fail");
                PassFails[0].Results[(int)SpecItem.XYHallCalibration].Val = 1;
                ShowDataResults(ch, (int)SpecItem.XYHallCalibration, (int)SpecItem.XYHallCalibration, InspType.OKNG, new double[] { });
                return;
            }


            SetEPA((int)AxisTypeDW.AxisX);
            #endregion

            #region OIS Y Hall Calibration
            AddLog(ch, "OIS Y PID Parameter Setting");
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x28, 1, 0x39);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x39, 1, 0xA0);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x7D, 1, 0x00);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x03, 1, 0x01);
            Wait(55);

            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x02, 1, 0x40);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x28, 1, 0x39);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x28, 1, 0xA0);

            for (int i = 0; i < IC_DATA_OIS_Y.Length; i++)
            {
                DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, IC_DATA_OIS_Y_REG[i], 1, IC_DATA_OIS_Y[i]);
            }

            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x02, 1, 0x40);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x02, 1, 0x04);
            Wait(800);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x02, 1, 0x40);


            data = DWDrvIC.Controls.ReadByte(DWDrvIC.OISY_Addr, 0x44, 1);
            if (data == 0x01)
            {
                AddLog(ch, $"OIS Y Hall Calibration Success");
                DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x28, 1, 0x39);
                DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x28, 1, 0xA0);
                DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x02, 1, 0x40);
                DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x03, 1, 0x01);
                Wait(20);
                DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x28, 1, 0x14);
                PassFails[0].Results[(int)SpecItem.XYHallCalibration].Val = 0;
                ShowDataResults(ch, (int)SpecItem.XYHallCalibration, (int)SpecItem.XYHallCalibration, InspType.OKNG, new double[] { });
            }
            else
            {
                AddLog(ch, $"OIS Y Hall Calibration Fail");
                PassFails[0].Results[(int)SpecItem.XYHallCalibration].Val = 1;
                ShowDataResults(ch, (int)SpecItem.XYHallCalibration, (int)SpecItem.XYHallCalibration, InspType.OKNG, new double[] { });
                return;
            }

            //SetEPA((int)AxisTypeDW.AxisY);
            #endregion
        }

        private void SetEPA(int iAxis)
        {
            FindResult res = null;
            double fullStroke = -1;
            int Btmpos = 0, TTL_RNG = 0, Top_pos =0, Top_Margin = 0;
            if (iAxis == (int)AxisTypeDW.AxisX)
            {
                Btmpos = Condition.iOISXEPABtmPos;
                TTL_RNG = Condition.iOISXEPATtlRng;
                Top_Margin = Condition.iOISXEPATopMargin;
            }
            else if (iAxis == (int)AxisTypeDW.AxisY)
            {
                Btmpos = Condition.iOISYEPABtmPos;
                TTL_RNG = Condition.iOISYEPATtlRng;
                Top_Margin = Condition.iOISYEPATopMargin;
            }
            
            Top_pos = Btmpos + TTL_RNG;

           

            DrvIC.AFOnOff(0, true);
            DWDrvIC.OISOnOff(0,true);

            LEDs_All_On(0, true);
            
            DrvIC.AFMove(0, DrvIC.AF_MID_CODE);

            if (iAxis == (int)AxisTypeDW.AxisX)
            {
                //soft landing to min code
                SoftLangdingForEPA(iAxis, 0);
                res = Measure();
                Wait(50);
                double min = res.cx[0];

                //Set Zero
                SoftLangdingForEPA(iAxis, 1);
                res = Measure();
                Wait(50);
                fullStroke = res.cx[0] - min;
            }
            else if (iAxis == (int)AxisTypeDW.AxisY)
            {

                SoftLangdingForEPA(iAxis, 0);
                res = Measure();
                Wait(50);
                double min = res.cy[0];


                //Set Zero


                SoftLangdingForEPA(iAxis, 1);
                Wait(100);
                res = Measure();
                Wait(50);
                fullStroke = res.cy[0] - min;
            }

            DWDrvIC.Set_PT(iAxis, false);


            if (FindPosition_PCAL(iAxis, Top_pos, Top_Margin, fullStroke, TTL_RNG))
            {
                SoftLangdingForEPA(iAxis, 0);

                if (FindPosition_NCAL(iAxis, Btmpos, fullStroke))
                {
                    DWDrvIC.SetStore(iAxis);
                }
                else
                {
                    AddLog(0, $"NCAL Not Find");
                    return;
                }
            }
            else
            {
                AddLog(0, $"PCAL Not Find");
                return;
            }
            
            LEDs_All_On(0, false);
        }
        private void SoftLangdingForEPA(int iAxis, int mode)
        {
            if (mode == 0)
            {
                if(iAxis == (int)AxisTypeDW.AxisX)
                {
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MIN_CODE + (100 * 4), DWDrvIC.OIS_MID_CODE); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MIN_CODE + (20 * 4), DWDrvIC.OIS_MID_CODE); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MIN_CODE + (10 * 4), DWDrvIC.OIS_MID_CODE); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MIN_CODE, DWDrvIC.OIS_MID_CODE); Wait(50);
                     Wait(20);
                }
                else
                {
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MID_CODE, DWDrvIC.OIS_MIN_CODE + (100 * 4)); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MID_CODE, DWDrvIC.OIS_MIN_CODE + (20 * 4)); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MID_CODE, DWDrvIC.OIS_MIN_CODE + (10 * 4)); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MID_CODE, DWDrvIC.OIS_MIN_CODE); Wait(50);
                }
            }
           if (mode ==1)
            {
                if (iAxis == (int)AxisTypeDW.AxisX)
                {
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MAX_CODE - (100 * 4), DWDrvIC.OIS_MID_CODE); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MAX_CODE - (20 * 4), DWDrvIC.OIS_MID_CODE); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MAX_CODE - (10 * 4), DWDrvIC.OIS_MID_CODE); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MAX_CODE - 1, DWDrvIC.OIS_MID_CODE); Wait(50);
                }
                else
                {
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MID_CODE, DWDrvIC.OIS_MAX_CODE - (100 * 4)); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MID_CODE, DWDrvIC.OIS_MAX_CODE - (20 * 4)); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MID_CODE, DWDrvIC.OIS_MAX_CODE - (10 * 4)); Wait(20);
                    DWDrvIC.OISMove(0, DWDrvIC.OIS_MID_CODE, DWDrvIC.OIS_MAX_CODE - 1); Wait(50);
                }
            }

        }

        private bool FindPosition_PCAL(int iAxis, int Top_pos, int Top_Margin, double fullStroke, double TargetStroke)
        {
            FindResult res = null;
            int tmp_position = 0;
            int movecode = 0x00;
            int maxMoveCode = 0xFF;
            int loop = 0, mac_loop_max = 200;
            int Top_Cut = 0;
            SoftLangdingForEPA(iAxis, 1);
            Wait(200);
            res = Measure();
            int refPos;
            if (fullStroke - TargetStroke> Top_Margin)
            {
                Top_Cut =  (int)fullStroke - (int)TargetStroke ;
            }
            else
            {
                Top_Cut = Top_Margin;
            }
            if (iAxis == (int)AxisTypeDW.AxisX)
            {
                refPos = (int)res.cx[0];
            }
            else if (iAxis == (int)AxisTypeDW.AxisY)
            {
                refPos = (int)res.cy[0];
            }
            else
            {
                return false;
            }

            while (movecode <= maxMoveCode)
            {
                AddLog(0, $"PCAL.ADJ:{movecode}");

                DWDrvIC.SetPCAL(iAxis, movecode);

                Wait(200);

                res = Measure();

                if (iAxis == (int)AxisTypeDW.AxisX)
                {
                    tmp_position = (short)(refPos - res.cx[0]);
                }
                else if (iAxis == (int)AxisTypeDW.AxisY)
                {
                    tmp_position = (short)(refPos - res.cy[0]);
                    
                }

                AddLog(0, $"Position:{tmp_position}, PCAL.ADJ:{movecode:X2}");

                if (tmp_position > Top_Cut + 10)
                {
                    AddLog(0, $"Position:{tmp_position}, PCAL.ADJ:0x{movecode:X2}");
                    movecode -= 3;
                }

                if (tmp_position >= Top_Cut)
                {
                    AddLog(0, $"Reached Top position spec. Stop. Position:{tmp_position}, PCAL.ADJ:0x{movecode:X2}");
                    return true;
                }
                if (loop++ > mac_loop_max)
                {
                    AddLog(0, $"Loop count exceeded. Position:{tmp_position}, PCAL.ADJ:0x{movecode:X2}");
                    return false;
                }
                movecode++;

            }
            return false;
        }

        private bool FindPosition_NCAL(int iAxis, int BTM_POS, double fullStroke)
        {
            FindResult res = null;
            int tmp_position = 0;
            int movecode = 0x00;
            int maxMoveCode = 0xFF;
            int loop = 0, mac_loop_max = 100;
            Wait(200);
            res = Measure();
            int refPos;
            if (iAxis == (int)AxisTypeDW.AxisX)
            {
                refPos = (int)res.cx[0];
            }
            else if (iAxis == (int)AxisTypeDW.AxisY)
            {
                refPos = (int)res.cy[0];
            }
            else
            {
                return false;
            }

            while (movecode <= maxMoveCode)
            {
                AddLog(0, $"NCAL.ADJ:{movecode}");

                DWDrvIC.SetNCAL(iAxis, movecode);

                Wait(200);

                res = Measure();

                if (iAxis == (int)AxisTypeDW.AxisX)
                {
                    tmp_position = (short)(res.cx[0] - refPos);
                }
                else if (iAxis == (int)AxisTypeDW.AxisY)
                {
                    tmp_position = (short)(res.cy[0] - refPos);
                }

                AddLog(0, $"Position:{tmp_position}, NCAL.ADJ:0x{movecode:X2}");

                if (tmp_position > BTM_POS + 10)
                {
                    AddLog(0, $"Position:{tmp_position}, NCAL.ADJ:0x{movecode:X2}");
                    movecode -= 3;
                }

                if (tmp_position >= BTM_POS)
                {
                    AddLog(0, $"Reached Top position spec. Stop. Position:{tmp_position}, NCAL.ADJ:0x{movecode:X2}");
                    return true;
                }
                if (loop++ > mac_loop_max)
                {
                    AddLog(0, $"Loop count exceeded. Position:{tmp_position}, NCAL.ADJ:0x{movecode:X2}");
                    return false;
                }
                movecode++;

            }
            return false;

        }

        bool AFLinComp(int ch, int startpos, int endpos, int step, int margin_start, int margin_end, int s_value, int e_value, int linear_spec, int init_stroke)
        {
            int NUM_COEF = 13;
            FindResult tmpres = new FindResult();
            float[] targPosi = new float[step + 1]; // Array for storing target position data
            float[] lensPosi = new float[step + 1]; // Array for storing lens position data
            int[] readHall = new int[step + 1];
            float[] refLensPosi = new float[step + 1];
            int valueStepsize = step - s_value - e_value;
            float[] valueLensPosi = new float[valueStepsize + 1];
            float refStepsize = 0, gap = 0, valueStep = 0, valuegap = 0;
            float max_gap = 0, max_valuegap = 0;


            int ignInf = 0;
            int ignMac = 0;
            int numLinCompData;

            float RefData = 0;
            byte[] rbuf = new byte[1];
            int temp_table = endpos;
            int step_size = (endpos - startpos) / step;

            int[] linCoef = new int[NUM_COEF]; // Array for storing line compensation coefficients
            int pVtNew;    // Recalculation "POSVT" after linearity compensation
            int nVtNew;    // Recalculation "NEGVT" after linearity compensation
            float resError = 0;   // Variable for storing residual error after linearity compensation


            Dln.ReadArray(ch, DrvIC.AFSlaveAddr, 0x0E, 1, rbuf);
            byte pvt = rbuf[0];
            Dln.ReadArray(ch, DrvIC.AFSlaveAddr, 0x0F, 1, rbuf);
            byte nvt = rbuf[0];

            AddLog(ch, $"POSVT = {pvt}, NEGVT = {nvt}");
            AddLog(ch, $"Step Size : {step_size}");

            DrvIC.AK7314_Mode(ch, 1);
            DrvIC.Move(ch, "AF", endpos);
            Thread.Sleep(200);
            DWDrvIC.OISOnOff(ch, false);
            Wait(200);
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x3B });
            for (int i = 0; i < 13; i++)
                Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x30 + i, 1, new byte[] { 0x00 });
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x00 });


            AddLog(ch, $"Target\tReadHall\tPos");
            for (int i = step; i >= 0; i--)
            { // making input position table
                targPosi[i] = (float)temp_table;
                DrvIC.Move(ch, "AF", (int)targPosi[i]);
                Wait(150);
                readHall[i] = DrvIC.ReadHall(ch, "AF");
                tmpres = Measure();
                if (i != step) lensPosi[i] = (float)tmpres.cz[0] - RefData;
                else { lensPosi[i] = 0; RefData = (float)tmpres.cz[0]; }


                temp_table -= step_size; // From end to start
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
                    AFLinCompCoefAKM7314 coef = new AFLinCompCoefAKM7314();
                    int[] lincoef = new int[AFLinCompCoefAKM7314.NUM_COEF];
                    numLinCompData = lensPosi.Length;
                    AddLog(ch, $"numLinCompData = {numLinCompData}");
                    int res = coef.LinCompMain(targPosi, lensPosi, numLinCompData, pvt, nvt, ignInf, ignMac, ref lincoef, ref resError);
                    if (res != 0)
                    {
                        AddLog(ch, $"Linearity Comp Fail");

                        return false;
                    }
                    string s = $"0x30 : 0x{lincoef[0].ToString("X")}, 0x31 : 0x{lincoef[1].ToString("X")}, 0x32 : 0x{lincoef[2].ToString("X")}, 0x33 : 0x{lincoef[3].ToString("X")}, 0x34 : 0x{lincoef[4].ToString("X")}\r\n" +
                     $"0x35 : 0x{lincoef[5].ToString("X")}, 0x36 : 0x{lincoef[6].ToString("X")}, 0x37 : 0x{lincoef[7].ToString("X")}, 0x38 : 0x{lincoef[8].ToString("X")}, 0x39 : 0x{lincoef[9].ToString("X")}\r\n" +
                     $"0x3A : 0x{lincoef[10].ToString("X")}, 0x3B : 0x{lincoef[11].ToString("X")}, 0x3C : 0x{lincoef[12].ToString("X")}";

                    AddLog(ch, s);
                    DrvIC.Move(ch, "AF", AFCenter);

                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x3B });

                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x30, 1, new byte[] { (byte)lincoef[0] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x31, 1, new byte[] { (byte)lincoef[1] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x32, 1, new byte[] { (byte)lincoef[2] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x33, 1, new byte[] { (byte)lincoef[3] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x34, 1, new byte[] { (byte)lincoef[4] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x35, 1, new byte[] { (byte)lincoef[5] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x36, 1, new byte[] { (byte)lincoef[6] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x37, 1, new byte[] { (byte)lincoef[7] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x38, 1, new byte[] { (byte)lincoef[8] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x39, 1, new byte[] { (byte)lincoef[9] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x3A, 1, new byte[] { (byte)lincoef[10] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x3B, 1, new byte[] { (byte)lincoef[11] });
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x3C, 1, new byte[] { (byte)lincoef[12] });

                    DrvIC.AK7314_memory_update(ch, 1);
                    DrvIC.AK7314_memory_update(ch, 3);
                    Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x00 });

                    int btm_position = 0, top_position = 0, measured_stroke = 0, spec_stroke = 0;
                    DrvIC.Ak7314_soft_move(ch, 0, 10);
                    tmpres = Measure();
                    btm_position = (int)tmpres.cz[0];
                    DrvIC.Ak7314_soft_move(ch, 4095, 10);
                    tmpres = Measure();
                    top_position = (int)tmpres.cz[0];
                    measured_stroke = Math.Abs(btm_position - top_position);
                    spec_stroke = init_stroke * 8 / 10;
                    AddLog(ch, $"stroke : {measured_stroke}");
                    if (measured_stroke < spec_stroke)
                    {
                        AddLog(ch, $"stroke NG  (spec : over cal stroke 80%)");

                        return false;
                    }

                }
            }
            else
            {
                AddLog(ch, $"Linearity Comp Fail");
                return false;
            }
            DrvIC.AK7314_IC_Data(ch);
            return true;
        }
        void AFPM(int ch, string testItem, int inspCnt)
        {
            double resFreq = 0, respm = 0, res4dbpm = 0;
            int freqval, freqtemp = 0, gaintemp, freqpm = 0, oldfreq;
            int[] before_after_zero_freq = new int[2];
            double gainval = 0, pmval, phaestemp, prepm = 0, PM4dB;
            double[] before_after_zero_gain = new double[2];
            byte backup, flag_2nd = 0;
            byte fra_en;
            bool dB4PhaseFouund = false;
            bool PhaseFouund = false;

            DrvIC.SetSlaveAddr(ch, DrvIC.FRA_AFSlaveAddr);
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x02, 1, new byte[] { 0x00 });
            Wait(50);

            DrvIC.Move(ch, "AF", 2048); Wait(50);
            AddLog(ch, $"PM AF Code, Target {DrvIC.ReadHall(ch, "AF")}");
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x02, 1, new byte[] { 0x40 });
            Wait(1);
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x3B });

            AddLog(ch, "Phase margin test start");
            DrvIC.FRAModeEnable(ch);

            DrvIC.Set_Amp(ch, (int)Condition.iAFAmplitude);
            AddLog(ch, $"Amp\tFreq\tGain\tP/M");
            for (oldfreq = freqval = Condition.iAFChirpFrom; freqval >= Condition.iAFChirpTo; freqval -= freqtemp)
            {
                DrvIC.Set_Freq(ch, freqval);
                Wait(1000 / oldfreq + 5000 / freqval + 15);
                oldfreq = freqval;
                gainval = DrvIC.Get_Gain(ch);
                pmval = DrvIC.Get_Phase(ch, 1);
                AddLog(ch, $"{Condition.AFGMamp}\t{freqval}\t{gainval.ToString("F2")}\t{pmval.ToString("F0")}");


                if (!PhaseFouund && gainval > 0)
                {
                    respm = ((gainval * prepm) - (before_after_zero_gain[0] * pmval)) / (gainval - before_after_zero_gain[0]);
                    resFreq = (int)(((gainval * before_after_zero_freq[0]) - (before_after_zero_gain[0] * freqpm)) / (gainval - before_after_zero_gain[0]));
                    before_after_zero_freq[1] = freqval;
                    before_after_zero_gain[1] = gainval;
                    PhaseFouund = true;
                    if (dB4PhaseFouund)
                        break;
                }
                if (!dB4PhaseFouund && gainval >= -4 && before_after_zero_gain[0] <= -4)
                {
                    //pm1 + (targetGain - gain1) * (pm2 - pm1) / (gain2 - gain1);
                    res4dbpm = prepm + ((-4) - before_after_zero_gain[0]) * (pmval - prepm) / (gainval - before_after_zero_gain[0]);
                    //  res4dbpm = ((gainval * prepm) - (before_after_zero_gain[0] * pmval)) / (gainval - before_after_zero_gain[0]);
                    dB4PhaseFouund = true;
                    if (PhaseFouund) break;
                }
                else
                {
                    before_after_zero_freq[0] = freqval;
                    before_after_zero_gain[0] = gainval;
                }
                prepm = pmval;
                freqtemp = freqval * Condition.iAFFRAstep / 100;

                if (freqtemp < 1) freqtemp = 1;
            }
            AddLog(ch, $"Zero Freq before = {before_after_zero_freq[0]}Hz,{before_after_zero_gain[0].ToString("F2")}dB");
            AddLog(ch, $"Zero Freq after = {before_after_zero_freq[1]}Hz,{before_after_zero_gain[1].ToString("F2")}dB");

            if (freqval == Condition.iAFChirpFrom)
            {

                AddLog(ch, " Error type1 : Gain over zero at 1st cycle");
                DrvIC.FRAModeDisable(ch);
                resFreq = freqval;
                respm = 1;
            }
            if ((freqval <= Condition.iAFChirpTo) && (gainval <= 0))
            {

                if (gainval > -2)
                {
                    freqpm = before_after_zero_freq[0];
                    gainval = before_after_zero_gain[0];
                }
                else
                {
                    AddLog(ch, " Error type4 : No cross over point during period\n");
                    DrvIC.FRAModeDisable(ch);
                    resFreq = freqval;
                    respm = 4;                                                //result=4;
                }

                AddLog(ch, " Error type4 : No cross over point during period\n");
                resFreq = freqval;
                respm = 4;

            }
            if (Math.Abs(gainval - before_after_zero_gain[1]) > Condition.PMAFGainTH)
            {
                AddLog(ch, $"Error type 2: gain is changed drastically over {Condition.PMAFGainTH}");
                //---------------------------------------------------------
                // disable
                DrvIC.FRAModeDisable(ch);
                //---------------------------------------------------------
                Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x00 });
                DrvIC.AK7314_IC_reset(ch);
                resFreq = freqval;
                respm = 2;
            }

            AddLog(ch, "Use Linear Interpolation");
            AddLog(ch, $"{Condition.iAFAmplitude}, {resFreq}Hz, {gainval.ToString("F2")}dB, {respm.ToString("F0")}deg");
            AddLog(ch, $"-4dB Phase Margin = {res4dbpm.ToString("F0")}");

            DrvIC.FRAModeDisable(ch);
            DrvIC.AK7314_IC_reset(ch);
            //   PassFails[ch].Results[(int)SpecItem.FRAAF_PMFreq].Val = resFreq;
            PassFails[ch].Results[(int)SpecItem.FRAAF_PhaseMargin].Val = respm;
            PassFails[ch].Results[(int)SpecItem.FRAAF_4dB_PhaseMargin].Val = res4dbpm;

            ShowDataResults(ch, (int)SpecItem.FRAAF_PhaseMargin, (int)SpecItem.FRAAF_4dB_PhaseMargin, InspType.Normal, new double[] { });

            //int freq_val, freq_temp = 0, gain_temp, freq_PM = 0, old_freq;
            //int[] before_after_zero_freq = new int[2];
            //double gain_val = 0, phasemargin_val = 0, phase_temp, pre_pm = 0;
            //double[] before_after_zero_gain = new double[2];
            //double phase_min = 5000;
            //double PM_2nd = 0;
            //byte backup, flag_2nd = 0;
            //byte freq_en;

            //byte[] rbuf3 = new byte[3];

            //int zero_range = 3;
            //int StartFreq = Condition.iAFChirpFrom;
            //int EndFreq = Condition.iAFChirpTo;
            //int Step = Condition.iAFFRAstep;
            //double amp = Condition.iAFAmplitude;
            //int GainTh = Condition.PMAFGainTH;


            //Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x00);
            //DrvIC.AFMove(ch, Condition.AfPosPM); // 가이드 받으면 적용 
            //Wait(100);

            //Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x00, 1, 0x01);
            //Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x00, 1, 0x00);
            //Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x6F, 1, (byte)(DrvIC.AF_Addr << 1));

            //AddLog(ch, $"[AF Phase Margin test(High Freq Start)]");
            //Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x40);
            //Wait(1);
            //Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);

            //DrvIC.FRAModeEnable(ch);

            //Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x52, 1, (ushort)((int)amp << 6));

            //AddLog(ch, "--------------------------------------------");
            //AddLog(ch, " Amp	Freq	Gain	P/M ");
            //AddLog(ch, " [Dec]	[Hz]	[dB]	[deg] ");
            //AddLog(ch, "--------------------------------------------");

            //for (old_freq = freq_val = StartFreq; freq_val >= EndFreq; freq_val -= freq_temp)
            //{
            //    Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x50, 1, (ushort)(freq_val << 1));
            //    Wait((int)(1000.0 / old_freq + 5000.0 / freq_val + 10));
            //    old_freq = freq_val;

            //    Dln.ReadArray(ch, DrvIC.FRA_Addr, 0x94, 1, rbuf3);
            //    gain_temp = (rbuf3[0] << 16) + (rbuf3[1] << 8) + rbuf3[2];
            //    gain_val = Math.Log10(((double)gain_temp / 65536)) * 20;

            //    phase_temp = (double)Dln.Read2Byte(ch, DrvIC.FRA_Addr, 0x98, 1);
            //    phase_temp /= 128;
            //    if (phase_temp > 256) phase_temp -= 512;
            //    phasemargin_val = 180 + phase_temp;
            //    if (phasemargin_val > 180) phasemargin_val -= 360;
            //    if (phasemargin_val < -180) phasemargin_val += 360;

            //    AddLog(ch, $"{amp}, {freq_val}, {gain_val.ToString("F2")}, {phasemargin_val.ToString("F2")}");
            //    if (phasemargin_val < phase_min) phase_min = phasemargin_val;

            //    if(gain_val > 0)
            //    {
            //        phasemargin_val = ((gain_val * pre_pm) - (before_after_zero_gain[0] * phasemargin_val)) /
            //           (gain_val - before_after_zero_gain[0]);
            //        freq_PM = (int)(((gain_val * before_after_zero_freq[0]) - (before_after_zero_gain[0] *
            //                         freq_val)) / (gain_val - before_after_zero_gain[0]));

            //        before_after_zero_freq[1] = freq_val;
            //        before_after_zero_gain[1] = gain_val;
            //        break;
            //    }
            //    else
            //    {
            //        before_after_zero_freq[0] = freq_val;
            //        before_after_zero_gain[0] = gain_val;
            //    }

            //    if((Math.Abs(gain_val) < zero_range) && (flag_2nd !=1))
            //    {
            //        PM_2nd = phasemargin_val;
            //        flag_2nd = 1;
            //    }
            //    pre_pm = phasemargin_val;
            //    freq_temp = freq_val * Step / 100;

            //    if (freq_temp < 1) freq_temp = 1;
            //}

            //AddLog(ch, "--------------------------------------------");
            //AddLog(ch, $"Zero Freq Before = {before_after_zero_freq[0]} Hz,  {before_after_zero_gain[0].ToString("F2")} dB");
            //AddLog(ch, $"Zero Freq After  = {before_after_zero_freq[1]} Hz,  {before_after_zero_gain[1].ToString("F2")} dB");

            //if(freq_val == StartFreq)
            //{
            //    AddLog(ch, $"Error type1 : Gain over zero at 1st cycle");
            //    PassFails[ch].Results[(int)SpecItem.FRAAF_PhaseMargin].Val = 1; //plus Gain
            //    ShowDataResults(ch, (int)SpecItem.FRAAF_PhaseMargin, (int)SpecItem.FRAAF_PhaseMargin, InspType.Normal, new double[] { });
            //    DrvIC.FRAModeDisable(ch);
            //    return;
            //}
            //if((freq_val <= EndFreq) && (gain_val <= 0))
            //{
            //    AddLog(ch, $"Error type4 : No cross over point during period");
            //    PassFails[ch].Results[(int)SpecItem.FRAAF_PhaseMargin].Val = 4; //No cross
            //    ShowDataResults(ch, (int)SpecItem.FRAAF_PhaseMargin, (int)SpecItem.FRAAF_PhaseMargin, InspType.Normal, new double[] { });
            //    DrvIC.FRAModeDisable(ch);
            //    return;
            //}
            //if(Math.Abs(gain_val - before_after_zero_gain[1]) > GainTh)
            //{
            //    AddLog(ch, $"Error type2 : gain is changed drastically over {GainTh}");
            //    PassFails[ch].Results[(int)SpecItem.FRAAF_PhaseMargin].Val = 2; //No cross
            //    ShowDataResults(ch, (int)SpecItem.FRAAF_PhaseMargin, (int)SpecItem.FRAAF_PhaseMargin, InspType.Normal, new double[] { });
            //    DrvIC.FRAModeDisable(ch);
            //    return;

            //}
            //AddLog(ch, "\nUse Linear Interpolation");
            //AddLog(ch, "--------------------------------------------------");
            //AddLog(ch, $" {amp} amp, {freq_PM} Hz, {gain_val.ToString("F2")} dB, {phasemargin_val.ToString("F0")} deg");
            //AddLog(ch, "--------------------------------------------------");
            //AddLog(ch, $"Phase at -3dB : {PM_2nd.ToString("F0")} deg");

            //DrvIC.FRAModeDisable(ch);
            //Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            //DrvIC.AF_ICReset(ch);

            //PassFails[ch].Results[(int)SpecItem.FRAAF_PhaseMargin].Val = phasemargin_val;
            //ShowDataResults(ch, (int)SpecItem.FRAAF_PhaseMargin, (int)SpecItem.FRAAF_PhaseMargin, InspType.Normal, new double[] { });
        }
        void AFGM(int ch, string testItem, int inspCnt)
        {
            double res = 0;
            byte scancnt = 0;
            int freqval, freqtemp = 0, gaintemp, oldfreq;
            int[] before_after_zero_freq = new int[2];
            double[] before_after_zero_phase = new double[2];
            int[] freq_PM, freq_GM = new int[2];
            double[] gainval = new double[2] { 0, 0 };
            double[] pmval = new double[2];
            double gmval, phasetemp, prepm = 0;
            double[] pregm = new double[2] { 0, 0 };

            DrvIC.SetSlaveAddr(ch, DrvIC.FRA_AFSlaveAddr);
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x02, 1, new byte[] { 0x00 });
            Wait(50);

            DrvIC.Move(ch, "AF", 2048); Wait(50);
            AddLog(ch, $"GM AF Code, Target {DrvIC.ReadHall(ch, "AF")}");
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0x02, 1, new byte[] { 0x40 });
            Wait(50);
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x3B });

            AddLog(ch, "GainMargin test start");
            DrvIC.FRAModeEnable(ch);

            DrvIC.Set_Amp(ch, Condition.AFGMamp);
            AddLog(ch, $"SCnt\tAmp\tFreq\tGain\tP/M");
            for (oldfreq = freqval = Condition.AFGMEndFreq; freqval <= Condition.AFGMStartFreq; freqval += freqtemp)
            {
                DrvIC.Set_Freq(ch, freqval);
                Wait(1000 / oldfreq + 5000 / freqval + 10);
                oldfreq = freqval;
                gainval[scancnt] = DrvIC.Get_Gain(ch);
                pmval[scancnt] = DrvIC.Get_Phase(ch, 1);
                AddLog(ch, $"{scancnt + 1} \t {Condition.AFGMamp}\t{freqval}\t{gainval[scancnt].ToString("F2")}\t{pmval[scancnt].ToString("F0")}");
                if (pmval[scancnt] < 0)
                {
                    gainval[scancnt] = ((pmval[scancnt] * pregm[scancnt]) - (before_after_zero_phase[scancnt] * gainval[scancnt])) / (pmval[scancnt] - before_after_zero_phase[scancnt]);
                    freq_GM[scancnt] = (int)(((pmval[scancnt] * before_after_zero_freq[scancnt]) - (before_after_zero_phase[scancnt] * freqval)) / (pmval[scancnt] - before_after_zero_phase[scancnt]));

                    scancnt++;
                    if (scancnt == 2)
                    {
                        break;
                    }

                }
                else
                {
                    before_after_zero_freq[scancnt] = freqval;
                    before_after_zero_phase[scancnt] = pmval[scancnt];
                }
                pregm[scancnt] = gainval[scancnt];
                freqtemp = freqval * Condition.AFGMStep / 100;
                if (freqtemp < 1) freqtemp = 1;
            }
            if (freqval == Condition.AFGMStartFreq && scancnt == 0)
            {
                AddLog(ch, "Error type 1 : Gain over zero at 1st Scan");
                DrvIC.FRAModeDisable(ch);
                res = 1;
            }
            AddLog(ch, "\r\nUse Linear Interpolation");
            AddLog(ch, $"{1} \t {Condition.AFGMamp}\t{freq_GM[0]}\t{gainval[0].ToString("F2")}\t{pmval[0].ToString("F0")}");
            DrvIC.FRAModeDisable(ch);
            Dln.WriteArray(ch, DrvIC.AFSlaveAddr, 0xAE, 1, new byte[] { 0x00 });
            DrvIC.AK7314_IC_reset(ch);

            PassFails[ch].Results[(int)SpecItem.FRAAF_GainMargin].Val = Math.Abs(gainval[0]);
            ShowDataResults(ch, (int)SpecItem.FRAAF_GainMargin, (int)SpecItem.FRAAF_GainMargin, InspType.Normal, new double[] { });





            //int freq_val, freq_temp = 0, gain_temp, freq_GM = 0, old_freq;
            //int[] before_after_zero_freq = new int[2];
            //double gain_val = 0, phasemargin_val = 0, gainmargin_val = 0, phase_temp, pre_gm = 0;
            //double[] before_after_zero_phase = new double[2];

            //byte backup;
            //float result;
            //bool test_continue = true;

            //byte[] rbuf3 = new byte[3];


            //int StartFreq = Condition.AFGMStartFreq;
            //int EndFreq = Condition.AFGMEndFreq;
            //int Step = Condition.AFGMStep;
            //double amp = Condition.AFGMamp;
            //int GainTh = Condition.PMAFGainTH;


            //Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x00);
            //DrvIC.AFMove(ch, Condition.AFPosGM); // 가이드 받으면 적용 
            //Wait(100);

            //Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x00, 1, 0x01);
            //Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x00, 1, 0x00);
            //Dln.WriteByte(ch, DrvIC.FRA_Addr, 0x6F, 1, (byte)(DrvIC.AF_Addr << 1));

            //AddLog(ch, $"[AF Gain Margin test]");
            //Dln.WriteByte(ch, DrvIC.AF_Addr, 0x02, 1, 0x40);
            //Wait(1);
            //Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x3B);

            //DrvIC.FRAModeEnable(ch);

            //Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x52, 1, (ushort)((int)amp << 6));


            //freq_val = StartFreq;
            //freq_temp = freq_val * Step / 100;
            //freq_val += freq_temp;
            //Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x50, 1, (ushort)(freq_val << 1));
            //Wait(30000 / freq_val + 10);
            //Wait(100);
            //Dln.ReadArray(ch, DrvIC.FRA_Addr, 0x94, 1, rbuf3);
            //gain_temp = (rbuf3[0] << 16) + (rbuf3[1] << 8) + rbuf3[2];
            //gain_val = Math.Log10(((double)gain_temp / 65536)) * 20;


            //phase_temp = (double)Dln.Read2Byte(ch, DrvIC.FRA_Addr, 0x98, 1);
            //phase_temp /= 128;
            //if (phase_temp > 256) phase_temp -= 512;
            //phasemargin_val = 180 + phase_temp;
            //if (phasemargin_val > 180) phasemargin_val -= 360;
            //if (phasemargin_val < -180) phasemargin_val += 360;

            //AddLog(ch, "skip high freq for aging\n");
            //AddLog(ch, $"{amp},{freq_val} Hz, {gain_val.ToString("F2")} dB, {phasemargin_val.ToString("F0")} deg\n\n");


            //AddLog(ch, "--------------------------------------------");
            //AddLog(ch, " Amp	Freq	Gain	P/M ");
            //AddLog(ch, " [Dec]	[Hz]	[dB]	[deg] ");
            //AddLog(ch, "--------------------------------------------");

            //for (old_freq = freq_val = StartFreq; freq_val >= EndFreq; freq_val -= freq_temp)
            //{
            //    Dln.Write2Byte(ch, DrvIC.FRA_Addr, 0x50, 1, (ushort)(freq_val << 1));
            //    Wait((int)(1000.0 / old_freq + 5000.0 / freq_val + 10));
            //    old_freq = freq_val;

            //    Dln.ReadArray(ch, DrvIC.FRA_Addr, 0x94, 1, rbuf3);
            //    gain_temp = (rbuf3[0] << 16) + (rbuf3[1] << 8) + rbuf3[2];
            //    gain_val = Math.Log10(((double)gain_temp / 65536)) * 20;

            //    phase_temp = (double)Dln.Read2Byte(ch, DrvIC.FRA_Addr, 0x98, 1);
            //    phase_temp /= 128;
            //    if (phase_temp > 256) phase_temp -= 512;
            //    phasemargin_val = 180 + phase_temp;
            //    if (phasemargin_val > 180) phasemargin_val -= 360;
            //    if (phasemargin_val < -180) phasemargin_val += 360;

            //    AddLog(ch, $"{amp}, {freq_val}, {gain_val.ToString("F2")}, {phasemargin_val.ToString("F2")}");

            //    if (phasemargin_val > 0)
            //    {
            //        gain_val = ((phasemargin_val * pre_gm) - (before_after_zero_phase[0] * gain_val))  / (phasemargin_val - before_after_zero_phase[0]);
            //        freq_GM = (int)(((phasemargin_val * before_after_zero_freq[0]) - (before_after_zero_phase[0] *
            //                         freq_val)) / (phasemargin_val - before_after_zero_phase[0]));

            //        before_after_zero_freq[1] = freq_val;
            //        before_after_zero_phase[1] = phasemargin_val;
            //        break;
            //    }
            //    else
            //    {
            //        before_after_zero_freq[0] = freq_val;
            //        before_after_zero_phase[0] = phasemargin_val;
            //    }


            //    pre_gm = gain_val;
            //    freq_temp = freq_val * Step / 100;

            //    if (freq_temp < 1) freq_temp = 1;
            //}

            //AddLog(ch, "--------------------------------------------");
            //AddLog(ch, $"Zero Freq Before = {before_after_zero_freq[0]} Hz,  {before_after_zero_phase[0].ToString("F2")} dB");
            //AddLog(ch, $"Zero Freq After  = {before_after_zero_freq[1]} Hz,  {before_after_zero_phase[1].ToString("F2")} dB");

            //if (test_continue && (freq_val == StartFreq))
            //{
            //    AddLog(ch, $"Error type1 : Phase over zero at 1st cycle");
            //    PassFails[ch].Results[(int)SpecItem.FRAAF_GainMargin].Val = 1; //plus Gain
            //    ShowDataResults(ch, (int)SpecItem.FRAAF_GainMargin, (int)SpecItem.FRAAF_GainMargin, InspType.Normal, new double[] { });
            //    DrvIC.FRAModeDisable(ch);
            //    test_continue = false;
            //    return;
            //}
            //if(test_continue && (freq_val <= EndFreq) && (phasemargin_val <= 0))
            //{
            //    AddLog(ch, $"Error type4 : No cross over point during period");
            //    PassFails[ch].Results[(int)SpecItem.FRAAF_GainMargin].Val = 4; //plus Gain
            //    ShowDataResults(ch, (int)SpecItem.FRAAF_GainMargin, (int)SpecItem.FRAAF_GainMargin, InspType.Normal, new double[] { });
            //    DrvIC.FRAModeDisable(ch);
            //    test_continue = false;
            //    return;
            //}

            //AddLog(ch, "\nUse Linear Interpolation");
            //AddLog(ch, "--------------------------------------------------");
            //AddLog(ch, $" {amp} amp, {freq_GM} Hz, {gain_val.ToString("F2")} dB, {phasemargin_val.ToString("F0")} deg");
            //AddLog(ch, "--------------------------------------------------");

            //PassFails[ch].Results[(int)SpecItem.FRAAF_GainMargin].Val = (float)(-1 * gain_val);
            //ShowDataResults(ch, (int)SpecItem.FRAAF_GainMargin, (int)SpecItem.FRAAF_GainMargin, InspType.Normal, new double[] { });

            //DrvIC.FRAModeDisable(ch);
            //Dln.WriteByte(ch, DrvIC.AF_Addr, 0xAE, 1, 0x00);
            //DrvIC.AF_ICReset(ch);



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
        private void WriteOISUserMem(int ch, int res)
        {
            NVMWriteCollection readCollection = new NVMWriteCollection();
            ReadWirteNVM NVMReadWriter = new ReadWirteNVM(DWDrvIC.Controls,AddLog);
            var testvalue = PassFails[ch].Results[(int)SpecItem.OISX_Ratedstroke].Val;
            writeNVMParamX.AddRow(0xE0, res);
            
            var strokeX = Convert.ToInt32(Math.Round(PassFails[ch].Results[(int)SpecItem.OISX_Ratedstroke].Val / 4));
            if (strokeX < int.MaxValue)
                writeNVMParamX.AddRow(0xE1, strokeX);
            else
                writeNVMParamX.AddRow(0xE1, 0);

            var HysteresisX = Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.OISX_Hysteresis].Val * 10);

            if (HysteresisX < int.MaxValue)
                writeNVMParamX.AddRow(0xE2, HysteresisX);
            else
                writeNVMParamX.AddRow(0xE2, 0);

            var LinearityX = Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.OISX_Linearity].Val * 10);
            if (LinearityX < int.MaxValue)
                writeNVMParamX.AddRow(0xE3, LinearityX);
            else
                writeNVMParamX.AddRow(0xE3, 0);

            writeNVMParamX.AddRow(0xE4, 0x01);
            writeNVMParamX.AddRow(0xE5, (Condition.OISCalAFPos >> 4));
            writeNVMParamX.AddRow(0xE6, 0);
            writeNVMParamX.AddRow(0xE7, 0);
            writeNVMParamX.AddRow(0xE8, 0);
            writeNVMParamX.AddRow(0xE9, 0);
            writeNVMParamX.AddRow(0xEA, 0);
            writeNVMParamX.AddRow(0xEB, 0);
            writeNVMParamX.AddRow(0xEC, 0);
            writeNVMParamX.AddRow(0xED, 0);
            writeNVMParamX.AddRow(0xEE, 0);
            writeNVMParamX.AddRow(0xEF, 0);
            writeNVMParamX.AddRow(0xF0, 0);
            writeNVMParamX.AddRow(0xF1, 0);
            writeNVMParamX.AddRow(0xF2, 0);
            writeNVMParamX.AddRow(0xF3, 0);
            writeNVMParamX.AddRow(0xF4, 0);
            writeNVMParamX.AddRow(0xF5, 0);
            writeNVMParamX.AddRow(0xF6, 0);
            writeNVMParamX.AddRow(0xF7, 0);

            if (PassFails[ch].Results[(int)SpecItem.FRAX_PhaseMarginLow].Val < int.MaxValue)
                writeNVMParamX.AddRow(0xF8, Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.FRAX_PhaseMarginLow].Val));
            else
                writeNVMParamX.AddRow(0xF8, 0);

            if (PassFails[ch].Results[(int)SpecItem.FRAX_PhaseMargin].Val < int.MaxValue)
                writeNVMParamX.AddRow(0xF9, Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.FRAX_PhaseMargin].Val));
            else
                writeNVMParamX.AddRow(0xF9, 0);


                writeNVMParamX.AddRow(0xFA, 0);

            if (PassFails[ch].Results[(int)SpecItem.FRAX_Ringing].Val < int.MaxValue)
                writeNVMParamX.AddRow(0xFB, Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.FRAX_Ringing].Val));
            else
                writeNVMParamX.AddRow(0xFB, 0);

            if (PassFails[ch].Results[(int)SpecItem.FRAX_SineWave].Val < int.MaxValue)
                writeNVMParamX.AddRow(0xFC, Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.FRAX_SineWave].Val));
            else
                writeNVMParamX.AddRow(0xFC, 0);

            writeNVMParamX.AddRow(0xFD, 0);                                                                                     //퓨런티어??
            writeNVMParamX.AddRow(0xFE, 0);                                                                                     //OIS PID 버전???
            //TODO : FF 33 못날림??
            //writeNVMParamX.AddRow(0xFF, 0x33);        //?? MX 배포기준?? 이해못함.

            AddLog(ch, "OIS X Nvm Data Check");

            DWDrvIC.SetOperationMode(AxisTypeDW.AxisX, OperationTypeDW.StandbyMode);
            Thread.Sleep(10);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x28, 1, 0x39); 

            NVMReadWriter.SetWrite(ch,DWDrvIC.OISX_Addr, writeNVMParamX);

            DWDrvIC.SetOperationMode(AxisTypeDW.AxisX, OperationTypeDW.StandbyMode);
            Thread.Sleep(10);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x03, 1, 0x03);               //STORE?
            Thread.Sleep(10);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x28, 1, 0x14);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISX_Addr, 0x04, 1, 0x01);               //Reset??

            readCollection.CopyAddress(writeNVMParamX);
            NVMReadWriter.GetReadAddress(ch, DWDrvIC.OISX_Addr, readCollection);

            bool verifyFlg = true;
            verifyFlg  &= NVMReadWriter.CompareData(ch, writeNVMParamX, readCollection);

            var strokeY = Convert.ToInt32(Math.Round(PassFails[ch].Results[(int)SpecItem.OISY_Ratedstroke].Val / 4));
            if (strokeY < int.MaxValue)
                writeNVMParamY.AddRow(0xE1, strokeY);
            else
                writeNVMParamY.AddRow(0xE1, 0);

            var HisteresisY = Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.OISY_Hysteresis].Val * 10);
            if (HisteresisY < int.MaxValue)
                writeNVMParamY.AddRow(0xE2, HisteresisY);
            else
                writeNVMParamY.AddRow(0xE2, 0);

            var LinearityY = Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.OISY_Linearity].Val * 10);
            if (LinearityY < int.MaxValue)
                writeNVMParamY.AddRow(0xE3, LinearityY);
            else
                writeNVMParamY.AddRow(0xE3, 0);

            writeNVMParamY.AddRow(0xE4, 0x01);
            writeNVMParamY.AddRow(0xE5, (Condition.OISCalAFPos >> 4));
            writeNVMParamY.AddRow(0xE6, 0);
            writeNVMParamY.AddRow(0xE7, 0);
            writeNVMParamY.AddRow(0xE8, 0);
            writeNVMParamY.AddRow(0xE9, 0);
            writeNVMParamY.AddRow(0xEA, 0);
            writeNVMParamY.AddRow(0xEB, 0);
            writeNVMParamY.AddRow(0xEC, 0);
            writeNVMParamY.AddRow(0xED, 0);
            writeNVMParamY.AddRow(0xEE, 0);
            writeNVMParamY.AddRow(0xEF, 0);
            writeNVMParamY.AddRow(0xF0, 0);
            writeNVMParamY.AddRow(0xF1, 0);
            writeNVMParamY.AddRow(0xF2, 0);
            writeNVMParamY.AddRow(0xF3, 0);
            writeNVMParamY.AddRow(0xF4, 0);
            writeNVMParamY.AddRow(0xF5, 0);
            writeNVMParamY.AddRow(0xF6, 0);
            writeNVMParamY.AddRow(0xF7, 0);
            if (PassFails[ch].Results[(int)SpecItem.FRAY_PhaseMarginLow].Val < int.MaxValue)
                writeNVMParamY.AddRow(0xF8, Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.FRAY_PhaseMarginLow].Val));
            else
                writeNVMParamY.AddRow(0xF8, 0);

            if (PassFails[ch].Results[(int)SpecItem.FRAY_PhaseMargin].Val < int.MaxValue)
                writeNVMParamY.AddRow(0xF9, Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.FRAY_PhaseMargin].Val));
            else
                writeNVMParamY.AddRow(0xF9, 0);


                writeNVMParamY.AddRow(0xFA, 0);

            if (PassFails[ch].Results[(int)SpecItem.FRAY_Ringing].Val < int.MaxValue)
                writeNVMParamY.AddRow(0xFB, Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.FRAY_Ringing].Val));
            else
                writeNVMParamY.AddRow(0xFB, 0);

            if (PassFails[ch].Results[(int)SpecItem.FRAY_SineWave].Val < int.MaxValue)
                writeNVMParamY.AddRow(0xFC, Convert.ToInt32(PassFails[ch].Results[(int)SpecItem.FRAY_SineWave].Val));
            else
                writeNVMParamY.AddRow(0xFC, 0);

            writeNVMParamY.AddRow(0xFD, 0);         //퓨런티어??
            writeNVMParamY.AddRow(0xFE, 0);         //OIS PID 버전???
            //writeNVMParamY.AddRow(0xFF, 0);         //?? AF FD Position Repeat Test flg????

            DWDrvIC.SetOperationMode(AxisTypeDW.AxisY, OperationTypeDW.StandbyMode);
            Thread.Sleep(10);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x28, 1, 0x39);

            AddLog(ch, "OIS Y Nvm Data Check");
            NVMReadWriter.SetWrite(ch, DWDrvIC.OISY_Addr, writeNVMParamY);

            DWDrvIC.SetOperationMode(AxisTypeDW.AxisY, OperationTypeDW.StandbyMode);
            Thread.Sleep(10);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x03, 1, 0x03);               //STORE?
            Thread.Sleep(10);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x28, 1, 0x14);
            DWDrvIC.Controls.WriteByte(DWDrvIC.OISY_Addr, 0x04, 1, 0x01);               //Reset??

            readCollection.CopyAddress(writeNVMParamY);
            NVMReadWriter.GetReadAddress(ch, DWDrvIC.OISY_Addr, readCollection);
            verifyFlg &= NVMReadWriter.CompareData(ch, writeNVMParamY, readCollection);

            if (!verifyFlg)
            {
                if (PassFails[ch].FirstFailIndex == 0)
                {
                    AddLog(ch, "OIS NVM Verify NG");
                    PassFails[ch].Results[(int)SpecItem.OISPIDVerifyRes].Val = 1;
                    ShowDataResults(ch, (int)SpecItem.OISPIDVerifyRes, (int)SpecItem.OISPIDVerifyRes, InspType.Normal, new double[] { });
                }
            }
            else
            {
                if (PassFails[ch].FirstFailIndex == 0)
                {
                    AddLog(ch, "OIS NVM Verify OK");
                    PassFails[ch].Results[(int)SpecItem.OISPIDVerifyRes].Val = 0;
                    ShowDataResults(ch, (int)SpecItem.OISPIDVerifyRes, (int)SpecItem.OISPIDVerifyRes, InspType.Normal, new double[] { });
                }
            }
        }

        private void OISSineWave(int ch, string testItem, int inspCnt)
        {
            byte[] u08_dat1 = new byte[1] { 0x00 };
            byte[] u08_dat2 = new byte[1] { 0x00 };
            var _Measurement = new Echo_AMA_Measurement(DWDrvIC.Controls,DWDrvIC,AddLog);

            if (!DWDrvIC.Echo_Board_WhoAmI(ch))
            {
                AddLog(ch, "Echo_FRA_Measurement] FRA Board Info Error!!!");
            }

            AMA_TestSetting_Params param = new AMA_TestSetting_Params()
            {
                Target_slave_id_X = (Condition.Slave_ID_X == 0) ? -1 : Condition.Slave_ID_X,
                Target_slave_id_Y = (Condition.Slave_ID_Y == 0) ? -1 : Condition.Slave_ID_Y,
                Target_slave_id_Z = (Condition.Slave_ID_Z == 0) ? -1 : Condition.Slave_ID_Z,
                Clock_devision =  (Condition.Clock_Devision == 0) ? -1 : Condition.Clock_Devision,
                EOIS_target_device_number =  (Condition.eOIS_Device_Number == 0) ? -1 : Condition.eOIS_Device_Number,
                Af_target_device_number = (Condition.AF_Target_Device_Number == 0) ? -1 : Condition.AF_Target_Device_Number,
                Set_read_address = (Condition.Set_Read_Address == 0) ? -1 : Condition.Set_Read_Address,
                Read_address_count = (Condition.Read_Address_Count == 0) ? -1 : Condition.Read_Address_Count,
                Frequency = (Condition.Frequency == 0) ? -1 : Condition.Frequency,
                Amplitude = (Condition.Amplitude == 0) ? -1 : Condition.Amplitude,
                Threshold = (Condition.Threshold == 0) ? -1 : Condition.Threshold,
                Measurement_cycle_count = (Condition.MeasurementCycleCount == 0) ? -1 : Condition.MeasurementCycleCount,
                Dummy_cycle_count = (Condition.DummyCycleCount == 0) ? -1 : Condition.DummyCycleCount
            };

            var result = _Measurement.SineWaveMeasurement(ch, param);

            AddLog(ch, $"Sine Wave Measurement Complete :" +
                $" DeltaX[{result.DeltaMaxX.ToString()}]," +
                $" DeltaY[{result.DeltaMaxY.ToString()}]," +
                $" NG Count X : {result.NgCountX.ToString()}," +
                $" NG Count Y : {result.NgCountY.ToString()}");


            PassFails[0].Results[(int)SpecItem.FRAX_SineWave].Val = result.DeltaMaxX;
            PassFails[0].Results[(int)SpecItem.FRAY_SineWave].Val = result.DeltaMaxY;
            ShowDataResults(ch, (int)SpecItem.FRAX_SineWave, (int)SpecItem.FRAX_SineWave, InspType.Normal, new double[] { });
            ShowDataResults(ch, (int)SpecItem.FRAY_SineWave, (int)SpecItem.FRAY_SineWave, InspType.Normal, new double[] { });
        }
        private void OISRinging(int ch, string testItem, int inspCnt)
        {
            byte[] u08_dat1 = new byte[1] { 0x00 };
            byte[] u08_dat2 = new byte[1] { 0x00 };
            var _Measurement = new Echo_AMA_Measurement(DWDrvIC.Controls, DWDrvIC, AddLog);

            if (!DWDrvIC.Echo_Board_WhoAmI(ch))
            {
                AddLog(ch, "Echo_FRA_Measurement] FRA Board Info Error!!!");
            }

            AMA_RingingSetting_Params param = new AMA_RingingSetting_Params()
            {
                Target_slave_id_X = (Condition.Slave_ID_X == 0) ? -1 : Condition.Slave_ID_X,
                Target_slave_id_Y = (Condition.Slave_ID_Y == 0) ? -1 : Condition.Slave_ID_Y,
                Target_slave_id_Z = (Condition.Slave_ID_Z == 0) ? -1 : Condition.Slave_ID_Z,
                Clock_devision = (Condition.Clock_Devision == 0) ? -1 : Condition.Clock_Devision,
                EOIS_target_device_number = (Condition.eOIS_Device_Number == 0) ? -1 : Condition.eOIS_Device_Number,
                Af_target_device_number = (Condition.AF_Target_Device_Number == 0) ? -1 : Condition.AF_Target_Device_Number,
                End_positionX = (Condition.Ringing_End_Position == 0) ? -1 : Condition.Ringing_End_Position,
                Start_positionX = (Condition.Ringing_Start_Position == 0) ? -1 : Condition.Ringing_Start_Position,
                End_positionY = (Condition.Ringing_End_PositionY == 0) ? -1 : Condition.Ringing_End_PositionY,
                Start_positionY = (Condition.Ringing_Start_PositionY == 0) ? -1 : Condition.Ringing_Start_PositionY,
                Start_time = (Condition.Ringing_Start_Time == 0) ? -1 : Condition.Ringing_Start_Time,
                End_time = (Condition.Ringing_End_Time == 0) ? -1 : Condition.Ringing_End_Time,
                Threshold = (Condition.Ringing_Threshold == 0) ? -1 : Condition.Ringing_Threshold,
            };

            var result = _Measurement.RingingMeasurement(ch, param);

            AddLog(ch, $"Sine Wave Measurement Complete :" +
                $" DeltaX[{result.OkCountX.ToString()}]," +
                $" DeltaY[{result.OkCountY.ToString()}]," +
                $" SettlingTimeX[{result.SettlingTimeX.ToString()}]," +
                $" SettlingTimeY[{result.SettlingTimeY.ToString()}],");


            PassFails[0].Results[(int)SpecItem.FRAX_Ringing].Val = result.SettlingTimeX;
            PassFails[0].Results[(int)SpecItem.FRAY_Ringing].Val = result.SettlingTimeY;
            ShowDataResults(ch, (int)SpecItem.FRAX_Ringing, (int)SpecItem.FRAX_Ringing, InspType.Normal, new double[] { });
            ShowDataResults(ch, (int)SpecItem.FRAY_Ringing, (int)SpecItem.FRAY_Ringing, InspType.Normal, new double[] { });
        }

        private bool NDataPointChecked()
        {
            if (Condition.OISLincompStep == 32 ||
                Condition.OISLincompStep == 16 ||
                Condition.OISLincompStep == 8 ||
                Condition.OISLincompStep == 4 )
            {
                return true;
            }

            return false;
        }
        private void OISLCCComp(int ch, string testItem, int inspCnt)
        {
            if(!NDataPointChecked())
            {
                AddLog(ch, "Invalid OIS Linear Compensation Step");
                return;
            }

            DWDrvIC.SetOperationMode(AxisTypeDW.AxisX, OperationTypeDW.StandbyMode);
            DWDrvIC.SetOperationMode(AxisTypeDW.AxisX, OperationTypeDW.OpenMode);
            DWDrvIC.OISMove(ch, 8191, 8191);      //current 0mA Position

            for (int i = 0; i < 30; i++)
            {
                DWDrvIC.OISMove(ch, DWDrvIC.OIS_MIN_CODE, 8191);
                Thread.Sleep(100);
                DWDrvIC.OISMove(ch, DWDrvIC.OIS_MAX_CODE, 8191);
                Thread.Sleep(100);
            }

            DWDrvIC.OISMove(ch, 8191, 8191);      //current 0mA Positio
            for (int i = 0; i < 30; i++)
            {
                DWDrvIC.OISMove(ch, 8191, DWDrvIC.OIS_MIN_CODE);
                Thread.Sleep(100);
                DWDrvIC.OISMove(ch, 8191, DWDrvIC.OIS_MAX_CODE);
                Thread.Sleep(100);
            }


            OISLineCompCoefDW_EX oISLinCompCoef = new OISLineCompCoefDW_EX();
            OISLineCompCoefDW_EX oISLinCompCoefY = new OISLineCompCoefDW_EX();
            FindResult res = new FindResult();
            var step = Condition.OISLincompStep;
            List<short> TargetCode = new List<short>();
            int testinter = (int)(DWDrvIC.OIS_MAX_CODE * 1.0);
            var step_interval = testinter / step;
            double[] ldmDataX = null;//new double;
            double[] ldmDataY = null;//new double;

            int AxisX = (int)AxisTypeDW.AxisX;
            int AxisY = (int)AxisTypeDW.AxisY;

            double ldmOffSetX = 0.0;
            double ldmOffSetY = 0.0;
            //여기까지
            short offset = (short)(DWDrvIC.OIS_MAX_CODE * 0.0);
            for (int i = 0; i < step+1; i++)
            {
                if ((step_interval * (i) > 16383))
                    TargetCode.Add(16383);
                else TargetCode.Add((short)((step_interval * (i)) + offset));
            }
                

            List<double> bufferLDMX = new List<double>();
            List<double> bufferLDMY = new List<double>();

            List<double> checkReadHallX = new List<double>();
            List<double> checkReadHallY = new List<double>();

            LEDs_All_On(0, true);
          
            DWDrvIC.OISOnOff(ch, true);
            Wait(100);

            DWDrvIC.LiearCompClearWrite((int)AxisTypeDW.AxisX);
            DWDrvIC.LiearCompClearWrite((int)AxisTypeDW.AxisY);

            DWDrvIC.SetOperationMode(AxisTypeDW.AxisX, OperationTypeDW.StandbyMode);
            DWDrvIC.SetOperationMode(AxisTypeDW.AxisX, OperationTypeDW.ClosedMode);
            Wait(30);
            DWDrvIC.OISMove(ch, DWDrvIC.OIS_MIN_CODE, DWDrvIC.OIS_MIN_CODE);
            Wait(100);
            res = Measure();

            ldmOffSetX = res.cx[0];
            ldmOffSetY = res.cy[0];

            bufferLDMX.Add(step_interval);
            bufferLDMY.Add(step_interval);

            for (int i = 0; i < TargetCode.Count; i++)
            {
                var targetCode = TargetCode[i];
                if (targetCode == DWDrvIC.OIS_MAX_CODE) targetCode -= 1;

                DWDrvIC.OISMove(ch, targetCode, targetCode);
                Wait(100);
                res = Measure();
                bufferLDMX.Add(res.cx[0]- ldmOffSetX);
                bufferLDMY.Add(res.cy[0]- ldmOffSetY);
            }
       
            AddLog(ch, $"MoveX\tMoveY");
            for (int i = 1; i < bufferLDMX.Count; i++)
            {
                AddLog(ch, $"{bufferLDMX[i].ToString("F2")}\t{bufferLDMY[i].ToString("F2")}");
            }

            ldmDataX = bufferLDMX.ToArray();
            ldmDataY = bufferLDMY.ToArray();

            oISLinCompCoefY.InputValLoad(ldmDataY);
            oISLinCompCoef.InputValLoad(ldmDataX);

            int[] LinCompValueX = new int[15];
            int[] LinCompValueY = new int[15];
            List<int> RealValueCollectionX = new List<int>();
            List<int> RealValueCollectionY = new List<int>();

            oISLinCompCoef.OutputCoeff(LinCompValueX);
            oISLinCompCoefY.OutputCoeff(LinCompValueY);
            RealValueCollectionX.AddRange(LinCompValueX);
            RealValueCollectionY.AddRange(LinCompValueY);

            DWDrvIC.LiearCompWrite(AxisX, RealValueCollectionX);
            DWDrvIC.LiearCompWrite(AxisY, RealValueCollectionY);

            //DWDrvIC.LiearCompWrite(AxisX, RealValue);
            //DWDrvIC.LiearCompWrite(AxisY, RealValue);

            DWDrvIC.SetStore(AxisX);
            DWDrvIC.SetStore(AxisY);

            //DWDrvIC.SetStore(AxisY);
            Wait(500);

            Dln.PowerOnOff(0, false);
            Wait(500);
            Dln.PowerOnOff(0, true);
            Wait(500);

            DWDrvIC.OISOnOff(0, true);
            Wait(500);
            //DWDrvIC.OISICReset(0);
            //Wait(500);
            LEDs_All_On(0,true);
            for (int i = 0; i < TargetCode.Count; i++)
            {
                DWDrvIC.OISMove(ch, TargetCode[i], TargetCode[i]);
                res = Measure();
                Wait(100);
                var positionx = DWDrvIC.ReadOISHall(0, AxisX, 0);
                var positiony = DWDrvIC.ReadOISHall(0, AxisX, 0);
                checkReadHallX.Add(positionx);
                checkReadHallY.Add(positiony);
            }
            LEDs_All_On(0, false);
            AddLog(ch, $"CheckedReadHall");
            AddLog(ch, $"TargetCodeX\tMoveX\tMoveY");
            for (int i = 1; i < checkReadHallX.Count; i++)
            {
                AddLog(ch, $"{TargetCode[i]}\t{checkReadHallX[i].ToString("F2")}\t{checkReadHallY[i].ToString("F2")}");
            }



            //DWDrvIC.OISOnOff(ch, true);
            //Wait(100);
            //Status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            //if (!Status)
            //{
            //    LEDs_All_On(0, false);
            //    PassFails[ch].Results[(int)SpecItem.OISLCCComp].Val = 1;
            //    ShowDataResults(ch, (int)SpecItem.OISLCCComp, (int)SpecItem.OISLCCComp, InspType.OKNG, new double[] { });
            //    return;
            //}
            //Dln.WriteByte(ch, DrvIC.OIS_Addr, 0x617A, 2, 0x01);
            //Status = DrvIC.OIS_StausCheck(ch, 0x01, 0x02);
            //if (!Status)
            //{
            //    LEDs_All_On(0, false);
            //    PassFails[ch].Results[(int)SpecItem.OISLCCComp].Val = 1;
            //    ShowDataResults(ch, (int)SpecItem.OISLCCComp, (int)SpecItem.OISLCCComp, InspType.OKNG, new double[] { });
            //    return;
            //}


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

        public bool OISPM(int ch, int axis, sFRA_TestSetting fra_setting,ref sFRA_Margin fra_result)
        {
            if (axis == (int)AxisTypeDW.AxisX)
                AddLog(ch, $"Phase Margin Axis X");
            if (axis == (int)AxisTypeDW.AxisY)
                AddLog(ch, $"Phase Margin Axis Y");

            Echo_FRA_Measurement measure = new Echo_FRA_Measurement(DWDrvIC, DWDrvIC.Controls, AddLog);
            Echo_FRA_Serch serch = new Echo_FRA_Serch(AddLog);

            fra_setting.ois_control_freq = (byte)measure.CTRL_FREQ_10KHZ;
           
            int msg = 0;

            DWDrvIC.OISReset(ch, (int)AxisTypeDW.AxisX, true);
            DWDrvIC.OISReset(ch, (int)AxisTypeDW.AxisY, true);

            if (!DWDrvIC.Echo_Board_WhoAmI(ch))
            {
                //m__G.sHistArray[m__G.sCIndex[ch], (int)FZ4P.Global.SpecItem.PassFail] = (int)Global.NonSpecItem.PM_TEST_NG;
                AddLog(ch, string.Format($"Not found Echo_Board info", false));
                m__G.m_ChannelOn[ch] = false;
                return false;
            }

            //fra_setting.ois_slave_id = 0x78;

            double[] freq_buf = new double[fra_setting.test_point];
            double[] gain_buf = new double[fra_setting.test_point];
            double[] phase_buf = new double[fra_setting.test_point];
            int SearchCnt = 0;

            msg = measure.Echo_FRA_Single_Measurement(ch, ref fra_result, ref fra_setting, ref freq_buf, ref gain_buf, ref phase_buf, ref SearchCnt,false);

            //260309 : Single 풀시캔시 해당 SearchCnt를 리턴해줫지만 .. 필터 기능이 없어지면서 해당 배열을 전부 스캔해야됨.
            msg = serch.Search_PM(ch, ref fra_result, fra_setting, freq_buf, gain_buf, phase_buf, SearchCnt);
            var realpoint = fra_setting.test_point - 2;

            if (axis != 0)
            {
                if (fra_result.phase_margin < Condition.iYPMMin || fra_result.phase_margin > Condition.iYPMMax || double.IsNaN(fra_result.phase_margin))
                    return false;
                else
                    return true;
            }

            return true;
        }
        public bool OISGM(int ch, int axis, sFRA_TestSetting fra_setting,ref sFRA_Margin fra_result)
        {
            if(axis == (int)AxisTypeDW.AxisX)
                AddLog(ch, $"Gain Margin Axis X");
            if (axis == (int)AxisTypeDW.AxisY)
                AddLog(ch, $"Gain Margin Axis Y");
            Echo_FRA_Measurement measure = new Echo_FRA_Measurement(DWDrvIC, DWDrvIC.Controls, AddLog);
            Echo_FRA_Serch serch = new Echo_FRA_Serch(AddLog);

            fra_setting.ois_control_freq = (byte)measure.CTRL_FREQ_10KHZ;

            int msg = 0;

            DWDrvIC.OISReset(ch, (int)AxisTypeDW.AxisX, true);
            DWDrvIC.OISReset(ch, (int)AxisTypeDW.AxisY, true);

            if (!DWDrvIC.Echo_Board_WhoAmI(ch))
            {
                //m__G.sHistArray[m__G.sCIndex[ch], (int)FZ4P.Global.SpecItem.PassFail] = (int)Global.NonSpecItem.PM_TEST_NG;
                AddLog(ch, string.Format($"Not found Echo_Board info", false));
                m__G.m_ChannelOn[ch] = false;
                return false;
            }

            double[] freq_buf = new double[fra_setting.test_point];
            double[] gain_buf = new double[fra_setting.test_point];
            double[] phase_buf = new double[fra_setting.test_point];
            int SearchCnt = 0;

            msg = measure.Echo_FRA_Single_Measurement(ch, ref fra_result, ref fra_setting, ref freq_buf, ref gain_buf, ref phase_buf, ref SearchCnt, false, true);

            msg = serch.Search_GM(ch, ref fra_result, fra_setting, freq_buf, gain_buf, phase_buf, SearchCnt);
            var realpoint = fra_setting.test_point - 2;
            //msg = Search_GM(ch, ref fra_result, fra_setting, freq_buf, gain_buf, phase_buf, realpoint);

            return true;

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


        public void OIS_ChangedI3C(int ch, string testItem, int InspCnt)
        {
            OISSetI3C(AxisTypeDW.AxisX);
            Thread.Sleep(100);
            OISSetI3C(AxisTypeDW.AxisY);
            Thread.Sleep(100);
        }

        public void OIS_ChangedI2C(int ch, string testItem, int InspCnt)
        {
            OISSetI2C(AxisTypeDW.AxisX);
            Thread.Sleep(100);
            OISSetI2C(AxisTypeDW.AxisY);
            Thread.Sleep(100);
        }

        private void OISSetI2C(AxisTypeDW axisTypeDW)
        {
            DWDrvIC.SetOperationMode(axisTypeDW, OperationTypeDW.StandbyMode);
            Thread.Sleep(10);
            DWDrvIC.Set_PT((int)axisTypeDW, false);
            Thread.Sleep(10);
            DWDrvIC.SetRegisterI2CMode(axisTypeDW);
        }
        private void OISSetI3C(AxisTypeDW axisTypeDW)
        {
            DWDrvIC.SetOperationMode(axisTypeDW, OperationTypeDW.StandbyMode);
            Thread.Sleep(10);
            DWDrvIC.Set_PT((int)axisTypeDW, false);
            Thread.Sleep(10);
            DWDrvIC.SetRegisterI3CMode(axisTypeDW);
        }
        #endregion
    }
}
