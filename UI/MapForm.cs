using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 图鉴·地图界面
    /// 功能：展示地图信息，支持多张地图切换
    /// 左下角：四个透明热键按钮（对应不同地图）
    /// </summary>
    public class MapForm : BasePokedexForm
    {
        #region 数据字段

        private readonly string[] _mapFiles = {
            "map_bg.png",    // 初始地图
            "地图2.png",     // 地图2
            "地图3.png",     // 地图3
            "地图4.png"      // 地图4
        };

        private int _currentMapIndex = 0;

        #endregion

        #region UI 控件

        // 左下角：地图热键按钮
        private readonly Button[] _mapButtons = new Button[4];  // 四个透明热键按钮

        #endregion

        /// <summary>
        /// 构造函数：初始化地图界面
        /// </summary>
        public MapForm(string username) : base(username)
        {
            Text = "图鉴 · 地图";
            UpdateTitle();  // 添加用户信息到标题
            BLL.IconHelper.SetFormIcon(this);  // 设置窗体图标
            InitializeMapButtons();
        }

        protected override string GetBackgroundImageName()
        {
            return Path.Combine("map", _mapFiles[_currentMapIndex]);
        }

        protected override void DisableCurrentPageButton()
        {
            _btnMap.Enabled = false;
            _btnMap.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 初始化地图热键按钮
        /// </summary>
        private void InitializeMapButtons()
        {
            for (int i = 0; i < 4; i++)
            {
                _mapButtons[i] = new Button
                {
                    Text = "",
                    Width = 90,
                    Height = 70,
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Tag = i
                };

                _mapButtons[i].FlatAppearance.BorderSize = 0;
                _mapButtons[i].FlatAppearance.MouseOverBackColor = Color.Transparent;
                _mapButtons[i].Click += MapButton_Click;

                _bg.Controls.Add(_mapButtons[i]);
                _mapButtons[i].BringToFront();
            }

            // 延迟设置按钮位置，直到窗体完全加载
            _bg.Layout += (s, e) => PositionMapButtons();
        }

        private void PositionMapButtons()
        {
            for (int i = 0; i < 4; i++)
            {
                if (_mapButtons[i] != null)
                {
                    // 定位：左下角，水平排列，向右下移动半个大小左右
                    _mapButtons[i].Left = 50 + i * 120;  // 向右移动40像素
                    _mapButtons[i].Top = ClientSize.Height - 120;  // 向下移动25像素
                }
            }
        }

        /// <summary>
        /// 地图按钮点击事件
        /// </summary>
        private void MapButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is int index)
            {
                SwitchToMap(index);
            }
        }

        /// <summary>
        /// 切换到指定地图
        /// </summary>
        private void SwitchToMap(int index)
        {
            if (index < 0 || index >= _mapFiles.Length) return;

            _currentMapIndex = index;

            // 重新加载背景图片
            LoadMapBackground();
        }

        /// <summary>
        /// 加载地图背景图片
        /// </summary>
        private void LoadMapBackground()
        {
            string mapPath = Path.Combine(AppContext.BaseDirectory, "Images", "map", _mapFiles[_currentMapIndex]);
            if (File.Exists(mapPath))
            {
                _bg.BackgroundImage = Image.FromFile(mapPath);
            }
        }

        /// <summary>
        /// 重写背景加载，在初始化时加载地图背景
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // 重新加载地图背景，覆盖基类的UI背景
            LoadMapBackground();
        }
    }
}
