using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FZ4P.Commons.Helper
{
    public static class WindowHelper
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,
            int msg,
            int wParam,
            int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public static void Enable(Control dragControl, Form targetForm)
        {
            dragControl.MouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Left)
                    return;

                // ToolStrip인 경우 아이템 위에서는 드래그 막기
                if (dragControl is ToolStrip toolStrip)
                {
                    var item = toolStrip.GetItemAt(e.Location);

                    if (item != null)
                        return;
                }

                ReleaseCapture();
                SendMessage(
                    targetForm.Handle,
                    WM_NCLBUTTONDOWN,
                    HTCAPTION,
                    0);
            };
        }
    }
}
