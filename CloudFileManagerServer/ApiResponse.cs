using System.Text.Json.Serialization;

namespace CloudFileManagerServer
{
    public class ApiResponse<T>
    {
        public T Result { get; set; }

        public List<string> TraverseLog { get; set; }
    }

    public class ApiResponseMessage<T>
    {
        public string Message { get; set; }
        public T Result { get; set; }
    }
}
