# AsistenciaPyme

Aplicación web desarrollada para facilitar la administración del personal en una MIPYME, permitiendo gestionar empleados, controlar la asistencia, administrar vacaciones y generar planillas de forma sencilla y centralizada.

---

# Descripción

AsistenciaPyme es un sistema web orientado a pequeñas empresas que necesitan registrar la asistencia de sus empleados desde un dispositivo fijo y administrar la información del personal desde un panel administrativo.

El sistema implementa una API REST desarrollada con ASP.NET Core Web API y una arquitectura basada en Clean Architecture y CQRS.

---

# Funcionalidades

- Autenticación de administradores mediante JWT.
- Gestión de administradores.
- Gestión de cargos.
- Gestión de empleados.
- Registro de asistencia mediante código y PIN.
- Consulta de asistencias.
- Gestión de vacaciones.
- Gestión de tipos de deducción.
- Generación de planillas.

---

# Arquitectura

El backend está desarrollado utilizando:

- Clean Architecture
- CQRS
- API REST

Esta estructura permite mantener una separación clara entre la lógica del negocio, el acceso a datos y la presentación.

---

# Tecnologías

## Backend

- C#
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Npgsql
- JWT
  

## Frontend

- HTML
- CSS
- JavaScript

## Herramientas

- Visual Studio
- Visual Studio Code
- Git
- GitHub
- Swagger
- Obsidian

---

# Estructura del proyecto

```text
AsistenciaPyme/
│
├── backend/
│   ├── src/
│   └── AsistenciaPyme.slnx
│
├── frontend/
│
├── docs/
│
└── README.md
```

---

# Requisitos

- .NET 10 SDK
- PostgreSQL
- Visual Studio 2022 o superior
- Git

---

# Instalación

## Clonar el repositorio

```bash
git clone https://github.com/Fabian-Arteaga/asistencia-pyme.git

cd asistencia-pyme
```

## Configurar la base de datos

Crear una base de datos PostgreSQL y actualizar la cadena de conexión en:

```text
backend/src/AsistenciaPyme.WebApi/appsettings.json
```

## Aplicar las migraciones

Desde la carpeta `backend`:

```bash
dotnet ef database update \
--project src/AsistenciaPyme.Infrastructure \
--startup-project src/AsistenciaPyme.WebApi
```

## Ejecutar la API

```bash
dotnet run --project src/AsistenciaPyme.WebApi
```

Proyecto desarrollado para la asignatura **Proyecto de TI II**.
