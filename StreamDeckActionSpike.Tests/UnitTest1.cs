using Microsoft.Extensions.Configuration;
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
		[Theory]
		[InlineData(@"{""device"":""2A0A7BDABC77303D81BE832BE3A5196C"",""deviceInfo"":{""name"":""Stream Deck XL"",""size"":{""columns"":8,""rows"":4},""type"":2},""event"":""deviceDidConnect""}")]
		public async Task DeserializePayload_HasTheCorrectValues(string jsonPayload)
		{
			var actual = await jsonPayload.JsonDeserializeAsync<PayloadWrapperObject>();

			Assert.NotNull(actual);
			Assert.Equal("2A0A7BDABC77303D81BE832BE3A5196C", actual.device, StringComparer.InvariantCultureIgnoreCase);
			Assert.Equal(Events.deviceDidConnect, actual.@event);
		}

		[Fact]
		public void AddingCommandLineArgs_HasTheCorrectValues()
		{
			// Arrange
			var args = new string[8]
			{
				"-port",
				"28196",
				"-pluginUUID",
				"41D209C48FAD1F4B1F4F60809DD5B926",
				"-registerEvent",
				"registerPlugin",
				"-info",
				"{\"application\":{\"language\":\"en\",\"platform\":\"windows\",\"version\":\"4.8.1.13027\"},\"devicePixelRatio\":1,\"devices\":[{\"id\":\"2A0A7BDABC77303D81BE832BE3A5196C\",\"name\":\"Stream Deck XL\",\"size\":{\"columns\":8,\"rows\":4},\"type\":2}],\"plugin\":{\"version\":\"1.4\"}}",
			};

			var config = new ConfigurationBuilder()
				.AddStreamDeckCommandLine(args)
				.Build();

			// Act
			var actual = config.GetSection("StreamDeck").Get<ConfigObject>();

			// Assert
			Assert.NotNull(actual);
			Assert.Equal("41D209C48FAD1F4B1F4F60809DD5B926", actual.PluginUuid);
			Assert.Equal(28_196, actual.Port);
			Assert.Equal("registerPlugin", actual.RegisterEvent);
			Assert.NotNull(actual.Info);

			// Assert InfoDetails isn't calculated twice
			var left = actual.InfoDetails;
			var right = actual.InfoDetails;
			Assert.NotNull(left);
			Assert.NotNull(right);
			Assert.Same(left, right);

			Assert.NotNull(actual.InfoDetails);
			Assert.NotNull(actual.InfoDetails.application);
			Assert.Equal("en", actual.InfoDetails.application.language);
			Assert.Equal("windows", actual.InfoDetails.application.platform);
			Assert.Equal("4.8.1.13027", actual.InfoDetails.application.version);
			Assert.Equal(1, actual.InfoDetails.devicePixelRatio);
			Assert.NotNull(actual.InfoDetails.devices);
			Assert.NotEmpty(actual.InfoDetails.devices);
			Assert.Single(actual.InfoDetails.devices);
			Assert.Equal("2A0A7BDABC77303D81BE832BE3A5196C", actual.InfoDetails.devices[0].id);
			Assert.Equal("Stream Deck XL", actual.InfoDetails.devices[0].name);
			Assert.NotNull(actual.InfoDetails.devices[0].size);
			Assert.Equal(8, actual.InfoDetails.devices[0].size.columns);
			Assert.Equal(4, actual.InfoDetails.devices[0].size.rows);
			Assert.Equal(ConfigObject.DeviceObject.DeviceTypes.kESDSDKDeviceType_StreamDeckXL, actual.InfoDetails.devices[0].type);
			Assert.NotNull(actual.InfoDetails.plugin);
			Assert.Equal("1.4", actual.InfoDetails.plugin.version);
		}

		[Theory]
		[InlineData("{\"application\":{\"language\":\"en\",\"platform\":\"windows\",\"version\":\"4.8.1.13027\"},\"devicePixelRatio\":1,\"devices\":[{\"id\":\"2A0A7BDABC77303D81BE832BE3A5196C\",\"name\":\"Stream Deck XL\",\"size\":{\"columns\":8,\"rows\":4},\"type\":2}],\"plugin\":{\"version\":\"1.4\"}}")]
		public async Task DeserializeInfoOjbect_HasTheCorrectValues(string json)
		{
			var actual = await json.JsonDeserializeAsync<ConfigObject.InfoObject>();

			Assert.NotNull(actual);
			Assert.NotNull(actual.application);
			Assert.Equal("en", actual.application.language);
			Assert.Equal("windows", actual.application.platform);
			Assert.Equal("4.8.1.13027", actual.application.version);
			Assert.Equal(1, actual.devicePixelRatio);
			Assert.NotNull(actual.devices);
			Assert.NotEmpty(actual.devices);
			Assert.Single(actual.devices);
			Assert.NotNull(actual.devices[0]);
			Assert.Equal("2A0A7BDABC77303D81BE832BE3A5196C", actual.devices[0].id);
			Assert.Equal("Stream Deck XL", actual.devices[0].name);
			Assert.NotNull(actual.devices[0].size);
			Assert.Equal(8, actual.devices[0].size.columns);
			Assert.Equal(4, actual.devices[0].size.rows);
			Assert.Equal(ConfigObject.DeviceObject.DeviceTypes.kESDSDKDeviceType_StreamDeckXL, actual.devices[0].type);
			Assert.NotNull(actual.plugin);
			Assert.Equal("1.4", actual.plugin.version);
		}
	}
}
