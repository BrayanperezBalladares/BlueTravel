# 🔐 Configurar Google OAuth en BlueTravel

## 📋 **Pasos para Obtener Credenciales de Google**

### 1️⃣ **Ir a Google Cloud Console**
- Visita: https://console.cloud.google.com/
- Inicia sesión con tu cuenta de Google

### 2️⃣ **Crear un Nuevo Proyecto**
1. Click en el dropdown de proyectos (arriba a la izquierda)
2. Click en **"Nuevo Proyecto"**
3. Nombre del proyecto: `BlueTravel`
4. Click en **"Crear"**

### 3️⃣ **Habilitar Google+ API**
1. En el menú lateral, ve a **"APIs y Servicios" > "Biblioteca"**
2. Busca: `Google+ API`
3. Click en **"Habilitar"**

### 4️⃣ **Configurar Pantalla de Consentimiento**
1. En el menú lateral, ve a **"Pantalla de consentimiento de OAuth"**
2. Selecciona **"Externo"**
3. Click en **"Crear"**
4. Llena el formulario:
   - Nombre de la aplicación: `BlueTravel`
   - Correo de soporte: Tu email
   - Logo de la aplicación: (Opcional)
   - Dominio de la aplicación: `localhost:5035`
   - Correo del desarrollador: Tu email
5. Click en **"Guardar y continuar"**
6. En "Alcances", click en **"Guardar y continuar"** (no agregar nada)
7. En "Usuarios de prueba", agrega tu email de prueba
8. Click en **"Guardar y continuar"**

### 5️⃣ **Crear Credenciales OAuth 2.0**
1. En el menú lateral, ve a **"Credenciales"**
2. Click en **"Crear credenciales" > "ID de cliente de OAuth 2.0"**
3. Tipo de aplicación: **"Aplicación web"**
4. Nombre: `BlueTravel Web App`
5. **Orígenes autorizados de JavaScript:**
   ```
   https://localhost:7035
   http://localhost:5035
   ```
6. **URIs de redireccionamiento autorizados:**
   ```
   https://localhost:7035/signin-google
   http://localhost:5035/signin-google
   ```
7. Click en **"Crear"**

### 6️⃣ **Copiar las Credenciales**
Aparecerá un modal con:
- **Tu ID de cliente**: `123456789-abc123def456.apps.googleusercontent.com`
- **Tu secreto de cliente**: `GOCSPX-abc123def456xyz789`

📋 **Copia ambos valores**

---

## ⚙️ **Configurar en BlueTravel**

### 1️⃣ **Editar `appsettings.json`**

Reemplaza en el archivo `BlueTravel/appsettings.json`:

```json
"Authentication": {
  "Google": {
    "ClientId": "PEGA_TU_CLIENT_ID_AQUI",
    "ClientSecret": "PEGA_TU_CLIENT_SECRET_AQUI"
  }
}
```

**Ejemplo real:**
```json
"Authentication": {
  "Google": {
    "ClientId": "123456789-abc123def456.apps.googleusercontent.com",
    "ClientSecret": "GOCSPX-abc123def456xyz789"
  }
}
```

### 2️⃣ **¡Listo!**

Ahora ejecuta el proyecto:

```powershell
dotnet run
```

Ve a: http://localhost:5035/Account/Login

Deberías ver el botón **"Continuar con Google"** funcionando.

---

## 🧪 **Probar Google Login**

1. **Ejecuta el proyecto:**
   ```powershell
   dotnet run
   ```

2. **Intenta hacer una reserva sin login:**
   - Ve a Hospedajes
   - Click en "Ver Detalles"
   - Click en "Reservar Ahora"
   - Serás redirigido a Login con un mensaje contextual

3. **Click en "Continuar con Google":**
   - Se abrirá popup de Google
   - Selecciona tu cuenta de prueba
   - Acepta permisos
   - Serás redirigido automáticamente a la página de reserva

---

## ⚠️ **Troubleshooting**

### Error: "redirect_uri_mismatch"
✅ **Solución:** Verifica que las URLs de redirección en Google Cloud Console coincidan exactamente:
```
http://localhost:5035/signin-google
https://localhost:7035/signin-google
```

### Error: "El proyecto no está verificado"
✅ **Solución:** Es normal en desarrollo. Click en "Avanzado" > "Ir a BlueTravel (no seguro)"

### El botón de Google no aparece
✅ **Solución:** 
1. Verifica que agregaste las credenciales en `appsettings.json`
2. Verifica que instalaste el paquete NuGet:
   ```powershell
   dotnet add package Microsoft.AspNetCore.Authentication.Google
   ```

---

## 🔒 **Seguridad**

### ⚠️ **IMPORTANTE: NO SUBIR CREDENCIALES A GIT**

Crea un archivo `appsettings.Development.json` con tus credenciales:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "TU_CLIENT_ID_REAL",
      "ClientSecret": "TU_CLIENT_SECRET_REAL"
    }
  }
}
```

Agrega a `.gitignore`:
```
appsettings.Development.json
```

---

## 📊 **Resultado Final**

### ✅ **Lo que verás:**

1. **Página de Login moderna** con diseño split-screen
2. **Botón "Continuar con Google"** grande y destacado
3. **Mensaje contextual** cuando intentas reservar sin login
4. **Redirección automática** después de login exitoso
5. **Avatar con nombre** en el navbar después de login con Google

---

## 🎉 **Beneficios UX/UI Implementados:**

✅ **Mejor conversión:** Los usuarios pueden registrarse con 1 click  
✅ **Menos fricción:** No necesitan crear otra contraseña  
✅ **Más confianza:** Logo de Google genera credibilidad  
✅ **Contexto claro:** Saben por qué necesitan login  
✅ **Diseño moderno:** Parece Airbnb/Booking.com  

---

¿Necesitas ayuda configurando? ¡Avísame! 😊
