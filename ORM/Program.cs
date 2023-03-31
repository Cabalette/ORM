using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Reflection.Metadata;

namespace ORM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new Test1Context())
            {
                var query = (from a in context.Members
                            join b in context.Bookings on a.Memid equals b.Memid
                            join c in context.Facilities on b.Facid equals c.Facid
                            where c.Name =="Tennis Court 2" || c.Name == "Tennis Court 1"                           
                            select new
                            {
                                member = a.Firstname + " " + a.Surname,
                                facility = c.Name
                            }).Distinct().OrderBy(x=>x.member).ThenBy(x=>x.facility);
                //var query = context.Members.Select(x => new
                //{
                //    MemSurname = x.Surname,
                //    MemFirstname = x.Firstname,
                //    Recommendedby = x.Recommendedby
                //}).LeftJoin(context.Members.Select(x => new
                //{
                //    RecSurname = x.Surname,
                //    RecFirstname = x.Firstname,
                //    Memid= x.Memid
                //}), a => a.Recommendedby, b => b.Memid, (a, b) => new
                //{
                //    memfname = a.MemFirstname,
                //    memsname = a.MemSurname
                //    //recfname=b.RecFirstname,
                //    //recsname=b.RecSurname
                //}).OrderBy(x=>x.memsname).ThenBy(x=> x.memfname);
                Console.WriteLine( "Member \t\t\t Facility" );
                foreach (var stuff in query)
                {
                    Console.WriteLine($"" +
                    // $"facid - {stuff.Memid} " +
                    $" - {stuff.member}" +
                    $" - {stuff.facility}" +
                    // $"- Surname - {stuff.recfname}" +
                    //$" \t Firstname {stuff.recsname}" +
                    //$"- Joindate - {stuff.Joindate}" +
                    //$"\t - Start - {stuff.Start} \t " +
                    //$"- Name - {stuff.Name} \t" +
                    //$"- monthlymaintenance - {stuff.Monthlymaintenance}" +
                    " ");

                    //Console.WriteLine($"" +
                    //$"facid - {stuff.Facid} " +
                    //$"- name - {stuff.Name} \t cost {stuff.Cost}" +
                    //    $"- membercost - {stuff.Membercost}" +
                    //    $"\t - guestcost - {stuff.Guestcost} \t " +
                    //    $"- initialoutlay - {stuff.Initialoutlay} \t" +
                    //    $"- monthlymaintenance - {stuff.Monthlymaintenance}"+
                    //" " );
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


 */