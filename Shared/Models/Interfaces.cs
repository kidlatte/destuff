using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destuff.Shared.Models;

public interface IRequest
{
}

public interface IModel
{
    string Id { get; set; }
}
