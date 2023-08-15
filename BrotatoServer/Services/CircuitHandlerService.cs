using Microsoft.AspNetCore.Components.Server.Circuits;
using Prometheus;

namespace BrotatoServer.Services;

public class CircuitHandlerService : CircuitHandler
{
    private static readonly Counter BlazorCircuitConnections = Metrics.CreateCounter("brotato_blazor_connections_total", "Number of total Blazor circuit connections");
    private static readonly Gauge BlazorCurrentConnections = Metrics.CreateGauge("brotato_blazor_current_connections", "Number of current Blazor circuit connections");

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        BlazorCircuitConnections.Inc();
        BlazorCurrentConnections.Inc();
        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        BlazorCurrentConnections.Dec();
        return base.OnCircuitClosedAsync(circuit, cancellationToken);
    }
}