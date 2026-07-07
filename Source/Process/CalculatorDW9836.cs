using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{
    public class CalculatorDW9836
    {
        private readonly double _maxCurrentValue = 0.0;
        public CalculatorDW9836(double maxCurrentValue)
        {
            _maxCurrentValue = maxCurrentValue;
        }

        public double CalculatorCurrent(int currentCode)
        {
            if (currentCode >= 4096)
            {
                return _maxCurrentValue * ((currentCode - 4096.0) / 4096.0);
            }
                
            else if(currentCode < 4096)
                return -1.0 * (_maxCurrentValue * ((4096.0 - currentCode) / 4096.0));

            throw new ArgumentOutOfRangeException("범위 초과");
        }
    }
}
