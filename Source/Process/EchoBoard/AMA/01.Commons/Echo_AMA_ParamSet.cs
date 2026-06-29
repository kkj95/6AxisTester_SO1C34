using FZ4P.DriverIc.I2CBase;
using FZ4P.DriverIc.I2CBase.Interfaces;
using FZ4P.DriverIc.Interfaces;
using FZ4P.DriverIc.OISIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FZ4P
{
    public class Echo_AMA_ParamSet
    {
        private readonly AMA_TestSetting_Params _param = null;
        private readonly IOneTwoBytesDrivingIC _i2cControl = null;
        private int _slaveID = 0x00;
        private readonly Action<int, string> LogAction;
        public Echo_AMA_ParamSet(AMA_TestSetting_Params param, IOneTwoBytesDrivingIC function, int SlaveID, Action<int, string> LogAction)
        {
            _param = param;
            _i2cControl = function as IOneTwoBytesDrivingIC;
            _slaveID = SlaveID;
        }

        public void SetParam(int ch)
        {
            byte[] u08_dat1 = new byte[1] { 0x00 };
            byte[] u08_dat2 = new byte[1] { 0x00 };
            byte[] board_info = new byte[1] { 0x00 };
            int cnt = 0;
            byte[] sts_check = new byte[1] { 0x00 };
            byte[] err_list = new byte[1] { 0x00 };

            //i2cControl.WriteByte();


            /* --------------------
           * Board version / power info
           * -------------------- */
            u08_dat1[0] = _i2cControl.ReadByte(_slaveID, (int)RegisterMapFRA.VERSION_MAJOR, 1);
            u08_dat2[0] = _i2cControl.ReadByte(_slaveID, (int)RegisterMapFRA.VERSION_MINOR, 1);

            LogAction(ch, string.Format("[echo_fra_single_measurement] FRA Board Version(REG 0xF2): 0x{0:X2} 0x{1:X2}", u08_dat1[0], u08_dat2[0]));

            u08_dat1[0] = _i2cControl.ReadByte(_slaveID, (int)RegisterMapFRA.LOD_ENABLE, 1);
            LogAction(ch, string.Format("[echo_fra_single_measurement] VDD OUT(REG 0x32) = {0}", u08_dat1[0]));

            //WriteByte((int)RegisterMapFRA.I2C_CH1_AVDD, 0x01);
            Thread.Sleep(10);
            u08_dat1[0] = _i2cControl.ReadByte(_slaveID, (int)RegisterMapFRA.I2C_CH1_AVDD, 1);
            LogAction(ch, string.Format("[echo_fra_single_measurement] CH1 AVDD(REG 0x33) = {0}", u08_dat1[0]));

            //WriteByte((int)RegisterMapFRA.I2C_CH1_IOVDD, 0x01);
            Thread.Sleep(10);
            u08_dat1[0] = _i2cControl.ReadByte(_slaveID, (int)RegisterMapFRA.I2C_CH1_IOVDD, 1);
            LogAction(ch, string.Format("[echo_fra_single_measurement] CH1 IOVDD(REG 0x34) = {0}", u08_dat1[0]));

            //WriteByte((int)RegisterMapFRA.I2C_CH2_AVDD, 0x01);
            Thread.Sleep(10);
            u08_dat1[0] = _i2cControl.ReadByte(_slaveID, (int)RegisterMapFRA.I2C_CH2_AVDD, 1);
            LogAction(ch, string.Format("[echo_fra_single_measurement] CH2 AVDD(REG 0x35) = {0}", u08_dat1[0]));

            //WriteByte((int)RegisterMapFRA.I2C_CH2_IOVDD, 0x00);                                                                 //0x00 1.2V 0x01 1.8V   0x02 2.8V
            Thread.Sleep(10);
            u08_dat1[0] = _i2cControl.ReadByte(_slaveID, (int)RegisterMapFRA.I2C_CH2_IOVDD, 1);
            LogAction(ch, string.Format("[echo_fra_single_measurement] CH2 IOVDD(REG 0x36) = {0}", u08_dat1[0]));
        }
    }
}
