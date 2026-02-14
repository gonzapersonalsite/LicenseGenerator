

# 🗝️ License Generator

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.2-ff69b4?logo=avaloniaui)](https://avaloniaui.net/)
[![License](https://img.shields.io/badge/License-Custom%20EULA-blue)](../../LICENSE)
[![Docs](https://img.shields.io/badge/Docs-OPERATIONS.md-success)](OPERATIONS.md)

[🇪🇸 Español](../es/README.md) | [🇺🇸 English](../../README.md) | 🇩🇪 **Deutsch** | [🇧🇷 Português](../pt/README.md) | [🇨🇳 中文](../zh/README.md)

**License Generator** ist ein leistungsstarkes Desktop-Tool, mit dem unabhängige Entwickler den gesamten Lebenszyklus ihrer Softwarelizenzen autonom, sicher und offline verwalten können.

Die generierten Lizenzen verwenden offene kryptographische Standards (RSA + PEM + SHA256), sodass **jede Anwendung in jeder Sprache** (C#, Python, Node.js, Java, Go, Rust...) sie validieren kann. Sie verwalten die Schlüssel über dieses Tool; Ihre Kunden benötigen lediglich den öffentlichen Schlüssel.

---

## 🌟 Hauptfunktionen

- **RSA-Kryptographie**: Lizenzsignierung mit 2048-Bit-RSA unter Verwendung des modernen **PEM (PKCS#8)**-Standards.
- **Hardware-Bindung (HWID)**: Bindet Lizenzen an einen bestimmten Computer, um Piraterie zwischen PCs zu verhindern.
- **Multi-Produkt-Management**: Verwalten Sie Schlüssel für alle Ihre Projekte über eine einzige, übersichtliche Oberfläche.
- **Abonnements**: Erstellen Sie Lizenzen mit Ablaufdatum für zeitbasierte Geschäftsmodelle.
- **Absolute Privatsphäre**: Alles läuft lokal ab. Ihre privaten Schlüssel verlassen niemals Ihren Computer.
- **Vollständige Historie**: Detailliertes Protokoll jeder ausgestellten Lizenz für volle Kontrolle über Ihre Nutzerbasis.
- **Plattformunabhängig**: Lizenzen sind Standard-RSA-signierte JSON-Dateien – validierbar von jedem Tech-Stack.

---

## 📂 Dokumentation

Für eine erfolgreiche Integration konsultieren Sie diese detaillierten Handbücher:

1.  [**Betriebs- und Integrationsanleitung**](OPERATIONS.md): Schritt-für-Schritt-Anleitung zur Integration von Lizenzen in Ihre Apps, mit kompletten Beispielen in **C#**, **Python** und **Node.js**.
2.  [**Architektur und Sicherheit**](ARCHITECTURE.md): Technische Details zu RSA, SHA256 und dem Vertrauensfluss (Trust Flow).

---

## ⚡ Schnellstart

1.  **Erstellen Sie Ihre App**: Klicken Sie auf "App-Verwaltung" und fügen Sie Ihr Produkt hinzu. Dies generiert Ihre Schlüssel in `AppData`.
2.  **Schlüssel einbetten**: Kopieren Sie den generierten öffentlichen Schlüssel (`public.pem`) in Ihren Client-Code.
3.  **Lizenz generieren**: Geben Sie die HWID des Kunden ein und übergeben Sie ihm den resultierenden Code.

---

## 🛠️ Technologie-Stack

**Generator (dieses Tool):**
- **Framework**: Avalonia UI (.NET 8.0).
- **Muster**: MVVM mit CommunityToolkit.
- **Sicherheit**: System.Security.Cryptography.
- **Persistenz**: Lokales JSON in `%LocalAppData%`.

**Clients (Ihre Apps):**
- Jede Sprache, die RSA + PEM unterstützt (alle modernen Sprachen).
- Siehe [OPERATIONS.md](OPERATIONS.md) für Beispiele in C#, Python und Node.js.

---

## 📜 Lizenz

Diese Software wird unter einer benutzerdefinierten Endbenutzer-Lizenzvereinbarung (**EULA**) vertrieben.
- **Entwicklernutzung**: Es steht Ihnen frei, dieses Tool zu verwenden, um Lizenzen für Ihre eigenen kommerziellen oder kostenlosen Anwendungen zu generieren.
- **Einschränkungen**: Verkauf, Weitervertrieb, Modifikation oder Reverse Engineering dieser Software (License Generator) ist strengstens untersagt.
- Siehe die Datei [LICENSE](../../LICENSE) für die vollständigen Bedingungen.

---

*Entwickelt mit ❤️, um Software-Ersteller zu stärken.*
