using repo_man.domain.Diagram.Calculators;

namespace repo_man.domain.Diagram.FileColorMapper;

public class ConstantFileColorMapper : IFileColorMapper
{
    private readonly BoundedIntCalculator _boundedIntCalculator;


    //TODO: curate this better

    private string[] _extraColors = new[]
    {
        "#FF69B4", // Hot Pink
        "#FF4500", // Orange Red
        "#FF6347", // Tomato
        "#FF7F50", // Coral
        "#FFA07A", // Light Salmon
        "#FF8C00", // Dark Orange
        "#FFD700", // Gold
        "#FFA500", // Orange
        "#FF1493", // Deep Pink
        "#FF00FF", // Magenta
        "#DC143C", // Crimson
        "#FFC0CB", // Pink
    };

    private readonly Dictionary<string, string> _wellKnownFileTypes = new()
    {
        { ".cs", "#A377DA" },   //csharp purple
        { ".cshtml", "#A377DA" },   //csharp purple
        { ".sln", "#FF0000" },   //sln red
        { ".csproj", "#5988C6" },   //csproj slate blue
        { ".js", "#F7E018" },   //javascript yellow
        { ".ejs", "#F7E018" },   //javascript yellow
        { ".jsx", "#F7E018" },   //javascript yellow
        { ".ts", "#3178C6" },   //typescript blue
        { ".tsx", "#3178C6" },   //typescript blue
        { ".css", "#FC5FE7"},    //style sheet pink
        { ".scss", "#FC5FE7"},    //style sheet pink
        { ".less", "#FC5FE7"},    //style sheet pink
        { ".jpg", "#33CC33"},    //image light green
        { ".jpeg", "#33CC33"},    //image light green
        { ".bmp", "#33CC33"},    //image light green
        { ".png", "#33CC33"},    //image light green
        { ".gif", "#33CC33"},    //image light green
        { ".ico", "#33CC33"},    //image light green
        { ".svg", "#00CC66"},    //svg sea green
        { ".json", "#A1A1A1" },   //json light grey
        { ".xml", "#A1A1A1" },   //xml also light grey
        { ".csv", "#A1A1A1" },   //csv also light grey
        { ".bat", "#09B514" },   //shell green
        { ".sh", "#09B514" },   //shell green
        { ".ps1", "#09B514" },   //shell green
        { ".yaml", "#999966" },   //deploy beige
        { ".yml", "#999966" },   //deploy beige
        { ".build_wna", "#999966" },   //deploy beige
        { ".eslintrc", "#996666" },   //js config file copper
        { ".babelrc", "#996666" },   //js config file copper
        { ".prettierrc", "#996666" },   //js config file copper
        { ".prettierignore", "#996666" },   //js config file copper
        { ".nvmrc", "#996666" },   //js config file copper
        { ".env", "#996666" },   //js config file copper
        { ".md", "#FFFFFF" },   //white
        { "dockerfile", "#666666" },   //whale grey
        { ".dockerignore", "#666666" },   //whale grey
        { "codeowners", "#8E5E3C" },   //codeowners brown
        {".gitignore", "#F54D27"},   //git orange
        {".gitattributes", "#F54D27"}   //git orange
    };

    public ConstantFileColorMapper(BoundedIntCalculator boundedIntCalculator)
    {
        _boundedIntCalculator = boundedIntCalculator;
        _boundedIntCalculator.SetBounds(0, 100, 0, 255);
    }

    public string Map(string fileExtension, byte intensity = 100)
    {
        if (!_wellKnownFileTypes.ContainsKey(fileExtension.ToLowerInvariant()))
        {
            _wellKnownFileTypes.Add(fileExtension.ToLowerInvariant(),
                _extraColors[Random.Shared.Next(1, _extraColors.Length)]);
        }

        var baseResult = _wellKnownFileTypes[fileExtension.ToLowerInvariant()];
        if (intensity == 100)
        {
            return baseResult;
        }
        else
        {
            var transparency = _boundedIntCalculator.Calculate(intensity);
            return $"{baseResult}{transparency:X2}";
        }
    }
}