using System.Windows.Forms;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 图鉴·地图界面
    /// 功能：展示地图信息
    /// </summary>
    public class MapForm : BasePokedexForm
    {
        /// <summary>
        /// 构造函数：初始化地图界面
        /// </summary>
        public MapForm(string username) : base(username)
        {
            Text = "图鉴 · 地图";
            UpdateTitle();  // 添加用户信息到标题
        }

        protected override string GetBackgroundImageName()
        {
            return "map_bg.png";  // 地图背景图
        }

        protected override void DisableCurrentPageButton()
        {
            _btnMap.Enabled = false;
            _btnMap.Cursor = Cursors.Default;
        }
    }
}
