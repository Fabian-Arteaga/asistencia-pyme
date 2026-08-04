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

El presente documento define los requisitos del sistema **AsistenciaPyme**, una aplicación web orientada a facilitar la administración del personal y el registro de asistencia de una MIPYME.

El documento describe el problema que se desea solucionar, los objetivos, el alcance, los actores, los casos de uso, las historias de usuario y los requisitos principales del sistema.

---

# 2. Descripción del proyecto

**AsistenciaPyme** será una aplicación web para gestionar la información laboral y la asistencia de los empleados de una pequeña empresa.

El sistema contará con dos áreas principales:

- Un módulo de marcaje para los empleados.
- Un panel administrativo para la gestión del sistema.

Los empleados registrarán su asistencia desde un dispositivo fijo ubicado en la empresa. Para identificarse, deberán ingresar su código de empleado y PIN personal.

El administrador tendrá acceso al sistema mediante correo y contraseña, y podrá gestionar empleados, asistencias, vacaciones, planillas y deducciones.

---

# 3. Problema a resolver

Las pequeñas empresas suelen controlar la asistencia y la información de sus empleados mediante cuadernos, hojas de cálculo o documentos separados.

Esta forma de trabajo puede ocasionar:

- Pérdida o duplicación de información.
- Dificultad para controlar entradas y salidas.
- Falta de seguimiento de vacaciones.
- Errores en el cálculo de salarios y deducciones.
- Dificultad para consultar la información del personal.

---

# 4. Objetivos

## 4.1 Objetivo general

Desarrollar una aplicación web que facilite la gestión del personal y el control de asistencia de una MIPYME.

## 4.2 Objetivos específicos

- Registrar y mantener actualizada la información de los empleados.
- Registrar las entradas y salidas desde un dispositivo fijo.
- Gestionar vacaciones de los empleados.
- Calcular una planilla básica con ingresos y deducciones.

---

# 5. Alcance del sistema

El sistema permitirá:

- Iniciar y cerrar sesión como administrador.
- Registrar, consultar, actualizar y desactivar empleados.
- Asignar un código y PIN de marcaje a cada empleado.
- Guardar la fecha de contratación.
- Registrar cargo, funciones y responsabilidades.
- Registrar asistencia desde un dispositivo fijo.
- Determinar automáticamente si el marcaje corresponde a una entrada o salida.
- Consultar registros de asistencia.
- Registrar y consultar vacaciones.
- Registrar salario base e ingresos adicionales.
- Registrar deducciones.
- Calcular el salario neto.

---

# 6. Actores del sistema

## 6.1 Administrador

Será responsable de administrar la información general del sistema.

Podrá:

- Iniciar y cerrar sesión.
- Gestionar empleados.
- Asignar código y PIN de marcaje.
- Consultar asistencias.
- Registrar vacaciones.
- Registrar deducciones.
- Generar planillas.

## 6.2 Empleado

Utilizará únicamente el módulo de marcaje ubicado en el dispositivo fijo de la empresa.

Podrá:

- Ingresar su código de empleado.
- Ingresar su PIN personal.
- Registrar su asistencia.
- Recibir una confirmación del marcaje.

---

# 7. Módulos del sistema

## 7.1 Autenticación administrativa

Permitirá al administrador iniciar sesión, cerrar sesión y acceder al panel de gestión.

## 7.2 Marcaje de asistencia

Permitirá al empleado identificarse mediante código y PIN para registrar su asistencia.

El sistema determinará automáticamente si corresponde registrar una entrada o una salida.

## 7.3 Empleados

Permitirá registrar y actualizar los datos personales y laborales de los empleados, incluyendo su código y PIN de marcaje.

## 7.4 Asistencias

Permitirá consultar los registros de entrada y salida.

## 7.5 Vacaciones

Permitirá al administrador registrar, actualizar y consultar las vacaciones de los empleados.

## 7.6 Planillas y deducciones

Permitirá registrar salarios, ingresos adicionales, deducciones y calcular el salario neto.

---

# 8. Casos de uso

| Código | Actor         | Caso de uso           | Descripción                                                            |
| ------ | ------------- | --------------------- | ---------------------------------------------------------------------- |
| CU-01  | Administrador | Iniciar sesión        | Permite acceder al panel administrativo mediante credenciales válidas. |
| CU-02  | Administrador | Gestionar empleados   | Permite registrar, editar, consultar y desactivar empleados.           |
| CU-03  | Empleado      | Registrar asistencia  | Permite identificarse mediante código y PIN para registrar un marcaje. |
| CU-04  | Administrador | Consultar asistencias | Permite consultar los registros de asistencia de los empleados.        |
| CU-05  | Administrador | Gestionar vacaciones  | Permite registrar, actualizar y consultar vacaciones.                  |

## 8.1 Flujo general de inicio de sesión

1. El administrador ingresa su correo y contraseña.
2. El sistema valida la información.
3. Si las credenciales son correctas, permite acceder al panel administrativo.
4. Si las credenciales son incorrectas, muestra un mensaje de error.

## 8.2 Flujo general del registro de asistencia

1. El empleado utiliza el dispositivo fijo de la empresa.
2. Ingresa su código de empleado y PIN.
3. El sistema valida su identidad.
4. El empleado selecciona **Registrar asistencia**.
5. El sistema verifica si existe una asistencia con entrada pendiente de salida.
6. Si no existe una entrada pendiente, registra una entrada.
7. Si existe una entrada pendiente, registra la salida.
8. El sistema guarda automáticamente la fecha y hora.
9. El sistema muestra una confirmación.

---

# 9. Historias de usuario

| Código | Historia de usuario |
| ------ | ------------------- |
| HU-01 | Como administrador, quiero iniciar sesión para acceder de forma segura al panel administrativo. |
| HU-02 | Como administrador, quiero registrar empleados para mantener actualizado el personal. |
| HU-03 | Como administrador, quiero asignar un código y PIN a cada empleado para identificarlo durante el marcaje. |
| HU-04 | Como empleado, quiero registrar mi asistencia mediante mi código y PIN para dejar constancia de mi jornada. |
| HU-05 | Como administrador, quiero consultar las asistencias para supervisar el cumplimiento de horarios. |
| HU-06 | Como administrador, quiero registrar vacaciones para mantener actualizado el historial del empleado. |
| HU-07 | Como administrador, quiero generar la planilla para calcular el salario neto de cada empleado. |

---

# 10. Requerimientos funcionales

| Código | Requerimiento |
| ------ | ------------- |
| RF-01 | El sistema deberá permitir que el administrador inicie y cierre sesión. |
| RF-02 | El sistema deberá permitir registrar, consultar, actualizar y desactivar empleados. |
| RF-03 | El sistema deberá registrar la fecha de contratación, cargo, funciones y responsabilidades del empleado. |
| RF-04 | El sistema deberá asignar un código único y un PIN personal a cada empleado. |
| RF-05 | El sistema deberá permitir registrar la asistencia desde un dispositivo fijo ubicado en la empresa. |

---

# 11. Requerimientos no funcionales

| Código | Requerimiento |
| ------ | ------------- |
| RNF-01 | El panel administrativo deberá estar protegido mediante autenticación personalizada y tokens JWT. |
| RNF-02 | Las contraseñas y los PIN deberán almacenarse de forma segura mediante PBKDF2 y no en texto plano. |
| RNF-03 | La pantalla de marcaje deberá ser clara, sencilla y fácil de utilizar. |
| RNF-04 | Las operaciones comunes deberán responder en un tiempo máximo aproximado de tres segundos bajo condiciones normales. |
| RNF-05 | El sistema deberá validar la información antes de almacenarla y mostrar mensajes comprensibles cuando ocurra un error. |