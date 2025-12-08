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
        
        private readonly Panel _btnGomoku = new()  // 五子棋按钮（左下角红色图标）
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnRanking = new()  // 排行榜按钮（底部第二个，箭头/消息框图标）
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnMatchLog = new()  // 对局日志按钮（底部第三个，放大镜图标）
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnExit = new() 
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnExecution = new()  // 处刑按钮（魔女图鉴下方171像素）
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnSettings = new()  // 设置按钮（魔女图鉴右边170像素）
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnLive = new()  // 直播按钮（处刑右边170像素）
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnRecord = new()  // 录音按钮（处刑下方）
        { 
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnCamera = new()  // 照相按钮（右下角）
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
            
            // 窗口显示后再检查通知
            this.Shown += (s, e) => CheckNotifications();
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
            
            // 设置应用程序图标
            BLL.IconHelper.SetFormIcon(this);
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

            // 五子棋按钮（底部左下角红色图标，第一个）
            _btnGomoku.Size = new Size(60, 60);  // 底部图标大小
            _btnGomoku.Left = 70;   // 底部左侧第一个图标
            _btnGomoku.Top = ClientSize.Height - 145;  // 底部位置（向上移动25像素：120+25=145）
            _bg.Controls.Add(_btnGomoku);

            // 排行榜按钮（底部第二个，箭头/消息框图标）
            _btnRanking.Size = new Size(60, 60);  // 底部图标大小
            _btnRanking.Left = 140;  // 底部第二个图标位置（第一个70，间隔70）
            _btnRanking.Top = ClientSize.Height - 145;
            _bg.Controls.Add(_btnRanking);

            // 对局日志按钮（底部第三个，放大镜图标）
            _btnMatchLog.Size = new Size(60, 60);  // 底部图标大小
            _btnMatchLog.Left = 210;  // 底部第三个图标位置（第一个70，间隔70）
            _btnMatchLog.Top = ClientSize.Height - 145;
            _bg.Controls.Add(_btnMatchLog);

            // 退出按钮（右上角X）
            _btnExit.Size = new Size(50, 50);
            _btnExit.Left = ClientSize.Width - 80;  // 右上角
            _btnExit.Top = 30;   // 状态栏区域
            _bg.Controls.Add(_btnExit);

            // 处刑按钮（魔女图鉴下方171像素）
            _btnExecution.Size = new Size(120, 140);  // 与魔女图鉴同样大小
            _btnExecution.Left = 50;   // 与魔女图鉴同样位置
            _btnExecution.Top = 120 + 171;  // 魔女图鉴下方171像素：120+171=291
            _bg.Controls.Add(_btnExecution);

            // 设置按钮（魔女图鉴右边170像素）
            _btnSettings.Size = new Size(120, 140);  // 与魔女图鉴同样大小
            _btnSettings.Left = 50 + 170;  // 魔女图鉴右边170像素：50+170=220
            _btnSettings.Top = 120;   // 与魔女图鉴同样高度
            _bg.Controls.Add(_btnSettings);

            // 直播按钮（处刑右边170像素）
            _btnLive.Size = new Size(120, 140);  // 与处刑同样大小
            _btnLive.Left = 50 + 170;  // 处刑右边170像素：50+170=220
            _btnLive.Top = 120 + 171;  // 与处刑同样高度：120+171=291
            _bg.Controls.Add(_btnLive);

            // 录音按钮（处刑下方）
            _btnRecord.Size = new Size(120, 140);  // 与其他按钮同样大小
            _btnRecord.Left = 50;   // 与处刑同样位置
            _btnRecord.Top = 120 + 171 + 140 + 20;  // 处刑下方：291+140+20=451
            _bg.Controls.Add(_btnRecord);

            // 照相按钮（右下角）
            _btnCamera.Size = new Size(120, 140);  // 与其他按钮同样大小
            _btnCamera.Left = 50 + 170;  // 与直播按钮同样位置：220
            _btnCamera.Top = 120 + 171 + 140 + 20;  // 与录音按钮同行：451
            _bg.Controls.Add(_btnCamera);

            // 绑定点击事件
            _btnPokedex.Click += OnPokedexClick;
            _btnGomoku.Click += OnGomokuClick;
            _btnRanking.Click += OnRankingClick;
            _btnMatchLog.Click += OnMatchLogClick;
            _btnExit.Click += OnExitClick;
            _btnExecution.Click += OnExecutionClick;
            _btnSettings.Click += OnSettingsClick;
            _btnLive.Click += OnLiveClick;
            _btnRecord.Click += OnRecordClick;
            _btnCamera.Click += OnCameraClick;

            // 确保按钮在最上层
            _btnPokedex.BringToFront();
            _btnGomoku.BringToFront();
            _btnRanking.BringToFront();
            _btnMatchLog.BringToFront();
            _btnExit.BringToFront();
            _btnExecution.BringToFront();
            _btnSettings.BringToFront();
            _btnLive.BringToFront();
            _btnRecord.BringToFront();
            _btnCamera.BringToFront();
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
        /// 点击五子棋按钮：跳转到五子棋模式选择界面
        /// </summary>
        private void OnGomokuClick(object? sender, EventArgs e)
        {
            var gomokuModeForm = new GomokuModeForm(_username);
            gomokuModeForm.FormClosed += (s, args) => this.Show();  // 五子棋窗口关闭时显示手机界面
            this.Hide();
            gomokuModeForm.Show();
        }

        /// <summary>
        /// 点击排行榜按钮：显示五子棋积分排行榜
        /// </summary>
        private void OnRankingClick(object? sender, EventArgs e)
        {
            ShowGomokuRanking();
        }

        /// <summary>
        /// 点击对局日志按钮：显示五子棋对局日志
        /// </summary>
        private void OnMatchLogClick(object? sender, EventArgs e)
        {
            var matchLogForm = new GomokuMatchLogForm(_username);
            matchLogForm.ShowDialog();
        }

        /// <summary>
        /// 显示五子棋积分排行榜
        /// </summary>
        private void ShowGomokuRanking()
        {
            try
            {
                // 查询所有魔女的五子棋积分，按积分降序排列
                const string sql = @"
SELECT TOP 13 u.Username, w.Name AS WitchName, u.GomokuScore
FROM wt.[User] u
LEFT JOIN wt.UserWitch uw ON uw.UserID = u.UserID
LEFT JOIN wt.Witch w ON w.WitchID = uw.WitchID
WHERE u.RoleID = 4
ORDER BY u.GomokuScore DESC, u.Username ASC";

                var dt = WitchTrialSystem.DAL.DBHelper.ExecDataTable(sql);
                
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("暂无排行榜数据", "五子棋排行榜", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 构建排行榜文本
                var rankingText = new System.Text.StringBuilder();
                rankingText.AppendLine("═══════════════════════════");
                rankingText.AppendLine("        五子棋积分排行榜");
                rankingText.AppendLine("═══════════════════════════");
                rankingText.AppendLine();

                int rank = 1;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    string username = row["Username"].ToString() ?? "";
                    string witchName = row["WitchName"] == DBNull.Value ? username : row["WitchName"].ToString() ?? username;
                    int score = row["GomokuScore"] == DBNull.Value ? 0 : Convert.ToInt32(row["GomokuScore"]);

                    // 前三名使用特殊标记
                    string rankIcon = rank switch
                    {
                        1 => "🥇",
                        2 => "🥈",
                        3 => "🥉",
                        _ => $"{rank}."
                    };

                    rankingText.AppendLine($"{rankIcon,4} {witchName,-12} {score,6} 分");
                    rank++;
                }

                rankingText.AppendLine();
                rankingText.AppendLine("═══════════════════════════");

                // 显示排行榜
                MessageBox.Show(rankingText.ToString(), "五子棋排行榜", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载排行榜失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 点击退出按钮
        /// </summary>
        private void OnExitClick(object? sender, EventArgs e)
        {
            DoLogout();
        }

        /// <summary>
        /// 点击处刑按钮：检查审判状态并跳转到对应界面
        /// </summary>
        private void OnExecutionClick(object? sender, EventArgs e)
        {
            try
            {
                // 获取当前用户信息
                var userInfo = GetCurrentUserInfo();
                
                // 如果无法获取用户信息，直接进入普通处刑模式
                if (userInfo == null)
                {
                    var executionForm = new ExecutionForm(_username);
                    executionForm.FormClosed += (s, args) => this.Show();
                    this.Hide();
                    executionForm.Show();
                    return;
                }
                
                // 检查当前审判状态
                var state = WitchTrialSystem.BLL.TrialSessionService.GetCurrentState(userInfo.Value.UserID, userInfo.Value.IslandID);
                
                // 调试：显示当前状态
                MessageBox.Show($"调试信息：\nUserID: {userInfo.Value.UserID}\nIslandID: {userInfo.Value.IslandID}\nWitchID: {userInfo.Value.WitchID}\n当前状态: {state}", "调试", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                switch (state)
                {
                    case WitchTrialSystem.Models.TrialState.Idle:
                        // 无审判进行中，进入普通处刑模式
                        var executionForm1 = new ExecutionForm(_username);
                        executionForm1.FormClosed += (s, args) => this.Show();
                        this.Hide();
                        executionForm1.Show();
                        break;
                    
                    case WitchTrialSystem.Models.TrialState.NotParticipating:
                        // 不是参与者，进入普通处刑模式
                        var executionForm2 = new ExecutionForm(_username);
                        executionForm2.FormClosed += (s, args) => this.Show();
                        this.Hide();
                        executionForm2.Show();
                        break;
                    
                    case WitchTrialSystem.Models.TrialState.WaitingToStart:
                        MessageBox.Show("审判已创建，等待典狱长开始投票...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    
                    case WitchTrialSystem.Models.TrialState.Voting:
                        // 跳转到投票界面
                        ShowVotingForm(userInfo.Value);
                        break;
                    
                    case WitchTrialSystem.Models.TrialState.WaitingForOthersToVote:
                        MessageBox.Show("您已投票，等待其他人投票...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    
                    case WitchTrialSystem.Models.TrialState.WaitingForExecutionAnnouncement:
                        MessageBox.Show("投票已完成，等待典狱长宣布处刑对象...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    
                    case WitchTrialSystem.Models.TrialState.ConfirmingExecution:
                        // 跳转到处刑对象确认界面
                        ShowExecutionConfirmForm(userInfo.Value);
                        break;
                    
                    case WitchTrialSystem.Models.TrialState.WaitingForOthersToConfirm:
                        MessageBox.Show("您已确认处刑，等待其他人确认...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    
                    case WitchTrialSystem.Models.TrialState.Completed:
                        MessageBox.Show("审判已完成。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    
                    default:
                        // 普通处刑模式
                        var executionForm3 = new ExecutionForm(_username);
                        executionForm3.FormClosed += (s, args) => this.Show();
                        this.Hide();
                        executionForm3.Show();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"检查审判状态失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // 出错时也允许进入普通处刑模式
                var executionForm = new ExecutionForm(_username);
                executionForm.FormClosed += (s, args) => this.Show();
                this.Hide();
                executionForm.Show();
            }
        }
        
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        private (int UserID, int IslandID, int WitchID)? GetCurrentUserInfo()
        {
            try
            {
                const string sql = @"
SELECT u.UserID, u.IslandID, uw.WitchID
FROM wt.[User] u
LEFT JOIN wt.UserWitch uw ON uw.UserID = u.UserID
WHERE u.Username = @Username";

                var dt = WitchTrialSystem.DAL.DBHelper.ExecDataTable(sql,
                    new Microsoft.Data.SqlClient.SqlParameter("@Username", _username));
                
                if (dt.Rows.Count > 0)
                {
                    int userId = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    int islandId = dt.Rows[0]["IslandID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["IslandID"]);
                    int witchId = dt.Rows[0]["WitchID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["WitchID"]);
                    
                    return (userId, islandId, witchId);
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// 显示投票界面
        /// </summary>
        private void ShowVotingForm((int UserID, int IslandID, int WitchID) userInfo)
        {
            try
            {
                var session = WitchTrialSystem.BLL.TrialSessionService.GetActiveSession(userInfo.IslandID);
                if (session == null)
                {
                    MessageBox.Show("未找到进行中的审判。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                var votingForm = new TrialVotingForm(session.SessionID, userInfo.UserID, userInfo.WitchID);
                votingForm.FormClosed += (s, args) => this.Show();
                this.Hide();
                votingForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开投票界面失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// 显示处刑对象确认界面
        /// </summary>
        private void ShowExecutionConfirmForm((int UserID, int IslandID, int WitchID) userInfo)
        {
            try
            {
                var session = WitchTrialSystem.BLL.TrialSessionService.GetActiveSession(userInfo.IslandID);
                if (session == null)
                {
                    MessageBox.Show("未找到进行中的审判。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                var confirmForm = new TrialExecutionConfirmForm(session.SessionID, userInfo.UserID, userInfo.WitchID, _username);
                confirmForm.FormClosed += (s, args) => this.Show();
                this.Hide();
                confirmForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开确认界面失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 点击设置按钮
        /// </summary>
        private void OnSettingsClick(object? sender, EventArgs e)
        {
            MessageBox.Show("设置功能开发中……", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 点击直播按钮
        /// </summary>
        private void OnLiveClick(object? sender, EventArgs e)
        {
            MessageBox.Show("直播功能开发中……", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 点击录音按钮
        /// </summary>
        private void OnRecordClick(object? sender, EventArgs e)
        {
            MessageBox.Show("录音功能开发中……", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 点击照相按钮：打开照相对话框
        /// </summary>
        private void OnCameraClick(object? sender, EventArgs e)
        {
            var cameraForm = new CameraForm(_username);
            cameraForm.FormClosed += (s, args) => this.Show();  // 相机窗口关闭时显示手机界面
            this.Hide();
            cameraForm.Show();
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
                _isLoggingOut = true; // 标记正在退出
                var login = new LoginForm();
                login.Show();
                this.Close();
            }
        }
        
        private bool _isLoggingOut = false; // 标记是否正在退出
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            // 如果是用户关闭窗口（点击X），返回登录界面
            if (e.CloseReason == CloseReason.UserClosing && !_isLoggingOut)
            {
                e.Cancel = true; // 取消关闭
                DoLogout(); // 执行退出登录逻辑
            }
        }
        
        /// <summary>
        /// 检查未读通知
        /// </summary>
        private void CheckNotifications()
        {
            try
            {
                var userInfo = GetCurrentUserInfo();
                if (userInfo == null) return;
                
                var notifications = WitchTrialSystem.BLL.TrialNotificationService.GetUnreadNotifications(userInfo.Value.UserID);
                
                foreach (var notification in notifications)
                {
                    ShowNotificationPopup(notification);
                    WitchTrialSystem.BLL.TrialNotificationService.MarkAsRead(notification.NotificationID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查通知失败：{ex.Message}");
            }
        }
        
        /// <summary>
        /// 显示通知弹窗
        /// </summary>
        private void ShowNotificationPopup(WitchTrialSystem.Models.TrialNotificationModel notification)
        {
            var popup = new NotificationPopupForm(notification);
            popup.Show();
        }
        
        #endregion
    }
}