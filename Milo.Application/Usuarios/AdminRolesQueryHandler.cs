using MediatR;
using Microsoft.Extensions.Options;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Usuarios.DTOs;
using Milo.Domain.Entities;
using Milo.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Usuarios
{
    internal sealed class AdminRolesQueryHandler : IRequestHandler<AdminRolesQuery, Info_data>
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

        public AdminRolesQueryHandler(IMediator mediator, ApplicationDbContext context)
        {
            this._mediator = mediator;
            this._context = context;
        }

        public async Task<Info_data> Handle(AdminRolesQuery req, CancellationToken cancellationToken)
        {
            Info_data response = new Info_data();
            try
            {

                switch (req.Opcion.ToLower())
                {
                    case "c":
                        var nuevoRol = new Rol
                        {
                            Nombre = req.Rol.Nombre
                        };
                        _context.roles.Add(nuevoRol);
                        var filasAfectadas = await _context.SaveChangesAsync();

                        if (filasAfectadas > 0)
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeSuccess,
                                message = Constants.MessageSuccess,
                                message_error = "",
                            };
                            return response;
                        }
                        else
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeCatch,
                                message = Constants.MessageCatch,
                                message_error = Constants.MessageError,
                            };
                            return response;
                        }

                    case "u":
                        var rolExistente = await _context.roles.FindAsync(req.Rol.Id);
                        if (rolExistente == null)
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeCatch,
                                message = Constants.MessageCatch,
                                message_error = Constants.MessageRolNotFound,
                            };
                            return response;
                        }

                        rolExistente.Nombre = req.Rol.Nombre;
                        _context.roles.Update(rolExistente);
                        var filasAfectadasU = await _context.SaveChangesAsync();
                        if (filasAfectadasU > 0)
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeSuccess,
                                message = Constants.MessageSuccess,
                                message_error = "",
                            };
                            return response;
                        }
                        else
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeCatch,
                                message = Constants.MessageCatch,
                                message_error = Constants.MessageError,
                            };
                            return response;
                        }
                    case "d":
                        var rolEliminar = await _context.roles.FindAsync(req.Rol.Id);
                        if (rolEliminar == null)
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeCatch,
                                message = Constants.MessageCatch,
                                message_error = Constants.MessageRolNotFound,
                            };
                            return response;
                        }

                        _context.roles.Remove(rolEliminar);
                        var filasAfectadasD = await _context.SaveChangesAsync();
                        if (filasAfectadasD > 0)
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeSuccess,
                                message = Constants.MessageSuccess,
                                message_error = "",
                            };
                            return response;
                        }
                        else
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeCatch,
                                message = Constants.MessageCatch,
                                message_error = Constants.MessageError,
                            };
                            return response;
                        }

                    default:
                        response = new Info_data()
                        {
                            code = Constants.CodeCatch,
                            message = Constants.MessageCatch,
                            message_error = "Opcion ingresada no valida, Verifique los datos ingresados",
                        };
                        return response;
                }
            }
            catch (Exception ex)
            {
                response = new Info_data()
                {
                    code = Constants.CodeCatch,
                    message = Constants.MessageCatch,
                    message_error = ex.Message,
                };
                return response;
            }
        }
    }
}
