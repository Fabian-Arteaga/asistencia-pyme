import {
    obtenerAdministrador,
    cerrarSesion
} from "./auth.js";

export function inicializarLayout(configuracion) {
    const titulo =
        configuracion?.titulo ??
        "Panel administrativo";

    const paginaActiva =
        configuracion?.paginaActiva ??
        "dashboard";

    renderizarSidebar();
    renderizarTopbar(titulo);
    activarPagina(paginaActiva);
    cargarAdministrador();
    configurarEventos();
}

function renderizarSidebar() {
    const contenedor =
        document.getElementById(
            "sidebarContainer"
        );

    if (!contenedor) {
        return;
    }

    contenedor.innerHTML = `
        <aside
            id="sidebar"
            class="sidebar"
        >
            <div class="sidebar__marca">
                <div class="sidebar__logo">
                    AP
                </div>

                <div>
                    <strong>AsistenciaPyme</strong>
                    <small>Administración</small>
                </div>
            </div>

            <nav class="sidebar__navegacion">
                <span class="sidebar__titulo-menu">
                    Menú principal
                </span>

                <a
                    href="../dashboard/dashboard.html"
                    class="menu-link"
                    data-menu="dashboard"
                >
                    <span class="menu-link__icono">
                        D
                    </span>

                    <span>Dashboard</span>
                </a>

                <a
                    href="../empleados/empleados.html"
                    class="menu-link"
                    data-menu="empleados"
                >
                    <span class="menu-link__icono">
                        E
                    </span>

                    <span>Empleados</span>
                </a>

                <a
                    href="../../modules/cargos/cargos.html"
                    class="menu-link"
                    data-menu="cargos"
                >
                    <span class="menu-link__icono">
                        C
                    </span>

                    <span>Cargos</span>
                </a>

                <a
                    href="../asistencias/asistencias.html"
                    class="menu-link"
                    data-menu="asistencias"
                >
                    <span class="menu-link__icono">
                        A
                    </span>

                    <span>Asistencias</span>
                </a>

                <a
                    href="../vacaciones/vacaciones.html"
                    class="menu-link"
                    data-menu="vacaciones"
                >
                    <span class="menu-link__icono">
                        V
                    </span>

                    <span>Vacaciones</span>
                </a>

                <a
                    href="../deducciones/deducciones.html"
                    class="menu-link"
                    data-menu="deducciones"
                >
                    <span class="menu-link__icono">
                        TD
                    </span>

                    <span>Tipos de deducción</span>
                </a>

                <a
                    href="../planillas/planillas.html"
                    class="menu-link"
                    data-menu="planillas"
                >
                    <span class="menu-link__icono">
                        P
                    </span>

                    <span>Planillas</span>
                </a>

                <a
                    href="../administradores/administradores.html"
                    class="menu-link"
                    data-menu="administradores"
                >
                    <span class="menu-link__icono">
                        AD
                    </span>

                    <span>Administradores</span>
                </a>

            </nav>

            <div class="sidebar__pie">
                <span>
                    Sistema de asistencia
                </span>

                <small>
                    AsistenciaPyme
                </small>
            </div>
        </aside>
    `;
}

function renderizarTopbar(titulo) {
    const contenedor =
        document.getElementById(
            "topbarContainer"
        );

    if (!contenedor) {
        return;
    }

    contenedor.innerHTML = `
        <header class="topbar">
            <div class="topbar__izquierda">
                <button
                    type="button"
                    id="btnAbrirMenu"
                    class="topbar__boton-menu"
                    aria-label="Abrir menú"
                >
                    ☰
                </button>

                <h1>${escaparHtml(titulo)}</h1>
            </div>

            <div class="topbar__usuario">
                <div class="topbar__datos">
                    <strong id="topbarNombre">
                        Administrador
                    </strong>

                    <small id="topbarRol">
                        Administrador
                    </small>
                </div>

                <div
                    id="topbarInicial"
                    class="topbar__avatar"
                >
                    A
                </div>

                <button
                    type="button"
                    id="btnCerrarSesion"
                    class="topbar__cerrar-sesion"
                >
                    Cerrar sesión
                </button>
            </div>
        </header>
    `;
}

function activarPagina(paginaActiva) {
    const enlaces =
        document.querySelectorAll(
            "[data-menu]"
        );

    enlaces.forEach(enlace => {
        enlace.classList.remove(
            "menu-link--activo"
        );

        if (
            enlace.dataset.menu ===
            paginaActiva
        ) {
            enlace.classList.add(
                "menu-link--activo"
            );
        }
    });
}

function cargarAdministrador() {
    const administrador =
        obtenerAdministrador();

    const nombre =
        administrador?.nombreCompleto ??
        "Administrador";

    const rol =
        administrador?.rol ??
        "Administrador";

    const elementoNombre =
        document.getElementById(
            "topbarNombre"
        );

    const elementoRol =
        document.getElementById(
            "topbarRol"
        );

    const elementoInicial =
        document.getElementById(
            "topbarInicial"
        );

    if (elementoNombre) {
        elementoNombre.textContent =
            nombre;
    }

    if (elementoRol) {
        elementoRol.textContent =
            rol;
    }

    if (elementoInicial) {
        elementoInicial.textContent =
            obtenerInicial(nombre);
    }
}

function configurarEventos() {
    const botonMenu =
        document.getElementById(
            "btnAbrirMenu"
        );

    const botonCerrarSesion =
        document.getElementById(
            "btnCerrarSesion"
        );

    const overlay =
        document.getElementById(
            "sidebarOverlay"
        );

    botonMenu?.addEventListener(
        "click",
        alternarSidebar
    );

    overlay?.addEventListener(
        "click",
        cerrarSidebar
    );

    botonCerrarSesion?.addEventListener(
        "click",
        cerrarSesion
    );
}

function alternarSidebar() {
    document.body.classList.toggle(
        "sidebar-abierto"
    );
}

function cerrarSidebar() {
    document.body.classList.remove(
        "sidebar-abierto"
    );
}

function obtenerInicial(nombre) {
    const nombreLimpio =
        nombre.trim();

    if (!nombreLimpio) {
        return "A";
    }

    return nombreLimpio
        .charAt(0)
        .toUpperCase();
}

function escaparHtml(texto) {
    const elemento =
        document.createElement("div");

    elemento.textContent = texto;

    return elemento.innerHTML;
}