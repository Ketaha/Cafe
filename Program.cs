using System;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using IT_Homework.Models;

namespace IT_Homework
{
    class Program
    {
        private const string cnxn = "Server=localhost; Database=Cafe; Integrated Security = True";
        
        private const string MONEY_FORMAT = "{0:0.00}";
            
        private static CafeContext context = new CafeContext();

        private static void Main(string[] args)
        {
            while (true)
            {
                using (SqlConnection sql = new SqlConnection(cnxn))
                {
                    Console.Write("Enter option: ");
                    var options = Console.ReadKey();
                    Console.WriteLine();
                    sql.Open();

                    switch (options.Key)
                    {
                        // Add order
                        case ConsoleKey.A:
                            using (var command = new SqlCommand("udp_TakeOrder", sql) {
                                CommandType = CommandType.StoredProcedure
                            }) {
                                Console.WriteLine($"ProductId: ");
                                command.Parameters.AddWithValue("ProductId", byte.Parse(Console.ReadLine()));
                                Console.WriteLine($"TableId: ");
                                command.Parameters.AddWithValue("tableId", byte.Parse(Console.ReadLine()));

                                command.ExecuteNonQuery();
                            }
                        break;

                        // Change status of isServed to 'true'
                        case ConsoleKey.B:
                            Console.WriteLine($"OrderId");
                            context.Orders.FirstOrDefault(c => c.Id == int.Parse(Console.ReadLine())).IsServed = true;

                            context.SaveChanges();
                        break;

                        // Remove order
                        case ConsoleKey.C:
                            Console.WriteLine($"OrderId");
                            context.Orders.Remove(context.Orders.FirstOrDefault(c => c.Id == int.Parse(Console.ReadLine())));

                            context.SaveChanges();
                        break;

                        // Print table orders
                        case ConsoleKey.E:
                            Console.WriteLine($"Print all?: Y / N");
                            var printAll = Console.ReadKey();
                            Console.WriteLine();
                            var comm = "SELECT * FROM V_AllTabs ";
                            
                            if (printAll.Key == ConsoleKey.N) {
                                Console.Write($"TableId: ");
                                comm += "WHERE tableId = " + byte.Parse(Console.ReadLine());
                            }

                            using (var command = new SqlCommand(comm, sql)) 
                                using (SqlDataReader reader = command.ExecuteReader()) 
                                    while (reader.Read()) 
                                        ReadSingleRow((IDataRecord)reader);

                            void ReadSingleRow(IDataRecord record) => Console.WriteLine($"{record[0]}, {record[1]} за маса {record[2]}" + 
                            " с цена {string.Format(MONEY_FORMAT, record[4])}. Сервирано?: {record[3]}");
                        break;
                        
                        // Close tab or check amount owned
                        case ConsoleKey.D:
                            using (var command = new SqlCommand("udp_CloseTab", sql) {
                                CommandType = CommandType.StoredProcedure
                            }) {
                                Console.WriteLine($"TableId: ");
                                var _tableId = byte.Parse(Console.ReadLine());
                                command.Parameters.AddWithValue("tableId", _tableId);
                                var tab = (decimal)command.ExecuteScalar();

                                Console.WriteLine($"Bill is: {string.Format(MONEY_FORMAT, tab)}лв.{Environment.NewLine}Close tab? Y / N");
                                if (Console.ReadKey().Key == ConsoleKey.Y) {
                                    context.Profits.Add(new Profit { Profit1 = tab, EarnedOn = DateTime.Now });
                                    context.Orders.RemoveRange(context.Orders.Where(c => c.TableId == _tableId));

                                    context.SaveChanges();
                                }
                            }
                        break;

                        // Извлечение за период
                        case ConsoleKey.F:
                        using (var command = new SqlCommand("udp_ProfitReportForPeriod", sql) {
                                CommandType = CommandType.StoredProcedure
                            }) {
                                Console.WriteLine("Поръчки за период.{0} Начало на извлечение: ", Environment.NewLine);
                                var start = DateTime.Parse(Console.ReadLine());
                                Console.WriteLine("End: ");
                                var end = DateTime.Parse(Console.ReadLine());

                                command.Parameters.AddWithValue("startPeriod", start);
                                command.Parameters.AddWithValue("endPeriod", end);

                                using (SqlDataReader reader = command.ExecuteReader()) {
                                    while (reader.Read()) {
                                        _ReadSingleRow((IDataRecord)reader);
                                    }
                                }

                                void _ReadSingleRow(IDataRecord record) => Console.WriteLine($"Получение пари - {string.Format(MONEY_FORMAT, record[1])}, за ден {record[2]}");
                            }
                        break;

                        // Оборот за ден / период
                        case ConsoleKey.G:
                            var groupSum = DateTime.Parse(Console.ReadLine());
                            var temp = DateTime.Parse(Console.ReadLine());
                            var result = context.Profits.GroupBy(o => o.EarnedOn >= groupSum && o.EarnedOn <= temp).Select(g => new { membername = g.Key, total = g.Sum(i => i.Profit1) });

                            foreach (var singleRow in result)
                                Console.WriteLine("Оборот за ден/период - {0}", string.Format(MONEY_FORMAT, singleRow.total));
                        break;

                        // End day
                        case ConsoleKey.Z:
                            using (var command = new SqlCommand("TRUNCATE TABLE Orders; DBCC CHECKIDENT (Orders, reseed, 0)", sql)) {
                                command.ExecuteNonQuery();
                                Environment.Exit(0);
                            }
                        break;
                    }
                }
            }
        }
    }
}