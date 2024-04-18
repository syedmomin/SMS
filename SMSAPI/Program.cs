using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SMS_API;
using SMSAPI.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaulutConnection")));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



#region JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JwtKey"]);

builder.Services.AddAuthentication(o =>
{
  o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer(x =>
{
  x.RequireHttpsMetadata = false;
  x.SaveToken = true;
  x.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ClockSkew = TimeSpan.Zero,
  };
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
  var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
  context.Database.Migrate();
}
app.UseCors(builder =>
          builder
          .AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod());

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

app.Run();
