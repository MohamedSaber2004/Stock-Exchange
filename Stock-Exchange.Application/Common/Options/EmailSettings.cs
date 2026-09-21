namespace Stock_Exchange.Application.Common.Options
{
    public class EmailSettings
    {
        public string Email { get; set; } = "dev.mohamed104saber@gmail.com";
        public string Name { get; set; } = "StockExchange@Team";
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string Username { get; set; } = "dev.mohamed104saber@gmail.com";
        public string Password { get; set; } = "yxnucokntgvujogk";
        public int VerificationCodeExpiryMinutes { get; set; } = 10;
    }
}
