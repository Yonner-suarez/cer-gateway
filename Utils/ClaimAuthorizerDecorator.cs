using Ocelot.Authorisation;
using Ocelot.DownstreamRouteFinder.UrlMatcher;
using Ocelot.Responses;
using System.Security.Claims;

namespace cer_gateway.Utils
{
    public class ClaimAuthorizerDecorator : IClaimsAuthoriser
    {
        private readonly ClaimsAuthoriser _authoriser;

        public ClaimAuthorizerDecorator(ClaimsAuthoriser authoriser)
        {
            _authoriser = authoriser;
        }

        public Response<bool> Authorise(ClaimsPrincipal claimsPrincipal,
                                        Dictionary<string, string> routeClaimsRequirement,
                                        List<PlaceholderNameAndValue> urlPathPlaceholderNameAndValues)
        {
            var newRouteClaimsRequirement = new Dictionary<string, string>();
            foreach (var kvp in routeClaimsRequirement)
            {
                if (kvp.Key.StartsWith("http///"))
                {
                    var key = kvp.Key.Replace("http///", "http://");
                    newRouteClaimsRequirement.Add(key, kvp.Value);
                }
                else
                {
                    newRouteClaimsRequirement.Add(kvp.Key, kvp.Value);
                }
            }

            return _authoriser.Authorise(claimsPrincipal, newRouteClaimsRequirement, urlPathPlaceholderNameAndValues);
        }
    }
}
