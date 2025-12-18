# Cundi.XAF.ApiKey

A standalone XAF module for generating, managing, and authenticating via API Keys.

## Features

- **Secure Key Generation**: Uses `RandomNumberGenerator` (256-bit) with SHA256 hashing
- **Key Expiration**: Configurable expiration (5 min to 90 days)
- **Admin-Only Access**: Only users with `IsAdministrative` role can manage keys
- **One Key Per User**: Generating a new key automatically revokes the old one
- **Usage Tracking**: Records creation time and last used timestamp

## Installation

Add a reference to `Cundi.XAF.ApiKey` in your XAF module project.

```csharp
// In your module's constructor
RequiredModuleTypes.Add(typeof(Cundi.XAF.ApiKey.ApiKeyModule));
```

## Usage

### Generating API Keys

1. Navigate to any user's **DetailView** (requires `IsAdministrative` role)
2. Click **"Generate API Key"** in the toolbar
3. Select expiration period: 5 min / 30 min / 1 day / 30 days / 60 days / 90 days
4. A popup displays the generated key (copied to clipboard automatically)

> ⚠️ **Important**: The key is shown only once and cannot be retrieved later.

### Revoking API Keys

1. Open the user's DetailView
2. Click **"Revoke API Key"** in the toolbar
3. Confirm the action

### Managing API Keys

API Keys are stored in the `ApiKeyInfo` entity, accessible via **Security** navigation.

| Property | Description |
|----------|-------------|
| `UserOid` | Associated user's Oid |
| `ExpiresAt` | Expiration timestamp |
| `IsActive` | Enable/disable the key |
| `LastUsedAt` | Last authentication timestamp |
| `IsExpired` | Computed: whether the key has expired |
| `IsValid` | Computed: `IsActive && !IsExpired` |

## API Key Format

```
cak_<base64-url-safe-random-bytes>
```

- Prefix: `cak_` (Cundi API Key)
- Body: 32 bytes (256 bits) encoded as URL-safe Base64

## Project Structure

```
Cundi.XAF.ApiKey/
├── BusinessObjects/
│   └── ApiKeyInfo.cs          # API Key entity
├── Controllers/
│   └── ApiKeyViewController.cs # Generate/Revoke actions
├── Parameters/
│   ├── ApiKeyGenerationParameters.cs  # Expiration selection
│   └── ApiKeyResultDisplay.cs         # Key display popup
├── Services/
│   ├── ApiKeyGenerator.cs     # Key generation
│   └── ApiKeyValidator.cs     # Key validation
└── ApiKeyModule.cs
```

## Security

- API Keys are **never stored in plaintext**
- Only SHA256 hash is stored in the database
- Keys are generated using cryptographically secure random number generator

## License

MIT License
