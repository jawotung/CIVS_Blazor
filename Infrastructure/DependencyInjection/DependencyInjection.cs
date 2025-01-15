using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Infrastructure.Data;
using Infrastructure.Handlers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAPI;

namespace Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection InfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

           
            services.AddDbContext<PRDSVSContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("PRDSVSContext").Decrypt());
            });

            services.AddDbContext<AppDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("BOCCIVSContext").Decrypt());
            });

            services.AddDbContext<FinacleSVSContext>(options => {
                options.UseNpgsql(configuration.GetConnectionString("FinacleSVSContext").Decrypt());
            });




            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); 
            services.AddScoped<CommonClass>();
            services.AddScoped<ADClass>();
            services.AddScoped<GeneratePDF>();
            services.AddScoped<IRolePolicyService, RolePolicyService>();

            services.AddTransient<IAuthRepository, AuthRepositories>();
            services.AddTransient<IGroupRepository, GroupRepositories>();
            services.AddTransient<IGroupAccessRepositories, GroupAccessRepositories>();
            services.AddTransient<IGroupMemberRepositories, GroupMemberRepositories>();
            services.AddTransient<IAmountLimitsRepository, AmountLimitsRepositories>();
            services.AddTransient<IBranchRepository, BranchRepositories>();
            services.AddTransient<IBranchModelAuxesRepository, BranchModelAuxesRepositories>();
            services.AddTransient<IEmailTemplateRepository, EmailTemplateRepositories>();
            services.AddTransient<IMenuRepository, MenuRepositories>();
            services.AddTransient<IRejectReasonRepository, RejectReasonRepositories>();
            services.AddTransient<IUserAmountLimitRepository, UserAmountLimitRepositories>();
            services.AddTransient<IUserTypeRepository, UserTypeRepositories>();
            services.AddTransient<IUserRepository, UserRepositories>();
            services.AddTransient<IInwardClearingChequeUploadRepository, InwardClearingChequeUploadRepositories>();
            services.AddTransient<IInwardClearingChequeRepository, InwardClearingChequeRepositories>();
            services.AddTransient<IInwardClearingChequeReportRepository, InwardClearingChequeReportRepositories>();
            services.AddTransient<IApiServiceRepository, ApiServiceRepository>();
            services.AddTransient<ISVSRepository, SVSRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
