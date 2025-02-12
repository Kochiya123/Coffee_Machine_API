using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Transaction
{
    public long TransactionId { get; set; }

    public decimal TransactionAmount { get; set; }

    public DateTime TransactionDate { get; set; }

    public int TransactionType { get; set; }

    public int Status { get; set; }

    public long WalletId { get; set; }

    public int OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Wallet Wallet { get; set; } = null!;
}
