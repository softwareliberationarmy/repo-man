using Microsoft.Extensions.Logging;
using Moq;

namespace repo_man.xunit._extensions
{
    /// <summary>
    /// Borrowed from https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq
    /// </summary>
    internal static class MoqLoggerExtensions
    {
        internal static Mock<ILogger<T>> VerifyInfoWasCalled<T>(this Mock<ILogger<T>> logger, string expectedMessage, Times times)
        {
            Func<object, Type, bool> state = (v, t) => String.Compare(v.ToString(), expectedMessage, StringComparison.Ordinal) == 0;

            VerifyLoggingHappened(logger, times, LogLevel.Information, state);

            return logger;
        }

        internal static Mock<ILogger<T>> VerifyErrorWasCalled<T>(this Mock<ILogger<T>> logger, Func<string,bool> expectedMessage, Times times)
        {
            Func<object, Type, bool> state = (v, t) => expectedMessage(v.ToString()!);

            VerifyLoggingHappened(logger, times, LogLevel.Error, state);

            return logger;
        }

        private static void VerifyLoggingHappened<T>(Mock<ILogger<T>> logger, Times times, LogLevel logLevel, Func<object, Type, bool> state)
        {
            logger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => state(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), times);
        }
    }
}
