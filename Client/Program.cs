using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DataMigrationApp.Client;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("WebAPI"));

builder.Services.AddHttpClient<ISubscriptionsClient>(nameof(ISubscriptionsClient), (sp, http) =>
{
    http.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
})
.AddTypedClient<ISubscriptionsClient>((http, sp) => new SubscriptionsClient(http));

builder.Services.AddHttpClient<ICustomersClient>(nameof(ICustomersClient), (sp, http) =>
{
    http.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
})
.AddTypedClient<ICustomersClient>((http, sp) => new CustomersClient(http));

builder.Services.AddHttpClient<IMigrationClient>(nameof(IMigrationClient), (sp, http) =>
{
    http.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
})
.AddTypedClient<IMigrationClient>((http, sp) => new MigrationClient(http));

await builder.Build().RunAsync();