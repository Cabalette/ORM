using System;
using System.Collections.Generic;

namespace ORM;

public partial class Booking
{
    public int Bookid { get; set; }

    public int Facid { get; set; }

    public int Memid { get; set; }

    public DateTime Starttime { get; set; }

    public int Slots { get; set; }

    public virtual Facility Fac { get; set; } = null!;

    public virtual Member Mem { get; set; } = null!;
}
