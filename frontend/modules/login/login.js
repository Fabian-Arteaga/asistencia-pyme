import {
    iniciarSesion
} from "./login.service.js";

const TOKEN_KEY =
    "asistenciaPyme_token";

const ADMIN_KEY =
    "asistenciaPyme_administrador";

const EXPIRACION_KEY =
    "asistenciaPyme_expiracion";

const formLogin =
    document.getElementById("formLogin");

const inputCorreo =
    document.getElementById("correo");

const inputContrasena =
    document.getElementById("contrasena");

const botonIniciarSesion =
    document.getElementById("btnIniciarSesion");

const mensajeLogin =
    document.getElementById("mensajeLogin");

document.addEventListener(
    "DOMContentLoaded",
    inicializarLogin
);

formLogin.addEventListener(
    "submit",
    procesarLogin
);

function inicializarLogin() {
    const token =
        sessionStorage.getItem(TOKEN_KEY);

    if (token) {
        window.location.href =
            "../dashboard/dashboard.html";

        return;
    }

    formLogin.reset();
    inputCorreo.focus();
}

async function procesarLogin(event) {
    event.preventDefault();

    limpiarMensaje();

    const correo =
        inputCorreo.value
            .trim()
            .toLowerCase();

    const contrasena =
        inputContrasena.value;

    if (!correo) {
        mostrarError(
            "Debe ingresar su correo electrónico."
        );

        inputCorreo.focus();
        return;
    }

    if (!esCorreoValido(correo)) {
        mostrarError(
            "Ingrese un correo electrónico válido."
        );

        inputCorreo.focus();
        return;
    }

    if (!contrasena) {
        mostrarError(
            "Debe ingresar su contraseña."
        );

        inputContrasena.focus();
        return;
    }

    bloquearFormulario(true);

    try {
        const respuesta = await iniciarSesion(
            correo,
            contrasena
        );

        const token =
            respuesta?.token ??
            respuesta?.Token;

        if (!token) {
            throw new Error(
                "La respuesta del servidor no contiene un token válido."
            );
        }

        const administrador = {
            idAdministrador:
                respuesta?.idAdministrador ??
                respuesta?.IdAdministrador ??
                null,

            nombreCompleto:
                respuesta?.nombreCompleto ??
                respuesta?.NombreCompleto ??
                "Administrador",

            correo:
                respuesta?.correo ??
                respuesta?.Correo ??
                correo,

            rol:
                respuesta?.rol ??
                respuesta?.Rol ??
                "Administrador"
        };

        const expiracion =
            respuesta?.expiracionUtc ??
            respuesta?.ExpiracionUtc ??
            null;

        sessionStorage.setItem(
            TOKEN_KEY,
            token
        );

        sessionStorage.setItem(
            ADMIN_KEY,
            JSON.stringify(administrador)
        );

        if (expiracion) {
            sessionStorage.setItem(
                EXPIRACION_KEY,
                expiracion
            );
        }

        mostrarExito(
            "Inicio de sesión correcto. Redirigiendo..."
        );

        setTimeout(() => {
            window.location.href =
                "../dashboard/dashboard.html";
        }, 700);
    } catch (error) {
        console.error(error);

        sessionStorage.removeItem(TOKEN_KEY);
        sessionStorage.removeItem(ADMIN_KEY);
        sessionStorage.removeItem(EXPIRACION_KEY);

        const mensaje =
            error instanceof TypeError
                ? "No fue posible conectarse con el servidor."
                : error.message ||
                  "No fue posible iniciar sesión.";

        mostrarError(mensaje);

        inputContrasena.value = "";
        inputContrasena.focus();
    } finally {
        bloquearFormulario(false);
    }
}

function esCorreoValido(correo) {
    const expresion =
        /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    return expresion.test(correo);
}

function bloquearFormulario(bloqueado) {
    inputCorreo.disabled = bloqueado;
    inputContrasena.disabled = bloqueado;
    botonIniciarSesion.disabled = bloqueado;

    botonIniciarSesion.textContent =
        bloqueado
            ? "Iniciando sesión..."
            : "Iniciar sesión";
}

function mostrarError(mensaje) {
    mensajeLogin.textContent = mensaje;

    mensajeLogin.className =
        "mensaje mensaje--error";
}

function mostrarExito(mensaje) {
    mensajeLogin.textContent = mensaje;

    mensajeLogin.className =
        "mensaje mensaje--exito";
}

function limpiarMensaje() {
    mensajeLogin.textContent = "";

    mensajeLogin.className =
        "mensaje mensaje--oculto";
}