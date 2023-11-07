using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using repo_man.domain.Git;
using Commit = repo_man.domain.Git.Commit;
using Tree = LibGit2Sharp.Tree;

namespace repo_man.infrastructure
{
    public class LibGit2SharpGitRepoCrawler: IGitRepoCrawler
    {
        private readonly ILogger<LibGit2SharpGitRepoCrawler> _logger;
        private readonly IConfiguration _config;

        public LibGit2SharpGitRepoCrawler(ILogger<LibGit2SharpGitRepoCrawler> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IEnumerable<(string, long, Commit[])> GitThemFiles(bool includeCommits = false)
        {
            var repoPath = _config["repo"];
            if (string.IsNullOrEmpty(repoPath))
            {
                _logger.LogError("Unable to continue without repo path. Configuration: {@config}", _config);
                throw new InvalidOperationException("Repo path not specified.");
            }

            _logger.LogInformation("Crawling git repo at {path}", repoPath);
            using var repo = new Repository(repoPath);
            var tree = repo.Head.Tip.Tree;

            return ParseTree(tree, repo, includeCommits);
        }

        private IEnumerable<(string, long, Commit[])> ParseTree(Tree tree, Repository repo, bool includeCommits)
        {
            var result = new List<(string, long, Commit[])>();

            foreach (var entry in tree)
            {
                if (entry.TargetType == TreeEntryTargetType.Tree)
                {
                    _logger.LogInformation("Crawling directory {path}", entry.Path);
                    foreach (var valueTuple in ParseTree((Tree)entry.Target, repo, includeCommits))
                    {
                        result.Add(valueTuple);
                    }
                }
                else if (entry.TargetType == TreeEntryTargetType.Blob)
                {
                    Commit[] history = Array.Empty<Commit>();
                    if (includeCommits)
                    {
                        var commits = repo.Commits.QueryBy(entry.Path, new CommitFilter { SortBy = CommitSortStrategies.Topological }).ToArray();
                        if (commits.Any())
                        {
                            history = commits.Select(
                                c => new Commit(c.Commit.Sha, c.Commit.Committer.When)).ToArray();
                        }
                    }
                    var size = ((Blob)entry.Target).Size;

                    result.Add((entry.Path, size, history));
                }
            }

            return result;
        }
    }
}
