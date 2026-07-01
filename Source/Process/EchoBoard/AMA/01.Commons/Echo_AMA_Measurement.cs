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
    public class Echo_AMA_Measurement : Ehco_BoardBase
    {
        private int TEST_LIMIT_CNT = 100;
        private byte DATA_SUCESS = 0xDC;
        private readonly Echo_AMA_ParamSet ParamSetExecutor = null;
        private readonly Action<int, string> _logAction;

        public Echo_AMA_Measurement(IOneTwoBytesDrivingIC function, IFRAFunction fRA, Action<int, string> LogAction) 
            : base(function, fRA)
        {
            _logAction = LogAction;
            ParamSetExecutor = new Echo_AMA_ParamSet(function, fRA, LogAction);
        }
        public bool PollAmaStatus(int expectedStatus, int intervalMs = 100, int maxRetries = 50)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                Thread.Sleep(intervalMs);
                var status = ReadByte((int)RegisterMapAMA.AMA_STATUS);

                if (status == expectedStatus) return true;
                if (status >= 0xE0) return false;
            }
            return false;
        }
        public bool Echo_AMA_SineWave_Measurement(int ch, AMA_TestSetting_Params parmas)
        {
            int cnt = 0;
            byte[] sts_check = new byte[1] { 0x00 };
            byte[] err_list = new byte[1] { 0x00 };

            int totalCycles = parmas.Dummy_cycle_count + parmas.Measurement_cycle_count;
            int cycleMs = parmas.Frequency > 0 ? (int)(1000.0 / parmas.Frequency) : 1000;
            int waitMs = Math.Max(500, totalCycles * cycleMs);

            ParamSetExecutor.SetParam(ch, parmas);

            ParamSetExecutor.SetErrorCount(ch, (int)AxisTypeDW.AxisX, 1);
            ParamSetExecutor.SetErrorCount(ch, (int)AxisTypeDW.AxisY, 1);

            ParamSetExecutor.SetAMA_MODE(ch, AMA_MODE.SINEWAVE);

            FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Ready);

            var statusSuccess = PollAmaStatus(0x01, 100, TEST_LIMIT_CNT);

            if (statusSuccess)
            {
                _logAction(ch, string.Format($"[echo_sinewave_measurement] Read OK (cnt={cnt})"));
            }
            else
            {
                sts_check[0] = ReadByte((int)RegisterMapAMA.AMA_STATUS);
                _logAction(ch, string.Format("[echo_sinewave_measurement] State timeout (Status=0x{0 :X2})",
                    sts_check[0]));
                FraFunction.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                Thread.Sleep(100);
                return false;
            }

            FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Start);

            Thread.Sleep(waitMs);

            statusSuccess = PollAmaStatus(DATA_SUCESS, 100, TEST_LIMIT_CNT);

            if (statusSuccess)
            {
                _logAction(ch, string.Format($"[echo_sinewave_measurement] Test OK (cnt={cnt})"));
            }
            else
            {
                sts_check[0] = ReadByte((int)RegisterMapAMA.AMA_STATUS);
                _logAction(ch, string.Format("[echo_sinewave_measurement] Test NG timeout (Status=0x{0 :X2})",
                    sts_check[0]));
                FraFunction.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                Thread.Sleep(100);
                return false;
            }
            return true;
        }
        public SineResult SineWaveMeasurement(int ch, AMA_TestSetting_Params parmas)
        {
            var result = new SineResult();
            
            var seccess = Echo_AMA_SineWave_Measurement(ch, parmas);

            if (seccess)
            {
                var DeltaX = ReadWord((int)RegisterMapAMA.SINE_WAVEX_MAX_H);
                var DeltaY = ReadWord((int)RegisterMapAMA.SINE_WAVEY_MAX_H);
                var ngCountX = ReadWord((int)RegisterMapAMA.SINE_WAVEX_NG_1) << 16 + ReadWord((int)RegisterMapAMA.SINE_WAVEX_NG_3);
                var ngCountY = ReadWord((int)RegisterMapAMA.SINE_WAVEY_NG_1) << 16 + ReadWord((int)RegisterMapAMA.SINE_WAVEY_NG_3);

                result.DeltaMaxX = DeltaX;
                result.DeltaMaxY = DeltaY;
                result.NgCountX = ngCountX;
                result.NgCountY = ngCountY;

                FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Stop);
                _logAction(ch, string.Format("[echo_sinewave_measurement] {2}(REG 0x{1:X4}) = 0x{0:X2}", 0x00, (byte)RegisterMapAMA.AMA_START, "AMA_STOP"));

                return result;
            }
            else
            {
                FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Stop);
                _logAction(ch, string.Format("[echo_sinewave_measurement] {2}(REG 0x{1:X4}) = 0x{0:X2}", 0x00, (byte)RegisterMapAMA.AMA_START, "AMA_STOP"));
                return result;
            }
        }

        public bool Echo_AMA_Ringing_Measurement(int ch, AMA_RingingSetting_Params parmas)
        {
            int cnt = 0;
            byte[] sts_check = new byte[1] { 0x00 };
            byte[] err_list = new byte[1] { 0x00 };

            ParamSetExecutor.SetParam(ch, parmas);

            //ParamSetExecutor.SetAMA_MODE(ch,AMA_MODE.RINGING);

            FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Ready);

            var statusSuccess = PollAmaStatus(0x01, 100, TEST_LIMIT_CNT);

            if (statusSuccess)
            {
                _logAction(ch, string.Format($"[echo_sinewave_measurement] Read OK (cnt={cnt})"));
            }
            else
            {
                sts_check[0] = ReadByte((int)RegisterMapAMA.AMA_STATUS);
                _logAction(ch, string.Format("[echo_sinewave_measurement] State timeout (Status=0x{0 :X2})",
                    sts_check[0]));
                FraFunction.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                Thread.Sleep(100);
                return false;
            }

            FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Start);

            //Thread.Sleep(waitMs);
            Thread.Sleep(100);

            statusSuccess = PollAmaStatus(DATA_SUCESS, 100, TEST_LIMIT_CNT);

            if (statusSuccess)
            {
                _logAction(ch, string.Format($"[echo_sinewave_measurement] Test OK (cnt={cnt})"));
            }
            else
            {
                sts_check[0] = ReadByte((int)RegisterMapAMA.AMA_STATUS);
                _logAction(ch, string.Format("[echo_sinewave_measurement] Test NG timeout (Status=0x{0 :X2})",
                    sts_check[0]));
                FraFunction.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                Thread.Sleep(100);
                return false;
            }
            return true;
        }
        public SineResult RingingMeasurement(int ch, AMA_RingingSetting_Params parmas)
        {
            var result = new SineResult();

            var seccess = Echo_AMA_Ringing_Measurement(ch, parmas);

            if (seccess)
            {
                var OkCountX = ReadWord((int)RegisterMapAMA.AMA_RINGING_OK_X_1) << 16 + +ReadWord((int)RegisterMapAMA.AMA_RINGING_OK_X_3);
                var OkCountY = ReadWord((int)RegisterMapAMA.AMA_RINGING_OK_Y_1) << 16 + +ReadWord((int)RegisterMapAMA.AMA_RINGING_OK_Y_3);

                FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Stop);
                _logAction(ch, string.Format("[echo_sinewave_measurement] {2}(REG 0x{1:X4}) = 0x{0:X2}", 0x00, (byte)RegisterMapAMA.AMA_START, "AMA_STOP"));

                return result;
            }
            else
            {
                FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Stop);
                _logAction(ch, string.Format("[echo_sinewave_measurement] {2}(REG 0x{1:X4}) = 0x{0:X2}", 0x00, (byte)RegisterMapAMA.AMA_START, "AMA_STOP"));
                return result;
            }
        }

        //2byte Read ?? 연속 읽기가 안되어 오버라이드 함.
        protected override ushort ReadWord(int startAddress)
        {
            byte[] tmp = new byte[2] { 0x00, 0x00 };
            tmp[0] = ReadByte(startAddress);
            tmp[1] = ReadByte(startAddress+1);
            
            var data = (tmp[0] << 8 | tmp[1]);
            _logAction(0, string.Format("[echo_sinewave_measurement] {2}(REG 0x{1:X4}) = 0x{0:X4}", (ushort)data, (ushort)(startAddress << 8)+ (startAddress+1), "Read Word"));

            return (ushort)(data);
        }
    }
}
