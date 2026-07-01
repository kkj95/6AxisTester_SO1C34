using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.OISIC
{
    public enum StartStopType
    { Stop, Start, Ready }
    public enum OperationTypeDW
    {
        StandbyMode = 0,
        SleepMode = 1,
        ClosedMode = 2,
        OpenMode = 3,
    }

    public enum AxisTypeDW
    {
        AxisX = 0,
        AxisY = 1,
        AxisZ = 2,
    }

    public enum RegisterMapDW9836N
    {
        Target = 0x00,                 //무브
        Target1 = 0x01,                 //무브1
        Mode = 0x02,                    //Operation 모드
        STORE_PROD_ID = 0x03,           //저장 관련
        SWREST = 0x04,                  // Software Reset
        IC_INFO = 0x21,                 // IC정보
        PCAL_LOW = 0x40,                // PCAL 
        NCAL_LOW = 0x42,                //NCAL 
        //PID 0x44~0x83 0x7F제외
        POSITION_READ_LOW = 0x84,       //ReadHall??
        POSITION_READ_HIGH = 0x85,       //ReadHall??
        IOUT_CURRENT_LOW = 0x8E,
        //PID
        //FREACCESS
    }

    public enum RegisterMapPIDDW9836N
    {
        PCAL = 0x46,
        NCAL = 0x47,
    }

    public enum RegisterMapFRA
    {
        BOARD_INFO = 0x00,
        FRA_START = 0x01,
        STATUS = 0x02,
        FRA_MODE = 0x03,
        AMPLITUDE_H = 0x04,
        AMPLITUDE_L = 0x05,
        OFFSET_H = 0x06,
        OFFSET_L = 0x07,
        REG_INIT = 0x08,
        ERROR_CODE = 0x09,
        FRA_POINT_H = 0x0A,
        FRA_POINT_L = 0x0B,
        START_FREQ_H = 0x0C,
        START_FREQ_L = 0x0D,
        END_FREQ_H = 0x0E,
        END_FREQ_L = 0x0F,
        TARGET_POS_H = 0x10,
        TARGET_POS_L = 0x11,
        I2C_CH = 0x12,
        FRA_CTRLER = 0x13,
        TARGET_WAIT_CYCLE = 0x14,
        TARGET_WAIT_TIME = 0x15,
        TARGET_POS2_H = 0x16,
        TARGET_POS2_L = 0x16,

        FRA_RESULT_FREQUENCY_H = 0x20,
        FRA_RESULT_FREQUENCY_L = 0x21,
        FRA_RESULT_MAGNITUDE_H = 0x22,
        FRA_RESULT_MAGNITUDE_L = 0x23,
        FRA_RESULT_PHASE_H = 0x24,
        FRA_RESULT_PHASE_L = 0x25,

        CONTROL_FREQ = 0x2F,

        TARGET_SLAVE_ADDR = 0x30,
        TARGET_DEVICE = 0x31,
        LOD_ENABLE = 0x32,
        I2C_CH1_AVDD = 0x33,
        I2C_CH1_IOVDD= 0x34,
        I2C_CH2_AVDD = 0x35,
        I2C_CH2_IOVDD = 0x36,
        I2C_CH1_SPEED = 0x37,
        I2C_CH2_SPEED = 0x38,
        CURRENT_SET_WRITE = 0x39,
        CURRENT_SET_READ = 0x40,

        AMP_MODE = 0xE1,


        VERSION_MAJOR = 0xF2,
        VERSION_MINOR = 0xF3,
    }

    public enum RegisterMapAMA
    {
        AMA_START = 0x70,
        AMA_STATUS = 0x71,
        AMA_MODE = 0x72,
        AMA_CLK_DIV = 0x73,
        AMA_ADDR_0 = 0x74,
        AMA_ADDR_1 = 0x75,
        AMA_ADDR_2 = 0x76,
        AMA_ADDR_3 = 0x77,
        AMA_ADDR_NUM = 0x78,
        AMA_ID_X = 0x7D,
        AMA_ID_Y = 0x7E,
        AMA_ID_Z = 0x7F,
        AMA_OIS_NUM= 0x80,
        AMA_Z_NUM = 0x81,
        AMA_RINGING_ENDPOS = 0x8A,
        AMA_RINGING_STARTPOS = 0x8B,
        AMA_RINGING_STARTTIME = 0x8C,
        AMA_RINGING_ENDTIME = 0x8D,
        AMA_RINGING_THD = 0x8E,


        AMA_SINEWAVE_FREQ = 0x97,
        AMA_SINEWAVE_AMP = 0x98,
        AMA_SINEWAVE_THD = 0x99,
        AMA_SINEWAVE_FIXED_CYCLE = 0x9A,
        AMA_SINEWAVE_DUMMY_CYCLE = 0x9B,
        AMA_SINEWAVE_ERRCNT_X_H = 0x9C,
        AMA_SINEWAVE_ERRCNT_X_L = 0x9D,
        AMA_SINEWAVE_ERRCNT_Y_H = 0x9E,
        AMA_SINEWAVE_ERRCNT_Y_L = 0x9F,

        SINE_WAVEX_MAX_H = 0xA8,
        SINE_WAVEX_MAX_L = 0xA9,
        SINE_WAVEY_MAX_H = 0xAA,
        SINE_WAVEY_MAX_L = 0xAB,

        SINE_WAVEX_NG_1 = 0xA0,
        SINE_WAVEX_NG_2 = 0xA1,
        SINE_WAVEX_NG_3 = 0xA2,
        SINE_WAVEX_NG_4 = 0xA3,

        SINE_WAVEY_NG_1 = 0xA4,
        SINE_WAVEY_NG_2 = 0xA5,
        SINE_WAVEY_NG_3 = 0xA6,
        SINE_WAVEY_NG_4 = 0xA7,
    }
}
