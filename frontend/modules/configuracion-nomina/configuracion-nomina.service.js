import { apiGet, apiPut } from "../../shared/js/api.js";

const RUTA = "/ConfiguracionNomina";

export async function obtenerConfiguracionNomina() {
    return apiGet(RUTA);
}

export async function actualizarConfiguracionNomina(datos) {
    return apiPut(RUTA, datos);
}
