using System;
using System.Drawing;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using repo_man.domain.Diagram;
using repo_man.xunit._helpers;

namespace repo_man.xunit.domain.Diagram
{
    public class SvgFrugalComposerTest : TestBase
    {
        [Theory]
        [InlineData(100, 100, 100, 30)]
        [InlineData(200, 200, 100, 60)]
        public void Sets_Width_And_Height_To_Combined_Dimensions(int chartX, int chartY, int legendX, int legendY)
        {
            var chartData = new ChartData { Data = "<g id=\"chart\" />", Size = new Point(chartX, chartY) };
            var legendData = new LegendData { Data = "<g id=\"legend\" />", Size = new Point(legendX, legendY) };

            var result = WhenIComposeTheSvg(chartData, legendData);

            result.Should().Be(
                $"<svg width=\"{chartX + legendX}\" height=\"{chartY + legendY}\" style=\"background:blanchedalmond;\" xmlns=\"http://www.w3.org/2000/svg\">" +
                "<g id=\"chart\" />" +
                "<g id=\"legend\" />" +
                "</svg>");
        }

        [Theory]
        [InlineData("<g id=\"chart\" />", "<g id=\"legend\" />")]
        [InlineData("<g id=\"alsoAChart\" />", "<g id=\"alsoALegend\" />")]
        public void Sets_Inner_Svg_To_Chart_And_Legend_Data(string chartData, string legendData)
        {
            var chart = new ChartData { Data = chartData, Size = new Point(100, 100) };
            var legend = new LegendData { Data = legendData, Size = new Point(100, 30) };

            var result = WhenIComposeTheSvg(chart, legend);

            result.Should().Be(
                $"<svg width=\"200\" height=\"130\" style=\"background:blanchedalmond;\" xmlns=\"http://www.w3.org/2000/svg\">" +
                chartData + legendData + "</svg>");
        }

        [Theory]
        [InlineData("green")]
        public void User_Can_Change_Background_Color(string color)
        {
            _mocker.GetMock<IConfiguration>().Setup(c => c["background"]).Returns(color);

            var chartData = new ChartData { Data = "<g id=\"chart\" />", Size = new Point(150, 150) };
            var legendData = new LegendData { Data = "<g id=\"legend\" />", Size = new Point(100, 60) };

            var result = WhenIComposeTheSvg(chartData, legendData);

            result.Should().Be(
                $"<svg width=\"250\" height=\"210\" style=\"background:{color};\" xmlns=\"http://www.w3.org/2000/svg\">" +
                "<g id=\"chart\" /><g id=\"legend\" /></svg>"
            );
        }

        private string WhenIComposeTheSvg(ChartData chart, LegendData legend)
        {
            var target = _mocker.CreateInstance<SvgFrugalComposer>();
            var result = target.Compose(chart, legend);
            return result;
        }
    }
}
