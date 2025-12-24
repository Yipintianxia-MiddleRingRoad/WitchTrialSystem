using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WitchTrialSystem.BLL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 魔女投票界面
    /// 功能：显示参与魔女列表，选择投票对象
    /// </summary>
    public class TrialVotingForm : Form
    {
        private readonly int _sessionId;
        private readonly int _userId;
        private readonly int _witchId;
        private int _selectedWitchId = 0;
        private bool _hasVoted = false;
        private bool _hasShownEndMessage = false;  // 防止重复弹窗
        private System.Windows.Forms.Timer? _statusCheckTimer = null;  // 状态检查定时器
        
        private readonly Panel _bg = new() { Dock = DockStyle.Fill, BackgroundImageLayout = ImageLayout.Stretch };
        private readonly Label _lblTitle = new() { Text = "魔女审判 - 投票", AutoSize = true, Font = new Font("Microsoft YaHei UI", 14, FontStyle.Bold) };
        private readonly Label _lblInstruction = new() { Text = "请选择您认为应该被处刑的魔女", AutoSize = true, Font = new Font("Microsoft YaHei UI", 10) };
        private readonly FlowLayoutPanel _flowPanel = new() { AutoScroll = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = true };
        private readonly Button _btnConfirm = new() { Text = "确认投票", Width = 150, Height = 45, Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold) };
        
        public TrialVotingForm(int sessionId, int userId, int witchId)
        {
            _sessionId = sessionId;
            _userId = userId;
            _witchId = witchId;
            
            InitializeForm();
            SetupLayout();
            LoadParticipants();
        }

        private void InitializeForm()
        {
            Text = "魔女审判 - 投票";
            Width = 469;
            Height = 777;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            
            BLL.IconHelper.SetFormIcon(this);
        }

        private void SetupLayout()
        {
            // 加载背景图
            string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "trial_voting_bg.png");
            if (File.Exists(bgPath))
            {
                _bg.BackgroundImage = Image.FromFile(bgPath);
            }
            
            // 标题与说明统一文字颜色（偏暖的浅棕色）
            var mainTextColor = Color.FromArgb(47, 35, 34);

            // 标题 - 向右移动37px，向下移动59px，背景透明
            _lblTitle.Location = new Point(57, 79);
            _lblTitle.ForeColor = mainTextColor;
            _lblTitle.BackColor = Color.Transparent;
            
            // 说明文字 - 向右移动37px，向下移动59px，背景透明
            _lblInstruction.Location = new Point(57, 114);
            _lblInstruction.ForeColor = mainTextColor;
            _lblInstruction.BackColor = Color.Transparent;
            
            // 参与者列表容器 - 背景透明以显示手机背景图
            // 整体向右下移动，使卡片完全落在“手机屏幕”内，不被边框裁剪
            _flowPanel.Location = new Point(40, 140);
            // 略微缩小宽高，为左右和底部边框预留空间，同时保证底部按钮不被遮挡
            _flowPanel.Size = new Size(390, 480);
            _flowPanel.Padding = new Padding(5);
            _flowPanel.BackColor = Color.Transparent;
            
            // 确认按钮
            // 向上移动按钮位置，避免在小屏幕或缩放下被遮挡
            _btnConfirm.Location = new Point((Width - _btnConfirm.Width) / 2, 650);
            _btnConfirm.BackColor = Color.FromArgb(220, 50, 50);
            _btnConfirm.ForeColor = Color.White;
            _btnConfirm.FlatStyle = FlatStyle.Flat;
            _btnConfirm.Click += OnConfirmClick;
            
            _bg.Controls.Add(_lblTitle);
            _bg.Controls.Add(_lblInstruction);
            _bg.Controls.Add(_flowPanel);
            _bg.Controls.Add(_btnConfirm);
            
            Controls.Add(_bg);
        }

        private void LoadParticipants()
        {
            try
            {
                var participants = TrialVotingService.GetParticipants(_sessionId);
                
                foreach (var participant in participants)
                {
                    var card = CreateVoteCard(participant);
                    _flowPanel.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载参与者失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateVoteCard(TrialParticipantModel participant)
        {
            var card = new Panel
            {
                Width = 120,  // 缩小宽度以适应一行三个（390÷3=130，减去margin）
                Height = 160,  // 稍微缩小高度使布局更紧凑
                Margin = new Padding(3),  // 减小间距使布局更紧凑
                // 使用透明背景，让头像PNG的透明区域直接透出下方界面
                BackColor = Color.Transparent
            };
            
            // 头像（可点击）
            var avatar = new PictureBox
            {
                Width = 95,  // 稍微缩小头像
                Height = 95,
                Left = 12,
                Top = 5,  // 向上移动一点
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,  // 保证PNG透明区域不被底色填充
                Cursor = Cursors.Hand,
                Tag = participant.WitchID  // 存储WitchID
            };
            
            string avatarPath = Path.Combine(AppContext.BaseDirectory, participant.AvatarPath);
            if (File.Exists(avatarPath))
            {
                avatar.Image = Image.FromFile(avatarPath);
            }
            
            // 单选按钮
            var radioButton = new RadioButton
            {
                Width = 110,  // 增加宽度以显示完整姓名
                Left = 5,  // 左对齐
                Top = 110,  // 调整位置
                Text = participant.WitchName,
                Tag = participant.WitchID,
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei UI", 9),
                AutoSize = false,  // 禁用自动大小
                TextAlign = ContentAlignment.MiddleLeft  // 文本左对齐
            };
            
            // 点击头像也能选中
            avatar.Click += (s, e) =>
            {
                // 取消其他所有RadioButton
                foreach (Control ctrl in _flowPanel.Controls)
                {
                    if (ctrl is Panel panel)
                    {
                        foreach (Control innerCtrl in panel.Controls)
                        {
                            if (innerCtrl is RadioButton rb)
                            {
                                rb.Checked = false;
                            }
                        }
                    }
                }
                // 选中当前RadioButton
                radioButton.Checked = true;
            };
            
            radioButton.CheckedChanged += (s, e) =>
            {
                if (radioButton.Checked)
                {
                    _selectedWitchId = participant.WitchID;
                    
                    // 取消其他所有RadioButton（确保只能选一个）
                    foreach (Control ctrl in _flowPanel.Controls)
                    {
                        if (ctrl is Panel panel && panel != card)
                        {
                            foreach (Control innerCtrl in panel.Controls)
                            {
                                if (innerCtrl is RadioButton rb && rb != radioButton)
                                {
                                    rb.Checked = false;
                                }
                            }
                        }
                    }
                }
            };
            
            card.Controls.Add(avatar);
            card.Controls.Add(radioButton);
            
            return card;
        }

        private void OnConfirmClick(object? sender, EventArgs e)
        {
            if (_selectedWitchId == 0)
            {
                MessageBox.Show("请选择一个魔女！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var result = MessageBox.Show($"确定要投票给选中的魔女吗？\n\n投票后不能修改！", 
                "确认投票", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result != DialogResult.Yes) return;
            
            try
            {
                var voteResult = TrialVotingService.SubmitVote(_sessionId, _witchId, _selectedWitchId);
                
                if (voteResult.Success)
                {
                    _hasVoted = true;
                    MessageBox.Show("投票成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // 切换到等待界面
                    ShowWaitingUI();
                }
                else
                {
                    MessageBox.Show($"投票失败：{voteResult.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"投票失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowWaitingUI()
        {
            // 清空界面
            _bg.Controls.Clear();
            // 显示等待消息（文本背景透明、颜色调整并居中）
            var lblWaiting = new Label
            {
                Text = "投票成功！",
                AutoSize = true,
                Font = new Font("Microsoft YaHei UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(47, 35, 34),
                BackColor = Color.Transparent
            };

            var lblProgress = new Label
            {
                Name = "lblProgress",
                Text = "投票进度：加载中...",
                AutoSize = true,
                Font = new Font("Microsoft YaHei UI", 12),
                ForeColor = Color.FromArgb(47, 35, 34),
                BackColor = Color.Transparent
            };

            var lblTip = new Label
            {
                Text = "您可以关闭窗口，切换其他账号继续投票",
                AutoSize = true,
                Font = new Font("Microsoft YaHei UI", 10),
                ForeColor = Color.FromArgb(47, 35, 34),
                BackColor = Color.Transparent
            };

            _bg.Controls.Add(lblWaiting);
            _bg.Controls.Add(lblProgress);
            _bg.Controls.Add(lblTip);

            // 立即布局并居中显示，确保在不同窗口尺寸下也能居中
            lblWaiting.Left = Math.Max(10, (_bg.ClientSize.Width - lblWaiting.Width) / 2);
            lblWaiting.Top = (int)(_bg.ClientSize.Height * 0.35);

            lblProgress.Left = Math.Max(10, (_bg.ClientSize.Width - lblProgress.Width) / 2);
            lblProgress.Top = lblWaiting.Bottom + 20;

            lblTip.Left = Math.Max(10, (_bg.ClientSize.Width - lblTip.Width) / 2);
            lblTip.Top = lblProgress.Bottom + 10;

            // 窗口布局变化时重新居中
            _bg.Layout += (s, e) =>
            {
                lblWaiting.Left = Math.Max(10, (_bg.ClientSize.Width - lblWaiting.Width) / 2);
                lblWaiting.Top = (int)(_bg.ClientSize.Height * 0.35);

                lblProgress.Left = Math.Max(10, (_bg.ClientSize.Width - lblProgress.Width) / 2);
                lblProgress.Top = lblWaiting.Bottom + 20;

                lblTip.Left = Math.Max(10, (_bg.ClientSize.Width - lblTip.Width) / 2);
                lblTip.Top = lblProgress.Bottom + 10;
            };

            // 启动定时器检查状态
            _statusCheckTimer = new System.Windows.Forms.Timer { Interval = 2000 };
            _statusCheckTimer.Tick += (s, e) => CheckVotingProgress(_statusCheckTimer, lblProgress);
            _statusCheckTimer.Start();
        }

        private void CheckVotingProgress(System.Windows.Forms.Timer timer, Label lblProgress)
        {
            try
            {
                var session = TrialSessionService.GetSessionByID(_sessionId);
                if (session == null)
                {
                    timer.Stop();
                    timer.Dispose();
                    return;
                }
                
                var progress = TrialSessionService.GetVotingProgress(_sessionId);
                lblProgress.Text = $"投票进度：{progress.Voted}/{progress.Total} 人已投票";
                
                // 如果状态变化，关闭窗口
                if (session.Status != "Voting")
                {
                    timer.Stop();
                    timer.Dispose();
                    
                    // 防止重复弹窗
                    if (!_hasShownEndMessage)
                    {
                        _hasShownEndMessage = true;
                        MessageBox.Show("投票已结束，等待典狱长宣布结果...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                timer.Stop();
                timer.Dispose();
                MessageBox.Show($"检查状态失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 投票前不允许关闭
            if (!_hasVoted && e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show("您还未投票，确定要退出吗？\n\n退出后可以重新登录继续投票。", 
                    "确认退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            
            // 关键：停止定时器，防止窗口关闭后继续触发
            if (_statusCheckTimer != null)
            {
                _statusCheckTimer.Stop();
                _statusCheckTimer.Dispose();
                _statusCheckTimer = null;
            }
            
            base.OnFormClosing(e);
        }
    }
}
