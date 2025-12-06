using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WitchTrialSystem.BLL;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 移动记录查看界面
    /// 功能：查看和筛选处刑台移动历史记录
    /// </summary>
    public partial class MovementLogViewForm : Form
    {
        #region 字段定义
        
        private readonly string _username;
        private readonly string _roleName;
        private readonly int _islandId;
        private readonly MovementLogService _logService;
        
        // UI控件
        private readonly ComboBox _cbIsland;
        private readonly ComboBox _cbPlatformNumber;
        private readonly ComboBox _cbPosition;
        private readonly Button _btnApplyFilter;
        private readonly Button _btnResetFilter;
        private readonly Button _btnRefresh;
        private readonly Label _lblStatus;
        private readonly DataGridView _grid;
        
        #endregion

        #region 构造函数
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="roleName">角色名</param>
        /// <param name="islandId">岛屿ID</param>
        public MovementLogViewForm(string username, string roleName, int islandId)
        {
            _username = username;
            _roleName = roleName;
            _islandId = islandId;
            _logService = new MovementLogService();
            
            InitializeComponent();
            
            Text = "处刑台移动记录";
            Size = new Size(1200, 700);
            StartPosition = FormStartPosition.CenterScreen;
            
            // 设置图标
            IconHelper.SetFormIcon(this);
            
            // 初始化控件
            _cbIsland = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150, Height = 35 };
            _cbPlatformNumber = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120, Height = 35 };
            _cbPosition = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120, Height = 35 };
            _btnApplyFilter = new Button { Text = "应用筛选", Width = 100, Height = 35 };
            _btnResetFilter = new Button { Text = "重置筛选", Width = 100, Height = 35 };
            _btnRefresh = new Button { Text = "刷新", Width = 80, Height = 35 };
            _lblStatus = new Label { AutoSize = true, ForeColor = Color.DimGray, Padding = new Padding(8, 2, 8, 2) };
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false
            };
            
            // 顶部筛选面板
            var filterPanel = CreateFilterPanel();
            
            // 添加控件（注意顺序：先添加grid，再添加filterPanel，这样filterPanel在上面）
            Controls.Add(_grid);
            Controls.Add(filterPanel);
            
            // 事件绑定
            Load += Form_Load;
            _btnApplyFilter.Click += (s, e) => LoadData();
            _btnResetFilter.Click += (s, e) => ResetFilter();
            _btnRefresh.Click += (s, e) => LoadData();
            _cbIsland.SelectedIndexChanged += (s, e) => LoadPlatformNumbers();
        }
        
        #endregion

        #region UI创建方法
        
        /// <summary>
        /// 创建筛选面板
        /// </summary>
        private FlowLayoutPanel CreateFilterPanel()
        {
            // 使用FlowLayoutPanel自动排列（参考Form1的工具栏）
            var bar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 80,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(15, 15, 15, 15),
                BackColor = Color.WhiteSmoke
            };
            
            // 岛屿（仅Admin）
            if (_roleName == "Admin")
            {
                bar.Controls.Add(new Label { Text = "岛屿", AutoSize = true, Padding = new Padding(0, 8, 6, 0) });
                bar.Controls.Add(_cbIsland);
            }
            
            // 处刑台编号
            bar.Controls.Add(new Label { Text = "处刑台", AutoSize = true, Padding = new Padding(12, 8, 6, 0) });
            bar.Controls.Add(_cbPlatformNumber);
            
            // 位置
            bar.Controls.Add(new Label { Text = "位置", AutoSize = true, Padding = new Padding(12, 8, 6, 0) });
            bar.Controls.Add(_cbPosition);
            
            // 按钮
            bar.Controls.Add(_btnApplyFilter);
            bar.Controls.Add(_btnResetFilter);
            bar.Controls.Add(_btnRefresh);
            bar.Controls.Add(_lblStatus);
            
            return bar;
        }
        
        #endregion

        #region 数据加载方法
        
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void Form_Load(object? sender, EventArgs e)
        {
            try
            {
                // 加载岛屿列表（仅Admin）
                if (_roleName == "Admin")
                {
                    LoadIslands();
                }
                
                // 加载处刑台编号列表
                LoadPlatformNumbers();
                
                // 加载位置列表
                LoadPositions();
                
                // 加载数据
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// 加载岛屿列表
        /// </summary>
        private void LoadIslands()
        {
            // TODO: 从数据库加载岛屿列表
            // 暂时使用固定数据
            var dt = new DataTable();
            dt.Columns.Add("IslandID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(1, "梅露露岛");
            dt.Rows.Add(2, "乌蒂娜岛");
            
            _cbIsland.DisplayMember = "Name";
            _cbIsland.ValueMember = "IslandID";
            _cbIsland.DataSource = dt;
            
            // 选中当前岛屿
            _cbIsland.SelectedValue = _islandId;
        }
        
        /// <summary>
        /// 加载处刑台编号列表
        /// </summary>
        private void LoadPlatformNumbers()
        {
            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(int));
            dt.Columns.Add("Display", typeof(string));
            dt.Rows.Add(0, "全部");
            
            // 添加1-49号处刑台
            for (int i = 1; i <= 49; i++)
            {
                dt.Rows.Add(i, i.ToString());
            }
            
            _cbPlatformNumber.DisplayMember = "Display";
            _cbPlatformNumber.ValueMember = "Value";
            _cbPlatformNumber.DataSource = dt;
            _cbPlatformNumber.SelectedIndex = 0;
        }
        
        /// <summary>
        /// 加载位置列表
        /// </summary>
        private void LoadPositions()
        {
            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(int));
            dt.Columns.Add("Display", typeof(string));
            dt.Rows.Add(0, "全部");
            
            // 添加1-49号位置（地下室）
            for (int i = 1; i <= 49; i++)
            {
                dt.Rows.Add(i, $"地下室-{i}");
            }
            
            // 添加50号位置（审判庭）
            dt.Rows.Add(50, "审判庭");
            
            _cbPosition.DisplayMember = "Display";
            _cbPosition.ValueMember = "Value";
            _cbPosition.DataSource = dt;
            _cbPosition.SelectedIndex = 0;
        }
        
        /// <summary>
        /// 加载移动记录数据
        /// </summary>
        private void LoadData()
        {
            try
            {
                int currentIslandId = _roleName == "Admin" && _cbIsland.SelectedValue != null
                    ? Convert.ToInt32(_cbIsland.SelectedValue)
                    : _islandId;
                
                // 获取筛选条件
                int platformNumber = _cbPlatformNumber.SelectedValue != null ? Convert.ToInt32(_cbPlatformNumber.SelectedValue) : 0;
                int position = _cbPosition.SelectedValue != null ? Convert.ToInt32(_cbPosition.SelectedValue) : 0;
                
                // 查询所有记录（不限时间）
                var logs = _logService.GetLogsByIsland(currentIslandId);
                
                // 应用筛选
                var dt = new DataTable();
                dt.Columns.Add("LogID", typeof(int));
                dt.Columns.Add("MovementTime", typeof(DateTime));
                dt.Columns.Add("PlatformNumber", typeof(int));
                dt.Columns.Add("ToolName", typeof(string));
                dt.Columns.Add("FromLocationDescription", typeof(string));
                dt.Columns.Add("ToLocationDescription", typeof(string));
                dt.Columns.Add("MovementType", typeof(string));
                dt.Columns.Add("TimeSourceDescription", typeof(string));
                
                foreach (var log in logs)
                {
                    // 处刑台编号筛选
                    if (platformNumber != 0 && log.PlatformNumber != platformNumber)
                        continue;
                    
                    // 位置筛选（起始或目标位置匹配）
                    if (position != 0 && log.FromPosition != position && log.ToPosition != position)
                        continue;
                    
                    dt.Rows.Add(
                        log.LogID,
                        log.MovementTime,
                        log.PlatformNumber,
                        log.ToolName ?? "无",
                        log.FromLocationDescription,
                        log.ToLocationDescription,
                        log.MovementType,
                        log.TimeSourceDescription
                    );
                }
                
                // 按时间降序排序
                var view = dt.DefaultView;
                view.Sort = "MovementTime DESC";
                dt = view.ToTable();
                
                // 绑定数据
                _grid.DataSource = dt;
                
                // 配置列显示
                ConfigureGridColumns();
                
                _lblStatus.Text = $"共 {dt.Rows.Count} 条记录";
                _lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"加载失败：{ex.Message}";
                _lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"加载数据失败：{ex.Message}\n\n堆栈跟踪：{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// 配置数据网格列
        /// </summary>
        private void ConfigureGridColumns()
        {
            if (_grid.Columns.Count == 0) return;
            
            var cols = _grid.Columns;
            int displayIndex = 0;
            
            // 隐藏ID列
            if (cols.Contains("LogID"))
            {
                cols["LogID"].Visible = false;
            }
            
            // 移动时间
            if (cols.Contains("MovementTime"))
            {
                var c = cols["MovementTime"];
                c.HeaderText = "移动时间";
                c.Width = 160;
                c.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                c.DisplayIndex = displayIndex++;
            }
            
            // 处刑台编号
            if (cols.Contains("PlatformNumber"))
            {
                var c = cols["PlatformNumber"];
                c.HeaderText = "处刑台编号";
                c.Width = 100;
                c.DisplayIndex = displayIndex++;
            }
            
            // 刑具
            if (cols.Contains("ToolName"))
            {
                var c = cols["ToolName"];
                c.HeaderText = "刑具";
                c.Width = 120;
                c.DisplayIndex = displayIndex++;
            }
            
            // 起始位置描述
            if (cols.Contains("FromLocationDescription"))
            {
                var c = cols["FromLocationDescription"];
                c.HeaderText = "起始位置";
                c.Width = 150;
                c.DisplayIndex = displayIndex++;
            }
            
            // 目标位置描述
            if (cols.Contains("ToLocationDescription"))
            {
                var c = cols["ToLocationDescription"];
                c.HeaderText = "目标位置";
                c.Width = 150;
                c.DisplayIndex = displayIndex++;
            }
            
            // 移动类型
            if (cols.Contains("MovementType"))
            {
                var c = cols["MovementType"];
                c.HeaderText = "移动类型";
                c.Width = 100;
                c.DisplayIndex = displayIndex++;
            }
            
            // 时间来源
            if (cols.Contains("TimeSourceDescription"))
            {
                var c = cols["TimeSourceDescription"];
                c.HeaderText = "时间来源";
                c.Width = 120;
                c.DisplayIndex = displayIndex++;
            }
        }
        
        /// <summary>
        /// 重置筛选条件
        /// </summary>
        private void ResetFilter()
        {
            if (_cbPlatformNumber.Items.Count > 0)
                _cbPlatformNumber.SelectedIndex = 0;
            if (_cbPosition.Items.Count > 0)
                _cbPosition.SelectedIndex = 0;
            
            LoadData();
        }
        
        #endregion
    }
}
