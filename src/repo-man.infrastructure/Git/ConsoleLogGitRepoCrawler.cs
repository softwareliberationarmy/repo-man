using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using repo_man.domain.Git;

namespace repo_man.infrastructure.Git
{
    public class ConsoleLogGitRepoCrawler: IGitRepoCrawler
    {
        private readonly ILogger<ConsoleLogGitRepoCrawler> _logger;
        private readonly IConfiguration _config;
        private readonly GitLogParser _parser;

        public ConsoleLogGitRepoCrawler(ILogger<ConsoleLogGitRepoCrawler> logger, IConfiguration config, GitLogParser parser)
        {
            _logger = logger;
            _config = config;
            _parser = parser;
        }

        public IEnumerable<(string, long, Commit[])> GitThemFiles()
        {
            var repoDir = _config["repo"]!;
            var gitLogCommand = "git log --name-status";

            _logger.LogInformation($"Calling {gitLogCommand} on repo at {repoDir}");
            var processInfo = new ProcessStartInfo("cmd.exe", $"/c cd {repoDir} && {gitLogCommand}")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                using (var reader = process!.StandardOutput)
                {
                    string? result = reader.ReadLine();
                    while (result != null)
                    {
                        _parser.Parse(result);
                        result = reader.ReadLine();
                    }
                }
            }

            var gitFiles = _parser.GetGitFileData();

            if (!string.IsNullOrEmpty(_config["ignoreFileTypes"]))
            {
                var fileTypes = _config["ignoreFileTypes"]!.Split('|');
                gitFiles = gitFiles.Where(gf => !fileTypes.Any(ft => gf.Item1.EndsWith($".{ft}")));
            }
            return gitFiles;
        }
    }
}
