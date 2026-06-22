using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.OISIC
{
    public enum OperationTypeDW
    {
        StandbyMode = 0,
        SleepMode = 1,
        ClosedMode = 2,
    }

    public enum AxisTypeDW
    {
        AxisX = 0,
        AxisY = 1,
        AxisZ = 2,
    }

    public enum eRegisterMapDW9836N
    {
        Target1 = 0x00,                 //무브
        Target2 = 0x01,                 //무브
        Mode = 0x02,                    //Operation 모드
        STORE_PROD_ID = 0x03,           //저장 관련
        SWREST = 0x04,                  // Software Reset
        IC_INFO = 0x21,                 // IC정보
        PCAL_LOW = 0x40,                // PCAL 
        PCAL_HIGH = 0x41,
        NCAL_LOW = 0x42,                //NCAL 
        NCAL_HIGH = 0x43,
        //PID 0x44~0x83 0x7F제외
        POSITION_READ_LOW = 0x84,       //ReadHall??
        POSITION_READ_HIGH = 0x85,
        IOUT_CURRENT_LOW = 0x8E,
        IOUT_CURRENT_HIGH = 0x8F,
        //PID
        //FREACCESS
    }
}
