import {
    protegerPagina
} from "../../shared/js/auth.js";

import {
    inicializarLayout
} from "../../shared/js/layout.js";

import {
    obtenerCargos,
    crearCargo,
    actualizarCargo,
    cambiarEstadoCargo
} from "./cargos.service.js";

const botonNuevoCargo =
    document.getElementById(
        "btnNuevoCargo"
    );

const botonActualizar =
    document.getElementById(
        "btnActualizar"
    );

const inputBuscar =
    document.getElementById(
        "buscarCargo"
    );

const tablaCargosBody =
    document.getElementById(
        "tablaCargosBody"
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

const mensajeCargos =
    document.getElementById(
        "mensajeCargos"
    );

const modalCargo =
    document.getElementById(
        "modalCargo"
    );

const modalFondo =
    document.getElementById(
        "modalFondo"
    );

const tituloModalCargo =
    document.getElementById(
        "tituloModalCargo"
    );

const botonCerrarModal =
    document.getElementById(
        "btnCerrarModal"
    );

const botonCancelar =
    document.getElementById(
        "btnCancelar"
    );

const formCargo =
    document.getElementById(
        "formCargo"
    );

const inputIdCargo =
    document.getElementById(
        "idCargo"
    );

const inputNombreCargo =
    document.getElementById(
        "nombreCargo"
    );

const inputDescripcionCargo =
    document.getElementById(
        "descripcionCargo"
    );

const botonGuardarCargo =
    document.getElementById(
        "btnGuardarCargo"
    );

const mensajeFormulario =
    document.getElementById(
        "mensajeFormulario"
    );

let cargos = [];
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
        titulo: "Cargos",
        paginaActiva: "cargos"
    });

    configurarEventos();

    await cargarCargos();
}

function configurarEventos() {
    botonNuevoCargo.addEventListener(
        "click",
        abrirModalNuevo
    );

    botonActualizar.addEventListener(
        "click",
        cargarCargos
    );

    inputBuscar.addEventListener(
        "input",
        filtrarCargos
    );

    formCargo.addEventListener(
        "submit",
        guardarCargo
    );

    botonCerrarModal.addEventListener(
        "click",
        cerrarModal
    );

    botonCancelar.addEventListener(
        "click",
        cerrarModal
    );

    modalFondo.addEventListener(
        "click",
        cerrarModal
    );

    document.addEventListener(
        "keydown",
        event => {
            if (
                event.key === "Escape" &&
                !modalCargo.classList.contains(
                    "modal--oculto"
                )
            ) {
                cerrarModal();
            }
        }
    );
}

async function cargarCargos() {
    mostrarCargando(true);
    ocultarMensajePrincipal();

    botonActualizar.disabled = true;
    botonActualizar.textContent =
        "Actualizando...";

    try {
        cargos = await obtenerCargos();

        renderizarCargos(cargos);
    } catch (error) {
        console.error(error);

        cargos = [];

        renderizarCargos([]);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cargar los cargos.",
            "error"
        );
    } finally {
        mostrarCargando(false);

        botonActualizar.disabled = false;
        botonActualizar.textContent =
            "Actualizar";
    }
}

function renderizarCargos(listaCargos) {
    tablaCargosBody.innerHTML = "";

    if (listaCargos.length === 0) {
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

    listaCargos.forEach(cargo => {
        const fila =
            document.createElement("tr");

        const descripcion =
            cargo.descripcion ||
            "Sin descripción";

        const claseEstado =
            cargo.activo
                ? "estado estado--activo"
                : "estado estado--inactivo";

        const textoEstado =
            cargo.activo
                ? "Activo"
                : "Inactivo";

        const claseBotonEstado =
            cargo.activo
                ? "boton-tabla boton-tabla--desactivar"
                : "boton-tabla boton-tabla--activar";

        const textoBotonEstado =
            cargo.activo
                ? "Desactivar"
                : "Activar";

        fila.innerHTML = `
            <td>
                <strong>
                    ${escaparHtml(cargo.nombre)}
                </strong>
            </td>

            <td>
                ${escaparHtml(descripcion)}
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
                        data-id="${cargo.idCargo}"
                    >
                        Editar
                    </button>

                    <button
                        type="button"
                        class="${claseBotonEstado}"
                        data-accion="estado"
                        data-id="${cargo.idCargo}"
                    >
                        ${textoBotonEstado}
                    </button>
                </div>
            </td>
        `;

        tablaCargosBody.appendChild(fila);
    });

    configurarBotonesTabla();
}

function configurarBotonesTabla() {
    const botonesEditar =
        document.querySelectorAll(
            '[data-accion="editar"]'
        );

    botonesEditar.forEach(boton => {
        boton.addEventListener(
            "click",
            () => {
                const idCargo =
                    Number(boton.dataset.id);

                abrirModalEditar(idCargo);
            }
        );
    });

    const botonesEstado =
        document.querySelectorAll(
            '[data-accion="estado"]'
        );

    botonesEstado.forEach(boton => {
        boton.addEventListener(
            "click",
            async () => {
                const idCargo =
                    Number(boton.dataset.id);

                await procesarCambioEstado(
                    idCargo,
                    boton
                );
            }
        );
    });
}

function abrirModalNuevo() {
    formCargo.reset();

    inputIdCargo.value = "";

    tituloModalCargo.textContent =
        "Nuevo cargo";

    botonGuardarCargo.textContent =
        "Guardar";

    limpiarMensajeFormulario();

    mostrarModal();

    inputNombreCargo.focus();
}

function abrirModalEditar(idCargo) {
    const cargo =
        cargos.find(
            item =>
                Number(item.idCargo) ===
                Number(idCargo)
        );

    if (!cargo) {
        mostrarMensajePrincipal(
            "No se encontró el cargo seleccionado.",
            "error"
        );

        return;
    }

    formCargo.reset();

    inputIdCargo.value =
        cargo.idCargo;

    inputNombreCargo.value =
        cargo.nombre;

    inputDescripcionCargo.value =
        cargo.descripcion;

    tituloModalCargo.textContent =
        "Editar cargo";

    botonGuardarCargo.textContent =
        "Guardar cambios";

    limpiarMensajeFormulario();

    mostrarModal();

    inputNombreCargo.focus();
}

async function guardarCargo(event) {
    event.preventDefault();

    limpiarMensajeFormulario();

    const idCargo =
        inputIdCargo.value.trim();

    const nombre =
        inputNombreCargo.value.trim();

    const descripcion =
        inputDescripcionCargo.value.trim();

    if (!nombre) {
        mostrarMensajeFormulario(
            "Debe ingresar el nombre del cargo."
        );

        inputNombreCargo.focus();
        return;
    }

    if (nombre.length < 2) {
        mostrarMensajeFormulario(
            "El nombre debe contener al menos 2 caracteres."
        );

        inputNombreCargo.focus();
        return;
    }

    const nombreDuplicado =
        cargos.some(cargo => {
            const mismoNombre =
                cargo.nombre
                    .trim()
                    .toLowerCase() ===
                nombre.toLowerCase();

            const mismoCargo =
                Number(cargo.idCargo) ===
                Number(idCargo);

            return (
                mismoNombre &&
                !mismoCargo
            );
        });

    if (nombreDuplicado) {
        mostrarMensajeFormulario(
            "Ya existe un cargo con ese nombre."
        );

        inputNombreCargo.focus();
        return;
    }

    bloquearFormulario(true);

    try {
        const datos = {
            nombre,
            descripcion
        };

        if (idCargo) {
            await actualizarCargo(
                Number(idCargo),
                datos
            );

            cerrarModal();

            mostrarMensajePrincipal(
                "Cargo actualizado correctamente.",
                "exito"
            );
        } else {
            await crearCargo(datos);

            cerrarModal();

            mostrarMensajePrincipal(
                "Cargo creado correctamente.",
                "exito"
            );
        }

        await cargarCargos();
    } catch (error) {
        console.error(error);

        mostrarMensajeFormulario(
            error.message ||
            "No fue posible guardar el cargo."
        );
    } finally {
        bloquearFormulario(false);
    }
}

async function procesarCambioEstado(
    idCargo,
    boton
) {
    const cargo =
        cargos.find(
            item =>
                Number(item.idCargo) ===
                Number(idCargo)
        );

    if (!cargo) {
        return;
    }

    const nuevoEstado =
        !cargo.activo;

    const accion =
        nuevoEstado
            ? "activar"
            : "desactivar";

    const confirmado =
        window.confirm(
            `¿Está seguro de ${accion} el cargo "${cargo.nombre}"?`
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
        await cambiarEstadoCargo(
            cargo.idCargo,
            nuevoEstado
        );

        mostrarMensajePrincipal(
            nuevoEstado
                ? "Cargo activado correctamente."
                : "Cargo desactivado correctamente.",
            "exito"
        );

        await cargarCargos();
    } catch (error) {
        console.error(error);

        mostrarMensajePrincipal(
            error.message ||
            "No fue posible cambiar el estado del cargo.",
            "error"
        );

        boton.disabled = false;
        boton.textContent =
            nuevoEstado
                ? "Activar"
                : "Desactivar";
    }
}

function filtrarCargos() {
    const texto =
        inputBuscar.value
            .trim()
            .toLowerCase();

    if (!texto) {
        renderizarCargos(cargos);
        return;
    }

    const resultados =
        cargos.filter(cargo => {
            const nombre =
                cargo.nombre
                    .toLowerCase();

            const descripcion =
                cargo.descripcion
                    .toLowerCase();

            return (
                nombre.includes(texto) ||
                descripcion.includes(texto)
            );
        });

    renderizarCargos(resultados);
}

function mostrarModal() {
    modalCargo.classList.remove(
        "modal--oculto"
    );

    modalCargo.setAttribute(
        "aria-hidden",
        "false"
    );

    document.body.style.overflow =
        "hidden";
}

function cerrarModal() {
    modalCargo.classList.add(
        "modal--oculto"
    );

    modalCargo.setAttribute(
        "aria-hidden",
        "true"
    );

    document.body.style.overflow =
        "";

    formCargo.reset();
    inputIdCargo.value = "";

    limpiarMensajeFormulario();
}

function bloquearFormulario(bloqueado) {
    inputNombreCargo.disabled =
        bloqueado;

    inputDescripcionCargo.disabled =
        bloqueado;

    botonCancelar.disabled =
        bloqueado;

    botonCerrarModal.disabled =
        bloqueado;

    botonGuardarCargo.disabled =
        bloqueado;

    botonGuardarCargo.textContent =
        bloqueado
            ? "Guardando..."
            : inputIdCargo.value
                ? "Guardar cambios"
                : "Guardar";
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

    mensajeCargos.textContent =
        mensaje;

    mensajeCargos.className =
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

    mensajeCargos.textContent = "";

    mensajeCargos.className =
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

function escaparHtml(texto) {
    const elemento =
        document.createElement("div");

    elemento.textContent =
        texto ?? "";

    return elemento.innerHTML;
}