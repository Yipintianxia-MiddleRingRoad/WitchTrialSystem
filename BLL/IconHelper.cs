using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WitchTrialSystem.BLL
{
    /// <summary>
    /// 图标管理辅助类
    /// 统一管理应用程序图标
    /// </summary>
    public static class IconHelper
    {
        private static Icon? _appIcon;

        /// <summary>
        /// 获取应用程序图标
        /// </summary>
        public static Icon? AppIcon
        {
            get
            {
                if (_appIcon == null)
                {
                    try
                    {
                        // 尝试加载 ICO 文件
                        string icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "ui", "app_icon.ico");
                        if (File.Exists(icoPath))
                        {
                            _appIcon = new Icon(icoPath);
                        }
                        else
                        {
                            // 如果 ICO 不存在，尝试从 PNG 创建
                            string pngPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "ui", "app_icon.png");
                            if (File.Exists(pngPath))
                            {
                                using (var bitmap = new Bitmap(pngPath))
                                {
                                    _appIcon = Icon.FromHandle(bitmap.GetHicon());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"加载应用图标失败: {ex.Message}");
                    }
                }
                return _appIcon;
            }
        }

        /// <summary>
        /// 为窗体设置应用程序图标
        /// </summary>
        /// <param name="form">要设置图标的窗体</param>
        public static void SetFormIcon(Form form)
        {
            if (form != null && AppIcon != null)
            {
                form.Icon = AppIcon;
            }
        }
    }
}
