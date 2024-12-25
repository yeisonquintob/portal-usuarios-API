# User Portal API

## Descripción General

La **User Portal API** es una aplicación backend RESTful desarrollada en **ASP.NET Core** para manejar la autenticación y la gestión de perfiles de usuarios. Incluye funcionalidades CRUD completas y está diseñada para ser consumida por aplicaciones frontend como Angular, React o Vue. Además, soporta JWT para autenticación segura y permite la implementación de roles para diferenciar vistas y permisos entre administradores y usuarios regulares.

## Características

- Registro de usuarios (POST /users/register).
- Inicio de sesión con generación de tokens JWT (POST /users/login).
- Visualización y edición de perfiles de usuario (GET, PUT /users/{id}).
- Eliminación lógica de usuarios (DELETE /users/{id}).
- Roles: Administrador y Usuario Regular.
- Manejo avanzado de errores y validaciones robustas.
- Sistema de actualización de imágenes de perfil.

## Tecnologías Utilizadas

- **Backend**: ASP.NET Core 8.0.
- **Base de Datos**: SQL Server.
- **Autenticación**: JSON Web Tokens (JWT).
- **ORM**: Entity Framework Core.
- **Logging**: Serilog.
- **Validaciones**: FluentValidation.
- **Seguridad**: Contraseñas encriptadas con BCrypt.

## Arquitectura

La arquitectura sigue el patrón Clean Architecture y está dividida en las siguientes capas:

1. **UserPortal.API**: 
   - Punto de entrada de la aplicación.
   - Configuración de servicios, autenticación y middleware.
   - Controladores para manejar las solicitudes HTTP.

2. **UserPortal.Business**:
   - Lógica de negocio.
   - Servicios para la implementación de funcionalidades principales como autenticación y gestión de usuarios.
   - Validaciones personalizadas.

3. **UserPortal.Data**:
   - Acceso a datos mediante Entity Framework Core.
   - Contexto de base de datos y configuraciones de entidades.
   - Repositorios para abstracción del acceso a datos.

4. **UserPortal.Shared**:
   - DTOs para manejar datos entre las capas.
   - Excepciones personalizadas.
   - Helpers y constantes comunes.

Esta separación permite:
- Escalabilidad del proyecto.
- Reutilización de código.
- Mantenimiento sencillo.

### Decisiones de Diseño

- **JWT**: Se eligió JWT para manejar la autenticación debido a su soporte nativo en ASP.NET Core y su compatibilidad con sistemas distribuidos.
- **BCrypt**: Utilizado para almacenar contraseñas de manera segura gracias a su soporte de hashing robusto y salting.
- **Entity Framework Core**: ORM seleccionado por su integración con ASP.NET Core y soporte para múltiples proveedores de bases de datos.
- **Validaciones**: Implementadas con FluentValidation para asegurar la integridad de los datos recibidos desde el cliente.

## Configuración Inicial

1. **Variables de Entorno**:
   - Configurar `JwtSettings` en `appsettings.json`.
   - Proporcionar la cadena de conexión para la base de datos en `ConnectionStrings.DefaultConnection`.

2. **Migraciones de Base de Datos**:
   - Ejecutar el comando `dotnet ef database update` para aplicar las migraciones iniciales.

3. **CORS**:
   - Configurar los orígenes permitidos en `appsettings.json` bajo la sección `Cors:Origins`.

4. **Ejecución**:
   - Ejecutar el proyecto usando `dotnet run` y acceder a Swagger en `http://localhost:{puerto}/swagger`.

## Ejecución y Pruebas de la API

### Ejecución de la API
1. Clona este repositorio y navega a la carpeta del proyecto.
2. Asegúrate de tener configurado .NET 8.0 o superior.
3. Ejecuta el siguiente comando para restaurar las dependencias:
   ```bash
   dotnet restore
   ```
4. Corre la aplicación usando:
   ```bash
   dotnet run
   ```
5. Accede a la documentación Swagger en `http://localhost:{puerto}/swagger` para explorar los endpoints disponibles.

### Pruebas Manuales

#### Registro de Usuarios
- Endpoint: `POST /users/register`
- Datos requeridos:
  ```json
  {
    "username": "user123",
    "email": "user123@example.com",
    "password": "Password123!",
    "confirmPassword": "Password123!",
    "firstName": "User",
    "lastName": "Example"
  }
  ```
- Respuesta esperada: HTTP 200 con el token JWT.

#### Inicio de Sesión
- Endpoint: `POST /users/login`
- Datos requeridos:
  ```json
  {
    "username": "user123",
    "password": "Password123!"
  }
  ```
- Respuesta esperada: HTTP 200 con el token JWT.

#### Obtención de Perfil
- Endpoint: `GET /users/{id}`
- Requiere el token JWT en el encabezado de autorización.
- Respuesta esperada: HTTP 200 con los datos del usuario.

#### Actualización de Perfil
- Endpoint: `PUT /users/{id}`
- Datos opcionales para actualizar:
  ```json
  {
    "email": "newemail@example.com",
    "firstName": "NewName",
    "lastName": "NewLastName"
  }
  ```
- Respuesta esperada: HTTP 200 con los datos actualizados.

#### Eliminación de Usuario
- Endpoint: `DELETE /users/{id}`
- Requiere permisos de administrador y token JWT.
- Respuesta esperada: HTTP 200 confirmando la eliminación lógica.

## Pruebas Automatizadas

### Unitarias
Se implementaron pruebas unitarias para las siguientes funcionalidades clave:
- Registro de usuario.
- Inicio de sesión.
- Obtención de perfil de usuario.
- Actualización de información del perfil.

### Manuales
- **Registro**: Probar el endpoint POST /users/register con datos válidos e inválidos.
- **Inicio de Sesión**: Validar el inicio de sesión con credenciales correctas e incorrectas.
- **Perfil**: Verificar los permisos de acceso y edición según el usuario.
- **Eliminación**: Confirmar que el borrado lógico afecta solo al usuario seleccionado.

## Futuras Mejoras

- Implementación de tests automáticos de integración.
- Optimización de consultas en la base de datos.
- Soporte para almacenamiento en nube para imágenes de perfil.
- Auditoría de eventos.

---

**Equipo de Desarrollo**: Fredy Fuentes y Yeison D. Quinto.

