[🇪🇸 Español](../es/README.md) | [🇺🇸 English](../../README.md) | [🇩🇪 Deutsch](../de/README.md) | [🇧🇷 Português](../pt/README.md) | 🇨🇳 **中文**

# 🗝️ License Generator

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.2-ff69b4?logo=avaloniaui)](https://avaloniaui.net/)
[![License](https://img.shields.io/badge/License-Custom%20EULA-blue)](../../LICENSE)
[![Docs](https://img.shields.io/badge/Docs-OPERATIONS.md-success)](OPERATIONS.md)

**License Generator** 是一款强大的桌面实用工具，专为独立开发者设计，用于自主、安全且离线地管理其软件许可证的完整生命周期。

生成的许可证使用开放的加密标准 (RSA + PEM + SHA256)，因此**任何语言的任何应用程序** (C#, Python, Node.js, Java, Go, Rust...) 都可以验证它们。您掌管本工具的私钥；您的客户只需要公钥。

---

## 🌟 主要特性

- **RSA 加密**: 使用现代 **PEM (PKCS#8)** 标准生成 2048 位 RSA 签名的许可证。
- **硬件绑定 (HWID)**: 将许可证绑定到特定计算机，防止在不同 PC 间盗用。
- **多产品管理**: 通过一个简洁的界面管理您所有项目的密钥。
- **订阅支持**: 生成带有过期日期的许可证，适用于基于时间的业务模式。
- **绝对隐私**: 一切都在本地运行。您的私钥永远不会离开您的计算机。
- **完整历史记录**: 详细记录每一个颁发的许可证，完全掌控您的用户群。
- **跨平台**: 许可证是符合标准的 RSA 签名 JSON —— 可在任何技术栈中验证。

---

## 📂 文档

为了因为集成成功，请参考以下详细手册：

1.  [**操作与集成指南**](OPERATIONS.md): 将许可证集成到您的 App 中的分步指南，包含 **C#**, **Python** 和 **Node.js** 的完整示例。
2.  [**架构与安全**](ARCHITECTURE.md): 关于 RSA, SHA256 和信任流的技术细节。

---

## ⚡ 快速开始

1.  **创建您的 App**: 点击 "应用管理 (Apps)" 并添加您的产品。这将在 `AppData` 中生成您的密钥。
2.  **嵌入密钥**: 将生成的公钥 (`public.pem`) 复制到您的客户端代码中。
3.  **生成许可证**: 输入客户的 HWID 并将生成的代码发送给他们。

---

## 🛠️ 技术栈

**生成器 (本工具):**
- **Framework**: Avalonia UI (.NET 8.0).
- **模式**: MVVM with CommunityToolkit.
- **安全**: System.Security.Cryptography.
- **持久化**: `%LocalAppData%` 中的本地 JSON。

**客户端 (您的应用):**
- 任何支持 RSA + PEM 的语言 (所有现代语言均支持)。
- 参见 [OPERATIONS.md](OPERATIONS.md) 获取 C#, Python 和 Node.js 示例。

---

## 📜 许可协议

本软件根据自定义的最终用户许可协议 (**EULA**) 分发。
- **开发者使用**: 您可以免费使用本工具为您的商业或免费应用程序生成许可证。
- **生成器限制**: 严禁出售、重新分发、修改或对本软件 (License Generator) 进行逆向工程。
- 请查阅 [LICENSE](../../LICENSE) 文件以了解完整条款。

---

*用 ❤️ 开发，为软件创作者赋能。*
