using BHSystem.Web.Core;
using BHSystem.Web.Extensions;
using Blazor.AdminLte;
using Blazored.LocalStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 2147483647;
    options.Limits.MaxRequestBufferSize = 2147483647;
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
    options.DisconnectedCircuitMaxRetained = 100;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(240);
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(10);
    options.MaxBufferedUnacknowledgedRenderBatches = 10;

});
builder.Services.AddAdminLte();// sử dụng templete của admin blazor
builder.Services.AddBlazoredToast();// sử dụng toast thông báo
builder.Services.AddTelerikBlazor(); // sử dụng telerik 
builder.Services.AddBlazoredLocalStorage(); // dùng local storage

//đăng kí service
builder.Services.AddScoped<ILoadingCore, LoadingCore>(); //mỗi yêu cầu 1 HTTP được tạo, thì 1 phiên bản mới của dịch vụ được tạo
builder.Services.AddScoped<IWebStateCore, WebStateCore>(); //mỗi yêu cầu 1 HTTP được tạo, thì 1 phiên bản mới của dịch vụ được tạo
builder.Services.AddSingleton<LoggerCore>(); //dịch vụ được đăng ký sẽ tồn tại suốt vòng đời của ứng dụng
builder.Services.AddHttpContextAccessor();
builder.Services.AddClientAuthorization();
builder.Services.AddClientScopeService();
builder.Services.AddComponentService();
string apiuri = builder.Configuration.GetSection("appSettings:ApiUrl").Value;// được từ chuỗi appsetting.json
builder.Services.AddHttpClient("api", c =>
{
    c.BaseAddress = new Uri(apiuri);
    c.Timeout = TimeSpan.FromMinutes(60);

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
