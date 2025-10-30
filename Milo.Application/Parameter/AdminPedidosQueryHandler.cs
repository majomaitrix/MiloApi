using MediatR;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Domain.Entities;
using Milo.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Parameter
{
    internal sealed class AdminPedidosQueryHandler : IRequestHandler<AdminPedidosQuery, Info_data>
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

        public AdminPedidosQueryHandler(IMediator mediator, ApplicationDbContext context)
        {
            this._mediator = mediator;
            this._context = context;
        }

        public async Task<Info_data> Handle(AdminPedidosQuery req, CancellationToken cancellationToken)
        {
            Info_data response = new Info_data();
            try
            {
                switch (req.Opcion.ToLower())
                {
                    case "c":
                        var nuevoPedido = new Pedido
                        {
                            Fecha = req.Pedido.Fecha,
                            Total = req.Pedido.Total,
                        };
                        _context.pedidos.Add(nuevoPedido);
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
                        var pedidoExistente = await _context.pedidos.FindAsync(req.Pedido.Id);
                        if (pedidoExistente == null)
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeCatch,
                                message = Constants.MessageCatch,
                                message_error = Constants.MessageRolNotFound,
                            };
                            return response;
                        }
                        pedidoExistente.Total = req.Pedido.Total;
                        if(req.Pedido.UsuarioId != null)
                        {
                            pedidoExistente.UsuarioId = req.Pedido.UsuarioId;
                        }
                        
                        _context.pedidos.Update(pedidoExistente);
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
                        var pedidoEliminar = await _context.pedidos.FindAsync(req.Pedido.Id);
                        if (pedidoEliminar == null)
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeCatch,
                                message = Constants.MessageCatch,
                                message_error = Constants.MessageRolNotFound,
                            };
                            return response;
                        }

                        _context.pedidos.Remove(pedidoEliminar);
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
