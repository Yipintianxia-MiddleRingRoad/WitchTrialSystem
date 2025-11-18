// namespace WitchTrialSystem;

// static class Program
// {
//     /// <summary>
//     ///  The main entry point for the application.
//     /// </summary>
//     [STAThread]
//     static void Main()
//     {
//         // To customize application configuration such as set high DPI settings or default font,
//         // see https://aka.ms/applicationconfiguration.
//         ApplicationConfiguration.Initialize();
//         Application.Run(new Form1());
//     }    
// }

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
