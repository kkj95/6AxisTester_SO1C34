using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{
    public static class TestHelper
    {
        public static void Test()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug().WriteTo.Console().WriteTo.File("test/log.txt", rollingInterval: RollingInterval.Day).CreateLogger();
        }
    }
}
