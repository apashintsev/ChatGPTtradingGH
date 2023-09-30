﻿namespace ChatGPTtrading.Domain.Config;

public class JwtSettings
{
    public string Secret { get; set; }
    public string Subject { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}
