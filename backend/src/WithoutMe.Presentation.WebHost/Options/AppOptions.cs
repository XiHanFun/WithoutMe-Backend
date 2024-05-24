﻿#region <<版权版本注释>>

// ----------------------------------------------------------------
// Copyright ©2024 ZhaiFanhua All Rights Reserved.
// Licensed under the MulanPSL2 License. See LICENSE in the project root for license information.
// FileName:AppOptions
// Guid:f97374a5-1f8f-4e97-958d-9967a88b7cda
// Author:Administrator
// Email:me@zhaifanhua.com
// CreateTime:2024-05-24 下午 05:27:05
// ----------------------------------------------------------------

#endregion <<版权版本注释>>

namespace WithoutMe.Presentation.WebHost.Options;

/// <summary>
/// AppOptions
/// </summary>
public class AppOptions
{
    /// <summary>
    /// 是否演示模式
    /// </summary>
    public bool IsDemoMode { get; set; }

    /// <summary>
    /// 环境
    /// </summary>
    public string EnvironmentName { get; set; } = string.Empty;

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; }
}