import { protegerPagina } from "../../shared/js/auth.js";
import { inicializarLayout } from "../../shared/js/layout.js";
import { obtenerHorariosLaborales, crearHorarioLaboral, actualizarHorarioLaboral, cambiarEstadoHorarioLaboral } from "./horarios-laborales.service.js";

const botonNuevoHorario = document.getElementById("btnNuevoHorario");
const botonActualizar = document.getElementById("btnActualizar");
const inputBuscar = document.getElementById("buscarHorario");
const tablaHorariosBody = document.getElementById("tablaHorariosBody");
const contenedorTabla = document.getElementById("contenedorTabla");
const estadoCarga = document.getElementById("estadoCarga");
const sinResultados = document.getElementById("sinResultados");
const mensajeHorarios = document.getElementById("mensajeHorarios");
const modalHorario = document.getElementById("modalHorario");
const modalFondo = document.getElementById("modalFondo");
const tituloModalHorario = document.getElementById("tituloModalHorario");
const botonCerrarModal = document.getElementById("btnCerrarModal");
const botonCancelar = document.getElementById("btnCancelar");
const formHorario = document.getElementById("formHorario");
const inputIdHorarioLaboral = document.getElementById("idHorarioLaboral");
const inputNombreHorario = document.getElementById("nombreHorario");
const inputHoraEntrada = document.getElementById("horaEntrada");
const inputHoraSalida = document.getElementById("horaSalida");
const inputDiasLaborales = document.getElementById("diasLaborales");
const botonGuardarHorario = document.getElementById("btnGuardarHorario");
const mensajeFormulario = document.getElementById("mensajeFormulario");
const contadorHorarios = document.getElementById("contadorHorarios");

let horarios = [];
let temporizadorMensaje = null;

document.addEventListener("DOMContentLoaded", inicializarModulo);

async function inicializarModulo() {
    const accesoPermitido = protegerPagina();
    if (!accesoPermitido) return;

    inicializarLayout({ titulo: "Horarios laborales", paginaActiva: "horarios" });
    configurarEventos();
    await cargarHorarios();
}

function configurarEventos() {
    botonNuevoHorario.addEventListener("click", abrirModalNuevo);
    botonActualizar.addEventListener("click", cargarHorarios);
    inputBuscar.addEventListener("input", filtrarHorarios);
    formHorario.addEventListener("submit", guardarHorario);
    botonCerrarModal.addEventListener("click", cerrarModal);
    botonCancelar.addEventListener("click", cerrarModal);
    modalFondo.addEventListener("click", cerrarModal);
    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape" && !modalHorario.classList.contains("modal--oculto")) {
            cerrarModal();
        }
    });
}

async function cargarHorarios() {
    mostrarCargando(true);
    ocultarMensajePrincipal();
    botonActualizar.disabled = true;
    botonActualizar.textContent = "Actualizando...";

    try {
        horarios = await obtenerHorariosLaborales();
        renderizarHorarios(horarios);
        actualizarContador();
    } catch (error) {
        console.error(error);
        horarios = [];
        renderizarHorarios([]);
        mostrarMensajePrincipal(error.message || "No fue posible cargar los horarios.", "error");
    } finally {
        mostrarCargando(false);
        botonActualizar.disabled = false;
        botonActualizar.textContent = "Actualizar";
    }
}

function filtrarHorarios() {
    const texto = inputBuscar.value.trim().toLowerCase();
    const resultados = horarios.filter(horario => {
        const nombre = `${horario.nombre} ${horario.diasLaborales}`.toLowerCase();
        return !texto || nombre.includes(texto);
    });
    renderizarHorarios(resultados);
}

function renderizarHorarios(lista) {
    tablaHorariosBody.innerHTML = "";

    if (lista.length === 0) {
        contenedorTabla.classList.add("tabla-contenedor--oculto");
        sinResultados.classList.remove("estado-tabla--oculto");
        return;
    }

    contenedorTabla.classList.remove("tabla-contenedor--oculto");
    sinResultados.classList.add("estado-tabla--oculto");

    lista.forEach(horario => {
        const fila = document.createElement("tr");
        const claseEstado = horario.activo ? "estado estado--activo" : "estado estado--inactivo";
        const textoEstado = horario.activo ? "Activo" : "Inactivo";
        const textoBotonEstado = horario.activo ? "Desactivar" : "Activar";
        const claseBotonEstado = horario.activo ? "boton-tabla boton-tabla--desactivar" : "boton-tabla boton-tabla--activar";

        fila.innerHTML = `
            <td><strong>${escapeHtml(horario.nombre || "Sin nombre")}</strong></td>
            <td>${escapeHtml(horario.horaEntrada || "-")}</td>
            <td>${escapeHtml(horario.horaSalida || "-")}</td>
            <td>${escapeHtml(horario.diasLaborales || "-")}</td>
            <td><span class="${claseEstado}">${textoEstado}</span></td>
            <td>
              <div class="celda-acciones">
                <button type="button" class="boton-tabla boton-tabla--editar" data-accion="editar" data-id="${horario.idHorarioLaboral}">Editar</button>
                <button type="button" class="${claseBotonEstado}" data-accion="estado" data-id="${horario.idHorarioLaboral}">${textoBotonEstado}</button>
              </div>
            </td>
        `;

        tablaHorariosBody.appendChild(fila);
    });

    document.querySelectorAll('[data-accion="editar"]').forEach(boton => {
        boton.addEventListener("click", () => abrirModalEditar(Number(boton.dataset.id)));
    });

    document.querySelectorAll('[data-accion="estado"]').forEach(boton => {
        boton.addEventListener("click", () => cambiarEstado(Number(boton.dataset.id), boton));
    });
}

function abrirModalNuevo() {
    formHorario.reset();
    inputIdHorarioLaboral.value = "";
    tituloModalHorario.textContent = "Nuevo horario";
    botonGuardarHorario.textContent = "Guardar";
    limpiarMensajeFormulario();
    mostrarModal(modalHorario);
    inputNombreHorario.focus();
}

async function abrirModalEditar(idHorarioLaboral) {
    limpiarMensajeFormulario();
    try {
        const horario = horarios.find(item => Number(item.idHorarioLaboral) === Number(idHorarioLaboral));
        if (!horario) throw new Error("No se encontró el horario.");

        formHorario.reset();
        inputIdHorarioLaboral.value = horario.idHorarioLaboral;
        inputNombreHorario.value = horario.nombre;
        inputHoraEntrada.value = horario.horaEntrada;
        inputHoraSalida.value = horario.horaSalida;
        inputDiasLaborales.value = horario.diasLaborales;
        tituloModalHorario.textContent = "Editar horario";
        botonGuardarHorario.textContent = "Guardar cambios";
        mostrarModal(modalHorario);
        inputNombreHorario.focus();
    } catch (error) {
        console.error(error);
        mostrarMensajePrincipal(error.message || "No fue posible consultar el horario.", "error");
    }
}

async function guardarHorario(event) {
    event.preventDefault();
    limpiarMensajeFormulario();

    const datos = {
        nombre: inputNombreHorario.value.trim(),
        horaEntrada: inputHoraEntrada.value,
        horaSalida: inputHoraSalida.value,
        diasLaborales: inputDiasLaborales.value.trim()
    };

    if (!datos.nombre || !datos.horaEntrada || !datos.horaSalida || !datos.diasLaborales) {
        mostrarMensajeFormulario("Debe completar todos los campos del horario.");
        return;
    }

    try {
        if (inputIdHorarioLaboral.value) {
            await actualizarHorarioLaboral(Number(inputIdHorarioLaboral.value), datos);
            mostrarMensajePrincipal("Horario actualizado correctamente.", "exito");
        } else {
            await crearHorarioLaboral(datos);
            mostrarMensajePrincipal("Horario registrado correctamente.", "exito");
        }
        cerrarModal();
        await cargarHorarios();
    } catch (error) {
        console.error(error);
        mostrarMensajeFormulario(error.message || "No fue posible guardar el horario.");
    }
}

async function cambiarEstado(idHorarioLaboral, boton) {
    const horario = horarios.find(item => Number(item.idHorarioLaboral) === Number(idHorarioLaboral));
    if (!horario) return;

    const confirmado = window.confirm(`¿Está seguro de ${horario.activo ? "desactivar" : "activar"} el horario ${horario.nombre}?`);
    if (!confirmado) return;

    boton.disabled = true;
    try {
        await cambiarEstadoHorarioLaboral(idHorarioLaboral, !horario.activo);
        mostrarMensajePrincipal(horario.activo ? "Horario desactivado." : "Horario activado.", "exito");
        await cargarHorarios();
    } catch (error) {
        console.error(error);
        mostrarMensajePrincipal(error.message || "No fue posible cambiar el estado.", "error");
        boton.disabled = false;
    }
}

function actualizarContador() {
    contadorHorarios.textContent = `${horarios.length} horarios`;
}

function cerrarModal() {
    modalHorario.classList.add("modal--oculto");
    modalHorario.setAttribute("aria-hidden", "true");
    document.body.style.overflow = "";
    formHorario.reset();
    inputIdHorarioLaboral.value = "";
    limpiarMensajeFormulario();
}

function mostrarModal(modal) {
    modal.classList.remove("modal--oculto");
    modal.setAttribute("aria-hidden", "false");
    document.body.style.overflow = "hidden";
}

function mostrarCargando(cargando) {
    if (cargando) {
        estadoCarga.classList.remove("estado-tabla--oculto");
        contenedorTabla.classList.add("tabla-contenedor--oculto");
        sinResultados.classList.add("estado-tabla--oculto");
        return;
    }
    estadoCarga.classList.add("estado-tabla--oculto");
}

function mostrarMensajePrincipal(mensaje, tipo) {
    clearTimeout(temporizadorMensaje);
    mensajeHorarios.textContent = mensaje;
    mensajeHorarios.className = tipo === "exito" ? "mensaje mensaje--exito" : "mensaje mensaje--error";
    temporizadorMensaje = setTimeout(ocultarMensajePrincipal, 4000);
}

function ocultarMensajePrincipal() {
    clearTimeout(temporizadorMensaje);
    mensajeHorarios.textContent = "";
    mensajeHorarios.className = "mensaje mensaje--oculto";
}

function mostrarMensajeFormulario(mensaje) {
    mensajeFormulario.textContent = mensaje;
    mensajeFormulario.className = "mensaje mensaje--error";
}

function limpiarMensajeFormulario() {
    mensajeFormulario.textContent = "";
    mensajeFormulario.className = "mensaje mensaje--oculto";
}

function escapeHtml(valor) {
    return String(valor ?? "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/\"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
