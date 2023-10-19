namespace Daleya.API.Utility
{
    public static class SD
    {
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";

        public static string Cloudinary_Secretkey = "";
        public static string Cloudinary_CloudName = "";
        public static string Cloudinary_Apikey = "";

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyForPickup = "ReadyForPickup";
        public const string Status_Completed = "Completed";
        public const string Status_Refunded = "Refunded";
        public const string Status_Cancelled = "Cancelled";
    }
}
