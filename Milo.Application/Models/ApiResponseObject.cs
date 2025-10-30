namespace Milo.Application.Models
{
    public class ApiResponseObject<T>
    {
        public ApiRequestResponse ApiResponse { get; set; }
        public T Data { get; set; }

        public ApiResponseObject()
        {

        }
    }
}
