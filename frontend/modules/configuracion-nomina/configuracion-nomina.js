import { protegerPagina } from "../../shared/js/auth.js";
import { inicializarLayout } from "../../shared/js/layout.js";
import { obtenerConfiguracionNomina, actualizarConfiguracionNomina } from "./configuracion-nomina.service.js";

const form = document.getElementById("formConfiguracionNomina");
const mensaje = document.getElementById("mensajeConfiguracionNomina");
const inputMinutosToleranciaEntrada = document.getElementById("minutosToleranciaEntrada");
const inputMultiplicadorHoraExtra = document.getElementById("multiplicadorHoraExtra");
const inputDiasVacacionesPorMes = document.getElementById("diasVacacionesPorMes");
const inputDiasBaseProrrateoVacaciones = document.getElementById("diasBaseProrrateoVacaciones");
const inputPoliticaDescuentoTardanza = document.getElementById("politicaDescuentoTardanza");
const inputMinutosMaximosVerificacionDepartamento = document.getElementById("minutosMaximosVerificacionDepartamento");
const inputTasaINSS = document.getElementById("tasaINSS");

const botonGuardar = document.getElementById("btnGuardarConfiguracionNomina");

document.addEventListener("DOMContentLoaded", inicializarModulo);

async function inicializarModulo() {
    const accesoPermitido = protegerPagina();
    if (!accesoPermitido) {
        return;
    }

    inicializarLayout({ titulo: "Configuración de nómina", paginaActiva: "configuracion-nomina" });
    form.addEventListener("submit", guardarConfiguracion);
    await cargarConfiguracion();
}

async function cargarConfiguracion() {
    try {
        const configuracion = await obtenerConfiguracionNomina();
        if (!configuracion) {
            mostrarMensaje("No existe configuración registrada.", "error");
            return;
        }

        inputMinutosToleranciaEntrada.value = configuracion.minutosToleranciaEntrada ?? 10;
        inputMultiplicadorHoraExtra.value = configuracion.multiplicadorHoraExtra ?? 2;
        inputDiasVacacionesPorMes.value = configuracion.diasVacacionesPorMes ?? 2.5;
        inputDiasBaseProrrateoVacaciones.value = configuracion.diasBaseProrrateoVacaciones ?? 30;
        inputPoliticaDescuentoTardanza.value = configuracion.politicaDescuentoTardanza ?? 1;
        inputMinutosMaximosVerificacionDepartamento.value = configuracion.minutosMaximosVerificacionDepartamento ?? 10;
        inputTasaINSS.value = configuracion.tasaINSS ?? "";
    } catch (error) {
        console.error(error);
        mostrarMensaje(error.message || "No fue posible cargar la configuración.", "error");
    }
}

async function guardarConfiguracion(event) {
    event.preventDefault();

    const payload = {
        minutosToleranciaEntrada: Number(inputMinutosToleranciaEntrada.value || 0),
        multiplicadorHoraExtra: Number(inputMultiplicadorHoraExtra.value || 0),
        diasVacacionesPorMes: Number(inputDiasVacacionesPorMes.value || 0),
        diasBaseProrrateoVacaciones: Number(inputDiasBaseProrrateoVacaciones.value || 0),
        politicaDescuentoTardanza: Number(inputPoliticaDescuentoTardanza.value || 0),
        minutosMaximosVerificacionDepartamento: Number(inputMinutosMaximosVerificacionDepartamento.value || 0),
        tasaINSS: inputTasaINSS.value === "" ? null : Number(inputTasaINSS.value)
    };

    try {
        botonGuardar.disabled = true;
        botonGuardar.textContent = "Guardando...";
        await actualizarConfiguracionNomina(payload);
        mostrarMensaje("Configuración actualizada correctamente.", "exito");
    } catch (error) {
        console.error(error);
        mostrarMensaje(error.message || "No fue posible guardar la configuración.", "error");
    } finally {
        botonGuardar.disabled = false;
        botonGuardar.textContent = "Guardar cambios";
    }
}

function mostrarMensaje(texto, tipo) {
    mensaje.textContent = texto;
    mensaje.classList.remove("mensaje--error", "mensaje--exito");
    mensaje.classList.add(tipo === "error" ? "mensaje--error" : "mensaje--exito");
    mensaje.classList.add("mensaje--visible");
}
