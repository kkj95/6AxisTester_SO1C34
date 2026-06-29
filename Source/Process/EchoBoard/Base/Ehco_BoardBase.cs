using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{
    public abstract class Ehco_BoardBase
    {
        private readonly IOneTwoBytesDrivingIC _i2cControl = null;
        private readonly IFRAFunction _fraFunction = null;

        protected IFRAFunction FraFunction => _fraFunction;

        public Ehco_BoardBase(IOneTwoBytesDrivingIC i2cControl, IFRAFunction fraFunction)
        {
            _i2cControl = i2cControl;
            _fraFunction = fraFunction;
        }

        protected virtual void WriteByte(int addr, byte data)
        {
            _i2cControl.WriteByte(FraFunction.FRA_Addr, addr, 1, data);
        }
        protected virtual byte ReadByte(int addr)
        {
            return _i2cControl.ReadByte(FraFunction.FRA_Addr, addr, 1);
        }
    }
}
