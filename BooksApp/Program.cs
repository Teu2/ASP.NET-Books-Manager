using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// IoC container
builder.Services.AddSingleton<IAuthorsService, AuthorsService>(); // instantiate only one time - until closing kestral
builder.Services.AddSingleton<IBooksService, BooksService>(); // instantiate only one time - until closing kestral

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
