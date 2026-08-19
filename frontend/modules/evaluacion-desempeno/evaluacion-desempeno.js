import { protegerPagina } from "../../shared/js/auth.js";
import { inicializarLayout } from "../../shared/js/layout.js";
import {
    obtenerEvaluaciones,
    obtenerEvaluacionPorId,
    asignarEvaluacion,
    guardarRespuestasEvaluacion,
    completarEvaluacion,
    obtenerConsolidado360,
    obtenerHistorialEmpleado,
    obtenerPeriodos,
    obtenerPeriodoActivo,
    crearPeriodo,
    actualizarPeriodo,
    cambiarEstadoPeriodo,
    obtenerChecklist,
    configurarPonderaciones,
    crearCriterio,
    actualizarCriterio,
    cambiarEstadoCriterio,
    obtenerEmpleados,
    obtenerDepartamentos
} from "./evaluacion-desempeno.services.js";

// Estado global del módulo
let evaluacionesGlobales = [];
let periodosGlobales = [];
let checklistGlobal = [];
let empleadosGlobales = [];
let departamentosGlobales = [];
let periodoActivoGlobal = null;
let evaluacionEnCurso = null;

document.addEventListener("DOMContentLoaded", inicializarModulo);

async function inicializarModulo() {
    const permitido = protegerPagina();
    if (!permitido) return;

    inicializarLayout({ titulo: "Evaluación de Desempeño 360°", paginaActiva: "evaluacion-desempeno" });
    configurarEventosGenerales();
    await cargarDatosIniciales();
}

function configurarEventosGenerales() {
    // 1. Navegación por pestañas
    const botonesPestana = document.querySelectorAll(".pestana-btn");
    botonesPestana.forEach(btn => {
        btn.addEventListener("click", () => {
            const pestana = btn.dataset.pestana;
            activarPestana(pestana);
        });
    });

    // 2. Modales (Cerrar por fondo o botón con data-cerrar)
    document.querySelectorAll("[data-cerrar]").forEach(el => {
        el.addEventListener("click", () => {
            const idModal = el.dataset.cerrar;
            cerrarModal(idModal);
        });
    });

    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            document.querySelectorAll(".modal:not(.modal--oculto)").forEach(m => m.classList.add("modal--oculto"));
        }
    });

    // 3. Botones principales de encabezado
    document.getElementById("btnAbrirAsignar")?.addEventListener("click", abrirModalAsignar);
    document.getElementById("btnAbrirNuevoPeriodo")?.addEventListener("click", () => abrirModalPeriodo());
    document.getElementById("btnCrearPeriodoDirecto")?.addEventListener("click", () => abrirModalPeriodo());
    document.getElementById("btnNuevoCriterio")?.addEventListener("click", () => abrirModalCriterio());

    // 4. Filtros
    document.getElementById("btnFiltrarEvaluaciones")?.addEventListener("click", aplicarFiltrosEvaluaciones);
    document.getElementById("filtroBuscar")?.addEventListener("input", aplicarFiltrosEvaluaciones);
    document.getElementById("filtroPeriodo")?.addEventListener("change", aplicarFiltrosEvaluaciones);
    document.getElementById("filtroDepartamento")?.addEventListener("change", aplicarFiltrosEvaluaciones);
    document.getElementById("filtroEstado")?.addEventListener("change", aplicarFiltrosEvaluaciones);
    document.getElementById("filtroTipo")?.addEventListener("change", aplicarFiltrosEvaluaciones);

    // 5. Formularios
    document.getElementById("formAsignarEvaluacion")?.addEventListener("submit", manejarSubmitAsignar);
    document.getElementById("formPeriodo")?.addEventListener("submit", manejarSubmitPeriodo);
    document.getElementById("formPonderaciones")?.addEventListener("submit", manejarSubmitPonderaciones);
    document.getElementById("formCriterio")?.addEventListener("submit", manejarSubmitCriterio);
    document.getElementById("formCuestionarioEvaluacion")?.addEventListener("submit", manejarSubmitCompletarEvaluacion);
    document.getElementById("btnGuardarBorrador")?.addEventListener("click", manejarGuardarBorrador);

    // 6. Cambios interactivos en modal Asignar
    document.getElementById("asignarTipoEvaluador")?.addEventListener("change", actualizarSelectsJerarquia);
    document.getElementById("asignarEmpleadoEvaluado")?.addEventListener("change", actualizarSelectsJerarquia);

    // 7. Eventos en tablas
    document.getElementById("tablaEvaluacionesBody")?.addEventListener("click", manejarAccionTablaEvaluaciones);
    document.getElementById("tablaPeriodosBody")?.addEventListener("click", manejarAccionTablaPeriodos);
    document.getElementById("contenedorChecklistCategorias")?.addEventListener("click", manejarAccionChecklist);

    // 8. Botón imprimir consolidado
    document.getElementById("btnImprimirConsolidado")?.addEventListener("click", () => window.print());
}

function activarPestana(pestanaNombre) {
    document.querySelectorAll(".pestana-btn").forEach(btn => {
        btn.classList.toggle("pestana-btn--activa", btn.dataset.pestana === pestanaNombre);
    });

    document.getElementById("seccionEvaluaciones").classList.toggle("seccion-pestana--activa", pestanaNombre === "evaluaciones");
    document.getElementById("seccionPeriodos").classList.toggle("seccion-pestana--activa", pestanaNombre === "periodos");
    document.getElementById("seccionPonderaciones").classList.toggle("seccion-pestana--activa", pestanaNombre === "ponderaciones");
}

function abrirModal(idModal) {
    const m = document.getElementById(idModal);
    if (m) {
        m.classList.remove("modal--oculto");
        m.setAttribute("aria-hidden", "false");
    }
}

function cerrarModal(idModal) {
    const m = document.getElementById(idModal);
    if (m) {
        m.classList.add("modal--oculto");
        m.setAttribute("aria-hidden", "true");
    }
}

function mostrarMensaje(texto, tipo = "exito") {
    const alerta = document.getElementById("mensajeAlerta");
    if (!alerta) return;

    alerta.textContent = texto;
    alerta.className = `mensaje mensaje--${tipo}`;
    alerta.classList.remove("mensaje--oculto");

    setTimeout(() => {
        alerta.classList.add("mensaje--oculto");
    }, 6000);
}

function escapeHtml(texto) {
    if (!texto) return "";
    return String(texto)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// ==========================================================================
// Carga de Datos Iniciales
// ==========================================================================
async function cargarDatosIniciales() {
    try {
        const [empleados, departamentos, periodos, checklist, activo] = await Promise.all([
            obtenerEmpleados().catch(() => []),
            obtenerDepartamentos().catch(() => []),
            obtenerPeriodos().catch(() => []),
            obtenerChecklist().catch(() => []),
            obtenerPeriodoActivo().catch(() => null)
        ]);

        empleadosGlobales = empleados || [];
        departamentosGlobales = departamentos || [];
        periodosGlobales = periodos || [];
        checklistGlobal = checklist || [];
        periodoActivoGlobal = activo;

        poblarSelectsFiltros();
        renderizarPeriodos(periodosGlobales);
        renderizarPonderaciones(checklistGlobal);
        renderizarChecklistAcordeon(checklistGlobal);

        await cargarEvaluaciones();
    } catch (error) {
        console.error("Error al cargar datos iniciales:", error);
        mostrarMensaje(error.message || "Error al cargar datos del módulo.", "error");
    }
}

function poblarSelectsFiltros() {
    // Select de períodos en filtros
    const selPeriodo = document.getElementById("filtroPeriodo");
    if (selPeriodo) {
        selPeriodo.innerHTML = '<option value="">Todos los períodos</option>' +
            periodosGlobales.map(p => `<option value="${p.idPeriodoEvaluacion}" ${periodoActivoGlobal && periodoActivoGlobal.idPeriodoEvaluacion === p.idPeriodoEvaluacion ? "selected" : ""}>${escapeHtml(p.nombre)} (${p.estadoTexto})</option>`).join("");
    }

    // Select de departamentos en filtros
    const selDepto = document.getElementById("filtroDepartamento");
    if (selDepto) {
        selDepto.innerHTML = '<option value="">Todos los departamentos</option>' +
            departamentosGlobales.map(d => `<option value="${d.idDepartamento ?? d.IdDepartamento}">${escapeHtml(d.nombre ?? d.Nombre)}</option>`).join("");
    }
}

// ==========================================================================
// PESTAÑA 1: Evaluaciones 360°
// ==========================================================================
async function cargarEvaluaciones() {
    const idPeriodo = document.getElementById("filtroPeriodo")?.value || (periodoActivoGlobal ? periodoActivoGlobal.idPeriodoEvaluacion : null);
    const idDepartamento = document.getElementById("filtroDepartamento")?.value;
    const estado = document.getElementById("filtroEstado")?.value;
    const tipoEvaluador = document.getElementById("filtroTipo")?.value;

    try {
        evaluacionesGlobales = await obtenerEvaluaciones({
            idPeriodo,
            idDepartamento,
            estado,
            tipoEvaluador
        });
        aplicarFiltrosEvaluaciones();
    } catch (error) {
        console.error("Error al cargar evaluaciones:", error);
        renderizarTablaEvaluaciones([]);
    }
}

function aplicarFiltrosEvaluaciones() {
    const busqueda = (document.getElementById("filtroBuscar")?.value || "").toLowerCase().trim();
    const idPeriodo = document.getElementById("filtroPeriodo")?.value;
    const idDepto = document.getElementById("filtroDepartamento")?.value;
    const estado = document.getElementById("filtroEstado")?.value;
    const tipo = document.getElementById("filtroTipo")?.value;

    let filtradas = [...evaluacionesGlobales];

    if (idPeriodo) {
        filtradas = filtradas.filter(e => String(e.idPeriodoEvaluacion) === String(idPeriodo));
    }
    if (idDepto) {
        filtradas = filtradas.filter(e => {
            const emp = empleadosGlobales.find(empItem => Number(empItem.idEmpleado ?? empItem.IdEmpleado) === Number(e.idEmpleadoEvaluado));
            return emp && String(emp.idDepartamento ?? emp.IdDepartamento) === String(idDepto);
        });
    }
    if (estado) {
        filtradas = filtradas.filter(e => String(e.estado) === String(estado));
    }
    if (tipo) {
        filtradas = filtradas.filter(e => String(e.tipoEvaluador) === String(tipo));
    }
    if (busqueda) {
        filtradas = filtradas.filter(e =>
            (e.nombreCompletoEvaluado || "").toLowerCase().includes(busqueda) ||
            (e.codigoEmpleadoEvaluado || "").toLowerCase().includes(busqueda) ||
            (e.nombreCompletoEvaluador || "").toLowerCase().includes(busqueda)
        );
    }

    renderizarTablaEvaluaciones(filtradas);
}

function renderizarTablaEvaluaciones(lista) {
    const tbody = document.getElementById("tablaEvaluacionesBody");
    const contenedor = document.getElementById("contenedorTablaEvaluaciones");
    const sinResultados = document.getElementById("sinEvaluaciones");

    if (!tbody) return;
    tbody.innerHTML = "";

    if (!lista || lista.length === 0) {
        contenedor?.classList.add("tabla-contenedor--oculto");
        sinResultados?.classList.remove("estado-tabla--oculto");
        return;
    }

    contenedor?.classList.remove("tabla-contenedor--oculto");
    sinResultados?.classList.add("estado-tabla--oculto");

    lista.forEach(e => {
        const tr = document.createElement("tr");

        let tipoBadgeClass = "badge-tipo--auto";
        if (e.tipoEvaluador === 2) tipoBadgeClass = "badge-tipo--jefe";
        if (e.tipoEvaluador === 3) tipoBadgeClass = "badge-tipo--sub";

        let estadoBadgeClass = "estado--pendiente";
        if (e.estado === 2) estadoBadgeClass = "estado--proceso";
        if (e.estado === 3) estadoBadgeClass = "estado--completada";

        let clasifClass = "";
        if (e.clasificacion === "Excelente") clasifClass = "clasificacion--excelente";
        else if (e.clasificacion === "Muy bueno") clasifClass = "clasificacion--muy-bueno";
        else if (e.clasificacion === "Bueno") clasifClass = "clasificacion--bueno";
        else if (e.clasificacion === "Aceptable") clasifClass = "clasificacion--aceptable";
        else if (e.clasificacion === "Necesita mejorar") clasifClass = "clasificacion--necesita-mejorar";

        const esCompletada = (e.estado === 3);

        tr.innerHTML = `
            <td>
                <strong>${escapeHtml(e.nombreCompletoEvaluado)}</strong><br>
                <small style="color: #64748b;">${escapeHtml(e.codigoEmpleadoEvaluado)}</small>
            </td>
            <td>${escapeHtml(e.cargoEvaluado)}<br><small style="color: #64748b;">${escapeHtml(e.departamentoEvaluado)}</small></td>
            <td>${escapeHtml(e.nombreCompletoEvaluador)}<br><small style="color: #64748b;">${escapeHtml(e.cargoEvaluador)}</small></td>
            <td><span class="badge-tipo ${tipoBadgeClass}">${escapeHtml(e.tipoEvaluadorTexto)}</span></td>
            <td>${escapeHtml(e.nombrePeriodo)}</td>
            <td><span class="estado ${estadoBadgeClass}">${escapeHtml(e.estadoTexto)}</span></td>
            <td><span class="puntaje-numero">${e.puntajeFinal != null ? e.puntajeFinal.toFixed(1) + " / 100" : "-"}</span></td>
            <td>${e.clasificacion && e.clasificacion !== "-" ? `<span class="clasificacion-badge ${clasifClass}">${escapeHtml(e.clasificacion)}</span>` : "-"}</td>
            <td>
                <div class="celda-acciones">
                    <button type="button" class="boton-tabla ${esCompletada ? "boton-tabla--ver" : "boton-tabla--evaluar"}" data-accion="responder" data-id="${e.idEvaluacionDesempeno}">
                        ${esCompletada ? "Ver Detalle" : "Evaluar"}
                    </button>
                    <button type="button" class="boton-tabla boton-tabla--consolidado" data-accion="consolidado" data-id-empleado="${e.idEmpleadoEvaluado}" data-id-periodo="${e.idPeriodoEvaluacion}" title="Ver consolidado 360°">
                        360°
                    </button>
                    <button type="button" class="boton-tabla boton-tabla--historial" data-accion="historial" data-id-empleado="${e.idEmpleadoEvaluado}" title="Ver historial semestral">
                        Historial
                    </button>
                </div>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

function manejarAccionTablaEvaluaciones(e) {
    const btn = e.target.closest("button[data-accion]");
    if (!btn) return;

    const accion = btn.dataset.accion;
    if (accion === "responder") {
        const id = Number(btn.dataset.id);
        abrirModalEvaluar(id);
    } else if (accion === "consolidado") {
        const idEmpleado = Number(btn.dataset.idEmpleado);
        const idPeriodo = Number(btn.dataset.idPeriodo);
        abrirModalConsolidado(idEmpleado, idPeriodo);
    } else if (accion === "historial") {
        const idEmpleado = Number(btn.dataset.idEmpleado);
        abrirModalHistorial(idEmpleado);
    }
}

// ==========================================================================
// PESTAÑA 2: Períodos Semestrales
// ==========================================================================
function renderizarPeriodos(lista) {
    const tbody = document.getElementById("tablaPeriodosBody");
    const contenedor = document.getElementById("contenedorTablaPeriodos");
    const sinResultados = document.getElementById("sinPeriodos");

    if (!tbody) return;
    tbody.innerHTML = "";

    if (!lista || lista.length === 0) {
        contenedor?.classList.add("tabla-contenedor--oculto");
        sinResultados?.classList.remove("estado-tabla--oculto");
        return;
    }

    contenedor?.classList.remove("tabla-contenedor--oculto");
    sinResultados?.classList.add("estado-tabla--oculto");

    lista.forEach(p => {
        const tr = document.createElement("tr");

        let estadoBadge = "estado--pendiente";
        if (p.estado === 2) estadoBadge = "estado--activo";
        if (p.estado === 3) estadoBadge = "estado--finalizado";

        tr.innerHTML = `
            <td><strong>${escapeHtml(p.nombre)}</strong></td>
            <td>${p.fechaInicio}</td>
            <td>${p.fechaFin}</td>
            <td><span class="estado ${estadoBadge}">${escapeHtml(p.estadoTexto)}</span></td>
            <td><strong>${p.totalEvaluaciones}</strong></td>
            <td style="color: #155724;"><strong>${p.evaluacionesCompletadas}</strong></td>
            <td style="color: #856404;"><strong>${p.evaluacionesPendientes}</strong></td>
            <td>
                <div class="celda-acciones">
                    <button type="button" class="boton-tabla boton-tabla--ver" data-accion="editar-periodo" data-id="${p.idPeriodoEvaluacion}">Editar</button>
                    ${p.estado !== 2 ? `<button type="button" class="boton-tabla boton-tabla--activar" data-accion="estado-periodo" data-id="${p.idPeriodoEvaluacion}" data-estado="2">Activar</button>` : ""}
                    ${p.estado !== 3 ? `<button type="button" class="boton-tabla boton-tabla--finalizar" data-accion="estado-periodo" data-id="${p.idPeriodoEvaluacion}" data-estado="3">Finalizar</button>` : ""}
                </div>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

function abrirModalPeriodo(idPeriodo = null) {
    const idInput = document.getElementById("idPeriodoModal");
    const nombreInput = document.getElementById("nombrePeriodo");
    const fechaIniInput = document.getElementById("fechaInicioPeriodo");
    const fechaFinInput = document.getElementById("fechaFinPeriodo");
    const tituloModal = document.getElementById("tituloModalPeriodo");

    if (idPeriodo) {
        const p = periodosGlobales.find(item => item.idPeriodoEvaluacion === idPeriodo);
        if (!p) return;
        idInput.value = p.idPeriodoEvaluacion;
        nombreInput.value = p.nombre;
        fechaIniInput.value = p.fechaInicio;
        fechaFinInput.value = p.fechaFin;
        tituloModal.textContent = "Editar Período Semestral";
    } else {
        idInput.value = "";
        nombreInput.value = "";
        fechaIniInput.value = "";
        fechaFinInput.value = "";
        tituloModal.textContent = "Nuevo Período Semestral";
    }

    abrirModal("modalPeriodo");
}

async function manejarSubmitPeriodo(e) {
    e.preventDefault();
    const id = document.getElementById("idPeriodoModal")?.value;
    const nombre = document.getElementById("nombrePeriodo")?.value.trim();
    const fechaInicio = document.getElementById("fechaInicioPeriodo")?.value;
    const fechaFin = document.getElementById("fechaFinPeriodo")?.value;

    if (!nombre || !fechaInicio || !fechaFin) {
        mostrarMensaje("Todos los campos marcados con * son obligatorios.", "error");
        return;
    }

    try {
        if (id) {
            await actualizarPeriodo(Number(id), { idPeriodoEvaluacion: Number(id), nombre, fechaInicio, fechaFin });
            mostrarMensaje("Período actualizado exitosamente.");
        } else {
            await crearPeriodo({ nombre, fechaInicio, fechaFin });
            mostrarMensaje("Período creado exitosamente.");
        }
        cerrarModal("modalPeriodo");
        periodosGlobales = await obtenerPeriodos();
        periodoActivoGlobal = await obtenerPeriodoActivo();
        poblarSelectsFiltros();
        renderizarPeriodos(periodosGlobales);
    } catch (error) {
        console.error(error);
        mostrarMensaje(error.message || "Error al guardar el período.", "error");
    }
}

async function manejarAccionTablaPeriodos(e) {
    const btn = e.target.closest("button[data-accion]");
    if (!btn) return;

    const accion = btn.dataset.accion;
    const id = Number(btn.dataset.id);

    if (accion === "editar-periodo") {
        abrirModalPeriodo(id);
    } else if (accion === "estado-periodo") {
        const nuevoEstado = Number(btn.dataset.estado);
        try {
            await cambiarEstadoPeriodo(id, nuevoEstado);
            mostrarMensaje("Estado del período modificado con éxito.");
            periodosGlobales = await obtenerPeriodos();
            periodoActivoGlobal = await obtenerPeriodoActivo();
            poblarSelectsFiltros();
            renderizarPeriodos(periodosGlobales);
        } catch (error) {
            mostrarMensaje(error.message || "No fue posible cambiar el estado.", "error");
        }
    }
}

// ==========================================================================
// PESTAÑA 3: Ponderaciones y Checklist
// ==========================================================================
function renderizarPonderaciones(categorias) {
    const grid = document.getElementById("gridPonderaciones");
    if (!grid) return;
    grid.innerHTML = "";

    categorias.forEach(cat => {
        const card = document.createElement("div");
        card.className = "tarjeta-ponderacion";
        card.innerHTML = `
            <div class="tarjeta-ponderacion__info">
                <h4>${escapeHtml(cat.nombre)}</h4>
                <p>${escapeHtml(cat.descripcion || "Categoría de evaluación")}</p>
            </div>
            <div class="tarjeta-ponderacion__input-wrap">
                <input type="number" step="0.01" min="0" max="100" class="input-ponderacion" data-id="${cat.idCategoriaEvaluacion}" value="${cat.ponderacion.toFixed(2)}">
                <span>%</span>
            </div>
        `;
        grid.appendChild(card);
    });

    // Event listener para cálculo en vivo de la suma total
    grid.querySelectorAll(".input-ponderacion").forEach(input => {
        input.addEventListener("input", recalcularSumaPonderaciones);
    });

    recalcularSumaPonderaciones();
}

function recalcularSumaPonderaciones() {
    const inputs = document.querySelectorAll(".input-ponderacion");
    let suma = 0;
    inputs.forEach(inp => {
        suma += parseFloat(inp.value) || 0;
    });

    const totalElem = document.getElementById("totalSumaPonderacion");
    const btnGuardar = document.getElementById("btnGuardarPonderaciones");
    if (!totalElem) return;

    totalElem.textContent = suma.toFixed(2) + "%";

    if (Math.abs(suma - 100.00) < 0.01) {
        totalElem.className = "suma-correcta";
        if (btnGuardar) btnGuardar.disabled = false;
    } else {
        totalElem.className = "suma-incorrecta";
        if (btnGuardar) btnGuardar.disabled = true;
    }
}

async function manejarSubmitPonderaciones(e) {
    e.preventDefault();
    const inputs = document.querySelectorAll(".input-ponderacion");
    const ponderaciones = [];

    inputs.forEach(inp => {
        ponderaciones.push({
            idCategoriaEvaluacion: Number(inp.dataset.id),
            ponderacion: parseFloat(inp.value) || 0
        });
    });

    try {
        await configurarPonderaciones(ponderaciones);
        mostrarMensaje("Ponderaciones configuradas y guardadas exitosamente.");
        checklistGlobal = await obtenerChecklist();
        renderizarPonderaciones(checklistGlobal);
        renderizarChecklistAcordeon(checklistGlobal);
    } catch (error) {
        console.error(error);
        mostrarMensaje(error.message || "Error al guardar ponderaciones.", "error");
    }
}

function renderizarChecklistAcordeon(categorias) {
    const contenedor = document.getElementById("contenedorChecklistCategorias");
    if (!contenedor) return;
    contenedor.innerHTML = "";

    categorias.forEach(cat => {
        const bloque = document.createElement("div");
        bloque.className = "categoria-bloque";

        const criteriosHtml = (cat.criterios || []).map(cr => `
            <div class="criterio-item-admin ${cr.activo ? "" : "criterio-item-admin--inactivo"}">
                <div class="criterio-item-admin__texto">
                    <strong>${escapeHtml(cr.texto)}</strong>
                    <span class="badge-estado-criterio ${cr.activo ? "badge-estado--activo" : "badge-estado--inactivo"}">
                        ${cr.activo ? "Activo" : "Inactivo"}
                    </span>
                    ${cr.descripcion ? `<br><small style="color: #64748b;">${escapeHtml(cr.descripcion)}</small>` : ""}
                </div>
                <div class="criterio-item-admin__acciones">
                    <button type="button" class="boton-tabla boton-tabla--ver" data-accion="editar-criterio" data-id="${cr.idCriterioEvaluacion}">Editar</button>
                    <button type="button" class="boton-tabla ${cr.activo ? "boton-tabla--finalizar" : "boton-tabla--activar"}" data-accion="toggle-criterio" data-id="${cr.idCriterioEvaluacion}" data-activo="${cr.activo ? "false" : "true"}">
                        ${cr.activo ? "Desactivar" : "Activar"}
                    </button>
                </div>
            </div>
        `).join("");

        bloque.innerHTML = `
            <div class="categoria-bloque__encabezado">
                <span class="categoria-bloque__titulo">${escapeHtml(cat.nombre)} (${(cat.criterios || []).length} criterios)</span>
                <span class="categoria-bloque__peso">${cat.ponderacion.toFixed(1)}%</span>
            </div>
            <div class="categoria-bloque__cuerpo">
                ${criteriosHtml || '<p style="color: #64748b; font-size: 13px;">No hay criterios registrados en esta categoría.</p>'}
            </div>
        `;
        contenedor.appendChild(bloque);
    });
}

function abrirModalCriterio(idCriterio = null) {
    const idInput = document.getElementById("idCriterioModal");
    const catSelect = document.getElementById("categoriaCriterioSelect");
    const textoInput = document.getElementById("textoCriterio");
    const descInput = document.getElementById("descripcionCriterio");
    const ordenInput = document.getElementById("ordenCriterio");
    const titulo = document.getElementById("tituloModalCriterio");

    // Llenar select de categorías
    catSelect.innerHTML = checklistGlobal.map(c => `<option value="${c.idCategoriaEvaluacion}">${escapeHtml(c.nombre)}</option>`).join("");

    if (idCriterio) {
        let encontrado = null;
        for (const cat of checklistGlobal) {
            const cr = (cat.criterios || []).find(item => item.idCriterioEvaluacion === idCriterio);
            if (cr) {
                encontrado = { ...cr, idCategoriaEvaluacion: cat.idCategoriaEvaluacion };
                break;
            }
        }

        if (!encontrado) return;
        idInput.value = encontrado.idCriterioEvaluacion;
        catSelect.value = encontrado.idCategoriaEvaluacion;
        catSelect.disabled = true;
        textoInput.value = encontrado.texto;
        descInput.value = encontrado.descripcion || "";
        ordenInput.value = encontrado.orden || 1;
        titulo.textContent = "Editar Criterio de Evaluación";
    } else {
        idInput.value = "";
        catSelect.disabled = false;
        textoInput.value = "";
        descInput.value = "";
        ordenInput.value = 1;
        titulo.textContent = "Nuevo Criterio de Evaluación";
    }

    abrirModal("modalCriterio");
}

async function manejarSubmitCriterio(e) {
    e.preventDefault();
    const id = document.getElementById("idCriterioModal")?.value;
    const idCategoria = Number(document.getElementById("categoriaCriterioSelect")?.value);
    const texto = document.getElementById("textoCriterio")?.value.trim();
    const descripcion = document.getElementById("descripcionCriterio")?.value.trim();
    const orden = Number(document.getElementById("ordenCriterio")?.value) || 1;

    if (!texto) {
        mostrarMensaje("El texto del criterio es obligatorio.", "error");
        return;
    }

    try {
        if (id) {
            await actualizarCriterio(Number(id), { idCriterioEvaluacion: Number(id), texto, descripcion, orden });
            mostrarMensaje("Criterio actualizado con éxito.");
        } else {
            await crearCriterio({ idCategoriaEvaluacion: idCategoria, texto, descripcion, orden });
            mostrarMensaje("Criterio creado con éxito.");
        }
        cerrarModal("modalCriterio");
        checklistGlobal = await obtenerChecklist();
        renderizarChecklistAcordeon(checklistGlobal);
    } catch (error) {
        console.error(error);
        mostrarMensaje(error.message || "Error al guardar el criterio.", "error");
    }
}

async function manejarAccionChecklist(e) {
    const btn = e.target.closest("button[data-accion]");
    if (!btn) return;

    const accion = btn.dataset.accion;
    const id = Number(btn.dataset.id);

    if (accion === "editar-criterio") {
        abrirModalCriterio(id);
    } else if (accion === "toggle-criterio") {
        const activo = btn.dataset.activo === "true";
        try {
            await cambiarEstadoCriterio(id, activo);
            mostrarMensaje("Estado del criterio actualizado.");
            checklistGlobal = await obtenerChecklist();
            renderizarChecklistAcordeon(checklistGlobal);
        } catch (error) {
            mostrarMensaje(error.message || "No fue posible modificar el criterio.", "error");
        }
    }
}

// ==========================================================================
// MODAL 1: Asignar Evaluación 360°
// ==========================================================================
function abrirModalAsignar() {
    const selPeriodo = document.getElementById("asignarPeriodo");
    const selEvaluado = document.getElementById("asignarEmpleadoEvaluado");

    selPeriodo.innerHTML = periodosGlobales
        .filter(p => p.estado !== 3) // No finalizados
        .map(p => `<option value="${p.idPeriodoEvaluacion}" ${periodoActivoGlobal && periodoActivoGlobal.idPeriodoEvaluacion === p.idPeriodoEvaluacion ? "selected" : ""}>${escapeHtml(p.nombre)}</option>`)
        .join("");

    selEvaluado.innerHTML = '<option value="">Seleccione colaborador...</option>' +
        empleadosGlobales.map(emp => `<option value="${emp.idEmpleado ?? emp.IdEmpleado}">${escapeHtml(emp.codigoEmpleado ?? "")} - ${escapeHtml(emp.nombres ?? "")} ${escapeHtml(emp.apellidos ?? "")}</option>`).join("");

    actualizarSelectsJerarquia();
    abrirModal("modalAsignar");
}

function actualizarSelectsJerarquia() {
    const idEvaluado = Number(document.getElementById("asignarEmpleadoEvaluado")?.value);
    const tipo = Number(document.getElementById("asignarTipoEvaluador")?.value);
    const selEvaluador = document.getElementById("asignarEvaluador");
    const infoDiv = document.getElementById("infoJerarquia");

    if (!selEvaluador || !infoDiv) return;

    const evaluado = empleadosGlobales.find(e => Number(e.idEmpleado ?? e.IdEmpleado) === idEvaluado);

    if (!idEvaluado || !evaluado) {
        selEvaluador.innerHTML = '<option value="">Seleccione primero el colaborador a evaluar</option>';
        infoDiv.innerHTML = "Seleccione un colaborador para verificar su jerarquía y opciones disponibles.";
        return;
    }

    if (tipo === 1) {
        // Autoevaluación: evaluador es el mismo
        selEvaluador.innerHTML = `<option value="${evaluado.idEmpleado ?? evaluado.IdEmpleado}" selected>${escapeHtml(evaluado.nombres ?? "")} ${escapeHtml(evaluado.apellidos ?? "")} (Mismo colaborador)</option>`;
        selEvaluador.disabled = true;
        infoDiv.innerHTML = `<strong>Autoevaluación:</strong> El colaborador evaluará su propio desempeño del período.`;
    } else if (tipo === 2) {
        // Jefe Directo
        selEvaluador.disabled = false;
        const idJefe = evaluado.idJefeDirecto ?? evaluado.IdJefeDirecto;
        if (idJefe) {
            const jefe = empleadosGlobales.find(e => Number(e.idEmpleado ?? e.IdEmpleado) === Number(idJefe));
            if (jefe) {
                selEvaluador.innerHTML = `<option value="${jefe.idEmpleado ?? jefe.IdEmpleado}" selected>${escapeHtml(jefe.codigoEmpleado ?? "")} - ${escapeHtml(jefe.nombres ?? "")} ${escapeHtml(jefe.apellidos ?? "")} (Jefe Asignado)</option>`;
                infoDiv.innerHTML = `<strong>Jefe Directo Registrado:</strong> ${escapeHtml(jefe.nombres ?? "")} ${escapeHtml(jefe.apellidos ?? "")}`;
            } else {
                selEvaluador.innerHTML = '<option value="">Jefe asignado no encontrado</option>';
                infoDiv.innerHTML = `<span style="color: #721c24;">El colaborador tiene asignado IdJefeDirecto (${idJefe}) pero no se encuentra en el catálogo.</span>`;
            }
        } else {
            // Mostrar todos los otros empleados pero advertir
            selEvaluador.innerHTML = '<option value="">-- Sin jefe asignado en el perfil --</option>' +
                empleadosGlobales.filter(e => Number(e.idEmpleado ?? e.IdEmpleado) !== idEvaluado)
                    .map(e => `<option value="${e.idEmpleado ?? e.IdEmpleado}">${escapeHtml(e.codigoEmpleado ?? "")} - ${escapeHtml(e.nombres ?? "")} ${escapeHtml(e.apellidos ?? "")}</option>`).join("");
            infoDiv.innerHTML = `<span style="color: #856404;"><strong>Advertencia:</strong> Este colaborador no tiene configurado un Jefe Directo en su ficha de empleado. Para asignarlo oficialmente, configure su jefe directo.</span>`;
        }
    } else if (tipo === 3) {
        // Subordinados
        selEvaluador.disabled = false;
        const subordinados = empleadosGlobales.filter(e => Number(e.idJefeDirecto ?? e.IdJefeDirecto) === idEvaluado);

        if (subordinados.length > 0) {
            selEvaluador.innerHTML = '<option value="">Seleccione subordinado...</option>' +
                subordinados.map(sub => `<option value="${sub.idEmpleado ?? sub.IdEmpleado}">${escapeHtml(sub.codigoEmpleado ?? "")} - ${escapeHtml(sub.nombres ?? "")} ${escapeHtml(sub.apellidos ?? "")}</option>`).join("");
            infoDiv.innerHTML = `<strong>Subordinados detectados (${subordinados.length}):</strong> Seleccione cuál de los subordinados realizará la evaluación ascendente.`;
        } else {
            selEvaluador.innerHTML = '<option value="">No tiene subordinados directos registrados</option>';
            infoDiv.innerHTML = `<span style="color: #721c24;">Este colaborador no tiene ningún empleado registrado con él como jefe directo.</span>`;
        }
    }
}

async function manejarSubmitAsignar(e) {
    e.preventDefault();
    const idPeriodo = Number(document.getElementById("asignarPeriodo")?.value);
    const tipoEvaluador = Number(document.getElementById("asignarTipoEvaluador")?.value);
    const idEmpleadoEvaluado = Number(document.getElementById("asignarEmpleadoEvaluado")?.value);
    const idEvaluador = Number(document.getElementById("asignarEvaluador")?.value);

    if (!idPeriodo || !tipoEvaluador || !idEmpleadoEvaluado || !idEvaluador) {
        mostrarMensaje("Debe completar todos los campos requeridos.", "error");
        return;
    }

    try {
        await asignarEvaluacion({
            idPeriodoEvaluacion: idPeriodo,
            tipoEvaluador,
            idEmpleadoEvaluado,
            idEvaluador
        });
        mostrarMensaje("Evaluación 360° asignada exitosamente.");
        cerrarModal("modalAsignar");
        await cargarEvaluaciones();
    } catch (error) {
        console.error(error);
        mostrarMensaje(error.message || "Error al asignar la evaluación.", "error");
    }
}

// ==========================================================================
// MODAL 3: Cuestionario / Responder Evaluación (Checklist 1-5)
// ==========================================================================
async function abrirModalEvaluar(idEvaluacion) {
    try {
        evaluacionEnCurso = await obtenerEvaluacionPorId(idEvaluacion);
        if (!evaluacionEnCurso) {
            mostrarMensaje("No se pudo cargar la evaluación.", "error");
            return;
        }

        document.getElementById("idEvaluacionCuestionario").value = evaluacionEnCurso.idEvaluacionDesempeno;
        document.getElementById("subtituloModalEvaluar").innerHTML = `
            Colaborador: <strong>${escapeHtml(evaluacionEnCurso.nombreCompletoEvaluado)}</strong> (${escapeHtml(evaluacionEnCurso.cargoEvaluado)}) &bull; 
            Evaluador: <strong>${escapeHtml(evaluacionEnCurso.nombreCompletoEvaluador)}</strong> &bull; 
            Perspectiva: <strong>${escapeHtml(evaluacionEnCurso.tipoEvaluadorTexto)}</strong>
        `;

        document.getElementById("observacionesGenerales").value = evaluacionEnCurso.observacionesGenerales || "";
        const esCompletada = (evaluacionEnCurso.estado === 3);

        const btnGuardarBorrador = document.getElementById("btnGuardarBorrador");
        const btnFinalizar = document.getElementById("btnFinalizarEvaluacion");
        if (btnGuardarBorrador) btnGuardarBorrador.style.display = esCompletada ? "none" : "inline-flex";
        if (btnFinalizar) btnFinalizar.style.display = esCompletada ? "none" : "inline-flex";

        renderizarCuestionarioForm(checklistGlobal, evaluacionEnCurso.detalles || [], esCompletada);
        recalcularPuntajeEstimadoCuestionario();

        abrirModal("modalEvaluar");
    } catch (error) {
        console.error(error);
        mostrarMensaje("Error al abrir cuestionario de evaluación.", "error");
    }
}

function renderizarCuestionarioForm(categorias, detallesGuardados, soloLectura) {
    const contenedor = document.getElementById("contenedorCuestionarioCategorias");
    if (!contenedor) return;
    contenedor.innerHTML = "";

    const mapaRespuestas = new Map();
    detallesGuardados.forEach(d => {
        mapaRespuestas.set(d.idCriterioEvaluacion, { puntuacion: d.puntuacion, comentario: d.comentario });
    });

    categorias.forEach(cat => {
        const bloque = document.createElement("div");
        bloque.className = "cuestionario-categoria-bloque";

        let preguntasHtml = "";
        (cat.criterios || []).forEach(cr => {
            const guardado = mapaRespuestas.get(cr.idCriterioEvaluacion);
            const puntuacionActual = guardado ? guardado.puntuacion : null;
            const comentarioActual = guardado ? guardado.comentario || "" : "";

            const escalaOpciones = [1, 2, 3, 4, 5].map(val => {
                const checked = (puntuacionActual === val) ? "checked" : "";
                const selectedClass = (puntuacionActual === val) ? "opcion-escala-btn--seleccionada" : "";
                const disabled = soloLectura ? "disabled" : "";

                return `
                    <label class="opcion-escala-btn ${selectedClass}" data-criterio="${cr.idCriterioEvaluacion}" data-valor="${val}">
                        <input type="radio" name="puntuacion_${cr.idCriterioEvaluacion}" value="${val}" ${checked} ${disabled}>
                        <span>${val}</span>
                    </label>
                `;
            }).join("");

            preguntasHtml += `
                <div class="cuestionario-pregunta" data-criterio-id="${cr.idCriterioEvaluacion}" data-categoria-id="${cat.idCategoriaEvaluacion}" data-peso="${cat.ponderacion}">
                    <div class="cuestionario-pregunta__enunciado">${escapeHtml(cr.texto)}</div>
                    ${cr.descripcion ? `<div class="cuestionario-pregunta__guia">${escapeHtml(cr.descripcion)}</div>` : ""}
                    
                    <div class="cuestionario-opciones-escala">
                        ${escalaOpciones}
                    </div>

                    <div class="cuestionario-pregunta__comentario">
                        <input type="text" placeholder="Comentario u observación sobre este criterio (opcional)..." value="${escapeHtml(comentarioActual)}" class="input-comentario-criterio" ${soloLectura ? "disabled" : ""}>
                    </div>
                </div>
            `;
        });

        bloque.innerHTML = `
            <div class="cuestionario-categoria-bloque__header">
                <h4>${escapeHtml(cat.nombre)}</h4>
                <span class="cuestionario-categoria-bloque__peso">Ponderación: ${cat.ponderacion.toFixed(1)}%</span>
            </div>
            <div class="cuestionario-categoria-bloque__preguntas">
                ${preguntasHtml}
            </div>
        `;
        contenedor.appendChild(bloque);
    });

    // Event listeners para selección de escala y recálculo
    if (!soloLectura) {
        contenedor.querySelectorAll(".opcion-escala-btn").forEach(btn => {
            btn.addEventListener("click", (e) => {
                const idCriterio = btn.dataset.criterio;
                const valor = Number(btn.dataset.valor);
                const radio = btn.querySelector('input[type="radio"]');
                if (radio) radio.checked = true;

                // Actualizar clases seleccionadas en este grupo
                const grupo = contenedor.querySelectorAll(`.opcion-escala-btn[data-criterio="${idCriterio}"]`);
                grupo.forEach(g => g.classList.toggle("opcion-escala-btn--seleccionada", Number(g.dataset.valor) === valor));

                recalcularPuntajeEstimadoCuestionario();
            });
        });
    }
}

function obtenerRespuestasDelFormulario() {
    const preguntas = document.querySelectorAll(".cuestionario-pregunta");
    const respuestas = [];

    preguntas.forEach(p => {
        const idCriterio = Number(p.dataset.criterioId);
        const radio = p.querySelector('input[type="radio"]:checked');
        const comentario = p.querySelector(".input-comentario-criterio")?.value.trim();

        if (radio) {
            respuestas.push({
                idCriterioEvaluacion: idCriterio,
                puntuacion: Number(radio.value),
                comentario: comentario || null
            });
        }
    });

    return respuestas;
}

function recalcularPuntajeEstimadoCuestionario() {
    let puntajeTotal = 0;

    checklistGlobal.forEach(cat => {
        const preguntasDeCat = document.querySelectorAll(`.cuestionario-pregunta[data-categoria-id="${cat.idCategoriaEvaluacion}"]`);
        if (preguntasDeCat.length === 0) return;

        let sumaPuntos = 0;
        let contestadas = 0;

        preguntasDeCat.forEach(p => {
            const radio = p.querySelector('input[type="radio"]:checked');
            if (radio) {
                sumaPuntos += Number(radio.value);
                contestadas++;
            }
        });

        if (contestadas > 0) {
            const promedioCat = sumaPuntos / contestadas;
            const ponderadoCat = (promedioCat / 5.0) * cat.ponderacion;
            puntajeTotal += ponderadoCat;
        }
    });

    const textoElem = document.getElementById("puntajeEstimadoTexto");
    const badgeElem = document.getElementById("clasificacionEstimadaBadge");

    if (textoElem && badgeElem) {
        textoElem.textContent = puntajeTotal.toFixed(1) + " / 100";

        let clasif = "Necesita mejorar";
        let badgeClass = "clasificacion--necesita-mejorar";

        if (puntajeTotal >= 90) { clasif = "Excelente"; badgeClass = "clasificacion--excelente"; }
        else if (puntajeTotal >= 80) { clasif = "Muy bueno"; badgeClass = "clasificacion--muy-bueno"; }
        else if (puntajeTotal >= 70) { clasif = "Bueno"; badgeClass = "clasificacion--bueno"; }
        else if (puntajeTotal >= 60) { clasif = "Aceptable"; badgeClass = "clasificacion--aceptable"; }

        badgeElem.textContent = clasif;
        badgeElem.className = `clasificacion-badge ${badgeClass}`;
    }
}

async function manejarGuardarBorrador() {
    const id = Number(document.getElementById("idEvaluacionCuestionario")?.value);
    const observacionesGenerales = document.getElementById("observacionesGenerales")?.value.trim();
    const respuestas = obtenerRespuestasDelFormulario();

    try {
        await guardarRespuestasEvaluacion(id, {
            idEvaluacionDesempeno: id,
            observacionesGenerales,
            respuestas
        });
        mostrarMensaje("Borrador guardado correctamente.");
        await cargarEvaluaciones();
    } catch (error) {
        console.error(error);
        mostrarMensaje(error.message || "Error al guardar el borrador.", "error");
    }
}

async function manejarSubmitCompletarEvaluacion(e) {
    e.preventDefault();
    const id = Number(document.getElementById("idEvaluacionCuestionario")?.value);
    const observacionesGenerales = document.getElementById("observacionesGenerales")?.value.trim();
    const respuestas = obtenerRespuestasDelFormulario();

    try {
        const resultado = await completarEvaluacion(id, {
            idEvaluacionDesempeno: id,
            observacionesGenerales,
            respuestas
        });

        mostrarMensaje(`Evaluación finalizada con éxito. Puntaje obtenido: ${resultado.puntajeFinal.toFixed(1)} / 100 (${resultado.clasificacion}).`);
        cerrarModal("modalEvaluar");
        await cargarEvaluaciones();
    } catch (error) {
        console.error(error);
        mostrarMensaje(error.message || "No fue posible finalizar la evaluación.", "error");
    }
}

// ==========================================================================
// MODAL 4: Reporte Consolidado 360°
// ==========================================================================
async function abrirModalConsolidado(idEmpleado, idPeriodo) {
    const cuerpo = document.getElementById("cuerpoConsolidado360");
    const subtitulo = document.getElementById("subtituloConsolidado");

    if (!cuerpo) return;
    cuerpo.innerHTML = '<p style="text-align: center; padding: 20px;">Cargando reporte consolidado...</p>';
    abrirModal("modalConsolidado");

    try {
        const datos = await obtenerConsolidado360(idEmpleado, idPeriodo);
        if (!datos) {
            cuerpo.innerHTML = '<p class="estado-tabla">No hay datos consolidados para este período.</p>';
            return;
        }

        subtitulo.innerHTML = `Colaborador: <strong>${escapeHtml(datos.nombreCompleto)}</strong> (${escapeHtml(datos.cargo)}) &bull; Período: <strong>${escapeHtml(datos.nombrePeriodo)}</strong>`;

        let clasifClass = "clasificacion--necesita-mejorar";
        if (datos.clasificacion === "Excelente") clasifClass = "clasificacion--excelente";
        else if (datos.clasificacion === "Muy bueno") clasifClass = "clasificacion--muy-bueno";
        else if (datos.clasificacion === "Bueno") clasifClass = "clasificacion--bueno";
        else if (datos.clasificacion === "Aceptable") clasifClass = "clasificacion--aceptable";

        const categoriasFilas = (datos.categoriasConsolidadas || []).map(cat => `
            <tr>
                <td><strong>${escapeHtml(cat.nombreCategoria)}</strong></td>
                <td>${cat.ponderacion.toFixed(1)}%</td>
                <td><strong>${cat.promedioRespuestas.toFixed(2)} / 5.0</strong></td>
                <td>${cat.rendimientoPorcentaje.toFixed(1)}%</td>
                <td style="color: #1e3f5f;"><strong>${cat.puntosObtenidos.toFixed(2)} pts</strong></td>
            </tr>
        `).join("");

        const evalIndividuales = (datos.evaluacionesIndividuales || []).map(ev => `
            <div style="padding: 10px 14px; background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 4px; margin-bottom: 8px;">
                <div style="display: flex; justify-content: space-between; margin-bottom: 4px;">
                    <strong>${escapeHtml(ev.tipoEvaluadorTexto)}: ${escapeHtml(ev.nombreCompletoEvaluador)}</strong>
                    <span>Puntaje: <strong>${ev.puntajeFinal != null ? ev.puntajeFinal.toFixed(1) + " / 100" : "Pendiente"}</strong></span>
                </div>
                ${ev.observacionesGenerales ? `<p style="font-size: 12px; color: #475569; margin: 0;"><em>"${escapeHtml(ev.observacionesGenerales)}"</em></p>` : ""}
            </div>
        `).join("");

        cuerpo.innerHTML = `
            <div class="resumen-consolidado-grid">
                <div class="tarjeta-perspectiva">
                    <h5>Autoevaluación</h5>
                    <div class="valor-perspectiva">${datos.promedioAutoevaluacion != null ? datos.promedioAutoevaluacion.toFixed(1) : "-"}</div>
                    <span class="badge-perspectiva-estado ${datos.promedioAutoevaluacion != null ? "estado--completada" : "estado--pendiente"}">
                        ${datos.promedioAutoevaluacion != null ? "Completada" : "Pendiente"}
                    </span>
                </div>

                <div class="tarjeta-perspectiva">
                    <h5>Jefe Directo</h5>
                    <div class="valor-perspectiva">${datos.promedioJefeDirecto != null ? datos.promedioJefeDirecto.toFixed(1) : "-"}</div>
                    <span class="badge-perspectiva-estado ${datos.promedioJefeDirecto != null ? "estado--completada" : "estado--pendiente"}">
                        ${datos.promedioJefeDirecto != null ? "Completada" : "Pendiente"}
                    </span>
                </div>

                <div class="tarjeta-perspectiva">
                    <h5>Subordinados</h5>
                    <div class="valor-perspectiva">${datos.promedioSubordinados != null ? datos.promedioSubordinados.toFixed(1) : "-"}</div>
                    <span class="badge-perspectiva-estado ${datos.promedioSubordinados != null ? "estado--completada" : "estado--pendiente"}">
                        ${datos.promedioSubordinados != null ? "Promedio calculado" : "Sin evaluaciones"}
                    </span>
                </div>

                <div class="tarjeta-perspectiva tarjeta-perspectiva--destacada">
                    <h5>Puntaje Consolidado 360°</h5>
                    <div class="valor-perspectiva">${datos.puntajeConsolidado != null ? datos.puntajeConsolidado.toFixed(1) + " / 100" : "-"}</div>
                    <span class="clasificacion-badge ${clasifClass}">${escapeHtml(datos.clasificacion)}</span>
                </div>
            </div>

            <h4 style="color: #1e3f5f; margin-bottom: 10px;">Desglose Consolidado por Categoría</h4>
            <div class="tabla-contenedor" style="margin-bottom: 22px;">
                <table class="tabla-principal">
                    <thead>
                        <tr>
                            <th>Categoría</th>
                            <th>Ponderación</th>
                            <th>Promedio (1-5)</th>
                            <th>Rendimiento %</th>
                            <th>Puntos Aportados</th>
                        </tr>
                    </thead>
                    <tbody>${categoriasFilas}</tbody>
                </table>
            </div>

            <h4 style="color: #1e3f5f; margin-bottom: 10px;">Evaluaciones y Retroalimentación Individual</h4>
            <div>${evalIndividuales || "<p>No hay evaluaciones registradas.</p>"}</div>
        `;
    } catch (error) {
        console.error(error);
        cuerpo.innerHTML = `<p class="mensaje mensaje--error">Error al cargar el consolidado: ${escapeHtml(error.message)}</p>`;
    }
}

// ==========================================================================
// MODAL 5: Historial Semestral del Empleado
// ==========================================================================
async function abrirModalHistorial(idEmpleado) {
    const subtitulo = document.getElementById("subtituloHistorial");
    const tbody = document.getElementById("tablaHistorialBody");
    const sinResultados = document.getElementById("sinHistorial");
    const contenedor = document.getElementById("contenedorTablaHistorial");

    if (!tbody) return;
    tbody.innerHTML = "";
    abrirModal("modalHistorial");

    try {
        const datos = await obtenerHistorialEmpleado(idEmpleado);
        if (!datos) return;

        subtitulo.innerHTML = `Colaborador: <strong>${escapeHtml(datos.nombreCompleto)}</strong> (${escapeHtml(datos.cargo)})`;

        if (!datos.periodos || datos.periodos.length === 0) {
            contenedor?.classList.add("tabla-contenedor--oculto");
            sinResultados?.classList.remove("estado-tabla--oculto");
            return;
        }

        contenedor?.classList.remove("tabla-contenedor--oculto");
        sinResultados?.classList.add("estado-tabla--oculto");

        datos.periodos.forEach(p => {
            const tr = document.createElement("tr");

            let clasifClass = "clasificacion--necesita-mejorar";
            if (p.clasificacion === "Excelente") clasifClass = "clasificacion--excelente";
            else if (p.clasificacion === "Muy bueno") clasifClass = "clasificacion--muy-bueno";
            else if (p.clasificacion === "Bueno") clasifClass = "clasificacion--bueno";
            else if (p.clasificacion === "Aceptable") clasifClass = "clasificacion--aceptable";

            tr.innerHTML = `
                <td><strong>${escapeHtml(p.nombrePeriodo)}</strong></td>
                <td>${p.fechaInicio} al ${p.fechaFin}</td>
                <td><span class="puntaje-numero">${p.puntajeConsolidado != null ? p.puntajeConsolidado.toFixed(1) + " / 100" : "-"}</span></td>
                <td>${p.clasificacion && p.clasificacion !== "-" ? `<span class="clasificacion-badge ${clasifClass}">${escapeHtml(p.clasificacion)}</span>` : "-"}</td>
                <td><strong>${p.evaluacionesRealizadas}</strong> completadas</td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error(error);
        mostrarMensaje("Error al obtener historial del empleado.", "error");
    }
}
