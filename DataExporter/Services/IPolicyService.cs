using DataExporter.Dtos;

namespace DataExporter.Services;

public interface IPolicyService
{
    public Task<ReadPolicyDto?> CreatePolicyAsync(CreatePolicyDto createPolicyDto);
    public Task<IList<ReadPolicyDto>> ReadPoliciesAsync();
    public Task<ReadPolicyDto?> ReadPolicyAsync(int id, CancellationToken cancellationToken = default);
}