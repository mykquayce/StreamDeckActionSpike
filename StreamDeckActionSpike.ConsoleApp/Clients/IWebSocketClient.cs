using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDeckActionSpike.ConsoleApp.Clients
{
	public interface IWebSocketClient : IDisposable
	{
		Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription = null, CancellationToken? cancellationToken = null);
		Task ConnectAsync(Uri uri, CancellationToken? cancellationToken = null);
		Task<T> ReceiveAsync<T>(CancellationToken? cancellationToken = null);
		Task SendAsync(object o, CancellationToken? cancellationToken = null);
	}
}
