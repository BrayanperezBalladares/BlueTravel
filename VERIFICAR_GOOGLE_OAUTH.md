# ?? VERIFICAR CONFIGURACIÓN DE GOOGLE OAUTH

## ? **Error Actual:**
```
Error 400: redirect_uri_mismatch
```

Esto significa que la URL que tu app envía a Google NO coincide con la configurada en Google Cloud Console.

---

## ?? **SOLUCIÓN PASO A PASO:**

### **Paso 1: Identifica la URL que usa tu app**

Cuando ejecutas `dotnet run`, mira en la consola:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5035
```

**Tu app corre en:** `http://localhost:5035`

---

### **Paso 2: URLs que Google DEBE tener configuradas**

Ve a: https://console.cloud.google.com/apis/credentials

Edita tu **OAuth 2.0 Client ID** y agrega **EXACTAMENTE** estas 4 URLs:

```
Authorized JavaScript origins:
?????????????????????????????
? http://localhost:5035     ?
? https://localhost:7035    ?
?????????????????????????????

Authorised redirect URIs:
????????????????????????????????????????????????????????
? http://localhost:5035/signin-google                  ?
? http://localhost:5035/Account/ExternalLoginCallback  ?
? https://localhost:7035/signin-google                 ?
? https://localhost:7035/Account/ExternalLoginCallback ?
????????????????????????????????????????????????????????
```

**?? CUIDADO CON:**
- ? Espacios al inicio o final
- ? Barras `/` al final (ej: `/signin-google/`)
- ? Mayúsculas incorrectas (debe ser `ExternalLoginCallback`)
- ? Protocolo incorrecto (`https` vs `http`)

---

### **Paso 3: Guarda y Espera**

1. Click en **"SAVE"** en Google Cloud Console
2. **Espera 2-3 minutos** (Google necesita propagar los cambios)
3. **Borra cookies del navegador** (Ctrl + Shift + Delete)

---

### **Paso 4: Verifica que las URLs coincidan**

Abre la consola de desarrollo de tu navegador (F12):

1. Ve a `http://localhost:5035/Account/Register`
2. Click en "Registrarse con Google"
3. En la pestaña **Network**, busca la request a Google
4. Mira el parámetro `redirect_uri` en la URL

Ejemplo:
```
https://accounts.google.com/o/oauth2/v2/auth?
client_id=507395208165-uglkrm13fof1s4su6jr2qqdmeviu5qsv.apps.googleusercontent.com
&redirect_uri=http%3A%2F%2Flocalhost%3A5035%2FAccount%2FExternalLoginCallback
&response_type=code
&scope=openid%20profile%20email
```

El `redirect_uri` decodificado debe ser:
```
http://localhost:5035/Account/ExternalLoginCallback
```

**Esta URL DEBE estar en Google Cloud Console.**

---

## ?? **SOLUCIONES ALTERNATIVAS:**

### **Solución A: Usar solo `/signin-google`**

Si quieres simplificar, podemos hacer que Google use SOLO `/signin-google` como callback.

Edita `Program.cs`:

```csharp
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
        options.CallbackPath = "/signin-google"; // ? Solo esta ruta
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.SaveTokens = true;
    });
```

Entonces en Google Cloud Console solo necesitas:
```
http://localhost:5035/signin-google
https://localhost:7035/signin-google
```

---

### **Solución B: Usar HTTPS**

Si quieres usar HTTPS (más seguro):

1. **Ejecuta con HTTPS:**
```powershell
dotnet run --urls "https://localhost:7035"
```

2. **Acepta el certificado** de desarrollo cuando el navegador lo pida

3. **Usa solo las URLs con HTTPS** en Google Cloud Console:
```
https://localhost:7035/signin-google
https://localhost:7035/Account/ExternalLoginCallback
```

---

## ? **VERIFICACIÓN FINAL:**

Después de configurar correctamente:

1. ? Google Cloud Console tiene las 4 URLs
2. ? Esperaste 2-3 minutos
3. ? Borraste cookies
4. ? Ejecutaste `dotnet run`
5. ? Click en "Registrarse con Google"
6. ? **Deberías ver la pantalla de selección de cuenta de Google**

---

## ?? **Tabla de Troubleshooting:**

| Error | Causa | Solución |
|-------|-------|----------|
| redirect_uri_mismatch | URL no está en Google Cloud | Agrega la URL exacta |
| redirect_uri_mismatch | Typo en la URL | Verifica mayúsculas/minúsculas |
| redirect_uri_mismatch | Cambios no propagados | Espera 2-3 minutos |
| Error 400 | URL con `/` al final | Remueve la barra final |
| Error 400 | http vs https | Usa el mismo protocolo |

---

## ?? **Checklist:**

```
? Google Cloud Console tiene las 4 URLs exactas
? No hay espacios ni barras / al final
? Protocolo correcto (http o https)
? Guardé los cambios en Google Cloud
? Esperé 2-3 minutos
? Borré cookies del navegador
? Ejecuté dotnet run
? Probé click en "Registrarse con Google"
```

---

¿Necesitas ayuda verificando la configuración de Google Cloud Console? Puedo guiarte paso a paso. ??
