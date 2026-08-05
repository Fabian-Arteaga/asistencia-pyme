import {
    protegerPagina,
    obtenerAdministrador
} from "../../shared/js/auth.js";

import {
    inicializarLayout
} from "../../shared/js/layout.js";

import {
    obtenerAdministradores,
    obtenerAdministradorPorId,
    crearAdministrador,
    actualizarAdministrador,
    cambiarEstadoAdministrador,
    cambiarMiContrasena
} from "../administradores/administradores.services.js";

/* =========================
   ELEMENTOS PRINCIPALES
   ========================= */

const botonNuevoAdministrador =
    document.getElementById(
        "btnNuevoAdministrador"
    );

const botonCambiarContrasena =
    document.getElementById(
        "btnCambiarContrasena"
    );

const botonActualizar =
    document.getElementById(
        "btnActualizar"
    );

const inputBuscar =
    document.getElementById(
        "buscarAdministrador"
    );

const filtroEstado =
    document.getElementById(
        "filtroEstado"
    );

const contadorAdministradores =
    document.getElementById(
        "contadorAdministradores"
    );

const tablaAdministradoresBody =
    document.getElementById(
        "tablaAdministradoresBody"
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

const mensajeAdministradores =
    document.getElementById(
        "mensajeAdministradores"
    );

/* =========================
   MODAL DE ADMINISTRADOR
   ========================= */

const modalAdministrador =
    document.getElementById(
        "modalAdministrador"
    );

const fondoModalAdministrador =
    document.getElementById(
        "fondoModalAdministrador"
    );

const formAdministrador =
    document.getElementById(
        "formAdministrador"
    );

const inputIdAdministrador =
    document.getElementById(
        "idAdministrador"
    );

const inputNombres =
    document.getElementById(
        "nombresAdministrador"
    );

const inputApellidos =
    document.getElementById(
        "apellidosAdministrador"
    );

const inputCorreo =
    document.getElementById(
        "correoAdministrador"
    );

const grupoContrasena =
    document.getElementById(
        "grupoContrasena"
    );

const grupoConfirmarContrasena =
    document.getElementById(
        "grupoConfirmarContrasena"
    );

const inputContrasena =
    document.getElementById(
        "contrasenaAdministrador"
    );

const inputConfirmarContrasena =
    document.getElementById(
        "confirmarContrasenaAdministrador"
    );

const tituloModalAdministrador =
    document.getElementById(
        "tituloModalAdministrador"
    );

const botonCerrarModalAdministrador =
    document.getElementById(
        "btnCerrarModalAdministrador"
    );

const botonCancelarAdministrador =
    document.getElementById(
        "btnCancelarAdministrador"
    );

const botonGuardarAdministrador =
    document.getElementById(
        "btnGuardarAdministrador"
    );

const mensajeFormulario =
    document.getElementById(
        "mensajeFormulario"
    );

/* =========================
   MODAL DE CONTRASEÑA
   ========================= */

const modalContrasena =
    document.getElementById(
        "modalContrasena"
    );

const fondoModalContrasena =
    document.getElementById(
        "fondoModalContrasena"
    );

const formContrasena =
    document.getElementById(
        "formContrasena"
    );

const inputContrasenaActual =
    document.getElementById(
        "contrasenaActual"
    );

const inputNuevaContrasena =
    document.getElementById(
        "nuevaContrasena"
    );

const inputConfirmarNuevaContrasena =
    document.getElementById(
        "confirmarNuevaContrasena"
    );

const botonCerrarModalContrasena =
    document.getElementById(
        "btnCerrarModalContrasena"
    );

const botonCancelarContrasena =
    document.getElementById(
        "btnCancelarContrasena"
    );

const botonGuardarContrasena =
    document.getElementById(
        "btnGuardarContrasena"
    );

const mensajeContrasena =
    document.getElementById(
        "mensajeContrasena"
    );

/* =========================
   VARIABLES
   ========================= */

const ADMIN_KEY =
    "asistenciaPyme_administrador";

let administradores = [];
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
        titulo: "Administradores",
        paginaActiva: "administradores"
    });

    configurarEventos();

    await cargarAdministradores();
}

function configurarEventos() {
    botonNuevoAdministrador.addEventListener(
        "click",
        abrirModalNuevo
    );

    botonCambiarContrasena.addEventListener(
        "click",
        abrirModalContrasena
    );

    botonActualizar.addEventListener(
        "click",
        cargarAdministradores
    );

    inputBuscar.addEventListener(
        "input",
        aplicarFiltros
    );

    filtroEstado.addEventListener(
        "change",
        aplicarFiltros
    );

    formAdministrador.addEventListener(
        "submit",
        guardarAdministrador
    );

    botonCerrarModalAdministrador.addEventListener(
        "click",
        cerrarModalAdministrador
    );

    botonCancelarAdministrador.addEventListener(
        "click",
        cerrarModalAdministrador
    );

    fondoModalAdministrador.addEventListener(
        "click",
        cerrarModalAdministrador
    );

    formContrasena.addEventListener(
        "submit",
        guardarCambioContrasena
    );

    botonCerrarModalContrasena.addEventListener(
        "click",
        cerrarModalContrasena
    );

    botonCancelarContrasena.addEventListener(
        "click",
        cerrarModalContrasena
    );

    fondoModalContrasena.addEventListener(
        "click",
        cerrarModalContrasena
    );

    document.addEventListener(
        "keydown",
        manejarTeclaEscape
    );
}

/* =========================
   CARGA
   ========================= */

async function cargarAdministradores(
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
        administradores =
            await obtenerAdministradores();

        ordenarAdministradores();
        actualizarContador();
        aplicarFiltros();
    } catch (error) {
        console.error(error);

        administradores = [];

        renderizarAdministradores([]);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cargar los administradores.",
            "error"
        );
    } finally {
        mostrarCargando(false);

        botonActualizar.disabled = false;
        botonActualizar.textContent =
            "Actualizar";
    }
}

function ordenarAdministradores() {
    administradores.sort(
        (a, b) =>
            a.nombreCompleto.localeCompare(
                b.nombreCompleto,
                "es"
            )
    );
}

/* =========================
   FILTROS
   ========================= */

function aplicarFiltros() {
    const texto =
        inputBuscar.value
            .trim()
            .toLowerCase();

    const estado =
        filtroEstado.value;

    const resultados =
        administradores.filter(
            administrador => {
                const coincideTexto =
                    !texto ||
                    administrador.nombreCompleto
                        .toLowerCase()
                        .includes(texto) ||
                    administrador.correo
                        .toLowerCase()
                        .includes(texto);

                const coincideEstado =
                    estado === "todos" ||
                    (
                        estado === "activo" &&
                        administrador.activo
                    ) ||
                    (
                        estado === "inactivo" &&
                        !administrador.activo
                    );

                return (
                    coincideTexto &&
                    coincideEstado
                );
            }
        );

    renderizarAdministradores(
        resultados
    );
}

/* =========================
   TABLA
   ========================= */

function renderizarAdministradores(lista) {
    tablaAdministradoresBody.innerHTML =
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

    const idAdministradorActual =
        obtenerIdAdministradorActual();

    lista.forEach(administrador => {
        const fila =
            document.createElement("tr");

        const esAdministradorActual =
            Number(
                administrador.idAdministrador
            ) === idAdministradorActual;

        const claseEstado =
            administrador.activo
                ? "estado estado--activo"
                : "estado estado--inactivo";

        const textoEstado =
            administrador.activo
                ? "Activo"
                : "Inactivo";

        const claseBotonEstado =
            administrador.activo
                ? "boton-tabla boton-tabla--desactivar"
                : "boton-tabla boton-tabla--activar";

        const textoBotonEstado =
            administrador.activo
                ? "Desactivar"
                : "Activar";

        const etiquetaActual =
            esAdministradorActual
                ? `
                    <span class="administrador-actual">
                        Cuenta actual
                    </span>
                `
                : "";

        fila.innerHTML = `
            <td>
                <span class="administrador-nombre">
                    ${escaparHtml(
                        administrador.nombreCompleto
                    )}
                </span>

                ${etiquetaActual}
            </td>

            <td>
                ${escaparHtml(
                    administrador.correo
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
                        data-id="${administrador.idAdministrador}"
                    >
                        Editar
                    </button>

                    <button
                        type="button"
                        class="${claseBotonEstado}"
                        data-accion="estado"
                        data-id="${administrador.idAdministrador}"
                        ${esAdministradorActual ? "disabled" : ""}
                    >
                        ${textoBotonEstado}
                    </button>
                </div>
            </td>
        `;

        tablaAdministradoresBody.appendChild(
            fila
        );
    });

    configurarBotonesTabla();
}

function configurarBotonesTabla() {
    document
        .querySelectorAll(
            '[data-accion="editar"]'
        )
        .forEach(boton => {
            boton.addEventListener(
                "click",
                () => {
                    abrirModalEditar(
                        Number(
                            boton.dataset.id
                        )
                    );
                }
            );
        });

    document
        .querySelectorAll(
            '[data-accion="estado"]'
        )
        .forEach(boton => {
            boton.addEventListener(
                "click",
                () => {
                    procesarCambioEstado(
                        Number(
                            boton.dataset.id
                        ),
                        boton
                    );
                }
            );
        });
}

/* =========================
   CREAR Y EDITAR
   ========================= */

function abrirModalNuevo() {
    formAdministrador.reset();

    inputIdAdministrador.value = "";

    tituloModalAdministrador.textContent =
        "Nuevo administrador";

    botonGuardarAdministrador.textContent =
        "Guardar";

    grupoContrasena.style.display =
        "block";

    grupoConfirmarContrasena.style.display =
        "block";

    inputContrasena.required = true;
    inputConfirmarContrasena.required = true;

    limpiarMensajeFormulario();

    mostrarModal(modalAdministrador);

    inputNombres.focus();
}

async function abrirModalEditar(
    idAdministrador
) {
    limpiarMensajeFormulario();

    try {
        const administrador =
            await obtenerAdministradorPorId(
                idAdministrador
            );

        if (!administrador) {
            throw new Error(
                "No se encontró el administrador."
            );
        }

        formAdministrador.reset();

        inputIdAdministrador.value =
            administrador.idAdministrador;

        inputNombres.value =
            administrador.nombres;

        inputApellidos.value =
            administrador.apellidos;

        inputCorreo.value =
            administrador.correo;

        tituloModalAdministrador.textContent =
            "Editar administrador";

        botonGuardarAdministrador.textContent =
            "Guardar cambios";

        grupoContrasena.style.display =
            "none";

        grupoConfirmarContrasena.style.display =
            "none";

        inputContrasena.required = false;
        inputConfirmarContrasena.required =
            false;

        inputContrasena.value = "";
        inputConfirmarContrasena.value = "";

        mostrarModal(modalAdministrador);

        inputNombres.focus();
    } catch (error) {
        console.error(error);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible consultar el administrador.",
            "error"
        );
    }
}

async function guardarAdministrador(event) {
    event.preventDefault();

    limpiarMensajeFormulario();

    const idAdministrador =
        inputIdAdministrador.value.trim();

    const datos = {
        nombres:
            inputNombres.value.trim(),

        apellidos:
            inputApellidos.value.trim(),

        correo:
            inputCorreo.value
                .trim()
                .toLowerCase(),

        contrasena:
            inputContrasena.value,

        confirmarContrasena:
            inputConfirmarContrasena.value
    };

    const errorValidacion =
        validarAdministrador(
            datos,
            Boolean(idAdministrador)
        );

    if (errorValidacion) {
        mostrarMensajeFormulario(
            errorValidacion
        );

        return;
    }

    const correoDuplicado =
        administradores.some(
            administrador => {
                const mismoCorreo =
                    administrador.correo
                        .trim()
                        .toLowerCase() ===
                    datos.correo;

                const mismoAdministrador =
                    Number(
                        administrador.idAdministrador
                    ) ===
                    Number(idAdministrador);

                return (
                    mismoCorreo &&
                    !mismoAdministrador
                );
            }
        );

    if (correoDuplicado) {
        mostrarMensajeFormulario(
            "Ya existe un administrador con ese correo."
        );

        inputCorreo.focus();
        return;
    }

    bloquearFormularioAdministrador(
        true
    );

    try {
        if (idAdministrador) {
            await actualizarAdministrador(
                Number(idAdministrador),
                datos
            );

            actualizarSesionSiEsActual(
                Number(idAdministrador),
                datos
            );

            cerrarModalAdministrador();

            mostrarMensajePrincipal(
                "Administrador actualizado correctamente.",
                "exito"
            );
        } else {
            await crearAdministrador(
                datos
            );

            cerrarModalAdministrador();

            mostrarMensajePrincipal(
                "Administrador creado correctamente.",
                "exito"
            );
        }

        await cargarAdministradores(false);
    } catch (error) {
        console.error(error);

        mostrarMensajeFormulario(
            error.message ||
            "No fue posible guardar el administrador."
        );
    } finally {
        bloquearFormularioAdministrador(
            false
        );
    }
}

function validarAdministrador(
    datos,
    editando
) {
    if (!datos.nombres) {
        return "Debe ingresar los nombres.";
    }

    if (datos.nombres.length < 2) {
        return "Los nombres deben contener al menos 2 caracteres.";
    }

    if (!datos.apellidos) {
        return "Debe ingresar los apellidos.";
    }

    if (datos.apellidos.length < 2) {
        return "Los apellidos deben contener al menos 2 caracteres.";
    }

    if (!datos.correo) {
        return "Debe ingresar el correo electrónico.";
    }

    const correoValido =
        /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!correoValido.test(datos.correo)) {
        return "Ingrese un correo electrónico válido.";
    }

    if (!editando) {
        if (!datos.contrasena) {
            return "Debe ingresar una contraseña.";
        }

        if (datos.contrasena.length < 8) {
            return "La contraseña debe contener al menos 8 caracteres.";
        }

        if (
            datos.contrasena !==
            datos.confirmarContrasena
        ) {
            return "Las contraseñas no coinciden.";
        }
    }

    return null;
}

function cerrarModalAdministrador() {
    ocultarModal(modalAdministrador);

    formAdministrador.reset();

    inputIdAdministrador.value = "";

    limpiarMensajeFormulario();
}

function bloquearFormularioAdministrador(
    bloqueado
) {
    const campos =
        formAdministrador.querySelectorAll(
            "input, button"
        );

    campos.forEach(campo => {
        campo.disabled = bloqueado;
    });

    botonCerrarModalAdministrador.disabled =
        bloqueado;

    botonGuardarAdministrador.textContent =
        bloqueado
            ? "Guardando..."
            : inputIdAdministrador.value
                ? "Guardar cambios"
                : "Guardar";
}


async function procesarCambioEstado(
    idAdministrador,
    boton
) {
    const administrador =
        administradores.find(
            item =>
                Number(
                    item.idAdministrador
                ) ===
                Number(idAdministrador)
        );

    if (!administrador) {
        return;
    }

    const idAdministradorActual =
        obtenerIdAdministradorActual();

    if (
        Number(idAdministrador) ===
        idAdministradorActual
    ) {
        mostrarMensajePrincipal(
            "No puede desactivar la cuenta que está utilizando.",
            "error"
        );

        return;
    }

    const nuevoEstado =
        !administrador.activo;

    if (!nuevoEstado) {
        const administradoresActivos =
            administradores.filter(
                item => item.activo
            );

        if (
            administradoresActivos.length <=
            1
        ) {
            mostrarMensajePrincipal(
                "Debe existir al menos un administrador activo.",
                "error"
            );

            return;
        }
    }

    const accion =
        nuevoEstado
            ? "activar"
            : "desactivar";

    const confirmado =
        window.confirm(
            `¿Está seguro de ${accion} a ${administrador.nombreCompleto}?`
        );

    if (!confirmado) {
        return;
    }

    boton.disabled = true;

    boton.textContent =
        nuevoEstado
            ? "Activando..."
            : "Desactivando...";

    try {
        await cambiarEstadoAdministrador(
            administrador.idAdministrador,
            nuevoEstado
        );

        mostrarMensajePrincipal(
            nuevoEstado
                ? "Administrador activado correctamente."
                : "Administrador desactivado correctamente.",
            "exito"
        );

        await cargarAdministradores(false);
    } catch (error) {
        console.error(error);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cambiar el estado.",
            "error"
        );

        boton.disabled = false;

        boton.textContent =
            nuevoEstado
                ? "Activar"
                : "Desactivar";
    }
}

function abrirModalContrasena() {
    formContrasena.reset();

    limpiarMensajeContrasena();

    mostrarModal(modalContrasena);

    inputContrasenaActual.focus();
}

function cerrarModalContrasena() {
    ocultarModal(modalContrasena);

    formContrasena.reset();

    limpiarMensajeContrasena();
}

async function guardarCambioContrasena(
    event
) {
    event.preventDefault();

    limpiarMensajeContrasena();

    const contrasenaActual =
        inputContrasenaActual.value;

    const nuevaContrasena =
        inputNuevaContrasena.value;

    const confirmarNuevaContrasena =
        inputConfirmarNuevaContrasena.value;

    if (!contrasenaActual) {
        mostrarMensajeContrasena(
            "Debe ingresar su contraseña actual."
        );

        inputContrasenaActual.focus();
        return;
    }

    if (!nuevaContrasena) {
        mostrarMensajeContrasena(
            "Debe ingresar la nueva contraseña."
        );

        inputNuevaContrasena.focus();
        return;
    }

    if (nuevaContrasena.length < 8) {
        mostrarMensajeContrasena(
            "La nueva contraseña debe contener al menos 8 caracteres."
        );

        inputNuevaContrasena.focus();
        return;
    }

    if (
        nuevaContrasena ===
        contrasenaActual
    ) {
        mostrarMensajeContrasena(
            "La nueva contraseña debe ser diferente de la contraseña actual."
        );

        inputNuevaContrasena.focus();
        return;
    }

    if (
        nuevaContrasena !==
        confirmarNuevaContrasena
    ) {
        mostrarMensajeContrasena(
            "Las nuevas contraseñas no coinciden."
        );

        inputConfirmarNuevaContrasena.focus();
        return;
    }

    bloquearFormularioContrasena(true);

    try {
        await cambiarMiContrasena({
            contrasenaActual,
            nuevaContrasena
        });

        cerrarModalContrasena();

        mostrarMensajePrincipal(
            "Contraseña actualizada correctamente.",
            "exito"
        );
    } catch (error) {
        console.error(error);

        mostrarMensajeContrasena(
            error.message ||
            "No fue posible cambiar la contraseña."
        );
    } finally {
        bloquearFormularioContrasena(
            false
        );
    }
}

function bloquearFormularioContrasena(
    bloqueado
) {
    inputContrasenaActual.disabled =
        bloqueado;

    inputNuevaContrasena.disabled =
        bloqueado;

    inputConfirmarNuevaContrasena.disabled =
        bloqueado;

    botonCancelarContrasena.disabled =
        bloqueado;

    botonCerrarModalContrasena.disabled =
        bloqueado;

    botonGuardarContrasena.disabled =
        bloqueado;

    botonGuardarContrasena.textContent =
        bloqueado
            ? "Guardando..."
            : "Cambiar contraseña";
}

function obtenerIdAdministradorActual() {
    const administrador =
        obtenerAdministrador();

    const id =
        Number(
            administrador?.idAdministrador ??
            administrador?.IdAdministrador ??
            administrador?.id ??
            administrador?.Id
        );

    return Number.isInteger(id)
        ? id
        : 0;
}

function actualizarSesionSiEsActual(
    idAdministrador,
    datos
) {
    const idActual =
        obtenerIdAdministradorActual();

    if (
        Number(idAdministrador) !==
        idActual
    ) {
        return;
    }

    const administradorActual =
        obtenerAdministrador() ?? {};

    const nombreCompleto =
        `${datos.nombres} ${datos.apellidos}`
            .trim();

    const administradorActualizado = {
        ...administradorActual,

        idAdministrador,

        nombreCompleto,

        correo:
            datos.correo,

        rol:
            administradorActual.rol ??
            "Administrador"
    };

    sessionStorage.setItem(
        ADMIN_KEY,
        JSON.stringify(
            administradorActualizado
        )
    );

    const elementoNombre =
        document.getElementById(
            "topbarNombre"
        );

    if (elementoNombre) {
        elementoNombre.textContent =
            nombreCompleto;
    }
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
        !modalAdministrador.classList.contains(
            "modal--oculto"
        )
    ) {
        cerrarModalAdministrador();
    }

    if (
        !modalContrasena.classList.contains(
            "modal--oculto"
        )
    ) {
        cerrarModalContrasena();
    }
}



function actualizarContador() {
    const activos =
        administradores.filter(
            administrador =>
                administrador.activo
        ).length;

    contadorAdministradores.textContent =
        `${administradores.length} registrados · ${activos} activos`;
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

/* =========================
   MENSAJES
   ========================= */

function mostrarMensajePrincipal(
    mensaje,
    tipo
) {
    clearTimeout(temporizadorMensaje);

    mensajeAdministradores.textContent =
        mensaje;

    mensajeAdministradores.className =
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

    mensajeAdministradores.textContent =
        "";

    mensajeAdministradores.className =
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
    mensajeFormulario.textContent = "";

    mensajeFormulario.className =
        "mensaje mensaje--oculto mensaje--modal";
}

function mostrarMensajeContrasena(
    mensaje
) {
    mensajeContrasena.textContent =
        mensaje;

    mensajeContrasena.className =
        "mensaje mensaje--error mensaje--modal";
}

function limpiarMensajeContrasena() {
    mensajeContrasena.textContent = "";

    mensajeContrasena.className =
        "mensaje mensaje--oculto mensaje--modal";
}



function escaparHtml(texto) {
    const elemento =
        document.createElement("div");

    elemento.textContent =
        texto ?? "";

    return elemento.innerHTML;
}