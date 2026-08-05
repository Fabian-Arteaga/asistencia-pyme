import {
    marcarAsistencia
} from "./kiosco.service.js";

const DURACION_POPUP = 1000;

const formMarcaje =
    document.getElementById("formMarcaje");

const inputCodigo =
    document.getElementById("codigoEmpleado");

const inputPin =
    document.getElementById("pin");

const botonMarcar =
    document.getElementById("btnMarcar");

const horaActual =
    document.getElementById("horaActual");

const fechaActual =
    document.getElementById("fechaActual");

const popupMarcaje =
    document.getElementById("popupMarcaje");

const popupIcono =
    document.getElementById("popupIcono");

const popupTitulo =
    document.getElementById("popupTitulo");

const popupMensaje =
    document.getElementById("popupMensaje");

let temporizadorPopup = null;

document.addEventListener(
    "DOMContentLoaded",
    inicializarKiosco
);

window.addEventListener(
    "pageshow",
    limpiarFormulario
);

formMarcaje.addEventListener(
    "submit",
    procesarMarcaje
);

inputCodigo.addEventListener(
    "pointerdown",
    habilitarCampo
);

inputCodigo.addEventListener(
    "focus",
    habilitarCampo
);

inputPin.addEventListener(
    "pointerdown",
    habilitarCampo
);

inputPin.addEventListener(
    "focus",
    habilitarCampo
);

popupMarcaje.addEventListener(
    "click",
    ocultarPopup
);

function inicializarKiosco() {
    limpiarFormulario();

    actualizarReloj();

    setInterval(
        actualizarReloj,
        1000
    );

    /*
     * Algunos navegadores completan los campos
     * unos milisegundos después de cargar.
     */
    setTimeout(
        limpiarFormulario,
        300
    );
}

async function procesarMarcaje(event) {
    event.preventDefault();

    const codigoEmpleado =
        inputCodigo.value
            .trim()
            .toUpperCase();

    const pin =
        inputPin.value.trim();

    if (!codigoEmpleado) {
        mostrarPopup(
            "error",
            "Código requerido",
            "Debe ingresar su código de empleado."
        );

        limpiarFormulario();
        return;
    }

    if (!/^[0-9]{4,6}$/.test(pin)) {
        mostrarPopup(
            "error",
            "PIN incorrecto",
            "El PIN debe contener entre 4 y 6 números."
        );

        limpiarFormulario();
        return;
    }

    bloquearFormulario(true);

    try {
        const data = await marcarAsistencia(
            codigoEmpleado,
            pin
        );

        const nombreEmpleado =
            data?.nombreEmpleado ??
            data?.NombreEmpleado ??
            "";

        const tipoMarcacion =
            data?.tipoMarcacion ??
            data?.TipoMarcacion ??
            "";

        const mensajeBackend =
            data?.mensaje ??
            data?.Mensaje ??
            "Asistencia registrada correctamente.";

        const titulo = tipoMarcacion
            ? `${tipoMarcacion} registrada`
            : "Asistencia registrada";

        const mensaje = nombreEmpleado
            ? `${nombreEmpleado}: ${mensajeBackend}`
            : mensajeBackend;

        mostrarPopup(
            "exito",
            titulo,
            mensaje
        );
    } catch (error) {
        console.error(error);

        const mensaje =
            error instanceof TypeError
                ? "No fue posible conectarse con el servidor."
                : error.message ||
                  "No fue posible registrar la asistencia.";

        mostrarPopup(
            "error",
            "Marcaje no realizado",
            mensaje
        );
    } finally {
        limpiarFormulario();
        bloquearFormulario(false);
    }
}

function limpiarFormulario() {
    formMarcaje.reset();

    /*
     * Se asigna vacío directamente para evitar
     * que el navegador conserve datos anteriores.
     */
    inputCodigo.value = "";
    inputPin.value = "";

    /*
     * Readonly evita que el navegador coloque
     * usuarios o contraseñas guardadas.
     */
    inputCodigo.setAttribute(
        "readonly",
        "readonly"
    );

    inputPin.setAttribute(
        "readonly",
        "readonly"
    );
}

function habilitarCampo(event) {
    const campo = event.currentTarget;

    if (campo.hasAttribute("readonly")) {
        campo.removeAttribute("readonly");
        campo.value = "";
    }
}

function mostrarPopup(
    tipo,
    titulo,
    mensaje
) {
    clearTimeout(temporizadorPopup);

    popupTitulo.textContent = titulo;
    popupMensaje.textContent = mensaje;

    if (tipo === "error") {
        popupMarcaje.className =
            "popup popup--error";

        popupIcono.textContent = "!";
    } else {
        popupMarcaje.className =
            "popup popup--exito";

        popupIcono.textContent = "✓";
    }

    popupMarcaje.setAttribute(
        "aria-hidden",
        "false"
    );

    temporizadorPopup = setTimeout(
        ocultarPopup,
        DURACION_POPUP
    );
}

function ocultarPopup() {
    clearTimeout(temporizadorPopup);

    popupMarcaje.className =
        "popup popup--oculto";

    popupMarcaje.setAttribute(
        "aria-hidden",
        "true"
    );

    limpiarFormulario();
}

function bloquearFormulario(bloqueado) {
    inputCodigo.disabled = bloqueado;
    inputPin.disabled = bloqueado;
    botonMarcar.disabled = bloqueado;

    botonMarcar.textContent = bloqueado
        ? "Procesando..."
        : "Marcar asistencia";
}

function actualizarReloj() {
    const ahora = new Date();

    horaActual.textContent =
        ahora.toLocaleTimeString(
            "es-NI",
            {
                hour: "2-digit",
                minute: "2-digit",
                second: "2-digit"
            }
        );

    fechaActual.textContent =
        ahora.toLocaleDateString(
            "es-NI",
            {
                weekday: "long",
                day: "2-digit",
                month: "long",
                year: "numeric"
            }
        );
}