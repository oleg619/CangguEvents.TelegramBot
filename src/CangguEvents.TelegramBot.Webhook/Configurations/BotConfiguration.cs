namespace CangguEvents.TelegramBot.Webhook.Configurations
{
    public class BotConfiguration
    {
        public BotConfiguration(string webhookUrl, string botToken, string socks5Host, int socks5Port)
        {
            WebhookUrl = webhookUrl;
            BotToken = botToken;
            Socks5Host = socks5Host;
            Socks5Port = socks5Port;
        }

        public BotConfiguration() : this("", "", "", 0)
        {
            
        }
        
        public string WebhookUrl { get; set; }
        public string BotToken { get; set; }

        public string Socks5Host { get; set; }

        public int Socks5Port { get; set; }
    }
}