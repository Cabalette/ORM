using System;
using System.Collections.Generic;

namespace ORM;

public partial class Member
{
    public int Memid { get; set; }

    public string Surname { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int Zipcode { get; set; }

    public string Telephone { get; set; } = null!;

    public int? Recommendedby { get; set; }

    public DateTime Joindate { get; set; }

    public virtual ICollection<Booking> Bookings { get; } = new List<Booking>();

    public virtual ICollection<Member> InverseRecommendedbyNavigation { get; } = new List<Member>();

    public virtual Member? RecommendedbyNavigation { get; set; }
}
