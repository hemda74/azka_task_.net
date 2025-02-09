using task_amwag.Repositories;
using task_amwag.Services;
public static class ServiceExtensions
{
    public static void ConfigureRepository(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeRepository, IEmployeeRepository>();
        services.AddScoped<ILeaveRepository, LeaveRepository>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ILeaveService, LeaveService>();
    }
}