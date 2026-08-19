import {
    apiGet,
    apiPost,
    apiPatch
} from "../../shared/js/api.js";

const RUTA_PLANILLAS =
    "/Planillas";

const RUTA_EMPLEADOS =
    "/Empleados";

const RUTA_TIPOS_DEDUCCION =
    "/tipos-deduccion";

const RUTA_DEPARTAMENTOS = "/Departamentos";


export async function obtenerPlanillas() {
    const respuesta =
        await apiGet(
            RUTA_PLANILLAS
        );

    return normalizarListaPlanillas(
        respuesta
    );
}

export async function obtenerPlanillaPorId(
    idPlanilla
) {
    const respuesta =
        await apiGet(
            `${RUTA_PLANILLAS}/${idPlanilla}`
        );

    return normalizarPlanilla(
        respuesta
    );
}

export async function obtenerPlanillasPorEmpleado(
    codigoEmpleado
) {
    const respuesta =
        await apiGet(
            `${RUTA_PLANILLAS}/empleado/${encodeURIComponent(
                codigoEmpleado
            )}`
        );

    return normalizarListaPlanillas(
        respuesta
    );
}

export async function generarPlanilla(
    datos
) {
    const payload = {
        idAdministrador: datos.idAdministrador ?? 0,
        idDepartamento: datos.idDepartamento,
        fechaInicioPeriodo: datos.fechaInicioPeriodo,
        fechaFinPeriodo: datos.fechaFinPeriodo
    };

    return apiPost(
        RUTA_PLANILLAS,
        payload
    );
}

export async function cambiarEstadoPlanilla(
    idPlanilla,
    estado
) {
    return apiPatch(
        `${RUTA_PLANILLAS}/${idPlanilla}/estado`,
        {
            estado
        }
    );
}

export async function obtenerEmpleadosParaPlanilla() {
    const respuesta =
        await apiGet(
            RUTA_EMPLEADOS
        );

    return normalizarListaEmpleados(
        respuesta
    );
}


export async function obtenerTiposDeduccionParaPlanilla() {
    const respuesta =
        await apiGet(
            RUTA_TIPOS_DEDUCCION
        );

    return normalizarListaTiposDeduccion(
        respuesta
    );
}

export async function obtenerDepartamentos() {
    const respuesta = await apiGet(RUTA_DEPARTAMENTOS);
    if (!Array.isArray(respuesta)) {
        return (respuesta.items || respuesta.datos || respuesta.registros || []).map(d => ({
            idDepartamento: d.idDepartamento ?? d.IdDepartamento ?? d.id,
            nombre: d.nombre ?? d.Nombre ?? d.nombreDepartamento ?? d.NombreDepartamento ?? ""
        }));
    }

    return respuesta.map(d => ({
        idDepartamento: d.idDepartamento ?? d.IdDepartamento ?? d.id,
        nombre: d.nombre ?? d.Nombre ?? d.nombreDepartamento ?? d.NombreDepartamento ?? ""
    }));
}

export async function recalcularPlanilla(idPlanilla, idAdministrador) {
    return apiPost(`${RUTA_PLANILLAS}/${idPlanilla}/recalcular`, { idAdministrador: idAdministrador ?? 0 });
}

export async function enviarPlanillaRevision(idPlanilla, idAdministrador) {
    return apiPost(`${RUTA_PLANILLAS}/${idPlanilla}/enviar-revision`, { idAdministrador: idAdministrador ?? 0 });
}

export async function cerrarPlanilla(idPlanilla, idAdministrador) {
    return apiPost(`${RUTA_PLANILLAS}/${idPlanilla}/cerrar`, { idAdministrador: idAdministrador ?? 0 });
}

export async function pagarPlanilla(idPlanilla, idAdministrador) {
    return apiPost(`${RUTA_PLANILLAS}/${idPlanilla}/pagar`, { idAdministrador: idAdministrador ?? 0 });
}

export async function anularPlanilla(idPlanilla, idAdministrador, motivo = "") {
    return apiPost(`${RUTA_PLANILLAS}/${idPlanilla}/anular`, {
        idAdministrador: idAdministrador ?? 0,
        motivo
    });
}


function extraerLista(
    respuesta
) {
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

    return Array.isArray(lista)
        ? lista
        : [];
}


function normalizarListaPlanillas(
    respuesta
) {
    return extraerLista(respuesta)
        .map(normalizarPlanilla)
        .filter(Boolean);
}

function normalizarPlanilla(
    planilla
) {
    if (!planilla) {
        return null;
    }

    const deducciones =
        planilla.deducciones ??
        planilla.Deducciones ??
        [];

    const salarioBase =
        numero(
            planilla.salarioBasePeriodo ??
            planilla.SalarioBasePeriodo
        );

    const ingresos =
        numero(
            planilla.ingresosAdicionales ??
            planilla.IngresosAdicionales
        );

    return {
        idPlanilla:
            planilla.idPlanilla ??
            planilla.IdPlanilla ??
            planilla.id ??
            planilla.Id,

        idEmpleado:
            planilla.idEmpleado ??
            planilla.IdEmpleado ??
            0,

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

        fechaInicioPeriodo:
            fecha(
                planilla.fechaInicioPeriodo ??
                planilla.FechaInicioPeriodo
            ),

        fechaFinPeriodo:
            fecha(
                planilla.fechaFinPeriodo ??
                planilla.FechaFinPeriodo
            ),

        salarioBasePeriodo:
            salarioBase,

        ingresosAdicionales:
            ingresos,

        salarioBruto:
            numero(
                planilla.salarioBruto ??
                planilla.SalarioBruto ??
                salarioBase + ingresos
            ),

        totalDeducciones:
            numero(
                planilla.totalDeducciones ??
                planilla.TotalDeducciones
            ),

        salarioNeto:
            numero(
                planilla.salarioNeto ??
                planilla.SalarioNeto
            ),

        estado:
            numero(
                planilla.estado ??
                planilla.Estado
            ),

        fechaCreacion:
            planilla.fechaCreacion ??
            planilla.FechaCreacion ??
            null,

        fechaActualizacion:
            planilla.fechaActualizacion ??
            planilla.FechaActualizacion ??
            null,

        deducciones:
            Array.isArray(deducciones)
                ? deducciones
                    .map(normalizarDeduccionPlanilla)
                    .filter(Boolean)
                : []
    };
}

function normalizarDeduccionPlanilla(
    deduccion
) {
    if (!deduccion) {
        return null;
    }

    return {
        idDeduccionPlanilla:
            deduccion.idDeduccionPlanilla ??
            deduccion.IdDeduccionPlanilla ??
            0,

        idTipoDeduccion:
            deduccion.idTipoDeduccion ??
            deduccion.IdTipoDeduccion ??
            0,

        nombreTipoDeduccion:
            deduccion.nombreTipoDeduccion ??
            deduccion.NombreTipoDeduccion ??
            "",

        tipoCalculo:
            deduccion.tipoCalculo ??
            deduccion.TipoCalculo ??
            0,

        valorAplicado:
            numero(
                deduccion.valorAplicado ??
                deduccion.ValorAplicado
            ),

        montoCalculado:
            numero(
                deduccion.montoCalculado ??
                deduccion.MontoCalculado
            ),

        observacion:
            deduccion.observacion ??
            deduccion.Observacion ??
            ""
    };
}

function normalizarListaEmpleados(
    respuesta
) {
    return extraerLista(respuesta)
        .map(normalizarEmpleado)
        .filter(Boolean)
        .filter(empleado => empleado.activo);
}

function normalizarEmpleado(
    empleado
) {
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
            empleado.Id,

        codigoEmpleado:
            empleado.codigoEmpleado ??
            empleado.CodigoEmpleado ??
            "",

        nombreCompleto:
            empleado.nombreCompleto ??
            empleado.NombreCompleto ??
            `${nombres} ${apellidos}`.trim(),

        salarioBase:
            numero(
                empleado.salarioBase ??
                empleado.SalarioBase
            ),

        fechaContratacion:
            fecha(
                empleado.fechaContratacion ??
                empleado.FechaContratacion
            ),

        activo:
            convertirActivo(
                empleado.activo ??
                empleado.Activo ??
                empleado.estado ??
                empleado.Estado
            )
    };
}

function normalizarListaTiposDeduccion(
    respuesta
) {
    return extraerLista(respuesta)
        .map(normalizarTipoDeduccion)
        .filter(Boolean)
        .filter(tipo => tipo.activo);
}

function normalizarTipoDeduccion(
    tipo
) {
    if (!tipo) {
        return null;
    }

    return {
        idTipoDeduccion:
            tipo.idTipoDeduccion ??
            tipo.IdTipoDeduccion ??
            tipo.id ??
            tipo.Id,

        nombre:
            tipo.nombre ??
            tipo.Nombre ??
            "",

        descripcion:
            tipo.descripcion ??
            tipo.Descripcion ??
            "",

        tipoCalculo:
            tipo.tipoCalculo ??
            tipo.TipoCalculo ??
            0,

        valorPredeterminado:
            numero(
                tipo.valorPredeterminado ??
                tipo.ValorPredeterminado
            ),

        activo:
            convertirActivo(
                tipo.activo ??
                tipo.Activo ??
                tipo.estado ??
                tipo.Estado
            )
    };
}
function fecha(
    valor
) {
    return valor
        ? String(valor).slice(0, 10)
        : "";
}

function numero(
    valor
) {
    const resultado =
        Number(valor);

    return Number.isFinite(resultado)
        ? resultado
        : 0;
}

function convertirActivo(
    valor
) {
    if (
        valor === undefined ||
        valor === null
    ) {
        return true;
    }

    if (typeof valor === "boolean") {
        return valor;
    }

    if (typeof valor === "number") {
        return valor === 1;
    }

    const texto =
        String(valor)
            .trim()
            .toLowerCase();

    return (
        texto === "activo" ||
        texto === "activa" ||
        texto === "true" ||
        texto === "1"
    );
}