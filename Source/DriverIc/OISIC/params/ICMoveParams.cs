using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P.DriverIc.DongWoon.Params
{
    public enum ICBitType
    {
        TypeBit_12 = 1,
        TypeBit_14 = 2,
    }
    public class ICMoveParams
    {
        public int TargetCode { get; set; } = 0;
        public ICBitType MoveType { get; set; } = ICBitType.TypeBit_12;
    }
}
