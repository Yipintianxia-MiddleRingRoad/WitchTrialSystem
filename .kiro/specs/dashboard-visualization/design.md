# 智慧可视化大屏 - 设计文档

## 概述

智慧可视化大屏是魔女审判系统的数据可视化模块，为Admin和Meruru角色提供实时的、交互式的魔女岛屿状态监控界面。该模块使用ScottPlot图表库实现多种图表类型，包括饼图、环形图、柱状图、热力图和折线图，通过直观的可视化方式展示魔女分布、状态、批次等关键数据。

### 核心目标
- 提供实时的魔女岛屿状态监控
- 支持多维度数据可视化（状态、岛屿、批次）
- 实现权限隔离（Admin看全部，Meruru看所属岛屿）
- 提供交互式数据探索功能
- 支持数据导出和报告生成

### 技术栈
- **UI框架**: WinForms (.NET 6.0+)
- **图表库**: ScottPlot 4.1+ (开源、高性能)
- **数据访问**: ADO.NET + SQL Server
- **布局**: TableLayoutPanel + FlowLayoutPanel
- **导出**: System.Drawing + iTextSharp (PDF)

---

## 架构设计

### 系统架构图

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│  ┌──────────────────────────────────────────────────────┐  │
│  │           DashboardForm (主窗口)                      │  │
│  │  ┌────────────┐  ┌────────────┐  ┌────────────┐     │  │
│  │  │ StatCard   │  │ StatCard   │  │ StatCard   │     │  │
│  │  └────────────┘  └────────────┘  └────────────┘     │  │
│  │  ┌──────────────────────────────────────────────┐   │  │
│  │  │        GlobalPieChart (饼图)                  │   │  │
│  │  └──────────────────────────────────────────────┘   │  │
│  │  ┌──────────────┐          ┌──────────────┐        │  │
│  │  │ IslandChart1 │          │ IslandChart2 │        │  │
│  │  │  (环形图)     │          │  (环形图)     │        │  │
│  │  └──────────────┘          └──────────────┘        │  │
│  │  ┌──────────────────────────────────────────────┐   │  │
│  │  │      BatchHeatmapChart (热力图)               │   │  │
│  │  └──────────────────────────────────────────────┘   │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    Business Logic Layer                      │
│  ┌──────────────────────────────────────────────────────┐  │
│  │           DashboardService                            │  │
│  │  - GetGlobalStats()                                   │  │
│  │  - GetStatusDistribution()                            │  │
│  │  - GetIslandStatusDistribution(islandId)             │  │
│  │  - GetBatchCapacityData(islandId)                    │  │
│  │  - GetBatchStatusMatrix()                            │  │
│  │  - GetTrendData(days)                                │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    Data Access Layer                         │
│  ┌──────────────────────────────────────────────────────┐  │
│  │           DashboardDAL                                │  │
│  │  - GetGlobalStatistics()                              │  │
│  │  - GetStatusCounts()                                  │  │
│  │  - GetIslandStatusCounts(islandId)                   │  │
│  │  - GetBatchCapacity(islandId)                        │  │
│  │  - GetBatchStatusMatrix()                            │  │
│  │  - GetTrendData(startDate, endDate)                 │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    Database (SQL Server)                     │
│  wt.Witch, wt.Island, wt.Batch                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 组件和接口

### 1. DashboardForm (主窗口)

**职责**: 大屏主窗口，负责布局管理和用户交互

**属性**:

```csharp
- string _username
- string _roleName
- int? _userIslandId  // Meruru用户的岛屿ID
- Timer _refreshTimer
- DashboardService _service
```

**方法**:
```csharp
+ void InitializeUI()
+ void LoadAllData()
+ void RefreshData()
+ void OnChartElementClicked(ChartElement element)
+ void ExportToPNG()
+ void ExportToExcel()
+ void ExportToPDF()
```

### 2. DashboardService (业务逻辑层)

**职责**: 处理业务逻辑，数据转换和权限控制

**方法**:
```csharp
+ GlobalStats GetGlobalStats(string username, string role)
+ List<StatusCount> GetStatusDistribution(string username, string role)
+ List<StatusCount> GetIslandStatusDistribution(int islandId)
+ List<BatchCapacity> GetBatchCapacityData(int islandId)
+ Dictionary<int, Dictionary<string, int>> GetBatchStatusMatrix()
+ List<TrendPoint> GetTrendData(int days)
```

### 3. DashboardDAL (数据访问层)

**职责**: 执行SQL查询，返回原始数据

**方法**:
```csharp
+ DataTable GetGlobalStatistics()
+ DataTable GetStatusCounts()
+ DataTable GetIslandStatusCounts(int islandId)
+ DataTable GetBatchCapacity(int islandId)
+ DataTable GetBatchStatusMatrix()
+ DataTable GetTrendData(DateTime startDate, DateTime endDate)
```

### 4. Chart Components (图表组件)

**StatCard**: 统计卡片
```csharp
- string Title
- string Value
- string SubValue
- Color BackColor
+ void UpdateValue(string value, string subValue)
```

**PieChartPanel**: 饼图面板
```csharp
- FormsPlot Plot
- List<StatusCount> Data
+ void LoadData(List<StatusCount> data)
+ void OnSliceClicked(int index)
```

**DonutChartPanel**: 环形图面板
```csharp
- FormsPlot Plot
- string IslandName
- List<StatusCount> Data
+ void LoadData(string islandName, List<StatusCount> data)
```

**BarChartPanel**: 柱状图面板
```csharp
- FormsPlot Plot
- List<BatchCapacity> Data
+ void LoadData(List<BatchCapacity> data)
```

**HeatmapPanel**: 热力图面板
```csharp
- FormsPlot Plot
- Dictionary<int, Dictionary<string, int>> Data
+ void LoadData(Dictionary<int, Dictionary<string, int>> data)
```

---

## 数据模型

### GlobalStats (全局统计)
```csharp
public class GlobalStats
{
    public int TotalWitches { get; set; }
    public int NewThisMonth { get; set; }
    public int TotalIslands { get; set; }
    public int ActiveIslands { get; set; }
    public int TotalBatches { get; set; }
    public int ActiveBatches { get; set; }
}
```

### StatusCount (状态统计)
```csharp
public class StatusCount
{
    public string Status { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
    public Color Color { get; set; }
}
```

### BatchCapacity (批次容量)
```csharp
public class BatchCapacity
{
    public int IslandId { get; set; }
    public int LocalBatchId { get; set; }
    public int CurrentCount { get; set; }
    public int MaxCapacity { get; set; }
    public double UsageRate { get; set; }
    public Color BarColor { get; set; }
}
```

### TrendPoint (趋势数据点)
```csharp
public class TrendPoint
{
    public DateTime Date { get; set; }
    public int TotalCount { get; set; }
    public int TrialCount { get; set; }
    public int ExecutedCount { get; set; }
}
```

---

## 数据查询SQL设计

### 1. 全局统计查询
```sql
-- 魔女总数和本月新增
SELECT 
    COUNT(*) as TotalWitches,
    SUM(CASE WHEN MONTH(CreatedAt) = MONTH(GETDATE()) 
             AND YEAR(CreatedAt) = YEAR(GETDATE()) 
        THEN 1 ELSE 0 END) as NewThisMonth
FROM wt.Witch;

-- 岛屿统计
SELECT 
    COUNT(*) as TotalIslands,
    COUNT(*) as ActiveIslands
FROM wt.Island;

-- 批次统计
SELECT 
    COUNT(*) as TotalBatches,
    COUNT(DISTINCT b.BatchID) as ActiveBatches
FROM wt.Batch b
LEFT JOIN wt.Witch w ON b.BatchID = w.BatchID
WHERE w.WitchID IS NOT NULL;
```

### 2. 状态分布查询
```sql
-- 全局状态统计
SELECT 
    Status,
    COUNT(*) as Count,
    CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) as Percentage
FROM wt.Witch
GROUP BY Status
ORDER BY Count DESC;

-- 岛屿状态统计
SELECT 
    w.Status,
    COUNT(*) as Count,
    CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) as Percentage
FROM wt.Witch w
WHERE w.IslandID = @IslandID
GROUP BY w.Status
ORDER BY Count DESC;
```

### 3. 批次容量查询
```sql
SELECT 
    b.IslandID,
    b.LocalBatchID,
    COUNT(w.WitchID) as CurrentCount,
    13 as MaxCapacity,
    CAST(COUNT(w.WitchID) * 100.0 / 13 AS DECIMAL(5,2)) as UsageRate
FROM wt.Batch b
LEFT JOIN wt.Witch w ON b.BatchID = w.BatchID
WHERE b.IslandID = @IslandID
GROUP BY b.IslandID, b.LocalBatchID, b.BatchID
ORDER BY b.LocalBatchID;
```

### 4. 批次状态矩阵查询
```sql
SELECT 
    b.LocalBatchID,
    w.Status,
    COUNT(w.WitchID) as Count
FROM wt.Batch b
LEFT JOIN wt.Witch w ON b.BatchID = w.BatchID
GROUP BY b.LocalBatchID, w.Status
ORDER BY b.LocalBatchID, w.Status;
```

### 5. 趋势数据查询（如果有时间字段）
```sql
SELECT 
    CAST(CaptureTime AS DATE) as Date,
    COUNT(*) as TotalCount,
    SUM(CASE WHEN Status = N'审判中' THEN 1 ELSE 0 END) as TrialCount,
    SUM(CASE WHEN Status LIKE N'死亡%' THEN 1 ELSE 0 END) as ExecutedCount
FROM wt.Witch
WHERE CaptureTime >= DATEADD(DAY, -30, GETDATE())
GROUP BY CAST(CaptureTime AS DATE)
ORDER BY Date;
```

---

## UI布局设计

### 窗口布局结构
```
DashboardForm (1920×1080, Maximized)
├─ TopPanel (Dock.Top, Height=120)
│  ├─ TitleLabel ("魔女审判系统 · 智慧监控大屏")
│  ├─ TimeLabel ("实时更新：2024-12-05 15:30:25")
│  └─ ButtonPanel
│     ├─ RefreshButton
│     ├─ ExportButton
│     └─ CloseButton
├─ StatsPanel (Dock.Top, Height=120)
│  ├─ StatCard1 (魔女总数)
│  ├─ StatCard2 (岛屿数)
│  └─ StatCard3 (批次数)
├─ MainPanel (Dock.Fill, TableLayoutPanel 2×2)
│  ├─ GlobalPiePanel (Row=0, Col=0, ColSpan=2)
│  ├─ Island1Panel (Row=1, Col=0)
│  │  ├─ IslandDonutChart
│  │  └─ BatchBarChart
│  ├─ Island2Panel (Row=1, Col=1)
│  │  ├─ IslandDonutChart
│  │  └─ BatchBarChart
│  └─ HeatmapPanel (Row=2, Col=0, ColSpan=2)
└─ BottomPanel (Dock.Bottom, Height=300, Optional)
   └─ TrendLineChart
```

### 配色方案
```csharp
public static class DashboardColors
{
    // 背景色
    public static Color Background = Color.FromArgb(26, 26, 46);      // #1a1a2e
    public static Color CardBackground = Color.FromArgb(40, 40, 60);  // #28283c
    
    // 主题色
    public static Color Primary = Color.FromArgb(157, 78, 221);       // #9d4edd
    public static Color Secondary = Color.FromArgb(255, 0, 110);      // #ff006e
    public static Color Accent = Color.FromArgb(0, 245, 255);         // #00f5ff
    
    // 状态色
    public static Color StatusPending = Color.FromArgb(108, 117, 125);    // 待分配 #6c757d
    public static Color StatusAssigned = Color.FromArgb(13, 110, 253);    // 分配至岛屿 #0d6efd
    public static Color StatusTrial = Color.FromArgb(253, 126, 20);       // 审判中 #fd7e14
    public static Color StatusDeathNormal = Color.FromArgb(220, 53, 69);  // 死亡（正常） #dc3545
    public static Color StatusDeathWitch = Color.FromArgb(139, 0, 0);     // 死亡（魔女化） #8b0000
    public static Color StatusOther = Color.FromArgb(157, 78, 221);       // 其它 #9d4edd
    
    // 容量色
    public static Color CapacityLow = Color.FromArgb(25, 135, 84);        // 绿色 <80%
    public static Color CapacityMedium = Color.FromArgb(255, 193, 7);     // 橙色 80-99%
    public static Color CapacityFull = Color.FromArgb(220, 53, 69);       // 红色 100%
    
    // 热力图色
    public static Color HeatLow = Color.FromArgb(25, 135, 84);            // 0-3人
    public static Color HeatMedium = Color.FromArgb(255, 193, 7);         // 4-6人
    public static Color HeatHigh = Color.FromArgb(220, 53, 69);           // 7+人
}
```

---

## 错误处理

### 异常处理策略
1. **数据库连接失败**: 显示友好错误提示，提供重试按钮
2. **数据查询超时**: 显示加载中状态，超时后提示用户
3. **图表渲染失败**: 显示占位符，记录错误日志
4. **导出失败**: 显示具体错误信息，提供重试选项
5. **权限不足**: 隐藏或禁用相关功能，不显示错误

### 日志记录
```csharp
public static class DashboardLogger
{
    public static void LogInfo(string message);
    public static void LogWarning(string message);
    public static void LogError(string message, Exception ex);
}
```

---

## 测试策略

### 单元测试
- DashboardService业务逻辑测试
- 数据转换和计算测试
- 权限控制逻辑测试

### 集成测试
- DashboardDAL数据查询测试
- 数据库连接和查询性能测试
- 多用户权限隔离测试

### UI测试
- 图表渲染测试
- 交互功能测试
- 响应式布局测试
- 导出功能测试

### 性能测试
- 大数据量渲染测试（1000+魔女）
- 刷新性能测试
- 内存泄漏测试
- 长时间运行稳定性测试

---

## 正确性属性

*属性是一个特征或行为，应该在系统的所有有效执行中保持为真——本质上是关于系统应该做什么的正式陈述。属性作为人类可读规范和机器可验证正确性保证之间的桥梁。*

### 属性 1: 权限隔离一致性
*对于任何* Admin用户，大屏应显示所有岛屿的数据；*对于任何* Meruru用户，大屏应仅显示其所属岛屿的数据
**验证需求**: 1.1, 1.2

### 属性 2: 数据一致性
*对于任何* 时间点，所有图表显示的数据应来自同一次数据库查询，确保数据一致性
**验证需求**: 8.2

### 属性 3: 状态颜色映射唯一性
*对于任何* 魔女状态，在所有图表中应使用相同的颜色表示，确保视觉一致性
**验证需求**: 3.2, 4.3

### 属性 4: 批次容量约束
*对于任何* 批次，显示的当前人数应小于或等于最大容量13人
**验证需求**: 5.3

### 属性 5: 百分比总和
*对于任何* 饼图或环形图，所有扇区的百分比总和应等于100%（误差±0.1%）
**验证需求**: 3.3, 4.2

### 属性 6: 交互响应时间
*对于任何* 用户交互操作（点击、悬停），系统应在100毫秒内提供视觉反馈
**验证需求**: 12.3

### 属性 7: 刷新数据完整性
*对于任何* 数据刷新操作，要么所有图表都更新成功，要么都保持原状态（原子性）
**验证需求**: 8.2, 8.5

### 属性 8: 导出数据一致性
*对于任何* 导出操作，导出的数据应与当前屏幕显示的数据完全一致
**验证需求**: 10.2, 10.3, 10.4

---

## 实现注意事项

### ScottPlot使用建议
1. 使用`FormsPlot`控件而非`WpfPlot`
2. 启用硬件加速：`plot.Configuration.UseRenderQueue = true`
3. 批量更新数据后调用`plot.Render()`
4. 避免频繁创建新的Plot对象，复用现有对象

### 性能优化建议
1. 使用异步加载数据，避免阻塞UI线程
2. 图表数据缓存，避免重复查询
3. 使用`BeginUpdate()`和`EndUpdate()`批量更新UI
4. 大数据量时使用数据采样或分页

### 可维护性建议
1. 图表组件封装为独立的UserControl
2. 配色方案集中管理，便于主题切换
3. SQL查询封装在DAL层，便于优化
4. 使用依赖注入，便于单元测试

---

## 未来扩展

### 可能的增强功能
1. **自定义时间范围**: 用户可选择查看特定时间段的数据
2. **数据对比**: 对比不同时间段或不同岛屿的数据
3. **告警功能**: 批次满员或异常状态自动告警
4. **数据钻取**: 从图表深入到详细数据表格
5. **主题切换**: 支持亮色/暗色主题切换
6. **多屏支持**: 支持多显示器扩展显示
7. **实时推送**: WebSocket实时推送数据更新
8. **移动端适配**: 响应式设计支持平板和手机

---

**设计文档版本**: 1.0  
**最后更新**: 2024-12-05
