using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDeckActionSpike.ConsoleApp.Extensions
{
	public static class SerializationExtensions
	{
		private readonly static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
		{
			Converters =
			{
				new JsonStringEnumConverter(),
			},
		};

		public static Task<T> JsonDeserializeAsync<T>(this string json, CancellationToken? cancellationToken = default)
			=> Encoding.UTF8.GetBytes(json).JsonDeserializeAsync<T>(cancellationToken ?? CancellationToken.None);

		public async static Task<T> JsonDeserializeAsync<T>(this byte[] bytes, CancellationToken? cancellationToken = default)
		{
			await using var stream = new MemoryStream(bytes);

			return await stream.JsonDeserializeAsync<T>(cancellationToken ?? CancellationToken.None);
		}

		public async static Task<T> JsonDeserializeAsync<T>(this Stream stream, CancellationToken? cancellationToken = default)
		{
			if (stream is null) throw new ArgumentNullException(nameof(stream));
			if (!stream.CanRead) throw new ArgumentOutOfRangeException(nameof(stream), stream, "Unreadable stream");

			try
			{
				return await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions, cancellationToken ?? CancellationToken.None);
			}
			catch (Exception ex)
			{
				if (stream.CanRead && stream.CanSeek)
				{
					stream.Seek(0, SeekOrigin.Begin);

					using var reader = new StreamReader(stream, leaveOpen: true);

					var json = await reader.ReadToEndAsync();

					if (!string.IsNullOrWhiteSpace(json))
					{
						ex.Data.Add(nameof(json), json);
					}
				}

				throw;
			}
		}

		public async static Task<ArraySegment<byte>> JsonSerializeAsync(this object o, CancellationToken? cancellationToken = default)
		{
			await using var stream = new MemoryStream();

			await JsonSerializer.SerializeAsync(stream, o, _jsonSerializerOptions, cancellationToken ?? CancellationToken.None);

			if (stream.TryGetBuffer(out var buffer))
			{
				return buffer;
			}

			throw new ArgumentOutOfRangeException(nameof(o), o, "Could not serialize object");
		}
	}
}
