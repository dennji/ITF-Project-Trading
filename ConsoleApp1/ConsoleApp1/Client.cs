using System;
using grpc = global::Grpc.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Efrei.ExchangeServer;
using Google.Protobuf;
using static Efrei.ExchangeServer.ExchangeClient;
using static Efrei.ExchangeServer.ExchangeEngine;

public class Client : Efrei.ExchangeServer.ExchangeClient.ExchangeClientBase
{
    private ExchangeEngineClient client;

    public Client(ExchangeEngineClient client)
    {
        this.client = client;
    }

    public override global::System.Threading.Tasks.Task<global::Efrei.ExchangeServer.Void> NewPrice(global::Efrei.ExchangeServer.NewPriceArgs request, grpc::ServerCallContext context)
    {
        return Task.FromResult(new Efrei.ExchangeServer.Void());
    }

    public override global::System.Threading.Tasks.Task<global::Efrei.ExchangeServer.Void> OrderEvent(global::Efrei.ExchangeServer.OrderEventArg request, grpc::ServerCallContext context)
    {
        Console.WriteLine("OrderEvent :" + request.ToString());
        return Task.FromResult(new Efrei.ExchangeServer.Void());
    }

    public override global::System.Threading.Tasks.Task<global::Efrei.ExchangeServer.Void> Ping(global::Efrei.ExchangeServer.Void request, grpc::ServerCallContext context)
    {
        Console.WriteLine("PING !");
        return Task.FromResult(request);
    }

}
