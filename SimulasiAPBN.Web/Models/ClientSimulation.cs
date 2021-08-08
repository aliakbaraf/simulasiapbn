using SimulasiAPBN.Core.Models;
using System.Collections.Generic;

namespace SimulasiAPBN.Web.Models
{
    public class ClientSimulation
    {

        public IEnumerable<Allocation> Allocations { get; set; }
        public IEnumerable<EconomicMacro> Economics { get; set; }

        public SimulationSession Session { get; set; }

    }
}
