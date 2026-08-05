import {
    protegerPagina,
    obtenerAdministrador,
    obtenerToken
} from "../../shared/js/auth.js";

import {
    inicializarLayout
} from "../../shared/js/layout.js";

import {
    obtenerVacaciones,
    obtenerVacacionPorId,
    registrarVacacion,
    actualizarVacacion,
    cancelarVacacion,
    obtenerEmpleadosParaVacaciones
} from "../vacaciones/vacaciones.services.js";


const botonNuevaVacacion =
    document.getElementById(
        "btnNuevaVacacion"
    );

const botonActualizar =
    document.getElementById(
        "btnActualizar"
    );

const botonLimpiarFiltros =
    document.getElementById(
        "btnLimpiarFiltros"
    );

const filtroEmpleado =
    document.getElementById(
        "filtroEmpleado"
    );

const filtroDesde =
    document.getElementById(
        "filtroDesde"
    );

const filtroHasta =
    document.getElementById(
        "filtroHasta"
    );

const filtroEstado =
    document.getElementById(
        "filtroEstado"
    );

const contadorVacaciones =
    document.getElementById(
        "contadorVacaciones"
    );

const tablaVacacionesBody =
    document.getElementById(
        "tablaVacacionesBody"
    );

const contenedorTabla =
    document.getElementById(
        "contenedorTabla"
    );

const estadoCarga =
    document.getElementById(
        "estadoCarga"
    );

const sinResultados =
    document.getElementById(
        "sinResultados"
    );

const mensajeVacaciones =
    document.getElementById(
        "mensajeVacaciones"
    );

/* =========================
   ELEMENTOS DEL MODAL
   ========================= */

const modalVacacion =
    document.getElementById(
        "modalVacacion"
    );

const fondoModalVacacion =
    document.getElementById(
        "fondoModalVacacion"
    );

const formVacacion =
    document.getElementById(
        "formVacacion"
    );

const inputIdVacacion =
    document.getElementById(
        "idVacacion"
    );

const selectEmpleadoVacacion =
    document.getElementById(
        "empleadoVacacion"
    );

const inputFechaInicio =
    document.getElementById(
        "fechaInicioVacacion"
    );

const inputFechaFin =
    document.getElementById(
        "fechaFinVacacion"
    );

const inputMotivo =
    document.getElementById(
        "motivoVacacion"
    );

const inputObservacion =
    document.getElementById(
        "observacionVacacion"
    );

const resumenDias =
    document.getElementById(
        "resumenDias"
    );

const tituloModalVacacion =
    document.getElementById(
        "tituloModalVacacion"
    );

const botonCerrarModalVacacion =
    document.getElementById(
        "btnCerrarModalVacacion"
    );

const botonCancelarFormulario =
    document.getElementById(
        "btnCancelarFormulario"
    );

const botonGuardarVacacion =
    document.getElementById(
        "btnGuardarVacacion"
    );

const mensajeFormulario =
    document.getElementById(
        "mensajeFormulario"
    );

/* =========================
   VARIABLES
   ========================= */

let vacaciones = [];
let empleados = [];
let temporizadorMensaje = null;

/* =========================
   INICIALIZACIÓN
   ========================= */

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
        titulo: "Vacaciones",
        paginaActiva: "vacaciones"
    });

    configurarEventos();

    await cargarEmpleados();
    await cargarVacaciones();
}

function configurarEventos() {
    botonNuevaVacacion.addEventListener(
        "click",
        abrirModalNuevo
    );

    botonActualizar.addEventListener(
        "click",
        () => cargarVacaciones()
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

    tablaVacacionesBody.addEventListener(
        "click",
        manejarAccionTabla
    );

    formVacacion.addEventListener(
        "submit",
        guardarVacacion
    );

    inputFechaInicio.addEventListener(
        "change",
        manejarCambioFechaInicio
    );

    inputFechaFin.addEventListener(
        "change",
        actualizarResumenDias
    );

    botonCerrarModalVacacion.addEventListener(
        "click",
        cerrarModal
    );

    botonCancelarFormulario.addEventListener(
        "click",
        cerrarModal
    );

    fondoModalVacacion.addEventListener(
        "click",
        cerrarModal
    );

    document.addEventListener(
        "keydown",
        manejarTeclaEscape
    );
}

/* =========================
   CARGA DE EMPLEADOS
   ========================= */

async function cargarEmpleados() {
    try {
        empleados =
            await obtenerEmpleadosParaVacaciones();

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
        empleados.map(empleado => `
            <option value="${escaparAtributo(
                empleado.codigoEmpleado
            )}">
                ${escaparHtml(
                    empleado.codigoEmpleado
                )} - ${escaparHtml(
                    empleado.nombreCompleto
                )}
            </option>
        `).join("");

    filtroEmpleado.innerHTML = `
        <option value="">
            Todos los empleados
        </option>

        ${opciones}
    `;

    selectEmpleadoVacacion.innerHTML = `
        <option value="">
            Seleccione un empleado
        </option>

        ${opciones}
    `;
}

/* =========================
   CARGA DE VACACIONES
   ========================= */

async function cargarVacaciones(
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
        vacaciones =
            await obtenerVacaciones();

        ordenarVacaciones();
        aplicarFiltros();
    } catch (error) {
        console.error(error);

        vacaciones = [];

        renderizarVacaciones([]);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cargar las vacaciones.",
            "error"
        );
    } finally {
        mostrarCargando(false);

        botonActualizar.disabled = false;
        botonActualizar.textContent =
            "Actualizar";
    }
}

function ordenarVacaciones() {
    vacaciones.sort(
        (a, b) => {
            const comparacionFecha =
                b.fechaInicio.localeCompare(
                    a.fechaInicio
                );

            if (comparacionFecha !== 0) {
                return comparacionFecha;
            }

            return a.nombreEmpleado.localeCompare(
                b.nombreEmpleado,
                "es"
            );
        }
    );
}

/* =========================
   FILTROS
   ========================= */

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
        vacaciones.filter(vacacion => {
            const coincideEmpleado =
                !codigoEmpleado ||
                vacacion.codigoEmpleado ===
                codigoEmpleado;

            /*
             * Se muestran vacaciones que se superponen
             * con el rango seleccionado.
             */
            const coincideDesde =
                !fechaDesde ||
                vacacion.fechaFin >= fechaDesde;

            const coincideHasta =
                !fechaHasta ||
                vacacion.fechaInicio <= fechaHasta;

            const coincideEstado =
                estado === "todos" ||
                (
                    estado === "activas" &&
                    !vacacion.cancelada
                ) ||
                (
                    estado === "canceladas" &&
                    vacacion.cancelada
                );

            return (
                coincideEmpleado &&
                coincideDesde &&
                coincideHasta &&
                coincideEstado
            );
        });

    actualizarContador(
        resultados.length
    );

    renderizarVacaciones(
        resultados
    );
}

function limpiarFiltros() {
    filtroEmpleado.value = "";
    filtroDesde.value = "";
    filtroHasta.value = "";
    filtroEstado.value = "todos";

    aplicarFiltros();
}

/* =========================
   TABLA
   ========================= */

function renderizarVacaciones(
    lista
) {
    tablaVacacionesBody.innerHTML =
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

    lista.forEach(vacacion => {
        const fila =
            document.createElement("tr");

        const claseEstado =
            vacacion.cancelada
                ? "estado estado--cancelada"
                : "estado estado--activa";

        const textoEstado =
            vacacion.cancelada
                ? "Cancelada"
                : "Activa";

        const nombreAdministrador =
            vacacion.nombreAdministrador ||
            "Sin información";

        const motivo =
            vacacion.motivo ||
            "Sin motivo";

        const observacion =
            vacacion.observacion ||
            "Sin observación";

        fila.innerHTML = `
            <td>
                <span class="empleado-nombre">
                    ${escaparHtml(
                        vacacion.nombreEmpleado
                    )}
                </span>

                <span class="empleado-codigo">
                    ${escaparHtml(
                        vacacion.codigoEmpleado
                    )}
                </span>
            </td>

            <td>
                ${formatearFecha(
                    vacacion.fechaInicio
                )}
            </td>

            <td>
                ${formatearFecha(
                    vacacion.fechaFin
                )}
            </td>

            <td>
                ${calcularDiasVacacion(
                    vacacion.fechaInicio,
                    vacacion.fechaFin
                )}
            </td>

            <td>
                <span
                    class="motivo-vacacion"
                    title="${escaparAtributo(
                        `${motivo}. ${observacion}`
                    )}"
                >
                    ${escaparHtml(motivo)}
                </span>
            </td>

            <td>
                <span class="${claseEstado}">
                    ${textoEstado}
                </span>
            </td>

            <td>
                <span
                    class="administrador-vacacion"
                    title="${escaparAtributo(
                        nombreAdministrador
                    )}"
                >
                    ${escaparHtml(
                        nombreAdministrador
                    )}
                </span>
            </td>

            <td>
                <div class="celda-acciones">
                    <button
                        type="button"
                        class="boton-tabla boton-tabla--editar"
                        data-accion="editar"
                        data-id="${vacacion.idVacacion}"
                        ${vacacion.cancelada ? "disabled" : ""}
                    >
                        Editar
                    </button>

                    <button
                        type="button"
                        class="boton-tabla boton-tabla--cancelar"
                        data-accion="cancelar"
                        data-id="${vacacion.idVacacion}"
                        ${vacacion.cancelada ? "disabled" : ""}
                    >
                        Cancelar
                    </button>
                </div>
            </td>
        `;

        tablaVacacionesBody.appendChild(
            fila
        );
    });
}

function manejarAccionTabla(event) {
    const boton =
        event.target.closest(
            "[data-accion]"
        );

    if (!boton) {
        return;
    }

    const idVacacion =
        Number(boton.dataset.id);

    if (!Number.isInteger(idVacacion)) {
        return;
    }

    const accion =
        boton.dataset.accion;

    if (accion === "editar") {
        abrirModalEditar(
            idVacacion
        );

        return;
    }

    if (accion === "cancelar") {
        procesarCancelacion(
            idVacacion,
            boton
        );
    }
}

/* =========================
   MODAL NUEVO
   ========================= */

function abrirModalNuevo() {
    formVacacion.reset();

    inputIdVacacion.value = "";

    tituloModalVacacion.textContent =
        "Registrar vacaciones";

    botonGuardarVacacion.textContent =
        "Guardar";

    selectEmpleadoVacacion.disabled =
        false;

    const fechaActual =
        obtenerFechaActual();

    inputFechaInicio.value =
        fechaActual;

    inputFechaFin.value =
        fechaActual;

    inputFechaFin.min =
        fechaActual;

    limpiarMensajeFormulario();
    actualizarResumenDias();
    mostrarModal();

    selectEmpleadoVacacion.focus();
}

/* =========================
   MODAL EDITAR
   ========================= */

async function abrirModalEditar(
    idVacacion
) {
    limpiarMensajeFormulario();

    try {
        let vacacion =
            vacaciones.find(
                item =>
                    Number(item.idVacacion) ===
                    Number(idVacacion)
            );

        if (!vacacion) {
            vacacion =
                await obtenerVacacionPorId(
                    idVacacion
                );
        }

        if (!vacacion) {
            throw new Error(
                "No se encontró el registro de vacaciones."
            );
        }

        if (vacacion.cancelada) {
            throw new Error(
                "No se puede editar una vacación cancelada."
            );
        }

        formVacacion.reset();

        inputIdVacacion.value =
            vacacion.idVacacion;

        selectEmpleadoVacacion.value =
            vacacion.codigoEmpleado;

        inputFechaInicio.value =
            vacacion.fechaInicio;

        inputFechaFin.value =
            vacacion.fechaFin;

        inputFechaFin.min =
            vacacion.fechaInicio;

        inputMotivo.value =
            vacacion.motivo;

        inputObservacion.value =
            vacacion.observacion;

        tituloModalVacacion.textContent =
            "Editar vacaciones";

        botonGuardarVacacion.textContent =
            "Guardar cambios";

        /*
         * El backend permite enviar codigoEmpleado
         * en el PUT. Se conserva el empleado
         * seleccionado para evitar cambios accidentales.
         */
        selectEmpleadoVacacion.disabled =
            true;

        actualizarResumenDias();
        mostrarModal();

        inputFechaInicio.focus();
    } catch (error) {
        console.error(error);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible consultar las vacaciones.",
            "error"
        );
    }
}

/* =========================
   GUARDAR
   ========================= */

async function guardarVacacion(event) {
    event.preventDefault();

    limpiarMensajeFormulario();

    const idVacacion =
        inputIdVacacion.value.trim();

    const idAdministrador =
        obtenerIdAdministradorActual();

    const datos = {
        codigoEmpleado:
            selectEmpleadoVacacion.value,

        idAdministrador,

        fechaInicio:
            inputFechaInicio.value,

        fechaFin:
            inputFechaFin.value,

        motivo:
            inputMotivo.value.trim(),

        observacion:
            inputObservacion.value.trim() ||
            null
    };

    const errorValidacion =
        validarDatosVacacion(
            datos,
            idVacacion
        );

    if (errorValidacion) {
        mostrarMensajeFormulario(
            errorValidacion
        );

        return;
    }

    bloquearFormulario(true);

    try {
        if (idVacacion) {
            await actualizarVacacion(
                Number(idVacacion),
                datos
            );

            cerrarModal();

            mostrarMensajePrincipal(
                "Vacaciones actualizadas correctamente.",
                "exito"
            );
        } else {
            await registrarVacacion(
                datos
            );

            cerrarModal();

            mostrarMensajePrincipal(
                "Vacaciones registradas correctamente.",
                "exito"
            );
        }

        await cargarVacaciones(false);
    } catch (error) {
        console.error(error);

        mostrarMensajeFormulario(
            error.message ||
            "No fue posible guardar las vacaciones."
        );
    } finally {
        bloquearFormulario(false);
    }
}

function validarDatosVacacion(
    datos,
    idVacacionActual
) {
    if (!datos.codigoEmpleado) {
        return "Debe seleccionar un empleado.";
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

    if (!datos.fechaInicio) {
        return "Debe seleccionar la fecha inicial.";
    }

    if (!datos.fechaFin) {
        return "Debe seleccionar la fecha final.";
    }

    if (
        datos.fechaFin <
        datos.fechaInicio
    ) {
        return (
            "La fecha final no puede ser " +
            "anterior a la fecha inicial."
        );
    }

    if (!datos.motivo) {
        return "Debe ingresar el motivo de las vacaciones.";
    }

    if (datos.motivo.length < 3) {
        return "El motivo debe contener al menos 3 caracteres.";
    }

    /*
     * Prevención de períodos superpuestos.
     * El backend continuará siendo la validación definitiva.
     */
    const existeCruce =
        vacaciones.some(vacacion => {
            const mismoRegistro =
                Number(vacacion.idVacacion) ===
                Number(idVacacionActual);

            if (
                mismoRegistro ||
                vacacion.cancelada
            ) {
                return false;
            }

            const mismoEmpleado =
                vacacion.codigoEmpleado ===
                datos.codigoEmpleado;

            const periodosSeCruzan =
                datos.fechaInicio <=
                    vacacion.fechaFin &&
                datos.fechaFin >=
                    vacacion.fechaInicio;

            return (
                mismoEmpleado &&
                periodosSeCruzan
            );
        });

    if (existeCruce) {
        return (
            "El empleado ya tiene vacaciones " +
            "registradas dentro de ese período."
        );
    }

    return null;
}

/* =========================
   CANCELAR
   ========================= */

async function procesarCancelacion(
    idVacacion,
    boton
) {
    const vacacion =
        vacaciones.find(
            item =>
                Number(item.idVacacion) ===
                Number(idVacacion)
        );

    if (!vacacion) {
        return;
    }

    if (vacacion.cancelada) {
        mostrarMensajePrincipal(
            "Las vacaciones ya se encuentran canceladas.",
            "error"
        );

        return;
    }

    const confirmado =
        window.confirm(
            `¿Está seguro de cancelar las vacaciones de ` +
            `${vacacion.nombreEmpleado} del ` +
            `${formatearFecha(vacacion.fechaInicio)} al ` +
            `${formatearFecha(vacacion.fechaFin)}?`
        );

    if (!confirmado) {
        return;
    }

    boton.disabled = true;
    boton.textContent =
        "Cancelando...";

    try {
        await cancelarVacacion(
            idVacacion
        );

        mostrarMensajePrincipal(
            "Vacaciones canceladas correctamente.",
            "exito"
        );

        await cargarVacaciones(false);
    } catch (error) {
        console.error(error);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cancelar las vacaciones.",
            "error"
        );

        boton.disabled = false;
        boton.textContent =
            "Cancelar";
    }
}

/* =========================
   FECHAS
   ========================= */

function manejarCambioFechaInicio() {
    const fechaInicio =
        inputFechaInicio.value;

    inputFechaFin.min =
        fechaInicio || "";

    if (
        fechaInicio &&
        (
            !inputFechaFin.value ||
            inputFechaFin.value <
                fechaInicio
        )
    ) {
        inputFechaFin.value =
            fechaInicio;
    }

    actualizarResumenDias();
}

function actualizarResumenDias() {
    const fechaInicio =
        inputFechaInicio.value;

    const fechaFin =
        inputFechaFin.value;

    if (
        !fechaInicio ||
        !fechaFin
    ) {
        resumenDias.textContent =
            "Seleccione las fechas para calcular la duración.";

        return;
    }

    if (fechaFin < fechaInicio) {
        resumenDias.textContent =
            "La fecha final es anterior a la fecha inicial.";

        return;
    }

    const dias =
        calcularDiasVacacion(
            fechaInicio,
            fechaFin
        );

    resumenDias.textContent =
        dias === 1
            ? "Duración: 1 día."
            : `Duración: ${dias} días.`;
}

function calcularDiasVacacion(
    fechaInicio,
    fechaFin
) {
    if (
        !fechaInicio ||
        !fechaFin
    ) {
        return 0;
    }

    const inicio =
        convertirFechaAUTC(
            fechaInicio
        );

    const fin =
        convertirFechaAUTC(
            fechaFin
        );

    const milisegundosPorDia =
        1000 * 60 * 60 * 24;

    return (
        Math.floor(
            (fin - inicio) /
            milisegundosPorDia
        ) + 1
    );
}

function convertirFechaAUTC(
    fecha
) {
    const partes =
        fecha.split("-")
            .map(Number);

    return Date.UTC(
        partes[0],
        partes[1] - 1,
        partes[2]
    );
}

function formatearFecha(
    fecha
) {
    if (!fecha) {
        return "Sin fecha";
    }

    const partes =
        fecha.split("-");

    if (partes.length !== 3) {
        return fecha;
    }

    return `${partes[2]}/${partes[1]}/${partes[0]}`;
}

function obtenerFechaActual() {
    const fecha =
        new Date();

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

/* =========================
   ADMINISTRADOR ACTUAL
   ========================= */

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

    const token =
        obtenerToken();

    const payload =
        obtenerPayloadToken(
            token
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
                    .map(caracter =>
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

/* =========================
   MODAL
   ========================= */

function mostrarModal() {
    modalVacacion.classList.remove(
        "modal--oculto"
    );

    modalVacacion.setAttribute(
        "aria-hidden",
        "false"
    );

    document.body.style.overflow =
        "hidden";
}

function cerrarModal() {
    modalVacacion.classList.add(
        "modal--oculto"
    );

    modalVacacion.setAttribute(
        "aria-hidden",
        "true"
    );

    document.body.style.overflow =
        "";

    formVacacion.reset();

    inputIdVacacion.value = "";

    selectEmpleadoVacacion.disabled =
        false;

    inputFechaFin.min = "";

    limpiarMensajeFormulario();
}

function manejarTeclaEscape(event) {
    if (
        event.key === "Escape" &&
        !modalVacacion.classList.contains(
            "modal--oculto"
        )
    ) {
        cerrarModal();
    }
}

function bloquearFormulario(
    bloqueado
) {
    const editando =
        Boolean(
            inputIdVacacion.value
        );

    selectEmpleadoVacacion.disabled =
        bloqueado || editando;

    inputFechaInicio.disabled =
        bloqueado;

    inputFechaFin.disabled =
        bloqueado;

    inputMotivo.disabled =
        bloqueado;

    inputObservacion.disabled =
        bloqueado;

    botonCerrarModalVacacion.disabled =
        bloqueado;

    botonCancelarFormulario.disabled =
        bloqueado;

    botonGuardarVacacion.disabled =
        bloqueado;

    botonGuardarVacacion.textContent =
        bloqueado
            ? "Guardando..."
            : editando
                ? "Guardar cambios"
                : "Guardar";
}

/* =========================
   CARGA Y CONTADOR
   ========================= */

function mostrarCargando(
    cargando
) {
    if (cargando) {
        estadoCarga.classList.remove(
            "estado-tabla--oculto"
        );

        contenedorTabla.classList.add(
            "tabla-contenedor--oculto"
        );

        sinResultados.classList.add(
            "estado-tabla--oculto"
        );

        return;
    }

    estadoCarga.classList.add(
        "estado-tabla--oculto"
    );
}

function actualizarContador(
    cantidadVisible
) {
    if (
        cantidadVisible ===
        vacaciones.length
    ) {
        contadorVacaciones.textContent =
            vacaciones.length === 1
                ? "1 vacación registrada"
                : `${vacaciones.length} vacaciones registradas`;

        return;
    }

    contadorVacaciones.textContent =
        `${cantidadVisible} de ${vacaciones.length} registros`;
}

/* =========================
   MENSAJES
   ========================= */

function mostrarMensajePrincipal(
    mensaje,
    tipo
) {
    clearTimeout(
        temporizadorMensaje
    );

    mensajeVacaciones.textContent =
        mensaje;

    mensajeVacaciones.className =
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

    mensajeVacaciones.textContent =
        "";

    mensajeVacaciones.className =
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

/* =========================
   SEGURIDAD DEL HTML
   ========================= */

function escaparHtml(
    texto
) {
    const elemento =
        document.createElement("div");

    elemento.textContent =
        texto ?? "";

    return elemento.innerHTML;
}

function escaparAtributo(
    texto
) {
    return String(texto ?? "")
        .replace(/&/g, "&amp;")
        .replace(/"/g, "&quot;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
}