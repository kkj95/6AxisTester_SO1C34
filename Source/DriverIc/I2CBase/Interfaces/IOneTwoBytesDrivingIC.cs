using FZ4P.Commons.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.I2CBase.Interfaces
{
    public interface IOneTwoBytesDrivingIC : IOneBytesDrivingIC,ITwoBytesDrivingIC
    {
        event EventHandler<StringEventArgs> exceptionError;
    }
}
