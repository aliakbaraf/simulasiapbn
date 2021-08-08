/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.Collections.Generic;
using System.Threading.Tasks;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Application.Repositories
{
    public interface ISpecialPolicyAllocationRepository : ISpecialPolicyRepository<IRepositoryOptions> {}

    public interface ISpecialPolicyAllocationRepository<out TRepositoryOptions>
	    : IGenericRepository<SpecialPolicyAllocation, TRepositoryOptions>
	    where TRepositoryOptions : IRepositoryOptions
    {
	    
	    IEnumerable<SpecialPolicyAllocation> GetByAllocation(Allocation allocation);
        
	    Task<IEnumerable<SpecialPolicyAllocation>> GetByAllocationAsync(Allocation allocation);
        
	    IEnumerable<SpecialPolicyAllocation> GetBySpecialPolicy(SpecialPolicy specialPolicy);
        
	    Task<IEnumerable<SpecialPolicyAllocation>> GetBySpecialPolicyAsync(SpecialPolicy specialPolicy);
    }
}