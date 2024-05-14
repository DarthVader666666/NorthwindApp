namespace Northwind.Application.Constants
{
    public static class UserRoles
    {
        public const string Admin = "admin";
        public const string Customer = "customer";
    }

    public static class SessionValues
    {
        public const string CustomerId = "CustomerId";
        public const string OrderId = "OrderId";

        public const string OrderStatus = "OrderStatus";

        public const string NotConfirmed = "Not Confirmed";
        public const string Confirmed = "Confirmed";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
    }
}
