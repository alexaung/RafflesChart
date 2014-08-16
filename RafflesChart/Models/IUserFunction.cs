using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafflesChart.Models {
    public interface IUserFunction {
        Guid UserId { get; set; }

        string FunctionName { get; set; }
    }
}
