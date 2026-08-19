import {
    protegerPagina
} from "../../shared/js/auth.js";

import {
    inicializarLayout
} from "../../shared/js/layout.js";

import {
    obtenerEmpleados,
    obtenerEmpleado,
    obtenerCargosActivos,
    obtenerDepartamentosActivos,
    obtenerHorariosLaboralesActivos,
    crearEmpleado,
    actualizarEmpleado,
    cambiarEstadoEmpleado,
    cambiarPinEmpleado
} from "../empleados/empleados.services.js";

const MAXIMO_EMPLEADOS_ACTIVOS = 10;


const botonNuevoEmpleado =
    document.getElementById("btnNuevoEmpleado");

const botonActualizar =
    document.getElementById("btnActualizar");

const inputBuscar =
    document.getElementById("buscarEmpleado");

const filtroEstado =
    document.getElementById("filtroEstado");

const contadorEmpleados =
    document.getElementById("contadorEmpleados");

const tablaEmpleadosBody =
    document.getElementById("tablaEmpleadosBody");

const contenedorTabla =
    document.getElementById("contenedorTabla");

const estadoCarga =
    document.getElementById("estadoCarga");

const sinResultados =
    document.getElementById("sinResultados");

const mensajeEmpleados =
    document.getElementById("mensajeEmpleados");

/* Modal empleado */

const modalEmpleado =
    document.getElementById("modalEmpleado");

const fondoModalEmpleado =
    document.getElementById("fondoModalEmpleado");

const tituloModalEmpleado =
    document.getElementById("tituloModalEmpleado");

const botonCerrarModalEmpleado =
    document.getElementById("btnCerrarModalEmpleado");

const botonCancelarEmpleado =
    document.getElementById("btnCancelarEmpleado");

const botonGuardarEmpleado =
    document.getElementById("btnGuardarEmpleado");

const formEmpleado =
    document.getElementById("formEmpleado");

const inputIdEmpleado =
    document.getElementById("idEmpleado");

const inputCodigoEmpleado =
    document.getElementById("codigoEmpleado");

const selectCargo =
    document.getElementById("idCargo");

const selectDepartamento =
    document.getElementById("idDepartamento");

const selectHorarioLaboral =
    document.getElementById("idHorarioLaboral");

const inputNumeroINSS =
    document.getElementById("numeroINSS");

const inputNombres =
    document.getElementById("nombres");

const inputApellidos =
    document.getElementById("apellidos");

const inputIdentificacion =
    document.getElementById("identificacion");

const inputTelefono =
    document.getElementById("telefono");

const inputCorreo =
    document.getElementById("correo");

const inputDireccion =
    document.getElementById("direccion");

const inputFechaContratacion =
    document.getElementById("fechaContratacion");

const inputSalarioBase =
    document.getElementById("salarioBase");

const grupoPin =
    document.getElementById("grupoPin");

const inputPin =
    document.getElementById("pin");

const mensajeFormulario =
    document.getElementById("mensajeFormulario");

/* Modal PIN */

const modalPin =
    document.getElementById("modalPin");

const fondoModalPin =
    document.getElementById("fondoModalPin");

const formPin =
    document.getElementById("formPin");

const inputIdEmpleadoPin =
    document.getElementById("idEmpleadoPin");

const nombreEmpleadoPin =
    document.getElementById("nombreEmpleadoPin");

const inputNuevoPin =
    document.getElementById("nuevoPin");

const botonCerrarModalPin =
    document.getElementById("btnCerrarModalPin");

const botonCancelarPin =
    document.getElementById("btnCancelarPin");

const botonGuardarPin =
    document.getElementById("btnGuardarPin");

const mensajePin =
    document.getElementById("mensajePin");

let empleados = [];
let cargos = [];
let departamentos = [];
let horariosLaborales = [];
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
        titulo: "Empleados",
        paginaActiva: "empleados"
    });

    configurarEventos();

    await cargarInformacion();
}

function configurarEventos() {
    botonNuevoEmpleado.addEventListener(
        "click",
        abrirModalNuevo
    );

    botonActualizar.addEventListener(
        "click",
        cargarInformacion
    );

    inputBuscar.addEventListener(
        "input",
        aplicarFiltros
    );

    filtroEstado.addEventListener(
        "change",
        aplicarFiltros
    );

    formEmpleado.addEventListener(
        "submit",
        guardarEmpleado
    );

    botonCerrarModalEmpleado.addEventListener(
        "click",
        cerrarModalEmpleado
    );

    botonCancelarEmpleado.addEventListener(
        "click",
        cerrarModalEmpleado
    );

    fondoModalEmpleado.addEventListener(
        "click",
        cerrarModalEmpleado
    );

    formPin.addEventListener(
        "submit",
        guardarNuevoPin
    );

    botonCerrarModalPin.addEventListener(
        "click",
        cerrarModalPin
    );

    botonCancelarPin.addEventListener(
        "click",
        cerrarModalPin
    );

    fondoModalPin.addEventListener(
        "click",
        cerrarModalPin
    );

    document.addEventListener(
        "keydown",
        manejarTeclaEscape
    );
}

async function cargarInformacion() {
    mostrarCargando(true);
    ocultarMensajePrincipal();

    botonActualizar.disabled = true;
    botonActualizar.textContent = "Actualizando...";

    try {
        const resultados =
            await Promise.all([
                obtenerEmpleados(),
                obtenerCargosActivos(),
                obtenerDepartamentosActivos(),
                obtenerHorariosLaboralesActivos()
            ]);

        empleados = resultados[0];
        cargos = resultados[1];
        departamentos = resultados[2];
        horariosLaborales = resultados[3];

        llenarSelectCargos();
        llenarSelectDepartamentos();
        llenarSelectHorariosLaborales();
        actualizarContador();
        aplicarFiltros();
    } catch (error) {
        console.error(error);

        empleados = [];
        cargos = [];

        renderizarEmpleados([]);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cargar los empleados.",
            "error"
        );
    } finally {
        mostrarCargando(false);

        botonActualizar.disabled = false;
        botonActualizar.textContent = "Actualizar";
    }
}

function aplicarFiltros() {
    const texto =
        inputBuscar.value
            .trim()
            .toLowerCase();

    const estado =
        filtroEstado.value;

    const resultados =
        empleados.filter(empleado => {
            const nombreCompleto =
                `${empleado.nombres} ${empleado.apellidos}`
                    .toLowerCase();

            const coincideTexto =
                !texto ||
                empleado.codigoEmpleado
                    .toLowerCase()
                    .includes(texto) ||
                nombreCompleto.includes(texto) ||
                empleado.identificacion
                    .toLowerCase()
                    .includes(texto) ||
                empleado.nombreCargo
                    .toLowerCase()
                    .includes(texto);

            const coincideEstado =
                estado === "todos" ||
                (
                    estado === "activo" &&
                    empleado.activo
                ) ||
                (
                    estado === "inactivo" &&
                    !empleado.activo
                );

            return coincideTexto && coincideEstado;
        });

    renderizarEmpleados(resultados);
}

function renderizarEmpleados(lista) {
    tablaEmpleadosBody.innerHTML = "";

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

    lista.forEach(empleado => {
        const fila =
            document.createElement("tr");

        const nombreCompleto =
            `${empleado.nombres} ${empleado.apellidos}`;

        const claseEstado =
            empleado.activo
                ? "estado estado--activo"
                : "estado estado--inactivo";

        const textoEstado =
            empleado.activo
                ? "Activo"
                : "Inactivo";

        const claseBotonEstado =
            empleado.activo
                ? "boton-tabla boton-tabla--desactivar"
                : "boton-tabla boton-tabla--activar";

        const textoBotonEstado =
            empleado.activo
                ? "Desactivar"
                : "Activar";

        fila.innerHTML = `
            <td>
                <strong>
                    ${escaparHtml(empleado.codigoEmpleado)}
                </strong>
            </td>

            <td>
                <span class="empleado-nombre">
                    ${escaparHtml(nombreCompleto)}
                </span>

                <span class="empleado-correo">
                    ${escaparHtml(
                        empleado.correo || "Sin correo"
                    )}
                </span>
            </td>

            <td>
                ${escaparHtml(empleado.identificacion)}
            </td>

            <td>
                ${escaparHtml(empleado.nombreCargo)}
            </td>

            <td>
                ${formatearFecha(
                    empleado.fechaContratacion
                )}
            </td>

            <td>
                ${formatearMoneda(
                    empleado.salarioBase
                )}
            </td>

            <td>
                <span class="${claseEstado}">
                    ${textoEstado}
                </span>
            </td>

            <td>
                <div class="celda-acciones">
                    <button
                        type="button"
                        class="boton-tabla boton-tabla--editar"
                        data-accion="editar"
                        data-id="${empleado.idEmpleado}"
                    >
                        Editar
                    </button>

                    <button
                        type="button"
                        class="boton-tabla boton-tabla--pin"
                        data-accion="pin"
                        data-id="${empleado.idEmpleado}"
                    >
                        Cambiar PIN
                    </button>

                    <button
                        type="button"
                        class="${claseBotonEstado}"
                        data-accion="estado"
                        data-id="${empleado.idEmpleado}"
                    >
                        ${textoBotonEstado}
                    </button>
                </div>
            </td>
        `;

        tablaEmpleadosBody.appendChild(fila);
    });

    configurarBotonesTabla();
}

function configurarBotonesTabla() {
    document
        .querySelectorAll('[data-accion="editar"]')
        .forEach(boton => {
            boton.addEventListener(
                "click",
                () => abrirModalEditar(
                    Number(boton.dataset.id)
                )
            );
        });

    document
        .querySelectorAll('[data-accion="pin"]')
        .forEach(boton => {
            boton.addEventListener(
                "click",
                () => abrirModalPin(
                    Number(boton.dataset.id)
                )
            );
        });

    document
        .querySelectorAll('[data-accion="estado"]')
        .forEach(boton => {
            boton.addEventListener(
                "click",
                () => procesarCambioEstado(
                    Number(boton.dataset.id),
                    boton
                )
            );
        });
}

function abrirModalNuevo() {
    const activos =
        empleados.filter(
            empleado => empleado.activo
        ).length;

    if (
        activos >=
        MAXIMO_EMPLEADOS_ACTIVOS
    ) {
        mostrarMensajePrincipal(
            "Ya existen 10 empleados activos. Debe desactivar uno antes de registrar otro.",
            "error"
        );

        return;
    }

    formEmpleado.reset();

    inputIdEmpleado.value = "";

    tituloModalEmpleado.textContent =
        "Nuevo empleado";

    botonGuardarEmpleado.textContent =
        "Guardar";

    grupoPin.style.display = "block";
    inputPin.required = true;

    limpiarMensajeFormulario();

    mostrarModal(modalEmpleado);

    inputCodigoEmpleado.focus();
}

async function abrirModalEditar(idEmpleado) {
    limpiarMensajeFormulario();

    try {
        const empleado =
            await obtenerEmpleado(idEmpleado);

        if (!empleado) {
            throw new Error(
                "No se encontró el empleado."
            );
        }

        formEmpleado.reset();

        inputIdEmpleado.value =
            empleado.idEmpleado;

        inputCodigoEmpleado.value =
            empleado.codigoEmpleado;

        selectCargo.value =
            empleado.idCargo;

        selectDepartamento.value =
            empleado.idDepartamento ?? "";

        selectHorarioLaboral.value =
            empleado.idHorarioLaboral ?? "";

        inputNumeroINSS.value =
            empleado.numeroINSS ?? "";

        inputNombres.value =
            empleado.nombres;

        inputApellidos.value =
            empleado.apellidos;

        inputIdentificacion.value =
            empleado.identificacion;

        inputTelefono.value =
            empleado.telefono;

        inputCorreo.value =
            empleado.correo;

        inputDireccion.value =
            empleado.direccion;

        inputFechaContratacion.value =
            obtenerFechaInput(
                empleado.fechaContratacion
            );

        inputSalarioBase.value =
            empleado.salarioBase;

        tituloModalEmpleado.textContent =
            "Editar empleado";

        botonGuardarEmpleado.textContent =
            "Guardar cambios";

        grupoPin.style.display = "none";
        inputPin.required = false;
        inputPin.value = "";

        mostrarModal(modalEmpleado);

        inputCodigoEmpleado.focus();
    } catch (error) {
        console.error(error);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible consultar el empleado.",
            "error"
        );
    }
}

async function guardarEmpleado(event) {
    event.preventDefault();

    limpiarMensajeFormulario();

    const idEmpleado =
        inputIdEmpleado.value.trim();

    const datos = obtenerDatosFormulario();

    const errorValidacion =
        validarEmpleado(datos, Boolean(idEmpleado));

    if (errorValidacion) {
        mostrarMensajeFormulario(
            errorValidacion
        );

        return;
    }

    bloquearFormularioEmpleado(true);

    try {
        if (idEmpleado) {
            await actualizarEmpleado(
                Number(idEmpleado),
                datos
            );

            cerrarModalEmpleado();

            mostrarMensajePrincipal(
                "Empleado actualizado correctamente.",
                "exito"
            );
        } else {
            await crearEmpleado(datos);

            cerrarModalEmpleado();

            mostrarMensajePrincipal(
                "Empleado registrado correctamente.",
                "exito"
            );
        }

        await cargarInformacion();
    } catch (error) {
        console.error(error);

        mostrarMensajeFormulario(
            error.message ||
            "No fue posible guardar el empleado."
        );
    } finally {
        bloquearFormularioEmpleado(false);
    }
}

function obtenerDatosFormulario() {
    return {
        idCargo:
            Number(selectCargo.value),

        idDepartamento:
            selectDepartamento.value
                ? Number(selectDepartamento.value)
                : null,

        idHorarioLaboral:
            selectHorarioLaboral.value
                ? Number(selectHorarioLaboral.value)
                : null,

        codigoEmpleado:
            inputCodigoEmpleado.value
                .trim()
                .toUpperCase(),

        pin:
            inputPin.value.trim(),

        identificacion:
            inputIdentificacion.value.trim(),

        numeroINSS:
            inputNumeroINSS.value.trim() || null,

        nombres:
            inputNombres.value.trim(),

        apellidos:
            inputApellidos.value.trim(),

        telefono:
            inputTelefono.value.trim() || null,

        correo:
            inputCorreo.value
                .trim()
                .toLowerCase() || null,

        direccion:
            inputDireccion.value.trim() || null,

        fechaContratacion:
            inputFechaContratacion.value,

        salarioBase:
            Number(inputSalarioBase.value)
    };
}

function validarEmpleado(datos, editando) {
    if (!datos.codigoEmpleado) {
        return "Debe ingresar el código del empleado.";
    }

    if (!datos.idCargo) {
        return "Debe seleccionar un cargo.";
    }

    if (!datos.nombres) {
        return "Debe ingresar los nombres.";
    }

    if (!datos.apellidos) {
        return "Debe ingresar los apellidos.";
    }

    if (!datos.identificacion) {
        return "Debe ingresar la identificación.";
    }

    if (!datos.fechaContratacion) {
        return "Debe seleccionar la fecha de contratación.";
    }

    if (
        !Number.isFinite(datos.salarioBase) ||
        datos.salarioBase <= 0
    ) {
        return "El salario base debe ser mayor que cero.";
    }

    if (
        datos.correo &&
        !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(
            datos.correo
        )
    ) {
        return "Ingrese un correo electrónico válido.";
    }

    if (
        !editando &&
        !/^[0-9]{4,6}$/.test(datos.pin)
    ) {
        return "El PIN debe contener entre 4 y 6 números.";
    }

    return null;
}

async function procesarCambioEstado(
    idEmpleado,
    boton
) {
    const empleado =
        empleados.find(
            item =>
                Number(item.idEmpleado) ===
                Number(idEmpleado)
        );

    if (!empleado) {
        return;
    }

    const nuevoEstado =
        !empleado.activo;

    if (nuevoEstado) {
        const activos =
            empleados.filter(
                item => item.activo
            ).length;

        if (
            activos >=
            MAXIMO_EMPLEADOS_ACTIVOS
        ) {
            mostrarMensajePrincipal(
                "No se pueden tener más de 10 empleados activos.",
                "error"
            );

            return;
        }
    }

    const accion =
        nuevoEstado
            ? "activar"
            : "desactivar";

    const nombreCompleto =
        `${empleado.nombres} ${empleado.apellidos}`;

    const confirmado =
        window.confirm(
            `¿Está seguro de ${accion} a ${nombreCompleto}?`
        );

    if (!confirmado) {
        return;
    }

    boton.disabled = true;

    try {
        await cambiarEstadoEmpleado(
            empleado.idEmpleado,
            nuevoEstado
        );

        mostrarMensajePrincipal(
            nuevoEstado
                ? "Empleado activado correctamente."
                : "Empleado desactivado correctamente.",
            "exito"
        );

        await cargarInformacion();
    } catch (error) {
        console.error(error);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cambiar el estado.",
            "error"
        );

        boton.disabled = false;
    }
}

function abrirModalPin(idEmpleado) {
    const empleado =
        empleados.find(
            item =>
                Number(item.idEmpleado) ===
                Number(idEmpleado)
        );

    if (!empleado) {
        return;
    }

    formPin.reset();

    inputIdEmpleadoPin.value =
        empleado.idEmpleado;

    nombreEmpleadoPin.textContent =
        `${empleado.nombres} ${empleado.apellidos}`;

    limpiarMensajePin();

    mostrarModal(modalPin);

    inputNuevoPin.focus();
}

async function guardarNuevoPin(event) {
    event.preventDefault();

    limpiarMensajePin();

    const idEmpleado =
        Number(inputIdEmpleadoPin.value);

    const nuevoPin =
        inputNuevoPin.value.trim();

    if (!/^[0-9]{4,6}$/.test(nuevoPin)) {
        mostrarMensajePin(
            "El PIN debe contener entre 4 y 6 números."
        );

        inputNuevoPin.focus();
        return;
    }

    bloquearFormularioPin(true);

    try {
        await cambiarPinEmpleado(
            idEmpleado,
            nuevoPin
        );

        cerrarModalPin();

        mostrarMensajePrincipal(
            "PIN actualizado correctamente.",
            "exito"
        );
    } catch (error) {
        console.error(error);

        mostrarMensajePin(
            error.message ||
            "No fue posible cambiar el PIN."
        );
    } finally {
        bloquearFormularioPin(false);
    }
}

function llenarSelectCargos() {
    selectCargo.innerHTML = `
        <option value="">
            Seleccione un cargo
        </option>
    `;

    cargos.forEach(cargo => {
        const opcion =
            document.createElement("option");

        opcion.value =
            cargo.idCargo;

        opcion.textContent =
            cargo.nombre;

        selectCargo.appendChild(opcion);
    });
}

function llenarSelectDepartamentos() {
    selectDepartamento.innerHTML = `
        <option value="">Seleccione un departamento</option>
    `;

    departamentos.forEach(departamento => {
        const opcion = document.createElement("option");
        opcion.value = departamento.idDepartamento;
        opcion.textContent = departamento.nombre;
        selectDepartamento.appendChild(opcion);
    });
}

function llenarSelectHorariosLaborales() {
    selectHorarioLaboral.innerHTML = `
        <option value="">Seleccione un horario</option>
    `;

    horariosLaborales.forEach(horario => {
        const opcion = document.createElement("option");
        opcion.value = horario.idHorarioLaboral;
        opcion.textContent = horario.nombre;
        selectHorarioLaboral.appendChild(opcion);
    });
}

function actualizarContador() {
    const activos =
        empleados.filter(
            empleado => empleado.activo
        ).length;

    contadorEmpleados.textContent =
        `${empleados.length} registrados · ${activos}/10 activos`;
}

function cerrarModalEmpleado() {
    ocultarModal(modalEmpleado);

    formEmpleado.reset();

    inputIdEmpleado.value = "";

    limpiarMensajeFormulario();
}

function cerrarModalPin() {
    ocultarModal(modalPin);

    formPin.reset();

    inputIdEmpleadoPin.value = "";

    limpiarMensajePin();
}

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
        !modalEmpleado.classList.contains(
            "modal--oculto"
        )
    ) {
        cerrarModalEmpleado();
    }

    if (
        !modalPin.classList.contains(
            "modal--oculto"
        )
    ) {
        cerrarModalPin();
    }
}

function bloquearFormularioEmpleado(bloqueado) {
    const campos =
        formEmpleado.querySelectorAll(
            "input, select, button"
        );

    campos.forEach(campo => {
        campo.disabled = bloqueado;
    });

    botonGuardarEmpleado.textContent =
        bloqueado
            ? "Guardando..."
            : inputIdEmpleado.value
                ? "Guardar cambios"
                : "Guardar";
}

function bloquearFormularioPin(bloqueado) {
    inputNuevoPin.disabled = bloqueado;
    botonCancelarPin.disabled = bloqueado;
    botonCerrarModalPin.disabled = bloqueado;
    botonGuardarPin.disabled = bloqueado;

    botonGuardarPin.textContent =
        bloqueado
            ? "Guardando..."
            : "Cambiar PIN";
}

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

    mensajeEmpleados.textContent =
        mensaje;

    mensajeEmpleados.className =
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

    mensajeEmpleados.textContent = "";

    mensajeEmpleados.className =
        "mensaje mensaje--oculto";
}

function mostrarMensajeFormulario(mensaje) {
    mensajeFormulario.textContent =
        mensaje;

    mensajeFormulario.className =
        "mensaje mensaje--error";
}

function limpiarMensajeFormulario() {
    mensajeFormulario.textContent = "";

    mensajeFormulario.className =
        "mensaje mensaje--oculto";
}

function mostrarMensajePin(mensaje) {
    mensajePin.textContent =
        mensaje;

    mensajePin.className =
        "mensaje mensaje--error";
}

function limpiarMensajePin() {
    mensajePin.textContent = "";

    mensajePin.className =
        "mensaje mensaje--oculto";
}

function formatearFecha(fecha) {
    if (!fecha) {
        return "No disponible";
    }

    const fechaLimpia =
        String(fecha).substring(0, 10);

    const partes =
        fechaLimpia.split("-");

    if (partes.length !== 3) {
        return fechaLimpia;
    }

    return `${partes[2]}/${partes[1]}/${partes[0]}`;
}

function obtenerFechaInput(fecha) {
    if (!fecha) {
        return "";
    }

    return String(fecha).substring(0, 10);
}

function formatearMoneda(valor) {
    const numero =
        Number(valor);

    if (!Number.isFinite(numero)) {
        return "C$ 0.00";
    }

    return new Intl.NumberFormat(
        "es-NI",
        {
            style: "currency",
            currency: "NIO",
            minimumFractionDigits: 2
        }
    ).format(numero);
}

function escaparHtml(texto) {
    const elemento =
        document.createElement("div");

    elemento.textContent =
        texto ?? "";

    return elemento.innerHTML;
}