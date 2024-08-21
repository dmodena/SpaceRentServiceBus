using Core.Entities;

namespace Consumer1;

public class DateBlocker
{
    public void BlockDate(Booking booking)
    {
        Console.WriteLine($"Blocking property {booking.Request.PropertyId} from date {booking.Request.RentStart} to date {booking.Request.RentEnd}...");
    }
}
