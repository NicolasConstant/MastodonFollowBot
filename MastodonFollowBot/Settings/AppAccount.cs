using Mastonet.Entities;

namespace MastodonFollowBot.Settings
{
    public class AppAccount
    {
        public string InstanceName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public AppRegistration AppRegistration { get; set; }
    }
}