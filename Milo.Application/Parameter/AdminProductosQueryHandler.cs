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
    internal sealed class AdminProductosQueryHandler : IRequestHandler<AdminProductosQuery, Info_data>
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

        public AdminProductosQueryHandler(IMediator mediator, ApplicationDbContext context)
        {
            this._mediator = mediator;
            this._context = context;
        }

        public async Task<Info_data> Handle(AdminProductosQuery req, CancellationToken cancellationToken)
        {
            Info_data response = new Info_data();
            try
            {
                switch (req.Opcion.ToLower())
                {
                    case "c":
                        var nuevoProducto = new Producto
                        {
                            Nombre = req.Producto.nombre,
                            Descripcion = req.Producto.descripcion,
                            Precio = req.Producto.precio,
                            Stock = req.Producto.stock,
                            CategoriaId = req.Producto.categoriaId,
                            Imagen = req.Producto.imagen,
                            Activo = req.Producto.activo
                        };
                        _context.productos.Add(nuevoProducto);
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
                        var productoExistente = await _context.productos.FindAsync(req.Producto.id);
                        if (productoExistente == null)
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeCatch,
                                message = Constants.MessageCatch,
                                message_error = Constants.MessageRolNotFound,
                            };
                            return response;
                        }
                        productoExistente.Nombre = req.Producto.nombre;
                        productoExistente.Descripcion = req.Producto.descripcion;
                        productoExistente.Precio = req.Producto.precio;
                        productoExistente.Stock = req.Producto.stock;
                        productoExistente.CategoriaId = req.Producto.categoriaId;
                        productoExistente.Imagen = req.Producto.imagen;
                        productoExistente.Activo = req.Producto.activo;

                        _context.productos.Update(productoExistente);
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
                        var productoEliminar = await _context.productos.FindAsync(req.Producto.id);
                        if (productoEliminar == null)
                        {
                            response = new Info_data()
                            {
                                code = Constants.CodeCatch,
                                message = Constants.MessageCatch,
                                message_error = Constants.MessageRolNotFound,
                            };
                            return response;
                        }

                        _context.productos.Remove(productoEliminar);
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
