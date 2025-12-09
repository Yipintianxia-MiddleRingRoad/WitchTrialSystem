using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WitchTrialSystem.BLL
{
    /// <summary>
    /// 图标帮助类：统一管理应用程序图标
    /// </summary>
    public static class IconHelper
    {
        private static Icon? _appIcon;

        /// <summary>
        /// 获取应用程序图标
        /// </summary>
        public static Icon? GetAppIcon()
        {
            if (_appIcon != null)
                return _appIcon;

            try
            {
                // 尝试加载 .ico 文件
                string iconPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "app_icon.ico");
                if (File.Exists(iconPath))
                {
                    _appIcon = new Icon(iconPath);
                    return _appIcon;
                }

                // 如果 .ico 不存在，尝试从 .png 转换
                string pngPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "app_icon.png");
                if (File.Exists(pngPath))
                {
                    using var bitmap = new Bitmap(pngPath);
                    IntPtr hIcon = bitmap.GetHicon();
                    _appIcon = Icon.FromHandle(hIcon);
                    return _appIcon;
                }
            }
            catch
            {
                // 加载失败，返回 null
            }

            return null;
        }

        /// <summary>
        /// 为窗体设置应用程序图标
        /// </summary>
        public static void SetFormIcon(Form form)
        {
            if (form == null) return;

            var icon = GetAppIcon();
            if (icon != null)
            {
                form.Icon = icon;
            }
        }
    }
}
