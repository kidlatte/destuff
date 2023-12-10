using Destuff.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destuff.Tests;

public class MockDateTimeProvider : IDateTimeProvider
{
    private DateTime? MockUtcNow { get; set; }

    public DateTime UtcNow => MockUtcNow ?? DateTime.UtcNow;

    public void SetUtcNow(DateTime utcNow) => MockUtcNow = utcNow;
}
