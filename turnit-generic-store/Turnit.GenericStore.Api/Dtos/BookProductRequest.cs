using System;

namespace Turnit.GenericStore.Api.Dtos
{
    public class BookProductRequest
    {
        public Guid StoreId { get; set; }
        public int Amount { get; set; }
    }
}
