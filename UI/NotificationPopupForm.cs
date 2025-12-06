using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 审判通知弹窗
    /// 功能：显示典狱长头像和通知文字，5秒后自动关闭
    /// </summary>
    public class NotificationPopupForm : Form
    {
        private readonly TrialNotificationModel _notification;
        private readonly System.Windows.Forms.Timer _autoCloseTimer;
        
        private readonly PictureBox _picAvatar = new() { SizeMode = PictureBoxSizeMode.StretchImage };
        private readonly Label _lblMessage = new() { AutoSize = false, TextAlign = ContentAlignment.MiddleLeft };
        
        public NotificationPopupForm(TrialNotificationModel notification)
        {
            _notification = notification;
            _autoCloseTimer = new System.Windows.Forms.Timer { Interval = 5000 }; // 5秒
            
            InitializeForm();
            SetupLayout();
            LoadWardenAvatar();
            
            _autoCloseTimer.Tick += (s, e) => 
            { 
                _autoCloseTimer.Stop();
                Close();
            };
            _autoCloseTimer.Start();
        }

        private void InitializeForm()
        {
            Text = "审判通知";
            Width = 400;
            Height = 150;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            TopMost = true; // 置顶显示
            
            BLL.IconHelper.SetFormIcon(this);
        }

        private void SetupLayout()
        {
            // 典狱长头像（左侧）
            _picAvatar.Location = new Point(15, 15);
            _picAvatar.Size = new Size(100, 100);
            
            // 通知文字（右侧）
            _lblMessage.Location = new Point(125, 15);
            _lblMessage.Size = new Size(250, 100);
            _lblMessage.Font = new Font("Microsoft YaHei UI", 10);
            _lblMessage.Text = _notification.Message;
            
            Controls.Add(_picAvatar);
            Controls.Add(_lblMessage);
        }

        private void LoadWardenAvatar()
        {
            try
            {
                // 查询典狱长头像
                const string sql = @"
SELECT w.AvatarPath
FROM wt.TrialSession ts
INNER JOIN wt.[User] u ON u.UserID = ts.CreatedBy
LEFT JOIN wt.UserWitch uw ON uw.UserID = u.UserID
LEFT JOIN wt.Witch w ON w.WitchID = uw.WitchID
WHERE ts.SessionID = @SessionID";

                var dt = WitchTrialSystem.DAL.DBHelper.ExecDataTable(sql,
                    new Microsoft.Data.SqlClient.SqlParameter("@SessionID", _notification.SessionID));
                
                if (dt.Rows.Count > 0 && dt.Rows[0]["AvatarPath"] != DBNull.Value)
                {
                    string avatarPath = dt.Rows[0]["AvatarPath"].ToString() ?? "";
                    string fullPath = Path.Combine(AppContext.BaseDirectory, avatarPath);
                    
                    if (File.Exists(fullPath))
                    {
                        _picAvatar.Image = Image.FromFile(fullPath);
                    }
                    else
                    {
                        LoadDefaultAvatar();
                    }
                }
                else
                {
                    LoadDefaultAvatar();
                }
            }
            catch
            {
                LoadDefaultAvatar();
            }
        }

        private void LoadDefaultAvatar()
        {
            // 使用默认头像
            string defaultPath = Path.Combine(AppContext.BaseDirectory, "Images", "avatars", "default.png");
            if (File.Exists(defaultPath))
            {
                _picAvatar.Image = Image.FromFile(defaultPath);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _autoCloseTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
