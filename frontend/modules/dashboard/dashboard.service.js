import {
    apiGet
} from "../../shared/js/api.js";

export async function obtenerResumenDashboard() {
    const resultados =
        await Promise.allSettled([
            apiGet("/empleados"),
            apiGet("/asistencias"),
            apiGet("/vacaciones"),
            apiGet("/planillas")
        ]);

    return {
        empleados: obtenerCantidad(resultados[0]),
        asistencias: obtenerCantidad(resultados[1]),
        vacaciones: obtenerCantidad(resultados[2]),
        planillas: obtenerCantidad(resultados[3])
    };
}

function obtenerCantidad(resultado) {
    if (resultado.status !== "fulfilled") {
        return null;
    }

    return contarRegistros(resultado.value);
}

function contarRegistros(respuesta) {
    if (!respuesta) {
        return 0;
    }

    if (Array.isArray(respuesta)) {
        return respuesta.length;
    }

    const total =
        respuesta.total ??
        respuesta.Total ??
        respuesta.totalCount ??
        respuesta.TotalCount;

    if (typeof total === "number") {
        return total;
    }

    const registros =
        respuesta.items ??
        respuesta.Items ??
        respuesta.datos ??
        respuesta.Datos ??
        respuesta.registros ??
        respuesta.Registros;

    if (Array.isArray(registros)) {
        return registros.length;
    }

    return 0;
}