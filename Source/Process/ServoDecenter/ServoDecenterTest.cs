using FZ4P.DriverIc.Interfaces;
using FZ4P.DriverIc.OISIC;
using S2System.Vision;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{
    public class ServoDecenterParams
    {
        public int targetCode { get; set; } = 0;
        public int CenterCode { get; set; } = 0;
        public int ServoDecenterDelay { get; set; } = 0;
    }
    public class ServoDecenterTest
    {
        private readonly IOISFunction _oISFunction = null;
        private readonly IAFunction _afFunction = null;
        private readonly Action<int, string> _actionLog = null;
        private FVision _fvision;

        private ServoDecenterParams _params = null;

        public ServoDecenterTest(IOISFunction oISFunction, IAFunction afFunction, FVision fVision,  Action<int, string> actionLog)
        {
            _oISFunction = oISFunction;
            _afFunction = afFunction;
            _fvision = fVision;
            _actionLog = actionLog;
        }
        public ServoDecenterTest SetParams(ServoDecenterParams agingParams)
        {
            _params = agingParams;
            return this;
        }
        public async Task<IEnumerable<double>> Execute()
        {
            List<double> doubleValue = new List<double>();
            _oISFunction.OISOnOff(0, true);
            _afFunction.AFOnOff(0, true);

            _afFunction.AFMove(0, _params.targetCode);
            await Task.Delay(300);

            _actionLog(0, $"AF Position : {_afFunction.ReadAFHall(0)}");
            _oISFunction.OISMove(0, _params.CenterCode, _params.CenterCode);
            await Task.Delay(200);

            var ServoOnMeasure = Measure();

            _oISFunction.OISOnOff(0, false);
            _afFunction.AFOnOff(0, false);

            await Task.Delay(_params.ServoDecenterDelay);
            var ServoOffMeasure = Measure();
            var ServoDecenterX = (ServoOffMeasure.cx[0] - ServoOnMeasure.cx[0]);
            var ServoDecenterY = (ServoOnMeasure.cy[0] - ServoOffMeasure.cy[0]);
            _actionLog(0, $"Decenter X = {ServoDecenterX.ToString("F2")}");
            _actionLog(0, $"Decenter Y = {ServoDecenterY.ToString("F2")}");
            _actionLog(0, "<<<  OIS Servo Decenter End  >>>");

            doubleValue.Add(ServoDecenterX);
            doubleValue.Add(ServoDecenterY);

            return doubleValue;
        }

        private FindResult Measure()
        {
            _fvision.m__G.oCam[0].Grab(0);
            var res = _fvision.MeasureTxTyTz(0);
            return res;
        }
    }
}
