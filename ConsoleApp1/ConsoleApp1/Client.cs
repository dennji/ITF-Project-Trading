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
    public long clientId = -1;

    private class rqst
    {
        public long InstrumentId = -1;
        public uint Bid = 0;
        public uint BidQty = 0;
        public uint Ask = 0;
        public uint AskQty = 0;

        public override String ToString()
        {
            return ('{'
                + "\n Instrument: Id = " + InstrumentId.ToString()
                + "\n Bid: Value = " + Bid.ToString() + ", Quantity = " + BidQty.ToString()
                + "\n Ask: Value = " + Ask.ToString() + ", Quantity = " + AskQty.ToString()
                + "\n}");
        }
    }

    rqst oldRequest = new rqst();

    public Client(ExchangeEngineClient client)
    {
        this.client = client;
    }

    public void update(NewPriceArgs request)
    {
        oldRequest.InstrumentId = request.InstrumentId;
        oldRequest.Bid = request.Bid;
        oldRequest.BidQty = request.BidQty;
        oldRequest.Ask = request.Ask;
        oldRequest.AskQty = request.AskQty;
    }


    public override Task<Efrei.ExchangeServer.Void> NewPrice(NewPriceArgs request, ServerCallContext context)
    {
        if (oldRequest.InstrumentId == -1 || oldRequest.InstrumentId == request.InstrumentId)
        {
            update(request);
        }
        else
        {
            if (request.Bid > oldRequest.Ask || request.Ask < oldRequest.Bid)
            {
                Console.WriteLine("Type\t|" + oldRequest.InstrumentId + "\t|" + request.InstrumentId);
                Console.WriteLine("Bid\t|" + oldRequest.Bid + "\t|" + request.Bid);
                Console.WriteLine("Ask\t|" + oldRequest.Ask + "\t|" + request.Ask);
                Console.WriteLine();

                oldRequest.InstrumentId = -1;
            }
        }


        return Task.FromResult(new Efrei.ExchangeServer.Void());
    }


    public override Task<Efrei.ExchangeServer.Void> OrderEvent(OrderEventArg request, ServerCallContext context)
    {
        Console.WriteLine("OrderEvent :" + request.ToString());
        return Task.FromResult(new Efrei.ExchangeServer.Void());
    }

    public override Task<Efrei.ExchangeServer.Void> Ping(Efrei.ExchangeServer.Void request, ServerCallContext context)
    {
        Console.WriteLine("PING !");
        return Task.FromResult(request);
    }

    public SendOrderResponse sendOrder(ulong clientId, int instrumentId, ulong price, int qty)
    {
        var sOArgs = new SendOrderArgs
        {
            ClientId = clientId,
            InstrumentId = instrumentId,
            //sign represents side
            Qty = qty
        };
        if (price != 0)
            sOArgs.Price = price;
        return client.SendOrder(sOArgs);
    }

}
