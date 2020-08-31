using System.Threading;
using System.Threading.Tasks;

namespace StreamDeckActionSpike.ConsoleApp.Clients
{
	public interface IStreamDeckClient : IWebSocketClient
	{
		Task RegisterAsync(string @event, string uuid, CancellationToken? cancellationToken = null);
		Task SetTitleAsync(string context, string title, CancellationToken? cancellationToken = null);
	}
}
