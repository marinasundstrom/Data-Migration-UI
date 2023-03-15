﻿using Asp.Versioning;
using DataMigrationApp.Server;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.ResponseCompression;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddRazorPages();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
      .AddApiExplorer(option =>
      {
          option.GroupNameFormat = "VVV";
          option.SubstituteApiVersionInUrl = true;
      })
       .EnableApiVersionBinding();

// Register the Swagger services

var apiVersionDescriptions = new[] {
            (ApiVersion: new ApiVersion(1, 0), foo: 1),
            (ApiVersion: new ApiVersion(2, 0), foo: 1)
        };

foreach (var description in apiVersionDescriptions)
{
    builder.Services.AddOpenApiDocument(config =>
    {
        config.DocumentName = $"v{GetApiVersion(description)}";
        config.PostProcess = document =>
        {
            document.Info.Title = "Data Migration API";
            document.Info.Version = $"v{GetApiVersion(description)}";
        };
        config.ApiGroupNames = new[] { GetApiVersion(description) };

        config.DefaultReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.NotNull;

        config.AddSecurity("JWT", new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Type into the textbox: Bearer {your JWT token}."
        });

        config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));

        //config.SchemaNameGenerator = new CustomSchemaNameGenerator();
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapEndpoints();

app.UseOpenApi();

app.UseSwaggerUi3(options =>
{
    var descriptions = app.DescribeApiVersions();

    // build a swagger endpoint for each discovered API version
    foreach (var description in descriptions)
    {
        var name = $"v{description.ApiVersion}";
        var url = $"/swagger/v{GetApiVersion(description)}/swagger.json";

        options.SwaggerRoutes.Add(new SwaggerUi3Route(name, url));
    }

    static string GetApiVersion(Asp.Versioning.ApiExplorer.ApiVersionDescription description)
    {
        var apiVersion = description.ApiVersion;
        return (apiVersion.MinorVersion == 0
            ? apiVersion.MajorVersion.ToString()
            : apiVersion.ToString())!;
    }
});

app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.Run();

static string GetApiVersion((ApiVersion ApiVersion, int foo) description)
{
    var apiVersion = description.ApiVersion;
    return (apiVersion.MinorVersion == 0
        ? apiVersion.MajorVersion.ToString()
        : apiVersion.ToString())!;
}