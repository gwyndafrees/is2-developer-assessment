using DataExporter.Dtos;
using DataExporter.Model;
using Microsoft.EntityFrameworkCore;

namespace DataExporter.Services;

public class PolicyService : IPolicyService
{
    private readonly ExporterDbContext _dbContext;
    private readonly ILogger<PolicyService> _logger;

    public PolicyService(ExporterDbContext dbContext, ILogger<PolicyService> logger)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureCreated();
        _logger = logger;
    }

    /// <summary>
    /// Creates a new policy from the DTO.
    /// </summary>
    /// <param name="createPolicyDto"></param>
    /// <param name="cancellationToken">An optional cancellation token to be forwarded to any subsequent async operations.</param>
    /// <returns>Returns a ReadPolicyDto representing the new policy, if succeeded. Returns null, otherwise.</returns>
    public async Task<ReadPolicyDto?> CreatePolicyAsync(CreatePolicyDto createPolicyDto, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var policy = new Policy
            {
                PolicyNumber = createPolicyDto.PolicyNumber,
                Premium = createPolicyDto.Premium,
                StartDate = createPolicyDto.StartDate
            };
            
            await _dbContext.Policies.AddAsync(policy, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
            return new ReadPolicyDto
            {
                Id = policy.Id,
                PolicyNumber = createPolicyDto.PolicyNumber,
                Premium = createPolicyDto.Premium,
                StartDate = createPolicyDto.StartDate
            };
        }
        catch (Exception ex)
        {
            // Rollback regardless of whether the operation was cancelled or an error occurred
            await transaction.RollbackAsync(cancellationToken);
            
            if (ex is OperationCanceledException) throw;
            
            _logger.LogError(ex, "Failed to create a new policy. Policy Number: {PolicyNumber}, Premium: {Premium}, StartDate: {StartDate}",
                createPolicyDto.PolicyNumber, createPolicyDto.Premium, createPolicyDto.StartDate);
            
            // Return null as stipulated by the XML documentation `returns` specification
            return null;
        }
    }

    /// <summary>
    /// Retrieves all policies.
    /// </summary>
    /// <returns>Returns a list of ReadPoliciesDto.</returns>
    public async Task<IList<ReadPolicyDto>> ReadPoliciesAsync(CancellationToken cancellationToken = default)
    {
        var policies = await _dbContext.Policies
            .Select(x => new ReadPolicyDto
            {
                Id = x.Id,
                PolicyNumber = x.PolicyNumber,
                Premium = x.Premium,
                StartDate = x.StartDate
            })
            .ToListAsync(cancellationToken);
        
        return policies;
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
        var policy = await _dbContext.Policies
            .FindAsync(new object[] { id }, cancellationToken: cancellationToken);
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