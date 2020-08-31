using StreamDeckActionSpike.ConsoleApp.Extensions;
using System;

namespace StreamDeckActionSpike.ConsoleApp.Models
{
	public class ConfigObject
	{
		private InfoObject? _infoDetails;

		public string? PluginUuid { get; set; }
		public int? Port { get; set; }
		public string? RegisterEvent { get; set; }
		public string? Info { get; set; }
		public InfoObject? InfoDetails => _infoDetails ??= Info?.JsonDeserializeAsync<InfoObject>().GetAwaiter().GetResult();

		public class InfoObject
		{
#pragma warning disable IDE1006 // Naming Styles
			public ApplicationObject? application { get; set; }
			public double? devicePixelRatio { get; set; }
			public DeviceObject[]? devices { get; set; }
			public PluginObject? plugin { get; set; }
#pragma warning restore IDE1006 // Naming Styles
		}

		public class ApplicationObject
		{
#pragma warning disable IDE1006 // Naming Styles
			public string? language { get; set; }
			public string? platform { get; set; }
			public string? version { get; set; }
#pragma warning restore IDE1006 // Naming Styles
			public Version? ParsedVersion => Version.TryParse(version, out var v)
				? v
				: default;
		}

		public class PluginObject
		{
#pragma warning disable IDE1006 // Naming Styles
			public string? version { get; set; }
#pragma warning restore IDE1006 // Naming Styles
		}

		public class DeviceObject
		{
#pragma warning disable IDE1006 // Naming Styles
			public string? id { get; set; }
			public string? name { get; set; }
			public SizeObject? size { get; set; }
			public DeviceTypes? type { get; set; }
#pragma warning restore IDE1006 // Naming Styles

			public enum DeviceTypes
			{
				kESDSDKDeviceType_StreamDeck = 0,
				kESDSDKDeviceType_StreamDeckMini = 1,
				kESDSDKDeviceType_StreamDeckXL = 2,
				kESDSDKDeviceType_StreamDeckMobile = 3,
				kESDSDKDeviceType_CorsairGKeys = 4,
			}
		}

		public class SizeObject
		{
#pragma warning disable IDE1006 // Naming Styles
			public int? columns { get; set; }
			public int? rows { get; set; }
#pragma warning restore IDE1006 // Naming Styles
		}
	}
}
