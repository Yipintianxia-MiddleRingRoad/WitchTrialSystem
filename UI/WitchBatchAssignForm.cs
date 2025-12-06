using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WitchTrialSystem.DAL;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 魔女批次分配对话框
    /// 功能：为魔女分配或修改本岛批次
    /// </summary>
    public class WitchBatchAssignForm : Form
    {
        private readonly int _witchId;
        private readonly int _islandId;
        private readonly string _witchName;
        private readonly string _prisonerNo;
        private readonly int? _currentLocalBatchId;
        
        private readonly WitchDAL _dal = new();
        
        private Label _lblInfo = null!;
        private Label _lblCurrentBatch = null!;
        private Label _lblSelectBatch = null!;
        private ComboBox _cmbBatch = null!;
        private Button _btnOK = null!;
        private Button _btnCancel = null!;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="witchId">魔女ID</param>
        /// <param name="islandId">岛屿ID</param>
        /// <param name="witchName">魔女姓名</param>
        /// <param name="prisonerNo">囚人番号</param>
        /// <param name="currentLocalBatchId">当前本岛批次ID（null表示待分配）</param>
        public WitchBatchAssignForm(int witchId, int islandId, string witchName, string prisonerNo, int? currentLocalBatchId)
        {
            _witchId = witchId;
            _islandId = islandId;
            _witchName = witchName;
            _prisonerNo = prisonerNo;
            _currentLocalBatchId = currentLocalBatchId;
            
            InitializeComponents();
            LoadBatches();
        }

        private void InitializeComponents()
        {
            // 窗体设置
            Text = "分配批次";
            Width = 450;
            Height = 280;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // 魔女信息标签
            _lblInfo = new Label
            {
                Text = $"魔女：{_witchName} (囚人番号: {_prisonerNo})",
                Left = 20,
                Top = 20,
                Width = 400,
                Height = 25,
                Font = new Font("Microsoft YaHei UI", 10, FontStyle.Bold)
            };

            // 当前批次标签
            string currentBatchText = _currentLocalBatchId.HasValue 
                ? $"批次 {_currentLocalBatchId.Value}" 
                : "待分配";
            
            _lblCurrentBatch = new Label
            {
                Text = $"当前批次：{currentBatchText}",
                Left = 20,
                Top = 55,
                Width = 400,
                Height = 25,
                Font = new Font("Microsoft YaHei UI", 9)
            };

            // 选择批次标签
            _lblSelectBatch = new Label
            {
                Text = "选择新批次：",
                Left = 20,
                Top = 95,
                Width = 100,
                Height = 25,
                Font = new Font("Microsoft YaHei UI", 9)
            };

            // 批次下拉框
            _cmbBatch = new ComboBox
            {
                Left = 20,
                Top = 125,
                Width = 400,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft YaHei UI", 9)
            };

            // 确定按钮
            _btnOK = new Button
            {
                Text = "确定",
                Left = 240,
                Top = 180,
                Width = 80,
                Height = 35,
                DialogResult = DialogResult.OK
            };
            _btnOK.Click += BtnOK_Click;

            // 取消按钮
            _btnCancel = new Button
            {
                Text = "取消",
                Left = 340,
                Top = 180,
                Width = 80,
                Height = 35,
                DialogResult = DialogResult.Cancel
            };

            // 添加控件
            Controls.Add(_lblInfo);
            Controls.Add(_lblCurrentBatch);
            Controls.Add(_lblSelectBatch);
            Controls.Add(_cmbBatch);
            Controls.Add(_btnOK);
            Controls.Add(_btnCancel);

            AcceptButton = _btnOK;
            CancelButton = _btnCancel;
        }

        /// <summary>
        /// 加载批次列表
        /// </summary>
        private void LoadBatches()
        {
            try
            {
                // 创建数据表
                var dt = new DataTable();
                dt.Columns.Add("LocalBatchID", typeof(int));
                dt.Columns.Add("DisplayText", typeof(string));
                dt.Columns.Add("CurrentCount", typeof(int));
                dt.Columns.Add("IsAvailable", typeof(bool));

                // 添加"待分配"选项
                dt.Rows.Add(0, "待分配", 0, true);

                // 获取批次数据
                var batches = _dal.GetLocalBatchesWithCount(_islandId);
                
                foreach (DataRow row in batches.Rows)
                {
                    int localBatchId = Convert.ToInt32(row["LocalBatchID"]);
                    string displayText = row["DisplayText"].ToString() ?? "";
                    int currentCount = Convert.ToInt32(row["CurrentCount"]);
                    bool isAvailable = currentCount < 13;
                    
                    dt.Rows.Add(localBatchId, displayText, currentCount, isAvailable);
                }

                // 绑定数据
                _cmbBatch.DisplayMember = "DisplayText";
                _cmbBatch.ValueMember = "LocalBatchID";
                _cmbBatch.DataSource = dt;

                // 默认选中当前批次
                if (_currentLocalBatchId.HasValue)
                {
                    for (int i = 0; i < _cmbBatch.Items.Count; i++)
                    {
                        var item = ((DataRowView)_cmbBatch.Items[i]).Row;
                        if (Convert.ToInt32(item["LocalBatchID"]) == _currentLocalBatchId.Value)
                        {
                            _cmbBatch.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    // 默认选中"待分配"
                    _cmbBatch.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载批次列表失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 确定按钮点击事件
        /// </summary>
        private void BtnOK_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_cmbBatch.SelectedValue == null)
                {
                    MessageBox.Show("请选择批次。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }

                int selectedLocalBatchId = Convert.ToInt32(_cmbBatch.SelectedValue);
                
                // 如果选择的批次与当前批次相同，直接返回
                if (selectedLocalBatchId == (_currentLocalBatchId ?? 0))
                {
                    MessageBox.Show("未做任何修改。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.Cancel;
                    return;
                }

                // 检查批次是否可用（已满的批次不能选择）
                var selectedRow = ((DataRowView)_cmbBatch.SelectedItem).Row;
                bool isAvailable = Convert.ToBoolean(selectedRow["IsAvailable"]);
                
                if (!isAvailable && selectedLocalBatchId != 0)
                {
                    MessageBox.Show("该批次已满，无法分配。", "提示", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }

                // 更新批次
                int? newLocalBatchId = selectedLocalBatchId == 0 ? null : (int?)selectedLocalBatchId;
                bool success = _dal.UpdateWitchLocalBatch(_witchId, newLocalBatchId, _islandId);

                if (success)
                {
                    string message = newLocalBatchId.HasValue 
                        ? $"已将魔女分配到批次 {newLocalBatchId.Value}。" 
                        : "已将魔女设置为待分配状态。";
                    
                    MessageBox.Show(message, "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("批次分配失败，请重试。", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"批次分配失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }
    }
}
