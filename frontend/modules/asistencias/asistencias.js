import {
    protegerPagina,
    obtenerAdministrador,
    obtenerToken
} from "../../shared/js/auth.js";

import {
    inicializarLayout
} from "../../shared/js/layout.js";

import {
    obtenerAsistencias,
    obtenerEmpleados,
    registrarAsistenciaManual,
    corregirAsistencia
} from "./asistencias.service.js";

/* =========================
   ELEMENTOS PRINCIPALES
   ========================= */

const botonNuevaAsistencia =
    document.getElementById(
        "btnNuevaAsistencia"
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

const fechaInicio =
    document.getElementById(
        "fechaInicio"
    );

const fechaFin =
    document.getElementById(
        "fechaFin"
    );

const filtroEstado =
    document.getElementById(
        "filtroEstado"
    );

const contadorAsistencias =
    document.getElementById(
        "contadorAsistencias"
    );

const tablaAsistenciasBody =
    document.getElementById(
        "tablaAsistenciasBody"
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

const mensajeAsistencias =
    document.getElementById(
        "mensajeAsistencias"
    );

/* =========================
   MODAL DE REGISTRO MANUAL
   ========================= */

const modalAsistencia =
    document.getElementById(
        "modalAsistencia"
    );

const fondoModal =
    document.getElementById(
        "fondoModal"
    );

const botonCerrarModal =
    document.getElementById(
        "btnCerrarModal"
    );

const botonCancelar =
    document.getElementById(
        "btnCancelar"
    );

const botonGuardar =
    document.getElementById(
        "btnGuardar"
    );

const formAsistencia =
    document.getElementById(
        "formAsistencia"
    );

const empleadoManual =
    document.getElementById(
        "empleadoManual"
    );

const horaEntrada =
    document.getElementById(
        "horaEntrada"
    );

const horaSalida =
    document.getElementById(
        "horaSalida"
    );

const observacionManual =
    document.getElementById(
        "observacion"
    );

const mensajeFormulario =
    document.getElementById(
        "mensajeFormulario"
    );

/* =========================
   MODAL DE CORRECCIÓN
   ========================= */

const modalCorreccion =
    document.getElementById(
        "modalCorreccion"
    );

const fondoModalCorreccion =
    document.getElementById(
        "fondoModalCorreccion"
    );

const formCorreccion =
    document.getElementById(
        "formCorreccion"
    );

const inputIdAsistenciaCorreccion =
    document.getElementById(
        "idAsistenciaCorreccion"
    );

const inputEmpleadoCorreccion =
    document.getElementById(
        "empleadoCorreccion"
    );

const inputEntradaCorreccion =
    document.getElementById(
        "entradaCorreccion"
    );

const inputSalidaCorreccion =
    document.getElementById(
        "salidaCorreccion"
    );

const inputObservacionCorreccion =
    document.getElementById(
        "observacionCorreccion"
    );

const inputMotivoCorreccion =
    document.getElementById(
        "motivoCorreccion"
    );

const mensajeCorreccion =
    document.getElementById(
        "mensajeCorreccion"
    );

const botonCerrarCorreccion =
    document.getElementById(
        "btnCerrarCorreccion"
    );

const botonCancelarCorreccion =
    document.getElementById(
        "btnCancelarCorreccion"
    );

const botonGuardarCorreccion =
    document.getElementById(
        "btnGuardarCorreccion"
    );

/* =========================
   VARIABLES
   ========================= */

let asistencias = [];
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
        titulo: "Asistencias",
        paginaActiva: "asistencias"
    });

    configurarEventos();
    establecerFechasIniciales();

    await cargarInformacion();
}

function configurarEventos() {
    botonNuevaAsistencia.addEventListener(
        "click",
        abrirModalManual
    );

    botonActualizar.addEventListener(
        "click",
        cargarInformacion
    );

    botonLimpiarFiltros.addEventListener(
        "click",
        limpiarFiltros
    );

    filtroEmpleado.addEventListener(
        "change",
        aplicarFiltros
    );

    fechaInicio.addEventListener(
        "change",
        aplicarFiltros
    );

    fechaFin.addEventListener(
        "change",
        aplicarFiltros
    );

    filtroEstado.addEventListener(
        "change",
        aplicarFiltros
    );

    formAsistencia.addEventListener(
        "submit",
        guardarAsistenciaManual
    );

    botonCerrarModal.addEventListener(
        "click",
        cerrarModalManual
    );

    botonCancelar.addEventListener(
        "click",
        cerrarModalManual
    );

    fondoModal.addEventListener(
        "click",
        cerrarModalManual
    );

    formCorreccion.addEventListener(
        "submit",
        guardarCorreccion
    );

    botonCerrarCorreccion.addEventListener(
        "click",
        cerrarModalCorreccion
    );

    botonCancelarCorreccion.addEventListener(
        "click",
        cerrarModalCorreccion
    );

    fondoModalCorreccion.addEventListener(
        "click",
        cerrarModalCorreccion
    );

    document.addEventListener(
        "keydown",
        manejarTeclaEscape
    );
}

/* =========================
   CARGA DE INFORMACIÓN
   ========================= */

async function cargarInformacion() {
    mostrarCargando(true);
    ocultarMensajePrincipal();

    botonActualizar.disabled = true;
    botonActualizar.textContent =
        "Actualizando...";

    try {
        const resultados =
            await Promise.all([
                obtenerAsistencias(),
                obtenerEmpleados()
            ]);

        asistencias =
            resultados[0];

        empleados =
            resultados[1];

        ordenarAsistencias();
        llenarSelectEmpleados();
        aplicarFiltros();
    } catch (error) {
        console.error(error);

        asistencias = [];
        empleados = [];

        renderizarAsistencias([]);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cargar las asistencias.",
            "error"
        );
    } finally {
        mostrarCargando(false);

        botonActualizar.disabled = false;
        botonActualizar.textContent =
            "Actualizar";
    }
}

function ordenarAsistencias() {
    asistencias.sort((a, b) => {
        return (
            obtenerTiempo(b.horaEntrada) -
            obtenerTiempo(a.horaEntrada)
        );
    });
}

/* =========================
   FILTROS
   ========================= */

function aplicarFiltros() {
    const idEmpleadoSeleccionado =
        Number(filtroEmpleado.value);

    const inicio =
        fechaInicio.value;

    const fin =
        fechaFin.value;

    const estado =
        filtroEstado.value;

    if (
        inicio &&
        fin &&
        inicio > fin
    ) {
        renderizarAsistencias([]);

        mostrarMensajePrincipal(
            "La fecha inicial no puede ser posterior a la fecha final.",
            "error"
        );

        return;
    }

    const resultados =
        asistencias.filter(asistencia => {
            const coincideEmpleado =
                !idEmpleadoSeleccionado ||
                Number(asistencia.idEmpleado) ===
                    idEmpleadoSeleccionado;

            const fechaAsistencia =
                obtenerFechaLocal(
                    asistencia.horaEntrada
                );

            const coincideInicio =
                !inicio ||
                fechaAsistencia >= inicio;

            const coincideFin =
                !fin ||
                fechaAsistencia <= fin;

            const completa =
                Boolean(asistencia.horaSalida);

            const coincideEstado =
                !estado ||
                (
                    estado === "completa" &&
                    completa
                ) ||
                (
                    estado === "pendiente" &&
                    !completa
                );

            return (
                coincideEmpleado &&
                coincideInicio &&
                coincideFin &&
                coincideEstado
            );
        });

    renderizarAsistencias(resultados);
}

function limpiarFiltros() {
    filtroEmpleado.value = "";
    filtroEstado.value = "";

    establecerFechasIniciales();

    ocultarMensajePrincipal();
    aplicarFiltros();
}

function establecerFechasIniciales() {
    const ahora =
        new Date();

    const primerDia =
        new Date(
            ahora.getFullYear(),
            ahora.getMonth(),
            1
        );

    fechaInicio.value =
        obtenerFechaInput(primerDia);

    fechaFin.value =
        obtenerFechaInput(ahora);
}

/* =========================
   TABLA
   ========================= */

function renderizarAsistencias(lista) {
    tablaAsistenciasBody.innerHTML = "";

    contadorAsistencias.textContent =
        `${lista.length} ${
            lista.length === 1
                ? "registro"
                : "registros"
        }`;

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

    lista.forEach(asistencia => {
        const fila =
            document.createElement("tr");

        const completa =
            Boolean(asistencia.horaSalida);

        const claseEstado =
            completa
                ? "estado estado--completa"
                : "estado estado--pendiente";

        const textoEstado =
            completa
                ? "Completa"
                : "Pendiente";

        const nombreEmpleado =
            asistencia.nombreEmpleado ||
            obtenerNombreEmpleado(
                asistencia.idEmpleado
            );

        const codigoEmpleado =
            asistencia.codigoEmpleado ||
            obtenerCodigoEmpleado(
                asistencia.idEmpleado
            );

        const textoObservacion =
            asistencia.observacion ||
            (
                asistencia.esManual
                    ? "Registro manual"
                    : "Sin observación"
            );

        fila.innerHTML = `
            <td>
                <span class="empleado">
                    ${escaparHtml(nombreEmpleado)}
                </span>

                <span class="codigo-empleado">
                    ${escaparHtml(codigoEmpleado)}
                </span>
            </td>

            <td>
                ${formatearFecha(
                    asistencia.horaEntrada
                )}
            </td>

            <td>
                ${formatearHora(
                    asistencia.horaEntrada
                )}
            </td>

            <td>
                ${
                    asistencia.horaSalida
                        ? formatearHora(
                            asistencia.horaSalida
                        )
                        : "--:--"
                }
            </td>

            <td>
                ${calcularDuracion(
                    asistencia.horaEntrada,
                    asistencia.horaSalida
                )}
            </td>

            <td>
                <span class="${claseEstado}">
                    ${textoEstado}
                </span>
            </td>

            <td>
                <span
                    class="observacion"
                    title="${escaparAtributo(
                        textoObservacion
                    )}"
                >
                    ${escaparHtml(textoObservacion)}
                </span>
            </td>

            <td>
                <div class="celda-acciones">
                    <button
                        type="button"
                        class="boton-tabla boton-tabla--corregir"
                        data-accion="corregir"
                        data-id="${asistencia.idAsistencia}"
                    >
                        Corregir
                    </button>
                </div>
            </td>
        `;

        tablaAsistenciasBody.appendChild(
            fila
        );
    });

    configurarBotonesCorreccion();
}

function configurarBotonesCorreccion() {
    const botones =
        document.querySelectorAll(
            '[data-accion="corregir"]'
        );

    botones.forEach(boton => {
        boton.addEventListener(
            "click",
            () => {
                abrirModalCorreccion(
                    Number(boton.dataset.id)
                );
            }
        );
    });
}

/* =========================
   SELECT DE EMPLEADOS
   ========================= */

function llenarSelectEmpleados() {
    const empleadoFiltroActual =
        filtroEmpleado.value;

    filtroEmpleado.innerHTML = `
        <option value="">
            Todos los empleados
        </option>
    `;

    empleadoManual.innerHTML = `
        <option value="">
            Seleccione un empleado
        </option>
    `;

    const empleadosOrdenados =
        [...empleados].sort(
            (a, b) =>
                obtenerNombreCompleto(a)
                    .localeCompare(
                        obtenerNombreCompleto(b),
                        "es"
                    )
        );

    empleadosOrdenados.forEach(empleado => {
        const nombreCompleto =
            obtenerNombreCompleto(empleado);

        const texto =
            `${empleado.codigoEmpleado} - ${nombreCompleto}`;

        const opcionFiltro =
            document.createElement("option");

        opcionFiltro.value =
            empleado.idEmpleado;

        opcionFiltro.textContent =
            texto;

        filtroEmpleado.appendChild(
            opcionFiltro
        );

        if (empleado.activo) {
            const opcionManual =
                document.createElement("option");

            opcionManual.value =
                empleado.idEmpleado;

            opcionManual.textContent =
                texto;

            empleadoManual.appendChild(
                opcionManual
            );
        }
    });

    filtroEmpleado.value =
        empleadoFiltroActual;
}

/* =========================
   REGISTRO MANUAL
   ========================= */

function abrirModalManual() {
    const empleadosActivos =
        empleados.filter(
            empleado => empleado.activo
        );

    if (empleadosActivos.length === 0) {
        mostrarMensajePrincipal(
            "No existen empleados activos para registrar una asistencia.",
            "error"
        );

        return;
    }

    formAsistencia.reset();

    horaEntrada.value =
        obtenerFechaHoraLocalActual();

    horaSalida.value = "";

    limpiarMensajeFormulario();

    mostrarModal(modalAsistencia);

    empleadoManual.focus();
}

function cerrarModalManual() {
    ocultarModal(modalAsistencia);

    formAsistencia.reset();

    limpiarMensajeFormulario();
}

async function guardarAsistenciaManual(event) {
    event.preventDefault();

    limpiarMensajeFormulario();

    const idEmpleado =
        Number(empleadoManual.value);

    const empleadoSeleccionado =
        empleados.find(
            empleado =>
                Number(empleado.idEmpleado) ===
                idEmpleado
        );

    const valorEntrada =
        horaEntrada.value;

    const valorSalida =
        horaSalida.value;

    const textoObservacion =
        observacionManual.value.trim();

    if (!idEmpleado) {
        mostrarMensajeFormulario(
            "Debe seleccionar un empleado."
        );

        empleadoManual.focus();
        return;
    }

    if (!empleadoSeleccionado) {
        mostrarMensajeFormulario(
            "No se encontró el empleado seleccionado."
        );

        return;
    }

    if (!empleadoSeleccionado.codigoEmpleado) {
        mostrarMensajeFormulario(
            "El empleado seleccionado no tiene un código válido."
        );

        return;
    }

    if (!valorEntrada) {
        mostrarMensajeFormulario(
            "Debe ingresar la fecha y hora de entrada."
        );

        horaEntrada.focus();
        return;
    }

    if (
        valorSalida &&
        new Date(valorSalida).getTime() <=
        new Date(valorEntrada).getTime()
    ) {
        mostrarMensajeFormulario(
            "La hora de salida debe ser posterior a la hora de entrada."
        );

        horaSalida.focus();
        return;
    }

    if (!textoObservacion) {
        mostrarMensajeFormulario(
            "Debe ingresar una observación."
        );

        observacionManual.focus();
        return;
    }

    if (textoObservacion.length < 5) {
        mostrarMensajeFormulario(
            "La observación debe contener al menos 5 caracteres."
        );

        observacionManual.focus();
        return;
    }

    bloquearFormularioManual(true);

    try {
        await registrarAsistenciaManual({
            codigoEmpleado:
                empleadoSeleccionado.codigoEmpleado,

            horaEntrada:
                convertirAUtc(valorEntrada),

            horaSalida:
                valorSalida
                    ? convertirAUtc(valorSalida)
                    : null,

            observacion:
                textoObservacion
        });

        cerrarModalManual();

        mostrarMensajePrincipal(
            "Asistencia registrada correctamente.",
            "exito"
        );

        await cargarInformacion();
    } catch (error) {
        console.error(error);

        mostrarMensajeFormulario(
            error.message ||
            "No fue posible registrar la asistencia."
        );
    } finally {
        bloquearFormularioManual(false);
    }
}

function bloquearFormularioManual(bloqueado) {
    empleadoManual.disabled =
        bloqueado;

    horaEntrada.disabled =
        bloqueado;

    horaSalida.disabled =
        bloqueado;

    observacionManual.disabled =
        bloqueado;

    botonCancelar.disabled =
        bloqueado;

    botonCerrarModal.disabled =
        bloqueado;

    botonGuardar.disabled =
        bloqueado;

    botonGuardar.textContent =
        bloqueado
            ? "Registrando..."
            : "Registrar";
}

/* =========================
   CORRECCIÓN DE ASISTENCIA
   ========================= */

function abrirModalCorreccion(idAsistencia) {
    const asistencia =
        asistencias.find(
            item =>
                Number(item.idAsistencia) ===
                Number(idAsistencia)
        );

    if (!asistencia) {
        mostrarMensajePrincipal(
            "No se encontró la asistencia seleccionada.",
            "error"
        );

        return;
    }

    formCorreccion.reset();

    inputIdAsistenciaCorreccion.value =
        asistencia.idAsistencia;

    inputEmpleadoCorreccion.value =
        asistencia.nombreEmpleado ||
        obtenerNombreEmpleado(
            asistencia.idEmpleado
        );

    inputEntradaCorreccion.value =
        convertirFechaAInput(
            asistencia.horaEntrada
        );

    inputSalidaCorreccion.value =
        asistencia.horaSalida
            ? convertirFechaAInput(
                asistencia.horaSalida
            )
            : "";

    inputObservacionCorreccion.value =
        asistencia.observacion || "";

    inputMotivoCorreccion.value = "";

    limpiarMensajeCorreccion();

    mostrarModal(modalCorreccion);

    inputMotivoCorreccion.focus();
}

function cerrarModalCorreccion() {
    ocultarModal(modalCorreccion);

    formCorreccion.reset();

    inputIdAsistenciaCorreccion.value =
        "";

    limpiarMensajeCorreccion();
}

async function guardarCorreccion(event) {
    event.preventDefault();

    limpiarMensajeCorreccion();

    const idAsistencia =
        Number(
            inputIdAsistenciaCorreccion.value
        );

    const entrada =
        inputEntradaCorreccion.value;

    const salida =
        inputSalidaCorreccion.value;

    const observacion =
        inputObservacionCorreccion.value
            .trim();

    const motivoCorreccion =
        inputMotivoCorreccion.value
            .trim();

    if (!idAsistencia) {
        mostrarMensajeCorreccion(
            "La asistencia seleccionada no es válida."
        );

        return;
    }

    if (!entrada) {
        mostrarMensajeCorreccion(
            "Debe ingresar la fecha y hora de entrada."
        );

        inputEntradaCorreccion.focus();
        return;
    }

    if (
        salida &&
        new Date(salida).getTime() <=
        new Date(entrada).getTime()
    ) {
        mostrarMensajeCorreccion(
            "La hora de salida debe ser posterior a la hora de entrada."
        );

        inputSalidaCorreccion.focus();
        return;
    }

    if (!motivoCorreccion) {
        mostrarMensajeCorreccion(
            "Debe indicar el motivo de la corrección."
        );

        inputMotivoCorreccion.focus();
        return;
    }

    if (motivoCorreccion.length < 5) {
        mostrarMensajeCorreccion(
            "El motivo debe contener al menos 5 caracteres."
        );

        inputMotivoCorreccion.focus();
        return;
    }

    const idAdministrador =
        obtenerIdAdministradorSesion();

    if (!idAdministrador) {
        mostrarMensajeCorreccion(
            "No se encontró el administrador de la sesión. Cierre sesión e ingrese nuevamente."
        );

        return;
    }

    bloquearFormularioCorreccion(true);

    try {
        await corregirAsistencia(
            idAsistencia,
            {
                horaEntrada:
                    convertirAUtc(entrada),

                horaSalida:
                    salida
                        ? convertirAUtc(salida)
                        : null,

                observacion:
                    observacion || null,

                motivoCorreccion,

                idAdministrador
            }
        );

        cerrarModalCorreccion();

        mostrarMensajePrincipal(
            "Asistencia corregida correctamente.",
            "exito"
        );

        await cargarInformacion();
    } catch (error) {
        console.error(error);

        mostrarMensajeCorreccion(
            error.message ||
            "No fue posible corregir la asistencia."
        );
    } finally {
        bloquearFormularioCorreccion(false);
    }
}

function bloquearFormularioCorreccion(
    bloqueado
) {
    inputEntradaCorreccion.disabled =
        bloqueado;

    inputSalidaCorreccion.disabled =
        bloqueado;

    inputObservacionCorreccion.disabled =
        bloqueado;

    inputMotivoCorreccion.disabled =
        bloqueado;

    botonCerrarCorreccion.disabled =
        bloqueado;

    botonCancelarCorreccion.disabled =
        bloqueado;

    botonGuardarCorreccion.disabled =
        bloqueado;

    botonGuardarCorreccion.textContent =
        bloqueado
            ? "Guardando..."
            : "Guardar corrección";
}

function obtenerIdAdministradorSesion() {
    const administrador =
        obtenerAdministrador();

    const idGuardado =
        administrador?.idAdministrador ??
        administrador?.IdAdministrador ??
        administrador?.id ??
        administrador?.Id;

    const idAdministrador =
        Number(idGuardado);

    if (
        Number.isInteger(idAdministrador) &&
        idAdministrador > 0
    ) {
        return idAdministrador;
    }

    return obtenerIdAdministradorDelToken();
}

function obtenerIdAdministradorDelToken() {
    const token =
        obtenerToken();

    if (!token) {
        return null;
    }

    try {
        const partes =
            token.split(".");

        if (partes.length < 2) {
            return null;
        }

        let payloadBase64 =
            partes[1]
                .replace(/-/g, "+")
                .replace(/_/g, "/");

        while (
            payloadBase64.length % 4 !== 0
        ) {
            payloadBase64 += "=";
        }

        const payload =
            JSON.parse(
                decodeURIComponent(
                    Array.prototype.map
                        .call(
                            atob(payloadBase64),
                            caracter =>
                                `%${caracter
                                    .charCodeAt(0)
                                    .toString(16)
                                    .padStart(2, "0")}`
                        )
                        .join("")
                )
            );

        const valorId =
            payload.idAdministrador ??
            payload.IdAdministrador ??
            payload.nameid ??
            payload.sub ??
            payload[
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            ];

        const idAdministrador =
            Number(valorId);

        return (
            Number.isInteger(idAdministrador) &&
            idAdministrador > 0
        )
            ? idAdministrador
            : null;
    } catch {
        return null;
    }
}

/* =========================
   MODALES
   ========================= */

function mostrarModal(modal) {
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

function ocultarModal(modal) {
    modal.classList.add(
        "modal--oculto"
    );

    modal.setAttribute(
        "aria-hidden",
        "true"
    );

    document.body.style.overflow =
        "";
}

function manejarTeclaEscape(event) {
    if (event.key !== "Escape") {
        return;
    }

    if (
        !modalAsistencia.classList.contains(
            "modal--oculto"
        )
    ) {
        cerrarModalManual();
    }

    if (
        !modalCorreccion.classList.contains(
            "modal--oculto"
        )
    ) {
        cerrarModalCorreccion();
    }
}

/* =========================
   INFORMACIÓN DE EMPLEADOS
   ========================= */

function obtenerNombreEmpleado(idEmpleado) {
    const empleado =
        empleados.find(
            item =>
                Number(item.idEmpleado) ===
                Number(idEmpleado)
        );

    return empleado
        ? obtenerNombreCompleto(empleado)
        : "Empleado no disponible";
}

function obtenerCodigoEmpleado(idEmpleado) {
    const empleado =
        empleados.find(
            item =>
                Number(item.idEmpleado) ===
                Number(idEmpleado)
        );

    return empleado?.codigoEmpleado || "";
}

function obtenerNombreCompleto(empleado) {
    return `${empleado.nombres} ${empleado.apellidos}`
        .trim();
}

/* =========================
   FECHAS Y HORAS
   ========================= */

function formatearFecha(valor) {
    const fecha =
        convertirFecha(valor);

    if (!fecha) {
        return "No disponible";
    }

    return fecha.toLocaleDateString(
        "es-NI",
        {
            day: "2-digit",
            month: "2-digit",
            year: "numeric"
        }
    );
}

function formatearHora(valor) {
    const fecha =
        convertirFecha(valor);

    if (!fecha) {
        return "--:--";
    }

    return fecha.toLocaleTimeString(
        "es-NI",
        {
            hour: "2-digit",
            minute: "2-digit"
        }
    );
}

function calcularDuracion(
    entrada,
    salida
) {
    if (!entrada || !salida) {
        return "Pendiente";
    }

    const fechaEntrada =
        convertirFecha(entrada);

    const fechaSalida =
        convertirFecha(salida);

    if (!fechaEntrada || !fechaSalida) {
        return "No disponible";
    }

    const diferencia =
        fechaSalida.getTime() -
        fechaEntrada.getTime();

    if (diferencia <= 0) {
        return "No disponible";
    }

    const minutosTotales =
        Math.floor(
            diferencia / 60000
        );

    const horas =
        Math.floor(
            minutosTotales / 60
        );

    const minutos =
        minutosTotales % 60;

    return `${horas} h ${minutos} min`;
}

function convertirFecha(valor) {
    if (!valor) {
        return null;
    }

    const fecha =
        new Date(valor);

    if (Number.isNaN(fecha.getTime())) {
        return null;
    }

    return fecha;
}

function obtenerTiempo(valor) {
    const fecha =
        convertirFecha(valor);

    return fecha
        ? fecha.getTime()
        : 0;
}

function obtenerFechaLocal(valor) {
    const fecha =
        convertirFecha(valor);

    if (!fecha) {
        return "";
    }

    return obtenerFechaInput(fecha);
}

function obtenerFechaInput(fecha) {
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

function obtenerFechaHoraLocalActual() {
    const ahora =
        new Date();

    ahora.setSeconds(0, 0);

    return convertirFechaAInput(ahora);
}

function convertirFechaAInput(valor) {
    const fecha =
        valor instanceof Date
            ? valor
            : new Date(valor);

    if (Number.isNaN(fecha.getTime())) {
        return "";
    }

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

    const horas =
        String(
            fecha.getHours()
        ).padStart(2, "0");

    const minutos =
        String(
            fecha.getMinutes()
        ).padStart(2, "0");

    return `${anio}-${mes}-${dia}T${horas}:${minutos}`;
}

function convertirAUtc(valorLocal) {
    const fecha =
        new Date(valorLocal);

    if (Number.isNaN(fecha.getTime())) {
        throw new Error(
            "La fecha ingresada no es válida."
        );
    }

    return fecha.toISOString();
}

/* =========================
   CARGA Y MENSAJES
   ========================= */

function mostrarCargando(cargando) {
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

function mostrarMensajePrincipal(
    mensaje,
    tipo
) {
    clearTimeout(temporizadorMensaje);

    mensajeAsistencias.textContent =
        mensaje;

    mensajeAsistencias.className =
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
    clearTimeout(temporizadorMensaje);

    mensajeAsistencias.textContent = "";

    mensajeAsistencias.className =
        "mensaje mensaje--oculto";
}

function mostrarMensajeFormulario(mensaje) {
    mensajeFormulario.textContent =
        mensaje;

    mensajeFormulario.className =
        "mensaje mensaje--error mensaje--modal";
}

function limpiarMensajeFormulario() {
    mensajeFormulario.textContent = "";

    mensajeFormulario.className =
        "mensaje mensaje--oculto mensaje--modal";
}

function mostrarMensajeCorreccion(mensaje) {
    mensajeCorreccion.textContent =
        mensaje;

    mensajeCorreccion.className =
        "mensaje mensaje--error mensaje--modal";
}

function limpiarMensajeCorreccion() {
    mensajeCorreccion.textContent = "";

    mensajeCorreccion.className =
        "mensaje mensaje--oculto mensaje--modal";
}


function escaparHtml(texto) {
    const elemento =
        document.createElement("div");

    elemento.textContent =
        texto ?? "";

    return elemento.innerHTML;
}

function escaparAtributo(texto) {
    return String(texto ?? "")
        .replace(/&/g, "&amp;")
        .replace(/"/g, "&quot;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
}