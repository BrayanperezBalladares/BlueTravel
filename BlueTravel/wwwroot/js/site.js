// ========================================
// BLUETRAVEL - UTILIDADES DE JAVASCRIPT
// Versión 2.0 - UX/UI Profesional
// ========================================

// ==================== TOAST NOTIFICATION SYSTEM ====================

const ToastManager = {
    // Configuración
    config: {
        duration: 5000,
        position: 'top-right',
        maxToasts: 3
    },

    // Contador de toasts
    toastCount: 0,

    // Crear contenedor si no existe
    init() {
        if (!document.querySelector('.toast-container')) {
            const container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }
    },

    // Mostrar toast
    show(message, type = 'info', title = null) {
        this.init();

        // Limitar cantidad de toasts
        const container = document.querySelector('.toast-container');
        const existingToasts = container.querySelectorAll('.toast-notification');
        if (existingToasts.length >= this.config.maxToasts) {
            existingToasts[0].remove();
        }

        // Determinar título automático si no se proporciona
        if (!title) {
            title = {
                'success': '¡Éxito!',
                'error': 'Error',
                'warning': 'Advertencia',
                'info': 'Información'
            }[type] || 'Notificación';
        }

        // Determinar icono
        const icon = {
            'success': 'bi-check-circle-fill',
            'error': 'bi-x-circle-fill',
            'warning': 'bi-exclamation-triangle-fill',
            'info': 'bi-info-circle-fill'
        }[type] || 'bi-info-circle-fill';

        // Crear toast
        const toast = document.createElement('div');
        toast.className = `toast-notification toast-${type}`;
        toast.innerHTML = `
            <div class="toast-icon">
                <i class="bi ${icon}"></i>
            </div>
            <div class="toast-content">
                <div class="toast-title">${title}</div>
                <p class="toast-message">${message}</p>
            </div>
            <button class="toast-close" onclick="this.parentElement.remove()">
                <i class="bi bi-x"></i>
            </button>
            <div class="toast-progress"></div>
        `;

        // Agregar al contenedor
        container.appendChild(toast);

        // Auto-remove después de duración
        setTimeout(() => {
            if (toast.parentElement) {
                toast.remove();
            }
        }, this.config.duration);

        return toast;
    },

    // Métodos de conveniencia
    success(message, title = null) {
        return this.show(message, 'success', title);
    },

    error(message, title = null) {
        return this.show(message, 'error', title);
    },

    warning(message, title = null) {
        return this.show(message, 'warning', title);
    },

    info(message, title = null) {
        return this.show(message, 'info', title);
    }
};

// Alias global para facilidad de uso
window.toast = ToastManager;

// ==================== LOADING OVERLAY ====================

const LoadingOverlay = {
    // Mostrar overlay
    show(message = 'Cargando...') {
        // Remover overlay existente
        this.hide();

        const overlay = document.createElement('div');
        overlay.className = 'loading-overlay';
        overlay.id = 'loadingOverlay';
        overlay.innerHTML = `
            <div class="loading-spinner-wrapper">
                <div class="loading-spinner"></div>
                <div class="loading-text">${message}</div>
            </div>
        `;

        document.body.appendChild(overlay);
        document.body.style.overflow = 'hidden';
    },

    // Ocultar overlay
    hide() {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.remove();
            document.body.style.overflow = '';
        }
    },

    // Mostrar durante una promesa
    async during(promise, message = 'Procesando...') {
        this.show(message);
        try {
            const result = await promise;
            return result;
        } finally {
            this.hide();
        }
    }
};

// Alias global
window.loading = LoadingOverlay;

// ==================== MODAL HELPERS ====================

const ModalHelper = {
    // Confirmar acción
    confirm(options = {}) {
        const defaults = {
            title: '¿Estás seguro?',
            message: 'Esta acción no se puede deshacer',
            confirmText: 'Confirmar',
            cancelText: 'Cancelar',
            type: 'danger',
            onConfirm: () => {},
            onCancel: () => {}
        };

        const config = { ...defaults, ...options };

        // Crear modal
        const modalId = 'confirmModal_' + Date.now();
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.id = modalId;
        modal.setAttribute('tabindex', '-1');
        modal.innerHTML = `
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content modal-confirm">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <i class="bi bi-exclamation-triangle me-2"></i>
                            ${config.title}
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <p class="mb-0">${config.message}</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                            ${config.cancelText}
                        </button>
                        <button type="button" class="btn btn-${config.type}" id="confirmBtn">
                            ${config.confirmText}
                        </button>
                    </div>
                </div>
            </div>
        `;

        document.body.appendChild(modal);

        // Inicializar modal de Bootstrap
        const bsModal = new bootstrap.Modal(modal);

        // Eventos
        modal.querySelector('#confirmBtn').addEventListener('click', () => {
            config.onConfirm();
            bsModal.hide();
        });

        modal.addEventListener('hidden.bs.modal', () => {
            modal.remove();
            config.onCancel();
        });

        bsModal.show();

        return bsModal;
    }
};

// Alias global
window.modal = ModalHelper;

// ==================== FORM VALIDATION HELPERS ====================

const FormValidator = {
    // Validar fecha futura
    validateFutureDate(inputElement) {
        const selectedDate = new Date(inputElement.value);
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        if (selectedDate < today) {
            this.showError(inputElement, 'La fecha debe ser futura');
            return false;
        }

        this.clearError(inputElement);
        return true;
    },

    // Validar rango de fechas
    validateDateRange(startInput, endInput) {
        const startDate = new Date(startInput.value);
        const endDate = new Date(endInput.value);

        if (endDate <= startDate) {
            this.showError(endInput, 'La fecha de fin debe ser posterior a la fecha de inicio');
            return false;
        }

        this.clearError(endInput);
        return true;
    },

    // Mostrar error
    showError(inputElement, message) {
        // Remover error anterior
        this.clearError(inputElement);

        inputElement.classList.add('is-invalid');
        
        const errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback';
        errorDiv.textContent = message;
        
        inputElement.parentElement.appendChild(errorDiv);
    },

    // Limpiar error
    clearError(inputElement) {
        inputElement.classList.remove('is-invalid');
        
        const errorDiv = inputElement.parentElement.querySelector('.invalid-feedback');
        if (errorDiv) {
            errorDiv.remove();
        }
    }
};

// Alias global
window.validator = FormValidator;

// ==================== UTILIDADES GENERALES ====================

// Debounce function
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Formatear moneda
function formatCurrency(amount, locale = 'es-CR', currency = 'CRC') {
    return new Intl.NumberFormat(locale, {
        style: 'currency',
        currency: currency
    }).format(amount);
}

// Formatear fecha
function formatDate(date, locale = 'es-CR') {
    return new Intl.DateTimeFormat(locale, {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    }).format(new Date(date));
}

// Copiar al portapapeles
async function copyToClipboard(text) {
    try {
        await navigator.clipboard.writeText(text);
        toast.success('Copiado al portapapeles');
        return true;
    } catch (err) {
        toast.error('Error al copiar');
        return false;
    }
}

// ==================== INICIALIZACIÓN ====================

document.addEventListener('DOMContentLoaded', function() {
    // Inicializar tooltips de Bootstrap
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Inicializar popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Auto-hide alerts después de 5 segundos
    const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });

    // Manejar TempData messages y mostrar toasts
    const successMessage = document.querySelector('[data-success-message]');
    if (successMessage) {
        toast.success(successMessage.dataset.successMessage);
    }

    const errorMessage = document.querySelector('[data-error-message]');
    if (errorMessage) {
        toast.error(errorMessage.dataset.errorMessage);
    }

    const warningMessage = document.querySelector('[data-warning-message]');
    if (warningMessage) {
        toast.warning(warningMessage.dataset.warningMessage);
    }

    const infoMessage = document.querySelector('[data-info-message]');
    if (infoMessage) {
        toast.info(infoMessage.dataset.infoMessage);
    }

    // Smooth scroll para anchors
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Navbar scroll effect
    const navbar = document.querySelector('.navbar');
    if (navbar) {
        window.addEventListener('scroll', debounce(() => {
            if (window.scrollY > 50) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        }, 10));
    }

    // Lazy loading de imágenes
    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.classList.remove('lazy');
                    imageObserver.unobserve(img);
                }
            });
        });

        document.querySelectorAll('img[data-src]').forEach(img => {
            imageObserver.observe(img);
        });
    }

    console.log('🚀 BlueTravel UX System initialized');
});

// Exponer utilidades globalmente
window.BlueTravel = {
    toast: ToastManager,
    loading: LoadingOverlay,
    modal: ModalHelper,
    validator: FormValidator,
    utils: {
        debounce,
        formatCurrency,
        formatDate,
        copyToClipboard
    }
};
