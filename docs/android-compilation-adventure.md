# 在 Android 手机上编译 APK 的冒险之旅

> 能不能在手机上直接编译这个 Android APK？于是开始了这段旅程...

## 环境信息

```bash
$ uname -m
aarch64  # ARM64 架构

$ java -version
openjdk version "21.0.9" 2025-10-21
OpenJDK Runtime Environment (build 21.0.9+13-Ubuntu-0.2510.1)
# Java 21 已就绪

$ df -h /storage/emulated/0 | tail -1
可用空间: 90G  # 存储充足
```

## 第一轮尝试：.NET 10 + MAUI

### 安装 Ubuntu (proot-distro)

```bash
$ proot-distro install ubuntu

Ubuntu 25.10 (aarch64) 安装成功！
Warning: CPU doesn't support 32-bit instructions
# （这是正常的，因为我们是纯 64 位 ARM）
```

### 安装 .NET 10 SDK

```bash
$ apt-get update && apt-get install -y dotnet-sdk-10.0

$ dotnet --version
10.0.100  # 但 MAUI 不支持 .NET 10？！
```

> **重要发现**: .NET 10 于 2025年11月发布，但 MAUI workload 尚未完全支持。
> .NET 9 (2024年11月发布) 才是目前 MAUI 的最佳选择！

### 尝试安装 MAUI Workload

```bash
$ dotnet workload install maui
Workload installation failed: Workload ID maui is not recognized.

$ dotnet workload search

Workload ID                 Description
---------------------------------------------------------------------------
wasi-experimental           .NET WebAssembly experimental tooling
wasm-experimental           .NET WebAssembly experimental tooling
wasm-tools                  .NET WebAssembly build tools
wasm-tools-net8             .NET WebAssembly build tools for net8.0
wasm-tools-net9             .NET WebAssembly build tools for .NET 9.0

# 没有任何 Android 或 MAUI 相关的 workload！
```

### 尝试直接编译 Core 项目

```bash
$ dotnet build src/PrismaAI.Core/PrismaAI.Core.csproj -c Release

error : GC: Reserving 274877906944 bytes (256 GiB) for the regions range failed
error : GC heap initialization failed with error 0x8007000E
error : Failed to create CoreCLR, HRESULT: 0x8007000E

# proot 环境的虚拟内存限制！GC 试图分配 256GB...
```

### 包版本大作战

```bash
# 第一次尝试 - 使用最新版本
<PackageReference Include="Microsoft.ML.OnnxRuntime.Extensions" Version="1.21.0" />
<PackageReference Include="Serilog" Version="5.0.0" />

error NU1102: Unable to find package Microsoft.ML.OnnxRuntime.Extensions with version (>= 1.21.0)
error NU1102: Unable to find package Serilog with version (>= 5.0.0)

# 第二次尝试 - 使用稳定版本
<PackageReference Include="Microsoft.ML.OnnxRuntime.Extensions" Version="0.11.0" />
<PackageReference Include="Serilog" Version="4.2.0" />

# 包依赖解析成功！但编译失败（内存限制）
```

### 失败原因分析

| 层级 | 问题 | 原因 |
|------|------|------|
| **硬件** | ARM64 架构 | 不被某些原生工具支持 |
| **操作系统** | Android | 不是传统的 Linux 发行版 |
| **虚拟化** | proot | 用户空间虚拟化，有限制 |
| **.NET Workload** | MAUI 不可用 | 官方不支持 proot 环境 |
| **内存管理** | GC 256GiB 失败 | proot 虚拟内存地址空间限制 |

---

## 第二轮尝试：.NET 9 + GC 限制

虽然第一轮以失败告终，但我们没有放弃！

### 新的策略

```bash
# 降级到 .NET 9
$ apt-get update && apt-get install -y dotnet-sdk-9.0

$ dotnet --version
9.0.112

# 设置 GC 堆限制（关键！）
export DOTNET_GCHeapHardLimit=0x10000000  # 256 MB
export DOTNET_GCHeapCount=1

# 尝试编译 Core 项目
$ dotnet build src/PrismaAI.Core/PrismaAI.Core.csproj

Build succeeded.

    3 Warning(s)
    0 Error(s)

Time Elapsed 00:00:05.35
```

### Core 项目编译成功！

通过设置 `DOTNET_GCHeapHardLimit=0x10000000`，我们成功绕过了 proot 环境的内存限制！

但 Android APK 仍然无法编译：

```bash
$ dotnet build src/PrismaAI.UI/PrismaAI.UI.csproj -f net9.0-android

error NETSDK1147: To build this project, the following workloads must be installed: wasi-experimental

$ dotnet workload restore
No workloads installed for this feature band.
Installing workloads: wasi-experimental  # (不是 MAUI!)

$ dotnet workload search android
# (空结果)

$ dotnet workload search maui
# (空结果)
```

---

## 第三轮尝试：Android SDK CLI + ARM64 构建工具

既然 .NET MAUI workload 不可用，那直接用 Android SDK 命令行工具呢？

### 安装 Android SDK CLI

```bash
# 下载命令行工具
$ wget https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip

$ unzip -q commandlinetools-linux-11076708_latest.zip -d /opt/android-sdk/cmdline-tools/

$ sdkmanager --version
12.0  # 可以运行！
```

### 安装 Android 构建工具

```bash
$ sdkmanager "platform-tools" "platforms;android-35" "build-tools;35.0.0"

# 接受许可证后安装成功
```

### 问题：构建工具是 x86-64 架构

```bash
$ file /opt/android-sdk/build-tools/35.0.0/aapt2
aapt2: ELF 64-bit LSB pie executable, x86-64, version 1 (SYSV)

$ /opt/android-sdk/build-tools/35.0.0/aapt2 version
cannot execute: required file not found  # 无法在 ARM64 上运行
```

**Google 的 Android SDK Build-Tools 只提供 x86/x86-64 版本，没有 ARM64 版本！**

---

## 第四轮尝试：社区 ARM64 构建工具

既然官方没有，那社区呢？找到了几个开源项目：

- [rendiix/termux-aapt](https://github.com/rendiix/termux-aapt) - ARM64 版 aapt/aapt2
- [lzhiyong/android-sdk-tools](https://github.com/lzhiyong/android-sdk-tools) - 从源码构建
- [skyleecm/android-build-tools-for-arm](https://github.com/skyleecm/android-build-tools-for-arm) - ARM Linux 构建

### 安装 ARM64 aapt/aapt2

```bash
$ git clone https://github.com/rendiix/termux-aapt.git
$ ls termux-aapt/prebuilt-binary/arm64/
aapt  aapt2

$ file termux-aapt/prebuilt-binary/arm64/aapt2
aapt2: ELF 64-bit LSB executable, ARM aarch64, version 1 (SYSV), statically linked

$ cp termux-aapt/prebuilt-binary/arm64/* /opt/android-sdk/build-tools/35.0.0/

$ /opt/android-sdk/build-tools/35.0.0/aapt2 version
Android Asset Packaging Tool (aapt) 2.19-vanzdobz@gmail.com  # 可以运行！
```

### 最终状态

| 工具 | 来源 | 状态 |
|------|------|------|
| .NET 9 SDK | Ubuntu repo | 运行正常 |
| Java 21 | OpenJDK | 运行正常 |
| Android SDK CLI | Google official | 可用 |
| aapt/aapt2 (ARM64) | termux-aapt | 运行正常 |
| platform-tools | Google official | 可用 |
| **MAUI Workload** | Microsoft | **不可用** |

### 尝试构建 .NET MAUI

```bash
$ dotnet build src/PrismaAI.UI/PrismaAI.UI.csproj -f net9.0-android

error NETSDK1139: The target platform identifier android was not recognized.
```

**问题依然存在：.NET SDK 不识别 `android` 目标框架，因为 MAUI workload 没有安装。**

---

## 结论

### 在 Termux/proot 环境编译 .NET MAUI APK 不可行

| 组件 | 状态 | 原因 |
|------|------|------|
| .NET 9 SDK | 可用 | 官方支持 ARM64 Linux |
| Android SDK CLI | 可用 | Java 工具，跨平台 |
| aapt/aapt2 | 可用 | 社区 ARM64 版本 |
| zipalign | 不可用 | x86-64 二进制 |
| **MAUI Workload** | **不可用** | **Microsoft 不提供 ARM64 Linux 版本** |

### 根本原因

即使有 Android SDK 工具，.NET 构建系统需要 MAUI workload 的 MSBuild 目标和运行时，而 Microsoft 官方**不提供 ARM64 Linux 的 MAUI workload**。

### 可行的替代方案

1. **GitHub Actions** - 使用云端 x64 runner 构建
2. **真正的 ARM64 Linux PC** - 使用 Android Studio ARM64 原生版本
3. **x64 PC/Linux** - 完整支持 .NET MAUI

### 版本选择经验

| .NET 版本 | 发布时间 | MAUI 支持 | 推荐使用 |
|-----------|----------|-----------|----------|
| .NET 8 | 2023年11月 | 完全支持 | 稳定生产 |
| .NET 9 | 2024年11月 | 完全支持 | 推荐使用 |
| .NET 10 | 2025年11月 | 部分支持 | 等待更新 |

> **经验教训**: 对于 MAUI 项目，建议使用 .NET 9 (当前稳定) 而非 .NET 10 (太新)

---

## 教训总结

1. **合适的工具做合适的事** - 开发环境应该用 PC
2. **GitHub Actions 是朋友** - 云端构建省时省力
3. **Termux 适合学习/测试** - 不适合大型项目开发
4. **proot 有其限制** - 虚拟化不是万能的
5. **ARM64 Linux 支持需要社区** - 官方可能不提供

---

*"Success is not final, failure is not fatal: it is the courage to continue that counts."* 💪
