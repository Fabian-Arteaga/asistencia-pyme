import {
    protegerPagina
} from "../../shared/js/auth.js";

import {
    inicializarLayout
} from "../../shared/js/layout.js";

import {
    obtenerResumenDashboard
} from "./dashboard.service.js";

const totalEmpleados =
    document.getElementById("totalEmpleados");

const totalAsistencias =
    document.getElementById("totalAsistencias");

const totalVacaciones =
    document.getElementById("totalVacaciones");

const totalPlanillas =
    document.getElementById("totalPlanillas");

const mensajeDashboard =
    document.getElementById("mensajeDashboard");

document.addEventListener(
    "DOMContentLoaded",
    inicializarDashboard
);

async function inicializarDashboard() {
    const accesoPermitido =
        protegerPagina();

    if (!accesoPermitido) {
        return;
    }

    inicializarLayout({
        titulo: "Dashboard",
        paginaActiva: "dashboard"
    });

    await cargarResumen();
}

async function cargarResumen() {
    ocultarMensaje();

    try {
        const resumen =
            await obtenerResumenDashboard();

        mostrarCantidad(
            totalEmpleados,
            resumen.empleados
        );

        mostrarCantidad(
            totalAsistencias,
            resumen.asistencias
        );

        mostrarCantidad(
            totalVacaciones,
            resumen.vacaciones
        );

        mostrarCantidad(
            totalPlanillas,
            resumen.planillas
        );

        const sinInformacion =
            Object
                .values(resumen)
                .every(valor => valor === null);

        if (sinInformacion) {
            mostrarMensaje(
                "No fue posible cargar la información del dashboard."
            );
        }
    } catch (error) {
        console.error(error);

        mostrarCantidad(totalEmpleados, null);
        mostrarCantidad(totalAsistencias, null);
        mostrarCantidad(totalVacaciones, null);
        mostrarCantidad(totalPlanillas, null);

        mostrarMensaje(
            "No fue posible conectarse con la API."
        );
    }
}

function mostrarCantidad(elemento, cantidad) {
    elemento.textContent =
        cantidad === null
            ? "--"
            : cantidad;
}

function mostrarMensaje(mensaje) {
    mensajeDashboard.textContent = mensaje;

    mensajeDashboard.className =
        "mensaje-dashboard";
}

function ocultarMensaje() {
    mensajeDashboard.textContent = "";

    mensajeDashboard.className =
        "mensaje-dashboard mensaje-dashboard--oculto";
}