using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using AspNet.WebApi.Exceptions.Interfaces;
using Xunit;
using Xunit.Asserts.Compare;

namespace AspNet.WebApi.Exceptions.Tests
{
    /// <summary>Exceptions tests.</summary>
    public class ExceptionsTests
    {

        private readonly IEnumerable<Type> _exceptionTypes = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                                             from type in assembly.GetTypes()
                                                             where typeof(ApiException).IsAssignableFrom(type)
                                                             select type;

        /// <summary>Test for all current assembly Exceptions based on <see cref="ApiException"/>.</summary>
        [Fact]
        public void StatusCodes()
        {
            const int fakeStatusCode = 10;
            const string fakeReasonCode = "FAKE";
            const string message = "test message";
            const string innerMessage = "test inner message";

            var exception = new ArgumentNullException(innerMessage);

            foreach (var type in _exceptionTypes)
            {
                var statusCodeField = type.GetField("STATUS_CODE", BindingFlags.Static | BindingFlags.Public);
                var reasonCodeField = type.GetField("REASON_CODE", BindingFlags.Static | BindingFlags.Public);

                Assert.NotNull(statusCodeField);
                Assert.NotNull(reasonCodeField);

                var statusCode = (int)statusCodeField.GetValue(null);
                var reasonCode = (string)reasonCodeField.GetValue(null);
                var instance = Activator.CreateInstance(type) as ApiException;

                Assert.IsAssignableFrom<ApiException>(instance);
                Assert.NotNull(instance);
                Assert.Equal(statusCode, instance.StatusCode);
                Assert.Equal(reasonCode, instance.ReasonCode);

                var apiException = Activator.CreateInstance(type, message) as ApiException;

                Assert.IsAssignableFrom<ApiException>(instance);
                Assert.NotNull(apiException);
                Assert.Equal(statusCode, apiException.StatusCode);
                Assert.Equal(reasonCode, instance.ReasonCode);
                Assert.Equal(message, apiException.Message);

                var apiExceptionInner = Activator.CreateInstance(type, message, exception) as ApiException;

                Assert.IsAssignableFrom<ApiException>(instance);
                Assert.NotNull(apiExceptionInner);
                Assert.Equal(statusCode, apiExceptionInner.StatusCode);
                Assert.Equal(reasonCode, instance.ReasonCode);
                Assert.Equal(message, apiExceptionInner.Message);
                Assert.Equal(exception.Message, apiExceptionInner.InnerException?.Message);

                var apiExceptionStatusCode = Activator.CreateInstance(type, fakeStatusCode, fakeReasonCode, message, exception) as ApiException;

                Assert.IsAssignableFrom<ApiException>(instance);
                Assert.NotNull(apiExceptionStatusCode);
                Assert.Equal(fakeStatusCode, apiExceptionStatusCode.StatusCode);
                Assert.Equal(fakeReasonCode, apiExceptionStatusCode.ReasonCode);
                Assert.Equal(message, apiExceptionStatusCode.Message);
                Assert.Equal(exception.Message, apiExceptionStatusCode.InnerException?.Message);
            }
        }

        [Fact]
        public void Serialization()
        {
            foreach (var type in _exceptionTypes)
            {
                IApiException exception;

                try
                {
                    throw (ApiException)Activator.CreateInstance(type);
                }
                catch (ApiException ex)
                {
                    exception = ex;
                }

                var formatter = new BinaryFormatter();

                IApiException actual;

                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, exception);
                    ms.Seek(0, SeekOrigin.Begin);
                    actual = (IApiException)formatter.Deserialize(ms);
                }

                DeepAssert.Equal(exception, actual, "StackTrace", "TargetSite");
            }
        }
    }
}