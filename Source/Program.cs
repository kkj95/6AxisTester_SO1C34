using App.CoreModules.Logs.Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FZ4P
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;


            bool isNew;
            Mutex mutex = new Mutex(true, "FZ_Test", out isNew);

            if (isNew)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                try
                {
                    LogHelper.Create();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                Application.Run(new F_Main());

                mutex.ReleaseMutex();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else
            {
                MessageBox.Show("Still Running Process .....");
                Application.Exit();
            }
        }

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            string shortName = new AssemblyName(args.Name).Name;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, shortName + ".dll");
            return File.Exists(path) ? Assembly.LoadFrom(path) : null;
        }
    }
}
