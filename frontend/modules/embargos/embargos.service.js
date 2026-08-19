import { apiGet, apiPost, apiPut, apiPatch } from "../../shared/js/api.js";

const RUTA = "/Embargos";

export async function obtenerEmbargos() {
    return apiGet(RUTA);
}

export async function crearEmbargo(datos) {
    return apiPost(RUTA, datos);
}

export async function actualizarEmbargo(idEmbargo, datos) {
    return apiPut(`${RUTA}/${idEmbargo}`, datos);
}

export async function cambiarEstadoEmbargo(idEmbargo, activo) {
    return apiPatch(`${RUTA}/${idEmbargo}/estado`, { activo });
}

export async function obtenerEmpleados() {
    return apiGet("/Empleados");
}
