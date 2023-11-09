using FluentAssertions;
using repo_man.domain.Diagram;
using repo_man.xunit._helpers;

namespace repo_man.xunit.domain.Diagram
{
    public class ConstantFileColorMapperTest: TestBase
    {
        /// <summary>
        /// more file types: .mk, dockerfile, .build_wna, dockerfile, .yaml/.yml,
        /// .sqlite, .dockerignore, .ruleset, .txt, .config, .sql, .jwk, .cshtml, .resx, .html,
        /// .ds_store, .bat, .sh, .ps1, .properties, .lock, .snap,
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
        public void Returns_Constant_Color_Values_For_Common_File_Types(string extension, string expectedColor)
        {
            IFileColorMapper target = _mocker.CreateInstance<ConstantFileColorMapper>();

            target.Map(extension).Should().Be(expectedColor);
        }
    }

    public class ConstantFileColorMapper : IFileColorMapper
    {
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
            { ".json", "#A1A1A1" },   //json grey
            { ".xml", "#A1A1A1" },   //xml also grey
            { ".csv", "#A1A1A1" },   //csv also grey
            { ".eslintrc", "#996666" },   //js config file copper
            { ".babelrc", "#996666" },   //js config file copper
            { ".prettierrc", "#996666" },   //js config file copper
            { ".prettierignore", "#996666" },   //js config file copper
            { ".nvmrc", "#996666" },   //js config file copper
            { ".env", "#996666" },   //js config file copper
            { ".md", "#FFFFFF" },   //white
            { "codeowners", "#8E5E3C" },   //codeowners brown
            {".gitignore", "#F54D27"},   //git orange
            {".gitattributes", "#F54D27"}   //git orange
    };

        public string Map(string fileExtension)
        {
            return _wellKnownFileTypes[fileExtension.ToLowerInvariant()];
        }
    }
}
