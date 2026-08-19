import {
    protegerPagina,
    obtenerAdministrador,
    obtenerToken
} from "../../shared/js/auth.js";

import {
    inicializarLayout
} from "../../shared/js/layout.js";

import {
    obtenerPlanillas,
    obtenerPlanillaPorId,
    generarPlanilla,
    cambiarEstadoPlanilla,
    obtenerEmpleadosParaPlanilla,
    obtenerEmpleadosPorDepartamento,
    recalcularPlanilla,
    enviarPlanillaRevision,
    cerrarPlanilla,
    pagarPlanilla,
    anularPlanilla,
    obtenerDepartamentos
} from "./planillas.services.js";

// Estado del módulo
let planillas = [];
let empleados = [];
let departamentos = [];
let empleadosDisponiblesDepartamento = [];
let empleadosSeleccionadosPlanilla = new Set();
let temporizadorMensaje = null;

// Estados de planilla según enum backend
const ESTADOS_PLANILLA = {
    1: {
        nombre: "Borrador",
        clase: "estado estado--borrador"
    },
    2: {
        nombre: "Calculada",
        clase: "estado estado--calculada"
    },
    3: {
        nombre: "En revisión",
        clase: "estado estado--revision"
    },
    4: {
        nombre: "Cerrada",
        clase: "estado estado--cerrada"
    },
    5: {
        nombre: "Pagada",
        clase: "estado estado--pagada"
    },
    6: {
        nombre: "Anulada",
        clase: "estado estado--anulada"
    }
};

const obtenerElemento = id => document.getElementById(id);

// Elementos de la vista principal
const botonNuevaPlanilla = obtenerElemento("btnNuevaPlanilla");
const botonActualizar = obtenerElemento("btnActualizar");
const botonLimpiarFiltros = obtenerElemento("btnLimpiarFiltros");
const filtroEmpleado = obtenerElemento("filtroEmpleado");
const filtroDesde = obtenerElemento("filtroDesde");
const filtroHasta = obtenerElemento("filtroHasta");
const filtroEstado = obtenerElemento("filtroEstado");
const contadorPlanillas = obtenerElemento("contadorPlanillas");
const totalNetoVisible = obtenerElemento("totalNetoVisible");
const tablaPlanillasBody = obtenerElemento("tablaPlanillasBody");
const contenedorTabla = obtenerElemento("contenedorTabla");
const estadoCarga = obtenerElemento("estadoCarga");
const sinResultados = obtenerElemento("sinResultados");
const mensajePlanillas = obtenerElemento("mensajePlanillas");

// Elementos del modal Generar Planilla
const modalPlanilla = obtenerElemento("modalPlanilla");
const fondoModalPlanilla = obtenerElemento("fondoModalPlanilla");
const formPlanilla = obtenerElemento("formPlanilla");
const selectDepartamentoPlanilla = obtenerElemento("departamentoPlanilla");
const chkSeleccionarTodosEmpleados = obtenerElemento("chkSeleccionarTodosEmpleados");
const contadorEmpleadosSeleccionados = obtenerElemento("contadorEmpleadosSeleccionados");
const contenedorListaEmpleadosPlanilla = obtenerElemento("contenedorListaEmpleadosPlanilla");
const cargandoEmpleadosDepartamento = obtenerElemento("cargandoEmpleadosDepartamento");
const inputFechaInicio = obtenerElemento("fechaInicioPeriodo");
const inputFechaFin = obtenerElemento("fechaFinPeriodo");
const mensajeFormulario = obtenerElemento("mensajeFormulario");
const botonCerrarModalPlanilla = obtenerElemento("btnCerrarModalPlanilla");
const botonCancelarPlanilla = obtenerElemento("btnCancelarPlanilla");
const botonGuardarPlanilla = obtenerElemento("btnGuardarPlanilla");

// Elementos del modal Detalle
const modalDetalle = obtenerElemento("modalDetalle");
const fondoModalDetalle = obtenerElemento("fondoModalDetalle");
const contenidoDetallePlanilla = obtenerElemento("contenidoDetallePlanilla");
const botonCerrarModalDetalle = obtenerElemento("btnCerrarModalDetalle");
const botonCerrarDetalle = obtenerElemento("btnCerrarDetalle");

// Elementos del modal Cambiar Estado
const modalEstado = obtenerElemento("modalEstado");
const fondoModalEstado = obtenerElemento("fondoModalEstado");
const formEstado = obtenerElemento("formEstado");
const inputIdPlanillaEstado = obtenerElemento("idPlanillaEstado");
const descripcionCambioEstado = obtenerElemento("descripcionCambioEstado");
const selectNuevoEstado = obtenerElemento("nuevoEstadoPlanilla");
const mensajeEstado = obtenerElemento("mensajeEstado");
const botonCerrarModalEstado = obtenerElemento("btnCerrarModalEstado");
const botonCancelarEstado = obtenerElemento("btnCancelarEstado");
const botonGuardarEstado = obtenerElemento("btnGuardarEstado");

document.addEventListener("DOMContentLoaded", inicializarModulo);

async function inicializarModulo() {
    const accesoPermitido = protegerPagina();
    if (!accesoPermitido) {
        return;
    }

    inicializarLayout({
        titulo: "Planillas",
        paginaActiva: "planillas"
    });

    configurarEventos();

    // Carga de catálogos
    await Promise.all([
        cargarDepartamentos(),
        cargarEmpleados()
    ]);

    // Carga de planillas
    await cargarPlanillas();
}

function configurarEventos() {
    // Acciones principales
    botonNuevaPlanilla.addEventListener("click", abrirModalNuevaPlanilla);
    botonActualizar.addEventListener("click", () => cargarPlanillas());
    botonLimpiarFiltros.addEventListener("click", limpiarFiltros);

    // Filtros
    filtroEmpleado.addEventListener("change", aplicarFiltros);
    filtroDesde.addEventListener("change", aplicarFiltros);
    filtroHasta.addEventListener("change", aplicarFiltros);
    filtroEstado.addEventListener("change", aplicarFiltros);

    // Acciones en tabla
    tablaPlanillasBody.addEventListener("click", manejarAccionTabla);

    // Formulario de generación de planilla
    formPlanilla.addEventListener("submit", guardarPlanilla);
    selectDepartamentoPlanilla.addEventListener("change", manejarCambioDepartamento);
    chkSeleccionarTodosEmpleados.addEventListener("change", manejarToggleTodosEmpleados);
    contenedorListaEmpleadosPlanilla.addEventListener("change", manejarCambioCheckboxEmpleado);
    inputFechaInicio.addEventListener("change", manejarCambioFechaInicio);
    botonCerrarModalPlanilla.addEventListener("click", cerrarModalPlanilla);
    botonCancelarPlanilla.addEventListener("click", cerrarModalPlanilla);
    fondoModalPlanilla.addEventListener("click", cerrarModalPlanilla);

    // Modal Detalle
    botonCerrarModalDetalle.addEventListener("click", cerrarModalDetalle);
    botonCerrarDetalle.addEventListener("click", cerrarModalDetalle);
    fondoModalDetalle.addEventListener("click", cerrarModalDetalle);

    // Modal Estado
    formEstado.addEventListener("submit", guardarCambioEstado);
    botonCerrarModalEstado.addEventListener("click", cerrarModalEstado);
    botonCancelarEstado.addEventListener("click", cerrarModalEstado);
    fondoModalEstado.addEventListener("click", cerrarModalEstado);

    // Tecla Escape para modales
    document.addEventListener("keydown", manejarTeclaEscape);
}

async function cargarDepartamentos() {
    try {
        departamentos = await obtenerDepartamentos();
        departamentos.sort((a, b) => (a.nombre || "").localeCompare(b.nombre || "", "es"));

        const opciones = departamentos.map(d => `
            <option value="${escaparAtributo(d.idDepartamento)}">${escaparHtml(d.nombre)}</option>
        `).join("");

        if (selectDepartamentoPlanilla) {
            selectDepartamentoPlanilla.innerHTML = `
                <option value="">Seleccione un departamento</option>
                ${opciones}
            `;
        }
    } catch (error) {
        console.error("Error al cargar departamentos:", error);
    }
}

async function cargarEmpleados() {
    try {
        empleados = await obtenerEmpleadosParaPlanilla();
        empleados.sort((a, b) => a.nombreCompleto.localeCompare(b.nombreCompleto, "es"));
        llenarSelectEmpleados();
    } catch (error) {
        console.error("Error al cargar empleados para filtro:", error);
        empleados = [];
    }
}

function llenarSelectEmpleados() {
    const opciones = empleados.map(empleado => `
        <option value="${escaparAtributo(empleado.codigoEmpleado)}">
            ${escaparHtml(empleado.codigoEmpleado)} - ${escaparHtml(empleado.nombreCompleto)}
        </option>
    `).join("");

    filtroEmpleado.innerHTML = `
        <option value="">Todos los empleados</option>
        ${opciones}
    `;
}

async function cargarPlanillas(limpiarMensaje = true) {
    mostrarCargando(true);

    if (limpiarMensaje) {
        ocultarMensajePrincipal();
    }

    botonActualizar.disabled = true;
    botonActualizar.textContent = "Actualizando...";

    try {
        planillas = await obtenerPlanillas();
        planillas.sort((a, b) => b.fechaInicioPeriodo.localeCompare(a.fechaInicioPeriodo));
        aplicarFiltros();
    } catch (error) {
        console.error("Error al cargar planillas:", error);
        planillas = [];
        renderizarPlanillas([]);
        mostrarMensajePrincipal(error.message || "No se pudieron cargar las planillas.", "error");
    } finally {
        mostrarCargando(false);
        botonActualizar.disabled = false;
        botonActualizar.textContent = "Actualizar";
    }
}

function aplicarFiltros() {
    const codigoEmpleado = filtroEmpleado.value;
    const fechaDesde = filtroDesde.value;
    const fechaHasta = filtroHasta.value;
    const estado = filtroEstado.value;

    const resultados = planillas.filter(planilla => {
        const coincideEmpleado = !codigoEmpleado ||
            planilla.codigoEmpleado === codigoEmpleado ||
            (Array.isArray(planilla.detalles) && planilla.detalles.some(d => d.codigoEmpleado === codigoEmpleado));

        const coincideDesde = !fechaDesde || planilla.fechaFinPeriodo >= fechaDesde;
        const coincideHasta = !fechaHasta || planilla.fechaInicioPeriodo <= fechaHasta;
        const coincideEstado = !estado || Number(planilla.estado) === Number(estado);

        return coincideEmpleado && coincideDesde && coincideHasta && coincideEstado;
    });

    actualizarResumenListado(resultados);
    renderizarPlanillas(resultados);
}

function limpiarFiltros() {
    filtroEmpleado.value = "";
    filtroDesde.value = "";
    filtroHasta.value = "";
    filtroEstado.value = "";
    aplicarFiltros();
}

function renderizarPlanillas(lista) {
    tablaPlanillasBody.innerHTML = "";

    if (lista.length === 0) {
        contenedorTabla.classList.add("tabla-contenedor--oculto");
        sinResultados.classList.remove("estado-tabla--oculto");
        return;
    }

    contenedorTabla.classList.remove("tabla-contenedor--oculto");
    sinResultados.classList.add("estado-tabla--oculto");

    lista.forEach(planilla => {
        const estado = obtenerPresentacionEstado(planilla.estado);
        const fila = document.createElement("tr");

        const tituloPrincipal = planilla.nombreDepartamento || planilla.nombreEmpleado || "Planilla Departamental";
        const subtitulo = planilla.cantidadEmpleados
            ? `${planilla.cantidadEmpleados} ${planilla.cantidadEmpleados === 1 ? "colaborador" : "colaboradores"}`
            : (planilla.codigoEmpleado || "");

        fila.innerHTML = `
            <td>
                <span class="empleado-nombre">${escaparHtml(tituloPrincipal)}</span>
                <span class="empleado-codigo">${escaparHtml(subtitulo)}</span>
            </td>
            <td>
                ${formatearFecha(planilla.fechaInicioPeriodo)}
                <br>al ${formatearFecha(planilla.fechaFinPeriodo)}
            </td>
            <td class="monto">${formatearMoneda(planilla.salarioBasePeriodo)}</td>
            <td class="monto">${formatearMoneda(planilla.ingresosAdicionales)}</td>
            <td class="monto">${formatearMoneda(planilla.totalDeducciones)}</td>
            <td class="monto monto--neto">${formatearMoneda(planilla.salarioNeto)}</td>
            <td>
                <span class="${estado.clase}">${escaparHtml(estado.nombre)}</span>
            </td>
            <td>
                <div class="celda-acciones">
                    <button
                        type="button"
                        class="boton-tabla boton-tabla--detalle"
                        data-accion="detalle"
                        data-id="${planilla.idPlanilla}"
                    >
                        Ver detalle
                    </button>

                    <button
                        type="button"
                        class="boton-tabla boton-tabla--estado"
                        data-accion="estado"
                        data-id="${planilla.idPlanilla}"
                    >
                        Estado
                    </button>

                    <button
                        type="button"
                        class="boton-tabla"
                        data-accion="recalcular"
                        data-id="${planilla.idPlanilla}"
                    >
                        Recalcular
                    </button>

                    <button
                        type="button"
                        class="boton-tabla"
                        data-accion="revision"
                        data-id="${planilla.idPlanilla}"
                    >
                        Revisar
                    </button>

                    <button
                        type="button"
                        class="boton-tabla"
                        data-accion="cerrar"
                        data-id="${planilla.idPlanilla}"
                    >
                        Cerrar
                    </button>

                    <button
                        type="button"
                        class="boton-tabla"
                        data-accion="pagar"
                        data-id="${planilla.idPlanilla}"
                    >
                        Pagar
                    </button>

                    <button
                        type="button"
                        class="boton-tabla boton-tabla--anular"
                        data-accion="anular"
                        data-id="${planilla.idPlanilla}"
                    >
                        Anular
                    </button>
                </div>
            </td>
        `;

        tablaPlanillasBody.appendChild(fila);
    });
}

function manejarAccionTabla(event) {
    const boton = event.target.closest("[data-accion]");
    if (!boton) return;

    const idPlanilla = Number(boton.dataset.id);
    if (!Number.isInteger(idPlanilla) || idPlanilla <= 0) return;

    const accion = boton.dataset.accion;

    if (accion === "detalle") {
        abrirDetallePlanilla(idPlanilla);
        return;
    }

    if (accion === "estado") {
        abrirModalEstado(idPlanilla);
        return;
    }

    if (accion === "recalcular") {
        ejecutarAccionPlanilla(idPlanilla, "recalcular");
        return;
    }

    if (accion === "revision") {
        ejecutarAccionPlanilla(idPlanilla, "revision");
        return;
    }

    if (accion === "cerrar") {
        ejecutarAccionPlanilla(idPlanilla, "cerrar");
        return;
    }

    if (accion === "pagar") {
        ejecutarAccionPlanilla(idPlanilla, "pagar");
        return;
    }

    if (accion === "anular") {
        ejecutarAccionPlanilla(idPlanilla, "anular");
    }
}

async function ejecutarAccionPlanilla(idPlanilla, accion) {
    const idAdministrador = obtenerIdAdministradorActual();

    try {
        if (accion === "recalcular") {
            await recalcularPlanilla(idPlanilla, idAdministrador);
        } else if (accion === "revision") {
            await enviarPlanillaRevision(idPlanilla, idAdministrador);
        } else if (accion === "cerrar") {
            await cerrarPlanilla(idPlanilla, idAdministrador);
        } else if (accion === "pagar") {
            await pagarPlanilla(idPlanilla, idAdministrador);
        } else if (accion === "anular") {
            await anularPlanilla(idPlanilla, idAdministrador, "Anulación desde módulo web de planillas");
        }

        mostrarMensajePrincipal("Acción aplicada correctamente.", "exito");
        await cargarPlanillas(false);
    } catch (error) {
        console.error("Error al ejecutar acción de planilla:", error);
        mostrarMensajePrincipal(error.message || "No fue posible ejecutar la acción requerida.", "error");
    }
}

// -------------------------------------------------------------
// Modal Generar Planilla por Departamento
// -------------------------------------------------------------
function abrirModalNuevaPlanilla() {
    formPlanilla.reset();

    empleadosDisponiblesDepartamento = [];
    empleadosSeleccionadosPlanilla.clear();

    selectDepartamentoPlanilla.value = "";
    chkSeleccionarTodosEmpleados.checked = false;
    chkSeleccionarTodosEmpleados.disabled = true;
    botonGuardarPlanilla.disabled = true;

    contadorEmpleadosSeleccionados.textContent = "0 colaboradores seleccionados";
    contenedorListaEmpleadosPlanilla.innerHTML = `
        <div id="mensajeListaVacia" class="estado-lista-vacia">
            Seleccione un departamento para consultar y elegir a los colaboradores.
        </div>
    `;

    const periodo = obtenerPeriodoMensualActual();
    inputFechaInicio.value = periodo.inicio;
    inputFechaFin.value = periodo.fin;
    inputFechaFin.min = periodo.inicio;

    limpiarMensajeFormulario();
    mostrarModal(modalPlanilla);
    selectDepartamentoPlanilla.focus();
}

async function manejarCambioDepartamento() {
    const idDepartamento = Number(selectDepartamentoPlanilla.value || 0);

    // Limpiar estado y selecciones previas
    empleadosDisponiblesDepartamento = [];
    empleadosSeleccionadosPlanilla.clear();
    limpiarMensajeFormulario();

    if (!idDepartamento) {
        chkSeleccionarTodosEmpleados.checked = false;
        chkSeleccionarTodosEmpleados.disabled = true;
        botonGuardarPlanilla.disabled = true;
        actualizarContadorSeleccionados();
        contenedorListaEmpleadosPlanilla.innerHTML = `
            <div id="mensajeListaVacia" class="estado-lista-vacia">
                Seleccione un departamento para consultar y elegir a los colaboradores.
            </div>
        `;
        return;
    }

    cargandoEmpleadosDepartamento.classList.remove("estado-tabla--oculto");
    contenedorListaEmpleadosPlanilla.innerHTML = "";
    chkSeleccionarTodosEmpleados.disabled = true;
    botonGuardarPlanilla.disabled = true;

    try {
        const colaboradores = await obtenerEmpleadosPorDepartamento(idDepartamento);
        empleadosDisponiblesDepartamento = colaboradores || [];

        if (empleadosDisponiblesDepartamento.length === 0) {
            contenedorListaEmpleadosPlanilla.innerHTML = `
                <div id="mensajeListaVacia" class="estado-lista-vacia">
                    No hay colaboradores disponibles en este departamento.
                </div>
            `;
            chkSeleccionarTodosEmpleados.checked = false;
            chkSeleccionarTodosEmpleados.disabled = true;
            botonGuardarPlanilla.disabled = true;
            actualizarContadorSeleccionados();
            return;
        }

        // Seleccionar automáticamente todos los empleados del departamento
        empleadosDisponiblesDepartamento.forEach(emp => {
            empleadosSeleccionadosPlanilla.add(Number(emp.idEmpleado));
        });

        chkSeleccionarTodosEmpleados.checked = true;
        chkSeleccionarTodosEmpleados.disabled = false;
        botonGuardarPlanilla.disabled = false;

        renderizarListaEmpleadosDepartamento();
        actualizarContadorSeleccionados();
    } catch (error) {
        console.error("Error al cargar empleados del departamento:", error);
        contenedorListaEmpleadosPlanilla.innerHTML = `
            <div id="mensajeListaVacia" class="estado-lista-vacia" style="color: #b91c1c;">
                Error al cargar los colaboradores: ${escaparHtml(error.message || "Error de red")}
            </div>
        `;
        chkSeleccionarTodosEmpleados.checked = false;
        chkSeleccionarTodosEmpleados.disabled = true;
        botonGuardarPlanilla.disabled = true;
        actualizarContadorSeleccionados();
    } finally {
        cargandoEmpleadosDepartamento.classList.add("estado-tabla--oculto");
    }
}

function renderizarListaEmpleadosDepartamento() {
    if (empleadosDisponiblesDepartamento.length === 0) {
        contenedorListaEmpleadosPlanilla.innerHTML = `
            <div id="mensajeListaVacia" class="estado-lista-vacia">
                No hay colaboradores disponibles en este departamento.
            </div>
        `;
        return;
    }

    contenedorListaEmpleadosPlanilla.innerHTML = empleadosDisponiblesDepartamento.map(emp => {
        const id = Number(emp.idEmpleado);
        const estaSeleccionado = empleadosSeleccionadosPlanilla.has(id);
        const claseSeleccionado = estaSeleccionado ? "item-empleado-checkbox--seleccionado" : "";

        return `
            <label class="item-empleado-checkbox ${claseSeleccionado}" data-id="${id}">
                <div class="item-empleado-checkbox__datos">
                    <input type="checkbox" class="chk-empleado-planilla" value="${id}" ${estaSeleccionado ? "checked" : ""}>
                    <div class="item-empleado-checkbox__info">
                        <strong>${escaparHtml(emp.nombreCompleto)} (${escaparHtml(emp.codigoEmpleado)})</strong>
                        <small>${escaparHtml(emp.nombreCargo || "Colaborador")}</small>
                    </div>
                </div>
                <div class="item-empleado-checkbox__salario">
                    Salario Base: ${formatearMoneda(emp.salarioBase)}
                </div>
            </label>
        `;
    }).join("");
}

function manejarToggleTodosEmpleados() {
    const seleccionar = chkSeleccionarTodosEmpleados.checked;

    if (seleccionar) {
        empleadosDisponiblesDepartamento.forEach(emp => {
            empleadosSeleccionadosPlanilla.add(Number(emp.idEmpleado));
        });
    } else {
        empleadosSeleccionadosPlanilla.clear();
    }

    botonGuardarPlanilla.disabled = (empleadosSeleccionadosPlanilla.size === 0);
    renderizarListaEmpleadosDepartamento();
    actualizarContadorSeleccionados();
}

function manejarCambioCheckboxEmpleado(event) {
    const checkbox = event.target.closest(".chk-empleado-planilla");
    if (!checkbox) return;

    const idEmpleado = Number(checkbox.value);
    const itemLabel = checkbox.closest(".item-empleado-checkbox");

    if (checkbox.checked) {
        empleadosSeleccionadosPlanilla.add(idEmpleado);
        itemLabel?.classList.add("item-empleado-checkbox--seleccionado");
    } else {
        empleadosSeleccionadosPlanilla.delete(idEmpleado);
        itemLabel?.classList.remove("item-empleado-checkbox--seleccionado");
    }

    chkSeleccionarTodosEmpleados.checked = (
        empleadosDisponiblesDepartamento.length > 0 &&
        empleadosSeleccionadosPlanilla.size === empleadosDisponiblesDepartamento.length
    );

    botonGuardarPlanilla.disabled = (empleadosSeleccionadosPlanilla.size === 0);
    actualizarContadorSeleccionados();
}

function actualizarContadorSeleccionados() {
    const total = empleadosSeleccionadosPlanilla.size;
    contadorEmpleadosSeleccionados.textContent = `${total} ${total === 1 ? "colaborador seleccionado" : "colaboradores seleccionados"}`;
}

function manejarCambioFechaInicio() {
    inputFechaFin.min = inputFechaInicio.value || "";

    if (
        inputFechaInicio.value &&
        (!inputFechaFin.value || inputFechaFin.value < inputFechaInicio.value)
    ) {
        inputFechaFin.value = inputFechaInicio.value;
    }
}

async function guardarPlanilla(event) {
    event.preventDefault();
    limpiarMensajeFormulario();

    const idDepartamento = Number(selectDepartamentoPlanilla.value || 0);
    const idAdministrador = obtenerIdAdministradorActual();

    if (empleadosSeleccionadosPlanilla.size === 0) {
        mostrarMensajeFormulario("Debe seleccionar al menos un colaborador para generar la planilla.");
        return;
    }

    const datos = {
        idDepartamento,
        idAdministrador,
        fechaInicioPeriodo: inputFechaInicio.value,
        fechaFinPeriodo: inputFechaFin.value,
        idsEmpleadosSeleccionados: Array.from(empleadosSeleccionadosPlanilla)
    };

    const errorValidacion = validarPlanilla(datos);
    if (errorValidacion) {
        mostrarMensajeFormulario(errorValidacion);
        return;
    }

    bloquearFormularioPlanilla(true);

    try {
        await generarPlanilla(datos);
        cerrarModalPlanilla();
        mostrarMensajePrincipal("Planilla generada correctamente.", "exito");
        await cargarPlanillas(false);
    } catch (error) {
        console.error("Error al generar planilla:", error);
        mostrarMensajeFormulario(error.message || "No fue posible generar la planilla.");
    } finally {
        bloquearFormularioPlanilla(false);
    }
}

function validarPlanilla(datos) {
    if (!Number.isInteger(datos.idDepartamento) || datos.idDepartamento <= 0) {
        return "Debe seleccionar un departamento.";
    }

    if (!Number.isInteger(datos.idAdministrador) || datos.idAdministrador <= 0) {
        return "No se pudo identificar al administrador. Cierre sesión e inicie nuevamente.";
    }

    if (!datos.fechaInicioPeriodo) {
        return "Debe seleccionar la fecha inicial del período.";
    }

    if (!datos.fechaFinPeriodo) {
        return "Debe seleccionar la fecha final del período.";
    }

    if (datos.fechaFinPeriodo < datos.fechaInicioPeriodo) {
        return "La fecha final no puede ser anterior a la fecha inicial.";
    }

    if (!datos.idsEmpleadosSeleccionados || datos.idsEmpleadosSeleccionados.length === 0) {
        return "Debe seleccionar al menos un colaborador para generar la planilla.";
    }

    const existeDuplicada = planillas.some(
        planilla =>
            Number(planilla.idDepartamento) === Number(datos.idDepartamento) &&
            planilla.fechaInicioPeriodo === datos.fechaInicioPeriodo &&
            planilla.fechaFinPeriodo === datos.fechaFinPeriodo
    );

    if (existeDuplicada) {
        return "Ya existe una planilla para ese departamento y periodo.";
    }

    return null;
}

function bloquearFormularioPlanilla(bloqueado) {
    const elementos = formPlanilla.querySelectorAll("input, select, button");
    elementos.forEach(elemento => {
        elemento.disabled = bloqueado;
    });

    botonCerrarModalPlanilla.disabled = bloqueado;
    botonCancelarPlanilla.disabled = bloqueado;
    botonGuardarPlanilla.textContent = bloqueado ? "Generando..." : "Generar planilla";
}

function cerrarModalPlanilla() {
    ocultarModal(modalPlanilla);
    formPlanilla.reset();
    empleadosDisponiblesDepartamento = [];
    empleadosSeleccionadosPlanilla.clear();
    chkSeleccionarTodosEmpleados.checked = false;
    chkSeleccionarTodosEmpleados.disabled = true;
    botonGuardarPlanilla.disabled = true;
    inputFechaFin.min = "";
    limpiarMensajeFormulario();
}

// -------------------------------------------------------------
// Modal Detalle de Planilla
// -------------------------------------------------------------
async function abrirDetallePlanilla(idPlanilla) {
    contenidoDetallePlanilla.innerHTML = `
        <div class="estado-tabla">
            Cargando detalle...
        </div>
    `;

    mostrarModal(modalDetalle);

    try {
        const planilla = await obtenerPlanillaPorId(idPlanilla);
        if (!planilla) {
            throw new Error("No se encontró la planilla.");
        }

        renderizarDetallePlanilla(planilla);
    } catch (error) {
        console.error("Error al cargar detalle de planilla:", error);
        contenidoDetallePlanilla.innerHTML = `
            <div class="mensaje mensaje--error mensaje--modal">
                ${escaparHtml(error.message || "No fue posible cargar el detalle.")}
            </div>
        `;
    }
}

function renderizarDetallePlanilla(planilla) {
    const estado = obtenerPresentacionEstado(planilla.estado);

    const filasDetalles = (planilla.detalles && planilla.detalles.length > 0)
        ? planilla.detalles.map(d => `
            <tr>
                <td>
                    <span class="empleado-nombre">${escaparHtml(d.nombreEmpleado)}</span>
                    <span class="empleado-codigo">${escaparHtml(d.codigoEmpleado)}</span>
                </td>
                <td>${escaparHtml(d.cargo || "N/A")}</td>
                <td class="monto">${formatearMoneda(d.salarioBase)}</td>
                <td>${d.cantidadTardanzas} (${d.minutosTardanza} min)</td>
                <td>${d.minutosExtrasAprobados} min (${formatearMoneda(d.montoHorasExtras)})</td>
                <td class="monto">${formatearMoneda(d.totalIngresos)}</td>
                <td class="monto">${formatearMoneda(d.totalDeducciones)}</td>
                <td class="monto monto--neto">${formatearMoneda(d.salarioNeto)}</td>
            </tr>
        `).join("")
        : `
            <tr>
                <td colspan="8" style="text-align: center; padding: 20px; color: #666;">
                    Sin registros de colaboradores en el detalle.
                </td>
            </tr>
        `;

    contenidoDetallePlanilla.innerHTML = `
        <div class="detalle-cabecera">
            <div class="detalle-dato">
                <span>Departamento</span>
                <strong>${escaparHtml(planilla.nombreDepartamento || planilla.nombreEmpleado || "Planilla")}</strong>
            </div>

            <div class="detalle-dato">
                <span>Colaboradores</span>
                <strong>${planilla.cantidadEmpleados || (planilla.detalles ? planilla.detalles.length : 0)}</strong>
            </div>

            <div class="detalle-dato">
                <span>Período</span>
                <strong>${formatearFecha(planilla.fechaInicioPeriodo)} al ${formatearFecha(planilla.fechaFinPeriodo)}</strong>
            </div>

            <div class="detalle-dato">
                <span>Estado</span>
                <strong><span class="${estado.clase}">${escaparHtml(estado.nombre)}</span></strong>
            </div>

            <div class="detalle-dato">
                <span>Fecha de creación</span>
                <strong>${formatearFechaHora(planilla.fechaCreacion || planilla.fechaGeneracion)}</strong>
            </div>
        </div>

        <div class="detalle-totales">
            <div class="detalle-total">
                <span>Salario base</span>
                <strong>${formatearMoneda(planilla.salarioBasePeriodo)}</strong>
            </div>

            <div class="detalle-total">
                <span>Ingresos adicionales</span>
                <strong>${formatearMoneda(planilla.ingresosAdicionales)}</strong>
            </div>

            <div class="detalle-total">
                <span>Total deducciones</span>
                <strong>${formatearMoneda(planilla.totalDeducciones)}</strong>
            </div>

            <div class="detalle-total detalle-total--neto">
                <span>Salario neto</span>
                <strong>${formatearMoneda(planilla.salarioNeto)}</strong>
            </div>
        </div>

        <div class="detalle-deducciones">
            <h4>Colaboradores incluidos (${planilla.cantidadEmpleados || (planilla.detalles ? planilla.detalles.length : 0)})</h4>
            <div class="detalle-tabla-contenedor">
                <table class="detalle-tabla">
                    <thead>
                        <tr>
                            <th>Colaborador</th>
                            <th>Cargo</th>
                            <th>Salario base</th>
                            <th>Tardanzas</th>
                            <th>H. Extras</th>
                            <th>Ingresos</th>
                            <th>Deducciones</th>
                            <th>Salario neto</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${filasDetalles}
                    </tbody>
                </table>
            </div>
        </div>
    `;
}

function cerrarModalDetalle() {
    ocultarModal(modalDetalle);
    contenidoDetallePlanilla.innerHTML = "";
}

// -------------------------------------------------------------
// Modal Cambiar Estado
// -------------------------------------------------------------
function abrirModalEstado(idPlanilla) {
    const planilla = planillas.find(item => Number(item.idPlanilla) === Number(idPlanilla));
    if (!planilla) return;

    inputIdPlanillaEstado.value = planilla.idPlanilla;
    selectNuevoEstado.value = String(planilla.estado);

    const tituloPrincipal = planilla.nombreDepartamento || planilla.nombreEmpleado || "Planilla";
    descripcionCambioEstado.textContent = `${tituloPrincipal} · ${formatearFecha(planilla.fechaInicioPeriodo)} al ${formatearFecha(planilla.fechaFinPeriodo)}`;

    limpiarMensajeEstado();
    mostrarModal(modalEstado);
    selectNuevoEstado.focus();
}

async function guardarCambioEstado(event) {
    event.preventDefault();
    limpiarMensajeEstado();

    const idPlanilla = Number(inputIdPlanillaEstado.value);
    const estado = Number(selectNuevoEstado.value);
    const planilla = planillas.find(item => Number(item.idPlanilla) === idPlanilla);

    if (!Number.isInteger(idPlanilla) || idPlanilla <= 0) {
        mostrarMensajeEstado("La planilla seleccionada no es válida.");
        return;
    }

    if (!Number.isInteger(estado) || !ESTADOS_PLANILLA[estado]) {
        mostrarMensajeEstado("Debe seleccionar un estado válido.");
        return;
    }

    if (planilla && Number(planilla.estado) === estado) {
        mostrarMensajeEstado("La planilla ya tiene el estado seleccionado.");
        return;
    }

    bloquearFormularioEstado(true);

    try {
        await cambiarEstadoPlanilla(idPlanilla, estado);
        cerrarModalEstado();
        mostrarMensajePrincipal("Estado de la planilla actualizado correctamente.", "exito");
        await cargarPlanillas(false);
    } catch (error) {
        console.error("Error al cambiar estado:", error);
        mostrarMensajeEstado(error.message || "No fue posible cambiar el estado.");
    } finally {
        bloquearFormularioEstado(false);
    }
}

function bloquearFormularioEstado(bloqueado) {
    selectNuevoEstado.disabled = bloqueado;
    botonCerrarModalEstado.disabled = bloqueado;
    botonCancelarEstado.disabled = bloqueado;
    botonGuardarEstado.disabled = bloqueado;
    botonGuardarEstado.textContent = bloqueado ? "Guardando..." : "Guardar estado";
}

function cerrarModalEstado() {
    ocultarModal(modalEstado);
    formEstado.reset();
    inputIdPlanillaEstado.value = "";
    limpiarMensajeEstado();
}

// -------------------------------------------------------------
// Utilidades de Modal y Teclado
// -------------------------------------------------------------
function mostrarModal(modal) {
    modal.classList.remove("modal--oculto");
    modal.setAttribute("aria-hidden", "false");
    document.body.style.overflow = "hidden";
}

function ocultarModal(modal) {
    modal.classList.add("modal--oculto");
    modal.setAttribute("aria-hidden", "true");

    const existeModalAbierto = [modalPlanilla, modalDetalle, modalEstado].some(
        elemento => !elemento.classList.contains("modal--oculto")
    );

    if (!existeModalAbierto) {
        document.body.style.overflow = "";
    }
}

function manejarTeclaEscape(event) {
    if (event.key !== "Escape") return;

    if (!modalPlanilla.classList.contains("modal--oculto")) {
        cerrarModalPlanilla();
        return;
    }

    if (!modalDetalle.classList.contains("modal--oculto")) {
        cerrarModalDetalle();
        return;
    }

    if (!modalEstado.classList.contains("modal--oculto")) {
        cerrarModalEstado();
    }
}

// -------------------------------------------------------------
// Mensajes y Resumen
// -------------------------------------------------------------
function actualizarResumenListado(lista) {
    contadorPlanillas.textContent = lista.length === 1 ? "1 planilla" : `${lista.length} planillas`;

    const totalSalarioNeto = lista.reduce(
        (total, planilla) => total + convertirNumeroNoNegativo(planilla.salarioNeto),
        0
    );

    totalNetoVisible.textContent = `Total neto: ${formatearMoneda(totalSalarioNeto)}`;
}

function mostrarCargando(cargando) {
    estadoCarga.classList.toggle("estado-tabla--oculto", !cargando);

    if (cargando) {
        contenedorTabla.classList.add("tabla-contenedor--oculto");
        sinResultados.classList.add("estado-tabla--oculto");
    }
}

function mostrarMensajePrincipal(mensaje, tipo) {
    clearTimeout(temporizadorMensaje);
    mensajePlanillas.textContent = mensaje;
    mensajePlanillas.className = tipo === "exito" ? "mensaje mensaje--exito" : "mensaje mensaje--error";

    temporizadorMensaje = setTimeout(ocultarMensajePrincipal, 4000);
}

function ocultarMensajePrincipal() {
    clearTimeout(temporizadorMensaje);
    mensajePlanillas.textContent = "";
    mensajePlanillas.className = "mensaje mensaje--oculto";
}

function mostrarMensajeFormulario(mensaje) {
    mensajeFormulario.textContent = mensaje;
    mensajeFormulario.className = "mensaje mensaje--error mensaje--modal";
}

function limpiarMensajeFormulario() {
    mensajeFormulario.textContent = "";
    mensajeFormulario.className = "mensaje mensaje--oculto mensaje--modal";
}

function mostrarMensajeEstado(mensaje) {
    mensajeEstado.textContent = mensaje;
    mensajeEstado.className = "mensaje mensaje--error mensaje--modal";
}

function limpiarMensajeEstado() {
    mensajeEstado.textContent = "";
    mensajeEstado.className = "mensaje mensaje--oculto mensaje--modal";
}

// -------------------------------------------------------------
// Autenticación y Helper de Admin
// -------------------------------------------------------------
function obtenerIdAdministradorActual() {
    const administrador = obtenerAdministrador();

    const idSesion = Number(
        administrador?.idAdministrador ??
        administrador?.IdAdministrador ??
        administrador?.id ??
        administrador?.Id
    );

    if (Number.isInteger(idSesion) && idSesion > 0) {
        return idSesion;
    }

    const payload = obtenerPayloadToken(obtenerToken());
    const idToken = Number(
        payload?.idAdministrador ??
        payload?.IdAdministrador ??
        payload?.administradorId ??
        payload?.AdministradorId ??
        payload?.nameid ??
        payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ??
        payload?.sub
    );

    return Number.isInteger(idToken) && idToken > 0 ? idToken : 0;
}

function obtenerPayloadToken(token) {
    if (!token) return null;

    try {
        const partes = token.split(".");
        if (partes.length < 2) return null;

        const base64 = partes[1].replace(/-/g, "+").replace(/_/g, "/");
        const contenido = decodeURIComponent(
            atob(base64)
                .split("")
                .map(caracter => "%" + caracter.charCodeAt(0).toString(16).padStart(2, "0"))
                .join("")
        );

        return JSON.parse(contenido);
    } catch (error) {
        console.error("No se pudo leer el token:", error);
        return null;
    }
}

// -------------------------------------------------------------
// Formateadores y Utilitarios
// -------------------------------------------------------------
function obtenerPeriodoMensualActual() {
    const fechaActual = new Date();
    const fechaInicio = new Date(fechaActual.getFullYear(), fechaActual.getMonth(), 1);
    const fechaFin = new Date(fechaActual.getFullYear(), fechaActual.getMonth() + 1, 0);

    return {
        inicio: convertirFechaInput(fechaInicio),
        fin: convertirFechaInput(fechaFin)
    };
}

function convertirFechaInput(fecha) {
    const anio = fecha.getFullYear();
    const mes = String(fecha.getMonth() + 1).padStart(2, "0");
    const dia = String(fecha.getDate()).padStart(2, "0");
    return `${anio}-${mes}-${dia}`;
}

function formatearFecha(fecha) {
    if (!fecha) return "Sin fecha";
    const partes = String(fecha).slice(0, 10).split("-");
    if (partes.length !== 3) return fecha;
    return `${partes[2]}/${partes[1]}/${partes[0]}`;
}

function formatearFechaHora(valor) {
    if (!valor) return "Sin información";
    const fecha = new Date(valor);
    if (Number.isNaN(fecha.getTime())) return valor;

    return fecha.toLocaleString("es-NI", {
        dateStyle: "short",
        timeStyle: "short"
    });
}

function formatearMoneda(valor) {
    const numero = Number(valor);
    const monto = Number.isFinite(numero) ? numero : 0;

    return new Intl.NumberFormat("es-NI", {
        style: "currency",
        currency: "NIO",
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }).format(monto);
}

function convertirNumeroNoNegativo(valor) {
    const numero = Number(valor);
    if (!Number.isFinite(numero) || numero < 0) return 0;
    return numero;
}

function obtenerPresentacionEstado(estado) {
    return ESTADOS_PLANILLA[Number(estado)] ?? {
        nombre: `Estado ${estado}`,
        clase: "estado estado--desconocido"
    };
}

function escaparHtml(texto) {
    const elemento = document.createElement("div");
    elemento.textContent = texto ?? "";
    return elemento.innerHTML;
}

function escaparAtributo(texto) {
    return String(texto ?? "")
        .replace(/&/g, "&amp;")
        .replace(/"/g, "&quot;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
}