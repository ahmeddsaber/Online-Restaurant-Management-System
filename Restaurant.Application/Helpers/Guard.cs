
using BadRequestException = Restaurant.Application.Exceptions.BadRequestException;

namespace Restaurant.Application.Helpers
{
    public static class Guard
    {
        public static void AgainstNull(object obj, string message = "Value cannot be null")
        {
            if (obj == null)
                throw new ArgumentNullException(message);
        }

        public static void AgainstNullOrEmpty(string value, string message = "Value cannot be null or empty")
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(message);
        }

        public static void AgainstInvalidId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Id");
        }

        public static void AgainstInvalidPrice(decimal price)
        {
            if (price <= 0)
                throw new ArgumentException("Invalid Price");
        }

        public static void AgainstInvalidMinutes(int minutes)
        {
            if (minutes <= 0)
                throw new ArgumentException("Invalid Preparation Time");
        }

        public static void AgainstInvalidRange(decimal min, decimal max)
        {
            if (min < 0 || max < 0 || min > max)
                throw new ArgumentException("Invalid price range");
        }

        public static void AgainstInvalidDays(int days)
        {
            if (days <= 0)
                throw new ArgumentException("Invalid days number");
        }
    }


}
