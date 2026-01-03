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
openjdk version "21.0.9"  # Java 已就绪 ✅
```

#### 第二步：安装 Ubuntu

```bash
$ proot-distro install ubuntu
[*] Installing Ubuntu (25.10)...
[*] Downloading rootfs archive...
# 下载 56MB 的 Ubuntu rootfs...
[100%] ========================================
[*] Finished. ✅
```

#### 第三步：安装 .NET 10

```bash
$ apt-get install dotnet-sdk-10.0
$ dotnet --version
10.0.100 ✅
```

#### 第四步：安装 MAUI Workload

```bash
$ dotnet workload install maui
Workload installation failed: Workload ID maui is not recognized.

$ dotnet workload search
# 可用列表：
# wasm-tools, wasm-experimental...
# 没有 maui？没有 android？
```

#### 第五步：尝试编译

```bash
$ dotnet build src/PrismaAI.Core/PrismaAI.Core.csproj
error : GC: Reserving 274877906944 bytes (256 GiB) for the regions range failed
error : GC heap initialization failed with error 0x8007000E
error : Failed to create CoreCLR, HRESULT: 0x8007000E
```

#### 结论

在 Android/Termux/proot 环境中编译 .NET MAUI APK 的困难：

| 问题 | 状态 |
|------|------|
| 安装 Ubuntu | ✅ 成功 |
| 安装 .NET 10 SDK | ✅ 成功 |
| MAUI Workload 可用性 | ❌ 不支持 ARM64/proot |
| 内存限制 (GC 256GiB) | ❌ proot 虚拟内存限制 |

**教训**：有些事还是得在 PC 上做...或者使用 GitHub Actions 😄

```bash
# 推荐方式：在 PC 上构建
dotnet workload install maui
dotnet build -f net10.0-android

# 或使用 GitHub Actions 自动构建
```
