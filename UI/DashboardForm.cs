using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using ScottPlot.WinForms;
using WitchTrialSystem.BLL;
using WitchTrialSystem.Models;

// 解决命名冲突
using Label = System.Windows.Forms.Label;
using Color = System.Drawing.Color;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 智慧可视化大屏窗口
    /// 显示魔女岛屿状态的实时监控数据
    /// </summary>
    public class DashboardForm : Form
    {
        #region 字段定义

        private readonly string _username;
        private readonly string _roleName;
        private readonly int? _userIslandId;
        private readonly DashboardService _service = new();
        private System.Windows.Forms.Timer? _refreshTimer;
        
        // 当前选中的岛屿（用于Admin切换岛屿）
        private int? _currentSelectedIslandId;

        // UI控件
        private Panel _topPanel = new();
        private Label _titleLabel = new();
        private Label _timeLabel = new();
        private Button _btnRefresh = new();
        private Button _btnClose = new();

        // 统计卡片
        private Panel _statsPanel = new();
        private Panel _cardWitches = new();
        private Panel _cardIslands = new();
        private Panel _cardBatches = new();
        private Label _lblWitchCount = new();
        private Label _lblIslandCount = new();
        private Label _lblBatchCount = new();

        // 图表区域
        private TableLayoutPanel _mainPanel = new();
        private FormsPlot _globalPieChart = new();
        private Panel _islandsPanel = new();
        private FormsPlot _heatmapChart = new();
        private Panel _heatmapPanel = new();  // 热力图面板
        private ComboBox _cmbIslandSelector = new();  // 岛屿选择下拉框（放在热力图标题栏）

        #endregion

        #region 构造函数

        // 中文字体名称
        private const string ChineseFontName = "方正小标宋简体";

        public DashboardForm(string username, string roleName, int? userIslandId = null)
        {
            _username = username;
            _roleName = roleName;
            _userIslandId = userIslandId;
            
            // 初始化当前选中的岛屿
            // Regulator用户：使用自己管理的岛屿
            // Admin用户：默认选择第一个岛屿（后续可切换）
            _currentSelectedIslandId = userIslandId;

            // 加载自定义中文字体文件
            LoadChineseFont();

            InitializeForm();
            BuildUI();
            LoadAllData();
            StartAutoRefresh();
        }

        /// <summary>
        /// 加载中文字体文件到 ScottPlot
        /// </summary>
        private void LoadChineseFont()
        {
            try
            {
                // 尝试加载自定义字体文件
                string fontPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Fonts", "方正小标宋简.ttf");
                if (System.IO.File.Exists(fontPath))
                {
                    // 注册字体文件
                    ScottPlot.Fonts.AddFontFile(ChineseFontName, fontPath);
                    ScottPlot.Fonts.Default = ChineseFontName;
                }
            }
            catch
            {
                // 忽略字体加载错误
            }
        }
        
        /// <summary>
        /// 获取中文字体名称
        /// </summary>
        private string GetChineseFontPath()
        {
            return ChineseFontName;
        }


        private void InitializeForm()
        {
            Text = "魔女审判系统 · 智慧监控大屏";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = DashboardColors.Background;
            MinimumSize = new Size(1280, 720);
            KeyPreview = true;
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };
            
            BLL.IconHelper.SetFormIcon(this);
        }

        #endregion

        #region UI构建

        private void BuildUI()
        {
            // 顶部标题栏
            BuildTopPanel();

            // 统计卡片区
            BuildStatsPanel();

            // 主图表区
            BuildMainPanel();

            // 添加控件到窗口
            Controls.Add(_mainPanel);
            Controls.Add(_statsPanel);
            Controls.Add(_topPanel);
        }

        private void BuildTopPanel()
        {
            _topPanel.Dock = DockStyle.Top;
            _topPanel.Height = 70;
            _topPanel.BackColor = DashboardColors.CardBackground;
            _topPanel.Padding = new Padding(20, 15, 20, 15);

            // 标题
            _titleLabel.Text = "🔮 魔女审判系统 · 智慧监控大屏";
            _titleLabel.Font = new Font("微软雅黑", 20, FontStyle.Bold);
            _titleLabel.ForeColor = DashboardColors.TextPrimary;
            _titleLabel.AutoSize = true;
            _titleLabel.Location = new Point(20, 18);
            _topPanel.Controls.Add(_titleLabel);

            // 时间标签 - 字体大、白色、加粗
            _timeLabel.Text = $"实时更新：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            _timeLabel.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            _timeLabel.ForeColor = Color.White;
            _timeLabel.AutoSize = true;
            _topPanel.Controls.Add(_timeLabel);

            // 岛屿选择器（仅Admin可见）
            if (_roleName == "Admin")
            {
                var lblIsland = new Label
                {
                    Text = "选择岛屿：",
                    Font = new Font("微软雅黑", 10),
                    ForeColor = Color.White,
                    AutoSize = true
                };
                _topPanel.Controls.Add(lblIsland);

                _cmbIslandSelector.Font = new Font("微软雅黑", 10);
                _cmbIslandSelector.DropDownStyle = ComboBoxStyle.DropDownList;
                _cmbIslandSelector.Size = new Size(150, 30);
                _cmbIslandSelector.BackColor = Color.White;
                _cmbIslandSelector.ForeColor = Color.Black;
                _cmbIslandSelector.SelectedIndexChanged += OnIslandSelectionChanged;
                _topPanel.Controls.Add(_cmbIslandSelector);

                // 加载岛屿列表
                LoadIslandSelector();
            }

            // 刷新按钮
            _btnRefresh.Text = "🔄 刷新";
            _btnRefresh.Size = new Size(100, 35);
            _btnRefresh.Font = new Font("微软雅黑", 10);
            _btnRefresh.ForeColor = Color.White;
            _btnRefresh.BackColor = DashboardColors.Primary;
            _btnRefresh.FlatStyle = FlatStyle.Flat;
            _btnRefresh.FlatAppearance.BorderSize = 0;
            _btnRefresh.Click += (s, e) => RefreshData();
            _topPanel.Controls.Add(_btnRefresh);

            // 关闭按钮
            _btnClose.Text = "✖ 关闭";
            _btnClose.Size = new Size(100, 35);
            _btnClose.Font = new Font("微软雅黑", 10);
            _btnClose.ForeColor = Color.White;
            _btnClose.BackColor = Color.FromArgb(192, 57, 43);
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.FlatAppearance.BorderSize = 0;
            _btnClose.Click += (s, e) => Close();
            _topPanel.Controls.Add(_btnClose);

            // 布局调整
            _topPanel.Resize += (s, e) =>
            {
                int rightX = _topPanel.Width - 20;
                _btnClose.Location = new Point(rightX - 100, 17);
                rightX -= 110;
                _btnRefresh.Location = new Point(rightX - 100, 17);
                
                // 岛屿选择器位置（如果是Admin）
                if (_roleName == "Admin")
                {
                    rightX -= 110;
                    _cmbIslandSelector.Location = new Point(rightX - 150, 20);
                    rightX -= 160;
                    var lblIsland = _topPanel.Controls.OfType<Label>().FirstOrDefault(l => l.Text == "选择岛屿：");
                    if (lblIsland != null)
                    {
                        lblIsland.Location = new Point(rightX - lblIsland.Width, 24);
                    }
                }
                
                // 时间标签居中显示
                int centerX = (_topPanel.Width - _timeLabel.Width) / 2;
                _timeLabel.Location = new Point(centerX, 22);
            };
        }

        private void BuildStatsPanel()
        {
            _statsPanel.Dock = DockStyle.Top;
            _statsPanel.Height = 130;  // 从120增加到130，确保数字不被遮挡
            _statsPanel.BackColor = DashboardColors.Background;
            _statsPanel.Padding = new Padding(20, 5, 20, 5);  // 减小上下内边距从10到5

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false  // 删除滚动条
            };

            // 魔女总数卡片
            _cardWitches = CreateStatCard("📊 魔女总数", "0", DashboardColors.Primary, out _lblWitchCount);
            flow.Controls.Add(_cardWitches);

            // 岛屿数卡片
            _cardIslands = CreateStatCard("🏝️ 岛屿数", "0", DashboardColors.Secondary, out _lblIslandCount);
            flow.Controls.Add(_cardIslands);

            // 批次数卡片
            _cardBatches = CreateStatCard("📦 批次数", "0", DashboardColors.Accent, out _lblBatchCount);
            flow.Controls.Add(_cardBatches);

            _statsPanel.Controls.Add(flow);
        }

        private Panel CreateStatCard(string title, string value, Color accentColor, out Label valueLabel)
        {
            var card = new Panel
            {
                Width = 480,  // 进一步增加宽度到480，确保"5/20"等数字完整显示
                Height = 90,
                Margin = new Padding(10),
                BackColor = DashboardColors.CardBackground,
                Padding = new Padding(15)
            };

            // 左侧色条
            var colorBar = new Panel
            {
                Width = 5,
                Dock = DockStyle.Left,
                BackColor = accentColor
            };
            card.Controls.Add(colorBar);

            // 标题
            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("微软雅黑", 11),
                ForeColor = DashboardColors.TextSecondary,
                AutoSize = true,
                Location = new Point(25, 12)
            };
            card.Controls.Add(lblTitle);

            // 数值 - 使用MaximumSize确保不会被截断
            valueLabel = new Label
            {
                Text = value,
                Font = new Font("微软雅黑", 28, FontStyle.Bold),
                ForeColor = DashboardColors.TextPrimary,
                AutoSize = true,
                MaximumSize = new Size(440, 50),  // 设置最大宽度，防止被截断
                Location = new Point(25, 38)
            };
            card.Controls.Add(valueLabel);

            return card;
        }

        private void BuildMainPanel()
        {
            _mainPanel.Dock = DockStyle.Fill;
            _mainPanel.BackColor = DashboardColors.Background;
            _mainPanel.Padding = new Padding(20);
            _mainPanel.ColumnCount = 2;
            _mainPanel.RowCount = 2;
            _mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            _mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            _mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            _mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            // 全局饼图
            var piePanel = CreateChartPanel("🌍 全局状态分布");
            _globalPieChart.Dock = DockStyle.Fill;
            _globalPieChart.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#28283c");
            _globalPieChart.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#28283c");
            piePanel.Controls.Add(_globalPieChart);
            _mainPanel.Controls.Add(piePanel, 0, 0);

            // 岛屿区域
            _islandsPanel = CreateChartPanel("🏝️ 岛屿状态分布");
            _mainPanel.Controls.Add(_islandsPanel, 1, 0);

            // 热力图
            var heatmapPanel = CreateChartPanel("📊 批次状态矩阵");
            _heatmapChart.Dock = DockStyle.Fill;
            _heatmapChart.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#28283c");
            _heatmapChart.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#28283c");
            heatmapPanel.Controls.Add(_heatmapChart);
            _mainPanel.Controls.Add(heatmapPanel, 0, 1);
            _mainPanel.SetColumnSpan(heatmapPanel, 2);
        }

        private Panel CreateChartPanel(string title)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = DashboardColors.CardBackground,
                Margin = new Padding(10),
                Padding = new Padding(10)
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("微软雅黑", 12, FontStyle.Bold),
                ForeColor = DashboardColors.TextPrimary,
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panel.Controls.Add(lblTitle);

            return panel;
        }

        #endregion

        #region 岛屿选择器

        /// <summary>
        /// 加载岛屿选择器数据
        /// </summary>
        private void LoadIslandSelector()
        {
            if (_roleName != "Admin") return;

            try
            {
                _cmbIslandSelector.Items.Clear();
                
                // 获取所有岛屿
                var islands = _service.GetIslands(_username, _roleName, null);
                
                foreach (var island in islands)
                {
                    _cmbIslandSelector.Items.Add(new IslandItem
                    {
                        IslandId = island.IslandId,
                        Name = island.Name
                    });
                }

                // 默认选择第一个岛屿
                if (_cmbIslandSelector.Items.Count > 0)
                {
                    _cmbIslandSelector.SelectedIndex = 0;
                    var firstIsland = _cmbIslandSelector.Items[0] as IslandItem;
                    _currentSelectedIslandId = firstIsland?.IslandId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载岛屿列表失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 岛屿选择变更事件 - 只刷新热力图
        /// </summary>
        private void OnIslandSelectionChanged(object? sender, EventArgs e)
        {
            if (_cmbIslandSelector.SelectedItem is IslandItem selectedIsland)
            {
                _currentSelectedIslandId = selectedIsland.IslandId;
                // 只刷新热力图，不刷新其他数据
                RefreshHeatmap();
            }
        }
        
        /// <summary>
        /// 刷新热力图数据
        /// </summary>
        private void RefreshHeatmap()
        {
            try
            {
                // 使用当前选中的岛屿ID加载热力图数据
                var cells = _service.GetBatchStatusCells(_username, _roleName, _currentSelectedIslandId);
                UpdateHeatmap(cells);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新热力图失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 岛屿选择器项
        /// </summary>
        private class IslandItem
        {
            public int IslandId { get; set; }
            public string Name { get; set; } = "";

            public override string ToString() => Name;
        }

        #endregion

        #region 数据加载

        private void LoadAllData()
        {
            try
            {
                // 确定要查询的岛屿ID
                // Regulator：使用自己管理的岛屿
                // Admin：使用当前选中的岛屿
                int? queryIslandId = _roleName == "Admin" ? _currentSelectedIslandId : _userIslandId;

                // 加载统计数据
                var stats = _service.GetGlobalStats(_username, _roleName, queryIslandId);
                _lblWitchCount.Text = stats.TotalWitches.ToString();
                _lblIslandCount.Text = stats.TotalIslands.ToString();
                _lblBatchCount.Text = $"{stats.ActiveBatches}/{stats.TotalBatches}";

                // 加载状态分布
                var statusData = _service.GetStatusDistribution(_username, _roleName, queryIslandId);
                UpdatePieChart(statusData);

                // 加载岛屿数据
                var islands = _service.GetIslands(_username, _roleName, queryIslandId);
                UpdateIslandsPanel(islands);

                // 加载热力图数据（显示本岛屿的LocalBatch）
                var cells = _service.GetBatchStatusCells(_username, _roleName, queryIslandId);
                UpdateHeatmap(cells);

                // 更新时间
                _timeLabel.Text = $"实时更新：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePieChart(List<StatusCount> data)
        {
            _globalPieChart.Plot.Clear();

            if (data.Count == 0)
            {
                _globalPieChart.Plot.Title("暂无数据");
                _globalPieChart.Refresh();
                return;
            }

            // 创建饼图切片 - 直接使用数据中已经根据状态分配好的颜色
            var slices = new List<ScottPlot.PieSlice>();
            foreach (var item in data)
            {
                var slice = new ScottPlot.PieSlice
                {
                    Value = item.Count,
                    Label = $"{item.Status}: {item.Count}人 ({item.Percentage:F1}%)",
                    FillColor = ScottPlot.Color.FromColor(item.Color)  // 使用数据中已分配的正确颜色
                };
                // 设置切片标签样式 - 白色字体，放大
                slice.LabelStyle.ForeColor = ScottPlot.Colors.White;
                slice.LabelStyle.FontSize = 24;
                slice.LabelStyle.Bold = true;
                // 使用系统字体名称
                slice.LabelStyle.FontName = "Microsoft YaHei";
                slices.Add(slice);
            }

            var pie = _globalPieChart.Plot.Add.Pie(slices);
            pie.ExplodeFraction = 0.02;

            // 设置图例 - 使用中文字体
            _globalPieChart.Plot.ShowLegend();
            var legend = _globalPieChart.Plot.Legend;
            legend.ManualItems.Clear();
            
            // 手动添加图例项 - 使用数据中的颜色
            foreach (var item in data)
            {
                legend.ManualItems.Add(new ScottPlot.LegendItem
                {
                    LabelText = $"{item.Status}: {item.Count}人 ({item.Percentage:F1}%)",
                    FillColor = ScottPlot.Color.FromColor(item.Color)  // 使用数据中已分配的正确颜色
                });
            }
            
            legend.FontName = ScottPlot.Fonts.Default;
            legend.FontSize = 16;
            legend.FontColor = ScottPlot.Colors.White;
            legend.BackgroundColor = ScottPlot.Color.FromHex("#28283c");
            legend.OutlineColor = ScottPlot.Color.FromHex("#555577");
            legend.Alignment = ScottPlot.Alignment.LowerLeft;
            
            _globalPieChart.Refresh();
        }


        private void UpdateIslandsPanel(List<IslandInfo> islands)
        {
            // 清除现有控件（保留标题）
            var controls = new List<Control>();
            foreach (Control c in _islandsPanel.Controls)
            {
                if (c is Label lbl && lbl.Dock == DockStyle.Top)
                    continue;
                controls.Add(c);
            }
            foreach (var c in controls)
            {
                _islandsPanel.Controls.Remove(c);
                c.Dispose();
            }

            if (islands.Count == 0)
            {
                var lblNoData = new Label
                {
                    Text = "暂无岛屿数据",
                    Font = new Font("微软雅黑", 12),
                    ForeColor = DashboardColors.TextMuted,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                _islandsPanel.Controls.Add(lblNoData);
                return;
            }

            // 创建岛屿信息面板 - 不使用滚动条
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,  // 去掉滚动条
                Padding = new Padding(5)
            };

            int index = 0;
            foreach (var island in islands)
            {
                var islandPanel = CreateIslandInfoPanel(island, index);
                flow.Controls.Add(islandPanel);
                index++;
            }

            _islandsPanel.Controls.Add(flow);
        }

        private Panel CreateIslandInfoPanel(IslandInfo island, int index)
        {
            // 第一个岛屿顶部边距15px，第二个岛屿顶部边距35px（额外增加20px）
            int topMargin = index == 0 ? 15 : 35;
            
            // 根据状态数量计算所需高度：标题42 + 人数30 + 起始位置3 + (状态数 × 行间距28)
            int requiredHeight = 75 + (island.StatusDistribution.Count * 28) + 20;  // 额外加20px底部空间
            int minHeight = Math.Max(220, requiredHeight);  // 至少220px
            
            var panel = new Panel
            {
                Dock = DockStyle.Fill,  // 占满可用空间
                MinimumSize = new Size(650, minHeight),  // 根据内容动态设置最小高度
                Margin = new Padding(5, topMargin, 5, 10),  // 根据索引调整顶部边距
                BackColor = DashboardColors.PanelBackground,
                Padding = new Padding(15)
            };

            // 岛屿名称和人数
            var lblName = new Label
            {
                Text = $"🏝️ {island.Name}",
                Font = new Font("微软雅黑", 14, FontStyle.Bold),
                ForeColor = DashboardColors.TextPrimary,
                AutoSize = true,
                Location = new Point(15, 12)
            };
            panel.Controls.Add(lblName);

            var lblCount = new Label
            {
                Text = $"总人数: {island.WitchCount}人",
                Font = new Font("微软雅黑", 12),
                ForeColor = DashboardColors.TextSecondary,
                AutoSize = true,
                Location = new Point(15, 42)
            };
            panel.Controls.Add(lblCount);

            // 状态分布 - 增加行间距
            int y = 75;  // 起始位置
            int lineSpacing = 28;  // 行间距从20增加到28
            foreach (var status in island.StatusDistribution)
            {
                var colorBox = new Panel
                {
                    Size = new Size(16, 16),  // 稍微增大色块
                    Location = new Point(18, y + 2),
                    BackColor = status.Color
                };
                panel.Controls.Add(colorBox);

                var lblStatus = new Label
                {
                    Text = $"{status.Status}: {status.Count}人 ({status.Percentage:F1}%)",
                    Font = new Font("微软雅黑", 11),
                    ForeColor = DashboardColors.TextSecondary,
                    AutoSize = true,
                    Location = new Point(42, y)
                };
                panel.Controls.Add(lblStatus);
                y += lineSpacing;  // 使用更大的行间距
            }

            return panel;
        }

        private void UpdateHeatmap(List<BatchStatusCell> cells)
        {
            _heatmapChart.Plot.Clear();

            if (cells.Count == 0)
            {
                _heatmapChart.Plot.Title("暂无批次数据");
                _heatmapChart.Refresh();
                return;
            }

            // 固定的6种状态分类（严格按照用户要求的顺序和名称）
            var statuses = new List<string>
            {
                "待抓捕",
                "分配至岛屿",
                "审判中",
                "死亡(正常)",
                "死亡(魔女化)",
                "其它"
            };

            // 获取所有批次
            var batches = cells.Select(c => c.LocalBatchId).Distinct().OrderBy(x => x).ToList();

            if (batches.Count == 0)
            {
                _heatmapChart.Plot.Title("暂无有效数据");
                _heatmapChart.Refresh();
                return;
            }

            // 状态映射函数：将数据库中的状态映射到6种固定分类
            string MapStatus(string dbStatus)
            {
                return dbStatus switch
                {
                    "待抓捕" => "待抓捕",
                    "分配至岛屿" => "分配至岛屿",
                    "审判中" => "审判中",
                    "死亡（正常）" => "死亡(正常)",  // 注意：数据库可能用全角括号
                    "死亡(正常)" => "死亡(正常)",
                    "死亡（魔女化）" => "死亡(魔女化)",  // 注意：数据库可能用全角括号
                    "死亡(魔女化)" => "死亡(魔女化)",
                    _ => "其它"  // 所有其他状态（如"已处刑"等）都归入"其它"
                };
            }

            // 创建热力图数据（6行 × N列）
            double[,] heatmapData = new double[statuses.Count, batches.Count];
            
            // 遍历所有单元格，将数据映射到固定的6种状态
            foreach (var cell in cells)
            {
                if (cell.Status == "无") continue;
                
                string mappedStatus = MapStatus(cell.Status);
                int statusIndex = statuses.IndexOf(mappedStatus);
                int batchIndex = batches.IndexOf(cell.LocalBatchId);
                
                if (statusIndex >= 0 && batchIndex >= 0)
                {
                    heatmapData[statusIndex, batchIndex] += cell.Count;
                }
            }

            var heatmap = _heatmapChart.Plot.Add.Heatmap(heatmapData);
            heatmap.Colormap = new ScottPlot.Colormaps.Turbo();
            
            // 在每个单元格上添加数字标注 - 显示所有值包括0
            string fontPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Fonts", "msyh.ttf");
            for (int i = 0; i < statuses.Count; i++)
            {
                for (int j = 0; j < batches.Count; j++)
                {
                    int count = (int)heatmapData[i, j];
                    var text = _heatmapChart.Plot.Add.Text(count.ToString(), j, i);
                    text.LabelFontSize = 16;
                    text.LabelBold = true;
                    text.LabelFontColor = ScottPlot.Colors.White;
                    text.LabelFontName = "Microsoft YaHei";  // 使用系统字体
                }
            }

            // 确定批次标签类型
            // Regulator或Admin选择了岛屿：显示"本岛批次"
            // Admin未选择岛屿：显示"全局批次"
            string batchLabel = (_roleName == "Admin" && _currentSelectedIslandId.HasValue) || _userIslandId.HasValue 
                ? "本岛批次" 
                : "全局批次";
            
            // 设置X轴（批次）标签 - 明确标注批次类型
            _heatmapChart.Plot.Axes.Bottom.Label.Text = batchLabel;
            _heatmapChart.Plot.Axes.Bottom.Label.ForeColor = ScottPlot.Colors.White;
            _heatmapChart.Plot.Axes.Bottom.Label.FontSize = 14;
            _heatmapChart.Plot.Axes.Bottom.Label.Bold = true;
            if (System.IO.File.Exists(fontPath))
            {
                _heatmapChart.Plot.Axes.Bottom.Label.FontName = fontPath;
            }
            
            _heatmapChart.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                batches.Select((b, i) => new ScottPlot.Tick(i, $"批次{b}")).ToArray()
            );
            _heatmapChart.Plot.Axes.Bottom.TickLabelStyle.FontName = ScottPlot.Fonts.Default;
            _heatmapChart.Plot.Axes.Bottom.TickLabelStyle.FontSize = 14;
            _heatmapChart.Plot.Axes.Bottom.TickLabelStyle.ForeColor = ScottPlot.Colors.White;

            // 设置Y轴（状态）标签
            _heatmapChart.Plot.Axes.Left.Label.Text = "魔女状态";
            _heatmapChart.Plot.Axes.Left.Label.ForeColor = ScottPlot.Colors.White;
            _heatmapChart.Plot.Axes.Left.Label.FontSize = 14;
            _heatmapChart.Plot.Axes.Left.Label.Bold = true;
            if (System.IO.File.Exists(fontPath))
            {
                _heatmapChart.Plot.Axes.Left.Label.FontName = fontPath;
            }
            
            var shortStatuses = statuses.Select(s => GetShortStatus(s)).ToList();
            _heatmapChart.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                shortStatuses.Select((s, i) => new ScottPlot.Tick(i, s)).ToArray()
            );
            _heatmapChart.Plot.Axes.Left.TickLabelStyle.FontName = ScottPlot.Fonts.Default;
            _heatmapChart.Plot.Axes.Left.TickLabelStyle.FontSize = 14;
            _heatmapChart.Plot.Axes.Left.TickLabelStyle.ForeColor = ScottPlot.Colors.White;

            // 添加图例说明颜色含义
            _heatmapChart.Plot.ShowLegend();
            var legend = _heatmapChart.Plot.Legend;
            legend.ManualItems.Clear();
            legend.ManualItems.Add(new ScottPlot.LegendItem
            {
                LabelText = "颜色深浅表示人数多少",
                FillColor = ScottPlot.Colors.Transparent
            });
            legend.ManualItems.Add(new ScottPlot.LegendItem
            {
                LabelText = "深色 = 人数多",
                FillColor = ScottPlot.Color.FromHex("#FF0000")
            });
            legend.ManualItems.Add(new ScottPlot.LegendItem
            {
                LabelText = "浅色 = 人数少",
                FillColor = ScottPlot.Color.FromHex("#00FF00")
            });
            legend.FontName = ScottPlot.Fonts.Default;
            legend.FontSize = 12;
            legend.FontColor = ScottPlot.Colors.White;
            legend.BackgroundColor = ScottPlot.Color.FromHex("#28283c");
            legend.OutlineColor = ScottPlot.Color.FromHex("#555577");
            legend.Alignment = ScottPlot.Alignment.UpperRight;
            
            // 通过增加图表上边距来为图例留出空间
            _heatmapChart.Plot.Axes.Top.MinimumSize = 80;

            // 增加边距给中文标签留空间
            _heatmapChart.Plot.Axes.Left.MinimumSize = 140;
            _heatmapChart.Plot.Axes.Bottom.MinimumSize = 70;  // 增加底部空间给轴标签

            _heatmapChart.Refresh();
        }

        /// <summary>
        /// 获取状态的简短显示名称
        /// </summary>
        private static string GetShortStatus(string status)
        {
            // 直接返回状态名称，不做缩写（用户要求使用完全一致的字符串）
            return status;
        }

        #endregion

        #region 自动刷新

        private void StartAutoRefresh()
        {
            _refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 30000 // 30秒
            };
            _refreshTimer.Tick += (s, e) => RefreshData();
            _refreshTimer.Start();
        }

        private void RefreshData()
        {
            try
            {
                _btnRefresh.Enabled = false;
                _btnRefresh.Text = "刷新中...";
                Application.DoEvents();

                LoadAllData();

                _btnRefresh.Text = "🔄 刷新";
                _btnRefresh.Enabled = true;
            }
            catch (Exception ex)
            {
                _btnRefresh.Text = "🔄 刷新";
                _btnRefresh.Enabled = true;
                MessageBox.Show($"刷新数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
            base.OnFormClosing(e);
        }

        #endregion
    }
}
