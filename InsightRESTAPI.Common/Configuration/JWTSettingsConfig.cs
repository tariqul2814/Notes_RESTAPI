using System;
using System.Collections.Generic;
using System.Text;

namespace InsightRESTAPI.Common.Configuration
{
    public class JWTSettingsConfig
    {
        public string Secret { get; set; }
        public int ExpiresInMinutes { get; set; }
    }
}
