using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using LibGit2Sharp;
using repo_man.domain.Diagram;
using repo_man.domain.Git;
using repo_man.xunit._helpers;
using Commit = repo_man.domain.Git.Commit;

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

    public class SvgLegendDataWriter
    {
        private readonly IFileColorMapper _colorMapper;

        public SvgLegendDataWriter(IFileColorMapper colorMapper)
        {
            _colorMapper = colorMapper;
        }

        public LegendData WriteLegendData(GitTree tree, Point startingPoint)
        {
            var builder = new StringBuilder();
            builder.Append($"<g transform=\"translate({startingPoint.X}, {startingPoint.Y})\">");

            var extensions = new SortedSet<string>();
            foreach (var file in tree.Files)
            {
                CountFileExtension(file, extensions);
            }

            InspectFolders(tree.Folders, extensions);

            var y = 0;
            foreach (var extension in extensions)
            {
                builder.Append($"<g transform=\"translate(0, {y})\">");
                builder.Append($"<circle r=\"5\" fill=\"{_colorMapper.Map(extension)}\"></circle>");
                builder.Append(
                    $"<text x=\"10\" style=\"font-size:14px;font-weight:300\" dominant-baseline=\"middle\">{extension}</text>");
                builder.Append("</g>");
            }
            builder.Append("</g>");

            return new LegendData
            {
                Data = builder.ToString()
            };
        }

        private static void InspectFolders(IReadOnlyCollection<GitFolder> readOnlyCollection, SortedSet<string> extensions)
        {
            foreach (var folder in readOnlyCollection)
            {
                foreach (var file in folder.Files)
                {
                    CountFileExtension(file, extensions);
                }
                InspectFolders(folder.Folders, extensions);
            }
        }

        private static void CountFileExtension(GitFile file, SortedSet<string> extensions)
        {
            extensions.Add(file.Name.GetFileExtension());
        }

    }
}
