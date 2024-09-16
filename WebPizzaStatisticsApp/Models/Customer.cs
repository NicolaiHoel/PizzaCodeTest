namespace WebPizzaStatistics.Models
{
    // Define a customer class
    internal class Customer
    {
        public string Id { get; }
        public bool IsLoyaltyMember { get; set; }
        public int Orders { get; set; }
        public int CompletedOrders { get; set; }
        public int LoyaltyPoints { get; set; }

        public Customer(string id)
        {
            Id = id;
            IsLoyaltyMember = false;
            Orders = 0;
            CompletedOrders = 0;
            LoyaltyPoints = 0;
        }

        public void AddOrder()
        {
            Orders++;
        }

        public void PaymentReceived()
        {
            CompletedOrders++;
            RemoveOrder();

            if (CompletedOrders >= 3) // Customer becomes a loyalty member after 3 completed orders
                IsLoyaltyMember = true;

            if (IsLoyaltyMember)
                LoyaltyPoints += 10; // Earn 10 loyalty points for each payment received
        }

        public void RemoveOrder()
        {
            Orders--;
            if (Orders < 0)
                Orders = 0;
        }

        public void LoyaltySignup()
        {
            IsLoyaltyMember = true;
        }
    }
}