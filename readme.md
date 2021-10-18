# csharp-ddd-template

## 1. 安装DotNet开发环境
### 1.1 Mac
https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-5.0.401-macos-x64-installer
### 1.2 Linux
https://docs.microsoft.com/zh-cn/dotnet/core/install/linux?WT.mc_id=dotnet-35129-website
### 1.3 Windows
https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-5.0.401-windows-x64-installer

## 2. 安装项目模板
```SHELL
dotnet new -i CSharp.DDD.Template 
```

## 3. 创建项目
``` SHELL
dotnet new ddd -n myapp
cd myapp
```

## 4. 运行项目

### 4.1 基于配置文件
### 4.1.1 运行WebApi
``` SHELL
dotnet run --project ./src/myapp.API
```

### 4.1.2 运行Worker
``` SHELL
dotnet run --project ./src/myapp.Worker
```

### 4.2 基于配置中心
### 4.2.1 运行WebApi
``` SHELL
export Apollo_AppId=csharp-example
export Apollo_Cluster=default
export Apollo_MetaServer=http://test.apollo-configservice.service.consul:8080
export Apollo_Secret=
dotnet run --project ./src/myapp.API
```

### 4.2.2 运行Worker
``` SHELL
export Apollo_AppId=csharp-example
export Apollo_Cluster=default
export Apollo_MetaServer=http://test.apollo-configservice.service.consul:8080
export Apollo_Secret=
dotnet run --project ./src/myapp.Worker
```

## 5. 构建项目

* Windows
``` SHELL
ci-build-linux-x64.cmd
```

* Linux/Mac
``` SHELL
ci-build-linux-x64.sh
```

## 6. 编辑项目
请使用VSCode 或者 VisualStudio

## 8. 项目组件
https://github.com/guoming/Hummingbird
