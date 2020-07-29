using StreamDeckActionSpike.ConsoleApp.Extensions;
using StreamDeckActionSpike.ConsoleApp.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace StreamDeckActionSpike.Tests
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			var json = @"{
				""device"":""2A0A7BDABC77303D81BE832BE3A5196C"",
				""deviceInfo"":
				{
					""name"":""Stream Deck XL"",
					""size"":
					{
						""columns"":8,
						""rows"":4
					},
					""type"":2
				},
				""event"":""deviceDidConnect""
			}";

			var options = new JsonSerializerOptions
			{
				Converters =
				{
					new JsonStringEnumConverter(),
				},
			};

			var o = JsonSerializer.Deserialize<PayloadWrapperObject>(json, options);

			Assert.NotNull(o);
			Assert.Equal("2A0A7BDABC77303D81BE832BE3A5196C", o.device, StringComparer.InvariantCultureIgnoreCase);
			Assert.Equal(Events.deviceDidConnect, o.@event);
		}

		[Theory]
		[InlineData(@"{""device"":""2A0A7BDABC77303D81BE832BE3A5196C"",""deviceInfo"":{""name"":""Stream Deck XL"",""size"":{""columns"":8,""rows"":4},""type"":2},""event"":""deviceDidConnect""}")]
		public async Task PayloadDeserializationTests(string jsonPayload)
		{
			var actual = await jsonPayload.JsonDeserializeAsync<PayloadWrapperObject>();

			Assert.NotNull(actual);
			Assert.Equal("2A0A7BDABC77303D81BE832BE3A5196C", actual.device, StringComparer.InvariantCultureIgnoreCase);
			Assert.Equal(Events.deviceDidConnect, actual.@event);
		}
	}
}
