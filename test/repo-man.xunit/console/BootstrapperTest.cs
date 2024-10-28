using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using repo_man.console;
using repo_man.domain;

namespace repo_man.xunit.console
{
    public class BootstrapperTest
    {
        [Fact]
        public void ThrowsExceptionWhenNoArgsPassedIn()
        {
            Action callWithNoArgs = () =>
            {
                var host = Bootstrapper.BuildHost(Array.Empty<string>());
                host.Services.GetRequiredService<RepoMan>();
            };
            callWithNoArgs.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SuccessfullyInitializesRepositoryVisualizer()
        {
            var host = Bootstrapper.BuildHost(new []{ "diagram", "--repo=c:\\git\\il\\"});
            var target = host.Services.GetRequiredService<RepoMan>();
            target.Should().BeOfType<RepoMan>();
        }

        [Fact]
        public void SuccessfullyCreatesHostWithConfigurationValues()
        {
            var target = Bootstrapper.BuildHost(new[] { "diagram", "--repo=c:\\git\\il\\" });
            var config = target.Services.GetRequiredService<IConfiguration>();
            config["action"].Should().Be("diagram");
            config["repo"].Should().Be("c:\\git\\il\\");
        }
    }
}
