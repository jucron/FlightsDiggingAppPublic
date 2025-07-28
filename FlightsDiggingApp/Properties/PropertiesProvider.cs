using Microsoft.Extensions.Options;

namespace FlightsDiggingApp.Properties
{
    public class PropertiesProvider: IPropertiesProvider
    {
        public AffiliateProperties AffiliateProperties { get; }
        public AmadeusApiProperties AmadeusApiProperties { get; }
        public EnvironmentProperties EnvironmentProperties { get; }

        public PropertiesProvider(IOptions<AffiliateProperties> affiliatePropertiesOpt,
            IOptions<AmadeusApiProperties> amadeusApiPropertiesOpt, IOptions<EnvironmentProperties> environmentPropertiesOpt)
        {
            AffiliateProperties = affiliatePropertiesOpt.Value;
            AmadeusApiProperties = amadeusApiPropertiesOpt.Value;
            EnvironmentProperties = environmentPropertiesOpt.Value;
        }
    }
}
