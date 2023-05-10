using Microsoft.AspNetCore.Mvc;

namespace Taxiverk.Core.Infrastructure.Api
{
    public class ApiData
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public static ApiData Success() => new ApiData { IsSuccess = true };
        public static ApiData<T> Success<T>(T result) => new ApiData<T> { IsSuccess = true, Result = result };

        public static ApiData Error(string message) => new ApiData { IsSuccess = false, ErrorMessage = message };
        public static ApiData Error() => new ApiData { IsSuccess = false };
        public static ApiData<T> Error<T>() => new ApiData<T> { IsSuccess = false };
        public static implicit operator JsonResult(ApiData apiData) => new JsonResult(apiData);
    }

    public class ApiData<T> : ApiData
    {
        public T Result { get; set; }
    }
}
