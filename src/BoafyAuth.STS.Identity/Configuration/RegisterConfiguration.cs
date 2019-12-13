using BoafyAuth.STS.Identity.Configuration.Intefaces;

namespace BoafyAuth.STS.Identity.Configuration
{
    public class RegisterConfiguration : IRegisterConfiguration
    {
        public bool Enabled { get; set; } = true;
    }
}
