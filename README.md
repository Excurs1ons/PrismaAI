# PrismaAI

<div align="center">

**AI 实时字幕 / 翻译系统**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![MAUI](https://img.shields.io/badge/MAUI-9.0-blue.svg)](https://learn.microsoft.com/en-us/dotnet/maui/whats-new/dotnet-9?view=net-maui-9.0)

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
- **.NET 9**: 使用稳定的 .NET 9 版本

## 技术栈

| 功能 | 技术 | 版本 |
|------|------|------|
| UI 框架 | .NET MAUI | 9.0 |
| C# 语言 | C# 13 | preview |
| ASR 模型 | Whisper (GGUF/ONNX) | Large V3 Turbo |
| 翻译模型 | NLLB-200 / SeamlessM4T | - |
| TTS 模型 | VITS2 / Coqui TTS | - |
| 推理引擎 | ONNX Runtime / llama.cpp | 1.20 |
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

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
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
dotnet build -f net9.0-android

# iOS
dotnet build -f net9.0-ios

# Windows
dotnet build -f net9.0-windows10.0.19041.0

# macOS (MacCatalyst)
dotnet build -f net9.0-maccatalyst
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


## 彩蛋

> [在 Android 手机上编译 APK 的冒险之旅](docs/android-compilation-adventure.md) - 记录在 Termux/proot 环境下尝试编译 .NET MAUI APK 的完整过程
