import {
    apiGet,
    apiPost,
    apiPut,
    apiPatch
} from "../../shared/js/api.js";

const RUTA_TIPOS_DEDUCCION =
    "/tipos-deduccion";

export async function obtenerTiposDeduccion() {
    const respuesta =
        await apiGet(
            RUTA_TIPOS_DEDUCCION
        );

    return normalizarLista(
        respuesta
    );
}

export async function obtenerTipoDeduccionPorId(
    idTipoDeduccion
) {
    const respuesta =
        await apiGet(
            `${RUTA_TIPOS_DEDUCCION}/${idTipoDeduccion}`
        );

    return normalizarTipoDeduccion(
        respuesta
    );
}

export async function crearTipoDeduccion(
    datos
) {
    return apiPost(
        RUTA_TIPOS_DEDUCCION,
        {
            nombre:
                datos.nombre,

            descripcion:
                datos.descripcion,

            porcentaje:
                datos.porcentaje
        }
    );
}

export async function actualizarTipoDeduccion(
    idTipoDeduccion,
    datos
) {
    return apiPut(
        `${RUTA_TIPOS_DEDUCCION}/${idTipoDeduccion}`,
        {
            nombre:
                datos.nombre,

            descripcion:
                datos.descripcion,

            porcentaje:
                datos.porcentaje
        }
    );
}

export async function cambiarEstadoTipoDeduccion(
    idTipoDeduccion,
    activo
) {
    return apiPatch(
        `${RUTA_TIPOS_DEDUCCION}/${idTipoDeduccion}/estado`,
        {
            activo
        }
    );
}

function normalizarLista(respuesta) {
    if (!respuesta) {
        return [];
    }

    let registros;

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

    return registros
        .map(normalizarTipoDeduccion)
        .filter(Boolean);
}

function normalizarTipoDeduccion(tipo) {
    if (!tipo) {
        return null;
    }

    return {
        idTipoDeduccion:
            tipo.idTipoDeduccion ??
            tipo.IdTipoDeduccion ??
            tipo.id ??
            tipo.Id,

        nombre:
            tipo.nombre ??
            tipo.Nombre ??
            "",

        descripcion:
            tipo.descripcion ??
            tipo.Descripcion ??
            "",

        porcentaje:
            Number(
                tipo.porcentaje ??
                tipo.Porcentaje ??
                tipo.valorPorcentaje ??
                tipo.ValorPorcentaje ??
                0
            ),

        activo:
            convertirActivo(
                tipo.activo ??
                tipo.Activo ??
                tipo.estado ??
                tipo.Estado
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
        const estado =
            valor.trim().toLowerCase();

        return (
            estado === "activo" ||
            estado === "true" ||
            estado === "1"
        );
    }

    return true;
}