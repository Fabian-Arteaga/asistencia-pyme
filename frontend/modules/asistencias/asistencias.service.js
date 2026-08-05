import {
    apiGet,
    apiPost,
    apiPut
} from "../../shared/js/api.js";

const RUTA_ASISTENCIAS = "/asistencias";
const RUTA_EMPLEADOS = "/empleados";

export async function obtenerAsistencias() {
    const respuesta =
        await apiGet(RUTA_ASISTENCIAS);

    return normalizarLista(
        respuesta,
        normalizarAsistencia
    );
}

export async function obtenerEmpleados() {
    const respuesta =
        await apiGet(RUTA_EMPLEADOS);

    return normalizarLista(
        respuesta,
        normalizarEmpleado
    );
}

export async function registrarAsistenciaManual(datos) {
    return apiPost(
        `${RUTA_ASISTENCIAS}/manual`,
        {
            codigoEmpleado:
                datos.codigoEmpleado,

            horaEntrada:
                datos.horaEntrada,

            horaSalida:
                datos.horaSalida,

            observacion:
                datos.observacion
        }
    );
}

export async function corregirAsistencia(
    idAsistencia,
    datos
) {
    return apiPut(
        `${RUTA_ASISTENCIAS}/${idAsistencia}/corregir`,
        {
            horaEntrada:
                datos.horaEntrada,

            horaSalida:
                datos.horaSalida,

            observacion:
                datos.observacion,

            motivoCorreccion:
                datos.motivoCorreccion,

            idAdministrador:
                datos.idAdministrador
        }
    );
}

function normalizarLista(
    respuesta,
    normalizador
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
        .map(normalizador)
        .filter(Boolean);
}

function normalizarAsistencia(asistencia) {
    if (!asistencia) {
        return null;
    }

    return {
        idAsistencia:
            asistencia.idAsistencia ??
            asistencia.IdAsistencia ??
            asistencia.id ??
            asistencia.Id,

        idEmpleado:
            asistencia.idEmpleado ??
            asistencia.IdEmpleado,

        codigoEmpleado:
            asistencia.codigoEmpleado ??
            asistencia.CodigoEmpleado ??
            "",

        nombreEmpleado:
            asistencia.nombreEmpleado ??
            asistencia.NombreEmpleado ??
            obtenerNombreCompleto(asistencia),

        horaEntrada:
            asistencia.horaEntradaUtc ??
            asistencia.HoraEntradaUtc ??
            asistencia.horaEntrada ??
            asistencia.HoraEntrada ??
            asistencia.entrada ??
            asistencia.Entrada ??
            asistencia.fechaHoraEntrada ??
            asistencia.FechaHoraEntrada ??
            null,

        horaSalida:
            asistencia.horaSalidaUtc ??
            asistencia.HoraSalidaUtc ??
            asistencia.horaSalida ??
            asistencia.HoraSalida ??
            asistencia.salida ??
            asistencia.Salida ??
            asistencia.fechaHoraSalida ??
            asistencia.FechaHoraSalida ??
            null,

        observacion:
            asistencia.observacion ??
            asistencia.Observacion ??
            asistencia.descripcion ??
            asistencia.Descripcion ??
            "",

        esManual:
            convertirBooleano(
                asistencia.esManual ??
                asistencia.EsManual ??
                asistencia.registroManual ??
                asistencia.RegistroManual ??
                false
            )
    };
}

function normalizarEmpleado(empleado) {
    if (!empleado) {
        return null;
    }

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

        nombres:
            empleado.nombres ??
            empleado.Nombres ??
            "",

        apellidos:
            empleado.apellidos ??
            empleado.Apellidos ??
            "",

        activo:
            convertirEstadoActivo(
                empleado.activo ??
                empleado.Activo ??
                empleado.estado ??
                empleado.Estado
            )
    };
}

function obtenerNombreCompleto(registro) {
    const nombres =
        registro.nombres ??
        registro.Nombres ??
        "";

    const apellidos =
        registro.apellidos ??
        registro.Apellidos ??
        "";

    return `${nombres} ${apellidos}`.trim();
}

function convertirBooleano(valor) {
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
            valorNormalizado === "true" ||
            valorNormalizado === "1" ||
            valorNormalizado === "si" ||
            valorNormalizado === "sí"
        );
    }

    return false;
}

function convertirEstadoActivo(valor) {
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