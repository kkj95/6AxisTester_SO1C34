using Dln.I2cMaster;
using FZ4P.DriverIc.I2CBase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.I2CBase
{
    /// <summary>
    /// 해당 I2C 마스터에 어떤 방식으로 Read, Write 할지에 대한 책임을 가진다.
    /// </summary>
    public class I2CControl : IOneBytesDrivingIC, ITwoBytesDrivingIC, IOneTwoBytesDrivingIC
    {
        public readonly Port _i2CMaster = null;
        public readonly Action<string> SetError = null;
        public bool IsVirtual => STATIC.Process.IsVirtual;
        private object I2cLock = new object();

        public I2CControl(Port i2CMaster, Action<string> action)
        {
            _i2CMaster = i2CMaster;
            SetError = action;
        }

        private bool WriteArray(int slaveAddr, int memAddr, int memCnt, byte[] data)
        {
            try
            {
                if (IsVirtual) return true;
                lock (I2cLock)
                {
                    if (_i2CMaster != null) _i2CMaster.Write(slaveAddr, memCnt, memAddr, data);
                }

                return true;
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln WriteFail");
                }
                return false;
            }
        }
        private bool ReadArray(int slaveAddr, int memAddr, int memCnt, byte[] data)
        {

            try
            {
                if (IsVirtual) return true;
                lock (I2cLock)
                {
                    if (_i2CMaster != null) _i2CMaster.Read(slaveAddr, memCnt, memAddr, data);
                }
                return true;
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln ReadFail");

                }
                return false;
            }
        }

        public bool WriteByte(int slaveAddr, int memAddr, int memCnt, byte data)
        {
            byte[] tmp = new byte[] { data };

            try
            {
                lock (I2cLock) { if (_i2CMaster != null) _i2CMaster.Write(slaveAddr, memCnt, memAddr, tmp); }
                return true;
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln WriteFail");
                }
                return false;
            }
        }
        public bool Write2Byte(int slaveAddr, int memAddr, int memCnt, ushort data)
        {
            byte[] tmp = new byte[] { (byte)((data >> 8) & 0xff), (byte)(data & 0xff) };

            try
            {
                lock (I2cLock) { if (_i2CMaster != null) _i2CMaster.Write(slaveAddr, memCnt, memAddr, tmp); }
                return true;
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln WriteFail");
                }
                return false;
            }
        }
        public bool Write2Byte(int slaveAddr, int memAddr, int memCnt, short data)
        {
            byte[] tmp = new byte[] { (byte)((data >> 8) & 0xff), (byte)(data & 0xff) };
            try
            {
                lock (I2cLock) { if (_i2CMaster != null) _i2CMaster.Write(slaveAddr, memCnt, memAddr, tmp); }
                return true;
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln WriteFail");
                }
                return false;
            }
        }
        private bool Write4Byte(int slaveAddr, int memAddr, int memCnt, uint data)
        {
            byte[] tmp = new byte[] { (byte)((data >> 24) & 0xff), (byte)((data >> 16) & 0xff), (byte)((data >> 8) & 0xff), (byte)(data & 0xff) };
            try
            {
                lock (I2cLock) { if (_i2CMaster != null) _i2CMaster.Write(slaveAddr, memCnt, memAddr, tmp); }
                return true;
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln WriteFail");
                }
                return false;
            }
        }
        private bool Write4Byte(int slaveAddr, int memAddr, int memCnt, int data)
        {
            byte[] tmp = new byte[] { (byte)((data >> 24) & 0xff), (byte)((data >> 16) & 0xff), (byte)((data >> 8) & 0xff), (byte)(data & 0xff) };
            try
            {
                lock (I2cLock) { if (_i2CMaster != null) _i2CMaster.Write(slaveAddr, memCnt, memAddr, tmp); }
                return true;
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln WriteFail");
                }
                return false;
            }
        }

        private byte? ReadByteNull(int slaveAddr, int memAddr, int memCnt)
        {
            byte[] tmp = new byte[1];
            try
            {
                lock (I2cLock)
                {
                    if (_i2CMaster != null) _i2CMaster.Read(slaveAddr, memCnt, memAddr, tmp);
                }
                return tmp[0];
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln ReadFail");

                }
                return null;
            }
        }
        public byte ReadByte(int slaveAddr, int memAddr, int memCnt)
        {
            byte[] tmp = new byte[1];
            try
            {
                lock (I2cLock)
                {
                    if (_i2CMaster != null) _i2CMaster.Read(slaveAddr, memCnt, memAddr, tmp);
                }
                return tmp[0];
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln ReadFail");

                }
                return byte.MaxValue;
            }
        }
        public ushort Read2Byte(int slaveAddr, int memAddr, int memCnt)
        {
            byte[] tmp = new byte[2];
            try
            {
                lock (I2cLock)
                {
                    if (_i2CMaster != null) _i2CMaster.Read(slaveAddr, memCnt, memAddr, tmp);
                }
                return (ushort)(tmp[0] << 8 | tmp[1]);
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln ReadFail");

                }
                return ushort.MaxValue;
            }
        }
        public short Read2Byte_signed(int slaveAddr, int memAddr, int memCnt)
        {
            byte[] tmp = new byte[2];
            try
            {
                lock (I2cLock)
                {
                    if (_i2CMaster != null) _i2CMaster.Read(slaveAddr, memCnt, memAddr, tmp);
                }
                return (short)(tmp[0] << 8 | tmp[1]);
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln ReadFail");

                }
                return short.MinValue;
            }
        }
        private uint Read4Byte(int slaveAddr, int memAddr, int memCnt)
        {
            byte[] tmp = new byte[4];
            try
            {
                lock (I2cLock)
                {
                    if (_i2CMaster != null) _i2CMaster.Read(slaveAddr, memCnt, memAddr, tmp);
                }
                return (uint)(tmp[0] << 24 | tmp[1] << 16 | tmp[2] << 8 | tmp[3]);
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln ReadFail");

                }
                return uint.MaxValue;
            }
        }
        private int Read4Byte_signed(int slaveAddr, int memAddr, int memCnt)
        {
            byte[] tmp = new byte[4];
            try
            {
                lock (I2cLock)
                {
                    if (_i2CMaster != null) _i2CMaster.Read(slaveAddr, memCnt, memAddr, tmp);
                }
                return (int)(tmp[0] << 24 | tmp[1] << 16 | tmp[2] << 8 | tmp[3]);
            }
            catch
            {
                STATIC.I2CFailcnt++;
                if (STATIC.I2CFailcnt > 20)
                {
                    SetError("Dln ReadFail");

                }
                return int.MinValue;
            }
        }
    }
}
