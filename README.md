# Music & Mind 2.0
Bilingual BG/EN, Endel-like minimal design. Pages: Home, Focus, Science, About, Contact.
Language switcher writes a cookie. Audio players on Focus page.


## Configuration (secrets)

This project avoids committing credentials in source control.

### Admin seed user
Set the admin credentials via *User Secrets* (recommended for local dev) or environment variables:

```bash
dotnet user-secrets set "AdminSeed:Email" "admin@example.com"
dotnet user-secrets set "AdminSeed:Password" "YourStrongPassword123!"
```

### SMTP (order emails)
```bash
dotnet user-secrets set "SMTP:User" "your.email@gmail.com"
dotnet user-secrets set "SMTP:Password" "your-app-password"
```

If SMTP credentials are not configured, checkout will still complete, but the success page will show an email error message.

