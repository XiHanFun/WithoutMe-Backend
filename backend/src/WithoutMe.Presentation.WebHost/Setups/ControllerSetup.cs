﻿#region <<版权版本注释>>

// ----------------------------------------------------------------
// Copyright ©2024 ZhaiFanhua All Rights Reserved.
// Licensed under the MulanPSL2 License. See LICENSE in the project root for license information.
// FileName:ControllerSetup
// Guid:81f2bcc0-ca30-4527-bfc4-c85d32eaf10e
// Author:Administrator
// Email:me@zhaifanhua.com
// CreateTime:2024-05-25 上午 09:50:35
// ----------------------------------------------------------------

#endregion <<版权版本注释>>

using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using XiHan.Utils.System.Text.Json.Converter;
using WithoutMe.Presentation.WebHost.Filters;

namespace WithoutMe.Presentation.WebHost.Setups;

/// <summary>
/// ControllerSetup
/// </summary>
public static class ControllerSetup
{
    /// <summary>
    /// Controllers服务扩展
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddControllersSetup(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddControllers(configure =>
        {
            // 接受浏览器 Accept 标头
            configure.RespectBrowserAcceptHeader = true;

            // 全局注入过滤器
            configure.Filters.Add<ActionFilterAsyncAttribute>();
            configure.Filters.Add<AuthorizationFilterAsyncAttribute>();
            //configure.Filters.Add<ResourceFilterAsyncAttribute>();
            configure.Filters.Add<ResultFilterAsyncAttribute>();
            // 已弃用，由 GlobalExceptionMiddleware 全局日志中间件代替
            //configure.Filters.Add<ExceptionFilterAsyncAttribute>();
        }).ConfigureApiBehaviorOptions(setupAction =>
        {
            // 关闭默认模型验证，通过 ActionFilterAsyncAttribute 自定义验证
            setupAction.SuppressModelStateInvalidFilter = true;
        })
        .AddJsonOptions(options =>
        {
            // 序列化格式
            options.JsonSerializerOptions.WriteIndented = true;
            // 忽略循环引用
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            // 数字类型
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
            // 允许额外符号
            options.JsonSerializerOptions.AllowTrailingCommas = true;
            // 注释处理，允许在 JSON 输入中使用注释并忽略它们
            options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            // 属性名称使用区分大小写的比较
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
            // 数据格式首字母小写 JsonNamingPolicy.CamelCase 驼峰样式，null则为不改变大小写
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            // 获取或设置要在转义字符串时使用的编码器，UnsafeRelaxedJsonEscaping 为不转义字符
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            // 布尔类型
            options.JsonSerializerOptions.Converters.Add(new BooleanJsonConverter());
            // 数字类型
            options.JsonSerializerOptions.Converters.Add(new IntJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new DecimalJsonConverter());
            // 日期类型
            options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
        });

        return services;
    }
}