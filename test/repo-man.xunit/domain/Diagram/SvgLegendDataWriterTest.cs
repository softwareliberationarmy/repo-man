using System.Drawing;
using FluentAssertions;
using repo_man.domain.Diagram;
using repo_man.domain.Git;
using repo_man.xunit._helpers;

namespace repo_man.xunit.domain.Diagram
{
    public class SvgLegendDataWriterTest: TestBase
    {
        [Fact]
        public void Single_Cs_File()
        {
            var extension = ".cs";
            var expectedColor = "red";
            var startingPoint = new Point(100, 100);

            GivenTheseColorMappings(new Tuple<string, string>(extension, expectedColor));
            var tree = GivenThisFileTree(new Tuple<string, long>("src/Program.cs", 1000L));
            var target = _mocker.CreateInstance<SvgLegendDataWriter>();

            var data = target.WriteLegendData(tree, startingPoint);

            data.Data.Should().Be($"<g transform=\"translate({100}, {100})\">" +
                                  "<g transform=\"translate(0, 0)\">" +
                                  $"<circle r=\"5\" fill=\"{expectedColor}\"></circle>" +
                                  $"<text x=\"10\" style=\"font-size:14px;font-weight:300\" dominant-baseline=\"middle\">{extension}</text>" +
                                  $"</g>" +
                                  $"</g>");
        }

        [Fact]
        public void Cs_And_Md_Files()
        {
            var startingPoint = new Point(100, 100);

            GivenTheseColorMappings(new Tuple<string, string>(".cs", "red"),
                new Tuple<string, string>(".md", "green"));
            var tree = GivenThisFileTree(new Tuple<string, long>("src/Program.cs", 1000L),
                new Tuple<string, long>("ReadMe.md", 200L));
            var target = _mocker.CreateInstance<SvgLegendDataWriter>();

            var data = target.WriteLegendData(tree, startingPoint);

            data.Data.Should().Be($"<g transform=\"translate({100}, {100})\">" +
                                  "<g transform=\"translate(0, 0)\">" +
                                  $"<circle r=\"5\" fill=\"red\"></circle>" +
                                  $"<text x=\"10\" style=\"font-size:14px;font-weight:300\" dominant-baseline=\"middle\">.cs</text>" +
                                  $"</g>" +
                                  "<g transform=\"translate(0, 15)\">" +
                                  $"<circle r=\"5\" fill=\"green\"></circle>" +
                                  $"<text x=\"10\" style=\"font-size:14px;font-weight:300\" dominant-baseline=\"middle\">.md</text>" +
                                  $"</g>" +
                                  $"</g>");
        }
        
        private static GitTree GivenThisFileTree(params Tuple<string, long>[] files)
        {
            var tree = new GitTree();
            foreach (var file in files)
            {
                tree.AddFile(file.Item1, file.Item2, Array.Empty<Commit>());
            }

            return tree;
        }
        
        private void GivenTheseColorMappings(params Tuple<string, string>[] mappings)
        {
            foreach (var mapping in mappings)
            {
                _mocker.GetMock<IFileColorMapper>().Setup(x => x.Map(mapping.Item1)).Returns(mapping.Item2);
            }
        }
    }
}
