namespace AspNet.WebApi.Exceptions.Interfaces
{
    /// <summary>ApiException interface.</summary>
    public interface IApiException
    {
        /// <summary>Gets a message that describes the current exception.</summary>
        string Message { get; }

        /// <summary>Gets or sets StatusCode.</summary>
        int StatusCode { get; }

        /// <summary>Gets or sets ReasonCode.</summary>
        string ReasonCode { get; }
    }
}
