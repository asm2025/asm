﻿namespace essentialMix.Caching
{
    public interface ICacheObject
    {
        string Validator { get; set; }
        byte[] Item { get; set; }
    }
}