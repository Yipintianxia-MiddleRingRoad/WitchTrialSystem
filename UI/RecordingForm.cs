using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NAudio.Wave;
using WitchTrialSystem.BLL;
using Timer = System.Windows.Forms.Timer;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 录音页面：录制、暂停、结束、播放、删除，只显示当前账号的录音。
    /// 文件按账号所属编号（658-721）存储到 UI/recorder/{编号}/ 下。
    /// </summary>
    public class RecordingForm : Form
    {
        private readonly string _username;
        private readonly RecordingService _service = new();

        // 录音
        private WaveInEvent? _waveIn;
        private WaveFileWriter? _writer;
        private string? _currentFile;
        private bool _isRecording;
        private bool _isPaused;
        private int _seconds;
        private Timer? _timer;

        // 播放
        private WaveOutEvent? _waveOut;
        private AudioFileReader? _audioReader;

        // UI
        private readonly Label _lblStatus = new() { AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Text = "准备就绪" };
        private readonly Label _lblTime = new() { AutoSize = true, ForeColor = Color.OrangeRed, Font = new Font("Segoe UI", 20, FontStyle.Bold), Text = "00:00" };
        private readonly Button _btnStart = new() { Text = "开始录音", Width = 120, Height = 40, BackColor = Color.FromArgb(180, 60, 60, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        private readonly Button _btnPause = new() { Text = "暂停", Width = 120, Height = 40, BackColor = Color.FromArgb(180, 90, 90, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Enabled = false };
        private readonly Button _btnStop = new() { Text = "结束录音", Width = 120, Height = 40, BackColor = Color.FromArgb(180, 90, 60, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Enabled = false };
        private readonly Button _btnSave = new() { Text = "保存录音", Width = 120, Height = 40, BackColor = Color.FromArgb(180, 120, 80, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Enabled = false };
        private readonly Button _btnPlay = new() { Text = "播放", Width = 100, Height = 36, BackColor = Color.FromArgb(180, 60, 90, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Enabled = false };
        private readonly Button _btnDelete = new() { Text = "删除", Width = 100, Height = 36, BackColor = Color.FromArgb(180, 110, 60, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Enabled = false };
        private readonly TextBox _txtTitle = new() { PlaceholderText = "请输入录音标题后点击“保存录音”", Width = 260 };
        private readonly ListView _list = new()
        {
            View = View.Details,
            FullRowSelect = true,
            MultiSelect = false,
            HideSelection = false,
            Width = 520,
            Height = 350,
            GridLines = true,
            HeaderStyle = ColumnHeaderStyle.Nonclickable
        };
        private readonly Panel _content = new() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 30, 30) }; // 改为Fill填充整个窗口

        public RecordingForm(string username)
        {
            _username = username;
            InitializeForm();
            SetupLayout();
            HookEvents();
            EnsureRecordingFolderExists();
            LoadRecordings();
        }

        /// <summary>
        /// 确保录音文件夹存在，适应新环境
        /// </summary>
        private void EnsureRecordingFolderExists()
        {
            try
            {
                _currentFile = _service.BuildNewFilePath(_username);
                var dir = Path.GetDirectoryName(_currentFile);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                _currentFile = null; // 重置，实际录音时再设置
            }
            catch (Exception ex)
            {
                // 记录错误但不阻止界面显示
                System.Diagnostics.Debug.WriteLine($"创建录音文件夹失败：{ex.Message}");
            }
        }

        private void InitializeForm()
        {
            Text = $"录音（当前用户：{_username}）";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 880; // 适当加宽窗口，保证布局充足
            Height = 620;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;
            BackColor = Color.FromArgb(30, 30, 30);
            MinimumSize = new Size(800, 620); // 设置窗口最小宽度，避免布局挤压
            KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Close(); };
        }

        private void SetupLayout()
        {
            // 内容容器填充整个窗口
            Controls.Add(_content);

            // 初始化列，确保顺序明确
            _list.Columns.Clear();
            _list.Columns.AddRange(new[]
            {
                new ColumnHeader { Text = "文件名", Width = 260 },
                new ColumnHeader { Text = "时长", Width = 120 },
                new ColumnHeader { Text = "时间", Width = 140 }
            });

            // 上方显示区：列表
            var displayPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16), BackColor = Color.FromArgb(35, 35, 35) };
            _content.Controls.Add(displayPanel);

            var listTitle = new Label { Text = "我的录音", AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(6, 6) };
            _list.Location = new Point(6, 30);
            _list.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            displayPanel.Controls.Add(listTitle);
            displayPanel.Controls.Add(_list);

            // 下方控制区：按钮与状态
            var controlPanel = new Panel { Dock = DockStyle.Bottom, Height = 200, Padding = new Padding(16), BackColor = Color.FromArgb(40, 40, 40) };
            _content.Controls.Add(controlPanel);

            _lblStatus.Location = new Point(10, 10);
            _lblTime.Location = new Point(10, 40);
            _btnStart.Location = new Point(10, 90);
            _btnPause.Location = new Point(150, 90);
            _btnStop.Location = new Point(290, 90);
            _btnSave.Location = new Point(430, 90);
            _txtTitle.Location = new Point(10, 140);
            _btnPlay.Location = new Point(330, 140);
            _btnDelete.Location = new Point(450, 140);

            controlPanel.Controls.AddRange(new Control[] { _lblStatus, _lblTime, _btnStart, _btnPause, _btnStop, _btnSave, _txtTitle, _btnPlay, _btnDelete });

            // 绑定调整列宽事件
            Resize += (_, __) => AdjustListColumns();
            displayPanel.Resize += (_, __) => AdjustListColumns();
            AdjustListColumns();
        }

        private void HookEvents()
        {
            _btnStart.Click += (_, __) => StartRecording();
            _btnPause.Click += (_, __) => TogglePause();
            _btnStop.Click += (_, __) => StopRecording();
            _btnSave.Click += (_, __) => SaveRecording();
            _btnPlay.Click += (_, __) => TogglePlay();
            _btnDelete.Click += (_, __) => DeleteSelected();
            _list.SelectedIndexChanged += (_, __) => UpdateListButtons();
            FormClosing += (_, __) => { StopRecording(true); StopPlayback(); };
        }

        #region 录音控制
        private void StartRecording()
        {
            try
            {
                StopPlayback();

                _currentFile = _service.BuildNewFilePath(_username);
                var dir = Path.GetDirectoryName(_currentFile);
                if (string.IsNullOrEmpty(dir))
                {
                    throw new InvalidOperationException("无法确定录音文件存储路径");
                }
                
                // 确保文件夹存在，适应新环境
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                _waveIn = new WaveInEvent { WaveFormat = new WaveFormat(44100, 1) };
                _waveIn.DataAvailable += WaveIn_DataAvailable;
                _waveIn.RecordingStopped += WaveIn_RecordingStopped;

                _writer = new WaveFileWriter(_currentFile, _waveIn.WaveFormat);

                _waveIn.StartRecording();
                _isRecording = true;
                _isPaused = false;
                _seconds = 0;
                _lblTime.Text = "00:00";

                _timer = new Timer { Interval = 1000 };
                _timer.Tick += (_, __) =>
                {
                    if (!_isPaused)
                    {
                        _seconds++;
                        _lblTime.Text = RecordingService.FormatDuration(TimeSpan.FromSeconds(_seconds));
                    }
                };
                _timer.Start();

                _lblStatus.Text = "录音中...";
                _lblStatus.ForeColor = Color.OrangeRed;
                _btnStart.Enabled = false;
                _btnPause.Enabled = true;
                _btnPause.Text = "暂停";
                _btnStop.Enabled = true;
                _btnPlay.Enabled = false;
                _btnDelete.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"开始录音失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StopRecording(true);
            }
        }

        private void TogglePause()
        {
            if (!_isRecording) return;
            _isPaused = !_isPaused;
            _btnPause.Text = _isPaused ? "继续" : "暂停";
            _lblStatus.Text = _isPaused ? "已暂停" : "录音中...";
            _lblStatus.ForeColor = _isPaused ? Color.Gold : Color.OrangeRed;
        }

        private void StopRecording(bool silent = false)
        {
            if (_waveIn != null)
            {
                _waveIn.DataAvailable -= WaveIn_DataAvailable;
                _waveIn.RecordingStopped -= WaveIn_RecordingStopped;
                try { _waveIn.StopRecording(); } catch { }
                _waveIn.Dispose();
                _waveIn = null;
            }

            _writer?.Dispose();
            _writer = null;

            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;

            // 只要本次录音真正开始过，并且磁盘上有非空文件，就认为录音已完成
            var hasValidFile = !string.IsNullOrEmpty(_currentFile)
                               && File.Exists(_currentFile)
                               && new FileInfo(_currentFile).Length > 0;
            var fileCompleted = _isRecording && hasValidFile;
            _isRecording = false;
            _isPaused = false;

            _btnStart.Enabled = true;
            _btnPause.Enabled = false;
            _btnStop.Enabled = false;
            _btnPause.Text = "暂停";
            _btnPlay.Enabled = fileCompleted || _list.SelectedItems.Count > 0;
            _btnDelete.Enabled = _list.SelectedItems.Count > 0;

            if (fileCompleted && !silent)
            {
                // 录音已完成，等待用户输入标题并点击“保存录音”
                _lblStatus.Text = "录音结束，请输入标题后点击“保存录音”。";
                _lblStatus.ForeColor = Color.Gold;
                _btnSave.Enabled = true;
                _txtTitle.Focus();
                _txtTitle.SelectAll();
            }
            else
            {
                _lblStatus.Text = "准备就绪";
                _lblStatus.ForeColor = Color.White;
                _btnSave.Enabled = false;
            }
        }

        /// <summary>
        /// 保存当前录音：要求用户输入名称，并重命名文件后刷新列表
        /// </summary>
        private void SaveRecording()
        {
            if (string.IsNullOrEmpty(_currentFile) || !File.Exists(_currentFile))
            {
                MessageBox.Show("当前没有可保存的录音。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _btnSave.Enabled = false;
                return;
            }

            var title = _txtTitle.Text?.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("请先输入录音标题。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _txtTitle.Focus();
                return;
            }

            // 替换非法文件名字符
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                title = title.Replace(c, '_');
            }

            var dir = Path.GetDirectoryName(_currentFile)!;
            var target = Path.Combine(dir, $"{title}.wav");

            try
            {
                if (!string.Equals(_currentFile, target, StringComparison.OrdinalIgnoreCase))
                {
                    // 如已存在同名文件，询问是否覆盖
                    if (File.Exists(target))
                    {
                        var r = MessageBox.Show("已存在同名录音文件，是否覆盖？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (r != DialogResult.Yes)
                        {
                            return;
                        }
                        File.Delete(target);
                    }

                    File.Move(_currentFile, target);
                    _currentFile = target;
                }

                LoadRecordings();
                SelectCurrentFile();

                _btnSave.Enabled = false;
                _lblStatus.Text = "保存成功。";
                _lblStatus.ForeColor = Color.LimeGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存录音失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            if (_isPaused || _writer == null) return;
            _writer.Write(e.Buffer, 0, e.BytesRecorded);
            _writer.Flush();
        }

        private void WaveIn_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            _writer?.Dispose();
            _writer = null;
        }
        #endregion

        #region 播放
        private void TogglePlay()
        {
            if (_waveOut != null)
            {
                StopPlayback();
                return;
            }

            var file = GetSelectedFile();
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                MessageBox.Show("请选择要播放的录音。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _audioReader = new AudioFileReader(file);
                _waveOut = new WaveOutEvent();
                _waveOut.Init(_audioReader);
                _waveOut.PlaybackStopped += (_, __) => StopPlayback(true);
                _waveOut.Play();

                _btnPlay.Text = "停止播放";
                _lblStatus.Text = "播放中";
                _lblStatus.ForeColor = Color.DeepSkyBlue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"播放失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StopPlayback();
            }
        }

        private void StopPlayback(bool fromCallback = false)
        {
            try { _waveOut?.Stop(); } catch { }
            _waveOut?.Dispose();
            _waveOut = null;

            _audioReader?.Dispose();
            _audioReader = null;

            _btnPlay.Text = "播放";
            if (!fromCallback)
            {
                _lblStatus.Text = "准备就绪";
                _lblStatus.ForeColor = Color.White;
            }
        }
        #endregion

        #region 列表管理
        private void LoadRecordings()
        {
            var rows = new List<RecordingService.RecordingInfo>(_service.ListRecordings(_username));
            _list.Items.Clear();
            foreach (var r in rows)
            {
                var item = new ListViewItem(new[]
                {
                    r.FileName,
                    RecordingService.FormatDuration(r.Duration),
                    r.CreatedAt.ToString("MM-dd HH:mm")
                })
                { Tag = r.FilePath };
                _list.Items.Add(item);
            }
            UpdateListButtons();
            AdjustListColumns();
        }

        private void SelectCurrentFile()
        {
            if (string.IsNullOrEmpty(_currentFile)) return;
            foreach (ListViewItem item in _list.Items)
            {
                if (string.Equals(item.Tag as string, _currentFile, StringComparison.OrdinalIgnoreCase))
                {
                    item.Selected = true;
                    item.EnsureVisible();
                    break;
                }
            }
        }

        private string? GetSelectedFile()
        {
            return _list.SelectedItems.Count > 0 ? _list.SelectedItems[0].Tag as string : _currentFile;
        }

        private void DeleteSelected()
        {
            if (_list.SelectedItems.Count == 0) return;
            var file = _list.SelectedItems[0].Tag as string;
            if (string.IsNullOrEmpty(file)) return;

            if (MessageBox.Show("确定删除选中的录音吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    StopPlayback();
                    _service.DeleteRecording(file);
                    LoadRecordings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateListButtons()
        {
            var hasSelection = _list.SelectedItems.Count > 0;
            _btnPlay.Enabled = hasSelection || (!_isRecording && !string.IsNullOrEmpty(_currentFile));
            _btnDelete.Enabled = hasSelection;
        }
        #endregion

        /// <summary>
        /// 根据列表可用宽度动态调整列宽，避免左侧标题被遮挡
        /// </summary>
        private void AdjustListColumns()
        {
            if (_list.Columns.Count != 3) return;

            // 预留滚动条宽度和少量边距
            int available = _list.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 6;
            if (available < 300) available = 300;

            int fileWidth = (int)(available * 0.52);
            int durationWidth = (int)(available * 0.22);
            int timeWidth = available - fileWidth - durationWidth;

            _list.Columns[0].Width = fileWidth;
            _list.Columns[1].Width = durationWidth;
            _list.Columns[2].Width = timeWidth;
        }
    }
}