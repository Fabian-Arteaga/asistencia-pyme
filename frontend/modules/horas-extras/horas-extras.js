import { protegerPagina, obtenerAdministrador } from "../../shared/js/auth.js";
import { inicializarLayout } from "../../shared/js/layout.js";
import { obtenerHorasExtrasPendientes, aprobarHoraExtra, rechazarHoraExtra } from "./horas-extras.service.js";

const mensaje = document.getElementById("mensajeHoraExtra");
const tablaBody = document.getElementById("tablaHorasExtrasBody");
const sinResultados = document.getElementById("sinResultados");
const contenedorTabla = document.getElementById("contenedorTabla");
const modal = document.getElementById("modalHoraExtra");
const btnCerrarModal = document.getElementById("btnCerrarModalHoraExtra");
const btnCancelar = document.getElementById("btnCancelarHoraExtra");
const formAccion = document.getElementById("formHoraExtraAccion");
const inputIdHoraExtra = document.getElementById("idHoraExtraAccion");
const inputMinutosAprobados = document.getElementById("minutosAprobados");
const inputObservacion = document.getElementById("observacionHoraExtra");
const tituloModal = document.getElementById("tituloModalHoraExtra");
const btnGuardar = document.getElementById("btnGuardarHoraExtra");

let accionActual = "aprobar";

document.addEventListener("DOMContentLoaded", inicializarModulo);

async function inicializarModulo() {
    const accesoPermitido = protegerPagina();
    if (!accesoPermitido) {
        return;
    }

    inicializarLayout({ titulo: "Horas extra", paginaActiva: "horas-extras" });
    configurarEventos();
    await cargarHorasExtras();
}

function configurarEventos() {
    btnCerrarModal.addEventListener("click", cerrarModal);
    btnCancelar.addEventListener("click", cerrarModal);
    formAccion.addEventListener("submit", manejarGuardarAccion);
    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape" && !modal.classList.contains("modal--oculto")) {
            cerrarModal();
        }
    });
}

async function cargarHorasExtras() {
    try {
        const horasExtras = await obtenerHorasExtrasPendientes();
        renderizarHorasExtras(horasExtras);
    } catch (error) {
        console.error(error);
        renderizarHorasExtras([]);
        mostrarMensaje(error.message || "No fue posible cargar las horas extra.", "error");
    }
}

function renderizarHorasExtras(horasExtras) {
    tablaBody.innerHTML = "";

    if (!horasExtras || horasExtras.length === 0) {
        contenedorTabla.classList.add("tabla-contenedor--oculto");
        sinResultados.classList.remove("estado-tabla--oculto");
        return;
    }

    contenedorTabla.classList.remove("tabla-contenedor--oculto");
    sinResultados.classList.add("estado-tabla--oculto");

    horasExtras.forEach((hora) => {
        const fila = document.createElement("tr");
        fila.innerHTML = `
            <td>${escapeHtml(hora.nombreEmpleado || "-")}</td>
            <td>${formatDate(hora.fecha)}</td>
            <td>${formatHora(hora.horaSalidaProgramada)}</td>
            <td>${formatHora(hora.horaSalidaReal)}</td>
            <td>${Number(hora.minutosDetectados ?? 0)}</td>
            <td>${Number(hora.minutosAprobados ?? 0)}</td>
            <td><span class="estado ${estadoClass(hora.estado)}">${estadoTexto(hora.estado)}</span></td>
            <td>${escapeHtml(hora.observacion || "-")}</td>
            <td>
                <div class="celda-acciones">
                    <button type="button" class="boton boton--primario" data-accion="aprobar" data-id="${hora.idHoraExtra}">Aprobar</button>
                    <button type="button" class="boton boton--secundario" data-accion="rechazar" data-id="${hora.idHoraExtra}">Rechazar</button>
                </div>
            </td>
        `;

        tablaBody.appendChild(fila);
    });

    document.querySelectorAll("[data-accion]").forEach((boton) => {
        boton.addEventListener("click", () => abrirModal(boton.dataset.accion, Number(boton.dataset.id)));
    });
}

function abrirModal(accion, idHoraExtra) {
    accionActual = accion;
    inputIdHoraExtra.value = idHoraExtra;
    inputMinutosAprobados.value = "";
    inputObservacion.value = "";
    tituloModal.textContent = accion === "aprobar" ? "Aprobar hora extra" : "Rechazar hora extra";
    btnGuardar.textContent = accion === "aprobar" ? "Aprobar" : "Rechazar";
    modal.classList.remove("modal--oculto");
}

function cerrarModal() {
    modal.classList.add("modal--oculto");
    formAccion.reset();
}

async function manejarGuardarAccion(event) {
    event.preventDefault();

    const idHoraExtra = Number(inputIdHoraExtra.value);
    const observacion = inputObservacion.value.trim();

    if (!idHoraExtra) {
        mostrarMensaje("Seleccione una hora extra válida.", "error");
        return;
    }

    try {
        if (accionActual === "aprobar") {
            const minutosAprobados = Number(inputMinutosAprobados.value || 0);
            if (minutosAprobados <= 0) {
                throw new Error("Debe ingresar minutos aprobados mayores a 0.");
            }

            const administrador = obtenerAdministrador();
            await aprobarHoraExtra(idHoraExtra, {
                minutosAprobados,
                aprobadoPor: administrador?.idAdministrador ?? administrador?.id ?? null,
                observacion
            });
            mostrarMensaje("Hora extra aprobada correctamente.", "exito");
        } else {
            await rechazarHoraExtra(idHoraExtra, {
                observacion
            });
            mostrarMensaje("Hora extra rechazada correctamente.", "exito");
        }

        cerrarModal();
        await cargarHorasExtras();
    } catch (error) {
        console.error(error);
        mostrarMensaje(error.message || "No fue posible procesar la acción.", "error");
    }
}

function mostrarMensaje(mensajeTexto, tipo) {
    mensaje.textContent = mensajeTexto;
    mensaje.classList.remove("mensaje--error", "mensaje--exito");
    mensaje.classList.add(tipo === "error" ? "mensaje--error" : "mensaje--exito");
    mensaje.classList.add("mensaje--visible");
}

function formatDate(valor) {
    if (!valor) return "-";
    const fecha = new Date(valor);
    return Number.isNaN(fecha.getTime()) ? valor : fecha.toLocaleDateString("es-ES");
}

function formatHora(valor) {
    if (!valor) return "-";
    const fecha = new Date(valor);
    return Number.isNaN(fecha.getTime()) ? valor : fecha.toLocaleTimeString("es-ES", { hour: "2-digit", minute: "2-digit" });
}

function escapeHtml(valor) {
    return String(valor ?? "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/\"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

function estadoTexto(estado) {
    const mapa = { 1: "Pendiente", 2: "Aprobada", 3: "Rechazada" };
    return mapa[estado] || "-";
}

function estadoClass(estado) {
    const mapa = { 1: "estado--pendiente", 2: "estado--activo", 3: "estado--inactivo" };
    return mapa[estado] || "";
}
