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
    
    private class rqst
    {
        public long InstrumentId = -1;
        public uint Bid = 0;
        public uint BidQty = 0;
        public uint Ask = 0;
        public uint AskQty = 0;
    }

    rqst requestInfo = new rqst();

    public Client(ExchangeEngineClient client)
    {
        this.client = client;
    }

    public override global::System.Threading.Tasks.Task<global::Efrei.ExchangeServer.Void> NewPrice(global::Efrei.ExchangeServer.NewPriceArgs request, grpc::ServerCallContext context)
    {
        if (requestInfo.InstrumentId == -1)
        {
            requestInfo.InstrumentId = request.InstrumentId;
            requestInfo.Bid = request.Bid;
            requestInfo.BidQty = request.BidQty;
            requestInfo.Ask = request.Ask;
            requestInfo.AskQty = request.AskQty;
        }
        else
        {
            if (requestInfo.InstrumentId != request.InstrumentId)
                if (request.Ask < requestInfo.Bid || requestInfo.Bid < request.Ask)
                {
                    Console.WriteLine("Opportunity");
                }
        }
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
