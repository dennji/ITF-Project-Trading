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
            Channel chanL = new Channel("localhost:50000", ChannelCredentials.Insecure);
            var client = new ExchangeEngineClient(chanL);
            client.PingSrv(new Efrei.ExchangeServer.Void());
            var tradingClient = new Client(client, chanL);

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
            tradingClient.clientId = subs.ClientId;
            var clientId = (ulong)subs.ClientId;
            Console.WriteLine("Votre numéro unique client est " + clientId);
            if (clientId.Equals(-1))
            {
                Console.WriteLine("Problème de connexion..");
                return;
            }


            //tradingClient.sendOrder(clientId, 0, 9999999, 1);
            /// Le titre va être acheté au titre vendu qui a le prix le plus bas
            /// car sa limitation est 1000 fois plus haute que le meilleur prix de vente

            //tradingClient.sendOrder(123456, 0, 9999999, 1);
            /// Le serveur va rejeté l'ordre s'il n'existe pas de client qui possède cet id
            /// Le serveur peut accepté l'ordre si par hasard un client a cet id.

            //tradingClient.sendOrder(clientId, -1, 9999999, 1);
            /// Le serveur va rejeté l'ordre car cet id ne correspond à aucun instrument

            //tradingClient.sendOrder(clientId, 10, 9999999, 1);
            /// Le serveur va rejeté l'ordre car cet id ne correspond à aucun instrument

            //tradingClient.sendOrder(clientId, 0, 9999999, 0);
            /// Le serveur va rejeté l'ordre passé ne possède aucune quantité

            //tradingClient.sendOrder(clientId, 0, 1, 1);
            ///À partir d'un certain seuil, les prix trop haut pour la vente
            ///ou trop bas pour l'achat ne sont pas accepté

            //tradingClient.sendOrder(clientId, 0, -99999, 1);
            ///À partir d'un certain seuil, les prix trop haut pour la vente
            ///ou trop bas pour l'achat ne sont pas accepté
            
            while (Client.ouvert == true)
            {
            }
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");
            if (tradingClient.escarcelle >= 0)
                Console.WriteLine("Votre portefeuille est positif : Vous avez gagné " + tradingClient.escarcelle);
            else
                Console.WriteLine("Votre portefeuille est négatif : Vous avez perdu " + tradingClient.escarcelle);
            while (true)
            { }
        }
    }
}
