using CIVSUI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Application.Service;
using Application.Service.Interfaces;
using Blazored.LocalStorage;
using CIVSUI.Services;
using CIVSUI.States;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("WebAPI.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));


// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WebAPI.ServerAPI"));


builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IAmountLimitsServices, AmountLimitsServices>();
builder.Services.AddScoped<IBranchServices, BranchServices>();
builder.Services.AddScoped<IBranchModelAuxServices, BranchModelAuxServices>();
builder.Services.AddScoped<IEmailTemplateServices, EmailTemplateServices>();
builder.Services.AddScoped<IGroupServices, GroupServices>();
builder.Services.AddScoped<IGroupAccessServices, GroupAccessServices>();
builder.Services.AddScoped<IGroupMemberServices, GroupMemberServices>();
builder.Services.AddScoped<IMenuServices, MenuServices>();
builder.Services.AddScoped<IRejectReasonServices, RejectReasonServices>();
builder.Services.AddScoped<IUserTypeServices, UserTypeServices>();
builder.Services.AddScoped<IUserAmountLimitServices, UserAmountLimitServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IInwardClearingChequeUploadServices, InwardClearingChequeUploadServices>();
builder.Services.AddScoped<IInwardClearingChequeServices, InwardClearingChequeServices>();
builder.Services.AddScoped<IInwardClearingChequeReportServices, InwardClearingChequeReportServices>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthorizationService>();

await builder.Build().RunAsync();
