using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WitchTrialSystem.BLL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 典狱长审判管理界面
    /// 功能：发起审判、查看投票进度、确认处刑对象、完成处刑
    /// </summary>
    public class TrialManagementForm : Form
    {
        #region 字段定义
        
        private readonly string _username;
        private readonly int _userId;
        private readonly int _islandId;
        
        // 定时器：2秒刷新状态
        private readonly System.Windows.Forms.Timer _stateCheckTimer = new() { Interval = 2000 };
        
        // 当前审判会话
        private TrialSessionModel? _currentSession = null;
        
        // UI控件
        private readonly Label _lblStatus = new() { AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
        private readonly Label _lblInfo = new() { AutoSize = true, Font = new Font("Segoe UI", 10) };
        private readonly Button _btnCreateTrial = new() { Text = "发起审判", Width = 120, Height = 40 };
        private readonly Button _btnStartVoting = new() { Text = "开始投票", Width = 120, Height = 40 };
        private readonly Button _btnViewResults = new() { Text = "查看投票结果", Width = 120, Height = 40 };
        private readonly Button _btnAnnounce = new() { Text = "宣布处刑对象", Width = 120, Height = 40 };
        private readonly Button _btnComplete = new() { Text = "开始处刑", Width = 120, Height = 40 };
        private readonly Button _btnCancel = new() { Text = "取消审判", Width = 120, Height = 40 };
        private readonly Button _btnRefresh = new() { Text = "刷新", Width = 100, Height = 35 };
        private readonly DataGridView _gridParticipants = new() { ReadOnly = true, AllowUserToAddRows = false };
        
        #endregion

        #region 构造函数和初始化
        
        /// <summary>
        /// 构造函数：初始化典狱长审判管理界面
        /// </summary>
        public TrialManagementForm(string username, int userId, int islandId)
        {
            _username = username;
            _userId = userId;
            _islandId = islandId;
            
            InitializeForm();
            SetupLayout();
            SetupEvents();
            
            // 启动定时器
            _stateCheckTimer.Tick += OnStateCheckTimerTick;
            _stateCheckTimer.Start();
            
            // 初始加载
            RefreshUI();
        }

        /// <summary>
        /// 初始化窗体设置
        /// </summary>
        private void InitializeForm()
        {
            Text = $"审判管理 - {_username}";
            Width = 900;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            
            // 设置应用程序图标
            BLL.IconHelper.SetFormIcon(this);
        }

        /// <summary>
        /// 设置界面布局
        /// </summary>
        private void SetupLayout()
        {
            // 顶部状态面板
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(15)
            };
            
            _lblStatus.Location = new Point(15, 15);
            _lblInfo.Location = new Point(15, 45);
            
            topPanel.Controls.Add(_lblStatus);
            topPanel.Controls.Add(_lblInfo);
            
            // 按钮面板
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10),
                BackColor = Color.White
            };
            
            buttonPanel.Controls.Add(_btnCreateTrial);
            buttonPanel.Controls.Add(_btnStartVoting);
            buttonPanel.Controls.Add(_btnViewResults);
            buttonPanel.Controls.Add(_btnAnnounce);
            buttonPanel.Controls.Add(_btnComplete);
            buttonPanel.Controls.Add(_btnCancel);
            buttonPanel.Controls.Add(_btnRefresh);
            
            // 参与者列表
            _gridParticipants.Dock = DockStyle.Fill;
            _gridParticipants.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _gridParticipants.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            // 添加到窗体
            Controls.Add(_gridParticipants);
            Controls.Add(buttonPanel);
            Controls.Add(topPanel);
        }

        /// <summary>
        /// 设置事件处理
        /// </summary>
        private void SetupEvents()
        {
            _btnCreateTrial.Click += OnCreateTrialClick;
            _btnStartVoting.Click += OnStartVotingClick;
            _btnViewResults.Click += OnViewResultsClick;
            _btnAnnounce.Click += OnAnnounceClick;
            _btnComplete.Click += OnCompleteClick;
            _btnCancel.Click += OnCancelClick;
            _btnRefresh.Click += (s, e) => RefreshUI();
            
            FormClosing += (s, e) => _stateCheckTimer.Stop();
        }
        
        #endregion

        #region 状态刷新
        
        /// <summary>
        /// 定时器事件：2秒检查状态变化
        /// </summary>
        private void OnStateCheckTimerTick(object? sender, EventArgs e)
        {
            RefreshUI();
        }

        /// <summary>
        /// 刷新界面：根据当前审判状态显示对应的UI
        /// </summary>
        private void RefreshUI()
        {
            try
            {
                // 查询当前审判会话
                _currentSession = TrialSessionService.GetActiveSession(_islandId);
                
                if (_currentSession == null)
                {
                    // 无审判状态
                    ShowIdleUI();
                }
                else
                {
                    // 根据状态显示对应UI
                    switch (_currentSession.Status)
                    {
                        case "Pending":
                            ShowPendingUI();
                            break;
                        case "Voting":
                            ShowVotingUI();
                            break;
                        case "Confirmed":
                            ShowConfirmedUI();
                            break;
                        case "Executing":
                            ShowExecutingUI();
                            break;
                        case "Completed":
                            ShowCompletedUI();
                            break;
                        default:
                            ShowIdleUI();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _lblStatus.Text = "状态刷新失败";
                _lblInfo.Text = $"错误：{ex.Message}";
                _lblStatus.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// 显示无审判状态UI
        /// </summary>
        private void ShowIdleUI()
        {
            _lblStatus.Text = "当前状态：无审判进行中";
            _lblStatus.ForeColor = Color.Gray;
            _lblInfo.Text = "点击\"发起审判\"按钮开始新的审判流程";
            
            _btnCreateTrial.Enabled = true;
            _btnStartVoting.Enabled = false;
            _btnViewResults.Enabled = false;
            _btnAnnounce.Enabled = false;
            _btnComplete.Enabled = false;
            _btnCancel.Enabled = false;
            
            _gridParticipants.DataSource = null;
        }

        /// <summary>
        /// 显示待开始状态UI
        /// </summary>
        private void ShowPendingUI()
        {
            _lblStatus.Text = "当前状态：审判待开始";
            _lblStatus.ForeColor = Color.Blue;
            _lblInfo.Text = $"审判已创建，参与人数：{GetParticipantCount()}人。点击\"开始投票\"开始投票阶段。";
            
            _btnCreateTrial.Enabled = false;
            _btnStartVoting.Enabled = true;
            _btnViewResults.Enabled = false;
            _btnAnnounce.Enabled = false;
            _btnComplete.Enabled = false;
            _btnCancel.Enabled = true;
            
            LoadParticipants();
        }

        /// <summary>
        /// 显示投票中状态UI
        /// </summary>
        private void ShowVotingUI()
        {
            var progress = TrialSessionService.GetVotingProgress(_currentSession!.SessionID);
            
            _lblStatus.Text = "当前状态：投票进行中";
            _lblStatus.ForeColor = Color.Orange;
            _lblInfo.Text = $"投票进度：{progress.Voted}/{progress.Total} 人已投票";
            
            _btnCreateTrial.Enabled = false;
            _btnStartVoting.Enabled = false;
            _btnViewResults.Enabled = progress.Voted == progress.Total; // 所有人投票完成后可查看结果
            _btnAnnounce.Enabled = false;
            _btnComplete.Enabled = false;
            _btnCancel.Enabled = true;
            
            LoadParticipants();
        }

        /// <summary>
        /// 显示已确认状态UI
        /// </summary>
        private void ShowConfirmedUI()
        {
            _lblStatus.Text = "当前状态：处刑对象已确认";
            _lblStatus.ForeColor = Color.DarkOrange;
            
            // 获取处刑对象信息
            string targetName = GetExecutionTargetName();
            _lblInfo.Text = $"处刑对象：{targetName}。点击\"宣布处刑对象\"进入确认阶段。";
            
            _btnCreateTrial.Enabled = false;
            _btnStartVoting.Enabled = false;
            _btnViewResults.Enabled = true; // 可以重新查看投票结果
            _btnAnnounce.Enabled = true;
            _btnComplete.Enabled = false;
            _btnCancel.Enabled = true;
            
            LoadParticipants();
        }

        /// <summary>
        /// 显示处刑中状态UI
        /// </summary>
        private void ShowExecutingUI()
        {
            var progress = TrialSessionService.GetConfirmationProgress(_currentSession!.SessionID);
            
            _lblStatus.Text = "当前状态：等待魔女确认处刑";
            _lblStatus.ForeColor = Color.Red;
            
            string targetName = GetExecutionTargetName();
            _lblInfo.Text = $"处刑对象：{targetName}。确认进度：{progress.Confirmed}/{progress.Total} 人已确认";
            
            _btnCreateTrial.Enabled = false;
            _btnStartVoting.Enabled = false;
            _btnViewResults.Enabled = true;
            _btnAnnounce.Enabled = false;
            _btnComplete.Enabled = progress.Confirmed == progress.Total; // 所有人确认后可完成处刑
            _btnCancel.Enabled = false; // 处刑阶段不能取消
            
            LoadParticipants();
        }

        /// <summary>
        /// 显示已完成状态UI
        /// </summary>
        private void ShowCompletedUI()
        {
            _lblStatus.Text = "当前状态：审判已完成";
            _lblStatus.ForeColor = Color.Green;
            
            string targetName = GetExecutionTargetName();
            _lblInfo.Text = $"处刑对象：{targetName}。审判已完成，可以发起新的审判。";
            
            _btnCreateTrial.Enabled = true;
            _btnStartVoting.Enabled = false;
            _btnViewResults.Enabled = true;
            _btnAnnounce.Enabled = false;
            _btnComplete.Enabled = false;
            _btnCancel.Enabled = false;
            
            LoadParticipants();
        }

        /// <summary>
        /// 加载参与者列表
        /// </summary>
        private void LoadParticipants()
        {
            if (_currentSession == null) return;
            
            try
            {
                var participants = TrialVotingService.GetParticipants(_currentSession.SessionID);
                
                // 创建显示用的DataTable
                var dt = new DataTable();
                dt.Columns.Add("姓名", typeof(string));
                dt.Columns.Add("投票状态", typeof(string));
                dt.Columns.Add("投给", typeof(string));
                dt.Columns.Add("确认状态", typeof(string));
                
                foreach (var p in participants)
                {
                    string voteStatus = p.HasVoted ? "✓已投票" : "✗未投票";
                    string votedFor = p.HasVoted ? (p.VotedForWitchName ?? "-") : "-";
                    string confirmStatus = p.HasConfirmedExecution ? "✓已确认" : "✗未确认";
                    
                    dt.Rows.Add(p.WitchName, voteStatus, votedFor, confirmStatus);
                }
                
                _gridParticipants.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载参与者列表失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 获取参与人数
        /// </summary>
        private int GetParticipantCount()
        {
            if (_currentSession == null) return 0;
            
            try
            {
                var participants = TrialVotingService.GetParticipants(_currentSession.SessionID);
                return participants.Count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取处刑对象姓名
        /// </summary>
        private string GetExecutionTargetName()
        {
            if (_currentSession == null || !_currentSession.ExecutionTargetWitchID.HasValue)
                return "未知";
            
            try
            {
                // 从参与者列表中查找处刑对象
                var participants = TrialVotingService.GetParticipants(_currentSession.SessionID);
                var target = participants.Find(p => p.WitchID == _currentSession.ExecutionTargetWitchID.Value);
                return target?.WitchName ?? "未知";
            }
            catch
            {
                return "未知";
            }
        }
        
        #endregion

        #region 事件处理
        
        /// <summary>
        /// 点击发起审判按钮
        /// </summary>
        private void OnCreateTrialClick(object? sender, EventArgs e)
        {
            try
            {
                using var dialog = new CreateTrialDialog(_username, _userId, _islandId);
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    MessageBox.Show("审判已成功创建！", "成功", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发起审判失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 点击开始投票按钮
        /// </summary>
        private void OnStartVotingClick(object? sender, EventArgs e)
        {
            if (_currentSession == null) return;
            
            var result = MessageBox.Show("确定要开始投票吗？", "确认", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result != DialogResult.Yes) return;
            
            try
            {
                var startResult = TrialSessionService.StartVoting(_currentSession.SessionID, _userId);
                
                if (startResult.Success)
                {
                    MessageBox.Show("投票已开始！", "成功", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshUI();
                }
                else
                {
                    MessageBox.Show($"开始投票失败：{startResult.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"开始投票失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 点击查看投票结果按钮
        /// </summary>
        private void OnViewResultsClick(object? sender, EventArgs e)
        {
            if (_currentSession == null) return;
            
            try
            {
                using var dialog = new VotingResultDialog(_currentSession.SessionID, _userId);
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"查看投票结果失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 点击宣布处刑对象按钮
        /// </summary>
        private void OnAnnounceClick(object? sender, EventArgs e)
        {
            if (_currentSession == null) return;
            
            string targetName = GetExecutionTargetName();
            var msgResult = MessageBox.Show($"确定要宣布处刑对象\"{targetName}\"吗？", "确认", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (msgResult != DialogResult.Yes) return;
            
            try
            {
                var announceResult = TrialSessionService.AnnounceExecutionTarget(_currentSession.SessionID, _userId);
                
                if (announceResult.Success)
                {
                    MessageBox.Show("处刑对象已宣布！", "成功", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshUI();
                }
                else
                {
                    MessageBox.Show($"宣布失败：{announceResult.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"宣布失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 点击开始处刑按钮
        /// </summary>
        private void OnCompleteClick(object? sender, EventArgs e)
        {
            if (_currentSession == null) return;
            
            string targetName = GetExecutionTargetName();
            var result = MessageBox.Show($"确定要处刑\"{targetName}\"吗？\n\n此操作不可撤销！", "确认处刑", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            
            if (result != DialogResult.Yes) return;
            
            try
            {
                var completeResult = TrialSessionService.CompleteExecution(_currentSession.SessionID, _userId);
                
                if (completeResult.Success)
                {
                    MessageBox.Show("处刑已完成！", "成功", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshUI();
                }
                else
                {
                    MessageBox.Show($"处刑失败：{completeResult.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处刑失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 点击取消审判按钮
        /// </summary>
        private void OnCancelClick(object? sender, EventArgs e)
        {
            if (_currentSession == null) return;
            
            var result = MessageBox.Show("确定要取消当前审判吗？", "确认", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result != DialogResult.Yes) return;
            
            try
            {
                var cancelResult = TrialSessionService.CancelSession(_currentSession.SessionID, _userId);
                
                if (cancelResult.Success)
                {
                    MessageBox.Show("审判已取消！", "成功", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshUI();
                }
                else
                {
                    MessageBox.Show($"取消失败：{cancelResult.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"取消失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        #endregion
    }
}
