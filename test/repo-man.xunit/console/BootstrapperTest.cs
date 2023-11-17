using repo_man.console;
using repo_man.domain;

namespace repo_man.xunit.console
{
    public class BootstrapperTest
    {
        [Fact]
        public void SuccessfullyInitializesRepositoryVisualizer()
        {
            var target = Bootstrapper.InitializeToTopLevelService<RepositoryVisualizer>(Array.Empty<string>());
            target.Should().BeOfType<RepositoryVisualizer>();
        }
    }
}
