using Microsoft.Extensions.DependencyInjection;
using repo_man.console;
using repo_man.domain;

var host = Bootstrapper.BuildHost(args);
var visualizer = host.Services.GetRequiredService<RepositoryVisualizer>();
await visualizer.GenerateDiagram();

