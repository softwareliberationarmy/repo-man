﻿namespace repo_man.domain.Git
{
    public class GitRepositoryTreeExtracter : ITreeExtracter
    {
        private readonly IGitRepoCrawler _crawler;

        public GitRepositoryTreeExtracter(IGitRepoCrawler crawler)
        {
            _crawler = crawler;
        }

        public Tree GetFileTree()
        {
            var result = new Tree();
            foreach (var (path, size, commits) in _crawler.GetFilesWithCommits())
            {
                result.AddFile(path, size, commits);
            }

            return result;
        }
    }
}
