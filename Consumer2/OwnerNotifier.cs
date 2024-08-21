using Core.Entities;

namespace Consumer2;

public class OwnerNotifier
{
    public void NotifyOwner(Booking booking)
    {
        Console.WriteLine($"Notifying owner {booking.OwnerName} from property {booking.Request.PropertyId}...");
    }
}
