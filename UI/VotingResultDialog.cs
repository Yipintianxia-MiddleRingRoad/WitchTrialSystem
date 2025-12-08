using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WitchTrialSystem.BLL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.UI
{
    public class VotingResultDialog : Form
    {
        private readonly int _sessionId;
        private readonly int _userId;
        private TrialSessionModel? _session = null;
        private Dictionary<int, int>? _statistics = null;
        
        private readonly Label _lblTitle = new() { Text = "投票结果", AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
        private readonly DataGridView _gridStatistics = new() { ReadOnly = true, AllowUserToAddRows = false };
        private readonly DataGridView _gridDetails = new() { ReadOnly = true, AllowUserToAddRows = false };
        private readonly Button _btnConfirm = new() { Text = "确认处刑对象", Width = 140, Height = 40 };
        private readonly Button _btnClose = new() { Text = "关闭", Width = 100, Height = 35, DialogResult = DialogResult.Cancel };
        
        public VotingResultDialog(int sessionId, int userId)
        {
            _sessionId = sessionId;
            _userId = userId;
            
            InitializeForm();
            SetupLayout();
            LoadData();
            SetupEvents();
        }

        private void InitializeForm()
        {
            Text = "投票结果";
            Width = 800;
            Height = 700;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            BLL.IconHelper.SetFormIcon(this);
        }

        private void SetupLayout()
        {
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(15)
            };
            
            _lblTitle.Location = new Point(15, 15);
            topPanel.Controls.Add(_lblTitle);
            
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300
            };
            
            var lblStats = new Label 
            { 
                Text = "投票统计", 
                Dock = DockStyle.Top, 
                Height = 30, 
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Padding(5)
            };
            _gridStatistics.Dock = DockStyle.Fill;
            _gridStatistics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _gridStatistics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            splitContainer.Panel1.Controls.Add(_gridStatistics);
            splitContainer.Panel1.Controls.Add(lblStats);
            
            var lblDetails = new Label 
            { 
                Text = "投票详情", 
                Dock = DockStyle.Top, 
                Height = 30, 
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Padding(5)
            };
            _gridDetails.Dock = DockStyle.Fill;
            _gridDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _gridDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            splitContainer.Panel2.Controls.Add(_gridDetails);
            splitContainer.Panel2.Controls.Add(lblDetails);
            
            var bottomPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10)
            };
            
            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnConfirm);
            
            Controls.Add(splitContainer);
            Controls.Add(bottomPanel);
            Controls.Add(topPanel);
        }

        private void SetupEvents()
        {
            _btnConfirm.Click += OnConfirmClick;
        }
        
        private void LoadData()
        {
            try
            {
                _session = TrialSessionService.GetSessionByID(_sessionId);
                if (_session == null)
                {
                    MessageBox.Show("审判会话不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
                
                _statistics = TrialSessionService.GetVotingStatistics(_sessionId);
                LoadStatistics();
                LoadDetails();
                
                if (_session.Status == "Voting")
                {
                    _btnConfirm.Enabled = true;
                    _btnConfirm.Text = "确认处刑对象";
                }
                else if (_session.Status == "Confirmed" || _session.Status == "Executing" || _session.Status == "Completed")
                {
                    _btnConfirm.Enabled = false;
                    _btnConfirm.Text = "已确认";
                }
                else
                {
                    _btnConfirm.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载投票数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void LoadStatistics()
        {
            if (_statistics == null) return;
            
            try
            {
                var participants = TrialVotingService.GetParticipants(_sessionId);
                var dt = new DataTable();
                dt.Columns.Add("排名", typeof(int));
                dt.Columns.Add("姓名", typeof(string));
                dt.Columns.Add("得票数", typeof(int));
                dt.Columns.Add("得票率", typeof(string));
                dt.Columns.Add("WitchID", typeof(int));
                
                int totalVotes = _statistics.Values.Sum();
                int rank = 1;
                foreach (var kvp in _statistics.OrderByDescending(kv => kv.Value))
                {
                    int witchId = kvp.Key;
                    int voteCount = kvp.Value;
                    var participant = participants.Find(p => p.WitchID == witchId);
                    string witchName = participant?.WitchName ?? "未知";
                    double percentage = totalVotes > 0 ? (double)voteCount / totalVotes * 100 : 0;
                    dt.Rows.Add(rank, witchName, voteCount, $"{percentage:F1}%", witchId);
                    rank++;
                }
                
                _gridStatistics.DataSource = dt;
                if (_gridStatistics.Columns.Contains("WitchID"))
                {
                    _gridStatistics.Columns["WitchID"].Visible = false;
                }
                
                if (_gridStatistics.Rows.Count > 0)
                {
                    _gridStatistics.Rows[0].DefaultCellStyle.BackColor = Color.LightCoral;
                    _gridStatistics.Rows[0].DefaultCellStyle.Font = new Font(_gridStatistics.Font, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载投票统计失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDetails()
        {
            try
            {
                var details = TrialSessionService.GetVotingDetails(_sessionId);
                var dt = new DataTable();
                dt.Columns.Add("投票者", typeof(string));
                dt.Columns.Add("投给", typeof(string));
                dt.Columns.Add("投票时间", typeof(string));
                
                foreach (var detail in details)
                {
                    string voterName = detail.WitchName;
                    string votedFor = detail.VotedForWitchName ?? "-";
                    string votedAt = detail.VotedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-";
                    dt.Rows.Add(voterName, votedFor, votedAt);
                }
                
                _gridDetails.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载投票详情失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void OnConfirmClick(object? sender, EventArgs e)
        {
            if (_statistics == null || _session == null) return;
            
            try
            {
                if (_statistics.Count == 0)
                {
                    MessageBox.Show("没有人获得投票。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                int maxVotes = _statistics.Values.Max();
                var topVoters = _statistics.Where(kv => kv.Value == maxVotes).Select(kv => kv.Key).ToList();
                int targetWitchId;
                string targetName;
                var participants = TrialVotingService.GetParticipants(_sessionId);
                
                if (topVoters.Count == 1)
                {
                    targetWitchId = topVoters[0];
                    var participant = participants.Find(p => p.WitchID == targetWitchId);
                    targetName = participant?.WitchName ?? "未知";
                }
                else
                {
                    var candidates = topVoters.Select(witchId =>
                    {
                        var p = participants.Find(p => p.WitchID == witchId);
                        return new { WitchID = witchId, WitchName = p?.WitchName ?? "未知", VoteCount = maxVotes };
                    }).ToList();
                    
                    targetWitchId = ShowSelectionDialog(candidates);
                    if (targetWitchId == 0) return;
                    var target = candidates.First(c => c.WitchID == targetWitchId);
                    targetName = target.WitchName;
                }
                
                var result = MessageBox.Show($"确定要选择\"{targetName}\"作为处刑对象吗？\n\n得票数：{maxVotes} 票", 
                    "确认处刑对象", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result != DialogResult.Yes) return;
                
                var confirmResult = TrialSessionService.ConfirmExecutionTarget(_sessionId, targetWitchId, _userId);
                
                if (confirmResult.Success)
                {
                    MessageBox.Show("处刑对象已确认！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show($"确认失败：{confirmResult.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"确认处刑对象失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ShowSelectionDialog(dynamic candidates)
        {
            using var dialog = new Form
            {
                Text = "选择处刑对象",
                Width = 400,
                Height = 300,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };
            
            var lblInfo = new Label
            {
                Text = "以下魔女得票数相同，请选择一个作为处刑对象：",
                AutoSize = false,
                Width = 360,
                Height = 40,
                Location = new Point(15, 15)
            };
            
            var listBox = new ListBox { Location = new Point(15, 60), Width = 360, Height = 120 };
            
            foreach (var candidate in candidates)
            {
                listBox.Items.Add(new CandidateItem
                {
                    WitchID = candidate.WitchID,
                    Name = candidate.WitchName,
                    VoteCount = candidate.VoteCount
                });
            }
            
            if (listBox.Items.Count > 0) listBox.SelectedIndex = 0;
            
            var btnOk = new Button { Text = "确定", Width = 100, Height = 35, Location = new Point(170, 200), DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "取消", Width = 100, Height = 35, Location = new Point(280, 200), DialogResult = DialogResult.Cancel };
            
            dialog.Controls.Add(lblInfo);
            dialog.Controls.Add(listBox);
            dialog.Controls.Add(btnOk);
            dialog.Controls.Add(btnCancel);
            dialog.AcceptButton = btnOk;
            dialog.CancelButton = btnCancel;
            
            if (dialog.ShowDialog(this) == DialogResult.OK && listBox.SelectedItem != null)
            {
                return ((CandidateItem)listBox.SelectedItem).WitchID;
            }
            
            return 0;
        }
        
        private class CandidateItem
        {
            public int WitchID { get; set; }
            public string Name { get; set; } = "";
            public int VoteCount { get; set; }
            
            public override string ToString()
            {
                return $"{Name} ({VoteCount} 票)";
            }
        }
    }
}
