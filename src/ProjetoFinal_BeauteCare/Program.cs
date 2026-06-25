using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ProjetoFinal
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

       
        [STAThread] //melhorar a qualidade
        static void Main()
        {
            try { SetProcessDPIAware(); } catch { }//melhorrar a qualidade
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormLogin());//fazer com que o login corra
        }
    }
}
