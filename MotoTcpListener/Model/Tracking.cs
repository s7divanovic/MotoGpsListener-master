using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoTcpListener
{
	[Table("tracking_data")]
	public class Tracking
	{
		[Key]
		public int id { get; set;}

		public string imei { get; set;}

		public string type { get; set; }

		public string journey_data { get; set; }

		public DateTime timestamp { get; set;}
	}
}
