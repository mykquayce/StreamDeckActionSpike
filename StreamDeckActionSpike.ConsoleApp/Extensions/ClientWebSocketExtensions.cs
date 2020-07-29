using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDeckActionSpike.ConsoleApp.Extensions
{
	public static class ClientWebSocketExtensions
	{
		public async static Task<T> ReceiveAsync<T>(this ClientWebSocket socket, CancellationToken? cancellationToken = default)
		{
			var buffer = new byte[65_536];

			var result = await socket.ReceiveAsync(buffer, cancellationToken ?? CancellationToken.None);

			switch (result.CloseStatus)
			{
				case null:
				case WebSocketCloseStatus.Empty:
				case WebSocketCloseStatus.NormalClosure:
					var bytes = buffer[..result.Count];
					var value = await bytes.JsonDeserializeAsync<T>(cancellationToken ?? CancellationToken.None);
					return value;
				default:
					throw new ArgumentOutOfRangeException(nameof(WebSocketCloseStatus), result.CloseStatus, $"Unexpected {nameof(WebSocketCloseStatus)}: {result.CloseStatus}")
					{
						Data = { [nameof(WebSocketCloseStatus)] = result.CloseStatus, },
					};
			}
		}

		public static Task RegisterAsync(this ClientWebSocket socket, string @event, string uuid, CancellationToken? cancellationToken = default)
		{
			var payload = new { @event, uuid, };

			return socket.SendAsync(payload, cancellationToken ?? CancellationToken.None);
		}

		public async static Task SendAsync(this ClientWebSocket socket, object o, CancellationToken? cancellationToken = default)
		{
			var buffer = await o.JsonSerializeAsync();

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
