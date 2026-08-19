import {
    apiGet,
    apiPost,
    apiPut,
    apiPatch
} from "../../shared/js/api.js";

const RUTA_HORARIOS = "/horariosLaborales";

export async function obtenerHorariosLaborales() {
    const respuesta = await apiGet(RUTA_HORARIOS);
    return normalizarLista(respuesta, normalizarHorario);
}

export async function crearHorarioLaboral(datos) {
    return apiPost(RUTA_HORARIOS, {
        nombre: datos.nombre,
        horaEntrada: datos.horaEntrada,
        horaSalida: datos.horaSalida,
        diasLaborales: datos.diasLaborales
    });
}

export async function actualizarHorarioLaboral(idHorarioLaboral, datos) {
    return apiPut(`${RUTA_HORARIOS}/${idHorarioLaboral}`, {
        nombre: datos.nombre,
        horaEntrada: datos.horaEntrada,
        horaSalida: datos.horaSalida,
        diasLaborales: datos.diasLaborales
    });
}

export async function cambiarEstadoHorarioLaboral(idHorarioLaboral, activo) {
    return apiPatch(`${RUTA_HORARIOS}/${idHorarioLaboral}/estado`, { activo });
}

function normalizarLista(respuesta, normalizar) {
    if (!respuesta) return [];
    const registros = Array.isArray(respuesta)
        ? respuesta
        : respuesta.items ?? respuesta.Items ?? respuesta.datos ?? respuesta.Datos ?? respuesta.registros ?? respuesta.Registros ?? respuesta.resultados ?? respuesta.Resultados ?? [];
    return Array.isArray(registros) ? registros.map(normalizar) : [];
}

function normalizarHorario(horario) {
    return {
        idHorarioLaboral: horario.idHorarioLaboral ?? horario.IdHorarioLaboral ?? horario.id ?? horario.Id,
        nombre: horario.nombre ?? horario.Nombre ?? "",
        horaEntrada: horario.horaEntrada ?? horario.HoraEntrada ?? "",
        horaSalida: horario.horaSalida ?? horario.HoraSalida ?? "",
        diasLaborales: horario.diasLaborales ?? horario.DiasLaborales ?? "",
        activo: convertirActivo(horario.activo ?? horario.Activo ?? horario.estado ?? horario.Estado ?? true)
    };
}

function convertirActivo(valor) {
    if (typeof valor === "boolean") return valor;
    if (typeof valor === "number") return valor === 1;
    if (typeof valor === "string") {
        const valorNormalizado = valor.trim().toLowerCase();
        return valorNormalizado === "activo" || valorNormalizado === "true" || valorNormalizado === "1";
    }
    return true;
}
