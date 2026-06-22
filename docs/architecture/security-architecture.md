# Security Architecture

## Overview

AsegyaPay implements defense-in-depth security with PCI DSS Level 1 compliance readiness.

## Security Layers

### 1. Network Security
- **WAF**: Web Application Firewall at CDN edge
- **DDoS Protection**: Layer 3/4/7 protection
- **Network Policies**: Kubernetes network policies (deny-all default)
- **mTLS**: Service-to-service mutual TLS via service mesh
- **VPN**: Private network for internal services

### 2. Application Security
- **Authentication**: OAuth 2.1 with PKCE
- **Authorization**: RBAC + ABAC policies
- **Input Validation**: Request validation at every layer
- **Rate Limiting**: Per-API, per-merchant, per-IP limits
- **CORS**: Strict origin whitelisting
- **CSP**: Content Security Policy headers
- **CSRF**: Anti-CSRF tokens for web forms

### 3. Data Security
- **Encryption at Rest**: AES-256-GCM
- **Encryption in Transit**: TLS 1.3
- **Tokenization**: Card data tokenized via HSM
- **Key Management**: HSM-backed key rotation
- **Data Classification**: Automatic PII detection
- **Data Masking**: Sensitive fields masked in logs

### 4. Identity & Access
- **MFA**: TOTP, WebAuthn, SMS
- **SSO**: SAML 2.0 / OpenID Connect
- **API Keys**: Merchant-scoped, environment-specific
- **JWT**: Short-lived access tokens (15 min)
- **Refresh Tokens**: Rotating refresh tokens

### 5. Monitoring & Response
- **SIEM**: Security event aggregation
- **Anomaly Detection**: ML-based behavioral analysis
- **Audit Trail**: Immutable audit logs
- **Incident Response**: Automated playbooks
- **Vulnerability Scanning**: Continuous SAST/DAST

## PCI DSS Compliance

### Cardholder Data Environment (CDE)
- Isolated network segment for card processing
- HSM for key storage and cryptographic operations
- Tokenization to minimize CDE scope
- No card data stored on disk (only tokenized references)

### Key Controls
1. Install and maintain network security controls
2. Apply secure configurations
3. Protect stored account data (tokenization)
4. Encrypt transmission of cardholder data (TLS 1.3)
5. Protect against malware
6. Develop secure systems
7. Restrict access by business need-to-know
8. Identify users and authenticate access
9. Restrict physical access (cloud provider)
10. Log and monitor all access
11. Test security regularly
12. Maintain information security policy

## Encryption Standards

| Data Type | At Rest | In Transit | Key Management |
|-----------|---------|------------|----------------|
| Card Numbers | AES-256 + HSM | TLS 1.3 | HSM DUKPT |
| PII | AES-256-GCM | TLS 1.3 | Vault KMS |
| API Keys | Argon2id hash | TLS 1.3 | Auto-rotation |
| Passwords | Argon2id | TLS 1.3 | N/A |
| Session Data | AES-256 | TLS 1.3 | Auto-rotation |
| Logs | AES-256 | TLS 1.3 | Vault KMS |

## Threat Model

### High Priority Threats
1. **Card data breach** → Tokenization + HSM + minimal CDE
2. **Account takeover** → MFA + anomaly detection + device fingerprinting
3. **API abuse** → Rate limiting + WAF + bot detection
4. **Insider threat** → RBAC + audit logs + separation of duties
5. **Supply chain** → SCA scanning + container signing + SBOM

### Fraud Prevention
- Real-time ML scoring for every transaction
- Velocity checks (amount, frequency, geography)
- Device fingerprinting + behavioral biometrics
- IP reputation + geolocation validation
- 3D Secure 2.0 for card payments
- Rule engine for custom merchant rules
