# Google Authenticator - Two-Factor Authentication Web Application

A secure ASP.NET MVC web application that implements Google Authenticator-style two-factor authentication (2FA) for enhanced login security.

## 🔐 Features

- **Two-Factor Authentication**: Implements TOTP (Time-based One-Time Password) using Google Authenticator
- **QR Code Generation**: Automatically generates QR codes for easy setup with authenticator apps
- **Manual Setup**: Provides manual entry keys for authenticator app configuration
- **Session Management**: Secure session handling with Forms Authentication
- **Cookie-based Remember**: Optional "remember device" functionality using secure cookies
- **Bootstrap UI**: Modern, responsive user interface using Bootstrap 4
- **Configurable**: Easy configuration through web.config settings

## 🚀 Quick Start

### Prerequisites

- Visual Studio 2019 or later
- .NET Framework 4.6.1 or later
- IIS Express (included with Visual Studio)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/googleauthenticator.git
   cd googleauthenticator
   ```

2. **Open in Visual Studio**
   - Open `GAuthenticator.sln` in Visual Studio
   - Restore NuGet packages (right-click solution → Restore NuGet Packages)

3. **Configure the application**
   - Open `Web.config`
   - Update the following settings in `<appSettings>`:
   ```xml
   <add key="GAuthPrivateKey" value="YOUR_SECRET_KEY_HERE"/>
   <add key="GAuthEnable" value="1"/>
   ```

4. **Run the application**
   - Press F5 or click "Start Debugging"
   - Navigate to `http://localhost:port/Login/Login`

### Default Login Credentials

- **Username**: `TestUser`
- **Password**: `12345`

> ⚠️ **Important**: Change these credentials in production!

## 📱 How to Use

### First-Time Setup

1. **Login** with your credentials
2. **Scan QR Code** with your authenticator app (Google Authenticator, Authy, etc.)
3. **Enter Verification Code** from your authenticator app
4. **Access Protected Area** - you're now logged in with 2FA enabled

### Supported Authenticator Apps

- Google Authenticator
- Microsoft Authenticator
- Authy
- Any TOTP-compatible authenticator app

## 🛠️ Configuration

### Web.config Settings

```xml
<appSettings>
  <!-- Your secret key for generating unique user keys -->
  <add key="GAuthPrivateKey" value="YOUR_SECRET_KEY_HERE"/>
  
  <!-- Enable/disable 2FA (1 = enabled, 0 = disabled) -->
  <add key="GAuthEnable" value="1"/>
</appSettings>
```

### Security Considerations

1. **Change Default Credentials**: Update the hardcoded username/password in `LoginController.cs`
2. **Use Strong Secret Key**: Generate a strong, random secret key for `GAuthPrivateKey`
3. **Enable HTTPS**: Always use HTTPS in production
4. **Database Integration**: Replace hardcoded credentials with database authentication

## 🏗️ Project Structure

```
GAuthenticator/
├── Controllers/
│   ├── HomeController.cs      # Protected home page controller
│   └── LoginController.cs     # Authentication and 2FA logic
├── Models/
│   └── LoginModel.cs          # Login form model
├── Views/
│   ├── Login/
│   │   ├── Login.cshtml       # Login form and QR code display
│   │   └── UserProfile.cshtml # Protected user profile page
│   └── Shared/                # Shared layouts and views
├── Scripts/                   # JavaScript files (jQuery, Bootstrap)
├── Content/                   # CSS files (Bootstrap, custom styles)
└── Web.config                 # Application configuration
```

## 🔧 Technical Details

### Technologies Used

- **Framework**: ASP.NET MVC 5
- **Target Framework**: .NET Framework 4.6.1
- **Authentication**: Forms Authentication
- **2FA Library**: Google.Authenticator NuGet package
- **UI Framework**: Bootstrap 4
- **JavaScript**: jQuery 3.4.1

### Key Components

1. **TwoFactorAuthenticator**: Core 2FA functionality
2. **QR Code Generation**: Automatic QR code creation for easy setup
3. **Session Management**: Secure session handling
4. **Cookie-based Remember**: Optional device remembering

### Authentication Flow

1. User enters username/password
2. System validates credentials
3. If 2FA is enabled:
   - Generate unique key for user
   - Display QR code for authenticator app setup
   - Validate TOTP code from authenticator app
   - Set authentication cookie if valid
4. Redirect to protected area


## 🚀 Deployment

### IIS Deployment

1. **Build the application** in Release mode
2. **Publish** to a folder
3. **Copy files** to IIS web directory
4. **Configure IIS** application pool for .NET Framework 4.6.1
5. **Update web.config** with production settings

### Azure Deployment

1. Create an Azure App Service
2. Configure for .NET Framework 4.6.1
3. Deploy using Visual Studio or Azure DevOps
4. Update connection strings and app settings

## 🔒 Security Features

- **TOTP Algorithm**: Industry-standard time-based one-time passwords
- **Unique User Keys**: Each user gets a unique secret key
- **Session Security**: Secure session management
- **Forms Authentication**: Built-in ASP.NET authentication
- **HTTPS Ready**: Configured for secure communication

## 📝 Customization

### Adding New Users

Update the authentication logic in `LoginController.cs` to add new user credentials.

### Database Integration

Replace hardcoded credentials with database authentication using Entity Framework or your preferred data access method.

### Custom Styling

Modify CSS files in the `Content/` directory to match your brand.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [Google Authenticator Library](https://www.nuget.org/packages/GoogleAuthenticator/) for TOTP implementation
- [Bootstrap](https://getbootstrap.com/) for UI components
- [jQuery](https://jquery.com/) for JavaScript functionality

