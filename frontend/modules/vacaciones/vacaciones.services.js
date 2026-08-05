import {
    apiGet,
    apiPost,
    apiPut,
    apiPatch
} from "../../shared/js/api.js";

const RUTA_VACACIONES =
    "/Vacaciones";

const RUTA_EMPLEADOS =
    "/Empleados";


export async function obtenerVacaciones() {
    const respuesta =
        await apiGet(
            RUTA_VACACIONES
        );

    return normalizarListaVacaciones(
        respuesta
    );
}

export async function obtenerVacacionPorId(
    idVacacion
) {
    const respuesta =
        await apiGet(
            `${RUTA_VACACIONES}/${idVacacion}`
        );

    return normalizarVacacion(
        respuesta
    );
}

export async function obtenerVacacionesPorEmpleado(
    codigoEmpleado
) {
    const respuesta =
        await apiGet(
            `${RUTA_VACACIONES}/empleado/${encodeURIComponent(
                codigoEmpleado
            )}`
        );

    return normalizarListaVacaciones(
        respuesta
    );
}

export async function registrarVacacion(
    datos
) {
    return apiPost(
        RUTA_VACACIONES,
        {
            codigoEmpleado:
                datos.codigoEmpleado,

            idAdministrador:
                datos.idAdministrador,

            fechaInicio:
                datos.fechaInicio,

            fechaFin:
                datos.fechaFin,

            motivo:
                datos.motivo,

            observacion:
                datos.observacion
        }
    );
}

export async function actualizarVacacion(
    idVacacion,
    datos
) {
    return apiPut(
        `${RUTA_VACACIONES}/${idVacacion}`,
        {
            codigoEmpleado:
                datos.codigoEmpleado,

            idAdministrador:
                datos.idAdministrador,

            fechaInicio:
                datos.fechaInicio,

            fechaFin:
                datos.fechaFin,

            motivo:
                datos.motivo,

            observacion:
                datos.observacion
        }
    );
}

export async function cancelarVacacion(
    idVacacion
) {
    /*
     * El endpoint mostrado en Swagger solamente
     * recibe idVacacion en la ruta.
     *
     * Se envía un objeto vacío para que apiPatch
     * pueda construir la solicitud.
     */
    return apiPatch(
        `${RUTA_VACACIONES}/${idVacacion}/cancelar`,
        {}
    );
}


export async function obtenerEmpleadosParaVacaciones() {
    const respuesta =
        await apiGet(
            RUTA_EMPLEADOS
        );

    return normalizarListaEmpleados(
        respuesta
    );
}

function normalizarListaVacaciones(
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
        .map(normalizarVacacion)
        .filter(Boolean);
}

function normalizarVacacion(
    vacacion
) {
    if (!vacacion) {
        return null;
    }

    return {
        idVacacion:
            vacacion.idVacacion ??
            vacacion.IdVacacion ??
            vacacion.id ??
            vacacion.Id,

        idEmpleado:
            vacacion.idEmpleado ??
            vacacion.IdEmpleado ??
            0,

        codigoEmpleado:
            vacacion.codigoEmpleado ??
            vacacion.CodigoEmpleado ??
            "",

        nombreEmpleado:
            vacacion.nombreEmpleado ??
            vacacion.NombreEmpleado ??
            "",

        idAdministrador:
            vacacion.idAdministrador ??
            vacacion.IdAdministrador ??
            0,

        nombreAdministrador:
            vacacion.nombreAdministrador ??
            vacacion.NombreAdministrador ??
            "",

        fechaInicio:
            normalizarFecha(
                vacacion.fechaInicio ??
                vacacion.FechaInicio
            ),

        fechaFin:
            normalizarFecha(
                vacacion.fechaFin ??
                vacacion.FechaFin
            ),

        motivo:
            vacacion.motivo ??
            vacacion.Motivo ??
            "",

        observacion:
            vacacion.observacion ??
            vacacion.Observacion ??
            "",

        cancelada:
            convertirBooleano(
                vacacion.cancelada ??
                vacacion.Cancelada
            ),

        fechaCreacion:
            vacacion.fechaCreacion ??
            vacacion.FechaCreacion ??
            null,

        fechaActualizacion:
            vacacion.fechaActualizacion ??
            vacacion.FechaActualizacion ??
            null
    };
}

/* =========================
   NORMALIZACIÓN EMPLEADOS
   ========================= */

function normalizarListaEmpleados(
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
        .map(normalizarEmpleado)
        .filter(Boolean)
        .filter(empleado => empleado.activo);
}

function normalizarEmpleado(
    empleado
) {
    if (!empleado) {
        return null;
    }

    const nombres =
        empleado.nombres ??
        empleado.Nombres ??
        "";

    const apellidos =
        empleado.apellidos ??
        empleado.Apellidos ??
        "";

    const nombreCompleto =
        empleado.nombreCompleto ??
        empleado.NombreCompleto ??
        `${nombres} ${apellidos}`.trim();

    const estadoOriginal =
        empleado.activo ??
        empleado.Activo ??
        empleado.estado ??
        empleado.Estado;

    return {
        idEmpleado:
            empleado.idEmpleado ??
            empleado.IdEmpleado ??
            empleado.id ??
            empleado.Id,

        codigoEmpleado:
            empleado.codigoEmpleado ??
            empleado.CodigoEmpleado ??
            "",

        nombreCompleto,

        activo:
            convertirEstadoEmpleado(
                estadoOriginal
            )
    };
}

/* =========================
   FUNCIONES AUXILIARES
   ========================= */

function normalizarFecha(
    valor
) {
    if (!valor) {
        return "";
    }

    return String(valor).slice(0, 10);
}

function convertirBooleano(
    valor
) {
    if (typeof valor === "boolean") {
        return valor;
    }

    if (typeof valor === "number") {
        return valor === 1;
    }

    if (typeof valor === "string") {
        const texto =
            valor.trim().toLowerCase();

        return (
            texto === "true" ||
            texto === "1" ||
            texto === "si" ||
            texto === "sí"
        );
    }

    return false;
}

function convertirEstadoEmpleado(
    valor
) {
    if (
        valor === undefined ||
        valor === null
    ) {
        return true;
    }

    if (typeof valor === "boolean") {
        return valor;
    }

    if (typeof valor === "number") {
        /*
         * En caso de que el enum use:
         * 1 = Activo
         * 2 = Inactivo
         */
        return valor === 1;
    }

    if (typeof valor === "string") {
        const texto =
            valor.trim().toLowerCase();

        return (
            texto === "activo" ||
            texto === "activa" ||
            texto === "true" ||
            texto === "1"
        );
    }

    return true;
}