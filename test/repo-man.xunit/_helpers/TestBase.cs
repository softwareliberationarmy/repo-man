using AutoFixture;
using Moq.AutoMock;

namespace repo_man.xunit._helpers
{
    public class TestBase
    {
        protected Fixture _fixture;
        protected AutoMocker _mocker;
        public TestBase()
        {
            _fixture = new Fixture();
            _mocker = new AutoMocker();
        }
    }
}
