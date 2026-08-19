import {
    apiGet,
    apiPost,
    apiPut,
    apiPatch
} from "../../shared/js/api.js";

const RUTA_BASE = "/EvaluacionesDesempeno";
const RUTA_EMPLEADOS = "/empleados";
const RUTA_DEPARTAMENTOS = "/departamentos";

// 1. Evaluaciones
export async function obtenerEvaluaciones(filtros = {}) {
    const params = new URLSearchParams();
    if (filtros.idPeriodo) params.append("idPeriodo", filtros.idPeriodo);
    if (filtros.idEmpleadoEvaluado) params.append("idEmpleadoEvaluado", filtros.idEmpleadoEvaluado);
    if (filtros.idEvaluador) params.append("idEvaluador", filtros.idEvaluador);
    if (filtros.tipoEvaluador) params.append("tipoEvaluador", filtros.tipoEvaluador);
    if (filtros.estado) params.append("estado", filtros.estado);
    if (filtros.idDepartamento) params.append("idDepartamento", filtros.idDepartamento);

    const query = params.toString() ? `?${params.toString()}` : "";
    return await apiGet(`${RUTA_BASE}${query}`);
}

export async function obtenerEvaluacionPorId(id) {
    return await apiGet(`${RUTA_BASE}/${id}`);
}

export async function asignarEvaluacion(datos) {
    return await apiPost(`${RUTA_BASE}/asignar`, datos);
}

export async function guardarRespuestasEvaluacion(id, datos) {
    return await apiPut(`${RUTA_BASE}/${id}/guardar-respuestas`, datos);
}

export async function completarEvaluacion(id, datos) {
    return await apiPost(`${RUTA_BASE}/${id}/completar`, datos);
}

export async function obtenerConsolidado360(idEmpleado, idPeriodo) {
    return await apiGet(`${RUTA_BASE}/empleado/${idEmpleado}/consolidado-360?idPeriodo=${idPeriodo}`);
}

export async function obtenerHistorialEmpleado(idEmpleado) {
    return await apiGet(`${RUTA_BASE}/empleado/${idEmpleado}/historial`);
}

// 2. Períodos
export async function obtenerPeriodos() {
    return await apiGet(`${RUTA_BASE}/periodos`);
}

export async function obtenerPeriodoActivo() {
    try {
        return await apiGet(`${RUTA_BASE}/periodos/activo`);
    } catch {
        return null;
    }
}

export async function crearPeriodo(datos) {
    return await apiPost(`${RUTA_BASE}/periodos`, datos);
}

export async function actualizarPeriodo(id, datos) {
    return await apiPut(`${RUTA_BASE}/periodos/${id}`, datos);
}

export async function cambiarEstadoPeriodo(id, nuevoEstado) {
    return await apiPatch(`${RUTA_BASE}/periodos/${id}/estado`, {
        idPeriodoEvaluacion: id,
        nuevoEstado
    });
}

// 3. Checklist y Ponderaciones
export async function obtenerChecklist(soloActivos = null) {
    const query = soloActivos !== null ? `?soloActivos=${soloActivos}` : "";
    return await apiGet(`${RUTA_BASE}/checklist${query}`);
}

export async function configurarPonderaciones(ponderaciones) {
    return await apiPut(`${RUTA_BASE}/categorias/ponderaciones`, {
        ponderaciones
    });
}

export async function crearCriterio(datos) {
    return await apiPost(`${RUTA_BASE}/criterios`, datos);
}

export async function actualizarCriterio(id, datos) {
    return await apiPut(`${RUTA_BASE}/criterios/${id}`, datos);
}

export async function cambiarEstadoCriterio(id, activo) {
    return await apiPatch(`${RUTA_BASE}/criterios/${id}/estado`, {
        idCriterioEvaluacion: id,
        activo
    });
}

// Auxiliares (Empleados y Departamentos)
export async function obtenerEmpleados() {
    return await apiGet(RUTA_EMPLEADOS);
}

export async function obtenerDepartamentos() {
    return await apiGet(RUTA_DEPARTAMENTOS);
}
