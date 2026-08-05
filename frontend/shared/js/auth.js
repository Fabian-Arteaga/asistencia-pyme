const TOKEN_KEY = "asistenciaPyme_token";
const ADMIN_KEY = "asistenciaPyme_administrador";
const EXPIRACION_KEY = "asistenciaPyme_expiracion";

export function obtenerToken() {
    return sessionStorage.getItem(TOKEN_KEY);
}

export function obtenerAdministrador() {
    const administradorGuardado =
        sessionStorage.getItem(ADMIN_KEY);

    if (!administradorGuardado) {
        return null;
    }

    try {
        return JSON.parse(administradorGuardado);
    } catch {
        return null;
    }
}

export function sesionValida() {
    const token = obtenerToken();

    if (!token) {
        return false;
    }

    const expiracion =
        sessionStorage.getItem(EXPIRACION_KEY);

    if (!expiracion) {
        return true;
    }

    const fechaExpiracion = new Date(expiracion);

    if (Number.isNaN(fechaExpiracion.getTime())) {
        return true;
    }

    return fechaExpiracion.getTime() > Date.now();
}

export function protegerPagina() {
    if (sesionValida()) {
        return true;
    }

    limpiarSesion();

    window.location.replace(
        "../login/login.html"
    );

    return false;
}

export function cerrarSesion() {
    limpiarSesion();

    window.location.replace(
        "../../index.html"
    );
}

export function limpiarSesion() {
    sessionStorage.removeItem(TOKEN_KEY);
    sessionStorage.removeItem(ADMIN_KEY);
    sessionStorage.removeItem(EXPIRACION_KEY);
}