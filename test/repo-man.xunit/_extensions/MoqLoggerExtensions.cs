using Microsoft.Extensions.Logging;
using Moq;

namespace repo_man.xunit._extensions
{
    /// <summary>
    /// Borrowed from https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq
    /// </summary>
    internal static class MoqLoggerExtensions
    {
        internal static Mock<ILogger<T>> VerifyErrorWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage, Times times)
        {
            Func<object, Type, bool> state = (v, t) => String.Compare(v.ToString(), expectedMessage, StringComparison.Ordinal) == 0;

            logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => state(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), times);

            return logger;
        }
    }
}
