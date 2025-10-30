namespace Milo.Application.Models
{
    public class ApiResponseList<T>
    {
        public ApiRequestResponse ApiResponse { get; set; }
        public List<T> List { get; set; }

        public ApiResponseList()
        {

        }
    }
}
