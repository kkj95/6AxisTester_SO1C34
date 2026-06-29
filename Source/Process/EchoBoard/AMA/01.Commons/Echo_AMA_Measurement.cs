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
        private uint TEST_LIMIT_CNT = 500;
        private byte DATA_SUCESS = 0xDC;
        private readonly Echo_AMA_ParamSet ParamSetExecutor = null;
        private readonly Action<int, string> _logAction;
        public Echo_AMA_Measurement(IOneTwoBytesDrivingIC function, IFRAFunction fRA, Action<int, string> LogAction) 
            : base(function, fRA)
        {
            _logAction = LogAction;
            ParamSetExecutor = new Echo_AMA_ParamSet(function, fRA, LogAction);
        }

        public bool Echo_AMA_SineWave_Measurement(int ch, AMA_TestSetting_Params parmas)
        {
            int cnt = 0;
            byte[] sts_check = new byte[1] { 0x00 };
            byte[] err_list = new byte[1] { 0x00 };

            ParamSetExecutor.SetParam(ch, parmas);

            ParamSetExecutor.SetErrorCount(ch, (int)AxisTypeDW.AxisX, 100);
            ParamSetExecutor.SetErrorCount(ch, (int)AxisTypeDW.AxisY, 100);

            FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Ready);

            for (cnt = 0; cnt <= TEST_LIMIT_CNT; cnt++)
            {
                //Thread.Sleep(20);
                sts_check[0] = ReadByte((int)RegisterMapAMA.AMA_STATUS);
                Thread.Sleep(30);

                if (sts_check[0] == 0x01 )
                {
                    _logAction(ch, string.Format($"[echo_fra_single_measurement] Mode ON OK (cnt={cnt})"));
                    break;
                }

                if (cnt == (TEST_LIMIT_CNT))
                {
                    err_list[0] = ReadByte((int)RegisterMapFRA.ERROR_CODE);
                    _logAction(ch, string.Format("[echo_fra_single_measurement] Mode ON timeout (Status={0}, Err={1})",
                        sts_check[0], err_list[0]));
                    FraFunction.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                    Thread.Sleep(100);
                    return false;
                }
            }

            FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Start);

            for (cnt = 0; cnt <= TEST_LIMIT_CNT; cnt++)
            {
                //Thread.Sleep(20);
                sts_check[0] = ReadByte((int)RegisterMapAMA.AMA_STATUS);
                Thread.Sleep(30);

                if (sts_check[0] == DATA_SUCESS)
                {
                    _logAction(ch, string.Format($"[echo_fra_single_measurement] Mode ON OK (cnt={cnt})"));
                    break;
                }

                if (cnt == (TEST_LIMIT_CNT))
                {
                    err_list[0] = ReadByte((int)RegisterMapFRA.ERROR_CODE);
                    _logAction(ch, string.Format("[echo_fra_single_measurement] Mode ON timeout (Status={0}, Err={1})",
                        sts_check[0], err_list[0]));
                    FraFunction.FRA_Echoboard_StartStop(ch, StartStopType.Stop);
                    Thread.Sleep(100);
                    return false;
                }
            }

            FraFunction.AMA_Echoboard_StartStop(ch, StartStopType.Stop);

            return true;
        }
    }
}
