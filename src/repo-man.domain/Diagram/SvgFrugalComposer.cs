using Microsoft.Extensions.Configuration;

namespace repo_man.domain.Diagram
{
    public class SvgFrugalComposer: ISvgComposer
    {
        private const string DefaultBackground = "blanchedalmond";
        private readonly IConfiguration _configuration;

        public SvgFrugalComposer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Compose(ChartData chartData, LegendData legendData)
        {
            var background = _configuration["background"] ?? DefaultBackground;
            var width = chartData.Size.X + legendData.Size.X;
            var height = chartData.Size.Y + legendData.Size.Y;

            return
                $"<svg width=\"{width}\" height=\"{height}\" style=\"background:{background};\" xmlns=\"http://www.w3.org/2000/svg\">" +
                chartData.Data +
                legendData.Data +
                "</svg>";
        }
    }
}
