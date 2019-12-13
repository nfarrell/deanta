using BoafyAuth.STS.Identity.Configuration.Intefaces;

namespace BoafyAuth.STS.Identity.Configuration
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; } = "https://admin.boafy.com";
    }
}