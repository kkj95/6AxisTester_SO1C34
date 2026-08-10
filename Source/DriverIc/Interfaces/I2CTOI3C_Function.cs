using FZ4P.DriverIc.OISIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.Interfaces
{
    /// <summary>
    /// I3C를 직접 제어하는것이 아닌 
    /// I2C를 통하여 I3C 통신을 하는 역할을 수행 합니다.
    /// </summary>
    public interface I2CTOI3C_Function
    {
        IOISFunction OIS { get; }
        void SetI3CByPaaMode(bool Onoff);
        void SetH503WakeUp();
        short GetI3CCheckBuffer(AxisTypeDW axisTypeDW, int bufferCheckType);
    }
}
