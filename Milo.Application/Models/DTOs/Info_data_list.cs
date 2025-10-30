using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milo.Application.Models.DTOs
{
    public class Info_data_list<T>
    {
        public string code { get; set; }
        public string message { get; set; }
        public string message_error { get; set; }
        public List<T> List { get; set; }
    }
}
