﻿namespace Checkout.Business.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string? message) : base(message) { }
    }
}
