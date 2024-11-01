using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using repo_man.domain.CodeQuality;

namespace repo_man.infrastructure.CodeQuality
{
    public class SonarQubeCodeQualityDataSource: ICodeQualityDataSource<SonarQubeCodeQualityData>
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;

        public SonarQubeCodeQualityDataSource(IConfiguration config, HttpClient client)
        {
            _config = config;
            _client = client;
        }

        public async Task<List<SonarQubeCodeQualityData>> GetCodeQualityData()
        {
            var result = new List<SonarQubeCodeQualityData>();
            var url = _config["sonarqube:url"];
            var token = _config["sonarqube:token"];
            var project = _config["sqProjectKey"];

            // Example of using HttpClient to make a request
            _client.BaseAddress = new Uri(url!);
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            int page = 0;

            SonarQubeResult? data;
            do
            {
                page++;
                var requestUri = $"/api/measures/component_tree?component={project}&metricKeys=complexity,code_smells,critical_violations,major_violations,violations&p={page}&ps=500&qualifiers=FIL";
                var response = await _client.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                data = await JsonSerializer.DeserializeAsync<SonarQubeResult>(await response.Content.ReadAsStreamAsync(CancellationToken.None));

                result.AddRange(ParseData(data!));
            }
            while(data!.Paging!.Total > (data!.Paging!.PageIndex * data!.Paging!.PageSize));

            return result;
        }

        private IEnumerable<SonarQubeCodeQualityData> ParseData(SonarQubeResult data)
        {
            foreach (var component in data.Components!)
            {
                var path = component.Path!;
                var complexity = component.Measures.FirstOrDefault(x => x.Metric == "complexity")?.Value ?? "0";
                var codeSmells = component.Measures.First(x => x.Metric == "code_smells")?.Value ?? "0";
                var criticalViolations = component.Measures.First(x => x.Metric == "critical_violations")?.Value ?? "0";
                var majorViolations = component.Measures.First(x => x.Metric == "major_violations")?.Value ?? "0";
                var allViolations = component.Measures.First(x => x.Metric == "violations")?.Value ?? "0";
                yield return new SonarQubeCodeQualityData
                {
                    FilePath = path,
                    CyclomaticComplexity = int.Parse(complexity!),
                    CodeSmells = int.Parse(codeSmells!),
                    CriticalViolations = int.Parse(criticalViolations!),
                    MajorViolations = int.Parse(majorViolations!),
                    AllViolations = int.Parse(allViolations!)
                };
            }
        }
    }

    public class SonarQubeResult
    {
        public PagingData? Paging { get; set; }

        public List<ComponentData>? Components { get; set; } = new();

    }

    public class PagingData
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }

    public class ComponentData
    {
        public string? Path { get; set; }
        public List<ComponentMeasureData> Measures { get; set; } = new();

    }

    public class ComponentMeasureData
    {
        public string? Metric { get; set; }
        public string? Value { get; set; }
    }
}
