import {
    apiGet,
    apiPost,
    apiPut,
    apiPatch
} from "../../shared/js/api.js";

const RUTA_CARGOS = "/cargos";

export async function obtenerCargos() {
    const respuesta =
        await apiGet(RUTA_CARGOS);

    return normalizarListaCargos(
        respuesta
    );
}

export async function crearCargo(datos) {
    return apiPost(
        RUTA_CARGOS,
        {
            nombre: datos.nombre,
            descripcion: datos.descripcion
        }
    );
}

export async function actualizarCargo(
    idCargo,
    datos
) {
    return apiPut(
        `${RUTA_CARGOS}/${idCargo}`,
        {
            nombre: datos.nombre,
            descripcion: datos.descripcion
        }
    );
}

export async function cambiarEstadoCargo(
    idCargo,
    activo
) {
    return apiPatch(
        `${RUTA_CARGOS}/${idCargo}/estado`,
        {
            activo
        }
    );
}

function normalizarListaCargos(respuesta) {
    if (!respuesta) {
        return [];
    }

    let registros = [];

    if (Array.isArray(respuesta)) {
        registros = respuesta;
    } else {
        registros =
            respuesta.items ??
            respuesta.Items ??
            respuesta.datos ??
            respuesta.Datos ??
            respuesta.registros ??
            respuesta.Registros ??
            respuesta.resultados ??
            respuesta.Resultados ??
            [];
    }

    if (!Array.isArray(registros)) {
        return [];
    }

    return registros.map(
        normalizarCargo
    );
}

function normalizarCargo(cargo) {
    const activoOriginal =
        cargo.activo ??
        cargo.Activo ??
        cargo.estado ??
        cargo.Estado ??
        true;

    return {
        idCargo:
            cargo.idCargo ??
            cargo.IdCargo ??
            cargo.id ??
            cargo.Id,

        nombre:
            cargo.nombre ??
            cargo.Nombre ??
            "",

        descripcion:
            cargo.descripcion ??
            cargo.Descripcion ??
            "",

        activo:
            convertirActivo(
                activoOriginal
            )
    };
}

function convertirActivo(valor) {
    if (typeof valor === "boolean") {
        return valor;
    }

    if (typeof valor === "number") {
        return valor === 1;
    }

    if (typeof valor === "string") {
        const valorNormalizado =
            valor.trim().toLowerCase();

        return (
            valorNormalizado === "activo" ||
            valorNormalizado === "true" ||
            valorNormalizado === "1"
        );
    }

    return true;
}