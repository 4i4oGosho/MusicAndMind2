using Microsoft.AspNetCore.Identity;

public class UserTrack
{
    public int Id { get; set; }

    public string UserId { get; set; }
    public IdentityUser User { get; set; }

    public int TrackId { get; set; }
    public FrequencyTrack Track { get; set; }
}
