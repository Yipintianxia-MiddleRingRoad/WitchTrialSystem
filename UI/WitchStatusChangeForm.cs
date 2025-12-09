using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WitchTrialSystem.DAL;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 魔女状态修改对话框
    /// 功能：修改魔女的当前状态
    /// </summary>
    public class WitchStatusChangeForm : Form
    {
        private readonly int _witchId;
        private readonly string _witchName;
        private readonly string _prisonerNo;
        private readonly string _currentStatus;
        
        private readonly WitchDAL _dal = new();
        
        private Label _lblInfo = null!;
        private Label _lblCurrentStatus = null!;
        private Label _lblSelectStatus = null!;
        private ComboBox _cmbStatus = null!;
        private Button _btnOK = null!;
        private Button _btnCancel = null!;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="witchId">魔女ID</param>
        /// <param name="witchName">魔女姓名</param>
        /// <param name="prisonerNo">囚人番号</param>
        /// <param name="currentStatus">当前状态</param>
        public WitchStatusChangeForm(int witchId, string witchName, string prisonerNo, string currentStatus)
        {
            _witchId = witchId;
            _witchName = witchName;
            _prisonerNo = prisonerNo;
            _currentStatus = currentStatus ?? "未知";
            
            InitializeComponents();
            BLL.IconHelper.SetFormIcon(this);  // 设置应用程序图标
            LoadStatuses();
        }

        private void InitializeComponents()
        {
            // 窗体设置
            Text = "修改状态";
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

            // 当前状态标签
            _lblCurrentStatus = new Label
            {
                Text = $"当前状态：{_currentStatus}",
                Left = 20,
                Top = 55,
                Width = 400,
                Height = 25,
                Font = new Font("Microsoft YaHei UI", 9)
            };

            // 选择状态标签
            _lblSelectStatus = new Label
            {
                Text = "选择新状态：",
                Left = 20,
                Top = 95,
                Width = 100,
                Height = 25,
                Font = new Font("Microsoft YaHei UI", 9)
            };

            // 状态下拉框
            _cmbStatus = new ComboBox
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
            Controls.Add(_lblCurrentStatus);
            Controls.Add(_lblSelectStatus);
            Controls.Add(_cmbStatus);
            Controls.Add(_btnOK);
            Controls.Add(_btnCancel);

            AcceptButton = _btnOK;
            CancelButton = _btnCancel;
        }

        /// <summary>
        /// 加载状态列表
        /// </summary>
        private void LoadStatuses()
        {
            try
            {
                // 使用硬编码的状态列表（与国家端保持一致）
                _cmbStatus.Items.AddRange(new object[] 
                { 
                    "待分配", 
                    "分配至岛屿", 
                    "审判中", 
                    "死亡(正常)", 
                    "死亡(魔女化)", 
                    "其它"
                });

                // 默认选中当前状态
                int selectedIndex = 0;
                for (int i = 0; i < _cmbStatus.Items.Count; i++)
                {
                    if (_cmbStatus.Items[i].ToString() == _currentStatus)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
                
                _cmbStatus.SelectedIndex = selectedIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载状态列表失败：{ex.Message}", "错误", 
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
                if (_cmbStatus.SelectedItem == null)
                {
                    MessageBox.Show("请选择状态。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }

                string newStatus = _cmbStatus.SelectedItem.ToString() ?? "";
                
                // 如果选择的状态与当前状态相同，直接返回
                if (newStatus == _currentStatus)
                {
                    MessageBox.Show("未做任何修改。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.Cancel;
                    return;
                }

                // 更新状态
                bool success = _dal.UpdateWitchStatusSimple(_witchId, newStatus);

                if (success)
                {
                    MessageBox.Show($"已将魔女状态修改为：{newStatus}", "成功", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("状态修改失败，请重试。", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"状态修改失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }
    }
}
