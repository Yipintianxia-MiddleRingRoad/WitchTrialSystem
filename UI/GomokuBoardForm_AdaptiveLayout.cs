using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using WitchTrialSystem.BLL;
using WitchTrialSystem.DAL;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 五子棋棋盘界面（自适应布局版本）
    /// 此文件为GomokuBoardForm的自适应布局重构示例
    /// </summary>
    public class GomokuBoardForm_AdaptiveLayout : Form
    {
        // 基本布局比例和基准尺寸
        private Size _baseSize = new Size(2403, 1387); // 原始设计尺寸
        private float _scaleX = 1f; // X轴缩放比例
        private float _scaleY = 1f; // Y轴缩放比例
        
        // 棋盘绘制参数（基于比例计算）
        private int BOARD_LEFT => (int)(188 * _scaleX);   // 左上角交叉点X
        private int BOARD_TOP => (int)(155 * _scaleY);    // 左上角交叉点Y
        private int CELL_SIZE_X => (int)(75 * _scaleX);   // 横向格子大小
        private int CELL_SIZE_Y => (int)(75 * _scaleY);   // 纵向格子大小
        private const int BOARD_SIZE = 15;
        
        // UI 控件
        private readonly Panel _bg = new() { Dock = DockStyle.Fill, BackgroundImageLayout = ImageLayout.Stretch }; // 改为Stretch以适应窗口大小
        private readonly Panel _boardPanel = new() { BackColor = Color.Transparent };
        private readonly Panel _btnBack = new() { BackColor = Color.Transparent, Cursor = Cursors.Hand };
        private readonly Panel _btnUndo = new() { BackColor = Color.Transparent, Cursor = Cursors.Hand }; // 魔法（悔棋）
        private readonly Panel _btnSurrender = new() { BackColor = Color.Transparent, Cursor = Cursors.Hand }; // 疑问（认输）
        private readonly Panel _btnDraw = new() { BackColor = Color.Transparent, Cursor = Cursors.Hand }; // 伪证（和棋）
        
        // 头像显示
        private readonly PictureBox _picPlayer1Avatar = new() { BackColor = Color.Transparent, SizeMode = PictureBoxSizeMode.Zoom };
        private readonly PictureBox _picPlayer2Avatar = new() { BackColor = Color.Transparent, SizeMode = PictureBoxSizeMode.Zoom };
        
        // 容器面板 - 用于组织控件布局
        private readonly Panel _panelPlayer1Info = new() { BackColor = Color.Transparent, Anchor = AnchorStyles.Top | AnchorStyles.Right };
        private readonly Panel _panelPlayer2Info = new() { BackColor = Color.Transparent, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
        
        // 玩家信息标签
        private readonly Label _lblPlayer1Name = new() { BackColor = Color.Transparent, ForeColor = Color.FromArgb(27, 16, 13) };
        private readonly Label _lblPlayer1Score = new() { BackColor = Color.Transparent, ForeColor = Color.FromArgb(27, 16, 13) };
        private readonly Label _lblPlayer2Name = new() { BackColor = Color.Transparent, ForeColor = Color.FromArgb(27, 16, 13) };
        private readonly Label _lblPlayer2Score = new() { BackColor = Color.Transparent, ForeColor = Color.FromArgb(27, 16, 13) };
        
        // 计时器标签
        private readonly Label _lblPlayer1StepTime = new() { BackColor = Color.Transparent, ForeColor = Color.White };
        private readonly Label _lblPlayer1GameTime = new() { BackColor = Color.Transparent, ForeColor = Color.White };
        private readonly Label _lblPlayer2StepTime = new() { BackColor = Color.Transparent, ForeColor = Color.White };
        private readonly Label _lblPlayer2GameTime = new() { BackColor = Color.Transparent, ForeColor = Color.White };

        // 构造函数
        public GomokuBoardForm_AdaptiveLayout(string username, bool isSingleDevice)
        {
            // 基本设置
            Text = "五子棋对弈（自适应布局）";
            BLL.IconHelper.SetFormIcon(this);
            Width = _baseSize.Width;
            Height = _baseSize.Height;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable; // 可调整大小
            MaximizeBox = true;
            MinimizeBox = true;
            DoubleBuffered = true;
            KeyPreview = true;
            
            // 注册Resize事件处理器
            Resize += OnFormResize;
            
            // 初始化布局
            InitializeLayout();
        }
        
        /// <summary>
        /// 初始化布局
        /// </summary>
        private void InitializeLayout()
        {
            // 添加背景面板
            Controls.Add(_bg);
            
            // 加载背景图
            LoadBackground();
            
            // 初始化所有控件
            InitializeControls();
            
            // 计算初始缩放比例
            CalculateScaleFactors();
            
            // 应用布局
            ApplyLayout();
        }
        
        /// <summary>
        /// 计算缩放比例
        /// </summary>
        private void CalculateScaleFactors()
        {
            _scaleX = (float)ClientSize.Width / _baseSize.Width;
            _scaleY = (float)ClientSize.Height / _baseSize.Height;
        }
        
        /// <summary>
        /// 初始化所有控件
        /// </summary>
        private void InitializeControls()
        {
            // 棋盘面板
            _boardPanel.Paint += DrawBoard;
            _boardPanel.MouseClick += OnBoardClick;
            
            // 按钮事件
            _btnBack.Click += OnBackClick;
            _btnUndo.Click += OnUndoClick;
            _btnSurrender.Click += OnSurrenderClick;
            _btnDraw.Click += OnDrawClick;
            
            // 添加控件到背景面板
            _bg.Controls.Add(_boardPanel);
            _bg.Controls.Add(_btnBack);
            _bg.Controls.Add(_btnUndo);
            _bg.Controls.Add(_btnSurrender);
            _bg.Controls.Add(_btnDraw);
            _bg.Controls.Add(_picPlayer1Avatar);
            _bg.Controls.Add(_picPlayer2Avatar);
            _bg.Controls.Add(_panelPlayer1Info);
            _bg.Controls.Add(_panelPlayer2Info);
            _bg.Controls.Add(_lblPlayer1Name);
            _bg.Controls.Add(_lblPlayer1Score);
            _bg.Controls.Add(_lblPlayer2Name);
            _bg.Controls.Add(_lblPlayer2Score);
            _bg.Controls.Add(_lblPlayer1StepTime);
            _bg.Controls.Add(_lblPlayer1GameTime);
            _bg.Controls.Add(_lblPlayer2StepTime);
            _bg.Controls.Add(_lblPlayer2GameTime);
        }
        
        /// <summary>
        /// 应用布局 - 根据缩放比例调整控件位置和大小
        /// </summary>
        private void ApplyLayout()
        {
            // 棋盘面板
            _boardPanel.Location = new Point(BOARD_LEFT - (int)(20 * _scaleX), BOARD_TOP - (int)(20 * _scaleY));
            _boardPanel.Size = new Size(CELL_SIZE_X * (BOARD_SIZE - 1) + (int)(60 * _scaleX), CELL_SIZE_Y * (BOARD_SIZE - 1) + (int)(60 * _scaleY));
            
            // 返回按钮 - 固定在右上角
            _btnBack.Size = new Size((int)(180 * _scaleX), (int)(120 * _scaleY));
            _btnBack.Location = new Point(ClientSize.Width - (int)(220 * _scaleX), (int)(40 * _scaleY));
            
            // 底部按钮区域 - 水平居中
            int buttonWidth = (int)(150 * _scaleX);
            int buttonHeight = (int)(80 * _scaleY);
            int buttonSpacing = (int)(30 * _scaleX);
            int totalButtonWidth = buttonWidth * 3 + buttonSpacing * 2;
            int buttonAreaLeft = (ClientSize.Width - totalButtonWidth) / 2;
            int buttonAreaTop = ClientSize.Height - (int)(200 * _scaleY);
            
            _btnUndo.Size = new Size(buttonWidth, buttonHeight);
            _btnUndo.Location = new Point(buttonAreaLeft, buttonAreaTop);
            
            _btnSurrender.Size = new Size(buttonWidth, buttonHeight);
            _btnSurrender.Location = new Point(buttonAreaLeft + buttonWidth + buttonSpacing, buttonAreaTop);
            
            _btnDraw.Size = new Size(buttonWidth, buttonHeight);
            _btnDraw.Location = new Point(buttonAreaLeft + (buttonWidth + buttonSpacing) * 2, buttonAreaTop);
            
            // 调整字体大小
            float baseFontSize = 16;
            float scaledFontSize = baseFontSize * Math.Min(_scaleX, _scaleY);
            Font scaledFont = new Font("微软雅黑", scaledFontSize, FontStyle.Bold);
            
            _lblPlayer1Name.Font = scaledFont;
            _lblPlayer1Score.Font = scaledFont;
            _lblPlayer2Name.Font = scaledFont;
            _lblPlayer2Score.Font = scaledFont;
            
            // 其他控件的位置和大小调整...
            // 这里只是示例，实际应用中需要根据具体布局需求调整
        }
        
        /// <summary>
        /// 窗口大小改变事件处理
        /// </summary>
        private void OnFormResize(object? sender, EventArgs e)
        {
            CalculateScaleFactors();
            ApplyLayout();
        }
        
        /// <summary>
        /// 加载背景图
        /// </summary>
        private void LoadBackground()
        {
            try
            {
                string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "gomoku_board_bg.png");
                if (File.Exists(bgPath))
                {
                    _bg.BackgroundImage = Image.FromFile(bgPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 加载背景图失败: {ex.Message}");
            }
        }
        
        // 以下是原有的事件处理方法
        // 这里只是示例，实际应用中需要从原文件复制相应方法
        
        private void DrawBoard(object? sender, PaintEventArgs e)
        {
            // 绘制棋盘的逻辑
        }
        
        private void OnBoardClick(object? sender, MouseEventArgs e)
        {
            // 棋盘点击处理逻辑
        }
        
        private void OnBackClick(object? sender, EventArgs e)
        {
            // 返回按钮处理逻辑
            this.Close();
        }
        
        private void OnUndoClick(object? sender, EventArgs e)
        {
            // 悔棋按钮处理逻辑
        }
        
        private void OnSurrenderClick(object? sender, EventArgs e)
        {
            // 认输按钮处理逻辑
        }
        
        private void OnDrawClick(object? sender, EventArgs e)
        {
            // 和棋按钮处理逻辑
        }
    }
}