namespace repo_man.domain.Git;

public class Commit
{
    public Commit(string hash)
    {
        Hash = hash;
    }

    public Commit(string commitSha, DateTimeOffset committerWhen, string authorName, string commitMessage)
    {
        Hash = commitSha;
        CommitDate = committerWhen;
        Author = authorName;
        Message = commitMessage;
    }

    public string Hash { get; set; }
    public DateTimeOffset? CommitDate { get; set; }
    public string? Author { get; set; }
    public string Message { get; set; } = "";
}