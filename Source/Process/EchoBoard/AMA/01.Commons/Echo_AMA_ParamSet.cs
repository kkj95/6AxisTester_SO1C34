using FZ4P.DriverIc.I2CBase;
using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using FZ4P.DriverIc.OISIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FZ4P
{
    public class Echo_AMA_ParamSet : Ehco_BoardBase
    {
        private int _slaveID = 0x00;
        private readonly Action<int, string> LogAction;
        public Echo_AMA_ParamSet(IOneTwoBytesDrivingIC function, IFRAFunction fRA, Action<int, string> LogAction) :
            base(function, fRA)
        {
        }

        public void SetAMA_MODE(int ch)
        {
            SetToWrite(ch, RegisterMapAMA.AMA_MODE, 0x04 << 1, "AMA_MODE");                                          // 0x03: RING MODE 0x04: SINEWAVE MODE
        }

        public void SetParam(int ch, AMA_TestSetting_Params _param)
        {
            /* --------------------
             * Board config
             * -------------------- */
            SetToWrite(ch, RegisterMapAMA.AMA_ID_X, _param.Target_slave_id_X << 1, "AMA_ID_X");
            SetToWrite(ch, RegisterMapAMA.AMA_ID_Y, _param.Target_slave_id_Y << 1, "AMA_ID_Y");
            SetToWrite(ch, RegisterMapAMA.AMA_ID_Z, _param.Target_slave_id_Z << 1, "AMA_ID_Z");
            SetToWrite(ch, RegisterMapAMA.AMA_CLK_DIV, _param.Clock_devision, "AMA_CLK_DIV");                   //*
            SetToWrite(ch, RegisterMapAMA.AMA_OIS_NUM, _param.EOIS_target_device_number, "AMA_OIS_NUM");
            SetToWrite(ch, RegisterMapAMA.AMA_Z_NUM, _param.Af_target_device_number, "AMA_Z_NUM");              //*
            SetToWrite(ch, RegisterMapAMA.AMA_ADDR_0, _param.Set_read_address, "AMA_ADDR_0");                   //*
            SetToWrite(ch, RegisterMapAMA.AMA_ADDR_NUM, _param.Read_address_count, "AMA_ADDR_NUM");             //*
            SetToWrite(ch, RegisterMapAMA.AMA_SINEWAVE_FREQ, _param.Frequency, "AMA_SINEWAVE_FREQ");
            SetToWrite(ch, RegisterMapAMA.AMA_SINEWAVE_AMP, _param.Amplitude, "AMA_SINEWAVE_AMP");
            SetToWrite(ch, RegisterMapAMA.AMA_SINEWAVE_THD, _param.Threshold, "AMA_SINEWAVE_THD");
            SetToWrite(ch, RegisterMapAMA.AMA_SINEWAVE_FIXED_CYCLE, _param.Measurement_cycle_count, "AMA_SINEWAVE_FIXED_CYCLE");
            SetToWrite(ch, RegisterMapAMA.AMA_SINEWAVE_DUMMY_CYCLE, _param.Dummy_cycle_count, "AMA_SINEWAVE_DUMMY_CYCLE");
        }

        public void SetErrorCount(int ch, int aixs, int iCount)
        {
            byte[] tmp = new byte[2] { 0x00, 0x00 };
            tmp[0] = (byte)(iCount & 0xFF); tmp[1] = (byte)(iCount >> 8);

            var axisType = (AxisTypeDW)aixs;
            switch (axisType)
            {
                case AxisTypeDW.AxisX:
                    SetToWrite(ch, RegisterMapAMA.AMA_SINEWAVE_ERRCNT_X_L, tmp[0], "AMA_SINEWAVE_ERRCNT_X_L");
                    SetToWrite(ch, RegisterMapAMA.AMA_SINEWAVE_ERRCNT_X_H, tmp[1], "AMA_SINEWAVE_ERRCNT_X_H");
                    break;
                case AxisTypeDW.AxisY:
                    SetToWrite(ch, RegisterMapAMA.AMA_SINEWAVE_ERRCNT_Y_L, tmp[0], "AMA_SINEWAVE_ERRCNT_Y_L");
                    SetToWrite(ch, RegisterMapAMA.AMA_SINEWAVE_ERRCNT_Y_H, tmp[1], "AMA_SINEWAVE_ERRCNT_Y_H");
                    break;
            }
        }

        private void SetToWrite(int ch, RegisterMapAMA amaRegister,int data, string registerName)
        {
            byte[] u08_dat1 = new byte[1] { 0x00 };
            if (data != 0)
            {
                WriteByte((int)amaRegister, (byte)(data));                                                    
                Thread.Sleep(10);
                u08_dat1[0] = ReadByte((int)amaRegister);
                LogAction(ch, string.Format("[echo_fra_single_measurement] {2}(REG 0x{1:X2}) = {0}", u08_dat1[0], (byte)data, registerName));
            }
        }
    }
}
