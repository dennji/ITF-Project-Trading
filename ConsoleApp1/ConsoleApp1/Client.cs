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
    private int inc = 0;
    private Channel chanL;
    static public bool ouvert = true;
    public int escarcelle = 0;

    private class rqst
    {
        public int InstrumentId = -1;
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

    public Client(ExchangeEngineClient client, Channel channel)
    {
        this.client = client;
        this.chanL = channel;
    }

    public void update(NewPriceArgs request)
    {
        oldRequest.InstrumentId = request.InstrumentId;
        oldRequest.Bid = request.Bid;
        oldRequest.BidQty = request.BidQty;
        oldRequest.Ask = request.Ask;
        oldRequest.AskQty = request.AskQty;
    }

    public void achat_vente(int idAchat, int idVente)
    {
        if (inc <= 6)
        {
            sendOrder((ulong)clientId, idAchat, 99999999, 5); 
            sendOrder((ulong)clientId, idVente, 2, -5);
            inc++;
        }
        else
        {
            Thread.Sleep(555);
            chanL.ShutdownAsync();
            ouvert = false;
        }
    }

    public override Task<Efrei.ExchangeServer.Void> NewPrice(NewPriceArgs request, ServerCallContext context)
    {
        if (oldRequest.InstrumentId == -1 || oldRequest.InstrumentId == request.InstrumentId)
        {
            update(request);
        }
        else
        {
                if (oldRequest.Ask + 3 < request.Bid || request.Ask + 3 < oldRequest.Bid)
                {
                    if (oldRequest.Ask + 3 < request.Bid)
                    {
                        achat_vente(oldRequest.InstrumentId, request.InstrumentId);
                    }
                    else
                    if (request.Ask + 3 < oldRequest.Bid)
                    {
                        achat_vente(request.InstrumentId, oldRequest.InstrumentId);
                    }
                    oldRequest.InstrumentId = -1;
                }
        }
        return Task.FromResult(new Efrei.ExchangeServer.Void());
    }


    public override Task<Efrei.ExchangeServer.Void> OrderEvent(OrderEventArg request, ServerCallContext context)
    {
        escarcelle += (int)request.Deal.Price * -request.Deal.Qty;

        if (request.Deal.Qty >= 0)
            Console.WriteLine("Achat de " + Math.Abs(request.Deal.Qty) + " actions à " + request.Deal.Price);
        else
            Console.WriteLine("Vente de " + Math.Abs(request.Deal.Qty) + " actions à " + request.Deal.Price + " Votre portefeuille est à " + escarcelle);

        return Task.FromResult(new Efrei.ExchangeServer.Void());
    }

    public override Task<Efrei.ExchangeServer.Void> Ping(Efrei.ExchangeServer.Void request, ServerCallContext context)
    {
        Console.WriteLine("Connexion avec le serveur réussie");
        return Task.FromResult(request);
    }

    public SendOrderResponse sendOrder(ulong clientId, int instrumentId, ulong price, int qty)
    {
        var sOArgs = new SendOrderArgs
        {
            ClientId = clientId,
            InstrumentId = instrumentId,
            //sign represents side
            Qty = qty,
            Price = price

    };
        return client.SendOrder(sOArgs);
    }

}
