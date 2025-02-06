using Damasco.GraphQL.Mutations;
using Damasco.GraphQL.Queries;

namespace Damasco.GraphQL
{
    public static class GQLDependencyInjection
    {
        public static IServiceCollection AddGQLDependencyInjection(this IServiceCollection services)
        {
            // query types
            //services.AddScoped<MerchantQueryType>();
            //services.AddScoped<RoleQueryType>();
            //services.AddScoped<UserQueryType>();
            //services.AddScoped<BrandQueryType>();
            //services.AddScoped<LocationQueryType>();
            //services.AddScoped<ModuleQueryType>();
            //services.AddScoped<UserQueryType>();
            //services.AddScoped<CategoryQueryType>();
            //services.AddScoped<DropdownQueryType>();
            //services.AddScoped<SupplierQueryType>();
            //services.AddScoped<AuditTrailQueryType>();
            //services.AddScoped<UOMQueryTpe>();


            //// mutations
            //services.AddScoped<AccountMutation>();
            //services.AddScoped<MerchantMutation>();
            //services.AddScoped<RoleMutation>();
            //services.AddScoped<UserMutation>();
            //services.AddScoped<CategoryMutation>();
            //services.AddScoped<BrandMutation>();
            //services.AddScoped<SupplierMutation>();

            return services;
        }
    }
}
