# Implementation Plan

- [x] 1. 备份和创建角色专用表单文件


  - 备份原 Form1.cs、Form1.Designer.cs、Form1.resx 为 Form1_Backup.*
  - 复制并重命名为 Form1_Admin.*、Form1_Regulator.*、Form1_Warden.*
  - 更新项目文件 (.csproj) 引用新文件
  - _Requirements: 1.1, 1.2_

- [x] 2. 修改角色专用表单的类名和构造函数

  - [x] 2.1 修改 Form1_Admin 类名和构造函数


    - 在 Form1_Admin.cs 中将类名从 `Form1` 改为 `Form1_Admin`
    - 在 Form1_Admin.Designer.cs 中同步修改类名
    - 修改构造函数，固定 `_roleName = "Admin"`
    - _Requirements: 1.3, 1.4_

  - [x] 2.2 修改 Form1_Regulator 类名和构造函数


    - 在 Form1_Regulator.cs 中将类名从 `Form1` 改为 `Form1_Regulator`
    - 在 Form1_Regulator.Designer.cs 中同步修改类名
    - 修改构造函数，固定 `_roleName = "Regulator"`
    - _Requirements: 1.3, 1.5_

  - [x] 2.3 修改 Form1_Warden 类名和构造函数


    - 在 Form1_Warden.cs 中将类名从 `Form1` 改为 `Form1_Warden`
    - 在 Form1_Warden.Designer.cs 中同步修改类名
    - 修改构造函数，固定 `_roleName = "Warden"`
    - _Requirements: 1.3, 1.6_

- [x] 3. 修改 LoginForm 的角色路由逻辑


  - 在 `OnLogin` 方法中添加 switch 语句
  - 根据角色路由到对应界面：Admin → Form1_Admin, Meruru/Utena → Form1_Regulator, Warden → Form1_Warden, Witch → PhoneForm
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [ ]* 3.1 编写角色路由的单元测试
  - 测试 Admin 路由到 Form1_Admin
  - 测试 Meruru/Utena 路由到 Form1_Regulator
  - 测试 Warden 路由到 Form1_Warden
  - 测试 Witch 路由到 PhoneForm
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [x] 4. 编译并测试基本功能



  - 编译项目，确保无编译错误
  - 使用不同角色登录，验证路由到正确界面
  - 验证三个角色界面的基本功能（加载数据、搜索、筛选）
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5_

- [x] 5. 在 WitchDAL 中添加 UpdateDescription 方法


  - 创建 `UpdateDescription(int witchId, string description)` 方法
  - 实现参数化 SQL UPDATE 语句
  - 处理 null 描述（存储为 DBNull.Value）
  - _Requirements: 5.1, 5.2, 5.3_

- [ ]* 5.1 编写 UpdateDescription 的单元测试
  - 测试正常更新流程
  - 测试 null 描述处理
  - 测试无效 witch ID 处理
  - _Requirements: 5.1, 5.2, 5.3, 5.5_

- [ ]* 5.2 编写属性测试：Non-null description storage
  - **Property 11: Non-null description storage**
  - **Validates: Requirements 5.3**
  - 生成随机非空描述字符串
  - 调用 UpdateDescription
  - 验证数据库存储的值完全匹配

- [x] 6. 创建 WitchEditDescriptionForm 编辑窗口

  - [x] 6.1 创建 WitchEditDescriptionForm.cs 文件


    - 定义构造函数接受 witchId, witchName, prisonerNo, currentDescription
    - 创建 UI 组件：信息标签、描述文本框、字数统计、保存/取消按钮
    - 设置窗口属性（大小、边框样式、启动位置）
    - _Requirements: 4.1, 4.2_

  - [x] 6.2 实现保存功能

    - 在 `BtnSave_Click` 中调用 `WitchDAL.UpdateDescription`
    - 成功时显示成功消息并关闭窗口（DialogResult.OK）
    - 失败时显示错误消息（包含异常详情）
    - _Requirements: 4.3, 4.4, 4.5_

  - [ ]* 6.3 编写属性测试：Character count accuracy
    - **Property 6: Character count accuracy**
    - **Validates: Requirements 4.2**
    - 生成随机文本字符串
    - 模拟文本框输入
    - 验证字符计数等于文本长度

- [x] 7. 在 Form1_Regulator 中添加右键菜单和编辑功能

  - [x] 7.1 创建右键菜单


    - 在 `InitializeContextMenu` 方法中创建 ContextMenuStrip
    - 添加"编辑公开描述"和"查看详情"菜单项
    - 绑定到 `_dgvWitches.ContextMenuStrip`
    - _Requirements: 3.1_

  - [x] 7.2 实现编辑描述功能

    - 在 `EditDescription_Click` 中获取选中魔女的信息
    - 检查岛屿权限（witchIslandId == _currentIslandId）
    - 如果权限不足，显示错误消息
    - 如果权限通过，打开 WitchEditDescriptionForm
    - 保存成功后刷新数据网格
    - _Requirements: 3.2, 3.3, 3.4, 3.5_

  - [ ]* 7.3 编写属性测试：Permission enforcement
    - **Property 2: Permission enforcement for cross-island editing**
    - **Validates: Requirements 3.3**
    - 生成随机 Regulator 用户和不同岛屿的魔女
    - 尝试编辑
    - 验证被拒绝

  - [ ]* 7.4 编写属性测试：Description update persistence
    - **Property 3: Description update persistence**
    - **Validates: Requirements 3.4**
    - 生成随机 witch ID 和描述文本
    - 调用 UpdateDescription
    - 验证数据库中的值匹配

- [x] 8. 确保 DescriptionPublic 列在数据网格中显示

  - 在 `LoadGrid` 方法中配置 DescriptionPublic 列
  - 设置列标题为"公开描述"
  - 设置列宽度和可见性
  - _Requirements: 3.5_

- [x] 9. 集成测试和验证


  - [x] 9.1 测试 Regulator 编辑本岛屿魔女描述

    - 使用 meruru_regulator 登录
    - 右键点击岛屿1的魔女
    - 选择"编辑公开描述"
    - 修改并保存
    - 验证数据网格刷新显示新描述
    - _Requirements: 3.1, 3.2, 3.4, 3.5_

  - [x] 9.2 测试 Regulator 跨岛屿编辑权限拒绝

    - 使用 meruru_regulator 登录
    - 尝试编辑岛屿2的魔女
    - 验证显示权限错误消息
    - _Requirements: 3.3_

  - [x] 9.3 测试所有角色的登录和界面功能

    - 测试 Admin 登录到 Form1_Admin
    - 测试 Meruru 登录到 Form1_Regulator
    - 测试 Utena 登录到 Form1_Regulator
    - 测试 Warden 登录到 Form1_Warden
    - 验证每个界面的基本功能正常
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 6.1, 6.2, 6.3, 6.4, 6.5_

- [x] 10. 最终检查点


  - 确保所有测试通过，如有问题请向用户询问
