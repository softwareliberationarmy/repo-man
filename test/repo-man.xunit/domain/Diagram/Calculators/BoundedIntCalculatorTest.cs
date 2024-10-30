using repo_man.domain.Diagram.Calculators;

namespace repo_man.xunit.domain.Diagram.Calculators
{
    public class BoundedIntCalculatorTest: TestBase
    {
        private readonly BoundedIntCalculator _target;

        public BoundedIntCalculatorTest()
        {
            _target = _mocker.CreateInstance<BoundedIntCalculator>();
        }

        [Fact]
        public void ThrowsExceptionWhenTryingToCalculateWithoutSettingBounds()
        {
            Action calculateWithoutSettingBounds = () => _target.Calculate(25);
            calculateWithoutSettingBounds.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ReturnsIsNotInitializedWhenNotInitialized()
        {
            _target.IsInitialized.Should().BeFalse();
        }

        [Fact]
        public void ReturnsIsInitializedWhenInitialized()
        {
            _target.SetBounds(0, 100, 1, 10);
            _target.IsInitialized.Should().BeTrue();
        }

        [Fact]
        public void ReturnsMinValueWhenValueFallsBelowMin()
        {
            _target.SetBounds(0, 100, 1, 10);
            var result = _target.Calculate(-1);
            result.Should().Be(1);
        }

        [Fact]
        public void ReturnsMaxValueWhenValueExceedsMax()
        {
            _target.SetBounds(1, 10, 0, 100);
            var result = _target.Calculate(11);
            result.Should().Be(100);
        }

        [Fact]
        public void ReturnsRoundedProportionalValue()
        {
            _target.SetBounds(2, 4, 1, 10);
            var result = _target.Calculate(3);
            result.Should().Be(5);
        }
    }
}
