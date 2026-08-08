using MedicalSupply.Application.Features.Departments;
using MedicalSupply.Application.Features.Items;
using MedicalSupply.Application.Services.SupplyRequests;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalSupply.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<SupplyRequestService>();
            services.AddScoped<DepartmentService>();
            services.AddScoped<ItemService>();

            return services;
        }
    }
}