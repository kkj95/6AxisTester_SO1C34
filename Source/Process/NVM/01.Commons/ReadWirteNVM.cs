using FZ4P.DriverIc.I2CBase.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FZ4P
{
    public class ReadWirteNVM
    {
        private readonly IOneTwoBytesDrivingIC _control;
        private readonly Action<int, string> _addLog = null;
        public ReadWirteNVM(IOneTwoBytesDrivingIC control, Action<int, string> addLog)
        {
            _control = control;
            _addLog = addLog;
        }

        public void SetWrite(int ch,int slaveId,NVMWriteCollection values)
        {
            values.ForEach(
                (element) =>
                {
                    _control.WriteByte(slaveId,element.Address,1, (byte)element.PlayLoad);
                    _addLog(ch, $"Addr : 0x{element.Address.ToString("X2")}, WData : 0x{element.PlayLoad.ToString("X2")}");
                });
        }

        public void GetReadAddress(int ch, int slaveId, NVMWriteCollection readCollection)
        {
            readCollection.ForEach(element => element.PlayLoad = _control.ReadByte(slaveId, element.Address,1));
        }

        public bool CompareData(int ch, NVMWriteCollection sourceWrite, NVMWriteCollection sourceRead)
        {
            bool result = false;
            foreach (var readValue in sourceRead)
            {
                var writeValue = sourceWrite.FirstOrDefault(element => element.PlayLoad == readValue.PlayLoad && element.Address == readValue.Address);

                if (sourceWrite.Any(element => (element.PlayLoad == readValue.PlayLoad) && (element.Address == readValue.Address)))
                {
                    _addLog(ch, $"Addr : 0x{readValue.Address.ToString("X2")}, WData : 0x{writeValue.PlayLoad.ToString("X2")}, RData : 0x{readValue.PlayLoad.ToString("X2")}");
                    return result = true;
                }
                else
                {
                    result = false;
                }
            }    
            return result;
        }
    }
}
