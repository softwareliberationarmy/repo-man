﻿using System.Globalization;
using Microsoft.Extensions.Configuration;
using repo_man.domain.Git;
using repo_man.infrastructure.FileSys;

namespace repo_man.infrastructure.Git;

public class GitLogParser
{
    private readonly WindowsFileSize _fileSizer;
    private readonly string _repoDirectory;
    private Commit? _currentCommit;
    private readonly List<GitLogFileEntry> _entries = new();
    private readonly Dictionary<string,GitLogFileEntry> _entriesByPath = new();

    public GitLogParser(IConfiguration config, WindowsFileSize fileSizer)
    {
        _fileSizer = fileSizer;
        _repoDirectory = config["repo"] ?? "";
    }

    public void Parse(string text)
    {
        if (text.StartsWith("commit "))
        {
            var hash = text.Replace("commit ", "").Trim();
            _currentCommit = new Commit(hash);
        }
        else if (text.StartsWith("Author: "))
        {
            _currentCommit!.Author = text.Replace("Author: ", "").Trim();
        }
        else if(text.StartsWith("Date: "))
        {
            //massage the date
            var dateString = text.Replace("Date: ", "").Trim();
            //insert a colon in the timezone offset
            dateString = dateString.Insert(dateString.Length - 2, ":");

            _currentCommit!.CommitDate = DateTimeOffset.ParseExact(
                dateString,
                "ddd MMM d HH:mm:ss yyyy K",
                CultureInfo.InvariantCulture
            );
        }
        else if (text.StartsWith(" ") && text.Trim().Length > 0)
        {
            _currentCommit!.Message = text.Trim();
        }
        else if (text.StartsWith("M "))
        {
            var path = text.Replace("M ", "").Trim();
            if (!_entriesByPath.ContainsKey(path))
            {
                var entry = new GitLogFileEntry
                {
                    CurrentName = path,
                    FileSize = _fileSizer.GetSize(Path.Combine(_repoDirectory!, path.Replace("/", "\\")))
                };
                _entriesByPath.Add(path, entry);
                _entries.Add(entry);
            }
            _entriesByPath[path].Commits.Add(_currentCommit!);
        }
        else if (text.StartsWith("A "))
        {
            var path = text.Replace("A ", "").Trim();
            if (!_entriesByPath.ContainsKey(path))
            {
                var entry = new GitLogFileEntry
                {
                    CurrentName = path,
                    FileSize = _fileSizer.GetSize(Path.Combine(_repoDirectory!, path.Replace("/", "\\")))
                };
                _entriesByPath.Add(path, entry);
                _entries.Add(entry);
            }
            _entriesByPath[path].Commits.Add(_currentCommit!);
        }
        else if (text.StartsWith("D "))
        {
            var path = text.Replace("D ", "").Trim();
            if (!_entriesByPath.ContainsKey(path))
            {
                var entry = new GitLogFileEntry
                {
                    CurrentName = path,
                    FileSize = 0L
                };
                _entriesByPath.Add(path, entry);
            }
            _entriesByPath[path].Commits.Add(_currentCommit!);
        }
        else if (text.StartsWith("R"))
        {
            var paths = text.Split('\t');
            var newPath = paths[2];
            var oldPath = paths[1];

            if (!_entriesByPath.ContainsKey(newPath))
            {
                var entry = new GitLogFileEntry
                {
                    CurrentName = newPath,
                    FileSize = _fileSizer.GetSize(Path.Combine(_repoDirectory!, newPath.Replace("/", "\\")))
                };
                _entriesByPath.Add(newPath, entry);
                _entries.Add(entry);    //file isn't in the dictionary yet, so it won't be in the list yet
            }
            if (!_entriesByPath.ContainsKey(oldPath))
            {
                _entriesByPath.Add(oldPath, _entriesByPath[newPath]);
                //but don't add old path to the returned files
            }

            _entriesByPath[newPath].Commits.Add(_currentCommit!);
        }
    }

    public IEnumerable<(string, long, Commit[])> GetGitFileData()
    {
        return _entries.Select(x => (x.CurrentName!, x.FileSize, x.Commits.ToArray())).AsEnumerable();
    }

    private class GitLogFileEntry
    {
        public string? CurrentName { get; set; }
        public long FileSize { get; set; }
        public List<Commit> Commits { get; set; } = new List<Commit>();
    }

    //TODO: support complex rhomboidal renaming scenarios (file A renamed to B, ..., file C later assumes A's path, all of the commits for the original A file will be attributed to the file that was originally C)
}