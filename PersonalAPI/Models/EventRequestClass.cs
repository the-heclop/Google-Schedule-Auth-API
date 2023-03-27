namespace PersonalAPI.Models
{
    public class EventRequestClass
    {          
        public string? Summary { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }    
        public bool? IsChecked { get; set; }
                
    }
}
