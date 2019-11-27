using System;
using Grpc.Core;
using Efrei.ExchangeServer;
using static Efrei.ExchangeServer.ExchangeEngine;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Channel chanL = new Channel("localhost:10001", ChannelCredentials.Insecure);
            ExchangeEngineClient client = new ExchangeEngineClient(chanL);
            client.PingSrv(new Efrei.ExchangeServer.Void());
            var tradingClient = new Client(client);

            var server = new Server
            {
                Ports = { new ServerPort("localhost", 10003, ServerCredentials.Insecure) },
                Services = { ExchangeClient.BindService(tradingClient) }
            };
            server.Start();

            var subs = client.Subscribe(new SubscribeArgs
            {
                Name = "client",
                Endpoint = "localhost:10003"
            });

            var clientId = (ulong)subs.ClientId;
            Console.WriteLine(clientId);
            
            while (true)
            { }
        }
    }
}
