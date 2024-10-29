using Microsoft.Extensions.DependencyInjection;
using repo_man.console;
using repo_man.domain;

var host = Bootstrapper.BuildHost(args);
var repoMan = host.Services.GetRequiredService<RepoMan>();
await repoMan.Run();

Console.WriteLine("*** DON'T FEAR THE REPO! ***");