# Índice

1. Introducción
2. Descripción del proyecto
3. Problema a resolver
4. Objetivos
5. Alcance del sistema
6. Actores del sistema
7. Módulos del sistema
8. Casos de uso
9. Historias de usuario
10. Requerimientos funcionales
11. Requerimientos no funcionales

---
# 1. Introducción

El presente documento define los requisitos del sistema **AsistenciaPyme**, una aplicación web orientada a facilitar la administración del personal de una MIPYME.

El documento describe el problema que se desea solucionar, los objetivos, el alcance, los actores, los casos de uso, las historias de usuario y los requisitos principales del sistema.

---
# 2. Descripción del proyecto

**AsistenciaPyme** será una aplicación web para gestionar la información laboral y la asistencia de los empleados de una pequeña empresa.

El sistema permitirá registrar empleados, controlar sus horarios de entrada y salida, gestionar vacaciones, calcular una planilla básica, aplicar deducciones y visualizar reportes.

La aplicación contará con dos tipos de usuario:

- Administrador.
- Empleado.

Cada usuario tendrá acceso únicamente a las funciones correspondientes a su rol.

---

# 3. Problema a resolver

Las pequeñas empresas suelen controlar la asistencia y la información de sus empleados mediante cuadernos, hojas de cálculo o documentos separados.

Esta forma de trabajo puede ocasionar:

- Pérdida o duplicación de información.
- Dificultad para controlar entradas y salidas.
- Falta de seguimiento de vacaciones.
- Errores en el cálculo de salarios y deducciones.
- Dificultad para consultar reportes del personal.

---

# 4. Objetivos

## 4.1 Objetivo general

Desarrollar una aplicación web que facilite la gestión del personal y el control de asistencia de una MIPYME 

## 4.2 Objetivos específicos

- Registrar y mantener actualizada la información de los empleados.
- Controlar las horas de entrada y salida.
- Gestionar solicitudes de vacaciones.
- Calcular una planilla básica con ingresos y deducciones.
- Permitir la consulta de reportes relacionados con el personal.
- Proteger el acceso mediante autenticación y roles.

---

# 5. Alcance del sistema

El sistema permitirá:

- Iniciar y cerrar sesión.
- Registrar datos personales y laborales.
- Guardar la fecha de contratación.
- Registrar cargo, funciones y responsabilidades.
- Registrar entradas y salidas.
- Consultar el historial de asistencia.
- Solicitar, aprobar o rechazar vacaciones.
- Registrar salario base e ingresos adicionales.
- Registrar deducciones.
- Calcular el salario neto.
- Visualizar reportes dentro del sistema.
- Consultar y actualizar información básica del perfil.


# 6. Actores del sistema

## 6.1 Administrador

Será responsable de administrar la información general del sistema.

Podrá:

- Gestionar empleados.
- Consultar y corregir asistencias.
- Aprobar o rechazar vacaciones.
- Registrar deducciones.
- Generar planillas.
- Visualizar reportes.

## 6.2 Empleado

Será responsable de registrar su asistencia y consultar su información.

Podrá:

- Registrar entrada y salida.
- Consultar su asistencia.
- Solicitar vacaciones.
- Consultar sus planillas y deducciones.
- Administrar su perfil.

---

# 7. Módulos del sistema

## 7.1 Autenticación

Permitirá iniciar sesión, cerrar sesión e identificar el rol del usuario.

## 7.2 Empleados

Permitirá registrar y actualizar los datos personales y laborales de los empleados.

## 7.3 Asistencia

Permitirá registrar entradas y salidas, además de consultar el historial de asistencia.

## 7.4 Vacaciones

Permitirá solicitar, aprobar, rechazar y consultar vacaciones.

## 7.5 Planillas y deducciones

Permitirá registrar salarios, ingresos adicionales, deducciones y calcular el salario neto.

## 7.6 Reportes

Permitirá visualizar información resumida de asistencia, vacaciones y planillas.

## 7.7 Perfil

Permitirá consultar información básica y cambiar la contraseña.

---

# 8. Casos de uso

| Código | Actor | Caso de uso | Descripción |
|---|---|---|---|
| CU-01 | Administrador y Empleado | Iniciar sesión | Permite acceder al sistema mediante credenciales válidas. |
| CU-02 | Administrador | Gestionar empleados | Permite registrar, editar, consultar y desactivar empleados. |
| CU-03 | Empleado | Registrar asistencia | Permite registrar la hora de entrada y salida. |
| CU-04 | Administrador | Consultar asistencias | Permite consultar y corregir registros de asistencia. |
| CU-05 | Empleado | Solicitar vacaciones | Permite registrar una solicitud de vacaciones. |
| CU-06 | Administrador | Gestionar vacaciones | Permite aprobar o rechazar solicitudes. |
| CU-07 | Administrador | Generar planilla | Permite calcular ingresos, deducciones y salario neto. |
| CU-08 | Administrador | Visualizar reportes | Permite consultar reportes dentro del sistema. |

## 8.1 Flujo general de inicio de sesión

1. El usuario ingresa sus credenciales.
2. El sistema valida la información.
3. El sistema identifica el rol del usuario.
4. El sistema muestra las funciones autorizadas.
5. Si las credenciales son incorrectas, se muestra un mensaje de error.

## 8.2 Flujo general del registro de asistencia

1. El empleado inicia sesión.
2. Accede al módulo de asistencia.
3. Selecciona registrar entrada o salida.
4. El sistema guarda la fecha y hora.
5. El sistema muestra una confirmación.
---
# 9. Historias de usuario

| Código | Historia de usuario                                                                               |
| ------ | ------------------------------------------------------------------------------------------------- |
| HU-01  | Como usuario, quiero iniciar sesión para acceder de forma segura al sistema.                      |
| HU-02  | Como administrador, quiero registrar empleados para mantener actualizado el personal.             |
| HU-03  | Como empleado, quiero registrar mi entrada y salida para dejar constancia de mi jornada.          |
| HU-04  | Como administrador, quiero consultar las asistencias para supervisar el cumplimiento de horarios. |
| HU-05  | Como administrador, quiero generar la planilla para calcular el salario neto de cada empleado.    |
| HU-06  | Como empleado, quiero consultar mi planilla para conocer mis ingresos y deducciones.              |

---

# 10. Requerimientos funcionales

| Código | Requerimiento                                                                                             |
| ------ | --------------------------------------------------------------------------------------------------------- |
| RF-01  | El sistema deberá permitir que los usuarios inicien y cierren sesión.                                     |
| RF-02  | El sistema deberá controlar el acceso según el rol de Administrador o Empleado.                           |
| RF-03  | El sistema deberá permitir registrar, consultar, actualizar y desactivar empleados.                       |
| RF-04  | El sistema deberá registrar la fecha de contratación, cargo, funciones, responsabilidades del empleado.   |
| RF-05  | El sistema deberá permitir que cada empleado registre su propia entrada y salida.                         |
| RF-06  | El sistema deberá guardar automáticamente la fecha y hora de cada registro de asistencia.                 |
| RF-07  | El sistema deberá impedir registrar una salida si no existe una entrada pendiente.                        |
| RF-08  | El sistema deberá permitir al administrador consultar y corregir asistencias justificadas.                |
| RF-09  | El sistema deberá permitir registrar deducciones como monto fijo o porcentaje.                            |
| RF-10  | El sistema deberá calcular el salario neto considerando salario base, ingresos adicionales y deducciones. |


---

# 11. Requerimientos no funcionales

| Código | Requerimiento |
|---|---|
| RNF-01 | El sistema deberá proteger las funciones mediante autenticación y autorización por roles. |
| RNF-02 | Las contraseñas no deberán almacenarse en texto plano. |
| RNF-03 | La interfaz deberá ser clara, sencilla y fácil de utilizar. |
| RNF-04 | Las operaciones comunes deberán responder en un tiempo máximo aproximado de tres segundos bajo condiciones normales. |
| RNF-05 | El sistema deberá validar la información antes de almacenarla y mostrar mensajes comprensibles cuando ocurra un error. |

