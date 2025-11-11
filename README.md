# BlueTravel - Sistema de Reservas Turísticas

![.NET](https://img.shields.io/badge/.NET-9.0-blue)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Razor%20Pages-green)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-purple)
![Stripe](https://img.shields.io/badge/Stripe-Payments-blue)
![License](https://img.shields.io/badge/License-Academic-green)

Sistema profesional de gestión de reservas turísticas para la región de Guanacaste, Costa Rica. Desarrollado con ASP.NET Core 9.0, Razor Pages y arquitectura moderna.

---

##  Descripción

BlueTravel es una plataforma web completa para la gestión de reservas turísticas en Costa Rica. Permite a los usuarios explorar, reservar y pagar por tours y hospedajes de forma segura, mientras que los administradores pueden gestionar todo el inventario y procesar pagos con Stripe.

** Proyecto Académico - Universidad [Universidad Nacional] 2025**

---

##  Características Principales

###  Gestión de Hospedajes
-  CRUD completo de hoteles, resorts, cabañas y villas
-  Sistema de disponibilidad en tiempo real
-  Cálculo dinámico de precios por noche
-  Cargos por personas extra
-  Restricciones de capacidad y validaciones
-  Configuración de horarios check-in/check-out
-  Filtros de búsqueda avanzados

###  Tours y Excursiones
-  Catálogo completo de tours con imágenes
-  Gestión de cupos y reservas simultáneas
-  Precios diferenciados (adultos/niños/seniors)
-  Descuentos por grupos
-  Niveles de dificultad y restricciones de edad
-  Sistema de confirmación manual para tours especiales
-  Validación automática de disponibilidad

###  Sistema de Reservas Inteligente
-  Creación de reservas con validación en tiempo real
-  Estados de reserva: Pendiente, Confirmada, Cancelada, Completada, Rechazada
-  Validación de disponibilidad de cupos
-  Cálculo automático de precios con descuentos
-  Historial completo para clientes y administradores
-  Cancelación con liberación automática de cupos
-  Panel "Mis Reservas" para clientes

###  Procesamiento de Pagos con Stripe
-  **Integración completa con Stripe** (modo TEST gratuito)
-  Múltiples métodos: Tarjeta de Crédito, SINPE Móvil, PayPal
-  Comprobantes digitales descargables en PDF
-  Sistema de reembolsos para administradores
-  Trazabilidad completa de transacciones
-  Dashboard de pagos con estadísticas
-  Enmascaramiento de datos sensibles (últimos 4 dígitos)
-  Validación de propiedad de pagos

### ?? Gestión de Usuarios y Seguridad
-  Autenticación con ASP.NET Core Identity
-  Login con Google OAuth 2.0
-  Sistema de roles (Admin/Cliente)
-  Registro y login seguros
-  Gestión de permisos granular
-  Protección CSRF con AntiForgeryToken
-  HTTPS forzado en producción

###  Panel Administrativo Completo
-  Dashboard con estadísticas en tiempo real
-  Gráficos de ingresos mensuales (Chart.js)
-  Gestión de hospedajes, tours, ofertas
-  Panel de reservas con filtros
-  Panel de pagos integrado con Stripe
-  Reportes de ocupación y ventas
-  Sistema de notificaciones

###  UX/UI Moderna y Responsive
-  Diseño responsive con Bootstrap 5
-  Animaciones suaves con AOS
-  Gradientes y efectos modernos
-  Optimizado para móviles y tablets
-  Accesibilidad WCAG 2.1
-  Fuente Poppins de Google Fonts

###  Otras Características
-  Catálogo de restaurantes recomendados
-  Sistema de ofertas especiales con validez temporal
-  Opciones de transporte
-  Sistema de caché para optimizar rendimiento
-  Paginación de resultados
-  Seed Data automático con datos de ejemplo

---

##  Tecnologías Utilizadas

### Backend
- **Framework**: ASP.NET Core 9.0 (Razor Pages + MVC)
- **Lenguaje**: C# 13.0
- **ORM**: Entity Framework Core 9.0
- **Base de Datos**: SQL Server (LocalDB/Azure SQL)
- **Autenticación**: ASP.NET Core Identity
- **Logging**: ILogger integrado

### Integraciones Externas
- **Pagos**: Stripe.NET v49.0.0
- **OAuth**: Google Authentication
- **Imágenes**: Unsplash API

### Frontend
- **UI Framework**: Bootstrap 5.3
- **Iconos**: Bootstrap Icons + FontAwesome 6
- **Animaciones**: AOS (Animate On Scroll)
- **Fuentes**: Google Fonts (Poppins)
- **Gráficos**: Chart.js

### Arquitectura y Patrones
- **Patrón Repository**: Abstracción de acceso a datos
- **Inyección de Dependencias**: Microsoft.Extensions.DependencyInjection
- **Servicios de Negocio**: `IPrecioService`, `IStripeService`, `INotificacionService`, `IReporteService`, `ICacheService`, `IDashboardService`
- **Separación de Responsabilidades**: Controllers  Services  Repositories  Data

---

##  Requisitos del Sistema

- **[.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)** (obligatorio)
- **[SQL Server](https://www.microsoft.com/sql-server)** (LocalDB incluido con Visual Studio)
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** (17.8 o superior) o VS Code
- **Cuenta de Stripe** (opcional - modo TEST gratuito para pagos)
- **Cuenta de Google Cloud** (opcional - para OAuth 2.0)

---

##  Instalación y Configuración

### 1 Clonar el Repositorio

```bash
git clone https://github.com/TU_USUARIO/BlueTravel.git
cd BlueTravel
```

### 2 Abrir en Visual Studio

- Abrir `BlueTravel.sln` en Visual Studio 2022
- Esperar a que se restauren los paquetes NuGet automáticamente

### 3 Configurar Base de Datos

La cadena de conexión por defecto usa **LocalDB**:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BlueTravel;Trusted_Connection=True;MultipleActiveResultSets=true"
```

Si usas otro servidor, edita `appsettings.json`.

### 4 Configurar User Secrets (Credenciales Seguras)

** IMPORTANTE:** No subas credenciales a GitHub. Usa User Secrets:

Abre la **Package Manager Console** en Visual Studio y ejecuta:

```powershell
cd BlueTravel

# Inicializar User Secrets
dotnet user-secrets init

# Configurar Stripe (opcional pero recomendado)
dotnet user-secrets set "Stripe:SecretKey" "sk_test_TU_CLAVE_SECRETA"
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_TU_CLAVE_PUBLICA"

# Configurar Google OAuth (opcional)
dotnet user-secrets set "Authentication:Google:ClientId" "TU_GOOGLE_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "TU_GOOGLE_CLIENT_SECRET"

# Verificar configuración
dotnet user-secrets list
```

####  Obtener Credenciales de Stripe

1. Regístrate en [Stripe](https://stripe.com) (gratis)
2. Activa el modo **Test**
3. Ve a **Developers  API keys**
4. Copia las claves de prueba

####  Obtener Credenciales de Google OAuth

1. Ve a [Google Cloud Console](https://console.cloud.google.com)
2. Crea un nuevo proyecto
3. Habilita **Google+ API**
4. Crea credenciales OAuth 2.0
5. Configura URIs autorizadas:
   ```
   https://localhost:5001/signin-google
   ```

### 5 Aplicar Migraciones y Seed Data

En la **Package Manager Console**:

```powershell
Update-Database
```

O usando la CLI de .NET:

```bash
dotnet ef database update --project BlueTravel
```

Esto creará:
-  Todas las tablas necesarias
-  Usuarios de prueba (Admin y Cliente)
-  Datos de ejemplo (hospedajes, tours, ofertas, restaurantes)

### 6 Ejecutar la Aplicación

Presiona **F5** en Visual Studio o ejecuta:

```bash
dotnet run --project BlueTravel
```

 Navega a: `https://localhost:5001`

---

##  Credenciales de Prueba

###  Usuario Administrador
```
Email: admin@bluetravel.com
Password: Admin123!
```

**Panel Admin**: `https://localhost:5001/Admin/Dashboard`

###  Usuario Cliente
Puedes registrarte normalmente en: `/Identity/Account/Register`

O usar Google OAuth si lo configuraste.

###  Tarjetas de Prueba Stripe

| Número | Resultado | CVV | Fecha |
|--------|-----------|-----|-------|
| `4242 4242 4242 4242` |  Pago exitoso | 123 | 12/25 |
| `4000 0000 0000 0002` |  Pago rechazado | 123 | 12/25 |
| `4000 0000 0000 9995` |  Fondos insuficientes | 123 | 12/25 |

**Fecha de expiración**: Cualquier fecha futura  
**CVV**: Cualquier 3 dígitos  
**ZIP**: Cualquier código postal

---

##  Estructura del Proyecto

```
BlueTravel/
    Controllers/              # Controladores MVC
       AdminController.cs    # Dashboard y gestión administrativa
       CatalogoController.cs # Catálogo público
       HospedajesController.cs
       ToursController.cs
       OfertasController.cs
       ReservasController.cs # Sistema de reservas
       PagosController.cs    # Procesamiento de pagos
       HomeController.cs
    Data/                     # Contexto y datos
        ApplicationDbContext.cs
        SeedData.cs          # Datos de ejemplo
       Repositories/        # Patrón Repository
       IRepository.cs
       Repository.cs
       HospedajeRepository.cs
       TourRepository.cs
       ReservaRepository.cs
       OfertaRepository.cs
       Migrations/          # Migraciones EF Core
    Models/                   # Modelos de dominio
    Hospedaje.cs
    Tour.cs
    Reserva.cs
    Pago.cs
    Oferta.cs
    Restaurante.cs
    Transporte.cs
    Resena.cs
    ViewModels/          # ViewModels
    ReservaViewModel.cs
          DashboardViewModel.cs
    Services/                 # Lógica de negocio
       ICacheService.cs
       CacheService.cs
       IDashboardService.cs
       DashboardService.cs
       INotificacionService.cs
       NotificacionService.cs
       IPrecioService.cs
       PrecioService.cs
       IReporteService.cs
       ReporteService.cs
       IStripeService.cs
       StripeService.cs
    Views/                    # Vistas Razor
    Admin/               # Panel administrativo
          Dashboard.cshtml
          Reservas.cshtml
          Pagos.cshtml
       Catalogo/            # Catálogo público
          Hospedajes.cshtml
          Tours.cshtml
          Ofertas.cshtml
          HospedajeDetails.cshtml
          TourDetails.cshtml
       Home/
          Index.cshtml     # Landing page
          Pagos/
          Create.cshtml
          Details.cshtml
          Success.cshtml
          Reservas/
          Create.cshtml
          CreateHospedaje.cshtml
          CreateTour.cshtml
          MisReservas.cshtml
          Details.cshtml
          Hospedajes/          # CRUD Admin
          Tours/               # CRUD Admin
          Ofertas/             # CRUD Admin
          Shared/
          _Layout.cshtml
          _LoginPartial.cshtml
          Error.cshtml
 wwwroot/                  # Archivos estáticos
    css/
    site.css
    js/
    site.js
    lib/                 # Librerías
    Imagenes/
 Program.cs                # Configuración de la app
 appsettings.json          # Configuración (SIN credenciales)
 BlueTravel.csproj
```

---

##  Flujo de Pago Completo

### Proceso Paso a Paso

```
1.  Cliente crea RESERVA
   
2.  Reserva guardada con Estado = "Pendiente"
   
3.  Cliente ve botón "PAGAR AHORA"
   
4.  Completa formulario de pago (método, datos tarjeta)
   
5.  Sistema procesa con Stripe
   
6.  Pago exitoso:
   - Crea registro en tabla Pagos
   - Actualiza Reserva.Estado = "Confirmada"
   - Asocia Pago.ReservaId con Reserva.Id
   - Envía notificación (simulada)
   
7.  Cliente recibe comprobante digital
   
8.  Admin ve pago en Dashboard
```

### Estados de Reserva

| Estado | Descripción |
|--------|-------------|
| **Pendiente** | Reserva creada pero sin pago |
| **Confirmada** | Pago procesado exitosamente |
| **Cancelada** | Cancelada por el cliente o admin |
| **Completada** | Servicio completado |
| **Rechazada** | No cumple requisitos |

### Estados de Pago

| Estado | Descripción |
|--------|-------------|
| **Pendiente** | En proceso |
| **Aprobado** | Pago exitoso |
| **Rechazado** | Pago fallido |
| **Reembolsado** | Dinero devuelto |

---

##  Seguridad Implementada

-  **HTTPS** forzado en producción con HSTS
-  **Protección CSRF** con `[ValidateAntiForgeryToken]`
-  **Validación de entrada** con Data Annotations
-  **Autenticación y Autorización** con ASP.NET Identity
-  **Roles y permisos** con `[Authorize(Roles = "Admin")]`
-  **Enmascaramiento de tarjetas** (solo últimos 4 dígitos guardados)
-  **Logging completo** de transacciones con ILogger
-  **Validación de propiedad** de recursos (usuarios solo ven sus datos)
-  **User Secrets** para credenciales en desarrollo
-  **Azure App Settings** para credenciales en producción
-  **Sanitización de HTML** en vistas con `@Html.Raw()` controlado

---

##  Despliegue en Azure

### Recursos Necesarios

1. **Azure SQL Database** (Basic - $5/mes)
2. **Azure App Service** (Free F1 o Basic B1)
3. **Application Insights** (opcional)

### Pasos Rápidos

1. **Crear recursos en Azure Portal**
2. **Configurar Connection String** en App Service  Configuration
3. **Configurar variables de entorno**:
   ```
   Stripe__SecretKey = [tu_clave]
   Stripe__PublishableKey = [tu_clave]
   Authentication__Google__ClientId = [tu_id]
   Authentication__Google__ClientSecret = [tu_secreto]
   ```
4. **Publicar desde Visual Studio**:
   - Click derecho en proyecto  Publish
   - Target: Azure  Azure App Service (Windows)
   - Seleccionar tu App Service
   - Publish

5. **Ejecutar migraciones en Azure**:
   ```powershell
   Update-Database -ConnectionString "[tu_azure_connection_string]"
   ```

### URL de Producción
```
https://tu-app.azurewebsites.net
```

---

##  Testing

### Flujo de Prueba Completo

#### 1. Exploración Pública
```
 Visitar /Home/Index
 Navegar a /Catalogo/Tours
 Ver detalles de un tour: /Catalogo/Details?tipo=tour&id=1
 Navegar a /Catalogo/Hospedajes
 Ver ofertas: /Catalogo/Ofertas
```

#### 2. Registro y Login
```
 Registrar nuevo usuario: /Identity/Account/Register
 Confirmar email (auto-confirmado en desarrollo)
 Iniciar sesión: /Identity/Account/Login
 Probar login con Google (si configurado)
```

#### 3. Crear Reserva
```
 Seleccionar un tour
 Click en "Reservar Ahora"
 Completar formulario: fechas, cantidad de personas
 Validar cálculo de precio automático
 Confirmar reserva
```

#### 4. Procesar Pago
```
 En "Mis Reservas", click en "Pagar Ahora"
 Seleccionar método de pago: Tarjeta de Crédito
 Ingresar tarjeta de prueba: 4242 4242 4242 4242
 Completar pago
 Verificar comprobante en /Pagos/Details/[id]
```

#### 5. Panel Admin
```
 Cerrar sesión
 Login como admin@bluetravel.com
 Acceder a /Admin/Dashboard
 Verificar estadísticas en tiempo real
 Ver reservas recientes
 Gestionar hospedajes: /Hospedajes
 Gestionar tours: /Tours
 Ver todos los pagos: /Pagos/AdminIndex
```

---

## ?? Características Técnicas Destacadas

### 1. Repository Pattern
Abstracción completa de acceso a datos:

```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### 2. Cache Service
Optimización de consultas frecuentes:

```csharp
var tours = await _cacheService.GetOrCreateAsync(
    "tours_page_1",
    async () => await _tourRepository.GetPaginatedAsync(1, 12),
    TimeSpan.FromMinutes(5)
);
```

### 3. Validación de Disponibilidad
Sistema robusto para evitar sobreventa:

```csharp
var disponible = await _tourRepository.VerificarDisponibilidad(
    tourId, 
    fechaInicio, 
    cantidadPersonas
);
```

### 4. Integración con Stripe
Procesamiento de pagos real:

```csharp
var resultado = await _stripeService.CrearIntencionPago(
    pago.TotalPagado,
    "usd"
);
```

---

##  Roadmap Futuro

- [ ] **Testing Unitario** completo con xUnit
- [ ] **Exportar reportes** a PDF/Excel con EPPlus
- [ ] **Sistema de cupones** de descuento
- [ ] **Notificaciones email** reales con SendGrid
- [ ] **API REST** para app móvil
- [ ] **Sistema de reseñas** con moderación y calificaciones
- [ ] **Multi-idioma** (i18n) Español/Inglés
- [ ] **PWA** (Progressive Web App)
- [ ] **Chat en vivo** con SignalR
- [ ] **Integración con Google Maps** API

---

##  Contribuir

Este es un proyecto académico, pero se aceptan sugerencias:

1. Fork el proyecto
2. Crea tu rama (`git checkout -b feature/NuevaCaracteristica`)
3. Commit tus cambios (`git commit -m 'feat: Agregar nueva característica'`)
4. Push a la rama (`git push origin feature/NuevaCaracteristica`)
5. Abre un Pull Request

### Commits Convencionales

```
feat: Nueva funcionalidad
fix: Corrección de bug
docs: Cambios en documentación
style: Cambios de formato
refactor: Refactorización
test: Agregar tests
chore: Tareas de mantenimiento
```

---

##  Licencia

Proyecto académico para fines educativos.  
**Universidad [Universidad Nacional de Costa Rica]** - 2025

---

## Autor

**[Brayan Pérez Balladares]**
-  Email: bpballadares57@gmail.com@ejemplo.com
-  GitHub: [@tu_usuario](https://github.com/BrayanperezBalladares)
-  LinkedIn: [Tu Perfil](https://linkedin.com/in/brayan-perez-28a4b7248)

**Profesor**: [Gloriana Peña]  
**Curso**: [Programación 3]  
**Semestre**: [2 Ciclo 2025]

---

##  Agradecimientos

- [ASP.NET Core Team](https://dotnet.microsoft.com) - Framework excepcional
- [Bootstrap](https://getbootstrap.com/) - UI Framework
- [Stripe](https://stripe.com/) - Procesamiento de pagos
- [Unsplash](https://unsplash.com/) - Imágenes de alta calidad
- [FontAwesome](https://fontawesome.com/) - Íconos
- [AOS Library](https://michalsnik.github.io/aos/) - Animaciones
- [Chart.js](https://www.chartjs.org/) - Gráficos

---

##  Soporte y Contacto

### Problemas Comunes

**Error: "No se puede conectar a la base de datos"**
- Verifica que SQL Server esté ejecutándose
- Confirma la cadena de conexión en `appsettings.json`

**Error: "Stripe no está configurado"**
- Configura las claves en User Secrets
- Verifica que estás en modo TEST

**Error: "Google OAuth redirect mismatch"**
- Actualiza las URIs autorizadas en Google Cloud Console
- Usa HTTPS en desarrollo

### Reportar Bugs

- Abre un [Issue](https://github.com/TU_USUARIO/BlueTravel/issues)
- Incluye:
  - Descripción del problema
  - Pasos para reproducir
  - Capturas de pantalla
  - Logs de error

---

<div align="center">

** Hecho con Amor en Costa Rica **

[ Volver arriba](#-bluetravel---sistema-de-reservas-turísticas)

---

? **Si este proyecto te fue útil, considera darle una estrella en GitHub!**

</div>
