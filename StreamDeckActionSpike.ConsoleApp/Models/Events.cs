using System;

namespace StreamDeckActionSpike.ConsoleApp.Models
{
	[Flags]
	public enum Events
	{
		applicationDidLaunch = 1,
		applicationDidTerminate = 2,
		deviceDidConnect = 4,
		deviceDidDisconnect = 8,
		didReceiveGlobalSettings = 16,
		didReceiveSettings = 32,
		getGlobalSettings = 64,
		getSettings = 128,
		keyDown = 256,
		keyUp = 512,
		logMessage = 1_024,
		openUrl = 2_048,
		propertyInspectorDidAppear = 4_096,
		propertyInspectorDidDisappear = 8_192,
		sendToPlugin = 16_384,
		sendToPropertyInspector = 32_768,
		setGlobalSettings = 65_536,
		setImage = 131_072,
		setSettings = 2621_44,
		setState = 524_288,
		setTitle = 1_048_576,
		showAlert = 2_097_152,
		showOk = 4_194_304,
		switchToProfile = 8_388_608,
		systemDidWakeUp = 16_777_216,
		titleParametersDidChange = 33_554_432,
		willAppear = 67_108_864,
		willDisappear = 134_217_728,
	}
}
