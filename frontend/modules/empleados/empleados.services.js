import {
    apiGet,
    apiPost,
    apiPut,
    apiPatch
} from "../../shared/js/api.js";

const RUTA_EMPLEADOS = "/empleados";
const RUTA_CARGOS = "/cargos";

export async function obtenerEmpleados() {
    const respuesta =
        await apiGet(RUTA_EMPLEADOS);

    return normalizarLista(
        respuesta,
        normalizarEmpleado
    );
}

export async function obtenerEmpleado(idEmpleado) {
    const respuesta =
        await apiGet(
            `${RUTA_EMPLEADOS}/${idEmpleado}`
        );

    return normalizarEmpleado(respuesta);
}

export async function obtenerCargosActivos() {
    const respuesta =
        await apiGet(RUTA_CARGOS);

    const cargos =
        normalizarLista(
            respuesta,
            normalizarCargo
        );

    return cargos.filter(
        cargo => cargo.activo
    );
}

export async function crearEmpleado(datos) {
    return apiPost(
        RUTA_EMPLEADOS,
        {
            idCargo: datos.idCargo,
            codigoEmpleado: datos.codigoEmpleado,
            pin: datos.pin,
            identificacion: datos.identificacion,
            nombres: datos.nombres,
            apellidos: datos.apellidos,
            telefono: datos.telefono,
            correo: datos.correo,
            direccion: datos.direccion,
            fechaContratacion: datos.fechaContratacion,
            salarioBase: datos.salarioBase
        }
    );
}

export async function actualizarEmpleado(
    idEmpleado,
    datos
) {
    return apiPut(
        `${RUTA_EMPLEADOS}/${idEmpleado}`,
        {
            idCargo: datos.idCargo,
            codigoEmpleado: datos.codigoEmpleado,
            identificacion: datos.identificacion,
            nombres: datos.nombres,
            apellidos: datos.apellidos,
            telefono: datos.telefono,
            correo: datos.correo,
            direccion: datos.direccion,
            fechaContratacion: datos.fechaContratacion,
            salarioBase: datos.salarioBase
        }
    );
}

export async function cambiarEstadoEmpleado(
    idEmpleado,
    activo
) {
    return apiPatch(
        `${RUTA_EMPLEADOS}/${idEmpleado}/estado`,
        {
            activo
        }
    );
}

export async function cambiarPinEmpleado(
    idEmpleado,
    nuevoPin
) {
    return apiPatch(
        `${RUTA_EMPLEADOS}/${idEmpleado}/pin`,
        {
            nuevoPin
        }
    );
}

function normalizarLista(
    respuesta,
    funcionNormalizar
) {
    if (!respuesta) {
        return [];
    }

    let registros;

    if (Array.isArray(respuesta)) {
        registros = respuesta;
    } else {
        registros =
            respuesta.items ??
            respuesta.Items ??
            respuesta.datos ??
            respuesta.Datos ??
            respuesta.registros ??
            respuesta.Registros ??
            respuesta.resultados ??
            respuesta.Resultados ??
            [];
    }

    if (!Array.isArray(registros)) {
        return [];
    }

    return registros.map(funcionNormalizar);
}

function normalizarEmpleado(empleado) {
    if (!empleado) {
        return null;
    }

    return {
        idEmpleado:
            empleado.idEmpleado ??
            empleado.IdEmpleado,

        idCargo:
            empleado.idCargo ??
            empleado.IdCargo,

        nombreCargo:
            empleado.nombreCargo ??
            empleado.NombreCargo ??
            "Sin cargo",

        codigoEmpleado:
            empleado.codigoEmpleado ??
            empleado.CodigoEmpleado ??
            "",

        identificacion:
            empleado.identificacion ??
            empleado.Identificacion ??
            "",

        nombres:
            empleado.nombres ??
            empleado.Nombres ??
            "",

        apellidos:
            empleado.apellidos ??
            empleado.Apellidos ??
            "",

        telefono:
            empleado.telefono ??
            empleado.Telefono ??
            "",

        correo:
            empleado.correo ??
            empleado.Correo ??
            "",

        direccion:
            empleado.direccion ??
            empleado.Direccion ??
            "",

        fechaContratacion:
            empleado.fechaContratacion ??
            empleado.FechaContratacion ??
            "",

        salarioBase:
            Number(
                empleado.salarioBase ??
                empleado.SalarioBase ??
                0
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

function normalizarCargo(cargo) {
    return {
        idCargo:
            cargo.idCargo ??
            cargo.IdCargo,

        nombre:
            cargo.nombre ??
            cargo.Nombre ??
            "",

        activo:
            convertirActivo(
                cargo.activo ??
                cargo.Activo ??
                cargo.estado ??
                cargo.Estado
            )
    };
}

function convertirActivo(valor) {
    if (typeof valor === "boolean") {
        return valor;
    }

    if (typeof valor === "number") {
        return valor === 1;
    }

    if (typeof valor === "string") {
        const estado =
            valor.trim().toLowerCase();

        return (
            estado === "activo" ||
            estado === "true" ||
            estado === "1"
        );
    }

    return true;
}