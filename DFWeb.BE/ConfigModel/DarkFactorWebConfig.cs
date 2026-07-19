
using DFCommonLib.Config;

namespace DFWeb.BE.ConfigModel
{
    public class AccountServerConfig
    {
        public string Endpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }

    public class WebConfig : AppSettings
    {
        public AccountServerConfig AccountServer { get; set; }
    }
}
