import { apiGet, apiPatch } from "../../shared/js/api.js";

const RUTA = "/HorasExtras";

export async function obtenerHorasExtrasPendientes() {
    return apiGet(`${RUTA}/pendientes`);
}

export async function aprobarHoraExtra(idHoraExtra, datos) {
    return apiPatch(`${RUTA}/${idHoraExtra}/aprobar`, datos);
}

export async function rechazarHoraExtra(idHoraExtra, datos) {
    return apiPatch(`${RUTA}/${idHoraExtra}/rechazar`, datos);
}
