namespace EventsListingApp.Model
{
    public class EventsWithVenues
    {
        // Use properties so System.Text.Json can deserialize
        public List<EventList> Events { get; set; } = new();
        public List<Venue> Venues { get; set; } = new();
    }
}
