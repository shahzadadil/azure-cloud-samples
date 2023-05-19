namespace Publisher.API.Models;

using System.ComponentModel.DataAnnotations;

public class OrderModel
{
    [Required]
    public Guid Id { get; set; }

    [Range(0, 50000)]
    public double Amount  { get; set; }
    public Int32 ScheduleOffsetSeconds { get; set; }
}
