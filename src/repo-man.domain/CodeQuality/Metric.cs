namespace repo_man.domain.CodeQuality;

public class Metric<T>
{
    public Metric(Func<T, long> getValue, long maxValue, double weight, string name = "")
    {
        GetValue = getValue;
        MaxValue = maxValue;
        Weight = weight;
        Name = name;
    }

    public Func<T, long> GetValue { get; set; }
    public long MaxValue { get; set; }
    public double Weight { get; set; }
    public string Name { get; set; }
}