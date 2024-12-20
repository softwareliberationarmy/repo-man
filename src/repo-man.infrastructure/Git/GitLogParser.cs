﻿using System.Globalization;
using Microsoft.Extensions.Configuration;
using repo_man.domain.FileSystem;
using repo_man.domain.Git;
using repo_man.infrastructure.FileSys;

namespace repo_man.infrastructure.Git;

public class GitLogParser
{
    private readonly IFileSystem _fileSystem;
    private readonly string _repoDirectory;
    private Commit? _currentCommit;
    private readonly List<GitLogFileEntry> _entries = new();
    private readonly Dictionary<string,GitLogFileEntry> _entriesByPath = new();

    public GitLogParser(IConfiguration config, IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _repoDirectory = config["repo"] ?? "";
    }

    public void Parse(string text)
    {
        if (text.StartsWith("commit "))
        {
            ProcessCommitHash(text);
        }
        else if (text.StartsWith("Author: "))
        {
            ProcessCommitAuthor(text);
        }
        else if(text.StartsWith("Date: "))
        {
            ProcessCommitDate(text);
        }
        else if (text.StartsWith(" ") && text.Trim().Length > 0)
        {
            ProcessCommitMessage(text);
        }
        else if (text.StartsWith("M\t"))
        {
            ProcessModifiedFile(text);
        }
        else if (text.StartsWith("A\t"))
        {
            ProcessAddedFile(text);
        }
        else if (text.StartsWith("D\t"))
        {
            ProcessDeletedFile(text);
        }
        else if (text.StartsWith("R"))
        {
            ProcessFileRename(text);
        }
    }

    private void ProcessFileRename(string text)
    {
        var paths = text.Split('\t');
        var newPath = paths[2];
        var oldPath = paths[1];

        if (!_entriesByPath.ContainsKey(newPath))
        {
            var entry = new GitLogFileEntry
            {
                CurrentName = newPath,
                FileSize = _fileSystem.GetFileSize(Path.Combine(_repoDirectory!, newPath.Replace("/", "\\")))
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

    private void ProcessDeletedFile(string text)
    {
        var path = text.Replace("D\t", "").Trim();
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

    private void ProcessAddedFile(string text)
    {
        var path = text.Replace("A\t", "").Trim();
        if (!_entriesByPath.ContainsKey(path))
        {
            var entry = new GitLogFileEntry
            {
                CurrentName = path,
                FileSize = _fileSystem.GetFileSize(Path.Combine(_repoDirectory!, path.Replace("/", "\\")))
            };
            _entriesByPath.Add(path, entry);
            _entries.Add(entry);
        }
        _entriesByPath[path].Commits.Add(_currentCommit!);
    }

    private void ProcessModifiedFile(string text)
    {
        var path = text.Replace("M\t", "").Trim();
        if (!_entriesByPath.ContainsKey(path))
        {
            var entry = new GitLogFileEntry
            {
                CurrentName = path,
                FileSize = _fileSystem.GetFileSize(Path.Combine(_repoDirectory!, path.Replace("/", "\\")))
            };
            _entriesByPath.Add(path, entry);
            _entries.Add(entry);
        }
        _entriesByPath[path].Commits.Add(_currentCommit!);
    }

    private void ProcessCommitMessage(string text)
    {
        if (_currentCommit!.Message.Length > 0)
        {
            _currentCommit!.Message += "\r\n";
        }
        _currentCommit!.Message += text.Trim();
    }

    private void ProcessCommitDate(string text)
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

    private void ProcessCommitAuthor(string text)
    {
        _currentCommit!.Author = text.Replace("Author: ", "").Trim();
    }

    private void ProcessCommitHash(string text)
    {
        var hash = text.Replace("commit ", "").Trim();
        _currentCommit = new Commit(hash);
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