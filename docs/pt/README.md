[🇪🇸 Español](../es/README.md) | [🇺🇸 English](../../README.md) | [🇩🇪 Deutsch](../de/README.md) | 🇧🇷 **Português** | [🇨🇳 中文](../zh/README.md)

# 🗝️ License Generator

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.2-ff69b4?logo=avaloniaui)](https://avaloniaui.net/)
[![License](https://img.shields.io/badge/License-Custom%20EULA-blue)](../../LICENSE)
[![Docs](https://img.shields.io/badge/Docs-OPERATIONS.md-success)](OPERATIONS.md)

**License Generator** é um poderoso utilitário de desktop projetado para que desenvolvedores independentes gerenciem o ciclo de vida completo de suas licenças de software de forma autônoma, segura e offline.

As licenças geradas usam padrões criptográficos abertos (RSA + PEM + SHA256), então **qualquer aplicativo em qualquer linguagem** (C#, Python, Node.js, Java, Go, Rust...) pode validá-las. Você gerencia as chaves desta ferramenta; seus clientes só precisam da chave pública.

---

## 🌟 Principais Recursos

- **Criptografia RSA**: Assinatura de licenças com RSA de 2048 bits usando o padrão moderno **PEM (PKCS#8)**.
- **Vínculo de Hardware (HWID)**: Vincula licenças a um computador específico para evitar pirataria entre PCs.
- **Gerenciamento Multi-Produto**: Gerencie as chaves de todos os seus projetos a partir de uma única interface limpa.
- **Assinaturas**: Gere licenças com data de expiração para modelos de negócios baseados em tempo.
- **Privacidade Absoluta**: Tudo é local. Suas chaves privadas nunca saem do seu computador.
- **Histórico Completo**: Registro detalhado de cada licença emitida para controle total da sua base de usuários.
- **Cross-Platform**: As licenças são JSON assinado com RSA padrão — validáveis de qualquer stack tecnológica.

---

## 📂 Documentação

Para uma integração bem-sucedida, consulte estes manuais detalhados:

1.  [**Guia de Operações e Integração**](OPERATIONS.md): Passo a passo para integrar licenças em seus apps, com exemplos completos em **C#**, **Python** e **Node.js**.
2.  [**Arquitetura e Segurança**](ARCHITECTURE.md): Detalhes técnicos sobre RSA, SHA256 e o fluxo de confiança.

---

## ⚡ Início Rápido

1.  **Crie seu App**: Clique em "Gerenciamento de Apps" e adicione seu produto. Isso gerará suas chaves em `AppData`.
2.  **Incorpore a Chave**: Copie a chave pública gerada (`public.pem`) para o seu código cliente.
3.  **Gere uma Licença**: Insira o HWID do cliente e entregue a ele o código resultante.

---

## 🛠️ Stack Tecnológico

**Gerador (esta ferramenta):**
- **Framework**: Avalonia UI (.NET 8.0).
- **Padrão**: MVVM com CommunityToolkit.
- **Segurança**: System.Security.Cryptography.
- **Persistência**: JSON local em `%LocalAppData%`.

**Clientes (seus apps):**
- Qualquer linguagem que suporte RSA + PEM (todas as modernas).
- Veja [OPERATIONS.md](OPERATIONS.md) para exemplos em C#, Python e Node.js.

---

## 📜 Licença

Este software é distribuído sob um contrato de licença de usuário final (**EULA**) personalizado.
- **Uso para Desenvolvedores**: Você é livre para usar esta ferramenta para gerar licenças para seus próprios aplicativos comerciais ou gratuitos.
- **Restrições do Gerador**: A venda, redistribuição, modificação ou engenharia reversa deste software (License Generator) é estritamente proibida.
- Consulte o arquivo [LICENSE](../../LICENSE) para ver os termos completos.

---

*Desenvolvido com ❤️ para empoderar criadores de software.*
