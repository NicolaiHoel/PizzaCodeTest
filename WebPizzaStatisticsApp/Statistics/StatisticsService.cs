using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPizzaStatistics.Enums;
using WebPizzaStatistics.Models;

namespace WebPizzaStatistics.Statistics;

public class StatisticsService
{
    private static readonly Dictionary<string, Customer> Customers = new();

    // Method to continuously update the console with statistics
    public static void DisplayStatisticsLoop()
    {
        while (true)
        {
            // Clear the console
            Console.Clear();

            // Display the current statistics
            Console.WriteLine("Web Pizza Statistics");
            Console.WriteLine("---------------------------------------------");
            DisplayStatistics();

            // Sleep for a short duration before refreshing the screen
            Thread.Sleep(1000);
        }
    }

    private static void DisplayStatistics()
    {
        lock (Customers)
        {
            Console.WriteLine("Statistics:");
            var table = new ConsoleTable("Customer ID", "Orders", "Completed Orders", "Loyalty Member", "Loyalty Points");

            foreach (var customer in Customers.Values)
            {
                var orders = customer.Orders;
                var completedOrders = customer.CompletedOrders;
                var isLoyaltyMember = customer.IsLoyaltyMember;
                var loyaltyPointsEarned = customer.LoyaltyPoints;

                table.AddRow(customer.Id, orders, completedOrders, isLoyaltyMember, loyaltyPointsEarned);
            }

            table.Write(Format.MarkDown);
        }
    }

    public static void UpdateStatistics(Event newEvent)
    {
        lock (Customers)
        {
            if (!DoesCustomerExist(newEvent))
            {
                CreateCustomer(newEvent);
            }

            var customerToBeUpdated = Customers[newEvent.CustomerId];

            switch (newEvent.Type)
            {
                case EventType.PizzaOrdered:
                    customerToBeUpdated.AddOrder();
                    break;
                case EventType.CustomerAbandonedOrder:
                    customerToBeUpdated.RemoveOrder();
                    break;
                case EventType.PizzaPrepared:
                    // newEvent.dataField
                    // Decrement used topping from inventory managment
                    // Add Order, with topping, to customer. Count favorite topping.
                    break;
                case EventType.PaymentReceived:
                    customerToBeUpdated.PaymentReceived();
                    break;
                case EventType.CustomerFeedbackReceived:
                    // newEvent.dataField
                    // Add Feedback to Customer
                    break;
                case EventType.LoyaltySignup:
                    customerToBeUpdated.LoyaltySignup();
                    break;
                default:
                    break;
            }
        }
    }

    private static void CreateCustomer(Event newEvent)
    {
        Customers[newEvent.CustomerId] = new Customer(newEvent.CustomerId);
    }

    private static bool DoesCustomerExist(Event newEvent)
    {
        return Customers.ContainsKey(newEvent.CustomerId);
    }
}
