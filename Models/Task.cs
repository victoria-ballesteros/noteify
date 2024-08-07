using System;
using System.Collections.Generic;

namespace Application.Models;

public partial class Task
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public byte[] IsDone { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiredAt { get; set; }

    public Guid ListId { get; set; }

    public virtual List List { get; set; } = null!;
}
