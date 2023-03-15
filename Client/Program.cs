using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DataMigrationApp.Client;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

const string HttpClientName = "Site";

builder.Services.AddHttpClient(HttpClientName, (sp, http) =>
{
    http.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<ISubscriptionsClient>(HttpClientName)
.AddTypedClient<ISubscriptionsClient>((http, sp) => new SubscriptionsClient(http));

builder.Services.AddHttpClient<ICustomersClient>(HttpClientName)
.AddTypedClient<ICustomersClient>((http, sp) => new CustomersClient(http));

builder.Services.AddHttpClient<IMigrationClient>(HttpClientName)
.AddTypedClient<IMigrationClient>((http, sp) => new MigrationClient(http));

await builder.Build().RunAsync();