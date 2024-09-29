// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Azure.Storage.Files.Shares;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Sas;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

builder.Services.AddMudServices(options =>
{
    options.PopoverOptions.ThrowOnDuplicateProvider = false;
});
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ShoppingCartService>();
builder.Services.AddSingleton<InventoryService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<PhotoService>();
builder.Services.AddScoped<ComponentStateChangedObserver>();
builder.Services.AddSingleton<ToastService>();
builder.Services.AddLocalStorageServices();
//builder.Services.AddDbContextFactory<SunfoxPrintingContext>(
//    options => 
//        options.UseNpgsql(confi)
//          IConfigurationRoot configuration = new ConfigurationBuilder()
//           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
//           .AddJsonFile("appsettings.json")
//           .Build();
//optionsBuilder.UseNpgsql("Name=SunfoxPrintingContext:ConnectionString");
//optionsBuilder.UseNpgsql(configuration.GetConnectionString("SunfoxPrintingContext: ConnectionString"));

//    );
//builder.Services.AddSingleton<SunfoxPrintingContext>();

//builder.Services.AddSingleton<ShareClient>(
//    options =>
//        options
//    );



if (builder.Environment.IsDevelopment())
{
    builder.Host.UseOrleans((_, builder) =>
    {
        builder
            .UseLocalhostClustering()
            .AddMemoryGrainStorage("shopping-cart")
            .AddStartupTask<SeedProductStoreTask>();
    });
}
else
{
    builder.Host.UseOrleans((context, builder) =>
    {
        var connectionString = context.Configuration["ORLEANS_AZURE_COSMOS_DB_CONNECTION_STRING"]!;

        builder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "ShoppingCartCluster";
            options.ServiceId = nameof(ShoppingCartService);
        });

        builder
            .UseCosmosClustering(o => o.ConfigureCosmosClient(connectionString))
            .AddCosmosGrainStorage("shopping-cart", o => o.ConfigureCosmosClient(connectionString));
    });
}

var app = builder.Build();
//builder.Services.AddSingleton<IConfiguration>(app.Configuration);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();
