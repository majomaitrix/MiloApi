using MediatR;
using Microsoft.EntityFrameworkCore;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Domain.Entities;
using Milo.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Usuarios
{
    public class CreateUserQueryHandler : IRequestHandler<CreateUserQuery,Info_data>
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

        public CreateUserQueryHandler(IMediator mediator, ApplicationDbContext context)
        {
            this._mediator = mediator;
            this._context = context;
        }

        public async Task<Info_data> Handle(CreateUserQuery req, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = new Usuario
                {
                    Nombre = req.User.Nombre,
                    Email = req.User.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(req.User.Contraseña), // Recuerda hashear la clave antes de guardarla.
                    RolId = req.User.Rol,
                    Direccion=req.User.Direccion,
                    Telefono=req.User.Telefono
                };
                _context.usuarios.Add(usuario);
                var result = await _context.SaveChangesAsync(); // Guarda los cambios en la BD

                if (result > 0)
                {
                    return new Info_data
                    {
                        code = Constants.CodeSuccess,
                        message = Constants.MessageSuccess,
                        message_error = ""
                    };
                }
                else
                {
                    return new Info_data
                    {
                        code = Constants.CodeCatch,
                        message = Constants.MessageErrCrearUser,
                        message_error = ""
                    };
                }
                
            }
            catch (DbUpdateException dbEx)
            {
                return new Info_data
                {
                    code = Constants.CodeCatch,
                    message = Constants.MessageCatch,
                    message_error = dbEx.InnerException?.Message ?? dbEx.Message
                };
            }
            catch (Exception ex)
            {
                return new Info_data
                {
                    code = Constants.CodeCatch,
                    message = Constants.MessageCatch,
                    message_error = ex.Message
                };
            }
        }
    }
}
