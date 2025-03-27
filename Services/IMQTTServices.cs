 public class MQTTService : IMQTTService
{
    private readonly IMqttClient _mqttClient;

    public MQTTService()
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithClientId("BackendAPI")
            .WithTcpServer("broker.hivemq.com", 1883)
            .Build();

        _mqttClient.ConnectAsync(options).Wait();
    }

    public async Task PublishOrder(OrderRequest order)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("order/new")
            .WithPayload(JsonConvert.SerializeObject(order))
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await _mqttClient.PublishAsync(message);
    }
}
