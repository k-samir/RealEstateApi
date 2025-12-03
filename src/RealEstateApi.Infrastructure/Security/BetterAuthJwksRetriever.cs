using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace RealEstateApi.Infrastructure.Security;

/// <summary>
/// Custom configuration retriever for Better Auth JWKS endpoint
/// Better Auth returns a raw JWKS document, not a full OIDC discovery document
/// </summary>
public class BetterAuthJwksRetriever : IConfigurationRetriever<OpenIdConnectConfiguration>
{
    public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(
        string address, 
        IDocumentRetriever retriever, 
        CancellationToken cancel)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentNullException(nameof(address));
        }

        // Fetch the JWKS document
        var doc = await retriever.GetDocumentAsync(address, cancel);
        
        // Parse as JsonWebKeySet
        var jwks = new JsonWebKeySet(doc);
        
        // Create OpenIdConnectConfiguration with the keys
        var config = new OpenIdConnectConfiguration
        {
            JsonWebKeySet = jwks,
        };
        
        // Add the keys to the SigningKeys collection
        foreach (var key in jwks.Keys)
        {
            config.SigningKeys.Add(key);
        }
        
        return config;
    }
}
