namespace repo_man.domain.CodeQuality;

public class RiskIndexCalculator<T>
{
    public virtual int CalculateRiskIndex(List<Metric<T>> metrics, T record)
    {
        var sumOfWeights = metrics.Sum(m => m.Weight);
        if (Math.Abs(sumOfWeights - 1.0) > 0.00001)
        {
            throw new InvalidOperationException("The sum of the weights of the metrics must be 100");
        }

        var riskScore = 0.0;
        foreach (var metric in metrics)
        {
            var maxValue = metric.MaxValue;
            var value = metric.GetValue(record);
            var normalizedValue = maxValue == 0 ? 0 : (double)value / maxValue;
            riskScore += normalizedValue * metric.Weight;
        }

        var riskIndex = (int)Math.Round(riskScore * 100);
        return riskIndex;
    }
}