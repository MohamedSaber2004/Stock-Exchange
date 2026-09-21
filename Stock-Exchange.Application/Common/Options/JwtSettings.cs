namespace Stock_Exchange.Application.Common.Options
{
    public class JwtSettings
    {
        public string Secret { get; set; } = "n]:#J:?,{%9SvotDc^+/FMs7XHl$R1D2c^,Sf7_6vGJ>L8^!WvK1$$BqjVjD}rHGp}[fxYa90K1%4l3yf;sx5:";
        public string Issuer { get; set; } = "StockExchangeAPI";
        public string Audience { get; set; } = "StockExchangeMobile,StockExchangeDashboard";
        public int ExpiryInDays { get; set; } = 30;
        public int RefreshTokenExpiryDays { get; set; } = 30;
    }
}
