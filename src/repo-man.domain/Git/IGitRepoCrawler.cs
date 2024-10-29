namespace repo_man.domain.Git;

public interface IGitRepoCrawler
{
    IEnumerable<(string,long, Commit[])> GitThemFiles();
}