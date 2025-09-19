using Ocelot.Authorisation;

namespace cer_gateway.Utils
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection DecorateClaimAuthoriser(this IServiceCollection services)
        {
            var serviceDescriptor = services.First(x => x.ServiceType == typeof(IClaimsAuthoriser));
            services.Remove(serviceDescriptor);

            var newServiceDescriptor = new ServiceDescriptor(serviceDescriptor.ImplementationType, serviceDescriptor.ImplementationType, serviceDescriptor.Lifetime);
            services.Add(newServiceDescriptor);

            services.AddTransient<IClaimsAuthoriser, ClaimAuthorizerDecorator>();

            return services;
        }
    }
}
