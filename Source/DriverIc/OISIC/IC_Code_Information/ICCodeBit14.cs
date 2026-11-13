using FZ4P.DriverIc.IC_Code_Information.Params;
using FZ4P.DriverIc.IC_CodeI_nformation;

namespace FZ4P.DriverIc.IC_Code_Information
{
    public class ICCodeBit14 : IICCodeValue
    {
        public ResultCodeValue GetResultCode()
        {
            return new ResultCodeValue()
            {
                MaxCode = 16383,
                MidCode = 8192,
                MinCode = 0,
            };
        }
    }
}
