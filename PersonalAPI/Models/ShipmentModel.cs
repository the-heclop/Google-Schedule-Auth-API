using System.ComponentModel.DataAnnotations;

namespace PersonalAPI.Models
{
    public class ShipmentModel
    {
        [Key]
        public string tracking_id { get; set; }
        public string last_scan_location { get; set; }
        public string order_number { get; set; }
        public string destination { get; set; }
        public string scan_time { get; set; }
    }
}
