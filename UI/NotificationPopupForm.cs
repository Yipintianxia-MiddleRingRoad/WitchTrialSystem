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
            Width = 500;   // 增加宽度：400 → 500
            Height = 200;  // 增加高度：150 → 200
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
            _picAvatar.Size = new Size(150, 150);  // 增加头像尺寸：100x100 → 150x150
            
            // 通知文字（右侧）
            _lblMessage.Location = new Point(180, 15);  // 调整位置
            _lblMessage.Size = new Size(300, 150);  // 增加文字区域：250x100 → 300x150
            _lblMessage.Font = new Font("Microsoft YaHei UI", 11);  // 增大字体：10 → 11
            _lblMessage.Text = _notification.Message;
            
            Controls.Add(_picAvatar);
            Controls.Add(_lblMessage);
        }

        private void LoadWardenAvatar()
        {
            try
            {
                // 查询典狱长用户名（warden或warden2）
                const string sql = @"
SELECT u.Username
FROM wt.TrialSession ts
INNER JOIN wt.[User] u ON u.UserID = ts.CreatedBy
WHERE ts.SessionID = @SessionID";

                var dt = WitchTrialSystem.DAL.DBHelper.ExecDataTable(sql,
                    new Microsoft.Data.SqlClient.SqlParameter("@SessionID", _notification.SessionID));
                
                if (dt.Rows.Count > 0 && dt.Rows[0]["Username"] != DBNull.Value)
                {
                    string username = dt.Rows[0]["Username"].ToString() ?? "";
                    
                    // 根据用户名加载对应的头像（warden.png或warden2.png）
                    string avatarFileName = $"{username}.png";
                    string avatarPath = Path.Combine(AppContext.BaseDirectory, "Images", avatarFileName);
                    
                    if (File.Exists(avatarPath))
                    {
                        _picAvatar.Image = Image.FromFile(avatarPath);
                        return;
                    }
                    
                    // 如果没有找到，尝试在Images/characters目录
                    avatarPath = Path.Combine(AppContext.BaseDirectory, "Images", "characters", avatarFileName);
                    if (File.Exists(avatarPath))
                    {
                        _picAvatar.Image = Image.FromFile(avatarPath);
                        return;
                    }
                }
                
                LoadDefaultAvatar();
            }
            catch
            {
                LoadDefaultAvatar();
            }
        }

        private void LoadDefaultAvatar()
        {
            // 使用默认典狱长头像
            string[] possiblePaths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "Images", "warden.png"),
                Path.Combine(AppContext.BaseDirectory, "Images", "Jailer.png"),
                Path.Combine(AppContext.BaseDirectory, "Images", "characters", "warden.png")
            };
            
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    _picAvatar.Image = Image.FromFile(path);
                    return;
                }
            }
            
            // 如果都没有，显示占位图
            _picAvatar.BackColor = Color.LightGray;
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
