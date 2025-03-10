using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Wallet
{
    public long WalletId { get; set; }

    public decimal Balance { get; set; }

    public DateTime CreateDate { get; set; }

    public int Status { get; set; }

    public long CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

public class WalletDto
{
    public long WalletId { get; set; }
    public decimal Balance { get; set; }

    public DateTime CreateDate { get; set; }

    public long CustomerId { get; set; }

    public int Status { get; set; }
}
