using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace StreamDeckActionSpike.ConsoleApp.Extensions
{
	public static class ConfigurationBuilderExtensions
	{
		public static IConfigurationBuilder AddStreamDeckCommandLine(this IConfigurationBuilder builder, params string[] args)
		{
			var switchMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				["-p"] = "StreamDeck:Port",
				["-port"] = "StreamDeck:Port",

				["-r"] = "StreamDeck:RegisterEvent",
				["-registerEvent"] = "StreamDeck:RegisterEvent",

				["-u"] = "StreamDeck:PluginUUID",
				["-pluginUUID"] = "StreamDeck:PluginUUID",

				["-i"] = "StreamDeck:Info",
				["-info"] = "StreamDeck:Info",
			};

			return builder
				.AddCommandLine(args, switchMappings);
		}
	}
}
