using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 处刑界面
    /// 功能：模拟魔女审判的处刑场景
    /// </summary>
    public class ExecutionForm : Form
    {
        #region 字段定义
        
        private readonly string _username;
        private readonly int? _sessionId;  // 审判会话ID（可选）
        private readonly int? _witchId;    // 魔女ID（可选）
        private bool _hasConfirmed = false; // 是否已确认处刑
        
        // 背景容器
        private readonly Panel _bg = new() { Dock = DockStyle.Fill, BackgroundImageLayout = ImageLayout.Stretch };
        
        // 退出按钮
        private readonly Panel _btnExit = new() 
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        // 处刑按钮（中间圆形区域）
        private readonly Panel _btnExecute = new() 
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        #endregion

        #region 构造函数和初始化
        
        /// <summary>
        /// 构造函数：初始化处刑界面（普通模式）
        /// </summary>
        /// <param name="username">当前登录的用户名</param>
        public ExecutionForm(string username) : this(username, null, null)
        {
        }
        
        /// <summary>
        /// 构造函数：初始化处刑界面（审判模式）
        /// </summary>
        /// <param name="username">当前登录的用户名</param>
        /// <param name="sessionId">审判会话ID</param>
        /// <param name="witchId">魔女ID</param>
        public ExecutionForm(string username, int? sessionId, int? witchId)
        {
            _username = username;
            _sessionId = sessionId;
            _witchId = witchId;
            
            InitializeForm();
            LoadBackground();
            SetupButtons();
        }

        /// <summary>
        /// 初始化窗体设置
        /// </summary>
        private void InitializeForm()
        {
            Text = $"魔女处刑 (当前用户：{_username})";
            
            // 与手机界面相同的尺寸
            Width = 450;
            Height = 800;
            
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            
            // 设置应用程序图标
            BLL.IconHelper.SetFormIcon(this);
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;

            Controls.Add(_bg);
            
            // 审判模式下禁用关闭按钮
            if (_sessionId.HasValue && _witchId.HasValue)
            {
                ControlBox = false;
            }
            else
            {
                // Esc 键返回手机界面（仅普通模式）
                KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) ReturnToPhone(); };
            }
        }

        /// <summary>
        /// 加载背景图
        /// </summary>
        private void LoadBackground()
        {
            string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "execution_bg.png");
            if (File.Exists(bgPath))
            {
                _bg.BackgroundImage = Image.FromFile(bgPath);
            }
            else
            {
                _bg.BackColor = Color.FromArgb(20, 20, 30); // 深色背景作为备用
            }
        }

        /// <summary>
        /// 设置按钮热键区域
        /// </summary>
        private void SetupButtons()
        {
            // 退出按钮（右上角X）
            _btnExit.Size = new Size(50, 50);
            _btnExit.Left = ClientSize.Width - 80;  // 右上角
            _btnExit.Top = 30;   // 状态栏区域
            _bg.Controls.Add(_btnExit);

            // 处刑按钮（中间圆形区域）
            _btnExecute.Size = new Size(120, 120);  // 圆形按钮大小
            _btnExecute.Left = (ClientSize.Width - _btnExecute.Width) / 2;  // 水平居中
            _btnExecute.Top = (ClientSize.Height - _btnExecute.Height) / 2;  // 垂直居中
            _bg.Controls.Add(_btnExecute);

            // 绑定点击事件
            _btnExit.Click += OnExitClick;
            _btnExecute.Click += OnExecuteClick;

            // 确保按钮在最上层
            _btnExit.BringToFront();
            _btnExecute.BringToFront();
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
        /// 点击处刑按钮：执行处刑
        /// </summary>
        private void OnExecuteClick(object? sender, EventArgs e)
        {
            ExecuteWitch();
        }

        /// <summary>
        /// 执行处刑
        /// </summary>
        private async void ExecuteWitch()
        {
            try
            {
                // 立刻更换背景到处刑完成图片
                LoadExecutionCompleteBackground();
                
                // 强制刷新UI，确保图片立即显示
                Application.DoEvents();
                
                // 异步等待1秒，不阻塞UI
                await System.Threading.Tasks.Task.Delay(1000);
                
                // 如果是审判模式，更新确认状态
                if (_sessionId.HasValue && _witchId.HasValue)
                {
                    var confirmResult = WitchTrialSystem.BLL.TrialVotingService.ConfirmExecution(_sessionId.Value, _witchId.Value);
                    
                    if (confirmResult.Success)
                    {
                        _hasConfirmed = true;
                        MessageBox.Show("处刑确认成功！", "处刑完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // 关闭窗口
                        Close();
                    }
                    else
                    {
                        MessageBox.Show($"确认失败：{confirmResult.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // 普通模式
                    MessageBox.Show("处刑成功！", "处刑完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处刑过程出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 加载处刑完成后的背景图
        /// </summary>
        private void LoadExecutionCompleteBackground()
        {
            string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "execution_complete.png");
            if (File.Exists(bgPath))
            {
                _bg.BackgroundImage = Image.FromFile(bgPath);
                _bg.Invalidate(); // 强制重绘
            }
            else
            {
                // 如果没有找到完成图片，保持原背景
                _bg.Invalidate();
            }
        }

        /// <summary>
        /// 返回手机界面
        /// </summary>
        private void ReturnToPhone()
        {
            // 直接关闭当前窗口，PhoneForm会自动显示
            this.Close();
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 审判模式下，未确认处刑不能关闭
            if (_sessionId.HasValue && _witchId.HasValue && !_hasConfirmed && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                MessageBox.Show("请先点击处刑按钮！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            base.OnFormClosing(e);
        }
        
        #endregion
    }
}
