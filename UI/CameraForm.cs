using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 照相界面
    /// 功能：模拟魔女手机照相功能
    /// </summary>
    public class CameraForm : Form
    {
        #region 字段定义
        
        private readonly string _username;
        
        // 背景容器
        private readonly Panel _bg = new() { Dock = DockStyle.Fill, BackgroundImageLayout = ImageLayout.Stretch };
        
        // 退出按钮
        private readonly Panel _btnExit = new() 
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        #endregion

        #region 构造函数和初始化
        
        /// <summary>
        /// 构造函数：初始化照相界面
        /// </summary>
        /// <param name="username">当前登录的用户名</param>
        public CameraForm(string username)
        {
            _username = username;
            InitializeForm();
            LoadBackground();
            SetupButtons();
        }

        /// <summary>
        /// 初始化窗体设置
        /// </summary>
        private void InitializeForm()
        {
            Text = $"魔女相机 (当前用户：{_username})";
            
            // 根据背景图片比例设置窗体尺寸（960x502px的1.5倍，加上Windows边框）
            Width = 1460;   // 960×1.5 + 20像素边框
            Height = 793;  // 502×1.5 + 40像素边框（包含标题栏）
            
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            
            // 设置应用程序图标
            BLL.IconHelper.SetFormIcon(this);
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;

            Controls.Add(_bg);
            
            // Esc 键返回手机界面
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) ReturnToPhone(); };
        }

        /// <summary>
        /// 加载背景图
        /// </summary>
        private void LoadBackground()
        {
            string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "camera_bg.png");
            if (File.Exists(bgPath))
            {
                _bg.BackgroundImage = Image.FromFile(bgPath);
            }
            else
            {
                _bg.BackColor = Color.FromArgb(30, 30, 40); // 深色背景作为备用
            }
        }

        /// <summary>
        /// 设置按钮热键区域
        /// </summary>
        private void SetupButtons()
        {
            // 退出按钮（右上角X）
            _btnExit.Size = new Size(40, 40);  // 适合小窗口的尺寸
            _btnExit.Left = ClientSize.Width - 60;  // 右上角
            _btnExit.Top = 10;   // 顶部区域
            _bg.Controls.Add(_btnExit);

            // 绑定点击事件
            _btnExit.Click += OnExitClick;

            // 确保按钮在最上层
            _btnExit.BringToFront();
        }
        
        #endregion

        #region 事件处理
        
        /// <summary>
        /// 点击退出按钮：返回手机界面
        /// </summary>
        private void OnExitClick(object? sender, EventArgs e)
        {
            ReturnToPhone();
        }

        /// <summary>
        /// 返回手机界面
        /// </summary>
        private void ReturnToPhone()
        {
            // 直接关闭当前窗口，PhoneForm会自动显示
            this.Close();
        }
        
        #endregion
    }
}
