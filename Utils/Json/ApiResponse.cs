namespace Utils.Json
{
    public class ApiResponse<T>
    {
        public T Data { get; init; }
        public bool Success { get; init; } = true;
        public string? Message { get; init; }

        public ApiResponse(T data) => Data = data;
    }
}
