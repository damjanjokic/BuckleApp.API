using System.Net;

namespace BuckleApp.Application.Exceptions;

public class RestException : Exception
{
    public RestException()
    {
    }

    public RestException(HttpStatusCode code, object errors = null)
    {
        Code = code;
        Errors = errors;
    }

    public HttpStatusCode Code { get; }
    public object Errors { get; }
}