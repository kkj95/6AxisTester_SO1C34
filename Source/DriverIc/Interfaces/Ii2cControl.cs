using FZ4P.DriverIc.I2CBase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.Interfaces
{
    internal interface Ii2cControl
    {
        IOneTwoBytesDrivingIC GetOneTwoBytesDrivingIC();
    }
}
