namespace Lenovo.NAT.Infrastructure
{
    public class AppSettings
    {
        public string AirflowApi { get; set; }
        public string AirflowUserName { get; set; }
        public string AirflowUserPassword { get; set; }
        public string AnymarketApiProd { get; set; }
        public string AnymarketToken { get; set; }
        public string AnymarketApiUAT { get; set; }
        public string AnymarketTokenUAT { get; set; }
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public string SaslUsername { get; set; }
        public string SaslPassword { get; set; }
        public string ConsumerTopic { get; set; }
    }
}
