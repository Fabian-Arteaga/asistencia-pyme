import {
    protegerPagina
} from "../../shared/js/auth.js";

import {
    inicializarLayout
} from "../../shared/js/layout.js";

import {
    obtenerTiposDeduccion,
    obtenerTipoDeduccionPorId,
    crearTipoDeduccion,
    actualizarTipoDeduccion,
    cambiarEstadoTipoDeduccion
} from "../deducciones/deducciones.services.js";


const botonNuevaDeduccion =
    document.getElementById(
        "btnNuevaDeduccion"
    );

const botonActualizar =
    document.getElementById(
        "btnActualizar"
    );

const inputBuscar =
    document.getElementById(
        "buscarDeduccion"
    );

const filtroEstado =
    document.getElementById(
        "filtroEstado"
    );

const contadorDeducciones =
    document.getElementById(
        "contadorDeducciones"
    );

const tablaDeduccionesBody =
    document.getElementById(
        "tablaDeduccionesBody"
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

const mensajeDeducciones =
    document.getElementById(
        "mensajeDeducciones"
    );

/* =========================
   MODAL
   ========================= */

const modalDeduccion =
    document.getElementById(
        "modalDeduccion"
    );

const fondoModalDeduccion =
    document.getElementById(
        "fondoModalDeduccion"
    );

const formDeduccion =
    document.getElementById(
        "formDeduccion"
    );

const inputIdTipoDeduccion =
    document.getElementById(
        "idTipoDeduccion"
    );

const inputNombre =
    document.getElementById(
        "nombreDeduccion"
    );

const inputPorcentaje =
    document.getElementById(
        "porcentajeDeduccion"
    );

const inputDescripcion =
    document.getElementById(
        "descripcionDeduccion"
    );

const tituloModalDeduccion =
    document.getElementById(
        "tituloModalDeduccion"
    );

const botonCerrarModalDeduccion =
    document.getElementById(
        "btnCerrarModalDeduccion"
    );

const botonCancelarDeduccion =
    document.getElementById(
        "btnCancelarDeduccion"
    );

const botonGuardarDeduccion =
    document.getElementById(
        "btnGuardarDeduccion"
    );

const mensajeFormulario =
    document.getElementById(
        "mensajeFormulario"
    );

/* =========================
   VARIABLES
   ========================= */

let tiposDeduccion = [];
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
        titulo: "Tipos de deducción",
        paginaActiva: "deducciones"
    });

    configurarEventos();

    await cargarTiposDeduccion();
}

function configurarEventos() {
    botonNuevaDeduccion.addEventListener(
        "click",
        abrirModalNuevo
    );

    botonActualizar.addEventListener(
        "click",
        cargarTiposDeduccion
    );

    inputBuscar.addEventListener(
        "input",
        aplicarFiltros
    );

    filtroEstado.addEventListener(
        "change",
        aplicarFiltros
    );

    formDeduccion.addEventListener(
        "submit",
        guardarTipoDeduccion
    );

    botonCerrarModalDeduccion.addEventListener(
        "click",
        cerrarModal
    );

    botonCancelarDeduccion.addEventListener(
        "click",
        cerrarModal
    );

    fondoModalDeduccion.addEventListener(
        "click",
        cerrarModal
    );

    document.addEventListener(
        "keydown",
        manejarTeclaEscape
    );
}

/* =========================
   CARGA
   ========================= */

async function cargarTiposDeduccion(
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
        tiposDeduccion =
            await obtenerTiposDeduccion();

        ordenarTiposDeduccion();
        actualizarContador();
        aplicarFiltros();
    } catch (error) {
        console.error(error);

        tiposDeduccion = [];

        renderizarTiposDeduccion([]);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cargar los tipos de deducción.",
            "error"
        );
    } finally {
        mostrarCargando(false);

        botonActualizar.disabled = false;
        botonActualizar.textContent =
            "Actualizar";
    }
}

function ordenarTiposDeduccion() {
    tiposDeduccion.sort(
        (a, b) =>
            a.nombre.localeCompare(
                b.nombre,
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
        tiposDeduccion.filter(tipo => {
            const coincideTexto =
                !texto ||
                tipo.nombre
                    .toLowerCase()
                    .includes(texto) ||
                tipo.descripcion
                    .toLowerCase()
                    .includes(texto);

            const coincideEstado =
                estado === "todos" ||
                (
                    estado === "activo" &&
                    tipo.activo
                ) ||
                (
                    estado === "inactivo" &&
                    !tipo.activo
                );

            return (
                coincideTexto &&
                coincideEstado
            );
        });

    renderizarTiposDeduccion(
        resultados
    );
}

/* =========================
   TABLA
   ========================= */

function renderizarTiposDeduccion(lista) {
    tablaDeduccionesBody.innerHTML =
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

    lista.forEach(tipo => {
        const fila =
            document.createElement("tr");

        const claseEstado =
            tipo.activo
                ? "estado estado--activo"
                : "estado estado--inactivo";

        const textoEstado =
            tipo.activo
                ? "Activo"
                : "Inactivo";

        const claseBotonEstado =
            tipo.activo
                ? "boton-tabla boton-tabla--desactivar"
                : "boton-tabla boton-tabla--activar";

        const textoBotonEstado =
            tipo.activo
                ? "Desactivar"
                : "Activar";

        const descripcion =
            tipo.descripcion ||
            "Sin descripción";

        fila.innerHTML = `
            <td>
                <span class="deduccion-nombre">
                    ${escaparHtml(tipo.nombre)}
                </span>
            </td>

            <td>
                <span
                    class="deduccion-descripcion"
                    title="${escaparAtributo(descripcion)}"
                >
                    ${escaparHtml(descripcion)}
                </span>
            </td>

            <td>
                <span class="porcentaje">
                    ${formatearPorcentaje(
                        tipo.porcentaje
                    )}
                </span>
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
                        data-id="${tipo.idTipoDeduccion}"
                    >
                        Editar
                    </button>

                    <button
                        type="button"
                        class="${claseBotonEstado}"
                        data-accion="estado"
                        data-id="${tipo.idTipoDeduccion}"
                    >
                        ${textoBotonEstado}
                    </button>
                </div>
            </td>
        `;

        tablaDeduccionesBody.appendChild(
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
    formDeduccion.reset();

    inputIdTipoDeduccion.value = "";

    tituloModalDeduccion.textContent =
        "Nueva deducción";

    botonGuardarDeduccion.textContent =
        "Guardar";

    limpiarMensajeFormulario();

    mostrarModal();

    inputNombre.focus();
}

async function abrirModalEditar(
    idTipoDeduccion
) {
    limpiarMensajeFormulario();

    try {
        let tipo =
            tiposDeduccion.find(
                item =>
                    Number(
                        item.idTipoDeduccion
                    ) ===
                    Number(idTipoDeduccion)
            );

        if (!tipo) {
            tipo =
                await obtenerTipoDeduccionPorId(
                    idTipoDeduccion
                );
        }

        if (!tipo) {
            throw new Error(
                "No se encontró el tipo de deducción."
            );
        }

        formDeduccion.reset();

        inputIdTipoDeduccion.value =
            tipo.idTipoDeduccion;

        inputNombre.value =
            tipo.nombre;

        inputPorcentaje.value =
            tipo.porcentaje;

        inputDescripcion.value =
            tipo.descripcion;

        tituloModalDeduccion.textContent =
            "Editar deducción";

        botonGuardarDeduccion.textContent =
            "Guardar cambios";

        mostrarModal();

        inputNombre.focus();
    } catch (error) {
        console.error(error);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible consultar la deducción.",
            "error"
        );
    }
}

async function guardarTipoDeduccion(event) {
    event.preventDefault();

    limpiarMensajeFormulario();

    const idTipoDeduccion =
        inputIdTipoDeduccion.value.trim();

    const datos = {
        nombre:
            inputNombre.value.trim(),

        porcentaje:
            Number(inputPorcentaje.value),

        descripcion:
            inputDescripcion.value.trim() ||
            null
    };

    const errorValidacion =
        validarDatos(datos);

    if (errorValidacion) {
        mostrarMensajeFormulario(
            errorValidacion
        );

        return;
    }

    const nombreDuplicado =
        tiposDeduccion.some(tipo => {
            const mismoNombre =
                tipo.nombre
                    .trim()
                    .toLowerCase() ===
                datos.nombre.toLowerCase();

            const mismoRegistro =
                Number(
                    tipo.idTipoDeduccion
                ) ===
                Number(idTipoDeduccion);

            return (
                mismoNombre &&
                !mismoRegistro
            );
        });

    if (nombreDuplicado) {
        mostrarMensajeFormulario(
            "Ya existe un tipo de deducción con ese nombre."
        );

        inputNombre.focus();
        return;
    }

    bloquearFormulario(true);

    try {
        if (idTipoDeduccion) {
            await actualizarTipoDeduccion(
                Number(idTipoDeduccion),
                datos
            );

            cerrarModal();

            mostrarMensajePrincipal(
                "Tipo de deducción actualizado correctamente.",
                "exito"
            );
        } else {
            await crearTipoDeduccion(
                datos
            );

            cerrarModal();

            mostrarMensajePrincipal(
                "Tipo de deducción creado correctamente.",
                "exito"
            );
        }

        await cargarTiposDeduccion(false);
    } catch (error) {
        console.error(error);

        mostrarMensajeFormulario(
            error.message ||
            "No fue posible guardar el tipo de deducción."
        );
    } finally {
        bloquearFormulario(false);
    }
}

function validarDatos(datos) {
    if (!datos.nombre) {
        return "Debe ingresar el nombre de la deducción.";
    }

    if (datos.nombre.length < 2) {
        return "El nombre debe contener al menos 2 caracteres.";
    }

    if (
        !Number.isFinite(
            datos.porcentaje
        )
    ) {
        return "Debe ingresar un porcentaje válido.";
    }

    if (
        datos.porcentaje < 0 ||
        datos.porcentaje > 100
    ) {
        return "El porcentaje debe estar entre 0 y 100.";
    }

    return null;
}

/* =========================
   CAMBIAR ESTADO
   ========================= */

async function procesarCambioEstado(
    idTipoDeduccion,
    boton
) {
    const tipo =
        tiposDeduccion.find(
            item =>
                Number(
                    item.idTipoDeduccion
                ) ===
                Number(idTipoDeduccion)
        );

    if (!tipo) {
        return;
    }

    const nuevoEstado =
        !tipo.activo;

    const accion =
        nuevoEstado
            ? "activar"
            : "desactivar";

    const confirmado =
        window.confirm(
            `¿Está seguro de ${accion} la deducción "${tipo.nombre}"?`
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
        await cambiarEstadoTipoDeduccion(
            tipo.idTipoDeduccion,
            nuevoEstado
        );

        mostrarMensajePrincipal(
            nuevoEstado
                ? "Tipo de deducción activado correctamente."
                : "Tipo de deducción desactivado correctamente.",
            "exito"
        );

        await cargarTiposDeduccion(false);
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

/* =========================
   MODAL
   ========================= */

function mostrarModal() {
    modalDeduccion.classList.remove(
        "modal--oculto"
    );

    modalDeduccion.setAttribute(
        "aria-hidden",
        "false"
    );

    document.body.style.overflow =
        "hidden";
}

function cerrarModal() {
    modalDeduccion.classList.add(
        "modal--oculto"
    );

    modalDeduccion.setAttribute(
        "aria-hidden",
        "true"
    );

    document.body.style.overflow =
        "";

    formDeduccion.reset();

    inputIdTipoDeduccion.value = "";

    limpiarMensajeFormulario();
}

function manejarTeclaEscape(event) {
    if (
        event.key === "Escape" &&
        !modalDeduccion.classList.contains(
            "modal--oculto"
        )
    ) {
        cerrarModal();
    }
}

function bloquearFormulario(
    bloqueado
) {
    inputNombre.disabled =
        bloqueado;

    inputPorcentaje.disabled =
        bloqueado;

    inputDescripcion.disabled =
        bloqueado;

    botonCerrarModalDeduccion.disabled =
        bloqueado;

    botonCancelarDeduccion.disabled =
        bloqueado;

    botonGuardarDeduccion.disabled =
        bloqueado;

    botonGuardarDeduccion.textContent =
        bloqueado
            ? "Guardando..."
            : inputIdTipoDeduccion.value
                ? "Guardar cambios"
                : "Guardar";
}

/* =========================
   CONTADOR Y CARGA
   ========================= */

function actualizarContador() {
    const activos =
        tiposDeduccion.filter(
            tipo => tipo.activo
        ).length;

    contadorDeducciones.textContent =
        `${tiposDeduccion.length} registrados · ${activos} activos`;
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

    mensajeDeducciones.textContent =
        mensaje;

    mensajeDeducciones.className =
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

    mensajeDeducciones.textContent =
        "";

    mensajeDeducciones.className =
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
   FORMATO Y SEGURIDAD
   ========================= */

function formatearPorcentaje(valor) {
    const numero =
        Number(valor);

    if (!Number.isFinite(numero)) {
        return "0.00 %";
    }

    return `${numero.toFixed(2)} %`;
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