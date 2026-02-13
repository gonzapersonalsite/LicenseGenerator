# 🗝️ License Generator

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.2-ff69b4?logo=avaloniaui)](https://avaloniaui.net/)

**License Generator** es una potente utilidad de escritorio diseñada para que desarrolladores independientes gestionen el ciclo de vida completo de sus licencias de software de forma autónoma, segura y offline.

Las licencias generadas usan estándares criptográficos abiertos (RSA + PEM + SHA256), por lo que **cualquier aplicación en cualquier lenguaje** (C#, Python, Node.js, Java, Go, Rust...) puede validarlas. Tú gestionas las llaves desde esta herramienta; tus clientes solo necesitan la clave pública.

---

## 🌟 Características Destacadas

- **RSA Criptografía**: Firma de licencias con RSA de 2048 bits utilizando el estándar moderno **PEM (PKCS#8)**.
- **Bloqueo por Hardware (HWID)**: Vincula licencias a un equipo específico para evitar la piratería entre PCs.
- **Gestión Multi-Producto**: Administra las llaves de todos tus proyectos desde una única interfaz limpia.
- **Suscripciones**: Genera licencias con fecha de expiración para modelos de negocio basados en tiempo.
- **Privacidad Absoluta**: Todo es local. Tus llaves privadas nunca salen de tu ordenador.
- **Historial Completo**: Registro detallado de cada licencia emitida para un control total de tu base de usuarios.
- **Cross-Platform**: Las licencias son JSON firmado con RSA estándar — validables desde cualquier stack tecnológico.

---

## 📂 Documentación

Para una integración exitosa, consulta estos manuales detallados:

1.  [**Guía de Operaciones e Integración (Guía Burros)**](OPERATIONS.md): Paso a paso para integrar las licencias en tus apps, con ejemplos completos en **C#**, **Python** y **Node.js**.
2.  [**Arquitectura y Seguridad**](ARCHITECTURE.md): Detalle técnico sobre RSA, SHA256 y el flujo de confianza.

---

## ⚡ Inicio Rápido

1.  **Crea tu App**: Pulsa en "Gestión de Apps" y añade tu producto. Esto generará tus llaves en `AppData`.
2.  **Incrusta la Clave**: Copia la clave pública (`public.pem`) generada en tu código cliente.
3.  **Genera una Licencia**: Introduce el HWID del cliente y entrégale el código resultante.

---

## 🛠️ Stack Tecnológico

**Generator (esta herramienta):**
- **Framework**: Avalonia UI (.NET 8.0).
- **Patrón**: MVVM con CommunityToolkit.
- **Seguridad**: System.Security.Cryptography.
- **Persistencia**: Local JSON en `%LocalAppData%`.

**Clientes (tus apps):**
- Cualquier lenguaje que soporte RSA + PEM (todos los modernos).
- Ver [OPERATIONS.md](OPERATIONS.md) para ejemplos en C#, Python y Node.js.

---

## 📜 Licencia

Este software se distribuye bajo un contrato de licencia de usuario final (**EULA**) personalizado. 
- Se permite el uso personal e interno sin coste.
- Queda prohibida la redistribución comercial, modificación o ingeniería inversa del código fuente.
- Consulta el archivo [LICENSE](LICENSE) para ver los términos completos.

---

*Desarrollado con ❤️ para empoderar a los creadores de software.*
