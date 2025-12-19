using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WitchTrialSystem.BLL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 处刑对象确认界面
    /// 功能：显示处刑对象头像和姓名，播放音效，确认后跳转到处刑按钮界面
    /// </summary>
    public class TrialExecutionConfirmForm : Form
    {
        private readonly int _sessionId;
        private readonly int _userId;
        private readonly int _witchId;
        private readonly string _username;
        
        private readonly Panel _bg = new() { Dock = DockStyle.Fill, BackgroundImageLayout = ImageLayout.Stretch };
        private readonly Label _lblTitle = new() { Text = "处刑对象确认", AutoSize = true, Font = new Font("Microsoft YaHei UI", 14, FontStyle.Bold) };
        private readonly PictureBox _picTarget = new() { SizeMode = PictureBoxSizeMode.StretchImage };
        private readonly Label _lblTargetName = new() { AutoSize = true, Font = new Font("Microsoft YaHei UI", 16, FontStyle.Bold) };
        private readonly Button _btnConfirm = new() { Text = "确认处刑", Width = 150, Height = 45, Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold) };
        
        public TrialExecutionConfirmForm(int sessionId, int userId, int witchId, string username)
        {
            _sessionId = sessionId;
            _userId = userId;
            _witchId = witchId;
            _username = username;
            
            InitializeForm();
            SetupLayout();
            LoadExecutionTarget();
            PlayNotificationSound();
        }

        private void InitializeForm()
        {
            Text = "处刑对象确认";
            Width = 469;
            Height = 777;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            
            BLL.IconHelper.SetFormIcon(this);
            
            // 禁用关闭按钮
            ControlBox = false;
        }

        private void SetupLayout()
        {
            // 加载背景图（使用投票界面背景）
            string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "trial_voting_bg.png");
            if (File.Exists(bgPath))
            {
                _bg.BackgroundImage = Image.FromFile(bgPath);
            }
            
            // 标题
            _lblTitle.Location = new Point((Width - 200) / 2, 80);
            _lblTitle.ForeColor = Color.White;
            _lblTitle.BackColor = Color.Transparent;
            
            // 处刑对象头像
            _picTarget.Location = new Point((Width - 200) / 2, 150);
            _picTarget.Size = new Size(200, 200);
            _picTarget.BackColor = Color.Transparent;
            
            // 处刑对象姓名
            _lblTargetName.Location = new Point((Width - 200) / 2, 370);
            _lblTargetName.ForeColor = Color.White;
            _lblTargetName.BackColor = Color.Transparent;
            
            // 确认按钮
            _btnConfirm.Location = new Point((Width - _btnConfirm.Width) / 2, 600);
            _btnConfirm.BackColor = Color.FromArgb(220, 50, 50);
            _btnConfirm.ForeColor = Color.White;
            _btnConfirm.FlatStyle = FlatStyle.Flat;
            _btnConfirm.Click += OnConfirmClick;
            
            _bg.Controls.Add(_lblTitle);
            _bg.Controls.Add(_picTarget);
            _bg.Controls.Add(_lblTargetName);
            _bg.Controls.Add(_btnConfirm);
            
            Controls.Add(_bg);
        }

        private void LoadExecutionTarget()
        {
            try
            {
                var session = TrialSessionService.GetSessionByID(_sessionId);
                if (session == null || !session.ExecutionTargetWitchID.HasValue)
                {
                    MessageBox.Show("未找到处刑对象信息。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
                
                // 查询处刑对象信息
                const string sql = @"
SELECT w.Name, w.AvatarPath
FROM wt.Witch w
WHERE w.WitchID = @WitchID";

                var dt = WitchTrialSystem.DAL.DBHelper.ExecDataTable(sql,
                    new Microsoft.Data.SqlClient.SqlParameter("@WitchID", session.ExecutionTargetWitchID.Value));
                
                if (dt.Rows.Count > 0)
                {
                    string name = dt.Rows[0]["Name"].ToString() ?? "未知";
                    string avatarPath = dt.Rows[0]["AvatarPath"].ToString() ?? "";
                    
                    _lblTargetName.Text = $"{name} 将被处刑";
                    _lblTargetName.Left = (Width - _lblTargetName.Width) / 2; // 居中
                    
                    string fullPath = Path.Combine(AppContext.BaseDirectory, avatarPath);
                    if (File.Exists(fullPath))
                    {
                        _picTarget.Image = Image.FromFile(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载处刑对象信息失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PlayNotificationSound()
        {
            try
            {
                string soundPath = Path.Combine(AppContext.BaseDirectory, "Images", "sounds", "execution_notice.wav");
                if (File.Exists(soundPath))
                {
                    using (var player = new System.Media.SoundPlayer(soundPath))
                    {
                        player.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                // 音效播放失败不影响流程，只记录日志
                Console.WriteLine($"音效播放失败：{ex.Message}");
            }
        }

        private void OnConfirmClick(object? sender, EventArgs e)
        {
            try
            {
                // 跳转到处刑按钮界面
                var executionForm = new ExecutionForm(_username, _sessionId, _witchId);
                executionForm.FormClosed += (s, args) =>
                {
                    // 处刑完成后关闭确认界面
                    DialogResult = DialogResult.OK;
                    Close();
                };
                
                Hide();
                executionForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开处刑界面失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 允许关闭窗口，因为状态已保存在数据库中
            base.OnFormClosing(e);
        }
    }
}
