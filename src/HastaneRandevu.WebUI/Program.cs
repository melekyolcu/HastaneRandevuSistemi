using HastaneRandevu.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using HastaneRandevu.DataAccess.Abstract;
using HastaneRandevu.DataAccess.Concrete;
using HastaneRandevu.DataAccess.Seed;
using HastaneRandevu.Business.Abstract;
using HastaneRandevu.Business.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HastaneRandevuContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository kayıtları: biri IRandevuRepository isterse RandevuRepository verilir
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IDoktorRepository, DoktorRepository>();
builder.Services.AddScoped<IRandevuRepository, RandevuRepository>();

// Service kayıtları
builder.Services.AddScoped<IRandevuService, RandevuManager>();

var app = builder.Build();

// Geliştirme ortamında veritabanı boşsa örnek verileri yükle
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<HastaneRandevuContext>();
    await OrnekVeriYukleyici.YukleAsync(context);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
