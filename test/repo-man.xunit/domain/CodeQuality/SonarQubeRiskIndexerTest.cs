using Moq;
using repo_man.domain.CodeQuality;

namespace repo_man.xunit.domain.CodeQuality
{
    public class SonarQubeRiskIndexerTest: TestBase
    {
        [Fact]
        public async Task SetsTheMetricsCorrectly()
        {
            var file1 = "/src/file1.cs";
            var file2 = "/src/file2.cs";
            var tree = new GitTree();
            tree.AddFile(file1, 1000, new Commit[]{ new Commit("one")});
            tree.AddFile(file2, 500, new Commit[]{ new Commit("one"), new Commit("two")});

            _mocker.GetMock<ICodeQualityDataSource<SonarQubeCodeQualityData>>()
                .Setup(x => x.GetCodeQualityData())
                .ReturnsAsync(
                    new List<SonarQubeCodeQualityData>
                    {
                        new()
                        {
                            FilePath = file1,
                            AllViolations = 10,
                            CriticalViolations = 1,
                            MajorViolations = 2,
                            CodeSmells = 3,
                            CyclomaticComplexity = 100
                        },
                        new()
                        {
                            FilePath = file2,
                            AllViolations = 20,
                            CriticalViolations = 0,
                            MajorViolations = 4,
                            CodeSmells = 5,
                            CyclomaticComplexity = 200
                        }
                    });

            List<Metric<SonarQubeCodeQualityData>> passedInMetrics = null!;
            _mocker.GetMock<RiskIndexCalculator<SonarQubeCodeQualityData>>()
                .Setup(x => x.CalculateRiskIndex(It.IsAny<List<Metric<SonarQubeCodeQualityData>>>(),
                    It.IsAny<SonarQubeCodeQualityData>()))
                .Returns<List<Metric<SonarQubeCodeQualityData>>, SonarQubeCodeQualityData>((metrics, record) =>
                {
                    passedInMetrics = metrics;
                    return 79;
                });

            var target = _mocker.CreateInstance<SonarQubeRiskIndexer>();
            await target.DecorateTreeWithRiskIndex(tree);

            passedInMetrics!.Should().NotBeNull();
            passedInMetrics!.Count.Should().Be(7);
            passedInMetrics.First(x => x.Name == "CodeSmells").MaxValue.Should().Be(5);
            passedInMetrics.First(x => x.Name == "CriticalViolations").MaxValue.Should().Be(1);
            passedInMetrics.First(x => x.Name == "MajorViolations").MaxValue.Should().Be(4);
            passedInMetrics.First(x => x.Name == "AllViolations").MaxValue.Should().Be(20);
            passedInMetrics.First(x => x.Name == "CyclomaticComplexity").MaxValue.Should().Be(200);
            passedInMetrics.First(x => x.Name == "FileSizeInBytes").MaxValue.Should().Be(1000);
            passedInMetrics.First(x => x.Name == "GitCommitCount").MaxValue.Should().Be(2);

            tree.GetAllFiles().Select(x => x.RiskIndex).Distinct().Single().Should().Be(79);
        }
    }
}


// get risk data from SonarQube into a DTO list
// get the max values from the DTO list and the git tree
// for each file in the tree, find the corresponding DTO
// if the DTO exists, add the git commits and calculate the risk index

