namespace repo_man.domain.CodeQuality;

public interface ICodeQualityDataSource<T>
{
    Task<List<T>> GetCodeQualityData();
}