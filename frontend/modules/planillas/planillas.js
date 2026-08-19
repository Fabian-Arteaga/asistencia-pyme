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
    obtenerTiposDeduccionParaPlanilla,
    recalcularPlanilla,
    enviarPlanillaRevision,
    cerrarPlanilla,
    pagarPlanilla,
    anularPlanilla,
    obtenerDepartamentos
} from "./planillas.services.js";


const ESTADOS_PLANILLA = {
    1: {
        nombre: "Generada",
        clase: "estado estado--generada"
    },

    2: {
        nombre: "Pagada",
        clase: "estado estado--pagada"
    },

    3: {
        nombre: "Anulada",
        clase: "estado estado--anulada"
    }
};


const TIPO_PORCENTAJE = 1;
const TIPO_MONTO_FIJO = 2;


const obtenerElemento =
    id => document.getElementById(id);

const botonNuevaPlanilla =
    obtenerElemento(
        "btnNuevaPlanilla"
    );

const botonActualizar =
    obtenerElemento(
        "btnActualizar"
    );

const botonLimpiarFiltros =
    obtenerElemento(
        "btnLimpiarFiltros"
    );

const filtroEmpleado =
    obtenerElemento(
        "filtroEmpleado"
    );

const filtroDesde =
    obtenerElemento(
        "filtroDesde"
    );

const filtroHasta =
    obtenerElemento(
        "filtroHasta"
    );

const filtroEstado =
    obtenerElemento(
        "filtroEstado"
    );

const contadorPlanillas =
    obtenerElemento(
        "contadorPlanillas"
    );

const totalNetoVisible =
    obtenerElemento(
        "totalNetoVisible"
    );

const tablaPlanillasBody =
    obtenerElemento(
        "tablaPlanillasBody"
    );

const contenedorTabla =
    obtenerElemento(
        "contenedorTabla"
    );

const estadoCarga =
    obtenerElemento(
        "estadoCarga"
    );

const sinResultados =
    obtenerElemento(
        "sinResultados"
    );

const mensajePlanillas =
    obtenerElemento(
        "mensajePlanillas"
    );


const modalPlanilla =
    obtenerElemento(
        "modalPlanilla"
    );

const fondoModalPlanilla =
    obtenerElemento(
        "fondoModalPlanilla"
    );

const formPlanilla =
    obtenerElemento(
        "formPlanilla"
    );

const selectDepartamentoPlanilla =
    obtenerElemento(
        "departamentoPlanilla"
    );

const inputFechaInicio =
    obtenerElemento(
        "fechaInicioPeriodo"
    );

const inputFechaFin =
    obtenerElemento(
        "fechaFinPeriodo"
    );

const inputSalarioBase =
    obtenerElemento(
        "salarioBaseEmpleado"
    );

const inputIngresos =
    obtenerElemento(
        "ingresosAdicionales"
    );

const botonAgregarDeduccion =
    obtenerElemento(
        "btnAgregarDeduccion"
    );

const listaDeducciones =
    obtenerElemento(
        "listaDeducciones"
    );

const sinDeducciones =
    obtenerElemento(
        "sinDeducciones"
    );

const resumenSalarioBase =
    obtenerElemento(
        "resumenSalarioBase"
    );

const resumenIngresos =
    obtenerElemento(
        "resumenIngresos"
    );

const resumenSalarioBruto =
    obtenerElemento(
        "resumenSalarioBruto"
    );

const resumenDeducciones =
    obtenerElemento(
        "resumenDeducciones"
    );

const resumenSalarioNeto =
    obtenerElemento(
        "resumenSalarioNeto"
    );

const mensajeFormulario =
    obtenerElemento(
        "mensajeFormulario"
    );

const botonCerrarModalPlanilla =
    obtenerElemento(
        "btnCerrarModalPlanilla"
    );

const botonCancelarPlanilla =
    obtenerElemento(
        "btnCancelarPlanilla"
    );

const botonGuardarPlanilla =
    obtenerElemento(
        "btnGuardarPlanilla"
    );


const modalDetalle =
    obtenerElemento(
        "modalDetalle"
    );

const fondoModalDetalle =
    obtenerElemento(
        "fondoModalDetalle"
    );

const contenidoDetallePlanilla =
    obtenerElemento(
        "contenidoDetallePlanilla"
    );

const botonCerrarModalDetalle =
    obtenerElemento(
        "btnCerrarModalDetalle"
    );

const botonCerrarDetalle =
    obtenerElemento(
        "btnCerrarDetalle"
    );


const modalEstado =
    obtenerElemento(
        "modalEstado"
    );

const fondoModalEstado =
    obtenerElemento(
        "fondoModalEstado"
    );

const formEstado =
    obtenerElemento(
        "formEstado"
    );

const inputIdPlanillaEstado =
    obtenerElemento(
        "idPlanillaEstado"
    );

const descripcionCambioEstado =
    obtenerElemento(
        "descripcionCambioEstado"
    );

const selectNuevoEstado =
    obtenerElemento(
        "nuevoEstadoPlanilla"
    );

const mensajeEstado =
    obtenerElemento(
        "mensajeEstado"
    );

const botonCerrarModalEstado =
    obtenerElemento(
        "btnCerrarModalEstado"
    );

const botonCancelarEstado =
    obtenerElemento(
        "btnCancelarEstado"
    );

const botonGuardarEstado =
    obtenerElemento(
        "btnGuardarEstado"
    );


let planillas = [];
let empleados = [];
let tiposDeduccion = [];
let deduccionesFormulario = [];

let consecutivoDeduccion = 1;
let temporizadorMensaje = null;


document.addEventListener(
    "DOMContentLoaded",
    inicializarModulo
);

async function inicializarModulo() {
    const accesoPermitido =
        protegerPagina();

    if (!accesoPermitido) {
        return;
    }

    inicializarLayout({
        titulo: "Planillas",
        paginaActiva: "planillas"
    });

    configurarEventos();

    await Promise.all([
        cargarDepartamentos(),
        cargarTiposDeduccion()
    ]);

    await cargarPlanillas();
}

function configurarEventos() {
    botonNuevaPlanilla.addEventListener(
        "click",
        abrirModalNuevaPlanilla
    );

    botonActualizar.addEventListener(
        "click",
        () => cargarPlanillas()
    );

    botonLimpiarFiltros.addEventListener(
        "click",
        limpiarFiltros
    );

    filtroEmpleado.addEventListener(
        "change",
        aplicarFiltros
    );

    filtroDesde.addEventListener(
        "change",
        aplicarFiltros
    );

    filtroHasta.addEventListener(
        "change",
        aplicarFiltros
    );

    filtroEstado.addEventListener(
        "change",
        aplicarFiltros
    );

    tablaPlanillasBody.addEventListener(
        "click",
        manejarAccionTabla
    );

    formPlanilla.addEventListener(
        "submit",
        guardarPlanilla
    );

    selectDepartamentoPlanilla.addEventListener(
        "change",
            manejarCambioDepartamento
    );

    inputFechaInicio.addEventListener(
        "change",
        manejarCambioFechaInicio
    );

    inputFechaFin.addEventListener(
        "change",
        recalcularResumen
    );

    inputIngresos.addEventListener(
        "input",
        recalcularResumen
    );

    botonAgregarDeduccion.addEventListener(
        "click",
        agregarDeduccion
    );

    listaDeducciones.addEventListener(
        "change",
        manejarCambioDeduccion
    );

    listaDeducciones.addEventListener(
        "input",
        manejarCambioDeduccion
    );

    listaDeducciones.addEventListener(
        "click",
        manejarClickDeduccion
    );

    botonCerrarModalPlanilla.addEventListener(
        "click",
        cerrarModalPlanilla
    );

    botonCancelarPlanilla.addEventListener(
        "click",
        cerrarModalPlanilla
    );

    fondoModalPlanilla.addEventListener(
        "click",
        cerrarModalPlanilla
    );

    botonCerrarModalDetalle.addEventListener(
        "click",
        cerrarModalDetalle
    );

    botonCerrarDetalle.addEventListener(
        "click",
        cerrarModalDetalle
    );

    fondoModalDetalle.addEventListener(
        "click",
        cerrarModalDetalle
    );

    formEstado.addEventListener(
        "submit",
        guardarCambioEstado
    );

    botonCerrarModalEstado.addEventListener(
        "click",
        cerrarModalEstado
    );

    botonCancelarEstado.addEventListener(
        "click",
        cerrarModalEstado
    );

    fondoModalEstado.addEventListener(
        "click",
        cerrarModalEstado
    );

    document.addEventListener(
        "keydown",
        manejarTeclaEscape
    );
}


async function cargarEmpleados() {
    try {
        empleados =
            await obtenerEmpleadosParaPlanilla();

        empleados.sort(
            (a, b) =>
                a.nombreCompleto.localeCompare(
                    b.nombreCompleto,
                    "es"
                )
        );

        llenarSelectEmpleados();
    } catch (error) {
        console.error(error);

        empleados = [];

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cargar los empleados.",
            "error"
        );
    }
}

function llenarSelectEmpleados() {
    const opciones =
        empleados.map(
            empleado => `
                <option
                    value="${escaparAtributo(
                        empleado.codigoEmpleado
                    )}"
                >
                    ${escaparHtml(
                        empleado.codigoEmpleado
                    )} - ${escaparHtml(
                        empleado.nombreCompleto
                    )}
                </option>
            `
        ).join("");

    filtroEmpleado.innerHTML = `
        <option value="">
            Todos los empleados
        </option>

        ${opciones}
    `;

    selectDepartamentoPlanilla.innerHTML = `
        <option value="">
            Seleccione un empleado
        </option>

        ${opciones}
    `;
}


let departamentos = [];

async function cargarDepartamentos() {
    try {
        departamentos = await obtenerDepartamentos();

        departamentos.sort((a, b) => (a.nombre || '').localeCompare(b.nombre || '', 'es'));

        const opciones = departamentos.map(d => `
            <option value="${d.idDepartamento}">${d.nombre}</option>
        `).join('');

        const selectDepartamento = document.getElementById('departamentoPlanilla');
        if (selectDepartamento) {
            selectDepartamento.innerHTML = `
                <option value="">Seleccione un departamento</option>
                ${opciones}
            `;
        }
    } catch (error) {
        console.error(error);
    }
}

async function cargarTiposDeduccion() {
    try {
        tiposDeduccion =
            await obtenerTiposDeduccionParaPlanilla();

        tiposDeduccion.sort(
            (a, b) =>
                a.nombre.localeCompare(
                    b.nombre,
                    "es"
                )
        );
    } catch (error) {
        console.error(error);

        tiposDeduccion = [];

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cargar los tipos de deducción.",
            "error"
        );
    }
}


async function cargarPlanillas(
    limpiarMensaje = true
) {
    mostrarCargando(true);

    if (limpiarMensaje) {
        ocultarMensajePrincipal();
    }

    botonActualizar.disabled = true;
    botonActualizar.textContent =
        "Actualizando...";

    try {
        planillas =
            await obtenerPlanillas();

        planillas.sort(
            (a, b) =>
                b.fechaInicioPeriodo.localeCompare(
                    a.fechaInicioPeriodo
                )
        );

        aplicarFiltros();
    } catch (error) {
        console.error(error);

        planillas = [];

        renderizarPlanillas([]);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cargar las planillas.",
            "error"
        );
    } finally {
        mostrarCargando(false);

        botonActualizar.disabled = false;
        botonActualizar.textContent =
            "Actualizar";
    }
}


function aplicarFiltros() {
    const codigoEmpleado =
        filtroEmpleado.value;

    const fechaDesde =
        filtroDesde.value;

    const fechaHasta =
        filtroHasta.value;

    const estado =
        filtroEstado.value;

    const resultados =
        planillas.filter(
            planilla => {
                const coincideEmpleado =
                    !codigoEmpleado ||
                    planilla.codigoEmpleado ===
                    codigoEmpleado;

                const coincideDesde =
                    !fechaDesde ||
                    planilla.fechaFinPeriodo >=
                    fechaDesde;

                const coincideHasta =
                    !fechaHasta ||
                    planilla.fechaInicioPeriodo <=
                    fechaHasta;

                const coincideEstado =
                    !estado ||
                    Number(planilla.estado) ===
                    Number(estado);

                return (
                    coincideEmpleado &&
                    coincideDesde &&
                    coincideHasta &&
                    coincideEstado
                );
            }
        );

    actualizarResumenListado(
        resultados
    );

    renderizarPlanillas(
        resultados
    );
}

function limpiarFiltros() {
    filtroEmpleado.value = "";
    filtroDesde.value = "";
    filtroHasta.value = "";
    filtroEstado.value = "";

    aplicarFiltros();
}

function renderizarPlanillas(
    lista
) {
    tablaPlanillasBody.innerHTML =
        "";

    if (lista.length === 0) {
        contenedorTabla.classList.add(
            "tabla-contenedor--oculto"
        );

        sinResultados.classList.remove(
            "estado-tabla--oculto"
        );

        return;
    }

    contenedorTabla.classList.remove(
        "tabla-contenedor--oculto"
    );

    sinResultados.classList.add(
        "estado-tabla--oculto"
    );

    lista.forEach(
        planilla => {
            const estado =
                obtenerPresentacionEstado(
                    planilla.estado
                );

            const fila =
                document.createElement("tr");

            fila.innerHTML = `
                <td>
                    <span class="empleado-nombre">
                        ${escaparHtml(
                            planilla.nombreEmpleado
                        )}
                    </span>

                    <span class="empleado-codigo">
                        ${escaparHtml(
                            planilla.codigoEmpleado
                        )}
                    </span>
                </td>

                <td>
                    ${formatearFecha(
                        planilla.fechaInicioPeriodo
                    )}
                    <br>
                    al
                    ${formatearFecha(
                        planilla.fechaFinPeriodo
                    )}
                </td>

                <td class="monto">
                    ${formatearMoneda(
                        planilla.salarioBasePeriodo
                    )}
                </td>

                <td class="monto">
                    ${formatearMoneda(
                        planilla.ingresosAdicionales
                    )}
                </td>

                <td class="monto">
                    ${formatearMoneda(
                        planilla.totalDeducciones
                    )}
                </td>

                <td class="monto monto--neto">
                    ${formatearMoneda(
                        planilla.salarioNeto
                    )}
                </td>

                <td>
                    <span class="${estado.clase}">
                        ${escaparHtml(
                            estado.nombre
                        )}
                    </span>
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

            tablaPlanillasBody.appendChild(
                fila
            );
        }
    );
}

function manejarAccionTabla(
    event
) {
    const boton =
        event.target.closest(
            "[data-accion]"
        );

    if (!boton) {
        return;
    }

    const idPlanilla =
        Number(
            boton.dataset.id
        );

    if (!Number.isInteger(idPlanilla)) {
        return;
    }

    const accion =
        boton.dataset.accion;

    if (accion === "detalle") {
        abrirDetallePlanilla(
            idPlanilla
        );

        return;
    }

    if (accion === "estado") {
        abrirModalEstado(
            idPlanilla
        );
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
    const administrador = obtenerAdministrador();
    const idAdministrador = administrador?.idAdministrador ?? administrador?.IdAdministrador ?? 0;

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
            await anularPlanilla(idPlanilla, idAdministrador, "Anulación desde el frontend");
        }

        mostrarMensajePrincipal("Cambio de estado aplicado correctamente.", "success");
        await cargarPlanillas(false);
    } catch (error) {
        console.error(error);
        mostrarMensajePrincipal(error?.message || "No fue posible ejecutar la acción requerida.", "error");
    }
}


function abrirModalNuevaPlanilla() {
    formPlanilla.reset();

    deduccionesFormulario = [];
    consecutivoDeduccion = 1;

    const periodo =
        obtenerPeriodoMensualActual();

    inputFechaInicio.value =
        periodo.inicio;

    inputFechaFin.value =
        periodo.fin;

    inputFechaFin.min =
        periodo.inicio;

    inputIngresos.value =
        "0";

    inputSalarioBase.value =
        formatearMoneda(0);

    limpiarMensajeFormulario();
    renderizarDeduccionesFormulario();
    recalcularResumen();

    mostrarModal(
        modalPlanilla
    );

    selectDepartamentoPlanilla.focus();
}

function manejarCambioDepartamento() {
    const departamento = obtenerDepartamentoSeleccionado();

    // No salary preview for department-level generation. Just ensure dates are valid.
    if (departamento) {
        // Potential future behavior: adjust defaults based on department
    }

    recalcularResumen();
}

function manejarCambioFechaInicio() {
    inputFechaFin.min =
        inputFechaInicio.value ||
        "";

    if (
        inputFechaInicio.value &&
        (
            !inputFechaFin.value ||
            inputFechaFin.value <
            inputFechaInicio.value
        )
    ) {
        inputFechaFin.value =
            inputFechaInicio.value;
    }

    recalcularResumen();
}


function agregarDeduccion() {
    if (tiposDeduccion.length === 0) {
        mostrarMensajeFormulario(
            "No existen tipos de deducción activos."
        );

        return;
    }

    const idsUtilizados =
        new Set(
            deduccionesFormulario.map(
                item =>
                    Number(
                        item.idTipoDeduccion
                    )
            )
        );

    const tipoDisponible =
        tiposDeduccion.find(
            tipo =>
                !idsUtilizados.has(
                    Number(
                        tipo.idTipoDeduccion
                    )
                )
        );

    if (!tipoDisponible) {
        mostrarMensajeFormulario(
            "Ya agregó todos los tipos de deducción disponibles."
        );

        return;
    }

    deduccionesFormulario.push({
        idLocal:
            consecutivoDeduccion++,

        idTipoDeduccion:
            Number(
                tipoDisponible.idTipoDeduccion
            ),

        valorAplicado:
            Number(
                tipoDisponible.valorPredeterminado
            ),

        observacion:
            ""
    });

    limpiarMensajeFormulario();
    renderizarDeduccionesFormulario();
    recalcularResumen();
}

function manejarCambioDeduccion(
    event
) {
    const elemento =
        event.target.closest(
            "[data-deduccion-id]"
        );

    if (!elemento) {
        return;
    }

    const idLocal =
        Number(
            elemento.dataset.deduccionId
        );

    const deduccion =
        deduccionesFormulario.find(
            item =>
                item.idLocal ===
                idLocal
        );

    if (!deduccion) {
        return;
    }

    const campo =
        elemento.dataset.campo;

    if (
        campo ===
        "idTipoDeduccion"
    ) {
        deduccion.idTipoDeduccion =
            Number(
                elemento.value
            );

        const tipo =
            obtenerTipoDeduccion(
                deduccion.idTipoDeduccion
            );

        deduccion.valorAplicado =
            Number(
                tipo?.valorPredeterminado ??
                0
            );

        renderizarDeduccionesFormulario();
    }

    if (
        campo ===
        "valorAplicado"
    ) {
        deduccion.valorAplicado =
            convertirNumeroNoNegativo(
                elemento.value
            );
    }

    if (
        campo ===
        "observacion"
    ) {
        deduccion.observacion =
            elemento.value;
    }

    recalcularResumen();
}

function manejarClickDeduccion(
    event
) {
    const boton =
        event.target.closest(
            '[data-accion="quitar-deduccion"]'
        );

    if (!boton) {
        return;
    }

    const idLocal =
        Number(
            boton.dataset.deduccionId
        );

    deduccionesFormulario =
        deduccionesFormulario.filter(
            item =>
                item.idLocal !==
                idLocal
        );

    renderizarDeduccionesFormulario();
    recalcularResumen();
}

function renderizarDeduccionesFormulario() {
    listaDeducciones.innerHTML =
        "";

    sinDeducciones.style.display =
        deduccionesFormulario.length === 0
            ? "block"
            : "none";

    deduccionesFormulario.forEach(
        deduccion => {
            const tipoActual =
                obtenerTipoDeduccion(
                    deduccion.idTipoDeduccion
                );

            const elemento =
                document.createElement("div");

            elemento.className =
                "deduccion-item";

            elemento.innerHTML = `
                <div class="campo-formulario">
                    <label>
                        Tipo de deducción
                    </label>

                    <select
                        data-deduccion-id="${deduccion.idLocal}"
                        data-campo="idTipoDeduccion"
                    >
                        ${construirOpcionesTipos(
                            deduccion
                        )}
                    </select>

                    <span class="deduccion-ayuda">
                        ${escaparHtml(
                            obtenerTextoTipoCalculo(
                                tipoActual?.tipoCalculo ??
                                0
                            )
                        )}
                    </span>
                </div>

                <div class="campo-formulario">
                    <label>
                        Valor aplicado
                    </label>

                    <input
                        type="number"
                        min="0"
                        max="999999999.99"
                        step="0.01"
                        value="${deduccion.valorAplicado}"
                        data-deduccion-id="${deduccion.idLocal}"
                        data-campo="valorAplicado"
                    >
                </div>

                <div class="campo-formulario">
                    <label>
                        Observación
                    </label>

                    <input
                        type="text"
                        maxlength="300"
                        value="${escaparAtributo(
                            deduccion.observacion
                        )}"
                        placeholder="Opcional"
                        data-deduccion-id="${deduccion.idLocal}"
                        data-campo="observacion"
                    >
                </div>

                <button
                    type="button"
                    class="boton-quitar"
                    title="Quitar deducción"
                    data-accion="quitar-deduccion"
                    data-deduccion-id="${deduccion.idLocal}"
                >
                    ×
                </button>
            `;

            listaDeducciones.appendChild(
                elemento
            );
        }
    );
}

function construirOpcionesTipos(
    deduccionActual
) {
    const idsUtilizados =
        new Set(
            deduccionesFormulario
                .filter(
                    item =>
                        item.idLocal !==
                        deduccionActual.idLocal
                )
                .map(
                    item =>
                        Number(
                            item.idTipoDeduccion
                        )
                )
        );

    return tiposDeduccion
        .filter(
            tipo =>
                !idsUtilizados.has(
                    Number(
                        tipo.idTipoDeduccion
                    )
                ) ||
                Number(
                    tipo.idTipoDeduccion
                ) ===
                Number(
                    deduccionActual.idTipoDeduccion
                )
        )
        .map(
            tipo => `
                <option
                    value="${tipo.idTipoDeduccion}"
                    ${
                        Number(
                            tipo.idTipoDeduccion
                        ) ===
                        Number(
                            deduccionActual.idTipoDeduccion
                        )
                            ? "selected"
                            : ""
                    }
                >
                    ${escaparHtml(
                        tipo.nombre
                    )}
                </option>
            `
        )
        .join("");
}


function recalcularResumen() {
    const departamento = obtenerDepartamentoSeleccionado();

    // No per-employee salary preview in department mode
    const salarioBase = 0;

    const ingresosAdicionales =
        convertirNumeroNoNegativo(
            inputIngresos.value
        );

    const salarioBruto =
        salarioBase +
        ingresosAdicionales;

    const totalDeducciones =
        deduccionesFormulario.reduce(
            (
                total,
                deduccion
            ) =>
                total +
                calcularMontoDeduccion(
                    deduccion,
                    salarioBruto
                ),
            0
        );

    const salarioNeto =
        salarioBruto -
        totalDeducciones;

    resumenSalarioBase.textContent =
        formatearMoneda(
            salarioBase
        );

    resumenIngresos.textContent =
        formatearMoneda(
            ingresosAdicionales
        );

    resumenSalarioBruto.textContent =
        formatearMoneda(
            salarioBruto
        );

    resumenDeducciones.textContent =
        formatearMoneda(
            totalDeducciones
        );

    resumenSalarioNeto.textContent =
        formatearMoneda(
            salarioNeto
        );

    resumenSalarioNeto.style.color =
        salarioNeto < 0
            ? "#8a2e2e"
            : "";
}

function calcularMontoDeduccion(
    deduccion,
    salarioBruto
) {
    const tipo =
        obtenerTipoDeduccion(
            deduccion.idTipoDeduccion
        );

    const valorAplicado =
        convertirNumeroNoNegativo(
            deduccion.valorAplicado
        );

    if (!tipo) {
        return 0;
    }

    if (
        esTipoPorcentaje(
            tipo.tipoCalculo
        )
    ) {
        return (
            salarioBruto *
            valorAplicado /
            100
        );
    }

    return valorAplicado;
}

async function guardarPlanilla(
    event
) {
    event.preventDefault();

    limpiarMensajeFormulario();

    const departamento = obtenerDepartamentoSeleccionado();

    const datos = {
        idDepartamento: Number(selectDepartamentoPlanilla.value || 0),
        idAdministrador: obtenerIdAdministradorActual(),
        fechaInicioPeriodo: inputFechaInicio.value,
        fechaFinPeriodo: inputFechaFin.value,
        ingresosAdicionales: convertirNumeroNoNegativo(inputIngresos.value),

        deducciones:
            deduccionesFormulario.map(
                deduccion => ({
                    idTipoDeduccion:
                        Number(
                            deduccion.idTipoDeduccion
                        ),

                    valorAplicado:
                        convertirNumeroNoNegativo(
                            deduccion.valorAplicado
                        ),

                    observacion:
                        deduccion.observacion
                            .trim() ||
                        null
                })
            )
    };

    const errorValidacion =
        validarPlanilla(
            datos,
            empleado
        );

    if (errorValidacion) {
        mostrarMensajeFormulario(
            errorValidacion
        );

        return;
    }

    bloquearFormularioPlanilla(
        true
    );

    try {
        await generarPlanilla(
            datos
        );

        cerrarModalPlanilla();

        mostrarMensajePrincipal(
            "Planilla generada correctamente.",
            "exito"
        );

        await cargarPlanillas(
            false
        );
    } catch (error) {
        console.error(error);

        mostrarMensajeFormulario(
            error.message ||
            "No fue posible generar la planilla."
        );
    } finally {
        bloquearFormularioPlanilla(
            false
        );
    }
}

function validarPlanilla(
    datos,
    departamento
) {
    if (!Number.isInteger(datos.idDepartamento) || datos.idDepartamento <= 0) {
        return "Debe seleccionar un departamento.";
    }

    if (!departamento) {
        return "No se encontró la información del departamento.";
    }

    if (
        !Number.isInteger(
            datos.idAdministrador
        ) ||
        datos.idAdministrador <= 0
    ) {
        return (
            "No se pudo identificar al administrador. " +
            "Cierre sesión e inicie nuevamente."
        );
    }

    if (!datos.fechaInicioPeriodo) {
        return (
            "Debe seleccionar la fecha inicial del período."
        );
    }

    if (!datos.fechaFinPeriodo) {
        return (
            "Debe seleccionar la fecha final del período."
        );
    }

    if (
        datos.fechaFinPeriodo <
        datos.fechaInicioPeriodo
    ) {
        return (
            "La fecha final no puede ser anterior " +
            "a la fecha inicial."
        );
    }

    const existeDuplicada =
        planillas.some(
            planilla =>
                planilla.idDepartamento === datos.idDepartamento &&
                planilla.fechaInicioPeriodo === datos.fechaInicioPeriodo &&
                planilla.fechaFinPeriodo === datos.fechaFinPeriodo
        );

    if (existeDuplicada) {
        return (
            "Ya existe una planilla para ese departamento y periodo."
        );
    }

    return null;
}


async function abrirDetallePlanilla(
    idPlanilla
) {
    contenidoDetallePlanilla.innerHTML =
        `
            <div class="estado-tabla">
                Cargando detalle...
            </div>
        `;

    mostrarModal(
        modalDetalle
    );

    try {
        const planilla =
            await obtenerPlanillaPorId(
                idPlanilla
            );

        if (!planilla) {
            throw new Error(
                "No se encontró la planilla."
            );
        }

        renderizarDetallePlanilla(
            planilla
        );
    } catch (error) {
        console.error(error);

        contenidoDetallePlanilla.innerHTML = `
            <div class="mensaje mensaje--error mensaje--modal">
                ${escaparHtml(
                    error.message ||
                    "No fue posible cargar el detalle."
                )}
            </div>
        `;
    }
}

function renderizarDetallePlanilla(
    planilla
) {
    const estado =
        obtenerPresentacionEstado(
            planilla.estado
        );

    const filasDeducciones =
        planilla.deducciones.length > 0
            ? planilla.deducciones
                .map(
                    deduccion => `
                        <tr>
                            <td>
                                ${escaparHtml(
                                    deduccion.nombreTipoDeduccion
                                )}
                            </td>

                            <td>
                                ${escaparHtml(
                                    obtenerTextoTipoCalculo(
                                        deduccion.tipoCalculo
                                    )
                                )}
                            </td>

                            <td>
                                ${formatearValorAplicado(
                                    deduccion.tipoCalculo,
                                    deduccion.valorAplicado
                                )}
                            </td>

                            <td>
                                ${formatearMoneda(
                                    deduccion.montoCalculado
                                )}
                            </td>

                            <td>
                                ${escaparHtml(
                                    deduccion.observacion ||
                                    "Sin observación"
                                )}
                            </td>
                        </tr>
                    `
                )
                .join("")
            : `
                <tr>
                    <td colspan="5">
                        Sin deducciones aplicadas.
                    </td>
                </tr>
            `;

    contenidoDetallePlanilla.innerHTML = `
        <div class="detalle-cabecera">
            <div class="detalle-dato">
                <span>Empleado</span>

                <strong>
                    ${escaparHtml(
                        planilla.nombreEmpleado
                    )}
                </strong>
            </div>

            <div class="detalle-dato">
                <span>Código</span>

                <strong>
                    ${escaparHtml(
                        planilla.codigoEmpleado
                    )}
                </strong>
            </div>

            <div class="detalle-dato">
                <span>Período</span>

                <strong>
                    ${formatearFecha(
                        planilla.fechaInicioPeriodo
                    )}
                    al
                    ${formatearFecha(
                        planilla.fechaFinPeriodo
                    )}
                </strong>
            </div>

            <div class="detalle-dato">
                <span>Estado</span>

                <strong>
                    ${escaparHtml(
                        estado.nombre
                    )}
                </strong>
            </div>

            <div class="detalle-dato">
                <span>Generada por</span>

                <strong>
                    ${escaparHtml(
                        planilla.nombreAdministrador ||
                        "Sin información"
                    )}
                </strong>
            </div>

            <div class="detalle-dato">
                <span>Fecha de creación</span>

                <strong>
                    ${formatearFechaHora(
                        planilla.fechaCreacion
                    )}
                </strong>
            </div>
        </div>

        <div class="detalle-totales">
            <div class="detalle-total">
                <span>Salario base</span>

                <strong>
                    ${formatearMoneda(
                        planilla.salarioBasePeriodo
                    )}
                </strong>
            </div>

            <div class="detalle-total">
                <span>Ingresos adicionales</span>

                <strong>
                    ${formatearMoneda(
                        planilla.ingresosAdicionales
                    )}
                </strong>
            </div>

            <div class="detalle-total">
                <span>Total deducciones</span>

                <strong>
                    ${formatearMoneda(
                        planilla.totalDeducciones
                    )}
                </strong>
            </div>

            <div class="detalle-total detalle-total--neto">
                <span>Salario neto</span>

                <strong>
                    ${formatearMoneda(
                        planilla.salarioNeto
                    )}
                </strong>
            </div>
        </div>

        <div class="detalle-deducciones">
            <h4>Detalle de deducciones</h4>

            <div class="detalle-tabla-contenedor">
                <table class="detalle-tabla">
                    <thead>
                        <tr>
                            <th>Deducción</th>
                            <th>Tipo</th>
                            <th>Valor aplicado</th>
                            <th>Monto calculado</th>
                            <th>Observación</th>
                        </tr>
                    </thead>

                    <tbody>
                        ${filasDeducciones}
                    </tbody>
                </table>
            </div>
        </div>
    `;
}


function abrirModalEstado(
    idPlanilla
) {
    const planilla =
        planillas.find(
            item =>
                Number(
                    item.idPlanilla
                ) ===
                Number(
                    idPlanilla
                )
        );

    if (!planilla) {
        return;
    }

    inputIdPlanillaEstado.value =
        planilla.idPlanilla;

    selectNuevoEstado.value =
        String(
            planilla.estado
        );

    descripcionCambioEstado.textContent =
        `${planilla.nombreEmpleado} · ` +
        `${formatearFecha(
            planilla.fechaInicioPeriodo
        )} al ` +
        `${formatearFecha(
            planilla.fechaFinPeriodo
        )}`;

    limpiarMensajeEstado();

    mostrarModal(
        modalEstado
    );

    selectNuevoEstado.focus();
}

async function guardarCambioEstado(
    event
) {
    event.preventDefault();

    limpiarMensajeEstado();

    const idPlanilla =
        Number(
            inputIdPlanillaEstado.value
        );

    const estado =
        Number(
            selectNuevoEstado.value
        );

    const planilla =
        planillas.find(
            item =>
                Number(
                    item.idPlanilla
                ) ===
                idPlanilla
        );

    if (
        !Number.isInteger(idPlanilla) ||
        idPlanilla <= 0
    ) {
        mostrarMensajeEstado(
            "La planilla seleccionada no es válida."
        );

        return;
    }

    if (
        !Number.isInteger(estado) ||
        !ESTADOS_PLANILLA[estado]
    ) {
        mostrarMensajeEstado(
            "Debe seleccionar un estado válido."
        );

        return;
    }

    if (
        planilla &&
        Number(planilla.estado) ===
        estado
    ) {
        mostrarMensajeEstado(
            "La planilla ya tiene el estado seleccionado."
        );

        return;
    }

    bloquearFormularioEstado(
        true
    );

    try {
        await cambiarEstadoPlanilla(
            idPlanilla,
            estado
        );

        cerrarModalEstado();

        mostrarMensajePrincipal(
            "Estado de la planilla actualizado correctamente.",
            "exito"
        );

        await cargarPlanillas(
            false
        );
    } catch (error) {
        console.error(error);

        mostrarMensajeEstado(
            error.message ||
            "No fue posible cambiar el estado."
        );
    } finally {
        bloquearFormularioEstado(
            false
        );
    }
}

function obtenerIdAdministradorActual() {
    const administrador =
        obtenerAdministrador();

    const idSesion =
        Number(
            administrador?.idAdministrador ??
            administrador?.IdAdministrador ??
            administrador?.id ??
            administrador?.Id
        );

    if (
        Number.isInteger(idSesion) &&
        idSesion > 0
    ) {
        return idSesion;
    }

    const payload =
        obtenerPayloadToken(
            obtenerToken()
        );

    const idToken =
        Number(
            payload?.idAdministrador ??
            payload?.IdAdministrador ??
            payload?.administradorId ??
            payload?.AdministradorId ??
            payload?.nameid ??
            payload?.[
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            ] ??
            payload?.sub
        );

    return (
        Number.isInteger(idToken) &&
        idToken > 0
    )
        ? idToken
        : 0;
}

function obtenerPayloadToken(
    token
) {
    if (!token) {
        return null;
    }

    try {
        const partes =
            token.split(".");

        if (partes.length < 2) {
            return null;
        }

        const base64 =
            partes[1]
                .replace(/-/g, "+")
                .replace(/_/g, "/");

        const contenido =
            decodeURIComponent(
                atob(base64)
                    .split("")
                    .map(
                        caracter =>
                            "%" +
                            caracter
                                .charCodeAt(0)
                                .toString(16)
                                .padStart(2, "0")
                    )
                    .join("")
            );

        return JSON.parse(
            contenido
        );
    } catch (error) {
        console.error(
            "No se pudo leer el token.",
            error
        );

        return null;
    }
}

function obtenerDepartamentoSeleccionado() {
    return departamentos.find(
        d => Number(d.idDepartamento) === Number(selectDepartamentoPlanilla.value)
    ) ?? null;
}

function obtenerTipoDeduccion(
    idTipoDeduccion
) {
    return tiposDeduccion.find(
        tipo =>
            Number(
                tipo.idTipoDeduccion
            ) ===
            Number(
                idTipoDeduccion
            )
    ) ?? null;
}

function esTipoPorcentaje(
    tipoCalculo
) {
    if (
        Number(tipoCalculo) ===
        TIPO_PORCENTAJE
    ) {
        return true;
    }

    return (
        typeof tipoCalculo === "string" &&
        tipoCalculo
            .trim()
            .toLowerCase()
            .includes("porcentaje")
    );
}

function obtenerTextoTipoCalculo(
    tipoCalculo
) {
    if (
        esTipoPorcentaje(
            tipoCalculo
        )
    ) {
        return "Porcentaje";
    }

    if (
        Number(tipoCalculo) ===
        TIPO_MONTO_FIJO
    ) {
        return "Monto fijo";
    }

    if (
        typeof tipoCalculo === "string"
    ) {
        return tipoCalculo;
    }

    return `Tipo ${tipoCalculo}`;
}

function obtenerPresentacionEstado(
    estado
) {
    return (
        ESTADOS_PLANILLA[
            Number(estado)
        ] ??
        {
            nombre:
                `Estado ${estado}`,

            clase:
                "estado estado--desconocido"
        }
    );
}


function mostrarModal(
    modal
) {
    modal.classList.remove(
        "modal--oculto"
    );

    modal.setAttribute(
        "aria-hidden",
        "false"
    );

    document.body.style.overflow =
        "hidden";
}

function ocultarModal(
    modal
) {
    modal.classList.add(
        "modal--oculto"
    );

    modal.setAttribute(
        "aria-hidden",
        "true"
    );

    const existeModalAbierto =
        [
            modalPlanilla,
            modalDetalle,
            modalEstado
        ].some(
            elemento =>
                !elemento.classList.contains(
                    "modal--oculto"
                )
        );

    if (!existeModalAbierto) {
        document.body.style.overflow =
            "";
    }
}

function cerrarModalPlanilla() {
    ocultarModal(
        modalPlanilla
    );

    formPlanilla.reset();

    deduccionesFormulario = [];
    consecutivoDeduccion = 1;

    listaDeducciones.innerHTML =
        "";

    inputFechaFin.min =
        "";

    limpiarMensajeFormulario();
}

function cerrarModalDetalle() {
    ocultarModal(
        modalDetalle
    );

    contenidoDetallePlanilla.innerHTML =
        "";
}

function cerrarModalEstado() {
    ocultarModal(
        modalEstado
    );

    formEstado.reset();

    inputIdPlanillaEstado.value =
        "";

    limpiarMensajeEstado();
}

function manejarTeclaEscape(
    event
) {
    if (event.key !== "Escape") {
        return;
    }

    if (
        !modalPlanilla.classList.contains(
            "modal--oculto"
        )
    ) {
        cerrarModalPlanilla();
        return;
    }

    if (
        !modalDetalle.classList.contains(
            "modal--oculto"
        )
    ) {
        cerrarModalDetalle();
        return;
    }

    if (
        !modalEstado.classList.contains(
            "modal--oculto"
        )
    ) {
        cerrarModalEstado();
    }
}


function bloquearFormularioPlanilla(
    bloqueado
) {
    const elementos =
        formPlanilla.querySelectorAll(
            "input, select, textarea, button"
        );

    elementos.forEach(
        elemento => {
            elemento.disabled =
                bloqueado;
        }
    );

    inputSalarioBase.disabled =
        true;

    botonCerrarModalPlanilla.disabled =
        bloqueado;

    botonGuardarPlanilla.textContent =
        bloqueado
            ? "Generando..."
            : "Generar planilla";
}

function bloquearFormularioEstado(
    bloqueado
) {
    selectNuevoEstado.disabled =
        bloqueado;

    botonCerrarModalEstado.disabled =
        bloqueado;

    botonCancelarEstado.disabled =
        bloqueado;

    botonGuardarEstado.disabled =
        bloqueado;

    botonGuardarEstado.textContent =
        bloqueado
            ? "Guardando..."
            : "Guardar estado";
}


function actualizarResumenListado(
    lista
) {
    contadorPlanillas.textContent =
        lista.length === 1
            ? "1 planilla"
            : `${lista.length} planillas`;

    const totalSalarioNeto =
        lista.reduce(
            (
                total,
                planilla
            ) =>
                total +
                convertirNumeroNoNegativo(
                    planilla.salarioNeto
                ),
            0
        );

    totalNetoVisible.textContent =
        `Total neto: ${formatearMoneda(
            totalSalarioNeto
        )}`;
}


function mostrarCargando(
    cargando
) {
    estadoCarga.classList.toggle(
        "estado-tabla--oculto",
        !cargando
    );

    if (cargando) {
        contenedorTabla.classList.add(
            "tabla-contenedor--oculto"
        );

        sinResultados.classList.add(
            "estado-tabla--oculto"
        );
    }
}


function mostrarMensajePrincipal(
    mensaje,
    tipo
) {
    clearTimeout(
        temporizadorMensaje
    );

    mensajePlanillas.textContent =
        mensaje;

    mensajePlanillas.className =
        tipo === "exito"
            ? "mensaje mensaje--exito"
            : "mensaje mensaje--error";

    temporizadorMensaje =
        setTimeout(
            ocultarMensajePrincipal,
            4000
        );
}

function ocultarMensajePrincipal() {
    clearTimeout(
        temporizadorMensaje
    );

    mensajePlanillas.textContent =
        "";

    mensajePlanillas.className =
        "mensaje mensaje--oculto";
}

function mostrarMensajeFormulario(
    mensaje
) {
    mensajeFormulario.textContent =
        mensaje;

    mensajeFormulario.className =
        "mensaje mensaje--error mensaje--modal";
}

function limpiarMensajeFormulario() {
    mensajeFormulario.textContent =
        "";

    mensajeFormulario.className =
        "mensaje mensaje--oculto mensaje--modal";
}

function mostrarMensajeEstado(
    mensaje
) {
    mensajeEstado.textContent =
        mensaje;

    mensajeEstado.className =
        "mensaje mensaje--error mensaje--modal";
}

function limpiarMensajeEstado() {
    mensajeEstado.textContent =
        "";

    mensajeEstado.className =
        "mensaje mensaje--oculto mensaje--modal";
}


function obtenerPeriodoMensualActual() {
    const fechaActual =
        new Date();

    const fechaInicio =
        new Date(
            fechaActual.getFullYear(),
            fechaActual.getMonth(),
            1
        );

    const fechaFin =
        new Date(
            fechaActual.getFullYear(),
            fechaActual.getMonth() + 1,
            0
        );

    return {
        inicio:
            convertirFechaInput(
                fechaInicio
            ),

        fin:
            convertirFechaInput(
                fechaFin
            )
    };
}

function convertirFechaInput(
    fecha
) {
    const anio =
        fecha.getFullYear();

    const mes =
        String(
            fecha.getMonth() + 1
        ).padStart(2, "0");

    const dia =
        String(
            fecha.getDate()
        ).padStart(2, "0");

    return `${anio}-${mes}-${dia}`;
}

function formatearFecha(
    fecha
) {
    if (!fecha) {
        return "Sin fecha";
    }

    const partes =
        String(fecha)
            .slice(0, 10)
            .split("-");

    if (partes.length !== 3) {
        return fecha;
    }

    return (
        `${partes[2]}/` +
        `${partes[1]}/` +
        `${partes[0]}`
    );
}

function formatearFechaHora(
    valor
) {
    if (!valor) {
        return "Sin información";
    }

    const fecha =
        new Date(valor);

    if (
        Number.isNaN(
            fecha.getTime()
        )
    ) {
        return valor;
    }

    return fecha.toLocaleString(
        "es-NI",
        {
            dateStyle: "short",
            timeStyle: "short"
        }
    );
}


function formatearMoneda(
    valor
) {
    const numero =
        Number(valor);

    const monto =
        Number.isFinite(numero)
            ? numero
            : 0;

    return new Intl.NumberFormat(
        "es-NI",
        {
            style: "currency",
            currency: "NIO",
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }
    ).format(monto);
}

function formatearValorAplicado(
    tipoCalculo,
    valor
) {
    if (
        esTipoPorcentaje(
            tipoCalculo
        )
    ) {
        return `${Number(valor).toFixed(2)} %`;
    }

    return formatearMoneda(
        valor
    );
}

function convertirNumeroNoNegativo(
    valor
) {
    const numero =
        Number(valor);

    if (
        !Number.isFinite(numero) ||
        numero < 0
    ) {
        return 0;
    }

    return numero;
}


function escaparHtml(
    texto
) {
    const elemento =
        document.createElement("div");

    elemento.textContent =
        texto ??
        "";

    return elemento.innerHTML;
}

function escaparAtributo(
    texto
) {
    return String(
        texto ??
        ""
    )
        .replace(/&/g, "&amp;")
        .replace(/"/g, "&quot;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
}