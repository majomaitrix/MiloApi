using MediatR;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Parameter.DTOs;
using Milo.Application.Usuarios;
using Milo.Domain.Entities;
using Milo.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Parameter
{
    internal sealed class AdminCategoriaQueryHandler : IRequestHandler<AdminCategoriaQuery, Info_data>
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

        public AdminCategoriaQueryHandler(IMediator mediator, ApplicationDbContext context)
        {
            this._mediator = mediator;
            this._context = context;
        }

        public async Task<Info_data> Handle(AdminCategoriaQuery req, CancellationToken cancellationToken)
        {
            Info_data response = new Info_data();
            try
            {
                switch (req.Opcion.ToLower())
                {
                    case "c":
                        var nuevaCategoria = new Categoria
                        {
                            Nombre = req.Categoria.nombre
                        };
                        _context.categorias.Add(nuevaCategoria);
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
                        var categoriaExistente = await _context.categorias.FindAsync(req.Categoria.id);
                        if (categoriaExistente == null)
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeCatch,
                                message = Constants.MessageCatch,
                                message_error = Constants.MessageRolNotFound,
                            };
                            return response;
                        }

                        categoriaExistente.Nombre = req.Categoria.nombre;
                        _context.categorias.Update(categoriaExistente);
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
                        var categoriaEliminar = await _context.categorias.FindAsync(req.Categoria.id);
                        if (categoriaEliminar == null)
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeCatch,
                                message = Constants.MessageCatch,
                                message_error = Constants.MessageRolNotFound,
                            };
                            return response;
                        }

                        _context.categorias.Remove(categoriaEliminar);
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
            catch(Exception ex)
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
