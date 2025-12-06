# 智慧可视化大屏功能需求文档

## 简介

本文档定义了魔女审判系统的智慧可视化大屏功能需求。该功能为国家层管理员（Admin）和监管员（Meruru）提供实时的、可视化的魔女岛屿状态监控界面，通过多种图表类型展示魔女分布、状态、批次等关键数据。

## 术语表

- **System**: 魔女审判系统
- **Dashboard**: 智慧可视化大屏窗口
- **Admin**: 国家层管理员角色
- **Meruru**: 监管员角色
- **Witch**: 魔女实体
- **Island**: 魔女岛屿
- **Batch**: 魔女批次
- **Status**: 魔女状态（待分配、分配至岛屿、审判中、死亡（正常）、死亡（魔女化）、其它）
- **Chart**: 图表组件
- **ScottPlot**: 开源图表库

---

## 需求

### 需求 1：大屏窗口访问权限

**用户故事**：作为国家层管理员或监管员，我希望能够打开智慧可视化大屏窗口，以便实时监控魔女岛屿的整体状态。

#### 验收标准

1. WHEN Admin用户点击"智慧大屏"按钮 THEN THE System SHALL 打开可视化大屏窗口并显示所有岛屿的数据
2. WHEN Meruru用户点击"智慧大屏"按钮 THEN THE System SHALL 打开可视化大屏窗口并仅显示其所属岛屿的数据
3. WHEN Warden或Witch用户尝试访问大屏功能 THEN THE System SHALL 隐藏或禁用该功能入口
4. WHEN 大屏窗口打开时 THEN THE System SHALL 以最大化模式显示窗口
5. WHEN 大屏窗口加载时 THEN THE System SHALL 在3秒内完成所有图表的初始渲染

---

### 需求 2：全局统计卡片显示

**用户故事**：作为管理员，我希望在大屏顶部看到关键统计数据卡片，以便快速了解系统整体状况。

#### 验收标准

1. WHEN 大屏窗口加载完成 THEN THE System SHALL 在顶部显示三个统计卡片
2. WHEN 显示统计卡片时 THEN THE System SHALL 在第一个卡片中显示魔女总数和本月新增数量
3. WHEN 显示统计卡片时 THEN THE System SHALL 在第二个卡片中显示岛屿总数和运营状态
4. WHEN 显示统计卡片时 THEN THE System SHALL 在第三个卡片中显示批次总数和活跃批次数
5. WHEN 统计数据更新时 THEN THE System SHALL 在卡片上显示变化动画效果

---

### 需求 3：全局状态分布饼图

**用户故事**：作为管理员，我希望看到所有魔女的状态分布饼图，以便了解整体状态比例。

#### 验收标准

1. WHEN 大屏窗口加载完成 THEN THE System SHALL 显示全局状态分布饼图
2. WHEN 显示饼图时 THEN THE System SHALL 为每个状态使用不同的颜色（待分配=灰色、分配至岛屿=蓝色、审判中=橙色、死亡（正常）=红色、死亡（魔女化）=深红色、其它=紫色）
3. WHEN 显示饼图时 THEN THE System SHALL 在每个扇区显示状态名称、人数和百分比
4. WHEN 用户点击饼图扇区时 THEN THE System SHALL 打开该状态的魔女列表详情窗口
5. WHEN 饼图数据为空时 THEN THE System SHALL 显示"暂无数据"提示信息

---

### 需求 4：岛屿状态环形图

**用户故事**：作为管理员，我希望看到每个岛屿的状态分布环形图，以便对比不同岛屿的状况。

#### 验收标准

1. WHEN 大屏窗口加载完成 THEN THE System SHALL 为每个岛屿显示一个状态分布环形图
2. WHEN 显示环形图时 THEN THE System SHALL 在图表标题显示岛屿名称和总人数
3. WHEN 显示环形图时 THEN THE System SHALL 使用与全局饼图相同的颜色方案
4. WHEN Admin用户查看时 THEN THE System SHALL 并排显示所有岛屿的环形图
5. WHEN Meruru用户查看时 THEN THE System SHALL 仅显示其所属岛屿的环形图
6. WHEN 用户点击环形图扇区时 THEN THE System SHALL 打开该岛屿+状态的魔女列表详情窗口

---

### 需求 5：批次容量柱状图

**用户故事**：作为管理员，我希望看到每个批次的人数柱状图，以便了解批次容量使用情况。

#### 验收标准

1. WHEN 大屏窗口加载完成 THEN THE System SHALL 在每个岛屿区域下方显示批次容量柱状图
2. WHEN 显示柱状图时 THEN THE System SHALL 为每个批次显示一个柱子，高度代表当前人数
3. WHEN 显示柱状图时 THEN THE System SHALL 在柱子上标注"当前人数/最大容量"（如"11/13"）
4. WHEN 批次已满时 THEN THE System SHALL 将该柱子显示为红色并标注"满"
5. WHEN 批次人数超过80%时 THEN THE System SHALL 将该柱子显示为橙色
6. WHEN 批次人数低于80%时 THEN THE System SHALL 将该柱子显示为绿色
7. WHEN 用户点击柱子时 THEN THE System SHALL 打开该批次的魔女列表详情窗口

---

### 需求 6：批次状态热力图

**用户故事**：作为管理员，我希望看到批次状态矩阵热力图，以便快速定位问题批次。

#### 验收标准

1. WHEN 大屏窗口加载完成 THEN THE System SHALL 显示批次状态矩阵热力图
2. WHEN 显示热力图时 THEN THE System SHALL 以批次为列、状态为行的矩阵形式展示数据
3. WHEN 显示热力图单元格时 THEN THE System SHALL 根据人数使用不同颜色（0-3人=绿色、4-6人=黄色、7人以上=红色）
4. WHEN 显示热力图单元格时 THEN THE System SHALL 在单元格中显示具体人数
5. WHEN 用户点击热力图单元格时 THEN THE System SHALL 打开该批次+状态的魔女列表详情窗口
6. WHEN 热力图底部 THEN THE System SHALL 显示颜色说明图例

---

### 需求 7：趋势分析折线图（可选）

**用户故事**：作为管理员，我希望看到最近30天的魔女数量趋势图，以便了解系统使用趋势。

#### 验收标准

1. IF 数据库中存在时间字段 THEN WHEN 大屏窗口加载完成 THEN THE System SHALL 显示趋势分析折线图
2. WHEN 显示折线图时 THEN THE System SHALL 显示最近30天的数据
3. WHEN 显示折线图时 THEN THE System SHALL 绘制三条折线（总人数、审判中人数、已处刑人数）
4. WHEN 显示折线图时 THEN THE System SHALL 在图表底部显示日期轴，左侧显示人数轴
5. WHEN 显示折线图时 THEN THE System SHALL 在图表右上角显示图例说明
6. IF 数据库中不存在时间字段 THEN THE System SHALL 隐藏趋势分析区域

---

### 需求 8：数据实时刷新

**用户故事**：作为管理员，我希望大屏数据能够自动刷新，以便看到最新的系统状态。

#### 验收标准

1. WHEN 大屏窗口打开时 THEN THE System SHALL 启动定时器每30秒自动刷新数据
2. WHEN 数据刷新时 THEN THE System SHALL 重新查询数据库并更新所有图表
3. WHEN 数据刷新时 THEN THE System SHALL 在标题栏显示最后更新时间
4. WHEN 用户点击"手动刷新"按钮时 THEN THE System SHALL 立即刷新所有数据
5. WHEN 数据刷新失败时 THEN THE System SHALL 显示错误提示并保持上一次的数据显示

---

### 需求 9：图表交互功能

**用户故事**：作为管理员，我希望能够与图表进行交互，以便查看详细数据。

#### 验收标准

1. WHEN 用户鼠标悬停在图表元素上时 THEN THE System SHALL 显示该元素的详细数据提示框
2. WHEN 用户点击图表元素时 THEN THE System SHALL 打开对应的魔女列表详情窗口
3. WHEN 详情窗口打开时 THEN THE System SHALL 根据点击的元素筛选并显示相应的魔女列表
4. WHEN 详情窗口关闭时 THEN THE System SHALL 返回大屏窗口并保持当前状态
5. WHEN 用户在大屏窗口按ESC键时 THEN THE System SHALL 关闭大屏窗口

---

### 需求 10：导出功能

**用户故事**：作为管理员，我希望能够导出大屏数据和图表，以便制作报告。

#### 验收标准

1. WHEN 用户点击"导出图表"按钮时 THEN THE System SHALL 显示导出选项菜单
2. WHEN 用户选择"导出为图片"时 THEN THE System SHALL 将整个大屏截图保存为PNG文件
3. WHEN 用户选择"导出数据为Excel"时 THEN THE System SHALL 将所有统计数据导出为Excel文件
4. WHEN 用户选择"生成PDF报告"时 THEN THE System SHALL 生成包含所有图表和数据的PDF报告
5. WHEN 导出成功时 THEN THE System SHALL 显示成功提示并询问是否打开文件
6. WHEN 导出失败时 THEN THE System SHALL 显示错误信息并提示用户重试

---

### 需求 11：响应式布局

**用户故事**：作为管理员，我希望大屏能够适应不同的屏幕尺寸，以便在不同设备上使用。

#### 验收标准

1. WHEN 窗口尺寸改变时 THEN THE System SHALL 自动调整所有图表的大小和位置
2. WHEN 窗口宽度小于1280px时 THEN THE System SHALL 将岛屿环形图改为上下排列
3. WHEN 窗口高度不足时 THEN THE System SHALL 启用垂直滚动条
4. WHEN 图表调整大小时 THEN THE System SHALL 保持图表的宽高比和可读性
5. WHEN 窗口最大化或还原时 THEN THE System SHALL 在500毫秒内完成布局调整

---

### 需求 12：性能优化

**用户故事**：作为管理员，我希望大屏能够流畅运行，以便获得良好的使用体验。

#### 验收标准

1. WHEN 大屏窗口加载时 THEN THE System SHALL 在3秒内完成所有图表的初始渲染
2. WHEN 数据刷新时 THEN THE System SHALL 在1秒内完成所有图表的更新
3. WHEN 用户与图表交互时 THEN THE System SHALL 在100毫秒内响应用户操作
4. WHEN 魔女总数超过1000人时 THEN THE System SHALL 仍能保持流畅的渲染性能
5. WHEN 大屏窗口运行超过1小时时 THEN THE System SHALL 不出现内存泄漏或性能下降

---

## 非功能性需求

### 可用性
- 图表应使用清晰的颜色区分不同状态
- 字体大小应适合大屏显示（标题18-24pt，数据14-16pt）
- 所有交互元素应有明显的视觉反馈

### 可维护性
- 图表组件应模块化设计，便于替换和扩展
- 数据查询应封装在DAL层，便于优化和维护
- 配色方案应集中管理，便于主题切换

### 兼容性
- 支持Windows 10及以上操作系统
- 支持1920×1080及以上分辨率
- 兼容.NET 6.0及以上版本

### 安全性
- 仅Admin和Meruru角色可访问大屏功能
- Meruru用户仅能查看其所属岛屿的数据
- 所有数据查询应遵循权限控制规则
