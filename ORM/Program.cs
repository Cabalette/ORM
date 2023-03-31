using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ORM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new Test1Context())
            {
                var query = context.Members.Select(x => new
                {
                    Surname = x.Surname,
                    Firstname = x.Firstname,
                    Joindate = x.Joindate
                }).Where(x =>x.Joindate== context.Members.Max(x => x.Joindate));

                foreach (var stuff in query)
                {
                    Console.WriteLine($"" +
                    // $"facid - {stuff.Memid} " +
                    $"- Surname - {stuff.Surname}" +
                    $" \t Firstname {stuff.Firstname}" +
                    $"- Joindate - {stuff.Joindate}" +
                    //$"\t - guestcost - {stuff.Guestcost} \t " +
                    //$"- initialoutlay - {stuff.Initialoutlay} \t" +
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

 */