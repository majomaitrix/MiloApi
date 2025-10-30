using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Models
{
    public class Constants
    {
        public const string CodeCatch = "400";
        public const string CodeSuccess = "200";
        public const string MessageCatch = "Error en el sistema al ejecutar el procedimiento.";
        public const string MessageError = "Error, porfavor verificar los datos ingresados";
        public const string MessageSuccess = "Procedimiento ejecutado correctamente.";
        public const string MessageErrCrearUser = "200";
        public const string MessageNotFound = "Usuario no encontrado, Verifique las credenciales.";
        public const string MessageRolNotFound = "Rol no encontrado, Verifique los datos del rol.";
        public const string MessageNotFoundErr = "Usuario no encontrado en la DB";
    }
}
