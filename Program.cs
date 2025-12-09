using System;
using System.Windows.Forms;
using WitchTrialSystem.UI;
using WitchTrialSystem.BLL;

namespace WitchTrialSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            // 预加载应用程序图标
            IconHelper.GetAppIcon();
            
            Application.Run(new LoginForm());  // ← 先登录
        }
    }
}
