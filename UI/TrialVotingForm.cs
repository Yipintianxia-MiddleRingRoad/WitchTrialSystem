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
            
            // 标题 - 向右移动37px，向下移动59px，背景透明
            _lblTitle.Location = new Point(57, 79);
            _lblTitle.ForeColor = Color.White;
            _lblTitle.BackColor = Color.Transparent;
            
            // 说明文字 - 向右移动37px，向下移动59px，背景透明
            _lblInstruction.Location = new Point(57, 114);
            _lblInstruction.ForeColor = Color.LightGray;
            _lblInstruction.BackColor = Color.Transparent;
            
            // 参与者列表容器 - 背景透明以显示手机背景图
            _flowPanel.Location = new Point(20, 90);
            _flowPanel.Size = new Size(420, 580);
            _flowPanel.Padding = new Padding(5);
            _flowPanel.BackColor = Color.Transparent;
            
            // 确认按钮
            _btnConfirm.Location = new Point((Width - _btnConfirm.Width) / 2, 685);
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
                Width = 125,
                Height = 170,
                Margin = new Padding(5),
                BackColor = Color.FromArgb(50, 50, 50)
            };
            
            // 头像
            var avatar = new PictureBox
            {
                Width = 100,
                Height = 100,
                Left = 12,
                Top = 10,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            
            string avatarPath = Path.Combine(AppContext.BaseDirectory, participant.AvatarPath);
            if (File.Exists(avatarPath))
            {
                avatar.Image = Image.FromFile(avatarPath);
            }
            
            // 单选按钮
            var radioButton = new RadioButton
            {
                Width = 100,
                Left = 12,
                Top = 120,
                Text = participant.WitchName,
                Tag = participant.WitchID,
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei UI", 9)
            };
            
            radioButton.CheckedChanged += (s, e) =>
            {
                if (radioButton.Checked)
                {
                    _selectedWitchId = participant.WitchID;
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
            
            // 显示等待消息
            var lblWaiting = new Label
            {
                Text = "投票成功！",
                AutoSize = true,
                Font = new Font("Microsoft YaHei UI", 18, FontStyle.Bold),
                ForeColor = Color.LightGreen,
                Location = new Point(150, 250)
            };
            
            var lblProgress = new Label
            {
                Name = "lblProgress",
                Text = "投票进度：加载中...",
                AutoSize = true,
                Font = new Font("Microsoft YaHei UI", 12),
                ForeColor = Color.White,
                Location = new Point(120, 310)
            };
            
            var lblTip = new Label
            {
                Text = "您可以关闭窗口，切换其他账号继续投票",
                AutoSize = true,
                Font = new Font("Microsoft YaHei UI", 10),
                ForeColor = Color.Yellow,
                Location = new Point(90, 360)
            };
            
            _bg.Controls.Add(lblWaiting);
            _bg.Controls.Add(lblProgress);
            _bg.Controls.Add(lblTip);
            
            // 启动定时器检查状态
            var timer = new System.Windows.Forms.Timer { Interval = 2000 };
            timer.Tick += (s, e) => CheckVotingProgress(timer, lblProgress);
            timer.Start();
        }

        private void CheckVotingProgress(System.Windows.Forms.Timer timer, Label lblProgress)
        {
            try
            {
                var session = TrialSessionService.GetSessionByID(_sessionId);
                if (session == null)
                {
                    timer.Stop();
                    return;
                }
                
                var progress = TrialSessionService.GetVotingProgress(_sessionId);
                lblProgress.Text = $"投票进度：{progress.Voted}/{progress.Total} 人已投票";
                
                // 如果状态变化，关闭窗口
                if (session.Status != "Voting")
                {
                    timer.Stop();
                    MessageBox.Show("投票已结束，等待典狱长宣布结果...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                timer.Stop();
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
                }
            }
            
            base.OnFormClosing(e);
        }
    }
}
