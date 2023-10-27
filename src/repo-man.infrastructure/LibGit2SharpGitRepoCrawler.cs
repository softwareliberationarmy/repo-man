using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using repo_man.domain.Git;
using Commit = repo_man.domain.Git.Commit;

namespace repo_man.infrastructure
{
    public class LibGit2SharpGitRepoCrawler: IGitRepoCrawler
    {
        private readonly IConfiguration _config;

        public LibGit2SharpGitRepoCrawler(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<(string, long, Commit[])> GetFilesWithCommits()
        {
            var repoPath = _config["repo"];
            if (string.IsNullOrEmpty(repoPath))
            {
                throw new InvalidOperationException("Repo path not specified.");
            }

            using var repo = new Repository(repoPath);
            var tree = repo.Head.Tip.Tree;

            return ParseTree(tree, repo);
        }

        private IEnumerable<(string, long, Commit[])> ParseTree(Tree tree, Repository? repo)
        {
            var result = new List<(string, long, Commit[])>();

            foreach (var entry in tree)
            {
                if (entry.TargetType == TreeEntryTargetType.Tree)
                {
                    foreach (var valueTuple in ParseTree((Tree)entry.Target, repo))
                    {
                        result.Add(valueTuple);
                    }
                }
                else if (entry.TargetType == TreeEntryTargetType.Blob)
                {
                    var history = repo!.Commits.QueryBy(entry.Path).Select(
                        c => new Commit(c.Commit.Sha, c.Commit.Committer.When)).ToArray();
                    var size = ((Blob)entry.Target).Size;

                    result.Add((entry.Path, size, history));
                }
            }

            return result;
        }
    }
}
