using MediatR;
using Microsoft.AspNetCore.Mvc;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Usuarios;
using Milo.Application.Usuarios.DTOs;

namespace MiloAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsuariosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema
        /// </summary>
        /// <param name="request">Datos del usuario a crear</param>
        /// <returns>Respuesta indicando el resultado de la operación</returns>
        /// <response code="200">Usuario creado correctamente</response>
        /// <response code="400">Error en los datos proporcionados o usuario ya existe</response>
        [HttpPost("crear-usuario")]
        public async Task<IActionResult> CrearUsuario([FromBody] CreateUserIn request)
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = new ApiRequestResponse()
                {
                    Code = 400,
                    Message = "Error de validación",
                    Type = "Creación de usuarios",
                    StackTrace = string.Join("; ", errors)
                };
                return BadRequest(validationResponse);
            }

            ApiRequestResponse response = new ApiRequestResponse();
            var data = await _mediator.Send(new CreateUserQuery(request));
            if (data.code.Equals(Constants.CodeSuccess))
            {
                response = new ApiRequestResponse()
                {
                    Code = 200,
                    Message = Constants.MessageSuccess,
                    Type = "Creación de usuarios."
                };
                return Ok(response);
            }
            else
            {
                response = new ApiRequestResponse()
                {
                    Code = 400,
                    Message = Constants.MessageError,
                    Type = "Creación de usuarios.",
                    StackTrace = data.message_error
                };
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene la lista de roles disponibles en el sistema
        /// </summary>
        /// <returns>Lista de roles con sus IDs y nombres</returns>
        /// <response code="200">Lista de roles obtenida correctamente</response>
        /// <response code="400">Error al obtener los roles</response>
        [HttpGet("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            ApiRequestResponse response = new ApiRequestResponse();
            ApiResponseList<GetRolesOut> responseList = new ApiResponseList<GetRolesOut>();
            var data = await _mediator.Send(new GetRolesQuery());
            if (data.code.Equals(Constants.CodeSuccess))
            {
                response = new ApiRequestResponse()
                {
                    Code = 200,
                    Message = Constants.MessageSuccess,
                    Type = "Obtener Roles."
                };
                responseList= new ApiResponseList<GetRolesOut> { 
                    ApiResponse= response,
                    List=data.List
                };
                return Ok(responseList);
            }
            else
            {
                response = new ApiRequestResponse()
                {
                    Code = 400,
                    Message = Constants.MessageError,
                    Type = "Obtener Roles.",
                    StackTrace = data.message_error
                };
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Inicia sesión y obtiene un token JWT para autenticación
        /// </summary>
        /// <param name="request">Credenciales de acceso (email y contraseña)</param>
        /// <returns>Datos del usuario autenticado y token JWT</returns>
        /// <remarks>
        /// El token JWT obtenido debe incluirse en el header Authorization de las peticiones subsiguientes:
        /// Authorization: Bearer {token}
        /// El token tiene una validez de 60 minutos por defecto.
        /// </remarks>
        /// <response code="200">Login exitoso, token JWT generado</response>
        /// <response code="400">Credenciales inválidas o usuario no encontrado</response>
        [HttpPost("login-user")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserIn request)
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = new ApiRequestResponse()
                {
                    Code = 400,
                    Message = "Error de validación",
                    Type = "Login Usuario",
                    StackTrace = string.Join("; ", errors)
                };
                return BadRequest(validationResponse);
            }

            ApiRequestResponse response = new ApiRequestResponse();
            ApiResponseObject<LoginUserOut> responseObj = new ApiResponseObject<LoginUserOut>();
            var data = await _mediator.Send(new LoginUserQuery(request));
            if (data.code.Equals(Constants.CodeSuccess))
            {
                response = new ApiRequestResponse()
                {
                    Code = 200,
                    Message = Constants.MessageSuccess,
                    Type = "Login Usuario"
                };
                responseObj = new ApiResponseObject<LoginUserOut>()
                {
                    ApiResponse= response,
                    Data=data.Data
                };
                return Ok(responseObj);
            }
            else
            {
                response = new ApiRequestResponse()
                {
                    Code = 400,
                    Message = Constants.MessageCatch,
                    Type = "Login Usuario",
                    StackTrace = data.message_error
                };
                responseObj = new ApiResponseObject<LoginUserOut>()
                {
                    ApiResponse = response,
                    Data = null
                };
                return BadRequest(responseObj);
            }
        }

        /// <summary>
        /// Gestiona roles (crear, editar, eliminar)
        /// </summary>
        /// <param name="request">Datos del rol</param>
        /// <param name="opcion">Operación a realizar: "crear", "editar" o "eliminar"</param>
        /// <returns>Respuesta indicando el resultado de la operación</returns>
        /// <response code="200">Operación ejecutada correctamente</response>
        /// <response code="400">Error en los datos proporcionados</response>
        [HttpPost("admin-roles")]
        public async Task<IActionResult> AdminRoles([FromBody] GetRolesOut request, [FromQuery] string opcion)
        {
            ApiRequestResponse response = new ApiRequestResponse();
            var data = await _mediator.Send(new AdminRolesQuery(opcion, request));
            if (data.code.Equals(Constants.CodeSuccess))
            {
                response = new ApiRequestResponse()
                {
                    Code = 200,
                    Message = Constants.MessageSuccess,
                    Type = "Administracion de Roles."
                };
                return Ok(response);
            }
            else
            {
                response = new ApiRequestResponse()
                {
                    Code = 400,
                    Message = Constants.MessageError,
                    Type = "Administracion de Roles.",
                    StackTrace = data.message_error
                };
                return BadRequest(response);
            }
        }
    }
}
