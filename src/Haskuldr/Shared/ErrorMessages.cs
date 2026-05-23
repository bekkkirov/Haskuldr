namespace Haskuldr.Shared;

internal static class ErrorMessages
{
    internal const string NoAssembliesProvided = "At least one assembly must be provided";
    
    internal const string HttpInvalidHeader = "The specified header could not be applied to the request";
    internal const string HttpRequestAlreadySent = "This HTTP request builder is single-use and has already been executed";
    internal const string HttpResponseBodyRequired = "The HTTP response body was empty or deserialized to null";
    
    internal const string HttpRouteParamsMismatch = "The number of route parameters does not match the number of placeholders in the route";
    internal const string HttpInvalidBaseAddress = "The base address is invalid";
    internal const string InvalidRoute = "The route template is invalid";
    internal const string InvalidQueryString = "The query string is invalid";
}