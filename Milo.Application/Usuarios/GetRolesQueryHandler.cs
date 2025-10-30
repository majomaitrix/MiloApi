using MediatR;
using Milo.Application.Models;
using Milo.Application.Models.DTOs;
using Milo.Application.Usuarios.DTOs;
using Milo.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Usuarios
{
    internal sealed class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Info_data_list<GetRolesOut>>
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

        public GetRolesQueryHandler(IMediator mediator, ApplicationDbContext context)
        {
            this._mediator = mediator;
            this._context = context;
        }

        public async Task<Info_data_list<GetRolesOut>> Handle(GetRolesQuery req, CancellationToken cancellationToken)
        {
            try
            {
                GetRolesOut rolesRes = new GetRolesOut();
                List<GetRolesOut> listRolesRes = new List<GetRolesOut>();
                var roles = _context.roles.ToList();
                foreach (var role in roles)
                {
                    rolesRes = new GetRolesOut()
                    {
                        Id=role.Id,
                        Nombre=role.Nombre
                    };
                    listRolesRes.Add(rolesRes);
                }
                return new Info_data_list<GetRolesOut>
                {
                    code = Constants.CodeSuccess,
                    message = Constants.MessageSuccess,
                    message_error = "",
                    List= listRolesRes
                };
            }
            catch(Exception ex) {
                return new Info_data_list<GetRolesOut>
                {
                    code = Constants.CodeCatch,
                    message = Constants.MessageCatch,
                    message_error = ex.Message,
                    List=null
                };
            }
        }
    }
}
