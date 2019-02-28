using AspNet.WebApi.Exceptions.Models;
using Newtonsoft.Json;
using Xunit;
using Xunit.Asserts.Compare;

namespace AspNet.WebApi.Exceptions.Tests
{
    public class ApiExceptionModelTests
    {
        [Theory]
        [InlineData(123, "456", "message")]
        public void Serialization(int statusCode, string reasonCode, string message)
        {
            var apiException = new ApiException(123, "456", "message");
            var json = JsonConvert.SerializeObject(apiException);
            var actual = JsonConvert.DeserializeObject<ApiExceptionModel>(json);

            Assert.Equal(statusCode, actual.StatusCode);
            Assert.Equal(reasonCode, actual.ReasonCode);
            Assert.Equal(message, actual.Message);

            var expected = new ApiExceptionModel(apiException);

            DeepAssert.Equal(expected, actual);
        }
    }
}
