using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 魔女手机界面
    /// 功能：模拟手机主屏幕，提供魔女图鉴APP入口
    /// </summary>
    public class PhoneForm : Form
    {
        #region 字段定义
        
        private readonly string _username;
        
        // 背景容器
        private readonly Panel _bg = new() { Dock = DockStyle.Fill, BackgroundImageLayout = ImageLayout.Stretch };
        
        // 热键按钮区域
        private readonly Panel _btnPokedex = new() 
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnExit = new() 
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        #endregion

        #region 构造函数和初始化
        
        /// <summary>
        /// 构造函数：初始化手机界面
        /// </summary>
        /// <param name="username">当前登录的用户名</param>
        public PhoneForm(string username)
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
            Text = $"魔女手机 (当前用户：{_username})";
            
            // 根据图片比例设置窗体尺寸（手机屏幕比例）
            Width = 450;
            Height = 800;
            
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;

            Controls.Add(_bg);
            
            // Esc 键退出
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) DoLogout(); };
        }

        /// <summary>
        /// 加载背景图
        /// </summary>
        private void LoadBackground()
        {
            string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "phone_bg.png");
            if (File.Exists(bgPath))
            {
                _bg.BackgroundImage = Image.FromFile(bgPath);
            }
            else
            {
                _bg.BackColor = Color.FromArgb(40, 40, 40); // 深色背景作为备用
            }
        }

        /// <summary>
        /// 设置按钮热键区域
        /// </summary>
        private void SetupButtons()
        {
            // 魔女图鉴APP按钮（左上角，根据图片估算位置）
            _btnPokedex.Size = new Size(120, 140);  // 包含图标和文字的区域
            _btnPokedex.Left = 50;   // 左上角位置
            _btnPokedex.Top = 120;   // 状态栏下方
            _bg.Controls.Add(_btnPokedex);

            // 退出按钮（右上角X）
            _btnExit.Size = new Size(50, 50);
            _btnExit.Left = ClientSize.Width - 80;  // 右上角
            _btnExit.Top = 30;   // 状态栏区域
            _bg.Controls.Add(_btnExit);

            // 绑定点击事件
            _btnPokedex.Click += OnPokedexClick;
            _btnExit.Click += OnExitClick;

            // 确保按钮在最上层
            _btnPokedex.BringToFront();
            _btnExit.BringToFront();
        }
        
        #endregion

        #region 事件处理
        
        /// <summary>
        /// 点击魔女图鉴APP：跳转到图鉴·人物界面
        /// </summary>
        private void OnPokedexClick(object? sender, EventArgs e)
        {
            var pokedexForm = new PokedexForm(_username);
            // 不设置 FormClosed 事件，让图鉴页面自己处理退出逻辑
            this.Hide();
            pokedexForm.Show();
        }

        /// <summary>
        /// 点击退出按钮
        /// </summary>
        private void OnExitClick(object? sender, EventArgs e)
        {
            DoLogout();
        }

        /// <summary>
        /// 退出到登录界面
        /// </summary>
        private void DoLogout()
        {
            var result = MessageBox.Show("确定要退出登录吗？", "退出登录",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var login = new LoginForm();
                login.Show();
                this.Close();
            }
        }
        
        #endregion
    }
}