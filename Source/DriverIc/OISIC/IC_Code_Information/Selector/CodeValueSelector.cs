using FZ4P.DriverIc.DongWoon.Params;
using FZ4P.DriverIc.IC_CodeI_nformation;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.IC_Code_Information.Selector
{
    public class CodeValueSelector
    {
        public IICCodeValue GetExcutor(ICBitType type)
        {
            switch (type)
            {
                case ICBitType.TypeBit_12:
                    return new ICCodeBit12();
                case ICBitType.TypeBit_14:
                    return new ICCodeBit14();
                default :
                    return new ICCodeBit12();
            }
        }
    }
}
