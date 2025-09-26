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

## 💻 Sample Code

### Login Controller - Two-Factor Authentication Setup

```csharp
[HttpPost]
public ActionResult Login(LoginModel login)
{
    string message = "";
    bool status = false;
    
    // Get private key from configuration
    string GAuthPrivKey = WebConfigurationManager.AppSettings["GAuthPrivateKey"];
    byte[] UserUniqueKey = Encoding.ASCII.GetBytes(login.UserName + GAuthPrivKey);
    string UserUniqueKey1 = (login.UserName + GAuthPrivKey);
    
    // Validate credentials (replace with database lookup)
    if (login.UserName == "prithivi" && login.Password == "12345")
    {
        status = true;
        Session["UserName"] = login.UserName;

        if (WebConfigurationManager.AppSettings["GAuthEnable"].ToString() == "1")
        {
            // Check if user has already set up 2FA
            HttpCookie TwoFCookie = Request.Cookies["TwoFCookie"];
            
            if (TwoFCookie == null || string.IsNullOrEmpty(TwoFCookie.Values["UserCode"]))
            {
                // First time setup - generate QR code
                TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
                Session["UserUniqueKey"] = UserUniqueKey1;
                
                var setupInfo = TwoFacAuth.GenerateSetupCode(
                    "MyApp", 
                    login.UserName, 
                    UserUniqueKey, 
                    qrPixelsPerModule: 3, 
                    generateQrCode: true
                );
                
                ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                ViewBag.SetupCode = setupInfo.ManualEntryKey;
                message = "Two Factor Authentication Verification";
            }
            else
            {
                // User already has 2FA set up
                FormsAuthentication.SetAuthCookie(Session["Username"].ToString(), false);
                return RedirectToAction("UserProfile");
            }
        }
    }
    
    ViewBag.Message = message;
    ViewBag.Status = status;
    return View();
}
```

### TOTP Code Validation

```csharp
public string TwoFactorAuthenticate(string Code)
{
    var token = Code;
    TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
    string UserUniqueKeytwo = Session["UserUniqueKey"].ToString();
    
    byte[] UserUniqueKey = Encoding.ASCII.GetBytes(UserUniqueKeytwo);
    bool isValid = TwoFacAuth.ValidateTwoFactorPIN(UserUniqueKey, token);

    if (isValid)
    {
        // Set remember cookie for 30 days
        HttpCookie TwoFCookie = new HttpCookie("TwoFCookie");
        string UserCode = Encoding.ASCII.GetString(UserUniqueKey);
        TwoFCookie.Values.Add("UserCode", UserCode);
        TwoFCookie.Expires = DateTime.Now.AddDays(30);
        Response.Cookies.Add(TwoFCookie);
        
        Session["IsValidTwoFactorAuthentication"] = true;
        return "Success";
    }
    
    return "Wrong Code";
}
```

### Login Model

```csharp
public class LoginModel
{
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
```

### Frontend JavaScript - AJAX 2FA Validation

```javascript
function CODE() {
    var code = $('#Code').val();
    var url = "/Login/TwoFactorAuthenticate";
    
    $.get(url, { Code: code }, function (Data) {
        if (Data != "Success") {
            alert(Data);
        } else {
            alert("Authentication successful!");
            location.reload();
        }
    });
}
```

### Razor View - Login Form with QR Code

```html
@model GAuthenticator.Models.LoginModel

@if (ViewBag.Status == null || !ViewBag.Status)
{
    <!-- Login Form -->
    <div class="login-form">
        @using (Html.BeginForm())
        {
            <div class="form-group">
                <label for="UserName">Username:</label>
                @Html.TextBoxFor(a => a.UserName, new { @class = "form-control" })
            </div>
            <div class="form-group">
                <label for="Password">Password:</label>
                @Html.TextBoxFor(a => a.Password, new { @class = "form-control", type = "password" })
            </div>
            <input type="submit" value="Login" class="btn btn-primary" />
        }
    </div>
}
else
{
    <!-- 2FA Setup -->
    <div class="two-factor-setup">
        <h3>@ViewBag.Message</h3>
        
        <!-- QR Code for Authenticator App -->
        <div class="qr-code">
            <img src="@ViewBag.BarcodeImageUrl" alt="QR Code" />
        </div>
        
        <!-- Manual Entry Key -->
        <div class="manual-key">
            <p><strong>Manual Setup Code:</strong></p>
            <code>@ViewBag.SetupCode</code>
        </div>
        
        <!-- Verification Code Input -->
        <div class="verification">
            <input type="text" id="Code" name="CodeDigit" placeholder="Enter 6-digit code" />
            <button type="button" id="2fa" class="btn btn-success" onclick="CODE();">
                Verify Code
            </button>
        </div>
    </div>
}
```

### Web.config Configuration

```xml
<configuration>
  <appSettings>
    <!-- Your secret key for generating unique user keys -->
    <add key="GAuthPrivateKey" value="YOUR_SECRET_KEY_HERE"/>
    
    <!-- Enable/disable 2FA (1 = enabled, 0 = disabled) -->
    <add key="GAuthEnable" value="1"/>
  </appSettings>
  
  <system.web>
    <authentication mode="Forms">
      <forms loginUrl="~/Login/Login" timeout="2880"/>
    </authentication>
    
    <compilation debug="true" targetFramework="4.6.1"/>
    <httpRuntime targetFramework="4.6.1"/>
  </system.web>
</configuration>
```

### NuGet Package Installation

```xml
<!-- packages.config -->
<packages>
  <package id="GoogleAuthenticator" version="3.0.0" targetFramework="net461" />
  <package id="Microsoft.AspNet.Mvc" version="5.2.7" targetFramework="net461" />
  <package id="Microsoft.AspNet.WebPages" version="3.2.7" targetFramework="net461" />
  <package id="Microsoft.AspNet.Razor" version="3.2.7" targetFramework="net461" />
  <package id="Newtonsoft.Json" version="12.0.2" targetFramework="net461" />
</packages>
```

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

Update the authentication logic in `LoginController.cs`:

```csharp
if (login.UserName == "newuser" && login.Password == "newpassword")
{
    // Authentication logic here
}
```

### Database Integration

Replace hardcoded credentials with database authentication:

```csharp
// Example with Entity Framework
var user = db.Users.FirstOrDefault(u => u.Username == login.UserName);
if (user != null && VerifyPassword(login.Password, user.PasswordHash))
{
    // Authentication logic here
}
```

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

## 📞 Support

If you have any questions or need help with this project:

- Create an [issue](https://github.com/yourusername/googleauthenticator/issues)
- Check the [documentation](https://github.com/yourusername/googleauthenticator/wiki)
- Contact: [your-email@example.com]

---

**Made with ❤️ for secure authentication**