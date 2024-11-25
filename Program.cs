using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ODataWebApiApplication15;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddOData(opt =>
{
    opt.AddRouteComponents("odata", EdmModelBuilder.GetEdmModel())
        .EnableQueryFeatures(maxTopValue: 100);
    opt.RouteOptions.EnableControllerNameCaseInsensitive = true;
    opt.RouteOptions.EnableActionNameCaseInsensitive = true;
    opt.RouteOptions.EnablePropertyNameCaseInsensitive = true;
    opt.RouteOptions.EnableNonParenthesisForEmptyParameterFunction = true;
});

var app = builder.Build();


app.UseODataRouteDebug();

app.UseHttpsRedirection();
app.UseRouting();

app.MapControllers();

app.Run();
