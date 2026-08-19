import { obtenerDepartamentos } from './planillas/planillas.services.js';

export async function cargarDepartamentosEnSelect(selectElement) {
    try {
        const departamentos = await obtenerDepartamentos();
        departamentos.sort((a, b) => (a.nombre || '').localeCompare(b.nombre || '', 'es'));
        const opciones = departamentos.map(d => `
            <option value="${d.idDepartamento}">${d.nombre}</option>
        `).join('');
        selectElement.innerHTML = `<option value="">Seleccione un departamento</option>${opciones}`;
    } catch (err) {
        console.error(err);
        selectElement.innerHTML = `<option value="">No fue posible cargar departamentos</option>`;
    }
}
