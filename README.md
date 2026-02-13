[🇪🇸 Español](docs/es/README.md) | 🇺🇸 **English** | [🇩🇪 Deutsch](docs/de/README.md) | [🇧🇷 Português](docs/pt/README.md) | [🇨🇳 中文](docs/zh/README.md)

# 🗝️ License Generator

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.2-ff69b4?logo=avaloniaui)](https://avaloniaui.net/)
[![License](https://img.shields.io/badge/License-Custom%20EULA-blue)](../../LICENSE)
[![Docs](https://img.shields.io/badge/Docs-OPERATIONS.md-success)](OPERATIONS.md)

**License Generator** is a powerful desktop utility designed for independent developers to manage the full lifecycle of their software licenses autonomously, securely, and offline.

Generated licenses use open cryptographic standards (RSA + PEM + SHA256), so **any application in any language** (C#, Python, Node.js, Java, Go, Rust...) can validate them. You manage the keys from this tool; your clients only need the public key.

---

## 🌟 Key Features

- **RSA Cryptography**: License signing with 2048-bit RSA using the modern **PEM (PKCS#8)** standard.
- **Hardware Binding (HWID)**: Binds licenses to a specific machine to prevent piracy between PCs.
- **Multi-Product Management**: Manage keys for all your projects from a single clean interface.
- **Subscriptions**: Generate licenses with expiration dates for time-based business models.
- **Absolute Privacy**: Everything is local. Your private keys never leave your computer.
- **Complete History**: Detailed log of every issued license for total control over your user base.
- **Cross-Platform**: Licenses are standard RSA-signed JSON — validatable from any tech stack.

---

## 📂 Documentation

For successful integration, consult these detailed manuals:

1.  [**Operations and Integration Guide**](OPERATIONS.md): Step-by-step guide to integrating licenses into your apps, with complete examples in **C#**, **Python**, and **Node.js**.
2.  [**Architecture and Security**](ARCHITECTURE.md): Technical details on RSA, SHA256, and the trust flow.

---

## ⚡ Quick Start

1.  **Create your App**: Click on "App Management" and add your product. This will generate your keys in `AppData`.
2.  **Embed the Key**: Copy the generated public key (`public.pem`) into your client code.
3.  **Generate a License**: Enter the client's HWID and give them the resulting code.

---

## 🛠️ Tech Stack

**Generator (this tool):**
- **Framework**: Avalonia UI (.NET 8.0).
- **Pattern**: MVVM with CommunityToolkit.
- **Security**: System.Security.Cryptography.
- **Persistence**: Local JSON in `%LocalAppData%`.

**Clients (your apps):**
- Any language supporting RSA + PEM (all modern ones).
- See [OPERATIONS.md](OPERATIONS.md) for examples in C#, Python, and Node.js.

---

## 📜 License

This software is distributed under a custom End User License Agreement (**EULA**).
- **Developer Use**: You are free to use this tool to generate licenses for your own commercial or free applications.
- **Generator Restrictions**: Selling, redistributing, modifying, or reverse-engineering this software (License Generator) is strictly prohibited.
- See the [LICENSE](../../LICENSE) file for full terms.

---

*Developed with ❤️ to empower software creators.*
