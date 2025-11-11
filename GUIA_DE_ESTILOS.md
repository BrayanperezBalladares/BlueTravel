# 🎨 Guía de Estilos - BlueTravel

## 📋 **Índice**
1. [Frameworks y Librerías](#frameworks-y-librerías)
2. [Sistema de Diseño](#sistema-de-diseño)
3. [Paleta de Colores](#paleta-de-colores)
4. [Tipografía](#tipografía)
5. [Componentes](#componentes)
6. [Espaciado y Grid](#espaciado-y-grid)
7. [Animaciones](#animaciones)
8. [Iconografía](#iconografía)
9. [Patrones de Diseño](#patrones-de-diseño)
10. [Referencias y Inspiración](#referencias-y-inspiración)

---

## 🛠️ **Frameworks y Librerías**

### **CSS Frameworks:**
```html
✅ Bootstrap 5.3.2
   - Grid System (12 columnas)
   - Utilities classes
   - Components (cards, buttons, forms)
   
✅ Bootstrap Icons 1.11.1
   - Sistema de iconos oficial
   - https://icons.getbootstrap.com/
```

### **JavaScript Libraries:**
```html
✅ AOS (Animate On Scroll) 2.3.1
   - Animaciones al hacer scroll
   - https://michalsnik.github.io/aos/
   
✅ jQuery 3.7.1
   - Para validaciones de formularios
   - Bootstrap dependencies
```

### **Fuentes:**
```html
✅ Google Fonts: Poppins
   - Weights: 300, 400, 500, 600, 700, 800
   - https://fonts.google.com/specimen/Poppins
```

---

## 🎨 **Sistema de Diseño**

### **Filosofía de Diseño:**
El diseño de BlueTravel está inspirado en plataformas modernas de viajes como:
- **Airbnb** → Cards, imagery, spacing
- **Booking.com** → Filtros, disponibilidad, urgencia
- **Stripe** → Login/Register split-screen
- **TripAdvisor** → Ratings, reviews, itinerarios

### **Principios:**
1. ✅ **Minimalismo**: Espacios en blanco generosos
2. ✅ **Jerarquía Visual**: Títulos grandes, texto secundario suave
3. ✅ **Feedback Inmediato**: Hover states, animaciones suaves
4. ✅ **Mobile First**: Responsive desde 320px
5. ✅ **Accesibilidad**: Contraste AAA, labels, aria-labels

---

## 🎨 **Paleta de Colores**

### **Colores Primarios:**

```css
/* Primary (Azul/Morado) - Login, Navbar, CTAs */
--primary: #667eea;
--primary-dark: #764ba2;
--gradient-primary: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

/* Success (Verde) - Register, Confirmaciones */
--success: #00bfa5;
--success-dark: #00897b;
--gradient-success: linear-gradient(135deg, #00bfa5 0%, #00897b 100%);

/* Info (Turquesa) - Información, Alertas */
--info: #3b82f6;
--info-light: #60a5fa;

/* Warning (Amarillo) - Alertas, Descuentos */
--warning: #f59e0b;
--warning-light: #fbbf24;

/* Danger (Rojo) - Errores, Eliminación */
--danger: #ef4444;
--danger-light: #f87171;
```

### **Colores Neutros:**

```css
/* Backgrounds */
--bg-primary: #ffffff;
--bg-secondary: #f5f7fa;
--bg-tertiary: #e9ecef;

/* Text */
--text-primary: #1f2937;
--text-secondary: #6b7280;
--text-muted: #9ca3af;

/* Borders */
--border-light: #e0e0e0;
--border-medium: #d1d5db;
```

### **Gradientes Utilizados:**

```css
/* Login Page */
background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);

/* Register Page */
background: linear-gradient(135deg, #e0f7fa 0%, #b2ebf2 100%);

/* Hero Sections */
background: linear-gradient(135deg, rgba(102, 126, 234, 0.9) 0%, rgba(118, 75, 162, 0.9) 100%);

/* Cards (Overlay) */
background: linear-gradient(180deg, transparent 0%, rgba(0,0,0,0.7) 100%);
```

---

## 📝 **Tipografía**

### **Font Family:**
```css
font-family: 'Poppins', -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
```

### **Escalas de Tamaño:**

```css
/* Headings */
.display-1 { font-size: 6rem; }
.display-2 { font-size: 5.5rem; }
.display-3 { font-size: 4.5rem; }
.display-4 { font-size: 3.5rem; } /* Hero Titles */
.display-5 { font-size: 3rem; }

h1 { font-size: 2.5rem; font-weight: 700; }
h2 { font-size: 2rem; font-weight: 600; }
h3 { font-size: 1.75rem; font-weight: 600; }
h4 { font-size: 1.5rem; font-weight: 600; }
h5 { font-size: 1.25rem; font-weight: 500; }
h6 { font-size: 1rem; font-weight: 500; }

/* Body Text */
.lead { font-size: 1.25rem; font-weight: 300; }
body { font-size: 1rem; font-weight: 400; }
small { font-size: 0.875rem; }
.text-xs { font-size: 0.75rem; }
```

### **Pesos (Weights):**
```css
--font-light: 300;
--font-regular: 400;
--font-medium: 500;
--font-semibold: 600;
--font-bold: 700;
--font-extrabold: 800;
```

---

## 🧩 **Componentes**

### **1. Botones**

#### **Primario:**
```html
<button class="btn btn-primary rounded-pill px-4 py-2">
    <i class="bi bi-check-circle me-2"></i>Botón Primario
</button>
```

**Estilos:**
```css
.btn-primary {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    border: none;
    font-weight: 600;
    transition: all 0.3s ease;
}

.btn-primary:hover {
    transform: translateY(-2px);
    box-shadow: 0 10px 25px rgba(102, 126, 234, 0.3);
}
```

#### **Google OAuth:**
```html
<button class="btn btn-google w-100 py-3 rounded-pill">
    <i class="bi bi-google me-2"></i>Continuar con Google
</button>
```

**Estilos:**
```css
.btn-google {
    background: white;
    border: 2px solid #e0e0e0;
    color: #333;
    font-weight: 600;
}

.btn-google:hover {
    background: #4285f4;
    color: white;
    border-color: #4285f4;
    transform: translateY(-2px);
    box-shadow: 0 10px 25px rgba(66, 133, 244, 0.3);
}
```

### **2. Cards**

#### **Card Básica:**
```html
<div class="card border-0 shadow-sm">
    <div class="card-body p-4">
        <h3 class="fw-bold mb-3">Título</h3>
        <p class="text-muted">Contenido</p>
    </div>
</div>
```

**Estilos:**
```css
.card {
    border-radius: 1rem;
    transition: all 0.3s ease;
}

.card:hover {
    transform: translateY(-5px);
    box-shadow: 0 20px 40px rgba(0,0,0,0.1);
}
```

#### **Card de Tour/Hospedaje:**
```html
<div class="tour-card">
    <div class="tour-image-wrapper">
        <img src="..." class="tour-image">
        <span class="badge bg-danger position-absolute top-0 end-0 m-3">
            -20%
        </span>
    </div>
    <div class="tour-content p-3">
        <h5 class="fw-bold">Nombre del Tour</h5>
        <div class="rating mb-2">
            <i class="bi bi-star-fill text-warning"></i> 4.9 (120)
        </div>
        <div class="d-flex justify-content-between">
            <span class="text-muted">Desde</span>
            <span class="fw-bold text-primary">$150</span>
        </div>
    </div>
</div>
```

### **3. Formularios**

#### **Input con Icono:**
```html
<div class="mb-3">
    <label class="form-label fw-semibold">
        <i class="bi bi-envelope me-2"></i>Email
    </label>
    <input type="email" 
           class="form-control form-control-lg rounded-pill" 
           placeholder="ejemplo@correo.com">
</div>
```

**Estilos:**
```css
.form-control-lg {
    padding: 0.75rem 1.5rem;
    border-radius: 50rem;
}

.form-control:focus {
    border-color: #667eea;
    box-shadow: 0 0 0 0.2rem rgba(102, 126, 234, 0.15);
}
```

#### **Password Toggle:**
```html
<div class="input-group">
    <input type="password" 
           class="form-control form-control-lg rounded-pill" 
           id="passwordInput">
    <button class="btn btn-link position-absolute end-0 top-50 translate-middle-y z-3" 
            type="button" 
            onclick="togglePassword()">
        <i class="bi bi-eye" id="toggleIcon"></i>
    </button>
</div>
```

### **4. Badges**

```html
<!-- Success -->
<span class="badge bg-success">Disponible</span>

<!-- Warning -->
<span class="badge bg-warning text-dark">Pocos cupos</span>

<!-- Danger -->
<span class="badge bg-danger">Agotado</span>

<!-- Primary -->
<span class="badge bg-primary">Nuevo</span>

<!-- Gradient -->
<span class="badge bg-gradient-primary">Premium</span>
```

### **5. Alerts**

```html
<!-- Success -->
<div class="alert alert-success border-0 shadow-sm">
    <div class="d-flex align-items-center gap-3">
        <i class="bi bi-check-circle-fill fs-3"></i>
        <div>
            <strong>¡Éxito!</strong>
            <p class="mb-0">Operación completada</p>
        </div>
    </div>
</div>

<!-- Info -->
<div class="alert alert-info border-0 shadow-sm">
    <i class="bi bi-info-circle me-2"></i>
    Información importante
</div>

<!-- Danger -->
<div class="alert alert-danger border-0 shadow-sm">
    <i class="bi bi-exclamation-triangle me-2"></i>
    Error: Verifica los datos
</div>
```

---

## 📐 **Espaciado y Grid**

### **Sistema de Espaciado Bootstrap:**
```css
/* Margins y Paddings */
.m-0 { margin: 0; }
.m-1 { margin: 0.25rem; }  /* 4px */
.m-2 { margin: 0.5rem; }   /* 8px */
.m-3 { margin: 1rem; }     /* 16px */
.m-4 { margin: 1.5rem; }   /* 24px */
.m-5 { margin: 3rem; }     /* 48px */

/* Gaps */
.gap-1 { gap: 0.25rem; }
.gap-2 { gap: 0.5rem; }
.gap-3 { gap: 1rem; }
.gap-4 { gap: 1.5rem; }
.gap-5 { gap: 3rem; }
```

### **Breakpoints:**
```css
/* Extra small (xs) */
@media (max-width: 575.98px) { }

/* Small (sm) */
@media (min-width: 576px) { }

/* Medium (md) */
@media (min-width: 768px) { }

/* Large (lg) */
@media (min-width: 992px) { }

/* Extra large (xl) */
@media (min-width: 1200px) { }

/* Extra extra large (xxl) */
@media (min-width: 1400px) { }
```

### **Layout Común:**
```html
<!-- Container fluido -->
<div class="container-fluid">

<!-- Container normal (max-width) -->
<div class="container">

<!-- Grid de 2 columnas (Desktop) / 1 columna (Mobile) -->
<div class="row">
    <div class="col-lg-6">Columna 1</div>
    <div class="col-lg-6">Columna 2</div>
</div>

<!-- Grid de 3 columnas -->
<div class="row g-4">
    <div class="col-lg-4">Card 1</div>
    <div class="col-lg-4">Card 2</div>
    <div class="col-lg-4">Card 3</div>
</div>
```

---

## ✨ **Animaciones**

### **AOS (Animate On Scroll):**

```html
<!-- Fade Up -->
<div data-aos="fade-up">Contenido</div>

<!-- Fade Down -->
<div data-aos="fade-down">Contenido</div>

<!-- Fade Left -->
<div data-aos="fade-left">Contenido</div>

<!-- Fade Right -->
<div data-aos="fade-right">Contenido</div>

<!-- Con delay -->
<div data-aos="fade-up" data-aos-delay="100">Item 1</div>
<div data-aos="fade-up" data-aos-delay="200">Item 2</div>
<div data-aos="fade-up" data-aos-delay="300">Item 3</div>
```

**Configuración:**
```javascript
AOS.init({ 
    duration: 600,  // Duración en ms
    once: true,     // Animar solo una vez
    offset: 100     // Offset desde el viewport
});
```

### **Transitions CSS:**

```css
/* Duración estándar */
--transition-fast: 150ms;
--transition-base: 300ms;
--transition-slow: 500ms;

/* Uso */
.card {
    transition: all var(--transition-base) ease;
}

.btn {
    transition: all var(--transition-fast) ease-in-out;
}

/* Hover Effects */
.card:hover {
    transform: translateY(-5px);
    box-shadow: 0 20px 40px rgba(0,0,0,0.1);
}

.btn:hover {
    transform: translateY(-2px);
    box-shadow: 0 10px 25px rgba(102, 126, 234, 0.3);
}
```

---

## 🎯 **Iconografía**

### **Bootstrap Icons:**

```html
<!-- Navegación -->
<i class="bi bi-house-door"></i>        Inicio
<i class="bi bi-building"></i>          Hospedajes
<i class="bi bi-compass"></i>           Tours
<i class="bi bi-cup-hot"></i>           Restaurantes
<i class="bi bi-percent"></i>           Ofertas

<!-- Usuario -->
<i class="bi bi-person-fill"></i>       Usuario
<i class="bi bi-person-plus"></i>       Registro
<i class="bi bi-box-arrow-in-right"></i> Login
<i class="bi bi-box-arrow-right"></i>   Logout
<i class="bi bi-gear"></i>              Configuración

<!-- Acciones -->
<i class="bi bi-check-circle-fill"></i> Éxito
<i class="bi bi-x-circle-fill"></i>     Error
<i class="bi bi-info-circle-fill"></i>  Info
<i class="bi bi-exclamation-triangle-fill"></i> Advertencia

<!-- Redes Sociales -->
<i class="bi bi-google"></i>            Google
<i class="bi bi-facebook"></i>          Facebook
<i class="bi bi-instagram"></i>         Instagram
<i class="bi bi-twitter"></i>           Twitter

<!-- Otros -->
<i class="bi bi-star-fill"></i>         Rating
<i class="bi bi-heart-fill"></i>        Favorito
<i class="bi bi-calendar-event"></i>    Fecha
<i class="bi bi-geo-alt-fill"></i>      Ubicación
<i class="bi bi-clock-fill"></i>        Tiempo
<i class="bi bi-people-fill"></i>       Personas
```

**Tamaños:**
```html
<i class="bi bi-star-fill fs-1"></i>  <!-- Extra grande -->
<i class="bi bi-star-fill fs-2"></i>
<i class="bi bi-star-fill fs-3"></i>
<i class="bi bi-star-fill fs-4"></i>
<i class="bi bi-star-fill fs-5"></i>
<i class="bi bi-star-fill fs-6"></i>  <!-- Pequeño -->
```

---

## 🎨 **Patrones de Diseño**

### **1. Split-Screen (Login/Register):**

```html
<div class="row g-0">
    <!-- Izquierda: Imagen/Branding -->
    <div class="col-lg-6 d-none d-lg-block">
        <div class="image-side">
            <img src="..." class="w-100 h-100 object-cover">
            <div class="overlay"></div>
            <div class="content">
                <!-- Logo, Título, Descripción -->
            </div>
        </div>
    </div>
    
    <!-- Derecha: Formulario -->
    <div class="col-lg-6">
        <div class="form-side p-5">
            <!-- Formulario -->
        </div>
    </div>
</div>
```

### **2. Hero con Background Image:**

```html
<section class="hero position-relative">
    <img src="..." class="hero-bg">
    <div class="hero-overlay"></div>
    <div class="hero-content">
        <h1 class="display-4 fw-bold text-white">Título</h1>
        <p class="lead text-white-50">Descripción</p>
        <button class="btn btn-primary btn-lg">CTA</button>
    </div>
</section>
```

### **3. Card Grid:**

```html
<div class="row g-4">
    <div class="col-lg-4 col-md-6" data-aos="fade-up">
        <div class="card">...</div>
    </div>
    <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="100">
        <div class="card">...</div>
    </div>
    <div class="col-lg-4 col-md-6" data-aos="fade-up" data-aos-delay="200">
        <div class="card">...</div>
    </div>
</div>
```

### **4. Timeline (Itinerario):**

```css
.timeline {
    position: relative;
    padding-left: 2rem;
}

.timeline::before {
    content: '';
    position: absolute;
    left: 15px;
    top: 0;
    bottom: 0;
    width: 2px;
    background: linear-gradient(to bottom, #667eea, #764ba2);
}

.timeline-item {
    position: relative;
    padding-bottom: 2rem;
}

.timeline-marker {
    position: absolute;
    left: -2rem;
    width: 30px;
    height: 30px;
    border-radius: 50%;
    background: #667eea;
    border: 3px solid white;
}
```

---

## 📚 **Referencias y Inspiración**

### **Design Systems:**
- **Material Design 3** → Cards, elevations
- **Tailwind CSS** → Utility classes, spacing
- **Ant Design** → Forms, validations
- **Chakra UI** → Color schemes

### **Sitios Inspiración:**
```
✅ Airbnb         → https://www.airbnb.com/
✅ Booking.com    → https://www.booking.com/
✅ Stripe         → https://stripe.com/
✅ Linear         → https://linear.app/
✅ Vercel         → https://vercel.com/
```

### **Recursos Utilizados:**
```
✅ Unsplash       → Imágenes (https://unsplash.com/)
✅ Bootstrap Icons → Iconos (https://icons.getbootstrap.com/)
✅ Google Fonts   → Tipografía (https://fonts.google.com/)
✅ AOS Library    → Animaciones (https://michalsnik.github.io/aos/)
✅ Coolors        → Paletas de color (https://coolors.co/)
```

---

## ✅ **Checklist para Nuevos Componentes**

Antes de agregar un nuevo componente, verifica:

```
□ Usa Poppins como fuente principal
□ Respeta la paleta de colores (primario, success, etc.)
□ Agrega border-radius (0.5rem mínimo)
□ Incluye sombras sutiles (shadow-sm, shadow-md)
□ Agrega hover states con transform y box-shadow
□ Usa iconos de Bootstrap Icons
□ Agrega animaciones AOS donde corresponda
□ Es responsive (mobile-first)
□ Tiene contraste AAA (accesibilidad)
□ Usa spacing consistente (múltiplos de 4px: 0.25rem, 0.5rem, 1rem)
```

---

## 🎯 **Ejemplos de Código Reutilizable**

### **Card con Hover:**
```html
<div class="card border-0 shadow-sm hover-lift">
    <div class="card-body p-4">
        <h5 class="fw-bold">Título</h5>
        <p class="text-muted">Descripción</p>
    </div>
</div>

<style>
.hover-lift {
    transition: all 0.3s ease;
}
.hover-lift:hover {
    transform: translateY(-5px);
    box-shadow: 0 20px 40px rgba(0,0,0,0.1);
}
</style>
```

### **Botón con Gradiente:**
```html
<button class="btn btn-gradient">
    <i class="bi bi-star me-2"></i>
    Acción
</button>

<style>
.btn-gradient {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    border: none;
    color: white;
    font-weight: 600;
    padding: 0.75rem 1.5rem;
    border-radius: 50rem;
    transition: all 0.3s ease;
}
.btn-gradient:hover {
    transform: translateY(-2px);
    box-shadow: 0 10px 25px rgba(102, 126, 234, 0.3);
}
</style>
```

### **Badge con Gradiente:**
```html
<span class="badge-gradient">Premium</span>

<style>
.badge-gradient {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    padding: 0.5rem 1rem;
    border-radius: 50rem;
    font-weight: 600;
    display: inline-block;
}
</style>
```

---

## 📝 **Notas Finales**

### **Variables CSS Personalizadas:**

Si quieres usar variables CSS globales, agrega al inicio de `site.css`:

```css
:root {
    /* Colores */
    --color-primary: #667eea;
    --color-success: #00bfa5;
    --color-danger: #ef4444;
    
    /* Spacing */
    --space-xs: 0.25rem;
    --space-sm: 0.5rem;
    --space-md: 1rem;
    --space-lg: 1.5rem;
    --space-xl: 3rem;
    
    /* Border Radius */
    --radius-sm: 0.5rem;
    --radius-md: 1rem;
    --radius-lg: 1.5rem;
    --radius-full: 50rem;
    
    /* Shadows */
    --shadow-sm: 0 1px 2px rgba(0,0,0,0.05);
    --shadow-md: 0 4px 6px rgba(0,0,0,0.1);
    --shadow-lg: 0 10px 15px rgba(0,0,0,0.1);
    --shadow-xl: 0 20px 25px rgba(0,0,0,0.15);
    --shadow-2xl: 0 25px 50px rgba(0,0,0,0.25);
    
    /* Transitions */
    --transition-fast: 150ms ease;
    --transition-base: 300ms ease;
    --transition-slow: 500ms ease;
}
```

---

