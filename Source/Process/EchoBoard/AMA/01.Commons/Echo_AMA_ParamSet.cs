using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{
    public class Echo_AMA_ParamSet
    {
        private readonly AMA_TestSetting_Params _param = null;
        private readonly IOneTwoBytesDrivingIC i2cControl = null; 
        public Echo_AMA_ParamSet(AMA_TestSetting_Params param, IOneTwoBytesDrivingIC function)
        {
            _param = param;
            i2cControl = function as IOneTwoBytesDrivingIC;
        }

        public void SetParam()
        {
            i2cControl.WriteByte();
        }
    }
}
