using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.I2CBase.Interfaces
{
    //구동 제어 동작을 one byte 형식으로만 진행한다 .
    public interface IOneBytesDrivingIC
    {
        bool WriteByte(int slaveAddr, int memAddr, int memCnt, byte data);
        byte ReadByte(int slaveAddr, int memAddr, int memCnt);
    }
}
