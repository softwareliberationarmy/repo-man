using Microsoft.Extensions.Logging;

namespace repo_man.domain.Git
{
    public class GitRepositoryTreeExtracter : ITreeExtracter
    {
        private readonly ILogger<GitRepositoryTreeExtracter> _logger;
        private readonly IGitRepoCrawler _crawler;

        public GitRepositoryTreeExtracter(ILogger<GitRepositoryTreeExtracter> logger, IGitRepoCrawler crawler)
        {
            _logger = logger;
            _crawler = crawler;
        }

        public GitTree GetFileTree()
        {
            _logger.LogInformation("Collecting files from repository");
            var result = new GitTree();
            foreach (var (path, size, commits) in _crawler.GetFilesWithCommits())
            {
                _logger.LogInformation("Adding file {path} - {size} bytes - {commits} commits", path, size, commits.Length);
                result.AddFile(path, size, commits);
            }

            return result;
        }
    }
}
