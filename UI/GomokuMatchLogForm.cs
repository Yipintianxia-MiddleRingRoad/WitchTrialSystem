using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using WitchTrialSystem.DAL;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 五子棋对局日志界面
    /// </summary>
    public class GomokuMatchLogForm : Form
    {
        private readonly string _currentUsername;
        private readonly GomokuMatchLogDAL _matchLogDAL = new();
        
        // UI 控件
        private DataGridView _dgvMatchLog;
        private ComboBox _cmbFilterType;
        private ComboBox _cmbPlayer1;
        private ComboBox _cmbPlayer2;
        private Button _btnFilter;
        private Button _btnReset;
        private Button _btnClose;
        private Label _lblTitle;
        private Label _lblFilterType;
        private Label _lblPlayer1;
        private Label _lblPlayer2;

        public GomokuMatchLogForm(string username)
        {
            try
            {
                _currentUsername = username;
                InitializeForm();
                LoadPlayers();
                LoadAllMatchLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化对局日志窗口失败：{ex.Message}\n\n堆栈跟踪：{ex.StackTrace}", 
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void InitializeForm()
        {
            Text = "五子棋对局日志";
            Width = 2200;
            Height = 1300;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(240, 240, 240);

            // 标题
            _lblTitle = new Label
            {
                Text = "五子棋对局日志",
                Font = new Font("微软雅黑", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(20, 20),
                Size = new Size(400, 45),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(_lblTitle);

            // 筛选类型
            _lblFilterType = new Label
            {
                Text = "筛选类型：",
                Location = new Point(20, 80),
                Size = new Size(90, 30),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("微软雅黑", 10)
            };
            Controls.Add(_lblFilterType);

            _cmbFilterType = new ComboBox
            {
                Location = new Point(120, 80),
                Width = 150,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("微软雅黑", 10)
            };
            _cmbFilterType.Items.AddRange(new[] { "全部对局", "单个玩家", "两个玩家" });
            _cmbFilterType.SelectedIndex = 0;
            _cmbFilterType.SelectedIndexChanged += OnFilterTypeChanged;
            Controls.Add(_cmbFilterType);

            // 玩家1筛选
            _lblPlayer1 = new Label
            {
                Text = "玩家1：",
                Location = new Point(290, 80),
                Size = new Size(70, 30),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("微软雅黑", 10),
                Visible = false
            };
            Controls.Add(_lblPlayer1);

            _cmbPlayer1 = new ComboBox
            {
                Location = new Point(370, 80),
                Width = 180,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("微软雅黑", 10),
                Visible = false
            };
            Controls.Add(_cmbPlayer1);

            // 玩家2筛选
            _lblPlayer2 = new Label
            {
                Text = "玩家2：",
                Location = new Point(570, 80),
                Size = new Size(70, 30),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("微软雅黑", 10),
                Visible = false
            };
            Controls.Add(_lblPlayer2);

            _cmbPlayer2 = new ComboBox
            {
                Location = new Point(650, 80),
                Width = 180,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("微软雅黑", 10),
                Visible = false
            };
            Controls.Add(_cmbPlayer2);

            // 筛选按钮
            _btnFilter = new Button
            {
                Text = "筛选",
                Location = new Point(850, 78),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("微软雅黑", 11)
            };
            _btnFilter.Click += OnFilterClick;
            Controls.Add(_btnFilter);

            // 重置按钮
            _btnReset = new Button
            {
                Text = "重置",
                Location = new Point(950, 78),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("微软雅黑", 11)
            };
            _btnReset.Click += OnResetClick;
            Controls.Add(_btnReset);

            // 数据表格
            _dgvMatchLog = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(2150, 1080),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };
            Controls.Add(_dgvMatchLog);

            // 关闭按钮
            _btnClose = new Button
            {
                Text = "关闭",
                Location = new Point(2090, 1215),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnClose.Click += (s, e) => this.Close();
            Controls.Add(_btnClose);
        }

        private void LoadPlayers()
        {
            try
            {
                const string sql = @"
SELECT Username, ISNULL(w.Name, Username) AS DisplayName
FROM wt.[User] u
LEFT JOIN wt.UserWitch uw ON uw.UserID = u.UserID
LEFT JOIN wt.Witch w ON w.WitchID = uw.WitchID
WHERE u.RoleID = 4
ORDER BY Username";

                var dt = DBHelper.ExecDataTable(sql);
                
                if (dt == null)
                {
                    MessageBox.Show("无法加载玩家列表：数据库返回空结果", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                _cmbPlayer1.Items.Clear();
                _cmbPlayer2.Items.Clear();
                
                foreach (DataRow row in dt.Rows)
                {
                    string username = row["Username"]?.ToString() ?? "";
                    string displayName = row["DisplayName"]?.ToString() ?? "";
                    string item = $"{username} ({displayName})";
                    _cmbPlayer1.Items.Add(item);
                    _cmbPlayer2.Items.Add(item);
                }
                
                if (_cmbPlayer1.Items.Count > 0)
                {
                    _cmbPlayer1.SelectedIndex = 0;
                    if (_cmbPlayer2.Items.Count > 1)
                        _cmbPlayer2.SelectedIndex = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载玩家列表失败：{ex.Message}\n\n堆栈跟踪：{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllMatchLogs()
        {
            try
            {
                var dt = _matchLogDAL.GetAllMatchLogs();
                if (dt == null)
                {
                    MessageBox.Show("无法加载对局日志：数据库返回空结果", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                SetupDataGridView(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载对局日志失败：{ex.Message}\n\n堆栈跟踪：{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView(DataTable dt)
        {
            if (dt == null)
            {
                return;
            }
            
            _dgvMatchLog.DataSource = dt;
            
            if (_dgvMatchLog.Columns.Count > 0)
            {
                // 设置列顺序和属性
                if (_dgvMatchLog.Columns.Contains("MatchID"))
                {
                    _dgvMatchLog.Columns["MatchID"].HeaderText = "对局序号";
                    _dgvMatchLog.Columns["MatchID"].Width = 100;
                    _dgvMatchLog.Columns["MatchID"].DisplayIndex = 0;
                }
                
                // 玩家1信息
                if (_dgvMatchLog.Columns.Contains("Player1PrisonerNo"))
                {
                    _dgvMatchLog.Columns["Player1PrisonerNo"].HeaderText = "玩家1囚人番号";
                    _dgvMatchLog.Columns["Player1PrisonerNo"].Width = 150;
                    _dgvMatchLog.Columns["Player1PrisonerNo"].DisplayIndex = 1;
                }
                
                if (_dgvMatchLog.Columns.Contains("Player1Name"))
                {
                    _dgvMatchLog.Columns["Player1Name"].HeaderText = "玩家1姓名";
                    _dgvMatchLog.Columns["Player1Name"].Width = 120;
                    _dgvMatchLog.Columns["Player1Name"].DisplayIndex = 2;
                }
                
                // 玩家2信息
                if (_dgvMatchLog.Columns.Contains("Player2PrisonerNo"))
                {
                    _dgvMatchLog.Columns["Player2PrisonerNo"].HeaderText = "玩家2囚人番号";
                    _dgvMatchLog.Columns["Player2PrisonerNo"].Width = 150;
                    _dgvMatchLog.Columns["Player2PrisonerNo"].DisplayIndex = 3;
                }
                
                if (_dgvMatchLog.Columns.Contains("Player2Name"))
                {
                    _dgvMatchLog.Columns["Player2Name"].HeaderText = "玩家2姓名";
                    _dgvMatchLog.Columns["Player2Name"].Width = 120;
                    _dgvMatchLog.Columns["Player2Name"].DisplayIndex = 4;
                }
                
                // 时间信息
                if (_dgvMatchLog.Columns.Contains("StartTime"))
                {
                    _dgvMatchLog.Columns["StartTime"].HeaderText = "开始时间";
                    _dgvMatchLog.Columns["StartTime"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                    _dgvMatchLog.Columns["StartTime"].Width = 200;
                    _dgvMatchLog.Columns["StartTime"].DisplayIndex = 5;
                }
                
                if (_dgvMatchLog.Columns.Contains("EndTime"))
                {
                    _dgvMatchLog.Columns["EndTime"].HeaderText = "结束时间";
                    _dgvMatchLog.Columns["EndTime"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                    _dgvMatchLog.Columns["EndTime"].Width = 200;
                    _dgvMatchLog.Columns["EndTime"].DisplayIndex = 6;
                }
                
                // 玩家1结果
                if (_dgvMatchLog.Columns.Contains("Player1Result"))
                {
                    _dgvMatchLog.Columns["Player1Result"].HeaderText = "玩家1结果";
                    _dgvMatchLog.Columns["Player1Result"].Width = 100;
                    _dgvMatchLog.Columns["Player1Result"].DisplayIndex = 7;
                }
                
                if (_dgvMatchLog.Columns.Contains("Player1ScoreChange"))
                {
                    _dgvMatchLog.Columns["Player1ScoreChange"].HeaderText = "玩家1分数变化";
                    _dgvMatchLog.Columns["Player1ScoreChange"].Width = 150;
                    _dgvMatchLog.Columns["Player1ScoreChange"].DisplayIndex = 8;
                }
                
                // 玩家2结果
                if (_dgvMatchLog.Columns.Contains("Player2Result"))
                {
                    _dgvMatchLog.Columns["Player2Result"].HeaderText = "玩家2结果";
                    _dgvMatchLog.Columns["Player2Result"].Width = 100;
                    _dgvMatchLog.Columns["Player2Result"].DisplayIndex = 9;
                }
                
                if (_dgvMatchLog.Columns.Contains("Player2ScoreChange"))
                {
                    _dgvMatchLog.Columns["Player2ScoreChange"].HeaderText = "玩家2分数变化";
                    _dgvMatchLog.Columns["Player2ScoreChange"].Width = 150;
                    _dgvMatchLog.Columns["Player2ScoreChange"].DisplayIndex = 10;
                }
                
                // 对局统计
                if (_dgvMatchLog.Columns.Contains("TotalMoves"))
                {
                    _dgvMatchLog.Columns["TotalMoves"].HeaderText = "总步数";
                    _dgvMatchLog.Columns["TotalMoves"].Width = 90;
                    _dgvMatchLog.Columns["TotalMoves"].DisplayIndex = 11;
                }
                
                if (_dgvMatchLog.Columns.Contains("Duration"))
                {
                    _dgvMatchLog.Columns["Duration"].HeaderText = "时长(秒)";
                    _dgvMatchLog.Columns["Duration"].Width = 100;
                    _dgvMatchLog.Columns["Duration"].DisplayIndex = 12;
                }
                
                // 隐藏用户名列
                if (_dgvMatchLog.Columns.Contains("Player1Username"))
                    _dgvMatchLog.Columns["Player1Username"].Visible = false;
                if (_dgvMatchLog.Columns.Contains("Player2Username"))
                    _dgvMatchLog.Columns["Player2Username"].Visible = false;
            }
        }

        private void OnFilterTypeChanged(object? sender, EventArgs e)
        {
            switch (_cmbFilterType.SelectedIndex)
            {
                case 0: // 全部对局
                    _lblPlayer1.Visible = false;
                    _cmbPlayer1.Visible = false;
                    _lblPlayer2.Visible = false;
                    _cmbPlayer2.Visible = false;
                    break;
                case 1: // 单个玩家
                    _lblPlayer1.Visible = true;
                    _cmbPlayer1.Visible = true;
                    _lblPlayer2.Visible = false;
                    _cmbPlayer2.Visible = false;
                    break;
                case 2: // 两个玩家
                    _lblPlayer1.Visible = true;
                    _cmbPlayer1.Visible = true;
                    _lblPlayer2.Visible = true;
                    _cmbPlayer2.Visible = true;
                    break;
            }
        }

        private void OnFilterClick(object? sender, EventArgs e)
        {
            try
            {
                DataTable dt;
                
                switch (_cmbFilterType.SelectedIndex)
                {
                    case 0: // 全部对局
                        dt = _matchLogDAL.GetAllMatchLogs();
                        break;
                    case 1: // 单个玩家
                        if (_cmbPlayer1.SelectedItem == null)
                        {
                            MessageBox.Show("请选择玩家", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        string username1 = _cmbPlayer1.SelectedItem.ToString()!.Split(' ')[0];
                        dt = _matchLogDAL.GetMatchLogsByPlayer(username1);
                        break;
                    case 2: // 两个玩家
                        if (_cmbPlayer1.SelectedItem == null || _cmbPlayer2.SelectedItem == null)
                        {
                            MessageBox.Show("请选择两个玩家", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        string usernameA = _cmbPlayer1.SelectedItem.ToString()!.Split(' ')[0];
                        string usernameB = _cmbPlayer2.SelectedItem.ToString()!.Split(' ')[0];
                        dt = _matchLogDAL.GetMatchLogsByTwoPlayers(usernameA, usernameB);
                        break;
                    default:
                        dt = _matchLogDAL.GetAllMatchLogs();
                        break;
                }
                
                SetupDataGridView(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"筛选失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnResetClick(object? sender, EventArgs e)
        {
            _cmbFilterType.SelectedIndex = 0;
            LoadAllMatchLogs();
        }
    }
}
