using System;
using System.Data;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WitchTrialSystem.BLL;
using WitchTrialSystem.DAL;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 登录界面
    /// 功能：用户登录、密码验证、角色路由
    /// </summary>
    public class LoginForm : Form
    {
        #region 控件定义
        
        // 背景容器
        private readonly Panel _bg = new() { Dock = DockStyle.Fill, BackgroundImageLayout = ImageLayout.Stretch };
        
        // 输入框（深色背景，白色文字，禁用输入法）
        private readonly TextBox _txtUser = new() 
        { 
            Width = 303,  // 455 * 2/3 = 303
            Height = 33,  // 40 - 5 = 35再调整
            Font = new Font("Segoe UI", 14),
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(27, 16, 13),  // RGB(27, 16, 13)
            ForeColor = Color.White,
            ImeMode = ImeMode.Disable  // 禁用输入法，强制英文输入
        };
        
        private readonly TextBox _txtPass = new() 
        { 
            Width = 303,  // 455 * 2/3 = 303
            Height = 33,  // 40 - 5 = 35再调整
            Font = new Font("Segoe UI", 14),
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(27, 16, 13),  // RGB(27, 16, 13)
            ForeColor = Color.White,
            UseSystemPasswordChar = true,
            ImeMode = ImeMode.Disable  // 禁用输入法，强制英文输入
        };
        
        // 热键按钮区域（透明）
        private readonly Panel _btnLogin = new() 
        { 
            Size = new Size(175, 88),
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        private readonly Panel _btnExit = new() 
        { 
            Size = new Size(175, 88),
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        
        // 消息提示（放在底部，透明背景）
        private readonly Label _msg = new() 
        { 
            AutoSize = true,
            ForeColor = Color.OrangeRed,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", 9)
        };
        
        #endregion

        #region 构造函数和初始化
        
        /// <summary>
        /// 构造函数：初始化登录界面
        /// </summary>
        public LoginForm()
        {
            Text = "魔女审判 · 登录";
            Width = 1536;
            Height = 864;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;
            
            // 设置应用程序图标
            BLL.IconHelper.SetFormIcon(this);

            // 加载背景图
            LoadBackground();
            Controls.Add(_bg);

            // 添加输入框到背景（根据图片精确定位）
            // 用户名输入框：再向右10px
            _txtUser.Left = 845;  // 835 + 10
            _txtUser.Top = 327;
            _bg.Controls.Add(_txtUser);

            // 密码输入框：再向右10px，上移25px
            _txtPass.Left = 845;  // 835 + 10
            _txtPass.Top = 435;   // 再调整
            _bg.Controls.Add(_txtPass);

            // Load Game 按钮热键（登录）
            _btnLogin.Left = 800;
            _btnLogin.Top = 580;
            _bg.Controls.Add(_btnLogin);

            // Exit 按钮热键（退出程序）
            _btnExit.Left = 1005;
            _btnExit.Top = 580;
            _bg.Controls.Add(_btnExit);

            // 消息提示（放在底部中央）
            _msg.Left = 700;
            _msg.Top = 700;
            _bg.Controls.Add(_msg);

            // 绑定事件
            _btnLogin.Click += OnLogin;
            _btnExit.Click += OnExit;
            Shown += (_, __) => Bootstrap();
            
            // 回车键登录
            KeyDown += (s, e) => 
            { 
                if (e.KeyCode == Keys.Enter) 
                {
                    e.SuppressKeyPress = true;
                    OnLogin(null, EventArgs.Empty);
                }
            };
            
            // 确保控件在最上层
            _txtUser.BringToFront();
            _txtPass.BringToFront();
            _btnLogin.BringToFront();
            _btnExit.BringToFront();
            _msg.BringToFront();
        }
        
        #endregion

        #region 私有方法
        
        /// <summary>
        /// 加载背景图
        /// </summary>
        private void LoadBackground()
        {
            string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "login_bg.png");
            if (File.Exists(bgPath))
            {
                _bg.BackgroundImage = Image.FromFile(bgPath);
            }
            else
            {
                _bg.BackColor = Color.FromArgb(30, 30, 40); // 深色背景作为备用
            }
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        private void OnExit(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("确定要退出程序吗？", "退出", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// 启动时初始化：检查数据库连接，初始化默认密码
        /// </summary>
        private void Bootstrap()
        {
            try
            {
                var bll = new UserBLL();

                // 自动发现库里 Salt/Hash 仍为 PENDING 的账号，并统一写入默认口令 123456（仅初始化一次）
                var dt = DBHelper.ExecDataTable(
                    "SELECT Username FROM wt.[User] WHERE Salt='PENDING' OR PasswordHash='PENDING'");
                var toInit = DBHelper.ExecDataTable(
                    "SELECT Username FROM wt.[User] WHERE Salt='PENDING' OR PasswordHash='PENDING'")
                    .AsEnumerable()
                    .Select(r => r.Field<string?>("Username"))
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .Cast<string>()
                    .ToArray();

                if (toInit.Length > 0)
                {
                    new UserBLL().EnsureDefaults(toInit, "123456");
                    _msg.Text = $"已设置默认口令（123456）：{string.Join(", ", toInit)}";
                }

                var dbName = DBHelper.ExecScalar("SELECT DB_NAME()")?.ToString() ?? "(unknown)";
                this.Text = $"魔女审判 · 登录（{dbName}）";
            }
            catch (Exception ex)
            {
                _msg.Text = "数据库连接失败：" + ex.Message;
            }
        }

        /// <summary>
        /// 登录按钮点击事件：验证用户名密码，根据角色跳转到不同界面
        /// </summary>
        private void OnLogin(object? sender, EventArgs e)
        {
            try
            {
                var username = _txtUser.Text.Trim();
                var password = _txtPass.Text.Trim();
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _msg.Text = "请输入英文账号和密码。";
                    return;
                }

                var bll = new UserBLL();
                var res = bll.Login(username, password);
                if (res == null)
                {
                    _msg.Text = "账号或密码错误。";
                    return;
                }

                // 登录成功：根据角色路由到不同主界面
                var upDal = new UserProfileDAL();
                var prof  = upDal.GetProfile(username);
                string role = (prof.Rows.Count > 0 ? prof.Rows[0]["RoleName"] as string : null) ?? "Witch";

                Form main;
                switch (role)
                {
                    case "Admin":
                        main = new Form1_Admin(username);
                        break;
                    case "Meruru":
                    case "Utena":
                        main = new Form1_Regulator(username);
                        break;
                    case "Warden":
                        main = new Form1_Warden(username);
                        break;
                    case "Witch":
                        main = new WitchTrialSystem.UI.PhoneForm(username);
                        break;
                    default:
                        // 默认使用原 Form1（向后兼容）
                        main = new Form1(username)
                        {
                            Text = $"魔女审判 · 主面板（当前用户：{username}）"
                        };
                        break;
                }

                // 登录成功提示
                MessageBox.Show("登录成功！", "OK");

                // 先隐藏登录窗体，再显示主窗体（体验更顺滑）
                this.Hide();
                main.Show();


            }
            catch (Exception ex)
            {
                MessageBox.Show("登录异常：" + ex.Message, "Error");
            }
        }
        
        #endregion
    }
}
