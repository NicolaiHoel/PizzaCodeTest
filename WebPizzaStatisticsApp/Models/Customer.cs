namespace WebPizzaStatistics.Models
{
    // Define a customer class
    internal class Customer
    {
        public string Id { get; }
        public bool IsLoyaltyMember { get; set; }

        public Customer(string id)
        {
            Id = id;
            IsLoyaltyMember = false;
        }
    }
}