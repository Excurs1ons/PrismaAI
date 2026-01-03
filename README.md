# PrismaAI

<div align="center">

**AI 实时字幕 / 翻译系统**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![MAUI](https://img.shields.io/badge/MAUI-10.0-blue.svg)](https://learn.microsoft.com/en-us/dotnet/maui/whats-new/dotnet-10?view=net-maui-10.0)

[English](#) | [中文](#)

</div>

## 简介

PrismaAI 是一个跨平台的 AI 实时字幕和翻译应用程序，支持离线推理和云端 API 两种模式。

### 主要特性

- **跨平台支持**: Windows / macOS / Linux / iOS / Android
- **实时字幕**: 基于 Whisper 的语音识别
- **多语言翻译**: 支持 NLLB-200 / SeamlessM4T
- **语音合成**: 可选的 TTS 输出
- **离线优先**: 本地模型，无需网络
- **云端加速**: 支持云端 API (OpenAI / Groq / DeepSeek)
- **量化模型**: 支持 GGUF / ONNX 格式
- **.NET 10 LTS**: 使用最新的 .NET 长期支持版本

## 技术栈

| 功能 | 技术 | 版本 |
|------|------|------|
| UI 框架 | .NET MAUI | 10.0 |
| C# 语言 | C# 14 | preview |
| ASR 模型 | Whisper (GGUF/ONNX) | Large V3 Turbo |
| 翻译模型 | NLLB-200 / SeamlessM4T | - |
| TTS 模型 | VITS2 / Coqui TTS | - |
| 推理引擎 | ONNX Runtime / llama.cpp | 1.21 |
| 云端 API | OpenAI / Groq 兼容 | - |

## 架构

### 分层设计

1. **UI 层** (PrismaAI.UI)
   - MAUI 跨平台界面
   - MVVM 架构模式
   - 响应式数据绑定

2. **核心层** (PrismaAI.Core)
   - 音频捕获模块
   - AI 推理引擎
   - 处理流水线
   - 云端 API 客户端

3. **推理层**
   - 本地 ONNX Runtime
   - GGUF (llama.cpp)
   - 云端 API 调用

## 模型支持

### 语音识别 (ASR)

| 模型 | 格式 | 大小 | 推荐场景 |
|------|------|------|----------|
| Whisper Large V3 Turbo | GGUF/ONNX | ~1.5GB | 速度优先 |
| Whisper Large V3 | GGUF/ONNX | ~3GB | 精度优先 |
| Distil Whisper | GGUF/ONNX | ~500MB | 资源受限 |

### 翻译

| 模型 | 格式 | 支持语言 |
|------|------|----------|
| NLLB-200 distilled 600M | GGUF/ONNX | 200+ 语言 |
| SeamlessM4T V2 | GGUF/ONNX | 100+ 语言 |

### TTS

| 模型 | 格式 | 特点 |
|------|------|------|
| VITS2 | ONNX | 高质量 |
| Coqui TTS | ONNX | 多语言 |

## 快速开始

### 前置要求

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (LTS, 支持到 2028年11月)
- [MAUI Workload](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation)

### 安装

```bash
# 克隆仓库
git clone https://github.com/Excurs1ons/PrismaAI.git
cd PrismaAI

# 安装 MAUI 工作负载
dotnet workload install maui

# 还原依赖
dotnet restore

# 运行
dotnet run
```

### 构建特定平台

```bash
# Android
dotnet build -f net10.0-android

# iOS
dotnet build -f net10.0-ios

# Windows
dotnet build -f net10.0-windows10.0.26100

# macOS (MacCatalyst)
dotnet build -f net10.0-maccatalyst

# Linux (.NET 10 MAUI 新增支持)
dotnet build -f net10.0-linux
```

## 云端 API 配置

应用支持 OpenAI 兼容的 API：

1. 进入设置页面
2. 启用"使用云端 API"
3. 配置 API Endpoint 和 Key
4. 选择 API 提供商

### 支持的 API 提供商

- **OpenAI**: `https://api.openai.com/v1`
- **Groq**: `https://api.groq.com/openai/v1` (超快推理)
- **DeepSeek**: `https://api.deepseek.com/v1`
- **其他 OpenAI 兼容 API**

## 项目结构

```
PrismaAI/
├── src/
│   ├── PrismaAI.Core/           # 核心业务逻辑
│   │   ├── Audio/               # 音频捕获
│   │   ├── Inference/           # AI 推理引擎
│   │   ├── Pipeline/            # 处理流水线
│   │   └── Services/            # 云端 API 服务
│   └── PrismaAI.UI/             # MAUI UI
│       ├── Views/
│       ├── ViewModels/
│       └── Services/
├── models/                      # 模型文件 (Git LFS)
│   ├── whisper/
│   ├── translation/
│   └── tts/
└── .github/workflows/           # CI/CD
```

## 贡献

欢迎贡献！请查看 [CONTRIBUTING.md](CONTRIBUTING.md) 了解详情。

## 许可证

[MIT License](LICENSE)

## 致谢

- [OpenAI Whisper](https://github.com/openai/whisper)
- [faster-whisper](https://github.com/SYSTRAN/faster-whisper)
- [NLLB](https://github.com/facebookresearch/fairseq/tree/nllb)
- [SeamlessM4T](https://github.com/facebookresearch/seamless_communication)
- [.NET MAUI](https://github.com/dotnet/maui)
- [ONNX Runtime](https://github.com/microsoft/onnxruntime)

## Star History

[![Star History Chart](https://api.star-history.com/svg?repos=your-username/PrismaAI&type=Date)](https://star-history.com/#your-username/PrismaAI&Date)

---

## <img src="https://img.icons8.com/emoji/48/000000/easter-egg.png" width="24"/> 彩蛋

### 在 Android 手机上直接编译 APK 的冒险之旅

有一天，我们在 Termux (Android) 上突发奇想：**能不能在手机上直接编译这个 Android APK？**

于是开始了这段旅程...

#### 第一步：环境准备

```bash
# 系统信息
$ uname -m
aarch64  # ARM64 架构

$ java -version
openjdk version "21.0.9" 2025-10-21
OpenJDK Runtime Environment (build 21.0.9+13-Ubuntu-0.2510.1)
# Java 21 已就绪 ✅

$ df -h /storage/emulated/0 | tail -1
可用空间: 90G  # 存储充足 ✅
```

#### 第二步：安装 Ubuntu (proot-distro)

```bash
$ proot-distro install ubuntu

[1m[34m[*] [0m[32mInstalling [0m[34mUbuntu (25.10)[0m...
[1m[34m[*] [0m[32mCreating directory[0m...
[1m[34m[*] [0m[36mDownloading rootfs archive...[0m
URL: https://github.com/termux/proot-distro/releases/download/v4.30.1/ubuntu-questing-aarch64-pd-v4.30.1.tar.xz

  % Total    % Received % Xferd  Average Speed   Time    Time     Time     Speed
 100 56476k  100 56476k    0     0   58345      0  0:01:41  0:01:41 --:--:-- 583454

[1m[34m[*] [0m[36mChecking integrity, please wait...[0m
[1m[34m[*] [0m[36mExtracting rootfs, please wait...[0m
[1m[34m[*] [0m[36mFinished.[0m

# Ubuntu 25.10 (aarch64) 安装成功！[31mWarning: CPU doesn't support 32-bit instructions[0m
# （这是正常的，因为我们是纯 64 位 ARM）
```

#### 第三步：安装 .NET 10 SDK

```bash
$ proot-distro login ubuntu
$ apt-get update && apt-get install -y dotnet-sdk-10.0

Welcome to .NET 10.0!
---------------------
SDK Version: 10.0.100
---------------------

$ dotnet --version
10.0.100  ✅
```

#### 第四步：尝试安装 MAUI Workload

```bash
$ dotnet workload install maui
Workload installation failed: Workload ID maui is not recognized.

# 尝试搜索可用的工作负载
$ dotnet workload search

Workload ID                 Description
---------------------------------------------------------------------------
wasi-experimental           workloads/wasi-experimental/description
wasm-experimental           .NET WebAssembly experimental tooling
wasm-tools                  .NET WebAssembly build tools
wasm-tools-net8             .NET WebAssembly build tools for net8.0
wasm-tools-net9             .NET WebAssembly build tools for .NET 9.0

# 没有任何 Android 或 MAUI 相关的 workload！❌
```

#### 第五步：尝试 WebAssembly 构建

```bash
$ dotnet workload install wasm-tools

Installing pack Microsoft.NETCore.App.Runtime.AOT.linux-arm64.Cross.browser-wasm...
Installing pack Microsoft.NET.Runtime.MonoTargets.Sdk...
Installing pack Microsoft.NET.Runtime.MonoAOTCompiler.Task...
Installing pack Microsoft.NET.Runtime.Emscripten.3.1.56.Cache.linux-arm64...

Workload installation failed. Rolling back installed packs...

System.TypeInitializationException: The type initializer for 'Microsoft.DotNet.Cli.Parser' threw an exception.
---> System.IO.FileNotFoundException: Unable to find the specified file.
   at Interop.Sys.GetCwdHelper(Byte* ptr, Int32 bufferSize)
   at Interop.Sys.GetCwd()
```

#### 第六步：尝试直接编译 Core 项目

```bash
$ dotnet build src/PrismaAI.Core/PrismaAI.Core.csproj -c Release

/usr/lib/dotnet/sdk/10.0.100/Roslyn/Microsoft.CSharp.Core.targets(84,5): error : Failed to create CoreCLR, HRESULT: 0x8007000E

Build FAILED.

warning NU1510: PackageReference System.Text.Json will not be pruned.
warning NU1603: PrismaAI.Core depends on Microsoft.ML.OnnxRuntime.Extensions (>= 0.11.0) but Microsoft.ML.OnnxRuntime.Extensions 0.11.0 was not found.

error : GC: Reserving 274877906944 bytes (256 GiB) for the regions range failed
error : GC heap initialization failed with error 0x8007000E
error : Failed to create CoreCLR, HRESULT: 0x8007000E

# proot 环境的虚拟内存限制！GC 试图分配 256GB...❌
```

#### 第七步：包版本大作战

```bash
# 第一次尝试 - 使用最新版本
<PackageReference Include="Microsoft.ML.OnnxRuntime.Extensions" Version="1.21.0" />
<PackageReference Include="Microsoft.ML.AI" Version="10.0.0" />
<PackageReference Include="Serilog" Version="5.0.0" />

error NU1102: Unable to find package Microsoft.ML.OnnxRuntime.Extensions with version (>= 1.21.0)
error NU1101: Unable to find package Microsoft.ML.AI. No packages exist with this id
error NU1102: Unable to find package Serilog with version (>= 5.0.0)

# 第二次尝试 - 使用次新版本
<PackageReference Include="Microsoft.ML.OnnxRuntime.Extensions" Version="0.15.2" />
<PackageReference Include="Serilog" Version="4.3.1" />

error NU1102: Found 12 version(s) in nuget.org [ Nearest version: 0.15.2-dev-20251214-1129-7387a4eb ]
error NU1102: Found 587 version(s) in nuget.org [ Nearest version: 4.3.1-dev-02395 ]

# 第三次尝试 - 使用稳定版本 ✅
<PackageReference Include="Microsoft.ML.OnnxRuntime.Extensions" Version="0.11.0" />
<PackageReference Include="Serilog" Version="4.2.0" />

# 包依赖解析成功！但编译失败（内存限制）
```

#### 完整的错误日志

```
[31mWarning: CPU doesn't support 32-bit instructions, some software may not work.[0m
proot warning: can't sanitize binding "/proc/self/fd/1": No such file or directory
proot warning: can't sanitize binding "/proc/self/fd/2": No such file or directory

Build FAILED.

error : GC: Reserving 274877906944 bytes (256 GiB) for the regions range failed
error : GC heap initialization failed with error 0x8007000E
error : Failed to create CoreCLR, HRESULT: 0x8007000E

4 Warning(s)
3 Error(s)

Time Elapsed 00:00:52.80
```

#### 失败原因分析

| 层级 | 问题 | 原因 |
|------|------|------|
| **硬件** | ARM64 架构 | 不被某些原生工具支持 |
| **操作系统** | Android | 不是传统的 Linux 发行版 |
| **虚拟化** | proot | 用户空间虚拟化，有限制 |
| **.NET Workload** | MAUI 不可用 | 官方不支持 proot 环境 |
| **内存管理** | GC 256GiB 失败 | proot 虚拟内存地址空间限制 |

#### 尝试过的解决方案

```bash
# 方案 1: 直接安装 .NET (Termux)
$ pkg install dotnet
E: Unable to locate package dotnet  # ❌ Termux 没有 .NET 包

# 方案 2: 下载 .NET 安装脚本
$ curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version latest --channel 10.0
curl: (28) Failed to connect to builds.dotnet.microsoft.com port 443 after 35648 ms  # ❌ 网络问题

# 方案 3: 使用不同的 Ubuntu 仓库
echo 'deb [arch=arm64 signed-by=/usr/share/keyrings/microsoft-prod.gpg] https://packages.microsoft.com/ubuntu/24.04/prod jammy main' > /etc/apt/sources.list.d/microsoft.list
Err:2 https://packages.microsoft.com/ubuntu/24.04/prod jammy Release
  404  Not Found  # ❌ 仓库配置错误

# 方案 4: 尝试 WASM 构建
dotnet workload install wasm-tools
System.TypeInitializationException...  # ❌ proot 环境限制

# 方案 5: 降低复杂度，只编译 Core
dotnet build src/PrismaAI.Core/PrismaAI.Core.csproj
GC: Reserving 274877906944 bytes (256 GiB) failed  # ❌ 内存限制
```

#### 结论与教训

```
┌─────────────────────────────────────────────────────────────┐
│                    Termux 编译可行性矩阵                      │
├─────────────────────────────────────────────────────────────┤
│  项目          │ Termux │ proot Ubuntu │ 真机/PC │ CI/CD   │
├─────────────────────────────────────────────────────────────┤
│  .NET SDK      │   ❌   │      ✅      │    ✅   │   ✅    │
│  MAUI Workload │   ❌   │      ❌      │    ✅   │   ✅    │
│  Android SDK   │   ❌   │      ❌      │    ✅   │   ✅    │
│  编译 APK      │   ❌   │      ❌      │    ✅   │   ✅    │
└─────────────────────────────────────────────────────────────┘
```

**最终结论**:

> "在 Android 上编译 Android APK" 听起来是个很酷的想法，
> 但由于技术限制（MAUI workload 不支持、proot 内存限制、ARM64 兼容性），
> 这实际上是一个**不可能完成的任务**。

**教训**：
1. 🔧 **合适的工具做合适的事** - 开发环境应该用 PC
2. 🚀 **GitHub Actions 是朋友** - 云端构建省时省力
3. 📱 **Termux 适合学习/测试** - 不适合大型项目开发
4. 💡 **proot 有其限制** - 虚拟化不是万能的

**推荐方式**:

```bash
# 方案 A: 在 PC 上构建 (推荐)
dotnet workload install maui
dotnet build -f net10.0-android

# 方案 B: 使用 GitHub Actions (自动化)
# 在 .github/workflows/build-android.yml 中配置自动构建

# 方案 C: 使用 GitHub Codespaces (云端开发)
# 直接在浏览器中使用完整开发环境
```

---

*"A journey of a thousand commits begins with a single `dotnet build`"* 🎯
