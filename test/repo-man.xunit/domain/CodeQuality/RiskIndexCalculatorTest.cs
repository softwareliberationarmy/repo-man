using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using repo_man.domain.CodeQuality;

namespace repo_man.xunit.domain.CodeQuality
{
    public class RiskIndexCalculatorTest: TestBase
    {
        [Fact]
        public void ThrowsExceptionIfMetricsWeightDoesntAddUpTo100()
        {
            var metrics = new List<Metric<SonarQubeCodeQualityData>>
            {
                new(getValue: (data) => data.CyclomaticComplexity, maxValue: 100, weight: 0.15)
            };

            Action calculate = () =>
            {
                var target = _mocker.CreateInstance<RiskIndexCalculator<SonarQubeCodeQualityData>>();
                target.CalculateRiskIndex(metrics, new SonarQubeCodeQualityData());
            };

            calculate.Should().Throw<InvalidOperationException>()
                .WithMessage("The sum of the weights of the metrics must be 100");
        }

        [Fact]
        public void CalculatesRiskIndexWithOneMetric()
        {
            var metrics = new List<Metric<SonarQubeCodeQualityData>>
            {
                new(getValue: (data) => data.CyclomaticComplexity, maxValue: 60, weight: 1.0)
            };

            var record = new SonarQubeCodeQualityData
            {
                CyclomaticComplexity = 30
            };

            var target = _mocker.CreateInstance<RiskIndexCalculator<SonarQubeCodeQualityData>>();
            var result = target.CalculateRiskIndex(metrics, record);
            result.Should().Be(50);
        }

        [Fact]
        public void CalculatesRiskIndexWithTwoMetrics()
        {
            var metrics = new List<Metric<SonarQubeCodeQualityData>>
            {
                new(getValue: (data) => data.CyclomaticComplexity, maxValue: 60, weight: 0.5),
                new(getValue: (data) => data.GitCommitCount, maxValue: 20, weight: 0.5)
            };

            var record = new SonarQubeCodeQualityData
            {
                CyclomaticComplexity = 45,
                GitCommitCount = 5
            };

            var target = _mocker.CreateInstance<RiskIndexCalculator<SonarQubeCodeQualityData>>();
            var result = target.CalculateRiskIndex(metrics, record);
            result.Should().Be(50);
        }
    }
}
