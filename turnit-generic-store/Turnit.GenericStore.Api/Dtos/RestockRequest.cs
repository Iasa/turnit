using System;
using System.Collections.Generic;

namespace Turnit.GenericStore.Api.Dtos
{
    public class RestockRequest
    {
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
    }
}
