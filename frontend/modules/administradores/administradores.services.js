import {
    apiGet,
    apiPost,
    apiPut,
    apiPatch
} from "../../shared/js/api.js";

const RUTA_ADMINISTRADORES =
    "/administradores";

export async function obtenerAdministradores() {
    const respuesta =
        await apiGet(
            RUTA_ADMINISTRADORES
        );

    return normalizarListaAdministradores(
        respuesta
    );
}

export async function obtenerAdministradorPorId(
    idAdministrador
) {
    const respuesta =
        await apiGet(
            `${RUTA_ADMINISTRADORES}/${idAdministrador}`
        );

    return normalizarAdministrador(
        respuesta
    );
}

export async function crearAdministrador(datos) {
    return apiPost(
        RUTA_ADMINISTRADORES,
        {
            nombres:
                datos.nombres,

            apellidos:
                datos.apellidos,

            correo:
                datos.correo,

            contrasena:
                datos.contrasena
        }
    );
}

export async function actualizarAdministrador(
    idAdministrador,
    datos
) {
    return apiPut(
        `${RUTA_ADMINISTRADORES}/${idAdministrador}`,
        {
            nombres:
                datos.nombres,

            apellidos:
                datos.apellidos,

            correo:
                datos.correo
        }
    );
}

export async function cambiarEstadoAdministrador(
    idAdministrador,
    activo
) {
    return apiPatch(
        `${RUTA_ADMINISTRADORES}/${idAdministrador}/estado`,
        {
            activo
        }
    );
}

export async function cambiarMiContrasena(datos) {
    return apiPatch(
        `${RUTA_ADMINISTRADORES}/me/contrasena`,
        {
            contrasenaActual:
                datos.contrasenaActual,

            nuevaContrasena:
                datos.nuevaContrasena
        }
    );
}

function normalizarListaAdministradores(
    respuesta
) {
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
        .map(normalizarAdministrador)
        .filter(Boolean);
}

function normalizarAdministrador(
    administrador
) {
    if (!administrador) {
        return null;
    }

    const nombreCompletoOriginal =
        administrador.nombreCompleto ??
        administrador.NombreCompleto ??
        "";

    let nombres =
        administrador.nombres ??
        administrador.Nombres ??
        "";

    let apellidos =
        administrador.apellidos ??
        administrador.Apellidos ??
        "";

    if (
        !nombres &&
        nombreCompletoOriginal
    ) {
        const partes =
            nombreCompletoOriginal
                .trim()
                .split(/\s+/);

        nombres =
            partes.shift() ?? "";

        apellidos =
            partes.join(" ");
    }

    const nombreCompleto =
        nombreCompletoOriginal ||
        `${nombres} ${apellidos}`.trim();

    return {
        idAdministrador:
            administrador.idAdministrador ??
            administrador.IdAdministrador ??
            administrador.id ??
            administrador.Id,

        nombres,

        apellidos,

        nombreCompleto,

        correo:
            administrador.correo ??
            administrador.Correo ??
            "",

        activo:
            convertirActivo(
                administrador.activo ??
                administrador.Activo ??
                administrador.estado ??
                administrador.Estado
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