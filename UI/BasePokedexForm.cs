using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 图鉴系统的基类
    /// 功能：提供通用的导航、布局和退出逻辑
    /// 子类：EvidenceForm、MapForm、RulesForm、RecordsForm
    /// </summary>
    public abstract class BasePokedexForm : Form
    {
        #region 字段定义
        
        protected readonly string _username;
        protected readonly Panel _bg = new() { Dock = DockStyle.Fill, BackgroundImageLayout = ImageLayout.Stretch };
        
        // 导航按钮区域定义（热区）
        protected readonly Panel _btnEvidence = new() { Size = new Size(100, 55), BackColor = Color.Transparent, Cursor = Cursors.Hand };
        protected readonly Panel _btnCharacters = new() { Size = new Size(100, 50), BackColor = Color.Transparent, Cursor = Cursors.Hand };
        protected readonly Panel _btnMap = new() { Size = new Size(100, 55), BackColor = Color.Transparent, Cursor = Cursors.Hand };
        protected readonly Panel _btnRules = new() { Size = new Size(100, 55), BackColor = Color.Transparent, Cursor = Cursors.Hand };
        protected readonly Panel _btnRecords = new() { Size = new Size(100, 55), BackColor = Color.Transparent, Cursor = Cursors.Hand };
        protected readonly Panel _btnLogout = new() { Size = new Size(42, 42), BackColor = Color.Transparent, Cursor = Cursors.Hand };
        
        #endregion

        #region 构造函数和初始化
        
        /// <summary>
        /// 构造函数：初始化图鉴基类
        /// </summary>
        /// <param name="username">当前登录的用户名</param>
        protected BasePokedexForm(string username)
        {
            _username = username;
            InitializeForm();
            BLL.IconHelper.SetFormIcon(this);  // 设置应用程序图标
            LoadBackground();
            SetupNavigation();
            UpdateTitle();  // 更新标题以包含用户信息
        }
        
        /// <summary>
        /// 更新窗体标题，在子类设置 Text 后调用此方法添加用户信息
        /// </summary>
        protected void UpdateTitle()
        {
            if (!string.IsNullOrEmpty(Text) && !Text.Contains("当前用户"))
            {
                Text = $"{Text} (当前用户：{_username})";
            }
        }
        
        #endregion

        #region 抽象方法（子类实现）
        
        /// <summary>
        /// 获取背景图片文件名（子类实现）
        /// </summary>
        protected abstract string GetBackgroundImageName();

        /// <summary>
        /// 禁用当前页面的导航按钮（子类实现）
        /// </summary>
        protected abstract void DisableCurrentPageButton();
        
        #endregion

        #region 私有方法
        
        /// <summary>
        /// 初始化窗体基础设置
        /// </summary>
        private void InitializeForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1280;
            Height = 760;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;
            
            Controls.Add(_bg);
            
            // Esc 键退出
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) DoLogout(); };
        }

        /// <summary>
        /// 加载背景图片
        /// </summary>
        private void LoadBackground()
        {
            string bgName = GetBackgroundImageName();
            string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", bgName);
            if (File.Exists(bgPath))
                _bg.BackgroundImage = Image.FromFile(bgPath);
        }

        /// <summary>
        /// 设置导航按钮
        /// </summary>
        private void SetupNavigation()
        {
            // 右侧导航按钮位置（根据精确的像素范围）
            int rightX = ClientSize.Width - 110;

            // 按钮Y坐标（范围：140-195, 210-260, 275-330, 345-400, 415-470）
            _btnEvidence.Location = new Point(rightX, 140);
            _btnCharacters.Location = new Point(rightX, 210);
            _btnMap.Location = new Point(rightX, 275);
            _btnRules.Location = new Point(rightX, 345);
            _btnRecords.Location = new Point(rightX, 415);

            // 右上角退出按钮（叉叉）
            _btnLogout.Location = new Point(ClientSize.Width - 64, 18);

            // 添加到背景
            _bg.Controls.Add(_btnEvidence);
            _bg.Controls.Add(_btnCharacters);
            _bg.Controls.Add(_btnMap);
            _bg.Controls.Add(_btnRules);
            _bg.Controls.Add(_btnRecords);
            _bg.Controls.Add(_btnLogout);

            // 绑定点击事件
            _btnEvidence.Click += (s, e) => NavigateTo("Evidence");
            _btnCharacters.Click += (s, e) => NavigateTo("Characters");
            _btnMap.Click += (s, e) => NavigateTo("Map");
            _btnRules.Click += (s, e) => NavigateTo("Rules");
            _btnRecords.Click += (s, e) => NavigateTo("Records");
            _btnLogout.Click += (s, e) => DoLogout();

            // 禁用当前页面的按钮
            DisableCurrentPageButton();
        }

        /// <summary>
        /// 导航到指定页面
        /// </summary>
        private void NavigateTo(string pageName)
        {
            Form? newForm = pageName switch
            {
                "Evidence" => new EvidenceForm(_username),
                "Characters" => new PokedexForm(_username),
                "Map" => new MapForm(_username),
                "Rules" => new RulesForm(_username),
                "Records" => new RecordsForm(_username),
                _ => null
            };

            if (newForm != null)
            {
                newForm.FormClosed += (s, e) => this.Close();
                this.Hide();
                newForm.Show();
            }
        }

        /// <summary>
        /// 退出到手机界面
        /// </summary>
        private void DoLogout()
        {
            var result = MessageBox.Show("确定要返回手机界面吗？", "返回手机",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var phoneForm = new PhoneForm(_username);
                phoneForm.Show();
                this.Close();
            }
        }
        
        #endregion
    }
}
