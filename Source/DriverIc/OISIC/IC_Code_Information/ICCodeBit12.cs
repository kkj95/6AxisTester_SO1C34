using FZ4P.DriverIc.IC_Code_Information.Params;
using FZ4P.DriverIc.IC_CodeI_nformation;

namespace FZ4P.DriverIc.IC_Code_Information
{
    public class ICCodeBit12 : IICCodeValue
    {
        public ResultCodeValue GetResultCode()
        {
            return new ResultCodeValue()
            {
                MaxCode = 4095,
                MidCode = 2048,
                MinCode = 0,
            };
        }
    }
}
