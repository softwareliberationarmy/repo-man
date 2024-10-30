using repo_man.domain.Diagram.FileColorMapper;

namespace repo_man.xunit.domain.Diagram.FileColorMapper
{
    public class ConstantFileColorMapperTest : TestBase
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
        [InlineData("codeowners", "#8E5E3C")]
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

        [Theory]
        [InlineData(".md", 50, "#FFFFFF80")]
        [InlineData(".md", 0, "#FFFFFF00")]
        [InlineData(".md", 100, "#FFFFFF")]
        public void Returns_Partially_Transparent_Color_For_Files_With_Less_Intensity(string extension, byte intensity, string expected)
        {
            var target = _mocker.CreateInstance<ConstantFileColorMapper>();
            target.Map(extension, intensity).Should().Be(expected);
        }
    }
}
