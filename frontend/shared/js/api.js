import {
    obtenerToken,
    limpiarSesion
} from "./auth.js";

export const API_URL =
    "https://localhost:7214/api";

export async function apiGet(ruta) {
    return solicitarApi(
        ruta,
        {
            method: "GET"
        }
    );
}

export async function apiPost(ruta, datos) {
    return solicitarApi(
        ruta,
        {
            method: "POST",
            body: JSON.stringify(datos)
        }
    );
}

export async function apiPut(ruta, datos) {
    return solicitarApi(
        ruta,
        {
            method: "PUT",
            body: JSON.stringify(datos)
        }
    );
}

export async function apiPatch(ruta, datos) {
    return solicitarApi(
        ruta,
        {
            method: "PATCH",
            body: JSON.stringify(datos)
        }
    );
}

export async function apiDelete(ruta) {
    return solicitarApi(
        ruta,
        {
            method: "DELETE"
        }
    );
}

export async function solicitarApi(
    ruta,
    opciones = {}
) {
    const token = obtenerToken();

    const headers = {
        "Content-Type": "application/json",
        ...opciones.headers
    };

    if (token) {
        headers.Authorization =
            `Bearer ${token}`;
    }

    let response;

    try {
        response = await fetch(
            `${API_URL}${ruta}`,
            {
                ...opciones,
                headers
            }
        );
    } catch {
        throw new Error(
            "No fue posible conectarse con la API."
        );
    }

    if (response.status === 401) {
        limpiarSesion();

        window.location.replace(
            "../login/login.html"
        );

        throw new Error(
            "La sesión ha expirado."
        );
    }

    const data = await leerRespuesta(response);

    if (!response.ok) {
        throw new Error(
            obtenerMensajeError(data)
        );
    }

    return data;
}

async function leerRespuesta(response) {
    if (response.status === 204) {
        return null;
    }

    const texto = await response.text();

    if (!texto) {
        return null;
    }

    try {
        return JSON.parse(texto);
    } catch {
        return {
            mensaje: texto
        };
    }
}

function obtenerMensajeError(data) {
    if (!data) {
        return "Ocurrió un error al procesar la solicitud.";
    }

    if (data.mensaje || data.Mensaje) {
        return data.mensaje ?? data.Mensaje;
    }

    if (data.title || data.Title) {
        return data.title ?? data.Title;
    }

    if (data.errors || data.Errors) {
        const errores =
            data.errors ?? data.Errors;

        const mensajes =
            Object.values(errores).flat();

        if (mensajes.length > 0) {
            return mensajes[0];
        }
    }

    return "No fue posible completar la operación.";
}