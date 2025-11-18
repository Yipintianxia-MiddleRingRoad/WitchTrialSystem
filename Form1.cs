using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WitchTrialSystem.DAL;
using WitchTrialSystem.BLL;

namespace WitchTrialSystem
{
    /// <summary>
    /// 管理面板（Form1）
    /// 功能：管理员/典狱长/梅露露使用的主界面
    /// 包含：用户信息卡片、魔女列表、数据管理功能
    /// </summary>
    public partial class Form1 : Form
    {
        #region 字段定义
        
        // 核心字段
        private readonly string _username;
        private readonly UserProfileDAL _profileDal = new();
        private readonly WitchDAL _dal = new();

        private int _userId;
        private string _roleName = "";
        private bool _canManage = false;

        // 用户卡片 UI

        private readonly PictureBox _avatar = new() { Width = 86, Height = 86, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
        private readonly Label _lblUser  = new() { AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        private readonly Label _lblRole  = new() { AutoSize = true, ForeColor = Color.DimGray };
        private readonly Label _lblCn    = new() { AutoSize = true };
        private readonly Label _lblNo    = new() { AutoSize = true };
        private readonly Label _lblMagic = new() { AutoSize = true, MaximumSize = new Size(520, 0) };

        // 列表与工具栏
        private readonly ComboBox _cbIsland = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };
        private readonly ComboBox _cbBatch  = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly TextBox  _tbSearch = new() { PlaceholderText = "按名字搜索", Width = 220 };
        private readonly Button   _btnRefresh = new() { Text = "刷新", Width = 80 };
        private readonly Button   _btnAdd     = new() { Text = "新增魔女", Width = 100 };
        private readonly Label    _status     = new() { AutoSize = true, ForeColor = Color.DimGray, Padding = new Padding(8,2,8,2) };
        private readonly DataGridView _grid   = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false };

        // 用户操作按钮
        private readonly Button _btnChangePwd = new() { Text = "修改密码", AutoSize = true };
        private readonly Button _btnLogout    = new() { Text = "退出登录", AutoSize = true };
        private readonly Button _btnStatus     = new() { Text = "更改状态", Width = 100 };
        
        #endregion

        #region 构造函数
        
        /// <summary>
        /// 构造函数：初始化管理面板
        /// </summary>
        /// <param name="username">当前登录的用户名</param>
        public Form1(string username)
        {
            _username = username;

            InitializeComponent();
            Text = "魔女审判 · 主面板";
            Width = 980; Height = 640; StartPosition = FormStartPosition.CenterScreen;

            // —— 顶部：用户卡片 —— //
            var card = BuildUserCard();
            card.Dock = DockStyle.Top;

        // —— 第二行：工具条 —— //
        var bar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 54,                       // 稍微高一点，给滚动条留空间
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,              // ✅ 单行，不换行
            AutoScroll = true,                 // ✅ 超出时出现横向滚动条
            Padding = new Padding(8, 6, 8, 6),
            BackColor = Color.WhiteSmoke
        };

        _tbSearch.Width = 220;                 // 可按需要再调窄/变宽

        bar.Controls.Add(new Label{Text="岛屿", AutoSize=true, Padding=new Padding(0,8,6,0)});
        bar.Controls.Add(_cbIsland);
        bar.Controls.Add(new Label{Text="批次", AutoSize=true, Padding=new Padding(12,8,6,0)});
        bar.Controls.Add(_cbBatch);
        bar.Controls.Add(_tbSearch);
        bar.Controls.Add(_btnRefresh);
        bar.Controls.Add(_btnAdd);
        bar.Controls.Add(_btnStatus);          // 放在状态文本前
        bar.Controls.Add(_status);



            // —— 第三行：数据网格 —— //
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Controls.Add(_grid);
            Controls.Add(bar);
            Controls.Add(card);

            // 事件
            Load += Form1_Load;
            _btnRefresh.Click += (_,__) => LoadGrid();
            _tbSearch.KeyDown += (s,e)=>{ if(e.KeyCode==Keys.Enter){ e.SuppressKeyPress=true; LoadGrid(); } };
            _cbIsland.SelectedIndexChanged += (_,__) => { LoadBatches(); LoadGrid(); };
            _cbBatch.SelectedIndexChanged  += (_,__) => { LoadGrid(); };
            _btnAdd.Click += (_,__) => OnAddWitch();
            _btnChangePwd.Click += (_, __) => OnChangePassword();
            _btnLogout.Click    += (_, __) => OnLogout();
            _btnStatus.Click += (_, __) => OnChangeWitchStatus();

        }

        private Panel BuildUserCard()
        {
            // 右侧信息区：纵向排列，自动根据内容增高
            var right = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(12, 6, 6, 6),
                MaximumSize = new Size(760, 0) // 防止一行太长导致控件被挤出
            };
            right.Controls.Add(_lblUser);
            right.Controls.Add(_lblRole);
            right.Controls.Add(_lblCn);
            right.Controls.Add(_lblNo);
            right.Controls.Add(_lblMagic);

            // 按钮加入信息区（一定要在这里 Add）
            _btnChangePwd.Margin = new Padding(0, 6, 8, 0);
            _btnLogout.Margin    = new Padding(0, 6, 0, 0);
            right.Controls.Add(_btnChangePwd);
            right.Controls.Add(_btnLogout);

            // 卡片改为“自动尺寸”，不再固定 Height，避免内容被截断
            var card = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // 左侧头像
            _avatar.Width = 86; _avatar.Height = 86;
            _avatar.SizeMode = PictureBoxSizeMode.Zoom;
            _avatar.BorderStyle = BorderStyle.FixedSingle;

            // 手动布局：头像在左，信息在右
            card.Controls.Add(_avatar);
            _avatar.Left = 10; _avatar.Top = 10;

            card.Controls.Add(right);
            right.Left = _avatar.Right + 10;
            right.Top  = 8;

            // 底部分隔线（用 DockBottom，随 AutoSize 卡片一起增长）
            var line = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Color.Gainsboro };
            card.Controls.Add(line);

            return card;
        }
        
        #endregion

        #region 事件处理
        
        /// <summary>
        /// 窗体加载事件：加载用户信息和数据列表
        /// </summary>
        private void Form1_Load(object? sender, EventArgs e)
        {
            try
            {
                // 1) 加载用户卡片
                LoadUserCard();

                // 2) 加载列表
                var dbName = DBHelper.ExecScalar("SELECT DB_NAME()")?.ToString() ?? "(unknown)";
                _status.Text = "已连接：" + dbName;
                LoadIslands(); LoadBatches(); LoadGrid();
            }
            catch (Exception ex)
            {
                _status.Text = "数据库连接失败：" + ex.Message;
                _status.ForeColor = Color.OrangeRed;
            }
        }
        
        #endregion

        #region 数据加载方法
        
        /// <summary>
        /// 加载用户卡片信息
        /// </summary>
        private void LoadUserCard()
        {
            var dt = _profileDal.GetProfile(_username);
            if (dt.Rows.Count == 0)
            {
                _lblUser.Text = $"账号：{_username}";
                _lblRole.Text = $"角色：未知";
                _lblCn.Text   = "中文名：—";
                _lblNo.Text   = "囚犯编号：—";
                _lblMagic.Text= "魔法：—";
                return;
            }

            var r = dt.Rows[0];
            string role = r["RoleName"] as string ?? "Unknown";
            string cn   = r["CnName"]   as string ?? "—";
            string no   = r["PrisonerNo"] as string ?? "—";
            string mg   = r["Magic"]    as string ?? "—";
            string? avatar = r["AvatarPath"] as string;

            _lblUser.Text  = $"账号：{_username}";
            _lblRole.Text  = $"角色：{role}";
            _lblCn.Text    = $"中文名：{(cn ?? "—")}";
            _lblNo.Text    = $"囚犯编号：{(no ?? "—")}";
            _lblMagic.Text = $"魔法：{(mg ?? "—")}";
            _userId   = Convert.ToInt32(r["UserID"]);
            _roleName = Convert.ToString(r["RoleName"]) ?? "";
            _canManage = _roleName == "Admin" || _roleName == "Warden" || _roleName == "Meruru";

            _btnAdd.Enabled    = _canManage;   // 只有管理员/典狱长/梅露露能新增
            _btnStatus.Enabled = _canManage;   // 同上：能改状态


            // 头像：支持绝对/相对路径（相对路径以程序目录为基准），缺图用占位
            _avatar.Image = null;
            try
            {
                // 这里的 r 来自你上文的 DataRow：var r = dt.Rows[0];
                object avatarObj = r["AvatarPath"];
                string avatarPath = (avatarObj == null || avatarObj == DBNull.Value) ? null : avatarObj.ToString();

                // 占位图相对路径：Images/_placeholder.png
                string placeholder = Path.Combine(AppContext.BaseDirectory, "Images", "_placeholder.png");

                string resolved = null;
                if (!string.IsNullOrWhiteSpace(avatarPath))
                {
                    resolved = Path.IsPathRooted(avatarPath)
                        ? avatarPath
                        : Path.Combine(AppContext.BaseDirectory, avatarPath);
                }

                if (!string.IsNullOrEmpty(resolved) && File.Exists(resolved))
                {
                    _avatar.Image = Image.FromFile(resolved);
                }
                else if (File.Exists(placeholder))
                {
                    _avatar.Image = Image.FromFile(placeholder);
                }
            }
            catch
            {
                // 忽略图片加载异常
            }

                    }

        /// <summary>
        /// 加载岛屿列表
        /// </summary>
        private void LoadIslands()
        {
            var dt = _dal.GetIslands();
            _cbIsland.DisplayMember="Name"; _cbIsland.ValueMember="IslandID"; _cbIsland.DataSource=dt;
        }
        
        /// <summary>
        /// 加载批次列表（根据选中的岛屿）
        /// </summary>
        private void LoadBatches()
        {
            if (_cbIsland.SelectedValue is not int islandId) return;
            var dt = _dal.GetBatches(islandId);
            _cbBatch.DisplayMember="BatchID"; _cbBatch.ValueMember="BatchID"; _cbBatch.DataSource=dt;
        }
        
        /// <summary>
        /// 加载魔女数据网格
        /// </summary>
        private void LoadGrid()
        {
            int? islandId = _cbIsland.SelectedValue as int?;
            int? batchId  = _cbBatch.SelectedValue  as int?;
            var nameLike  = string.IsNullOrWhiteSpace(_tbSearch.Text) ? null : _tbSearch.Text.Trim();

            var dt = _dal.GetWitches(islandId, batchId, nameLike);
            _grid.DataSource = dt;

            // if (_grid.Columns.Contains("WitchID"))   _grid.Columns["WitchID"].HeaderText = "ID";
            // if (_grid.Columns.Contains("Name"))      _grid.Columns["Name"].HeaderText = "姓名";
            // if (_grid.Columns.Contains("Magic"))     _grid.Columns["Magic"].HeaderText = "魔法";
            // if (_grid.Columns.Contains("PrisonerNo"))_grid.Columns["PrisonerNo"].HeaderText = "囚犯编号";
            // if (_grid.Columns.Contains("Status"))    _grid.Columns["Status"].HeaderText = "状态";
            // if (_grid.Columns.Contains("IslandID"))  _grid.Columns["IslandID"].HeaderText = "岛";
            // if (_grid.Columns.Contains("BatchID"))   _grid.Columns["BatchID"].HeaderText = "批次";

            var cols = _grid.Columns;
            var c = cols["WitchID"];   if (c != null) c.HeaderText = "ID";
            c = cols["Name"];          if (c != null) c.HeaderText = "姓名";
            c = cols["Magic"];         if (c != null) c.HeaderText = "魔法";
            c = cols["PrisonerNo"];    if (c != null) c.HeaderText = "囚犯编号";
            c = cols["Status"];        if (c != null) c.HeaderText = "状态";
            c = cols["IslandID"];      if (c != null) c.HeaderText = "岛";
            c = cols["BatchID"];       if (c != null) c.HeaderText = "批次";

            _status.Text = $"共 {dt.Rows.Count} 条";
        }
        
        #endregion

        #region 业务操作方法
        
        /// <summary>
        /// 新增魔女
        /// </summary>
        private void OnAddWitch()
        {
            if (_cbIsland.SelectedValue is not int islandId || _cbBatch.SelectedValue is not int batchId)
            { MessageBox.Show("请先选择岛屿与批次。"); return; }

            string? name = Prompt("姓名（必填）：", "新增魔女"); if (string.IsNullOrWhiteSpace(name)) return;
            string? magic = Prompt("魔法（可空）：", "新增魔女");
            string? prisoner = Prompt("囚犯编号（可空）：", "新增魔女");

            try
            {
                _dal.AddWitch(name.Trim(),
                    string.IsNullOrWhiteSpace(magic)?null:magic.Trim(),
                    string.IsNullOrWhiteSpace(prisoner)?null:prisoner.Trim(),
                    islandId, batchId);
                LoadGrid();
                MessageBox.Show("新增成功（批次>13会被拒绝）。");
            }
            catch (Exception ex)
            {
                MessageBox.Show("新增失败：" + ex.Message);
            }
        }
        
        /// <summary>
        /// 修改密码
        /// </summary>
        private void OnChangePassword()
        {
            // 简易对话框：输入 旧密码 / 新密码 / 确认密码
            using var f = new Form { Width = 360, Height = 220, Text = "修改密码", StartPosition = FormStartPosition.CenterParent };
            var l1 = new Label { Left = 12, Top = 16, Text = "旧密码", AutoSize = true };
            var l2 = new Label { Left = 12, Top = 56, Text = "新密码", AutoSize = true };
            var l3 = new Label { Left = 12, Top = 96, Text = "确认新密码", AutoSize = true };
            var t1 = new TextBox { Left = 100, Top = 12, Width = 220, UseSystemPasswordChar = true };
            var t2 = new TextBox { Left = 100, Top = 52, Width = 220, UseSystemPasswordChar = true };
            var t3 = new TextBox { Left = 100, Top = 92, Width = 220, UseSystemPasswordChar = true };
            var ok = new Button  { Left = 160, Top = 130, Width = 70, Text = "确定", DialogResult = DialogResult.OK };
            var ca = new Button  { Left = 250, Top = 130, Width = 70, Text = "取消", DialogResult = DialogResult.Cancel };
            f.Controls.AddRange(new Control[] { l1, l2, l3, t1, t2, t3, ok, ca });
            f.AcceptButton = ok; f.CancelButton = ca;

            if (f.ShowDialog(this) != DialogResult.OK) return;

            var oldPwd = t1.Text; var newPwd = t2.Text; var confirm = t3.Text;
            if (string.IsNullOrEmpty(oldPwd) || string.IsNullOrEmpty(newPwd))
            { MessageBox.Show("密码不能为空。"); return; }
            if (newPwd != confirm)
            { MessageBox.Show("两次输入的新密码不一致。"); return; }

            try
            {
                var bll = new UserBLL();
                var okChange = bll.ChangePassword(_username, oldPwd, newPwd);
                if (!okChange)
                {
                    MessageBox.Show("修改失败：旧密码不正确或账号不存在。");
                    return;
                }
                MessageBox.Show("密码已更新。请使用新密码重新登录。");
                OnLogout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改密码出错：" + ex.Message);
            }
        }
        
        /// <summary>
        /// 退出登录
        /// </summary>
        private void OnLogout()
        {
            // 简易对话框：输入 旧密码 / 新密码 / 确认密码
            using var f = new Form { Width = 360, Height = 220, Text = "修改密码", StartPosition = FormStartPosition.CenterParent };
            var l1 = new Label { Left = 12, Top = 16, Text = "旧密码", AutoSize = true };
            var l2 = new Label { Left = 12, Top = 56, Text = "新密码", AutoSize = true };
            var l3 = new Label { Left = 12, Top = 96, Text = "确认新密码", AutoSize = true };
            var t1 = new TextBox { Left = 100, Top = 12, Width = 220, UseSystemPasswordChar = true };
            var t2 = new TextBox { Left = 100, Top = 52, Width = 220, UseSystemPasswordChar = true };
            var t3 = new TextBox { Left = 100, Top = 92, Width = 220, UseSystemPasswordChar = true };
            var ok = new Button  { Left = 160, Top = 130, Width = 70, Text = "确定", DialogResult = DialogResult.OK };
            var ca = new Button  { Left = 250, Top = 130, Width = 70, Text = "取消", DialogResult = DialogResult.Cancel };
            f.Controls.AddRange(new Control[] { l1, l2, l3, t1, t2, t3, ok, ca });
            f.AcceptButton = ok; f.CancelButton = ca;

            if (f.ShowDialog(this) != DialogResult.OK) return;

            var oldPwd = t1.Text; var newPwd = t2.Text; var confirm = t3.Text;
            if (string.IsNullOrEmpty(oldPwd) || string.IsNullOrEmpty(newPwd))
            { MessageBox.Show("密码不能为空。"); return; }
            if (newPwd != confirm)
            { MessageBox.Show("两次输入的新密码不一致。"); return; }

            try
            {
                var bll = new UserBLL();
                var okChange = bll.ChangePassword(_username, oldPwd, newPwd);
                if (!okChange)
                {
                    MessageBox.Show("修改失败：旧密码不正确或账号不存在。");
                    return;
                }
                MessageBox.Show("密码已更新。请使用新密码重新登录。");
                OnLogout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改密码出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 更改魔女状态
        /// </summary>
        private void OnChangeWitchStatus()
        {
            if (!_canManage)
            {
                MessageBox.Show("权限不足：仅 Admin/Warden/Meruru 可以更改状态。");
                return;
            }
            if (_grid.CurrentRow == null)
            {
                MessageBox.Show("请先选中一条记录。");
                return;
            }

            // 取选中行 WitchID
            var drv = _grid.CurrentRow.DataBoundItem as System.Data.DataRowView;
            if (drv == null || !drv.Row.Table.Columns.Contains("WitchID"))
            {
                MessageBox.Show("无法获取选中行的 WitchID。");
                return;
            }
            int witchId = Convert.ToInt32(drv["WitchID"]);
            string name = Convert.ToString(drv["Name"]) ?? $"#{witchId}";
            string oldStatus = Convert.ToString(drv["Status"]) ?? "Unknown";

            // 弹出对话框选择新状态 & 处刑结果
            using var f = new Form { Width = 360, Height = 230, Text = "更改状态", StartPosition = FormStartPosition.CenterParent };
            var l1 = new Label { Left = 12, Top = 14, Text = $"目标：{name}", AutoSize = true };
            var l2 = new Label { Left = 12, Top = 44, Text = "当前状态：" + oldStatus, AutoSize = true };
            var l3 = new Label { Left = 12, Top = 74, Text = "新状态", AutoSize = true };
            var cb = new ComboBox { Left = 100, Top = 70, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cb.Items.AddRange(new object[] { "Normal", "OnTrial", "Executed", "Acquitted" });
            cb.SelectedIndex = 0;

            var l4 = new Label { Left = 12, Top = 110, Text = "处刑结果（可空）", AutoSize = true };
            var tb = new TextBox { Left = 140, Top = 106, Width = 180 };

            var ok = new Button { Left = 170, Top = 140, Width = 75, Text = "确定", DialogResult = DialogResult.OK };
            var ca = new Button { Left = 255, Top = 140, Width = 75, Text = "取消", DialogResult = DialogResult.Cancel };
            f.Controls.AddRange(new Control[] { l1, l2, l3, cb, l4, tb, ok, ca });
            f.AcceptButton = ok; f.CancelButton = ca;

            if (f.ShowDialog(this) != DialogResult.OK) return;

            string newStatus = cb.SelectedItem?.ToString() ?? "Normal";
            string? execResult = string.IsNullOrWhiteSpace(tb.Text) ? null : tb.Text.Trim();

            try
            {
                _dal.UpdateStatus(witchId, newStatus, execResult);
                AuditDAL.Log(_userId, _username, "UpdateStatus",
                            $"Witch:{witchId}", $"from {oldStatus} to {newStatus}; result={execResult ?? "(null)"}");

                LoadGrid();
                MessageBox.Show("状态已更新。");
            }
            catch (Exception ex)
            {
                MessageBox.Show("更新失败：" + ex.Message);
            }
        }
        
        #endregion

        #region 辅助方法
        
        /// <summary>
        /// 简单输入对话框
        /// </summary>
        private static string? Prompt(string text, string title)
        {
            using var f=new Form{ Width=380, Height=140, Text=title, StartPosition=FormStartPosition.CenterParent };
            var lbl=new Label{ Left=10, Top=10, Width=340, Text=text };
            var tb =new TextBox{ Left=10, Top=40, Width=340 };
            var ok =new Button{ Text="确定", Left=190, Width=75, Top=70, DialogResult=DialogResult.OK };
            var cancel=new Button{ Text="取消", Left=275, Width=75, Top=70, DialogResult=DialogResult.Cancel };
            f.Controls.AddRange(new Control[]{lbl,tb,ok,cancel}); f.AcceptButton=ok; f.CancelButton=cancel;
            return f.ShowDialog()==DialogResult.OK ? tb.Text : null;
        }
        
        #endregion
    }
}

