using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace ORM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new Test1Context())
            {
                //select facid, extract(month from starttime) as month, sum(slots) as "Total Slots"
                //    from cd.bookings
                //    where extract(year from starttime) = 2012
                //    group by facid, month
                //order by facid, month;
                var query = context.Bookings.Where(x => x.Starttime.Year == 2012).GroupBy(x => new
                {
                    x.Facid,
                    x.Starttime.Month
                }).Select(x => new
                {
                    x.Key.Facid,
                    x.Key.Month,
                    slots = x.Sum(e => e.Slots)
                }).OrderBy(x => x.Facid).ThenBy(x => x.Month).ToList();

                var query2 = (from bks in context.Bookings
                              where bks.Starttime.Year == 2012
                              group bks by new
                              {
                                  bks.Facid,
                                  bks.Starttime.Month
                              } into bks1
                              select new
                              {
                                  bks1.Key.Facid,
                                  bks1.Key.Month,
                                  slots = bks1.Sum(x => x.Slots)
                              } into bks2
                              orderby bks2.Facid, bks2.Month select bks2).ToList();

                int some = 1;

                foreach (var stuff in query)
                {
                    Console.WriteLine($"" +
                 $"facid - {stuff.Facid} " +
                $"revenue - {stuff.Month}" +
                    $" - {stuff.slots}" +
                //     $"- {stuff.cost}" +
                //$" \t firstname {stuff.recsname}" +
                //$"- joindate - {stuff.joindate}" +
                //$"\t - start - {stuff.start} \t " +
                //$"- name - {stuff.name} \t" +
                //$"- monthlymaintenance - {stuff.monthlymaintenance}" +
                " ");
                }
            }
        }
    }
}



/*
var query = context.Facilities.Select(x => new
                {
                    Name = x.Name,
                    Membercost = x.Membercost
                }); Выбрать 2 столбца

var query = context.Facilities.Where(x => x.Membercost > 0); простой WHERE

var query = context.Facilities.Select(x => new
                {
                    Facid=x.Facid,
                    Name = x.Name,
                    Membercost = x.Membercost,
                    Monthlymaintenance=x.Monthlymaintenance
                }).Where(x=>x.Membercost>0 && x.Membercost<x.Monthlymaintenance/50); Select + Where

var query = context.Facilities.Where(x => x.Facid ==1 | x.Facid ==5); select +  where 

var query = context.Facilities.Select(x => new
                {
                    Name = x.Name,
                    Cost = x.Monthlymaintenance > 100 ? "expensive" : "cheap" 
                }) ; case 

var query = context.Members.Select(x => new
                {
                    Memid = x.Memid,
                    Surname = x.Surname,
                    Firstname = x.Firstname,
                    Joindate = x.Joindate
                }).Where(x => x.Joindate >= DateTime.Parse("2012-09-01"));  сравнить даты

var query = context.Members.Select(x => new
                {
                    Surname = x.Surname
                }).Distinct().OrderBy(x => x.Surname).Take(10); distinct

var query = context.Members.Select(x => new
                {
                    Surname = Convert.ToString(x.Surname)
                }).Union(context.Facilities.Select(x => new
                {
                    Surname = Convert.ToString(x.Name)
                })); union

var query = context.Members.Max(x=>x.Joindate); MAX

var query = context.Members.Select(x => new
                {
                    Surname = x.Surname,
                    Firstname = x.Firstname,
                    Joindate = x.Joindate
                }).Where(x =>x.Joindate== context.Members.Max(x => x.Joindate)); агрегатная функция в селекте

var query = context.Bookings.Select(x => new
                {
                    Memid = x.Memid,
                    Starttime = x.Starttime
                }).Join(context.Members.Select(x => new
                {
                    Memid = x.Memid,
                    Firstname = x.Firstname,
                    Surname = x.Surname
                }).Where(x => x.Firstname == "David" && x.Surname == "Farrell"), b => b.Memid, m => m.Memid, (b, m) => new
                {
                    Starttime = b.Starttime
                });         inner join + where

var query = context.Facilities.Select(x => new
                {
                    Facid = x.Facid,
                    Name = x.Name
                }).Where(x => x.Name == "Tennis Court 2" || x.Name == "Tennis Court 1").Join(context.Bookings.Select(x => new
                {
                    Facid = x.Facid,
                    Start = x.Starttime
                }).Where(x => x.Start >= DateTime.Parse("2012-09-21") && x.Start < DateTime.Parse("2012-09-22")), b => b.Facid, m => m.Facid, (b, m) => new
                {
                    Start = m.Start,
                    Name=b.Name
                });      еще один inner join + where

var query = context.Members.Select(x => new
                {
                    Recommendedby = x.Recommendedby
                }).Join(context.Members.Select(x => new
                {
                    Surname = x.Surname,
                    Firstname = x.Firstname,
                    Memid= x.Memid
                }), a => a.Recommendedby, b => b.Memid, (a, b) => new
                {
                    Firstname = b.Firstname,
                    Surname = b.Surname
                }).Distinct().OrderBy(x=>x.Surname).ThenBy(x=> x.Firstname);  self join + order по 2 полям

var query = from a in context.Members
                            join b in context.Members on a.Recommendedby equals b.Memid into newtable
                            from subshit in newtable.DefaultIfEmpty()
                            orderby a.Surname,a.Firstname
                            select new
                            {
                                memfname = a.Firstname,
                                memsname = a.Surname,
                                recfname = subshit == null ? string.Empty : subshit.Firstname,
                                recsname = subshit == null ? string.Empty : subshit.Surname
                            };  left join

var query = (from a in context.Members
                            join b in context.Bookings on a.Memid equals b.Memid
                            join c in context.Facilities on b.Facid equals c.Facid
                            where c.Name =="Tennis Court 2" || c.Name == "Tennis Court 1"                           
                            select new
                            {
                                member = a.Firstname + " " + a.Surname,
                                facility = c.Name
                            }).Distinct().OrderBy(x=>x.member).ThenBy(x=>x.facility);  тройной JOIN + distinct + 2 order

var query = (from mems in context.Members
                             join bks in context.Bookings on mems.Memid equals bks.Memid
                             join facs in context.Facilities on bks.Facid equals facs.Facid
                             where bks.Starttime >= DateTime.Parse("2012-09-14") && bks.Starttime < DateTime.Parse("2012-09-15") &&
                                   ((mems.Memid == 0 && bks.Slots * facs.Guestcost > 30) || (mems.Memid != 0 && bks.Slots * facs.Membercost > 30))
                             select new
                             {
                                 member = mems.Firstname + " " + mems.Surname,
                                 facility = facs.Name,
                                 cost = mems.Memid == 0 ? bks.Slots * facs.Guestcost : bks.Slots * facs.Membercost
                             }).OrderByDescending(x => x.cost);         тройной джойн

                var query = (from mems in context.Members
                             select new
                             {
                                 member = mems.Firstname + " " + mems.Surname,
                                 recommender = (from recs in context.Members
                                                where recs.Memid == mems.Recommendedby
                                                select recs.Firstname + " " + recs.Surname
                                                ).FirstOrDefault()
                             }).Distinct().OrderBy(x => x.member).ToList();    это подзапрос через линку выражения

                var query2 = context.Members.Select(x => new
                {
                    member = x.Firstname + " " + x.Surname,
                    recommender = context.Members.Where(e => e.Memid == x.Recommendedby).Select(e => e.Firstname + " " + e.Surname).FirstOrDefault()
                }).Distinct().OrderBy(x => x.member).ToList();         это подзапрос через методы расширения линку 


    +++++++++++++++++++++++var query = (from mem in (from mems in context.Members
                                          join bks in context.Bookings on mems.Memid equals bks.Memid
                                          join facs in context.Facilities on bks.Facid equals facs.Facid
                                          where bks.Starttime >= DateTime.Parse("2012-09-14") && bks.Starttime < DateTime.Parse("2012-09-15")
                                          select new
                                          {
                                              member = mems.Firstname + " " + mems.Surname,
                                              facility = facs.Name,
                                              cost = mems.Memid == 0 ? bks.Slots * facs.Guestcost : bks.Slots * facs.Membercost
                                          }).ToList()
                             where mem.cost > 30
                             orderby mem.cost descending
                             select new
                             {
                                 member = mem.member,
                                 facility = mem.facility,
                                 cost = mem.cost
                             });    трипл джойн + подзапрос

  ++++++++++++++++++++++++++++++ var query2 = context.Members.Select(x => x).Join(context.Bookings, a => a.Memid, b => b.Memid, (a, b) => new
                {
                    memid = a.Memid,
                    member = a.Firstname + " " + a.Surname,
                    slots = b.Slots,
                    starttime = b.Starttime,
                    facid = b.Facid
                }).Join(context.Facilities, a => a.facid, b => b.Facid, (a, b) => new
                {
                    member = a.member,
                    starttime = a.starttime,
                    facility = b.Name,
                    cost = a.memid == 0 ? a.slots * b.Guestcost : a.slots * b.Membercost
                }).Where(x => x.starttime >= DateTime.Parse("2012-09-14") && x.starttime < DateTime.Parse("2012-09-15")).ToList().
                         Select(x => new
                         {
                             member = x.member,
                             facility = x.facility,
                             cost = x.cost
                         }).Where(x => x.cost > 30).OrderByDescending(x => x.cost);   трипл джойн + подзапрос
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

var query = context.Facilities.Add(new Facility
                {
                    Facid = 9,
                    Name = "Spa",
                    Membercost = 20,
                    Guestcost = 30,
                    Initialoutlay = 100000,
                    Monthlymaintenance = 800
                });
                context.SaveChanges();   простой инсерт


Facility facility = new Facility
                {
                    Facid = context.Facilities.Max(x => x.Facid)+1,
                    Name = "Spa",
                    Membercost = 20,
                    Guestcost = 30,
                    Initialoutlay = 100000,
                    Monthlymaintenance = 800
                };    подзапрос в добавлении

Facility facility = context.Facilities.Where(x => x.Facid == 1).FirstOrDefault();
                
                facility.Initialoutlay = 10000;
                context.SaveChanges();  update

Facility [] facility = context.Facilities.Where(x => x.Facid == 0 | x.Facid==1).ToArray();
                
                foreach(var fac in facility)
                {
                    fac.Membercost = 6;
                    fac.Guestcost = 30;
                }
                context.SaveChanges(); upd неск кортежей


Facility facility = context.Facilities.Where(x => x.Facid == 0).FirstOrDefault();
                Facility facility1 = context.Facilities.Where(x => x.Facid == 1).FirstOrDefault();
                facility1.Membercost = facility.Membercost + facility.Membercost / 10;
                facility1.Guestcost = facility.Guestcost + facility.Guestcost / 10;
                context.SaveChanges();   в sql это подзапрос в from 


context.Bookings.RemoveRange(context.Bookings);
                context.SaveChanges(); очистить таблицу

context.Members.Remove(context.Members.Where(x=>x.Memid==37).FirstOrDefault());
                context.SaveChanges(); удалить + where


--------------------------------------------------------------------------------------------------------------------------------------
int query = context.Facilities.Count(); вернули количество строк

var query2 = (from mem in context.Members
                              where mem.Recommendedby != null
                              group mem by mem.Recommendedby into temp
                              select new
                              {
                                  temp.Key,
                                  count = temp.Count()
                              }).ToList(); группировка легкая

   ++++++++++++var query = context.Bookings.GroupBy(x => x.Facid).Select(x => new
                {
                    x.Key,
                    sum = x.Sum(x => x.Slots)
                }).OrderBy(x => x.Key).ToList();

                var query2= (from bks in context.Bookings
                            group bks by bks.Facid into temp
                            select new
                            {
                                temp.Key,
                                sum=temp.Sum(x=>x.Slots)
                            }).OrderBy(x=>x.Key).ToList();   агрегатные функции 


 var query2 = (from bks in context.Bookings
                              group bks by bks.Facid into t
                              select new
                              {
                                  facid = t.Key,
                                  Total_slots = t.Sum(x => x.Slots)
                              }).Where(x => x.Total_slots > 1000).OrderBy(x => x.facid);


------------------------------------------------------------------------------------------------------------------------------------
                    var query = context.Bookings.Select(x => x).Join(context.Facilities.Select(x => x), a => a.Facid, b => b.Facid, (a, b) => new
                {
                    b.Facid,
                    b.Name,
                    a.Slots,
                    a.Memid,
                    b.Guestcost,
                    b.Membercost
                }).GroupBy(x => x.Name).Select(x => new
                {
                    x.Key,
                    sum = x.Sum(x => x.Slots * (x.Memid == 0 ? x.Guestcost : x.Membercost))
                }).OrderBy(x => x.sum).ToList();

                var query2 = (from bks in context.Bookings
                              join fac in context.Facilities on bks.Facid equals fac.Facid
                              select new
                              {
                                  fac.Facid,
                                  fac.Name,
                                  bks.Slots,
                                  bks.Memid,
                                  fac.Guestcost,
                                  fac.Membercost
                              }).GroupBy(x => x.Name).Select(x => new
                              {
                                  x.Key,
                                  sum = x.Sum(x => x.Slots * (x.Memid == 0 ? x.Guestcost : x.Membercost))
                              }).OrderBy(x => x.sum).ToList();   джоин + агрегатная в селекте (аккуратнее со скобками надо быть))00)000))0))

------------------------------------------------------------------------------------------------------------------------------------
                var subquery = context.Bookings.GroupBy(x => x.Facid).Select(x => new
                {
                    x.Key,
                    Sum = x.Sum(e => e.Slots)
                });

                var query = subquery.Where(x=>x.Sum==subquery.Max(x=>x.Sum)).ToList();     псевдо CTE


------------------------------------------------------------------------------------------------------------------------------------

                var query = context.Bookings.Where(x => x.Starttime.Year == 2012).GroupBy(x => new
                {
                    x.Facid,
                    x.Starttime.Month
                }).Select(x => new
                {
                    x.Key.Facid,
                    x.Key.Month,
                    slots = x.Sum(e => e.Slots)
                }).OrderBy(x => x.Facid).ThenBy(x => x.Month).ToList();

                var query2 = (from bks in context.Bookings
                              where bks.Starttime.Year == 2012
                              group bks by new
                              {
                                  bks.Facid,
                                  bks.Starttime.Month
                              } into bks1
                              select new
                              {
                                  bks1.Key.Facid,
                                  bks1.Key.Month,
                                  slots = bks1.Sum(x => x.Slots)
                              } into bks2
                              orderby bks2.Facid, bks2.Month select bks2).ToList();    group by по ДВУМ полям сразу
------------------------------------------------------------------------------------------------------------------------------------




 */