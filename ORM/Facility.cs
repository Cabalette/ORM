using System;
using System.Collections.Generic;

namespace ORM;

public partial class Facility
{
    public int Facid { get; set; }

    public string Name { get; set; } = null!;

    public decimal Membercost { get; set; }

    public decimal Guestcost { get; set; }

    public decimal Initialoutlay { get; set; }

    public decimal Monthlymaintenance { get; set; }

    public virtual ICollection<Booking> Bookings { get; } = new List<Booking>();
}
