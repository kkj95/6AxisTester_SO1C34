using FZ4P.DriverIc.DongWoon.Params;
using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.IC_Code_Information.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.OISIC.Base
{
    public abstract class DriveICBase
    {
        private readonly IOneTwoBytesDrivingIC _controls;

        public int IC_MAX_CODE { get; set; } = 0;
        public int IC_MID_CODE { get; set; } = 0;
        public int IC_MIN_CODE { get; set; } = 0;

        protected abstract int GetAxisTypeID(AxisTypeDW axisType);

        protected DriveICBase(IOneTwoBytesDrivingIC controls)
        {
            _controls = controls;
        }

        protected void Lazy_Initialize(ICBitType type)
        {
            var context = new ResultCodeContext();
            var ICCodeValue = context.GetCodeValue(type);
            IC_MAX_CODE = ICCodeValue.MaxCode;
            IC_MID_CODE = ICCodeValue.MidCode;
            IC_MIN_CODE = ICCodeValue.MinCode;
        }
    }
}
