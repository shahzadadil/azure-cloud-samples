namespace Platform.Messages;
using System;

public class OrderCreated
{
    public Guid OrderId { get; set; }
    public DateTime CreatedOn { get; set; }
    public Double Amount { get; set; }
}
