using System;
using Grpc.Core;
using Efrei.ExchangeServer;
using static Efrei.ExchangeServer.ExchangeEngine;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Channel chanL = new Channel("localhost:10001", ChannelCredentials.Insecure);
            var client = new ExchangeEngineClient(chanL);
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
            {
                if (clientId.Equals(-1))
                {
                    Console.WriteLine("Problème de connexion..");
                    return;
                }
                client.SendOrder(new SendOrderArgs
                {
                    InstrumentId = 1,
                    Price = 9999999,
                    ClientId = clientId,
                    Qty = 1
                });
                Thread.Sleep(39400);
            }
            
        }
    }
}
