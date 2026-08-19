import {
    apiGet,
    apiPost,
    apiPatch
} from "../../shared/js/api.js";

const RUTA_PLANILLAS = "/Planillas";
const RUTA_EMPLEADOS = "/Empleados";
const RUTA_DEPARTAMENTOS = "/Departamentos";

export async function obtenerPlanillas() {
    const respuesta = await apiGet(RUTA_PLANILLAS);
    return normalizarListaPlanillas(respuesta);
}

export async function obtenerPlanillaPorId(idPlanilla) {
    const respuesta = await apiGet(`${RUTA_PLANILLAS}/${idPlanilla}`);
    return normalizarPlanilla(respuesta);
}

export async function obtenerPlanillasPorEmpleado(codigoEmpleado) {
    const respuesta = await apiGet(
        `${RUTA_PLANILLAS}/empleado/${encodeURIComponent(codigoEmpleado)}`
    );
    return normalizarListaPlanillas(respuesta);
}

export async function generarPlanilla(datos) {
    const payload = {
        idAdministrador: datos.idAdministrador ?? 0,
        idDepartamento: datos.idDepartamento,
        fechaInicioPeriodo: datos.fechaInicioPeriodo,
        fechaFinPeriodo: datos.fechaFinPeriodo,
        idsEmpleadosSeleccionados: datos.idsEmpleadosSeleccionados ?? []
    };

    return apiPost(RUTA_PLANILLAS, payload);
}

export async function cambiarEstadoPlanilla(idPlanilla, estado) {
    return apiPatch(`${RUTA_PLANILLAS}/${idPlanilla}/estado`, {
        estado
    });
}

export async function obtenerEmpleadosParaPlanilla() {
    const respuesta = await apiGet(RUTA_EMPLEADOS);
    return normalizarListaEmpleados(respuesta);
}

export async function obtenerEmpleadosPorDepartamento(idDepartamento) {
    const respuesta = await apiGet(
        `${RUTA_DEPARTAMENTOS}/${idDepartamento}/empleados?soloActivos=true`
    );
    return normalizarListaEmpleados(respuesta);
}

export async function obtenerDepartamentos() {
    const respuesta = await apiGet(RUTA_DEPARTAMENTOS);
    const lista = extraerLista(respuesta);
    return lista.map(d => ({
        idDepartamento: d.idDepartamento ?? d.IdDepartamento ?? d.id ?? d.Id ?? 0,
        nombre: d.nombre ?? d.Nombre ?? d.nombreDepartamento ?? d.NombreDepartamento ?? "",
        descripcion: d.descripcion ?? d.Descripcion ?? "",
        activo: convertirActivo(d.activo ?? d.Activo)
    })).filter(d => d.activo);
}

export async function recalcularPlanilla(idPlanilla, idAdministrador) {
    return apiPost(`${RUTA_PLANILLAS}/${idPlanilla}/recalcular`, {
        idAdministrador: idAdministrador ?? 0
    });
}

export async function enviarPlanillaRevision(idPlanilla, idAdministrador) {
    return apiPost(`${RUTA_PLANILLAS}/${idPlanilla}/enviar-revision`, {
        idAdministrador: idAdministrador ?? 0
    });
}

export async function cerrarPlanilla(idPlanilla, idAdministrador) {
    return apiPost(`${RUTA_PLANILLAS}/${idPlanilla}/cerrar`, {
        idAdministrador: idAdministrador ?? 0
    });
}

export async function pagarPlanilla(idPlanilla, idAdministrador) {
    return apiPost(`${RUTA_PLANILLAS}/${idPlanilla}/pagar`, {
        idAdministrador: idAdministrador ?? 0
    });
}

export async function anularPlanilla(idPlanilla, idAdministrador, motivo = "") {
    return apiPost(`${RUTA_PLANILLAS}/${idPlanilla}/anular`, {
        idAdministrador: idAdministrador ?? 0,
        motivo
    });
}

function extraerLista(respuesta) {
    if (Array.isArray(respuesta)) {
        return respuesta;
    }

    if (!respuesta) {
        return [];
    }

    const lista =
        respuesta.items ??
        respuesta.Items ??
        respuesta.datos ??
        respuesta.Datos ??
        respuesta.registros ??
        respuesta.Registros ??
        respuesta.resultados ??
        respuesta.Resultados ??
        [];

    return Array.isArray(lista) ? lista : [];
}

export function normalizarListaPlanillas(respuesta) {
    return extraerLista(respuesta)
        .map(normalizarPlanilla)
        .filter(Boolean);
}

export function normalizarPlanilla(planilla) {
    if (!planilla) {
        return null;
    }

    const salarioBase = numero(
        planilla.totalSalarioBase ??
        planilla.TotalSalarioBase ??
        planilla.salarioBasePeriodo ??
        planilla.SalarioBasePeriodo
    );

    const ingresos = numero(
        planilla.totalIngresos ??
        planilla.TotalIngresos ??
        planilla.totalHorasExtras ??
        planilla.TotalHorasExtras ??
        planilla.ingresosAdicionales ??
        planilla.IngresosAdicionales
    );

    const deducciones = numero(
        planilla.totalDeducciones ??
        planilla.TotalDeducciones
    );

    const salarioNeto = numero(
        planilla.totalNeto ??
        planilla.TotalNeto ??
        planilla.salarioNeto ??
        planilla.SalarioNeto
    );

    const detalles = (planilla.detalles ?? planilla.Detalles ?? [])
        .map(normalizarDetallePlanilla)
        .filter(Boolean);

    return {
        idPlanilla:
            planilla.idPlanilla ??
            planilla.IdPlanilla ??
            planilla.id ??
            planilla.Id ??
            0,

        idDepartamento:
            planilla.idDepartamento ??
            planilla.IdDepartamento ??
            null,

        nombreDepartamento:
            planilla.nombreDepartamento ??
            planilla.NombreDepartamento ??
            "",

        idEmpleado:
            planilla.idEmpleado ??
            planilla.IdEmpleado ??
            null,

        codigoEmpleado:
            planilla.codigoEmpleado ??
            planilla.CodigoEmpleado ??
            "",

        nombreEmpleado:
            planilla.nombreEmpleado ??
            planilla.NombreEmpleado ??
            "",

        idAdministrador:
            planilla.idAdministrador ??
            planilla.IdAdministrador ??
            0,

        nombreAdministrador:
            planilla.nombreAdministrador ??
            planilla.NombreAdministrador ??
            "",

        cantidadEmpleados: numero(
            planilla.cantidadEmpleados ??
            planilla.CantidadEmpleados ??
            detalles.length
        ),

        fechaInicioPeriodo: fecha(
            planilla.fechaInicioPeriodo ??
            planilla.FechaInicioPeriodo
        ),

        fechaFinPeriodo: fecha(
            planilla.fechaFinPeriodo ??
            planilla.FechaFinPeriodo
        ),

        salarioBasePeriodo: salarioBase,
        ingresosAdicionales: ingresos,
        totalDeducciones: deducciones,
        salarioNeto: salarioNeto,

        estado: numero(
            planilla.estado ??
            planilla.Estado ??
            1
        ),

        fechaGeneracion:
            planilla.fechaGeneracion ??
            planilla.FechaGeneracion ??
            null,

        fechaCierre:
            planilla.fechaCierre ??
            planilla.FechaCierre ??
            null,

        fechaCreacion:
            planilla.fechaCreacion ??
            planilla.FechaCreacion ??
            null,

        fechaActualizacion:
            planilla.fechaActualizacion ??
            planilla.FechaActualizacion ??
            null,

        detalles
    };
}

export function normalizarDetallePlanilla(detalle) {
    if (!detalle) {
        return null;
    }

    return {
        idDetallePlanilla:
            detalle.idDetallePlanilla ??
            detalle.IdDetallePlanilla ??
            0,

        idEmpleado:
            detalle.idEmpleado ??
            detalle.IdEmpleado ??
            0,

        codigoEmpleado:
            detalle.codigoEmpleado ??
            detalle.CodigoEmpleado ??
            "",

        nombreEmpleado:
            detalle.nombreEmpleado ??
            detalle.NombreEmpleado ??
            "",

        numeroINSS:
            detalle.numeroINSS ??
            detalle.NumeroINSS ??
            "",

        cargo:
            detalle.cargo ??
            detalle.Cargo ??
            "",

        departamento:
            detalle.departamento ??
            detalle.Departamento ??
            "",

        salarioBase: numero(
            detalle.salarioBase ??
            detalle.SalarioBase
        ),

        diasLaborados: numero(
            detalle.diasLaborados ??
            detalle.DiasLaborados
        ),

        minutosLaborados: numero(
            detalle.minutosLaborados ??
            detalle.MinutosLaborados
        ),

        cantidadTardanzas: numero(
            detalle.cantidadTardanzas ??
            detalle.CantidadTardanzas
        ),

        minutosTardanza: numero(
            detalle.minutosTardanza ??
            detalle.MinutosTardanza
        ),

        descuentoTardanza: numero(
            detalle.descuentoTardanza ??
            detalle.DescuentoTardanza
        ),

        minutosExtrasDetectados: numero(
            detalle.minutosExtrasDetectados ??
            detalle.MinutosExtrasDetectados
        ),

        minutosExtrasAprobados: numero(
            detalle.minutosExtrasAprobados ??
            detalle.MinutosExtrasAprobados
        ),

        montoHorasExtras: numero(
            detalle.montoHorasExtras ??
            detalle.MontoHorasExtras
        ),

        vacacionesAcumuladasPeriodo: numero(
            detalle.vacacionesAcumuladasPeriodo ??
            detalle.VacacionesAcumuladasPeriodo
        ),

        saldoVacaciones: numero(
            detalle.saldoVacaciones ??
            detalle.SaldoVacaciones
        ),

        totalIngresos: numero(
            detalle.totalIngresos ??
            detalle.TotalIngresos
        ),

        totalDeducciones: numero(
            detalle.totalDeducciones ??
            detalle.TotalDeducciones
        ),

        salarioNeto: numero(
            detalle.salarioNeto ??
            detalle.SalarioNeto
        ),

        indemnizacionProyectada: numero(
            detalle.indemnizacionProyectada ??
            detalle.IndemnizacionProyectada
        )
    };
}

export function normalizarListaEmpleados(respuesta) {
    return extraerLista(respuesta)
        .map(normalizarEmpleado)
        .filter(Boolean)
        .filter(empleado => empleado.activo);
}

export function normalizarEmpleado(empleado) {
    if (!empleado) {
        return null;
    }

    const nombres =
        empleado.nombres ??
        empleado.Nombres ??
        "";

    const apellidos =
        empleado.apellidos ??
        empleado.Apellidos ??
        "";

    return {
        idEmpleado:
            empleado.idEmpleado ??
            empleado.IdEmpleado ??
            empleado.id ??
            empleado.Id ??
            0,

        idDepartamento:
            empleado.idDepartamento ??
            empleado.IdDepartamento ??
            null,

        nombreDepartamento:
            empleado.nombreDepartamento ??
            empleado.NombreDepartamento ??
            "",

        nombreCargo:
            empleado.nombreCargo ??
            empleado.NombreCargo ??
            empleado.cargo?.nombre ??
            "",

        codigoEmpleado:
            empleado.codigoEmpleado ??
            empleado.CodigoEmpleado ??
            "",

        nombreCompleto:
            empleado.nombreCompleto ??
            empleado.NombreCompleto ??
            `${nombres} ${apellidos}`.trim(),

        salarioBase: numero(
            empleado.salarioBase ??
            empleado.SalarioBase
        ),

        fechaContratacion: fecha(
            empleado.fechaContratacion ??
            empleado.FechaContratacion
        ),

        activo: convertirActivo(
            empleado.activo ??
            empleado.Activo ??
            empleado.estado ??
            empleado.Estado
        )
    };
}

function fecha(valor) {
    return valor ? String(valor).slice(0, 10) : "";
}

function numero(valor) {
    const resultado = Number(valor);
    return Number.isFinite(resultado) ? resultado : 0;
}

function convertirActivo(valor) {
    if (valor === undefined || valor === null) {
        return true;
    }

    if (typeof valor === "boolean") {
        return valor;
    }

    if (typeof valor === "number") {
        return valor === 1;
    }

    const texto = String(valor).trim().toLowerCase();
    return (
        texto === "activo" ||
        texto === "activa" ||
        texto === "true" ||
        texto === "1"
    );
}