namespace repo_man.domain.Diagram.Calculators;

public class BoundedIntCalculator
{
    private long? _minInput;
    private long? _maxInput;
    private int? _minOutput;
    private int? _maxOutput;
    public bool IsInitialized { get; set; }

    public int Calculate(long input)
    {
        if (_minInput == null || _maxInput == null || _minOutput == null || _maxOutput == null)
        {
            throw new InvalidOperationException("Cannot calculate a relative value without min and max bounds");
        }

        if (input <= _minInput)
        {
            return _minOutput.Value;
        }

        if(input >= _maxInput)
        {
            return _maxOutput.Value;
        }

        var percent = (double)(input - _minInput.Value) / (_maxInput.Value - _minInput.Value);
        var increment = (int)Math.Round(percent * (_maxOutput.Value - _minOutput.Value));
        return _minOutput.Value + increment;
    }

    public void SetBounds(long minInput, long maxInput, int minOutput, int maxOutput)
    {
        _minInput = minInput;
        _maxInput = maxInput;
        _minOutput = minOutput;
        _maxOutput = maxOutput;
        IsInitialized = true;
    }
}