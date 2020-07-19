using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDeckActionSpike.ConsoleApp.Extensions
{
	public static class ClientWebSocketExtensions
	{
		public async static Task<(WebSocketReceiveResult, T)> ReceiveAsync<T>(this ClientWebSocket socket, CancellationToken? cancellationToken = default)
		{
			var buffer = new byte[65_536];

			var result = await socket.ReceiveAsync(buffer, cancellationToken ?? CancellationToken.None);

			var bytes = buffer[..result.Count];

			await using var stream = new MemoryStream(bytes);

			var value = await JsonSerializer.DeserializeAsync<T>(stream);

			return (result, value);
		}

		public static Task RegisterAsync(this ClientWebSocket socket, string @event, string uuid, CancellationToken? cancellationToken = default)
		{
			var payload = new { @event, uuid, };

			return socket.SendAsync(payload, cancellationToken ?? CancellationToken.None);
		}

		public async static Task SendAsync(this ClientWebSocket socket, object o, CancellationToken? cancellationToken = default)
		{
			await using var stream = new MemoryStream();

			await JsonSerializer.SerializeAsync(stream, o);

			stream.TryGetBuffer(out var buffer);

			await socket.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken ?? CancellationToken.None);
		}

		public static Task SetTitleAsync(this ClientWebSocket socket, string context, string title, CancellationToken? cancellationToken = default)
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

			return socket.SendAsync(payload, cancellationToken ?? CancellationToken.None);
		}
	}
}
