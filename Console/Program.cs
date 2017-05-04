using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace MyConsole
{
    public class Demo
    {
        public int Key { get; set; }
        public int? Id { get; set; }
    }

    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<Demo> Demos { get; set; }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new DbContextOptionsBuilder<Context>()
                .UseMySQL("server=localhost;user=root;password=root;Database=bugdemo;port=3306;SslMode=None;default command timeout=1800");

            var context = new Context(builder.Options);

            var alldemos = context.Demos.ToList();
            Console.WriteLine($"table contains {alldemos.Count} rows");

            // Will generate the following query:
            //
            //   SELECT `x`.`Id`, `x`.`Key` FROM `Demos` AS `x` WHERE `x`.`Id` IN (2, 5, 200)
            //
            var nullableIds = new int?[] { 2, 5, 200 };
            var whereInResult1 = context.Demos.Where(x => nullableIds.Contains(x.Id)).ToList();
            Console.WriteLine($"where in 1 returns {whereInResult1.Count} rows");

            // Will generate the following query that lacks the 'WHERE IN':
            //
            //   SELECT `x`.`Id`, `x`.`Key` FROM `Demos` AS `x`
            //
            var nonNullableIds = new int[] { 2, 5, 200 };
            var whereInResult2 = context.Demos.Where(x => nonNullableIds.Contains(x.Id.Value)).ToList();
            Console.WriteLine($"where in 2 returns {whereInResult2.Count} rows");
        }
    }
}
 