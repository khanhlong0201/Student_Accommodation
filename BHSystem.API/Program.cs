using BHSystem.API;
using BHSystem.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//AddCors -> cho client nào được gọi. hiện tại là ALL
builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                                                                    .AllowAnyMethod()
                                                                     .AllowAnyHeader())); // 

// add service Authen
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("Jwt:JwtIssuer").Value,
        ValidAudience = builder.Configuration.GetSection("Jwt:JwtAudience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:JwtSecurityKey").Value + "")),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddScoped<IBHouseDbContext, BHouseDbContext>(); // Add db Context
builder.Services.AddClientScopeService(); // đăng kí các service
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
