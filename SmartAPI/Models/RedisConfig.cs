﻿namespace SmartAPI.Models
{ 
    public class RedisConfig
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int ConnectTimeout { get; set; }
    }
}
