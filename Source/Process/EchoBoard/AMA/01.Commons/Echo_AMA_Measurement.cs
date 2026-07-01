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
        private int TEST_LIMIT_CNT = 500;
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

            ParamSetExecutor.SetErrorCount(ch, (int)AxisTypeDW.AxisX, 100);
            ParamSetExecutor.SetErrorCount(ch, (int)AxisTypeDW.AxisY, 100);

            ParamSetExecutor.SetAMA_MODE(ch);

            FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Ready);

            var statusSuccess = PollAmaStatus(0x01, 100, TEST_LIMIT_CNT);

            if (statusSuccess)
            {
                _logAction(ch, string.Format($"[echo_fra_single_measurement] Mode ON OK (cnt={cnt})"));
            }
            else
            {
                err_list[0] = ReadByte((int)RegisterMapFRA.ERROR_CODE);
                _logAction(ch, string.Format("[echo_fra_single_measurement] Mode ON timeout (Status={0}, Err={1})",
                    sts_check[0], err_list[0]));
                FraFunction.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                Thread.Sleep(100);
                return false;
            }

            FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Start);

            Thread.Sleep(waitMs);

            statusSuccess = PollAmaStatus(DATA_SUCESS, 100, TEST_LIMIT_CNT);

            if (statusSuccess)
            {
                _logAction(ch, string.Format($"[echo_fra_single_measurement] Mode ON OK (cnt={cnt})"));
            }
            else
            {
                err_list[0] = ReadByte((int)RegisterMapFRA.ERROR_CODE);
                _logAction(ch, string.Format("[echo_fra_single_measurement] Mode ON timeout (Status={0}, Err={1})",
                    sts_check[0], err_list[0]));
                FraFunction.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                Thread.Sleep(100);
                return false;
            }

            FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Stop);

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

                var ngCountX = ReadWord((int)RegisterMapAMA.SINE_WAVEX_NG_1)<<16 + ReadWord((int)RegisterMapAMA.SINE_WAVEX_NG_3);
                var ngCountY = ReadWord((int)RegisterMapAMA.SINE_WAVEY_NG_1) << 16 + ReadWord((int)RegisterMapAMA.SINE_WAVEY_NG_3);

                result.DeltaMaxX = DeltaX;
                result.DeltaMaxY = DeltaY;
                result.NgCountX = ngCountX;
                result.NgCountY = ngCountY;

                return result;
            }
            else
                return result;
        }
    }
}
