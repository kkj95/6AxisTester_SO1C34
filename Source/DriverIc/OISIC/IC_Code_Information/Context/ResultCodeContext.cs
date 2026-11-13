using FZ4P.DriverIc.DongWoon.Params;
using FZ4P.DriverIc.IC_Code_Information.Params;
using FZ4P.DriverIc.IC_Code_Information.Selector;
using FZ4P.DriverIc.IC_CodeI_nformation;

namespace FZ4P.DriverIc.IC_Code_Information.Context
{
    public class ResultCodeContext
    {
        IICCodeValue excutor = null;
        public ResultCodeValue GetCodeValue(ICBitType type)
        {
            excutor = new CodeValueSelector().GetExcutor(type);
            return excutor?.GetResultCode();
        }
    }
}
