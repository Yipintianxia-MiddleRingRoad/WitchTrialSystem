using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WitchTrialSystem.BLL;
using WitchTrialSystem.DAL;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 发起审判对话框
    /// 功能：选择参与审判的魔女（2-13人）
    /// </summary>
    public class CreateTrialDialog : Form
    {
        #region 字段定义
        
        private readonly string _username;
        private readonly int _userId;
        private readonly int _islandId;
        
        // UI控件
        private readonly Label _lblTitle = new() { Text = "选择参与审判的魔女（2-13人）", AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
        private readonly Label _lblCount = new() { Text = "已选择：0 人", AutoSize = true, ForeColor = Color.Blue };
        private readonly ComboBox _cbBatch = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
        private readonly CheckedListBox _listWitches = new() { CheckOnClick = true };
        private readonly Button _btnOk = new() { Text = "确定", Width = 100, Height = 35, DialogResult = DialogResult.OK };
        private readonly Button _btnCancel = new() { Text = "取消", Width = 100, Height = 35, DialogResult = DialogResult.Cancel };
        
        #endregion

        #region 构造函数和初始化
        
        /// <summary>
        /// 构造函数：初始化发起审判对话框
        /// </summary>
        public CreateTrialDialog(string username, int userId, int islandId)
        {
            _username = username;
            _userId = userId;
            _islandId = islandId;
            
            InitializeForm();
            SetupLayout();
            LoadBatches();
            SetupEvents();
        }

        /// <summary>
        /// 初始化窗体设置
        /// </summary>
        private void InitializeForm()
        {
            Text = "发起审判";
            Width = 500;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            
            // 设置应用程序图标
            BLL.IconHelper.SetFormIcon(this);
        }

        /// <summary>
        /// 设置界面布局
        /// </summary>
        private void SetupLayout()
        {
            // 顶部面板
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(15),
                // 让顶部区域与窗体背景融为一体，避免形成单独的“色块”
                BackColor = Color.Transparent
            };
            
            // 整体向上移动标题和计数标签，避免与下方列表视觉重叠
            _lblTitle.Location = new Point(15, 8);
            _lblTitle.BackColor = Color.Transparent;
            _lblCount.Location = new Point(15, 38);
            _lblCount.BackColor = Color.Transparent;
            
            // 批次选择说明保持在左侧，只将下拉框整体向右移动，减少与前面文字的拥挤感
            var lblBatch = new Label { Text = "批次：", AutoSize = true, Location = new Point(15, 70), BackColor = Color.Transparent };
            _cbBatch.Location = new Point(90, 67);
            
            topPanel.Controls.Add(_lblTitle);
            topPanel.Controls.Add(_lblCount);
            topPanel.Controls.Add(lblBatch);
            topPanel.Controls.Add(_cbBatch);
            
            // 魔女列表
            _listWitches.Dock = DockStyle.Fill;
            _listWitches.IntegralHeight = false;
            
            // 底部按钮面板
            var bottomPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10)
            };
            
            bottomPanel.Controls.Add(_btnCancel);
            bottomPanel.Controls.Add(_btnOk);
            
            // 添加到窗体
            Controls.Add(_listWitches);
            Controls.Add(bottomPanel);
            Controls.Add(topPanel);
        }

        /// <summary>
        /// 设置事件处理
        /// </summary>
        private void SetupEvents()
        {
            _cbBatch.SelectedIndexChanged += (s, e) => LoadWitches();
            _listWitches.ItemCheck += (s, e) => 
            {
                // 延迟更新计数（ItemCheck事件在状态改变前触发）
                BeginInvoke(new Action(UpdateCount));
            };
            _btnOk.Click += OnOkClick;
        }
        
        #endregion

        #region 数据加载
        
        /// <summary>
        /// 加载批次列表
        /// </summary>
        private void LoadBatches()
        {
            try
            {
                var dal = new WitchDAL();
                var dt = dal.GetBatches(_islandId);
                
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("当前岛屿没有批次数据。", "提示", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                _cbBatch.DisplayMember = "BatchID";
                _cbBatch.ValueMember = "BatchID";
                _cbBatch.DataSource = dt;
                
                // 默认选中第一个批次
                if (_cbBatch.Items.Count > 0)
                {
                    _cbBatch.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载批次失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 加载魔女列表
        /// </summary>
        private void LoadWitches()
        {
            if (_cbBatch.SelectedValue == null) return;
            
            try
            {
                int batchId = Convert.ToInt32(_cbBatch.SelectedValue);
                
                // 查询本岛屿本批次的魔女
                var dal = new WitchDAL();
                var dt = dal.GetWitches(_islandId, batchId, null);
                
                // 筛选可以参与审判的魔女（排除已死亡和待抓捕的）
                var witches = dt.AsEnumerable()
                    .Where(r => 
                    {
                        string status = r["Status"].ToString() ?? "";
                        // 排除：待抓捕、死亡(正常)、死亡(魔女化)
                        return status != "待抓捕" 
                            && status != "死亡(正常)" 
                            && status != "死亡(魔女化)";
                    })
                    .ToList();
                
                if (witches.Count == 0)
                {
                    MessageBox.Show("当前批次没有可参与审判的魔女。\n\n可参与状态：分配至岛屿、审判中、其它\n不可参与：待抓捕、死亡(正常)、死亡(魔女化)", "提示", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _listWitches.Items.Clear();
                    return;
                }
                
                // 填充列表
                _listWitches.Items.Clear();
                foreach (var row in witches)
                {
                    int witchId = Convert.ToInt32(row["WitchID"]);
                    string name = row["Name"].ToString() ?? "未知";
                    string prisonerNo = row["PrisonerNo"].ToString() ?? "";
                    
                    var item = new WitchItem
                    {
                        WitchID = witchId,
                        Name = name,
                        PrisonerNo = prisonerNo
                    };
                    
                    _listWitches.Items.Add(item);
                }
                
                UpdateCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载魔女列表失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 更新选择计数
        /// </summary>
        private void UpdateCount()
        {
            int count = _listWitches.CheckedItems.Count;
            _lblCount.Text = $"已选择：{count} 人";
            
            if (count < 2)
            {
                _lblCount.ForeColor = Color.Red;
            }
            else if (count > 13)
            {
                _lblCount.ForeColor = Color.Red;
            }
            else
            {
                _lblCount.ForeColor = Color.Green;
            }
        }
        
        #endregion

        #region 事件处理
        
        /// <summary>
        /// 点击确定按钮：创建审判会话
        /// </summary>
        private void OnOkClick(object? sender, EventArgs e)
        {
            try
            {
                // 验证选择人数
                int count = _listWitches.CheckedItems.Count;
                if (count < 2)
                {
                    MessageBox.Show("至少需要选择2个魔女参加审判。", "提示", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }
                
                if (count > 13)
                {
                    MessageBox.Show("最多只能选择13个魔女参加审判。", "提示", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }
                
                // 获取选中的魔女ID列表
                var witchIds = new List<int>();
                foreach (WitchItem item in _listWitches.CheckedItems)
                {
                    witchIds.Add(item.WitchID);
                }
                
                // 获取批次ID
                int batchId = Convert.ToInt32(_cbBatch.SelectedValue);
                
                // 创建审判会话
                var result = TrialSessionService.CreateSession(
                    _islandId, batchId, _userId, witchIds);
                
                if (!result.Success)
                {
                    MessageBox.Show($"创建审判失败：{result.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    return;
                }
                
                // 成功
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建审判失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }
        
        #endregion

        #region 辅助类
        
        /// <summary>
        /// 魔女列表项
        /// </summary>
        private class WitchItem
        {
            public int WitchID { get; set; }
            public string Name { get; set; } = "";
            public string PrisonerNo { get; set; } = "";
            
            public override string ToString()
            {
                return $"{PrisonerNo} - {Name}";
            }
        }
        
        #endregion
    }
}
