const API_URL = "https://localhost:7214/api";

export async function iniciarSesion(correo, contrasena) {
    const response = await fetch(
        `${API_URL}/autenticacion/login`,
        {
            method: "POST",

            headers: {
                "Content-Type": "application/json"
            },

            body: JSON.stringify({
                correo,
                contrasena
            })
        }
    );

    const data = await leerRespuesta(response);

    if (!response.ok) {
        throw new Error(
            obtenerMensajeError(data)
        );
    }

    return data;
}

async function leerRespuesta(response) {
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
        return "No fue posible iniciar sesión.";
    }

    if (data.mensaje || data.Mensaje) {
        return data.mensaje ?? data.Mensaje;
    }

    if (data.title) {
        return data.title;
    }

    if (data.errors) {
        const mensajes = Object
            .values(data.errors)
            .flat();

        if (mensajes.length > 0) {
            return mensajes[0];
        }
    }

    return "Correo o contraseña incorrectos.";
}