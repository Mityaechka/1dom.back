using System;
using Microsoft.AspNetCore.Mvc;

namespace Taxiverk.Core.Infrastructure.Api
{
    public class ApiController : ControllerBase
    {
        public static ApiData Success() => ApiData.Success();
        public static ApiData<T> Success<T>(T result) => ApiData.Success(result);
        public static ApiData Error(string message) => ApiData.Error(message);
        public static ApiData Error() => ApiData.Error();
        public static ApiData<T> Error<T>() => ApiData.Error<T>();


        public static ApiData<T> NullCheck<T>(T data) where T : class
        {
            if (data == null)
            {
                return Error<T>();
            }

            return Success<T>(data);
        }
        
        public static ApiData<U> NullCheck<T,U>(T data, Func<T,U> map) where T : class where U : class
        {
            if (data == null)
            {
                return Error<U>();
            }

            return Success<U>(map(data));
        }

        public Guid ClientId => new (((string)HttpContext.Items["ClientId"])!);

        public Guid? ClientIdSafe
        {
            get
            {
                var id = HttpContext.Items["ClientId"];
                if (id == null)
                {
                    return null;
                }

                return new Guid((string)id);
            }
        }
    }
}
