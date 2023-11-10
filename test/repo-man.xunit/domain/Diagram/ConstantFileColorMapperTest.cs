using System.Formats.Tar;
using FluentAssertions;
using repo_man.domain.Diagram;
using repo_man.xunit._helpers;

namespace repo_man.xunit.domain.Diagram
{
    public class ConstantFileColorMapperTest: TestBase
    {
        /// <summary>
        /// more file types: .mk, .sqlite, .ruleset, .txt, .config, .sql, .jwk, .resx, .html,
        /// .ds_store, .properties, .lock, .snap,
        /// textvariables, makefile, .log, .ttf, pre-commit, .pem, .zip, .py, .template, .conf 
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="expectedColor"></param>
        [Theory]
        [InlineData(".cs", "#A377DA")]
        [InlineData(".cshtml", "#A377DA")]
        [InlineData(".CS", "#A377DA")]  //handles different-case for file extension
        [InlineData(".ts", "#3178C6")]
        [InlineData(".tsx", "#3178C6")]
        [InlineData(".js", "#F7E018")]
        [InlineData(".ejs", "#F7E018")]
        [InlineData(".jsx", "#F7E018")]
        [InlineData(".css", "#FC5FE7")]
        [InlineData(".less", "#FC5FE7")]
        [InlineData(".scss", "#FC5FE7")]
        [InlineData(".json", "#A1A1A1")]
        [InlineData(".xml", "#A1A1A1")]
        [InlineData(".csv", "#A1A1A1")]
        [InlineData(".eslintrc", "#996666")]
        [InlineData(".prettierrc", "#996666")]
        [InlineData(".babelrc", "#996666")]
        [InlineData(".nvmrc", "#996666")]
        [InlineData(".env", "#996666")]
        [InlineData(".prettierignore", "#996666")]
        [InlineData(".png", "#33CC33")]
        [InlineData(".jpg", "#33CC33")]
        [InlineData(".bmp", "#33CC33")]
        [InlineData(".gif", "#33CC33")]
        [InlineData(".ico", "#33CC33")]
        [InlineData(".jpeg", "#33CC33")]
        [InlineData(".svg", "#00CC66")]
        [InlineData(".md", "#FFFFFF")]
        [InlineData(".sln", "#FF0000")]
        [InlineData(".csproj", "#5988C6")]
        [InlineData("codeowners","#8E5E3C")]
        [InlineData(".gitignore", "#F54D27")]
        [InlineData(".gitattributes", "#F54D27")]
        [InlineData(".bat", "#09B514")]
        [InlineData(".sh", "#09B514")]
        [InlineData(".ps1", "#09B514")]
        [InlineData(".build_wna", "#999966")]
        [InlineData(".yaml", "#999966")]
        [InlineData(".yml", "#999966")]
        [InlineData("dockerfile", "#666666")]
        [InlineData(".dockerignore", "#666666")]
        public void Returns_Constant_Color_Values_For_Common_File_Types(string extension, string expectedColor)
        {
            IFileColorMapper target = _mocker.CreateInstance<ConstantFileColorMapper>();

            target.Map(extension).Should().Be(expectedColor);
        }

        [Fact]
        public void Returns_Consistent_Color_For_Unknown_Extension()
        {
            var target = _mocker.CreateInstance<ConstantFileColorMapper>();
            var unknownExtension = ".xyz";

            var expectedColor = target.Map(unknownExtension);
            expectedColor.Should().NotBeNullOrWhiteSpace();

            for (int i = 0; i < 100; i++)
            {
                target.Map(unknownExtension).Should().Be(expectedColor);
            }
        }
    }

    public class ConstantFileColorMapper : IFileColorMapper
    {
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

        public string Map(string fileExtension)
        {
            if (!_wellKnownFileTypes.ContainsKey(fileExtension.ToLowerInvariant()))
            {
                _wellKnownFileTypes.Add(fileExtension.ToLowerInvariant(), _extraColors[Random.Shared.Next(1,_extraColors.Length)]);
            }
            return _wellKnownFileTypes[fileExtension.ToLowerInvariant()];
        }
    }
}
