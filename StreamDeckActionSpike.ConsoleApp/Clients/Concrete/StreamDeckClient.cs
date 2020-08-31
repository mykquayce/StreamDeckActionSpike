using System.Threading;
using System.Threading.Tasks;

namespace StreamDeckActionSpike.ConsoleApp.Clients.Concrete
{
	public class StreamDeckClient : WebSocketClient, IStreamDeckClient
	{
		public Task RegisterAsync(string @event, string uuid, CancellationToken? cancellationToken = default)
		{
			var payload = new { @event, uuid, };

			return base.SendAsync(payload, cancellationToken ?? CancellationToken.None);
		}

		public Task SetTitleAsync(string context, string title, CancellationToken? cancellationToken = default)
		{
			var payload = new
			{
				@event = "setTitle",
				context,
				payload = new
				{
					title,
					target = 0,
				},
			};

			return base.SendAsync(payload, cancellationToken ?? CancellationToken.None);
		}
	}
}
