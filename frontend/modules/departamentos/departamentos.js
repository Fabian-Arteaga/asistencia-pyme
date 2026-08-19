import { protegerPagina } from "../../shared/js/auth.js";
import { inicializarLayout } from "../../shared/js/layout.js";
import { obtenerDepartamentos, crearDepartamento, actualizarDepartamento, cambiarEstadoDepartamento } from "./departamentos.service.js";

const botonNuevoDepartamento = document.getElementById("btnNuevoDepartamento");
const botonActualizar = document.getElementById("btnActualizar");
const inputBuscar = document.getElementById("buscarDepartamento");
const tablaDepartamentosBody = document.getElementById("tablaDepartamentosBody");
const contenedorTabla = document.getElementById("contenedorTabla");
const estadoCarga = document.getElementById("estadoCarga");
const sinResultados = document.getElementById("sinResultados");
const mensajeDepartamentos = document.getElementById("mensajeDepartamentos");
const modalDepartamento = document.getElementById("modalDepartamento");
const modalFondo = document.getElementById("modalFondo");
const tituloModalDepartamento = document.getElementById("tituloModalDepartamento");
const botonCerrarModal = document.getElementById("btnCerrarModal");
const botonCancelar = document.getElementById("btnCancelar");
const formDepartamento = document.getElementById("formDepartamento");
const inputIdDepartamento = document.getElementById("idDepartamento");
const inputNombreDepartamento = document.getElementById("nombreDepartamento");
const inputDescripcionDepartamento = document.getElementById("descripcionDepartamento");
const botonGuardarDepartamento = document.getElementById("btnGuardarDepartamento");
const mensajeFormulario = document.getElementById("mensajeFormulario");
const contadorDepartamentos = document.getElementById("contadorDepartamentos");

let departamentos = [];
let temporizadorMensaje = null;

document.addEventListener("DOMContentLoaded", inicializarModulo);

async function inicializarModulo() {
    const accesoPermitido = protegerPagina();
    if (!accesoPermitido) return;

    inicializarLayout({ titulo: "Departamentos", paginaActiva: "departamentos" });
    configurarEventos();
    await cargarDepartamentos();
}

function configurarEventos() {
    botonNuevoDepartamento.addEventListener("click", abrirModalNuevo);
    botonActualizar.addEventListener("click", cargarDepartamentos);
    inputBuscar.addEventListener("input", filtrarDepartamentos);
    formDepartamento.addEventListener("submit", guardarDepartamento);
    botonCerrarModal.addEventListener("click", cerrarModal);
    botonCancelar.addEventListener("click", cerrarModal);
    modalFondo.addEventListener("click", cerrarModal);
    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape" && !modalDepartamento.classList.contains("modal--oculto")) {
            cerrarModal();
        }
    });
}

async function cargarDepartamentos() {
    mostrarCargando(true);
    ocultarMensajePrincipal();
    botonActualizar.disabled = true;
    botonActualizar.textContent = "Actualizando...";

    try {
        departamentos = await obtenerDepartamentos();
        renderizarDepartamentos(departamentos);
        actualizarContador();
    } catch (error) {
        console.error(error);
        departamentos = [];
        renderizarDepartamentos([]);
        mostrarMensajePrincipal(error.message || "No fue posible cargar los departamentos.", "error");
    } finally {
        mostrarCargando(false);
        botonActualizar.disabled = false;
        botonActualizar.textContent = "Actualizar";
    }
}

function filtrarDepartamentos() {
    const texto = inputBuscar.value.trim().toLowerCase();
    const resultados = departamentos.filter(departamento => {
        const nombre = `${departamento.nombre} ${departamento.descripcion}`.toLowerCase();
        return !texto || nombre.includes(texto);
    });
    renderizarDepartamentos(resultados);
}

function renderizarDepartamentos(lista) {
    tablaDepartamentosBody.innerHTML = "";

    if (lista.length === 0) {
        contenedorTabla.classList.add("tabla-contenedor--oculto");
        sinResultados.classList.remove("estado-tabla--oculto");
        return;
    }

    contenedorTabla.classList.remove("tabla-contenedor--oculto");
    sinResultados.classList.add("estado-tabla--oculto");

    lista.forEach(departamento => {
        const fila = document.createElement("tr");
        const claseEstado = departamento.activo ? "estado estado--activo" : "estado estado--inactivo";
        const textoEstado = departamento.activo ? "Activo" : "Inactivo";
        const textoBotonEstado = departamento.activo ? "Desactivar" : "Activar";
        const claseBotonEstado = departamento.activo ? "boton-tabla boton-tabla--desactivar" : "boton-tabla boton-tabla--activar";

        fila.innerHTML = `
            <td><strong>${escapeHtml(departamento.nombre || "Sin nombre")}</strong></td>
            <td>${escapeHtml(departamento.descripcion || "Sin descripción")}</td>
            <td><span class="${claseEstado}">${textoEstado}</span></td>
            <td>
              <div class="celda-acciones">
                <button type="button" class="boton-tabla boton-tabla--editar" data-accion="editar" data-id="${departamento.idDepartamento}">Editar</button>
                <button type="button" class="${claseBotonEstado}" data-accion="estado" data-id="${departamento.idDepartamento}">${textoBotonEstado}</button>
              </div>
            </td>
        `;

        tablaDepartamentosBody.appendChild(fila);
    });

    document.querySelectorAll('[data-accion="editar"]').forEach(boton => {
        boton.addEventListener("click", () => abrirModalEditar(Number(boton.dataset.id)));
    });

    document.querySelectorAll('[data-accion="estado"]').forEach(boton => {
        boton.addEventListener("click", () => cambiarEstado(Number(boton.dataset.id), boton));
    });
}

function abrirModalNuevo() {
    formDepartamento.reset();
    inputIdDepartamento.value = "";
    tituloModalDepartamento.textContent = "Nuevo departamento";
    botonGuardarDepartamento.textContent = "Guardar";
    limpiarMensajeFormulario();
    mostrarModal(modalDepartamento);
    inputNombreDepartamento.focus();
}

async function abrirModalEditar(idDepartamento) {
    limpiarMensajeFormulario();
    try {
        const departamento = departamentos.find(item => Number(item.idDepartamento) === Number(idDepartamento));
        if (!departamento) throw new Error("No se encontró el departamento.");

        formDepartamento.reset();
        inputIdDepartamento.value = departamento.idDepartamento;
        inputNombreDepartamento.value = departamento.nombre;
        inputDescripcionDepartamento.value = departamento.descripcion;
        tituloModalDepartamento.textContent = "Editar departamento";
        botonGuardarDepartamento.textContent = "Guardar cambios";
        mostrarModal(modalDepartamento);
        inputNombreDepartamento.focus();
    } catch (error) {
        console.error(error);
        mostrarMensajePrincipal(error.message || "No fue posible consultar el departamento.", "error");
    }
}

async function guardarDepartamento(event) {
    event.preventDefault();
    limpiarMensajeFormulario();

    const datos = {
        nombre: inputNombreDepartamento.value.trim(),
        descripcion: inputDescripcionDepartamento.value.trim()
    };

    if (!datos.nombre) {
        mostrarMensajeFormulario("Debe ingresar el nombre del departamento.");
        return;
    }

    try {
        if (inputIdDepartamento.value) {
            await actualizarDepartamento(Number(inputIdDepartamento.value), datos);
            mostrarMensajePrincipal("Departamento actualizado correctamente.", "exito");
        } else {
            await crearDepartamento(datos);
            mostrarMensajePrincipal("Departamento registrado correctamente.", "exito");
        }
        cerrarModal();
        await cargarDepartamentos();
    } catch (error) {
        console.error(error);
        mostrarMensajeFormulario(error.message || "No fue posible guardar el departamento.");
    }
}

async function cambiarEstado(idDepartamento, boton) {
    const departamento = departamentos.find(item => Number(item.idDepartamento) === Number(idDepartamento));
    if (!departamento) return;

    const confirmado = window.confirm(`¿Está seguro de ${departamento.activo ? "desactivar" : "activar"} el departamento ${departamento.nombre}?`);
    if (!confirmado) return;

    boton.disabled = true;
    try {
        await cambiarEstadoDepartamento(idDepartamento, !departamento.activo);
        mostrarMensajePrincipal(departamento.activo ? "Departamento desactivado." : "Departamento activado.", "exito");
        await cargarDepartamentos();
    } catch (error) {
        console.error(error);
        mostrarMensajePrincipal(error.message || "No fue posible cambiar el estado.", "error");
        boton.disabled = false;
    }
}

function actualizarContador() {
    contadorDepartamentos.textContent = `${departamentos.length} departamentos`;
}

function cerrarModal() {
    modalDepartamento.classList.add("modal--oculto");
    modalDepartamento.setAttribute("aria-hidden", "true");
    document.body.style.overflow = "";
    formDepartamento.reset();
    inputIdDepartamento.value = "";
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
    mensajeDepartamentos.textContent = mensaje;
    mensajeDepartamentos.className = tipo === "exito" ? "mensaje mensaje--exito" : "mensaje mensaje--error";
    temporizadorMensaje = setTimeout(ocultarMensajePrincipal, 4000);
}

function ocultarMensajePrincipal() {
    clearTimeout(temporizadorMensaje);
    mensajeDepartamentos.textContent = "";
    mensajeDepartamentos.className = "mensaje mensaje--oculto";
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
