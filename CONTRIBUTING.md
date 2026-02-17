# Contributing to Outclass Platform

First off, thank you for considering contributing to Outclass! It’s people like you who make Outclass such a great platform for the community.

This guide is designed to help you get started quickly, whether you are a C# expert, a React enthusiast, or a technical writer.

## 1. Our Philosophy
Outclass is built on **Metadata-Driven** principles. This means we prefer solutions that are configurable through data rather than hard-coded logic. If you are adding a feature, ask yourself: *"Can this be made into a reusable building block or a metadata setting?"*

## 2. Getting Started

### Prerequisites
- **Backend**: .NET 10 SDK
- **Frontend**: Node.js 20+
- **Infrastructure**: Docker Desktop (or local Postgres, Redis, RabbitMQ)

### Steps to Contribute
1. **Fork & Clone**: Fork the repository and clone it to your local machine.
2. **Setup**: Follow the instructions in the [main README.md](../../README.md) to get the platform running.
3. **Branching**: Create a short-lived feature branch.
   ```bash
   git checkout -b feat/your-feature-name
   # or
   git checkout -b fix/issue-description
   ```

## 3. Development Workflow

### Backend (C#)
- Follow **Domain-Driven Design (DDD)** patterns.
- Keep business logic in the `Application` or `Domain` projects.
- Use `BuildingBlocks` for cross-cutting concerns (logging, events, auth).
- **Naming**: Use PascalCase for classes and methods.

### Frontend (Next.js & TypeScript)
- Use **Functional Components** and Hooks.
- Ensure all new components are responsive (Tailwind CSS).
- Keep the UI **Metadata-Driven** whenever possible.
- **Naming**: Use PascalCase for components and camelCase for variables/functions.

### Inter-service Communication
- If your change affects multiple services, use **Events** via RabbitMQ. 
- Do not make direct HTTP calls between microservices if it involves writing data.

## 4. Commit Guidelines
We use [Conventional Commits](https://www.conventionalcommits.org/). This helps us automate our changelogs.

- `feat:` for new features.
- `fix:` for bug fixes.
- `docs:` for documentation changes.
- `chore:` for maintenance (updating dependencies, etc.).
- `refactor:` for code changes that neither fix a bug nor add a feature.

## 5. Pull Request (PR) Process
1. Ensure your code builds locally: `dotnet build` and `npm run build` (in the web folder).
2. Write a clear description of what the PR does.
3. Link any related issues.
4. Once submitted, a maintainer will review your code. We aim for a "friendly collaborator" vibe—we are here to help your code get merged!

## 6. Questions?
If you're unsure about anything, feel free to open a "Discussion" or an "Issue" labeled `question`. No question is too small!

---
*Happy Coding!* 🚀
