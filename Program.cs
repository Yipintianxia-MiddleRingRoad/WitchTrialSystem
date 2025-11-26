using System;
using System.Windows.Forms;
using WitchTrialSystem.UI;

namespace WitchTrialSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());  // ← 先登录
        }
    }
}
