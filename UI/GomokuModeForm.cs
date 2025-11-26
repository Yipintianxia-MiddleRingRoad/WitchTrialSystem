using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 五子棋对弈模式选择界面
    /// </summary>
    public class GomokuModeForm : Form
    {
        private readonly string _username;
        
        // 背景容器
        private readonly Panel _bg = new() { Dock = DockStyle.Fill, BackgroundImageLayout = ImageLayout.Stretch };
        
        // 热键按钮区域
        private readonly Panel _btnSingleDevice = new()  // 单设备对弈（左边）
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnMultiDevice = new()  // 多设备对弈（右边）
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnBack = new()  // 返回按钮（右上角X）
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };

        public GomokuModeForm(string username)
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
            Text = $"五子棋 - 选择模式 (当前用户：{_username})";
            
            // 根据图片实际尺寸设置窗体（2560x1435，按比例缩放到高度700）
            Width = 1248;   // 700/1435*2560 ≈ 1248
            Height = 700;
            
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;
            
            // 设置应用程序图标
            BLL.IconHelper.SetFormIcon(this);

            Controls.Add(_bg);
            
            // Esc 键返回
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) GoBack(); };
        }

        /// <summary>
        /// 加载背景图
        /// </summary>
        private void LoadBackground()
        {
            string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "gomoku_mode_bg.png");
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
            // 单设备对弈按钮（左边大块区域）
            _btnSingleDevice.Size = new Size(580, 600);  // 左侧区域
            _btnSingleDevice.Left = 20;
            _btnSingleDevice.Top = 50;
            _bg.Controls.Add(_btnSingleDevice);

            // 多设备对弈按钮（右边大块区域）
            _btnMultiDevice.Size = new Size(580, 600);  // 右侧区域
            _btnMultiDevice.Left = 630;
            _btnMultiDevice.Top = 50;
            _bg.Controls.Add(_btnMultiDevice);

            // 返回按钮（右上角X）
            _btnBack.Size = new Size(60, 60);
            _btnBack.Left = ClientSize.Width - 90;
            _btnBack.Top = 30;
            _bg.Controls.Add(_btnBack);

            // 绑定点击事件
            _btnSingleDevice.Click += OnSingleDeviceClick;
            _btnMultiDevice.Click += OnMultiDeviceClick;
            _btnBack.Click += OnBackClick;

            // 确保按钮在最上层
            _btnSingleDevice.BringToFront();
            _btnMultiDevice.BringToFront();
            _btnBack.BringToFront();
        }

        /// <summary>
        /// 点击单设备对弈
        /// </summary>
        private void OnSingleDeviceClick(object? sender, EventArgs e)
        {
            var boardForm = new GomokuBoardForm(_username, isSingleDevice: true);
            
            // 使用反射检查 _cancelled 字段，判断用户是否取消
            var cancelledField = boardForm.GetType().GetField("_cancelled", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool cancelled = cancelledField != null && (bool)cancelledField.GetValue(boardForm)!;
            
            if (cancelled)
            {
                // 用户取消了，释放窗口并保持当前界面显示
                boardForm.Dispose();
                return;
            }
            
            boardForm.FormClosed += (s, args) => this.Show();  // 棋盘窗口关闭时显示模式选择界面
            this.Hide();
            boardForm.Show();
        }

        /// <summary>
        /// 点击多设备对弈
        /// </summary>
        private void OnMultiDeviceClick(object? sender, EventArgs e)
        {
            MessageBox.Show("多设备对弈功能开发中...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // 未来实现：
            // var boardForm = new GomokuBoardForm(_username, isSingleDevice: false);
            // this.Hide();
            // boardForm.Show();
        }

        /// <summary>
        /// 点击返回按钮
        /// </summary>
        private void OnBackClick(object? sender, EventArgs e)
        {
            GoBack();
        }

        /// <summary>
        /// 返回手机界面（直接关闭，让 PhoneForm 的 FormClosed 事件处理显示）
        /// </summary>
        private void GoBack()
        {
            this.Close();
        }
    }
}
