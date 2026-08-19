import { protegerPagina } from "../../shared/js/auth.js";
import { inicializarLayout } from "../../shared/js/layout.js";
import { obtenerEmbargos, crearEmbargo, actualizarEmbargo, cambiarEstadoEmbargo, obtenerEmpleados } from "./embargos.service.js";

const mensaje = document.getElementById("mensajeEmbargos");
const tbody = document.getElementById("tablaEmbargosBody");
const contenedor = document.getElementById("contenedorTabla");
const sinResultados = document.getElementById("sinResultados");
const modal = document.getElementById("modalEmbargo");
const modalFondo = document.getElementById("modalFondo");
const btnNuevo = document.getElementById("btnNuevoEmbargo");
const btnActualizar = document.getElementById("btnActualizarEmbargos");
const btnCerrarModal = document.getElementById("btnCerrarModalEmbargo");
const btnCancelar = document.getElementById("btnCancelarEmbargo");
const formEmbargo = document.getElementById("formEmbargo");
const idEmbargoInput = document.getElementById("idEmbargo");
const idEmpleadoInput = document.getElementById("idEmpleado");
const fechaInicioInput = document.getElementById("fechaInicio");
const fechaFinInput = document.getElementById("fechaFin");
const tipoCalculoInput = document.getElementById("tipoCalculo");
const montoInput = document.getElementById("monto");
const porcentajeInput = document.getElementById("porcentaje");
const saldoInput = document.getElementById("saldoPendiente");
const referenciaInput = document.getElementById("referencia");
const observacionInput = document.getElementById("observacion");
const tituloModal = document.getElementById("tituloModalEmbargo");
const btnGuardar = document.getElementById("btnGuardarEmbargo");

let embargos = [];
let empleados = [];

document.addEventListener("DOMContentLoaded", inicializarModulo);

async function inicializarModulo() {
    const accesoPermitido = protegerPagina();
    if (!accesoPermitido) {
        return;
    }

    inicializarLayout({ titulo: "Embargos", paginaActiva: "embargos" });
    configurarEventos();
    await cargarDatos();
}

function configurarEventos() {
    btnNuevo.addEventListener("click", () => abrirModalNuevo());
    btnActualizar.addEventListener("click", cargarDatos);
    btnCerrarModal.addEventListener("click", cerrarModal);
    btnCancelar.addEventListener("click", cerrarModal);
    modalFondo?.addEventListener("click", cerrarModal);
    formEmbargo.addEventListener("submit", guardarEmbargo);
    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape" && !modal.classList.contains("modal--oculto")) {
            cerrarModal();
        }
    });
    tbody.addEventListener("click", manejarAccionTabla);
}

async function cargarDatos() {
    try {
        empleados = await obtenerEmpleados();
        embargos = await obtenerEmbargos();
        renderizarEmbargos(embargos);
        cargarSelectEmpleados();
    } catch (error) {
        console.error(error);
        renderizarEmbargos([]);
        mostrarMensaje(error.message || "No fue posible cargar los embargos.", "error");
    }
}

function cargarSelectEmpleados() {
    const opciones = [{ value: "", texto: "Seleccione un empleado" }]
        .concat(empleados.map((empleado) => ({
            value: empleado.idEmpleado ?? empleado.IdEmpleado,
            texto: `${empleado.codigoEmpleado ?? ""} - ${empleado.nombres ?? ""} ${empleado.apellidos ?? ""}`
        })))
        .map((op) => `<option value="${op.value}">${escapeHtml(op.texto)}</option>`)
        .join("");

    idEmpleadoInput.innerHTML = opciones;
}

function renderizarEmbargos(lista) {
    tbody.innerHTML = "";

    if (!lista || lista.length === 0) {
        contenedor.classList.add("tabla-contenedor--oculto");
        sinResultados.classList.remove("estado-tabla--oculto");
        return;
    }

    contenedor.classList.remove("tabla-contenedor--oculto");
    sinResultados.classList.add("estado-tabla--oculto");

    lista.forEach((embargo) => {
        const fila = document.createElement("tr");
        const empleado = empleados.find((item) => Number(item.idEmpleado ?? item.IdEmpleado) === Number(embargo.idEmpleado));
        const nombreCompleto = empleado ? `${empleado.nombres ?? ""} ${empleado.apellidos ?? ""}` : "-";
        const filaHtml = `
            <td><strong>${escapeHtml(nombreCompleto)}</strong></td>
            <td>${tipoCalculoTexto(embargo.tipoCalculo)}</td>
            <td>${formatearMoneda(embargo.monto)}</td>
            <td>${formatearPorcentaje(embargo.porcentaje)}</td>
            <td>${formatearMoneda(embargo.saldoPendiente)}</td>
            <td>${formatDate(embargo.fechaInicio)}</td>
            <td>${formatDate(embargo.fechaFin)}</td>
            <td><span class="estado ${embargo.activo ? "estado--activo" : "estado--inactivo"}">${embargo.activo ? "Activo" : "Inactivo"}</span></td>
            <td>${escapeHtml(embargo.referencia || "-")}</td>
            <td>${escapeHtml(embargo.observacion || "-")}</td>
            <td>
                <div class="celda-acciones">
                    <button type="button" class="boton-tabla boton-tabla--editar" data-accion="editar" data-id="${embargo.idEmbargo}">Editar</button>
                    <button type="button" class="boton-tabla ${embargo.activo ? "boton-tabla--desactivar" : "boton-tabla--activar"}" data-accion="estado" data-id="${embargo.idEmbargo}" data-activo="${embargo.activo ? "true" : "false"}">${embargo.activo ? "Desactivar" : "Activar"}</button>
                </div>
            </td>
        `;

        fila.innerHTML = filaHtml;
        tbody.appendChild(fila);
    });
}

function manejarAccionTabla(event) {
    const boton = event.target.closest("button[data-accion]");
    if (!boton) {
        return;
    }

    const idEmbargo = Number(boton.dataset.id);
    const accion = boton.dataset.accion;

    if (accion === "editar") {
        abrirModalEdicion(idEmbargo);
        return;
    }

    if (accion === "estado") {
        const activo = boton.dataset.activo !== "true";
        cambiarEstadoEmbargo(idEmbargo, activo)
            .then(() => {
                mostrarMensaje("Estado del embargo actualizado correctamente.", "exito");
                cargarDatos();
            })
            .catch((error) => {
                console.error(error);
                mostrarMensaje(error.message || "No fue posible cambiar el estado.", "error");
            });
    }
}

function abrirModalNuevo() {
    formEmbargo.reset();
    idEmbargoInput.value = "";
    tituloModal.textContent = "Nuevo embargo";
    btnGuardar.textContent = "Guardar";
    mostrarModal();
}

function abrirModalEdicion(idEmbargo) {
    const embargo = embargos.find((item) => Number(item.idEmbargo) === Number(idEmbargo));
    if (!embargo) {
        mostrarMensaje("No se encontró el embargo.", "error");
        return;
    }

    idEmbargoInput.value = embargo.idEmbargo;
    idEmpleadoInput.value = embargo.idEmpleado;
    fechaInicioInput.value = embargo.fechaInicio ? formatInputDate(embargo.fechaInicio) : "";
    fechaFinInput.value = embargo.fechaFin ? formatInputDate(embargo.fechaFin) : "";
    tipoCalculoInput.value = embargo.tipoCalculo ?? 1;
    montoInput.value = embargo.monto ?? "";
    porcentajeInput.value = embargo.porcentaje ?? "";
    saldoInput.value = embargo.saldoPendiente ?? 0;
    referenciaInput.value = embargo.referencia ?? "";
    observacionInput.value = embargo.observacion ?? "";

    tituloModal.textContent = "Editar embargo";
    btnGuardar.textContent = "Guardar cambios";
    mostrarModal();
}

function mostrarModal() {
    modal.classList.remove("modal--oculto");
}

function cerrarModal() {
    modal.classList.add("modal--oculto");
    formEmbargo.reset();
}

async function guardarEmbargo(event) {
    event.preventDefault();

    const payload = {
        idEmpleado: Number(idEmpleadoInput.value),
        fechaInicio: fechaInicioInput.value,
        fechaFin: fechaFinInput.value || null,
        tipoCalculo: Number(tipoCalculoInput.value),
        monto: montoInput.value ? Number(montoInput.value) : null,
        porcentaje: porcentajeInput.value ? Number(porcentajeInput.value) : null,
        saldoPendiente: Number(saldoInput.value || 0),
        referencia: referenciaInput.value.trim(),
        observacion: observacionInput.value.trim()
    };

    if (!payload.idEmpleado || !payload.fechaInicio) {
        mostrarMensaje("Debe seleccionar empleado y fecha de inicio.", "error");
        return;
    }

    try {
        if (idEmbargoInput.value) {
            await actualizarEmbargo(Number(idEmbargoInput.value), payload);
            mostrarMensaje("Embargo actualizado correctamente.", "exito");
        } else {
            await crearEmbargo(payload);
            mostrarMensaje("Embargo registrado correctamente.", "exito");
        }

        cerrarModal();
        await cargarDatos();
    } catch (error) {
        console.error(error);
        mostrarMensaje(error.message || "No fue posible guardar el embargo.", "error");
    }
}

function mostrarMensaje(texto, tipo) {
    mensaje.textContent = texto;
    mensaje.classList.remove("mensaje--error", "mensaje--exito");
    mensaje.classList.add(tipo === "error" ? "mensaje--error" : "mensaje--exito");
    mensaje.classList.add("mensaje--visible");
}

function tipoCalculoTexto(tipo) {
    return tipo === 1 ? "Porcentaje" : tipo === 2 ? "Monto fijo" : "-";
}

function formatearMoneda(valor) {
    if (valor === null || valor === undefined || valor === "") {
        return "C$ 0.00";
    }
    return new Intl.NumberFormat("es-NI", { style: "currency", currency: "NIO" }).format(Number(valor));
}

function formatearPorcentaje(valor) {
    if (valor === null || valor === undefined || valor === "") {
        return "0%";
    }
    return `${Number(valor)}%`;
}

function formatDate(valor) {
    if (!valor) return "-";
    const fecha = new Date(valor);
    return Number.isNaN(fecha.getTime()) ? valor : fecha.toLocaleDateString("es-ES");
}

function formatInputDate(valor) {
    if (!valor) return "";
    const fecha = new Date(valor);
    return Number.isNaN(fecha.getTime()) ? "" : fecha.toISOString().slice(0, 10);
}

function escapeHtml(valor) {
    return String(valor ?? "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/\"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
