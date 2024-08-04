namespace MicroserviceTest.Services
{
    public class ServiceResult<T>
    {
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
    }
}
