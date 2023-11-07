using FluentAssertions;
using Moq.AutoMock;
using repo_man.domain.Diagram;

namespace repo_man.xunit.domain.Diagram
{
    public class ConstantFileColorMapperTest
    {
        /// <summary>
        /// more file types: .ejs, .mk, .scss, .png, .svg, .jpg, .bmp, dockerfile, .build_wna, dockerfile, .yaml/.yml,
        /// .sqlite, .xml, .dockerignore, .ruleset, .sh, .txt, .config, .sql, .jwk, .cshtml, .css, .ico, .resx, .html,
        /// .ds_store, .bat, .properties, .lock, .eslintrc, .prettierignore, .prettierrc, .babelrc, .snap, .gif,
        /// textvariables, .nvmrc, .env, makefile, .log, .csv, .ttf, pre-commit, .pem, .zip, .py, .ps1, .template, .conf 
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="expectedColor"></param>
        [Theory]
        [InlineData(".cs", "#A377DA")]
        [InlineData(".CS", "#A377DA")]  //handles different-case for file extension
        [InlineData(".ts", "#3178C6")]
        [InlineData(".js", "#F7E018")]
        [InlineData(".json", "#A1A1A1")]
        [InlineData(".md", "#FFFFFF")]
        [InlineData(".sln", "#FF0000")]
        [InlineData(".csproj", "#5988C6")]
        [InlineData("codeowners","#8E5E3C")]
        [InlineData(".gitignore", "#F54D27")]
        [InlineData(".gitattributes", "#F54D27")]
        [InlineData(".jsx", "#C6BE21")]
        [InlineData(".tsx", "#F442E2")]
        public void Returns_Constant_Color_Values_For_Common_File_Types(string extension, string expectedColor)
        {
            var mocker = new AutoMocker();
            IFileColorMapper target = mocker.CreateInstance<ConstantFileColorMapper>();

            target.Map(extension).Should().Be(expectedColor);
        }
    }

    public class ConstantFileColorMapper : IFileColorMapper
    {
        private readonly Dictionary<string, string> _wellKnownFileTypes = new()
        {
            { ".cs", "#A377DA" },   //csharp purple
            { ".sln", "#FF0000" },   //sln red
            { ".csproj", "#5988C6" },   //csproj slate blue
            { ".js", "#F7E018" },   //javascript yellow
            { ".jsx", "#C6BE21" },   //jsx gold
            { ".ts", "#3178C6" },   //typescript blue
            { ".tsx", "#F442E2" },   //tsx pink
            { ".json", "#A1A1A1" },   //json grey
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
