namespace EventListingAPI.Model
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int VenueId { get; set; }
    }
}
