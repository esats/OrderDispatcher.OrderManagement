using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderDispatcher.CatalogService.Dal.Concrete.EntityFramework;
using OrderDispatcher.OrderManagement.API.Infrastructure;
using OrderDispatcher.OrderManagement.Dal.Abstract;
using OrderDispatcher.OrderManagement.Dal.Concrete.EntityFramework;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Services --------------------

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var jwtSection = builder.Configuration.GetSection("JwtTokenOptions");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<OrderManagementDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IBasketMaster, EfBasketMaster>();
builder.Services.AddScoped<IBasketDetail, EfBasketDetail>();
builder.Services.AddScoped<IOrder, EfOrder>();
builder.Services.AddSingleton<OrderMessagePublisher>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderManagementDBContext>();
    db.Database.Migrate();
}

// we use this for defective endpoints
app.MapGet("/_debug/apidesc", (IApiDescriptionGroupCollectionProvider provider) =>
{
    var items = provider.ApiDescriptionGroups.Items
        .SelectMany(g => g.Items)
        .Select(d => new
        {
            d.HttpMethod,
            d.RelativePath,
            Action = d.ActionDescriptor.DisplayName
        })
        .OrderBy(x => x.HttpMethod)
        .ThenBy(x => x.RelativePath);

    return Results.Ok(items);
})
.ExcludeFromDescription();

// -------------------- Pipeline --------------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs", o => o.Title = "Order Management API");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
