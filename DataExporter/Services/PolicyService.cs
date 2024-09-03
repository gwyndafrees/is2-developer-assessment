using DataExporter.Dtos;

namespace DataExporter.Services;

public class PolicyService : IPolicyService
{
    private readonly ExporterDbContext _dbContext;

    public PolicyService(ExporterDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureCreated();
    }

    /// <summary>
    /// Creates a new policy from the DTO.
    /// </summary>
    /// <param name="createPolicyDto"></param>
    /// <returns>Returns a ReadPolicyDto representing the new policy, if succeeded. Returns null, otherwise.</returns>
    public async Task<ReadPolicyDto?> CreatePolicyAsync(CreatePolicyDto createPolicyDto)
    {
        return await Task.FromResult(new ReadPolicyDto());
    }

    /// <summary>
    /// Retrieves all policies.
    /// </summary>
    /// <returns>Returns a list of ReadPoliciesDto.</returns>
    public async Task<IList<ReadPolicyDto>> ReadPoliciesAsync()
    {
        return await Task.FromResult(new List<ReadPolicyDto>());
    }

    /// <summary>
    /// Retrieves a policy by id.
    /// </summary>
    /// <param name="id">The id of the policy to retrieve.</param>
    /// <param name="cancellationToken">An optional cancellation token to be forwarded to any subsequent async operations.</param>
    /// <returns>Returns a ReadPolicyDto.</returns>
    public async Task<ReadPolicyDto?> ReadPolicyAsync(int id, CancellationToken cancellationToken = default)
    {
        // Could have used SingleOrDefaultAsync instead of the SingleAsync, but FindAsync is more fluent and performant since it's
        // specifically optimised for this operation, and it doesn't have to allocate a delegate on the heap.
        // Also forwarding the cancellation token to propagate a cancellation should a token have been passed in.
        var policy = await _dbContext.Policies.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (policy == null)
        {
            return null;
        }

        // Throwing in the event of a cancellation to break the application flow
        cancellationToken.ThrowIfCancellationRequested();
                
        // Combined the DTO initialisation and the return statement to improve readability by reducing redundant code since both
        // approaches result in the same IL code
        return new ReadPolicyDto
        {
            Id = policy.Id,
            PolicyNumber = policy.PolicyNumber,
            Premium = policy.Premium,
            StartDate = policy.StartDate
        };
    }
}