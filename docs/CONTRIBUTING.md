# Contributing to AsegyaPay

Thank you for considering contributing to AsegyaPay! This document provides guidelines for contributing.

## Development Setup

1. Clone the repository
2. Install prerequisites (.NET 9, Node.js 20+, Docker)
3. Run `docker-compose up -d` for infrastructure
4. Start developing!

## Code Style

### C# (.NET)
- Follow Microsoft C# coding conventions
- Use Clean Architecture layers
- Apply CQRS pattern for new features
- Write XML documentation for public APIs
- Use `nullable` reference types

### TypeScript (Frontend)
- Use strict TypeScript
- Follow ESLint/Prettier configuration
- Use functional components with hooks
- Apply Tailwind CSS utility classes

## Pull Request Process

1. Create a feature branch from `develop`
2. Make your changes with clear commit messages
3. Add/update tests for your changes
4. Ensure CI passes (build, test, lint)
5. Submit a PR to `develop` branch
6. Request review from at least one maintainer

## Branch Naming

- `feature/` - New features
- `fix/` - Bug fixes
- `refactor/` - Code refactoring
- `docs/` - Documentation updates
- `infra/` - Infrastructure changes

## Commit Messages

Follow conventional commits:
```
feat(payment): add UPI payment support
fix(merchant): resolve KYC validation error
docs(api): update payment API documentation
```

## Security

- Never commit secrets or credentials
- Report security vulnerabilities privately
- Follow PCI DSS guidelines for payment code
- Use parameterized queries (no SQL injection)
- Sanitize all user inputs
