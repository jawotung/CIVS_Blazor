using Microsoft.AspNetCore.ResponseCompression;
using Infrastructure.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebAPI.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); // Register HttpClient
builder.Services.InfrastructureServices(builder.Configuration);

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidateAudience = true,
    ValidAudience = builder.Configuration["Jwt:Audience"],
    RequireExpirationTime = true,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = tokenValidationParameters;
});

builder.Services.AddSingleton(tokenValidationParameters);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<RefererValidationMiddleware>();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
