using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// IoC container
builder.Services.AddSingleton<IAuthorsService, AuthorsService>(); // instantiate only one time - until closing kestral
builder.Services.AddSingleton<IBooksService, BooksService>(); // instantiate only one time - until closing kestral
builder.Services.AddDbContext<BooksDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
