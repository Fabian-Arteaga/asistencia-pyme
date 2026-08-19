import {
    apiGet,
    apiPost,
    apiPut,
    apiPatch
} from "../../shared/js/api.js";

const RUTA_DEPARTAMENTOS = "/departamentos";

export async function obtenerDepartamentos() {
    const respuesta = await apiGet(RUTA_DEPARTAMENTOS);
    return normalizarLista(respuesta, normalizarDepartamento);
}

export async function crearDepartamento(datos) {
    return apiPost(RUTA_DEPARTAMENTOS, {
        nombre: datos.nombre,
        descripcion: datos.descripcion
    });
}

export async function actualizarDepartamento(idDepartamento, datos) {
    return apiPut(`${RUTA_DEPARTAMENTOS}/${idDepartamento}`, {
        nombre: datos.nombre,
        descripcion: datos.descripcion
    });
}

export async function cambiarEstadoDepartamento(idDepartamento, activo) {
    return apiPatch(`${RUTA_DEPARTAMENTOS}/${idDepartamento}/estado`, { activo });
}

function normalizarLista(respuesta, normalizar) {
    if (!respuesta) return [];
    const registros = Array.isArray(respuesta)
        ? respuesta
        : respuesta.items ?? respuesta.Items ?? respuesta.datos ?? respuesta.Datos ?? respuesta.registros ?? respuesta.Registros ?? respuesta.resultados ?? respuesta.Resultados ?? [];
    return Array.isArray(registros) ? registros.map(normalizar) : [];
}

function normalizarDepartamento(departamento) {
    return {
        idDepartamento: departamento.idDepartamento ?? departamento.IdDepartamento ?? departamento.id ?? departamento.Id,
        nombre: departamento.nombre ?? departamento.Nombre ?? "",
        descripcion: departamento.descripcion ?? departamento.Descripcion ?? "",
        activo: convertirActivo(departamento.activo ?? departamento.Activo ?? departamento.estado ?? departamento.Estado ?? true)
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
