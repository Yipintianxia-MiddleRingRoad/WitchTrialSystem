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
    /// 五子棋棋盘界面
    /// </summary>
    public class GomokuBoardForm : Form
    {
        private readonly string _player1Username;
        private string _player2Username = "";
        private readonly bool _isSingleDevice;
        
        // 玩家信息
        private string _player1Name = "";
        private string _player2Name = "";
        private int _player1Score = 0;
        private int _player2Score = 0;
        private Image? _player1Avatar;
        private Image? _player2Avatar;
        
        // 自定义字体
        private readonly System.Drawing.Text.PrivateFontCollection _fontCollection = new();
        private Font? _customFont;
        
        // 游戏状态
        private int[,] _board = new int[15, 15]; // 0=空, 1=黑子, 2=白子
        private List<Point> _moveHistory = new(); // 落子历史
        private int _currentPlayer = 1; // 1=黑子先手, 2=白子
        private bool _gameOver = false;
        private DateTime _gameStartTime; // 对局开始时间
        
        // 计时器（倒计时模式，13ms间隔用于显示毫秒）
        private System.Windows.Forms.Timer _timer = new() { Interval = 13 };  // 13ms
        private int _player1StepTime = 60000; // 步时（毫秒，倒计时）
        private int _player2StepTime = 60000;
        private int _player1GameTime = 600000; // 局时（毫秒，10分钟倒计时）
        private int _player2GameTime = 600000;
        
        // 棋盘绘制参数（根据实际坐标计算）
        private const int BOARD_LEFT = 188;   // 左上角交叉点X
        private const int BOARD_TOP = 155;    // 左上角交叉点Y
        private const int CELL_SIZE_X = 75;   // 横向格子大小 (1325-182)/14，自己略做调整
        private const int CELL_SIZE_Y = 75;   // 纵向格子大小 (1321-157)/14，自己略做调整
        private const int BOARD_SIZE = 15;
        
        // UI 控件
        private readonly Panel _bg = new() { Dock = DockStyle.Fill, BackgroundImageLayout = ImageLayout.None };
        private readonly Panel _boardPanel = new() { BackColor = Color.Transparent };
        private readonly Panel _btnBack = new() { BackColor = Color.Transparent, Cursor = Cursors.Hand };
        private readonly Panel _btnUndo = new() { BackColor = Color.Transparent, Cursor = Cursors.Hand }; // 魔法（悔棋）
        private readonly Panel _btnDraw = new() { BackColor = Color.Transparent, Cursor = Cursors.Hand }; // 伪证（和棋）
        private readonly Panel _btnSurrender = new() { BackColor = Color.Transparent, Cursor = Cursors.Hand }; // 疑问（认输）
        
        // 头像显示
        private readonly PictureBox _picPlayer1Avatar = new() { BackColor = Color.Transparent, SizeMode = PictureBoxSizeMode.Zoom };
        private readonly PictureBox _picPlayer2Avatar = new() { BackColor = Color.Transparent, SizeMode = PictureBoxSizeMode.Zoom };
        
        // 容器面板
        private readonly Panel _panelPlayer1Info = new() { BackColor = Color.Transparent };  // 上方玩家信息容器
        private readonly Panel _panelPlayer2Info = new() { BackColor = Color.Transparent };  // 下方玩家信息容器
        
        // 玩家信息标签（字体颜色 RGB(27, 16, 13)，字号调大）
        private readonly Label _lblPlayer1Name = new() { BackColor = Color.Transparent, ForeColor = Color.FromArgb(27, 16, 13), Font = new Font("微软雅黑", 16, FontStyle.Bold) };
        private readonly Label _lblPlayer1Score = new() { BackColor = Color.Transparent, ForeColor = Color.FromArgb(27, 16, 13), Font = new Font("微软雅黑", 16, FontStyle.Bold) };
        private readonly Label _lblPlayer2Name = new() { BackColor = Color.Transparent, ForeColor = Color.FromArgb(27, 16, 13), Font = new Font("微软雅黑", 16, FontStyle.Bold) };
        private readonly Label _lblPlayer2Score = new() { BackColor = Color.Transparent, ForeColor = Color.FromArgb(27, 16, 13), Font = new Font("微软雅黑", 16, FontStyle.Bold) };
        
        // 计时器标签（字号再调大到18）
        private readonly Label _lblPlayer1StepTime = new() { BackColor = Color.Transparent, ForeColor = Color.White, Font = new Font("微软雅黑", 18) };
        private readonly Label _lblPlayer1GameTime = new() { BackColor = Color.Transparent, ForeColor = Color.White, Font = new Font("微软雅黑", 18) };
        private readonly Label _lblPlayer2StepTime = new() { BackColor = Color.Transparent, ForeColor = Color.White, Font = new Font("微软雅黑", 18) };
        private readonly Label _lblPlayer2GameTime = new() { BackColor = Color.Transparent, ForeColor = Color.White, Font = new Font("微软雅黑", 18) };

        private bool _cancelled = false; // 标记用户是否取消
        
        public GomokuBoardForm(string username, bool isSingleDevice)
        {
            _player1Username = username;
            _isSingleDevice = isSingleDevice;
            
            InitializeForm();
            LoadCustomFont(); // 加载自定义字体
            LoadBackground();
            
            if (_isSingleDevice)
            {
                // 选择对手并验证密码，如果取消则不继续初始化
                if (!SelectOpponent())
                {
                    _cancelled = true;
                    return; // 用户取消，直接返回
                }
            }
            
            LoadPlayerInfo();
            SetupUI();
            ApplyCustomFont(); // 应用自定义字体
            StartGame();
        }

        private void InitializeForm()
        {
            Text = "五子棋对弈";
            Width = 2403;
            Height = 1387;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable; // 恢复窗体边框
            MaximizeBox = true;
            MinimizeBox = true;
            DoubleBuffered = true;
            KeyPreview = true;
            
            Controls.Add(_bg);
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) OnBackClick(null, EventArgs.Empty); };
        }

        private void LoadCustomFont()
        {
            try
            {
                string fontPath = Path.Combine(AppContext.BaseDirectory, "Fonts", "方正小标宋简.ttf");
                
                if (File.Exists(fontPath))
                {
                    _fontCollection.AddFontFile(fontPath);
                    if (_fontCollection.Families.Length > 0)
                    {
                        _customFont = new Font(_fontCollection.Families[0], 16, FontStyle.Bold);
                        Console.WriteLine($"✅ 成功加载字体: {_fontCollection.Families[0].Name}");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ 字体文件不存在: {fontPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 加载字体失败: {ex.Message}");
            }
        }

        private void ApplyCustomFont()
        {
            if (_customFont != null)
            {
                // 应用到玩家姓名和积分标签
                _lblPlayer1Name.Font = _customFont;
                _lblPlayer1Score.Font = _customFont;
                _lblPlayer2Name.Font = _customFont;
                _lblPlayer2Score.Font = _customFont;
                
                // 应用到计时器标签（稍大一点）
                var timerFont = new Font(_fontCollection.Families[0], 18, FontStyle.Regular);
                _lblPlayer1StepTime.Font = timerFont;
                _lblPlayer1GameTime.Font = timerFont;
                _lblPlayer2StepTime.Font = timerFont;
                _lblPlayer2GameTime.Font = timerFont;
            }
        }

        private void LoadBackground()
        {
            string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "gomoku_board_bg.png");
            if (File.Exists(bgPath))
            {
                _bg.BackgroundImage = Image.FromFile(bgPath);
            }
            else
            {
                _bg.BackColor = Color.FromArgb(40, 35, 30);
            }
        }

        /// <summary>
        /// 选择对手并验证密码（单设备模式）
        /// </summary>
        /// <returns>是否成功选择对手</returns>
        private bool SelectOpponent()
        {
            // 从数据库获取所有魔女用户（关联 Witch 表获取中文名）
            const string sql = @"
SELECT u.Username, ISNULL(w.Name, u.Username) AS DisplayName, u.Salt, u.PasswordHash
FROM wt.[User] u
LEFT JOIN wt.UserWitch uw ON uw.UserID = u.UserID
LEFT JOIN wt.Witch w ON w.WitchID = uw.WitchID
WHERE u.RoleID = 4 AND u.Username != @u";
            var dt = DBHelper.ExecDataTable(sql, new Microsoft.Data.SqlClient.SqlParameter("@u", _player1Username));
            
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("系统中没有其他魔女可以对弈！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            // 选择对手对话框
            using var selectForm = new Form
            {
                Text = "选择对手",
                Width = 400,
                Height = 300,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };
            
            var lblPrompt = new Label
            {
                Text = "请选择对弈对手：",
                Left = 20,
                Top = 20,
                Width = 350,
                Font = new Font("微软雅黑", 10)
            };
            
            var cmbOpponents = new ComboBox
            {
                Left = 20,
                Top = 50,
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("微软雅黑", 10)
            };
            
            foreach (System.Data.DataRow row in dt.Rows)
            {
                string username = row["Username"].ToString() ?? "";
                string displayName = row["DisplayName"].ToString() ?? "";
                cmbOpponents.Items.Add($"{username} ({displayName})");
            }
            cmbOpponents.SelectedIndex = 0;
            
            var lblPassword = new Label
            {
                Text = "请输入对手密码进行验证：",
                Left = 20,
                Top = 90,
                Width = 350,
                Font = new Font("微软雅黑", 10)
            };
            
            var txtPassword = new TextBox
            {
                Left = 20,
                Top = 120,
                Width = 350,
                PasswordChar = '*',
                Font = new Font("微软雅黑", 10)
            };
            
            var btnConfirm = new Button
            {
                Text = "确认",
                Left = 120,
                Top = 180,
                Width = 80,
                Height = 35,
                Font = new Font("微软雅黑", 10)
            };
            
            var btnCancel = new Button
            {
                Text = "取消",
                Left = 220,
                Top = 180,
                Width = 80,
                Height = 35,
                Font = new Font("微软雅黑", 10)
            };
            
            bool confirmed = false;
            
            btnConfirm.Click += (s, e) =>
            {
                var selectedIndex = cmbOpponents.SelectedIndex;
                var row = dt.Rows[selectedIndex];
                var opponentUsername = row["Username"].ToString() ?? "";
                var salt = row["Salt"].ToString() ?? "";
                var hash = row["PasswordHash"].ToString() ?? "";
                var password = txtPassword.Text.Trim();
                
                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("请输入密码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // 验证密码
                if (Security.Verify(password, salt, hash))
                {
                    _player2Username = opponentUsername;
                    confirmed = true;
                    selectForm.DialogResult = DialogResult.OK;
                    selectForm.Close();
                }
                else
                {
                    MessageBox.Show("密码错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            };
            
            btnCancel.Click += (s, e) =>
            {
                selectForm.DialogResult = DialogResult.Cancel;
                selectForm.Close();
            };
            
            selectForm.Controls.AddRange(new Control[] { lblPrompt, cmbOpponents, lblPassword, txtPassword, btnConfirm, btnCancel });
            
            var result = selectForm.ShowDialog();
            if (result != DialogResult.OK || !confirmed)
            {
                // 用户取消
                return false;
            }
            
            return true;
        }


        /// <summary>
        /// 加载玩家信息
        /// </summary>
        private void LoadPlayerInfo()
        {
            var profileDAL = new UserProfileDAL();
            
            // 玩家1信息
            var profile1 = profileDAL.GetUserProfile(_player1Username);
            if (profile1 != null)
            {
                _player1Name = profile1.CnName ?? profile1.Username;
                _player1Score = profile1.GomokuScore;
                _player1Avatar = LoadAvatar(profile1.CharacterImage);
            }
            
            // 玩家2信息
            if (!string.IsNullOrEmpty(_player2Username))
            {
                var profile2 = profileDAL.GetUserProfile(_player2Username);
                if (profile2 != null)
                {
                    _player2Name = profile2.CnName ?? profile2.Username;
                    _player2Score = profile2.GomokuScore;
                    _player2Avatar = LoadAvatar(profile2.CharacterImage);
                }
            }
        }

        /// <summary>
        /// 加载头像图片（参考图鉴界面的实现）
        /// </summary>
        private Image? LoadAvatar(string? imagePath)
        {
            string defaultPlaceholder = Path.Combine(AppContext.BaseDirectory, "Images", "_placeholder.png");

            if (string.IsNullOrWhiteSpace(imagePath))
                return File.Exists(defaultPlaceholder) ? Image.FromFile(defaultPlaceholder) : null;

            string fullPath = Path.IsPathRooted(imagePath) ? imagePath : Path.Combine(AppContext.BaseDirectory, imagePath);
            return File.Exists(fullPath) ? Image.FromFile(fullPath) : 
                   File.Exists(defaultPlaceholder) ? Image.FromFile(defaultPlaceholder) : null;
        }

        /// <summary>
        /// 设置UI控件
        /// </summary>
        private void SetupUI()
        {
            // 棋盘面板（不绘制网格，使用背景图的棋盘）
            _boardPanel.Location = new Point(BOARD_LEFT - 20, BOARD_TOP - 20);
            _boardPanel.Size = new Size(CELL_SIZE_X * (BOARD_SIZE - 1) + 60, CELL_SIZE_Y * (BOARD_SIZE - 1) + 60);
            _boardPanel.BackColor = Color.Transparent;
            _boardPanel.Paint += DrawBoard;
            _boardPanel.MouseClick += OnBoardClick;
            _bg.Controls.Add(_boardPanel);
            
            // 返回按钮（右上角X，横向3倍，纵向2倍，往左往下移动40像素）
            _btnBack.Size = new Size(180, 120);  // 60*3, 60*2
            _btnBack.Location = new Point(2150, 40);  // 2190-40=2150, 0+40=40
            _btnBack.Click += OnBackClick;
            _bg.Controls.Add(_btnBack);
            
            // 魔法按钮（悔棋）- 向上移动更多
            _btnUndo.Size = new Size(150, 80);  // 扩大热键区域
            _btnUndo.Location = new Point(1580, 680);  // 向上移动到680
            _btnUndo.Click += OnUndoClick;
            _bg.Controls.Add(_btnUndo);
            
            // 伪证按钮（和棋/求和）- 在魔法按钮右边，再往右移动
            _btnDraw.Size = new Size(150, 80);  // 扩大热键区域
            _btnDraw.Location = new Point(1850, 680);  // 往右移动到1850
            _btnDraw.Click += OnDrawClick;
            _bg.Controls.Add(_btnDraw);
            
            // 疑问按钮（认输）- 在魔法和伪证按钮中间
            _btnSurrender.Size = new Size(150, 80);  // 扩大热键区域
            _btnSurrender.Location = new Point(1715, 680);  // 中间位置：(1580+1850)/2=1715
            _btnSurrender.Click += OnSurrenderClick;
            _bg.Controls.Add(_btnSurrender);
            
            // ========== 下方玩家（当前用户 Player1）头像 ==========
            // 原位置中心 (1604, 1114)，向左72，向上123，再向下20，再向下15，放大到240x240
            _picPlayer1Avatar.Size = new Size(240, 240);
            _picPlayer1Avatar.Location = new Point(1604 - 120 - 72, 1114 - 120 - 123 + 20 + 15);
            if (_player1Avatar != null)
                _picPlayer1Avatar.Image = _player1Avatar;
            _bg.Controls.Add(_picPlayer1Avatar);
            
            // ========== 下方玩家步时 ==========
            // 原位置 (2051, 1076)，向左72，向上123，再向上4，再向上5
            _lblPlayer1StepTime.Location = new Point(2051 - 72, 1076 - 123 - 4 - 5);
            _lblPlayer1StepTime.Size = new Size(200, 50);  // 增加高度以适应更大字号
            _lblPlayer1StepTime.Text = "01:00:000";
            _lblPlayer1StepTime.TextAlign = ContentAlignment.MiddleLeft;
            _bg.Controls.Add(_lblPlayer1StepTime);
            
            // ========== 下方玩家局时 ==========
            // 原位置 (2051, 1208)，向左72，向上123，再向上5
            _lblPlayer1GameTime.Location = new Point(2051 - 72, 1208 - 123 - 5);
            _lblPlayer1GameTime.Size = new Size(200, 50);  // 增加高度以适应更大字号
            _lblPlayer1GameTime.Text = "10:00:000";
            _lblPlayer1GameTime.TextAlign = ContentAlignment.MiddleLeft;
            _bg.Controls.Add(_lblPlayer1GameTime);
            
            // ========== 下方玩家姓名 ==========
            // 原位置 (1663, 642)，向左72，向上123，再向下217，再向下74，再向上5，再向上5
            _lblPlayer1Name.Location = new Point(1663 - 72, 642 - 123 + 217 + 74 - 5 - 5);
            _lblPlayer1Name.Size = new Size(400, 45);  // 增加高度
            _lblPlayer1Name.Text = $"{_player1Name}";
            _lblPlayer1Name.TextAlign = ContentAlignment.MiddleLeft;
            _bg.Controls.Add(_lblPlayer1Name);
            
            // ========== 下方玩家积分 ==========
            // 原位置 (2144, 642)，向左72，向上123，再向下217，再向下74，再向上5，再向上5
            // 不显示"积分："文字，只显示数字
            _lblPlayer1Score.Location = new Point(2144 - 72, 642 - 123 + 217 + 74 - 5 - 5);
            _lblPlayer1Score.Size = new Size(150, 45);  // 增加高度
            _lblPlayer1Score.Text = $"{_player1Score}";
            _lblPlayer1Score.TextAlign = ContentAlignment.MiddleLeft;
            _bg.Controls.Add(_lblPlayer1Score);
            
            // ========== 上方玩家（对手 Player2）头像 ==========
            // 原位置中心 (1604, 431)，向左72，向上123，再向下20，放大到240x240
            _picPlayer2Avatar.Size = new Size(240, 240);
            _picPlayer2Avatar.Location = new Point(1604 - 120 - 72, 431 - 120 - 123 + 20);
            if (_player2Avatar != null)
                _picPlayer2Avatar.Image = _player2Avatar;
            _bg.Controls.Add(_picPlayer2Avatar);
            
            // ========== 上方玩家步时 ==========
            // 原位置 (2051, 375)，向左72，向上123，再向上5
            _lblPlayer2StepTime.Location = new Point(2051 - 72, 375 - 123 - 5);
            _lblPlayer2StepTime.Size = new Size(200, 50);  // 增加高度以适应更大字号
            _lblPlayer2StepTime.Text = "01:00:000";
            _lblPlayer2StepTime.TextAlign = ContentAlignment.MiddleLeft;
            _bg.Controls.Add(_lblPlayer2StepTime);
            
            // ========== 上方玩家局时 ==========
            // 原位置 (2051, 510)，向左72，向上123，再向上5
            _lblPlayer2GameTime.Location = new Point(2051 - 72, 510 - 123 - 5);
            _lblPlayer2GameTime.Size = new Size(200, 50);  // 增加高度以适应更大字号
            _lblPlayer2GameTime.Text = "10:00:000";
            _lblPlayer2GameTime.TextAlign = ContentAlignment.MiddleLeft;
            _bg.Controls.Add(_lblPlayer2GameTime);
            
            // ========== 上方玩家姓名 ==========
            // 原位置 (1663, 661)，向左72，向上123，再向上5，再向上5
            _lblPlayer2Name.Location = new Point(1663 - 72, 661 - 123 - 5 - 5);
            _lblPlayer2Name.Size = new Size(400, 45);  // 增加高度
            _lblPlayer2Name.Text = $"{_player2Name}";
            _lblPlayer2Name.TextAlign = ContentAlignment.MiddleLeft;
            _bg.Controls.Add(_lblPlayer2Name);
            
            // ========== 上方玩家积分 ==========
            // 原位置 (2144, 661)，向左72，向上123，再向上5，再向上5
            // 不显示"积分："文字，只显示数字
            _lblPlayer2Score.Location = new Point(2144 - 72, 661 - 123 - 5 - 5);
            _lblPlayer2Score.Size = new Size(150, 45);  // 增加高度
            _lblPlayer2Score.Text = $"{_player2Score}";
            _lblPlayer2Score.TextAlign = ContentAlignment.MiddleLeft;
            _bg.Controls.Add(_lblPlayer2Score);
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            // 初始化棋盘
            _board = new int[15, 15];
            _moveHistory.Clear();
            _currentPlayer = 1; // 黑子先手
            _gameOver = false;
            _gameStartTime = DateTime.Now; // 记录开始时间
            
            // 重置计时器（倒计时模式，单位：毫秒）
            _player1StepTime = 60000;  // 60秒步时 = 60000毫秒
            _player2StepTime = 60000;
            _player1GameTime = 600000; // 10分钟局时 = 600000毫秒
            _player2GameTime = 600000;
            
            // 启动计时器（13ms间隔）
            _timer.Tick += OnTimerTick;
            _timer.Start();
            
            _boardPanel.Invalidate();
        }

        /// <summary>
        /// 计时器更新（倒计时模式，每13ms更新一次）
        /// </summary>
        private void OnTimerTick(object? sender, EventArgs e)
        {
            if (_gameOver) return;
            
            if (_currentPlayer == 1)
            {
                _player1StepTime -= 13;  // 减少13毫秒
                _player1GameTime -= 13;
                
                // 当局时小于步时，二者保持一致
                if (_player1GameTime < _player1StepTime)
                    _player1StepTime = _player1GameTime;
                
                _lblPlayer1StepTime.Text = FormatTime(_player1StepTime);
                _lblPlayer1GameTime.Text = FormatTime(_player1GameTime);
                
                // 超时判负
                if (_player1StepTime <= 0 || _player1GameTime <= 0)
                {
                    _gameOver = true;
                    _timer.Stop();
                    MessageBox.Show($"{_player1Name} 超时，{_player2Name} 获胜！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SaveMatchLog(winner: 2);
                    UpdateScores(winner: 2);
                }
            }
            else
            {
                _player2StepTime -= 13;  // 减少13毫秒
                _player2GameTime -= 13;
                
                // 当局时小于步时，二者保持一致
                if (_player2GameTime < _player2StepTime)
                    _player2StepTime = _player2GameTime;
                
                _lblPlayer2StepTime.Text = FormatTime(_player2StepTime);
                _lblPlayer2GameTime.Text = FormatTime(_player2GameTime);
                
                // 超时判负
                if (_player2StepTime <= 0 || _player2GameTime <= 0)
                {
                    _gameOver = true;
                    _timer.Stop();
                    MessageBox.Show($"{_player2Name} 超时，{_player1Name} 获胜！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SaveMatchLog(winner: 1);
                    UpdateScores(winner: 1);
                }
            }
        }

        /// <summary>
        /// 格式化时间显示（MM:SS:mmm格式，毫秒）
        /// </summary>
        private string FormatTime(int milliseconds)
        {
            if (milliseconds < 0) milliseconds = 0;
            
            int totalSeconds = milliseconds / 1000;
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            int ms = milliseconds % 1000;
            
            return $"{minutes:D2}:{seconds:D2}:{ms:D3}";
        }

        /// <summary>
        /// 绘制棋盘（只绘制棋子，不绘制网格，使用背景图的棋盘）
        /// </summary>
        private void DrawBoard(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // 只绘制棋子，不绘制网格线和星位（背景图已包含）
            int stoneSize = 30; // 棋子大小
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    if (_board[x, y] == 1) // 黑子
                    {
                        int posX = 20 + x * CELL_SIZE_X - stoneSize / 2;
                        int posY = 20 + y * CELL_SIZE_Y - stoneSize / 2;
                        g.FillEllipse(Brushes.Black, posX, posY, stoneSize, stoneSize);
                        g.DrawEllipse(new Pen(Color.Gray, 1), posX, posY, stoneSize, stoneSize);
                    }
                    else if (_board[x, y] == 2) // 白子
                    {
                        int posX = 20 + x * CELL_SIZE_X - stoneSize / 2;
                        int posY = 20 + y * CELL_SIZE_Y - stoneSize / 2;
                        g.FillEllipse(Brushes.White, posX, posY, stoneSize, stoneSize);
                        g.DrawEllipse(new Pen(Color.Black, 1), posX, posY, stoneSize, stoneSize);
                    }
                }
            }
            
            // 标记最后一步
            if (_moveHistory.Count > 0)
            {
                var lastMove = _moveHistory[^1];
                var markPen = new Pen(Color.Red, 3);
                int markX = 20 + lastMove.X * CELL_SIZE_X - 10;
                int markY = 20 + lastMove.Y * CELL_SIZE_Y - 10;
                g.DrawEllipse(markPen, markX, markY, 20, 20);
            }
        }

        /// <summary>
        /// 棋盘点击事件
        /// </summary>
        private void OnBoardClick(object? sender, MouseEventArgs e)
        {
            if (_gameOver) return;
            
            // 计算点击位置对应的棋盘坐标
            int x = (e.X - 20 + CELL_SIZE_X / 2) / CELL_SIZE_X;
            int y = (e.Y - 20 + CELL_SIZE_Y / 2) / CELL_SIZE_Y;
            
            // 检查坐标是否有效
            if (x < 0 || x >= BOARD_SIZE || y < 0 || y >= BOARD_SIZE)
                return;
            
            // 检查位置是否已有棋子
            if (_board[x, y] != 0)
                return;
            
            // 落子
            _board[x, y] = _currentPlayer;
            _moveHistory.Add(new Point(x, y));
            
            // 重置步时
            if (_currentPlayer == 1)
                _player1StepTime = 0;
            else
                _player2StepTime = 0;
            
            // 重绘棋盘
            _boardPanel.Invalidate();
            
            // 检查胜负
            if (CheckWin(x, y))
            {
                _gameOver = true;
                _timer.Stop();
                
                string winner = _currentPlayer == 1 ? _player1Name : _player2Name;
                MessageBox.Show($"{winner} 获胜！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 保存对局记录
                SaveMatchLog(winner: _currentPlayer);
                
                // 更新积分
                UpdateScores(winner: _currentPlayer);
                return;
            }
            
            // 检查平局
            if (_moveHistory.Count == BOARD_SIZE * BOARD_SIZE)
            {
                _gameOver = true;
                _timer.Stop();
                MessageBox.Show("平局！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 保存对局记录（平局）
                SaveMatchLog(winner: 0);
                return;
            }
            
            // 切换玩家并重置步时（60秒 = 60000毫秒）
            _currentPlayer = _currentPlayer == 1 ? 2 : 1;
            if (_currentPlayer == 1)
                _player1StepTime = 60000;
            else
                _player2StepTime = 60000;
        }


        /// <summary>
        /// 检查是否五子连珠
        /// </summary>
        private bool CheckWin(int x, int y)
        {
            int player = _board[x, y];
            
            // 四个方向：横、竖、左斜、右斜
            int[][] directions = {
                new[] { 1, 0 },   // 横向
                new[] { 0, 1 },   // 纵向
                new[] { 1, 1 },   // 右斜
                new[] { 1, -1 }   // 左斜
            };
            
            foreach (var dir in directions)
            {
                int count = 1; // 当前位置算1个
                
                // 正方向计数
                for (int i = 1; i < 5; i++)
                {
                    int nx = x + dir[0] * i;
                    int ny = y + dir[1] * i;
                    if (nx < 0 || nx >= BOARD_SIZE || ny < 0 || ny >= BOARD_SIZE)
                        break;
                    if (_board[nx, ny] != player)
                        break;
                    count++;
                }
                
                // 反方向计数
                for (int i = 1; i < 5; i++)
                {
                    int nx = x - dir[0] * i;
                    int ny = y - dir[1] * i;
                    if (nx < 0 || nx >= BOARD_SIZE || ny < 0 || ny >= BOARD_SIZE)
                        break;
                    if (_board[nx, ny] != player)
                        break;
                    count++;
                }
                
                if (count >= 5)
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// 保存对局记录
        /// </summary>
        private void SaveMatchLog(int winner)
        {
            try
            {
                var matchLogDAL = new GomokuMatchLogDAL();
                var endTime = DateTime.Now;
                var duration = (int)(endTime - _gameStartTime).TotalSeconds;
                
                string player1Result, player2Result;
                int player1ScoreChange, player2ScoreChange;
                
                if (winner == 1) // 玩家1获胜
                {
                    player1Result = "Win";
                    player2Result = "Lose";
                    player1ScoreChange = 10;
                    player2ScoreChange = -5;
                }
                else if (winner == 2) // 玩家2获胜
                {
                    player1Result = "Lose";
                    player2Result = "Win";
                    player1ScoreChange = -5;
                    player2ScoreChange = 10;
                }
                else // 平局
                {
                    player1Result = "Draw";
                    player2Result = "Draw";
                    player1ScoreChange = 0;
                    player2ScoreChange = 0;
                }
                
                matchLogDAL.SaveMatchLog(
                    _player1Username, _player1Name,
                    _player2Username, _player2Name,
                    _gameStartTime, endTime,
                    player1Result, player1ScoreChange,
                    player2Result, player2ScoreChange,
                    _moveHistory.Count, duration
                );
            }
            catch (Exception ex)
            {
                // 记录失败不影响游戏，只记录错误
                System.Diagnostics.Debug.WriteLine($"保存对局记录失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新积分
        /// </summary>
        private void UpdateScores(int winner)
        {
            try
            {
                var profileDAL = new UserProfileDAL();
                
                if (winner == 1) // 玩家1获胜
                {
                    _player1Score += 10;
                    _player2Score = Math.Max(0, _player2Score - 5);
                    
                    var profile1 = profileDAL.GetUserProfile(_player1Username);
                    if (profile1 != null)
                    {
                        profile1.GomokuScore = _player1Score;
                        profileDAL.UpdateUserProfile(profile1);
                    }
                    
                    if (!string.IsNullOrEmpty(_player2Username))
                    {
                        var profile2 = profileDAL.GetUserProfile(_player2Username);
                        if (profile2 != null)
                        {
                            profile2.GomokuScore = _player2Score;
                            profileDAL.UpdateUserProfile(profile2);
                        }
                    }
                }
                else // 玩家2获胜
                {
                    _player2Score += 10;
                    _player1Score = Math.Max(0, _player1Score - 5);
                    
                    var profile2 = profileDAL.GetUserProfile(_player2Username);
                    if (profile2 != null)
                    {
                        profile2.GomokuScore = _player2Score;
                        profileDAL.UpdateUserProfile(profile2);
                    }
                    
                    var profile1 = profileDAL.GetUserProfile(_player1Username);
                    if (profile1 != null)
                    {
                        profile1.GomokuScore = _player1Score;
                        profileDAL.UpdateUserProfile(profile1);
                    }
                }
                
                // 更新显示（只显示数字，不显示"积分："）
                _lblPlayer1Score.Text = $"{_player1Score}";
                _lblPlayer2Score.Text = $"{_player2Score}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新积分失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 悔棋（魔法按钮）
        /// </summary>
        private void OnUndoClick(object? sender, EventArgs e)
        {
            if (_gameOver)
            {
                MessageBox.Show("游戏已结束，无法悔棋！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            if (_moveHistory.Count == 0)
            {
                MessageBox.Show("还没有落子，无法悔棋！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var result = MessageBox.Show(
                "使用魔法进行时间回溯1步？\n（悔棋将撤销最后一步棋）",
                "魔法确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            
            if (result == DialogResult.Yes)
            {
                // 撤销最后一步
                var lastMove = _moveHistory[^1];
                _board[lastMove.X, lastMove.Y] = 0;
                _moveHistory.RemoveAt(_moveHistory.Count - 1);
                
                // 切换回上一个玩家并重置步时（60秒 = 60000毫秒）
                _currentPlayer = _currentPlayer == 1 ? 2 : 1;
                if (_currentPlayer == 1)
                    _player1StepTime = 60000;
                else
                    _player2StepTime = 60000;
                
                // 重绘棋盘
                _boardPanel.Invalidate();
                
                MessageBox.Show("时间回溯成功！", "魔法效果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 和棋（伪证按钮）
        /// </summary>
        private void OnDrawClick(object? sender, EventArgs e)
        {
            if (_gameOver)
            {
                MessageBox.Show("游戏已结束！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var result = MessageBox.Show(
                "提出伪证，宣称局面\"均势\"，请求和棋？\n（双方不分胜负，不计积分）",
                "伪证确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            
            if (result == DialogResult.Yes)
            {
                _gameOver = true;
                _timer.Stop();
                MessageBox.Show("双方接受和棋！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 保存对局记录（和棋）
                SaveMatchLog(winner: 0);
            }
        }

        /// <summary>
        /// 认输（疑问按钮）
        /// </summary>
        private void OnSurrenderClick(object? sender, EventArgs e)
        {
            if (_gameOver)
            {
                MessageBox.Show("游戏已结束！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // 获取当前玩家和对手
            string currentPlayerName = _currentPlayer == 1 ? _player1Name : _player2Name;
            string opponentName = _currentPlayer == 1 ? _player2Name : _player1Name;
            
            var result = MessageBox.Show(
                $"魔法：疑问\n{currentPlayerName}请求认输，{opponentName}是否同意？",
                "认输确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            
            if (result == DialogResult.Yes)
            {
                // 对手同意，认输成功
                _gameOver = true;
                _timer.Stop();
                
                int winner = _currentPlayer == 1 ? 2 : 1; // 对手获胜
                string winnerName = winner == 1 ? _player1Name : _player2Name;
                
                MessageBox.Show($"{winnerName} 接受认输，获得胜利！", "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 保存对局记录
                SaveMatchLog(winner: winner);
                
                // 更新积分
                UpdateScores(winner: winner);
            }
            // 如果对手不同意，什么都不做，继续游戏
        }

        /// <summary>
        /// 返回按钮
        /// </summary>
        private void OnBackClick(object? sender, EventArgs e)
        {
            if (!_gameOver && _moveHistory.Count > 0)
            {
                var result = MessageBox.Show(
                    "游戏尚未结束，确定要退出吗？",
                    "确认退出",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                
                if (result != DialogResult.Yes)
                    return;
            }
            
            GoBack();
        }

        /// <summary>
        /// 返回模式选择界面（直接关闭，让上层窗口处理）
        /// </summary>
        private void GoBack()
        {
            _timer.Stop();
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 如果是用户关闭窗口（点击X），检查是否需要确认
            if (e.CloseReason == CloseReason.UserClosing && !_gameOver && _moveHistory.Count > 0)
            {
                var result = MessageBox.Show(
                    "游戏尚未结束，确定要退出吗？",
                    "确认退出",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            
            // 清理资源
            try
            {
                _timer.Stop();
                _timer.Dispose();
                _player1Avatar?.Dispose();
                _player2Avatar?.Dispose();
            }
            catch { }
            
            base.OnFormClosing(e);
        }
    }
}