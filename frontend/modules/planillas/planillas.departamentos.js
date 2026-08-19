import { obtenerDepartamentos } from './planillas.services.js';
import { escaparAtributo, escaparHtml } from '../../shared/js/layout.js';

export async function cargarDepartamentos() {
    try {
        const departamentos = await obtenerDepartamentos();

        departamentos.sort((a, b) => (a.nombre || '').localeCompare(b.nombre || '', 'es'));

        const opciones = departamentos.map(d => `
            <option value="${escaparAtributo(d.idDepartamento)}">
                ${escaparHtml(d.nombre)}
            </option>
        `).join('');

        const filtroEmpleado = document.getElementById('filtroEmpleado');
        if (filtroEmpleado) {
            filtroEmpleado.innerHTML = `<option value="">Todos los empleados</option>`;
        }

        const selectDepartamentoPlanilla = document.getElementById('departamentoPlanilla');
        if (selectDepartamentoPlanilla) {
            selectDepartamentoPlanilla.innerHTML = `
                <option value="">Seleccione un departamento</option>
                ${opciones}
            `;
        }
    } catch (error) {
        console.error(error);
    }
}
