namespace ClercSystem.Authorisation
{
    public static class AuthorisationPolicies
    {

        // Create policies
        public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanRead", policy =>
                    policy.RequireRole("Admin", "User", "Observer"));

                options.AddPolicy("CanCreate", policy =>
                    policy.RequireRole("Admin", "User"));

                options.AddPolicy("CanUpdate", policy =>
                    policy.RequireRole("Admin", "User"));

                options.AddPolicy("CanDelete", policy =>
                    policy.RequireRole("Admin"));
            });

            return services;
        }
    }
}
