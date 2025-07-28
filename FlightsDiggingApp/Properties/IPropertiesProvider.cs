namespace FlightsDiggingApp.Properties
{
    public interface IPropertiesProvider
    {
        public AffiliateProperties AffiliateProperties { get; }
        public AmadeusApiProperties AmadeusApiProperties { get; }
        public EnvironmentProperties EnvironmentProperties { get; }
    }
}
