namespace repo_man.domain.Git;

public record GitFile(string Name, long FileSize, IReadOnlyCollection<Commit> Commits);