namespace StreamDeckActionSpike.ConsoleApp.Models
{
	public class PayloadWrapperObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public string? action { get; set; }
		public string? context { get; set; }
		public string? device { get; set; }
		public string? @event { get; set; }
#pragma warning restore IDE1006 // Naming Styles
	}
}
