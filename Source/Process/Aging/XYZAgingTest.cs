using FZ4P.DriverIc.Interfaces;
using FZ4P.DriverIc.OISIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{

    public class AgingParams
    {
        public int Freq { get; set; } = 0;
        public int Count { get; set; } = 0;
        public int AFMinCode { get; set; } = 0;
        public int AFMaxCode { get; set; } = 0;
        public int OISMinCode { get; set; } = 0;
        public int OISMaxCode { get; set; } = 0;
    }
    public class XYZAgingTest
    {
        private readonly IOISFunction _oISFunction = null;
        private readonly IAFunction _afFunction = null;
        private readonly Action<int,string> _actionLog = null;

        private const int period_ms = 1000;
        private const int period_ns = 1000000;

        private AgingParams _agingParams = null;
        public XYZAgingTest(IOISFunction oISFunction, IAFunction afFunction, Action<int,string> actionLog)
        {
            _oISFunction = oISFunction;
            _afFunction = afFunction;
            _actionLog = actionLog;
        }

        public XYZAgingTest SetParams(AgingParams agingParams)
        {
            _agingParams = agingParams;
            return this;
        }

        public async void Execute()
        {
            var half_period = GetPeriodms() / 2;
            _oISFunction.OISOnOff(0,true);
            _afFunction.AFOnOff(0,true);

            _actionLog(0, $"AFHall,\tXHall,\tYHall");

            for (int i = 0; i < _agingParams.Count; i++)
            {
                _afFunction.AFMove(0, _agingParams.AFMinCode);
                _oISFunction.OISMove(0, _agingParams.OISMinCode, _agingParams.OISMinCode);                // OIS min code move
                await Task.Delay(half_period);

                var afHall = _afFunction.ReadAFHall(0);
                var OISXHall= _oISFunction.ReadOISHall(0,(int)AxisTypeDW.AxisX);
                var OISYHall = _oISFunction.ReadOISHall(0, (int)AxisTypeDW.AxisY);
                _actionLog(0, $"{afHall},\t{OISXHall},\t{OISYHall}");

                _afFunction.AFMove(0, _agingParams.AFMaxCode);
                _oISFunction.OISMove(0, _agingParams.OISMaxCode, _agingParams.OISMaxCode);                // OIS max code move
                await Task.Delay(half_period);

                var hall = _afFunction.ReadAFHall(0);
                var hall2 = _oISFunction.ReadOISHall(0, (int)AxisTypeDW.AxisX);
                var hall3 = _oISFunction.ReadOISHall(0, (int)AxisTypeDW.AxisY);
            }

            _oISFunction.OISOnOff(0, false);
            _afFunction.AFOnOff(0, false);
        }

        private int GetPeriodms()
        {
            return (period_ms / _agingParams.Freq);
        }
    }
}
