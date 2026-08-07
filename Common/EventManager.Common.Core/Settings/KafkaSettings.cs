namespace EventManager.Common.Core.Settings;

public class KafkaSettings
{
    public const string SectionName = "Kafka";
    public string BootstrapServers { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = string.Empty;
}
