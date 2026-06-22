using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.I2CBase.Interfaces
{
    public interface ITwoBytesDrivingIC
    {
        bool Write2Byte(int slaveAddr, int memAddr, int memCnt, ushort data);
        bool Write2Byte(int slaveAddr, int memAddr, int memCnt, short data);
        ushort Read2Byte(int slaveAddr, int memAddr, int memCnt);
        short Read2Byte_signed(int slaveAddr, int memAddr, int memCnt);
    }
}
