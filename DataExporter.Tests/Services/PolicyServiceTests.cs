using DataExporter.Dtos;
using DataExporter.Model;
using DataExporter.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DataExporter.Tests.Services;

public class PolicyServiceTests
{
    private readonly PolicyService _policyService;
    private readonly ExporterDbContext _dbContext;
    private readonly ILogger<PolicyService> _logger = Substitute.For<ILogger<PolicyService>>();
    
    public PolicyServiceTests()
    {
        var options = new DbContextOptionsBuilder<ExporterDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        
        _dbContext = new ExporterDbContext(options);
        _policyService = new PolicyService(_dbContext, _logger);
    }
    
    [Fact]
    public async Task ReadPolicyAsync_ShouldReturnReadPolicyDto_WhenPolicyExists()
    {
        // Arrange
        var policy = new Policy
        {
            PolicyNumber = "123",
            Premium = 100
        };
        
        _dbContext.Policies.Add(policy);
        await _dbContext.SaveChangesAsync();
        
        var expectedResponse = new ReadPolicyDto
        {
            Id = policy.Id,
            PolicyNumber = policy.PolicyNumber,
            Premium = policy.Premium,
            StartDate = policy.StartDate
        };
        
        // Act
        var result = await _policyService.ReadPolicyAsync(policy.Id);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task ReadPolicyAsync_ShouldReturnNull_WhenPolicyDoesNotExist()
    {
        // Arrange
        
        // Getting the maximum id and incrementing it by 1 to account for policy seeding
        var id = _dbContext.Policies.Max(p => p.Id) + 1;
        
        // Act
        var result = await _policyService.ReadPolicyAsync(id);
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task ReadPolicyAsync_ShouldThrowOperationCanceledException_WhenCancellationTokenIsCancelled()
    {
        // Arrange
        var policy = new Policy
        {
            PolicyNumber = "123",
            Premium = 100
        };
        
        _dbContext.Policies.Add(policy);
        await _dbContext.SaveChangesAsync();
        
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        // Act
        Func<Task> act = async () => await _policyService.ReadPolicyAsync(policy.Id, cancellationTokenSource.Token);
        
        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
    
    [Fact]
    public async Task ReadPoliciesAsync_ShouldReturnListOfReadPolicyDto_WhenPoliciesExist()
    {
        // Arrange
        var policy1 = new Policy
        {
            PolicyNumber = "123",
            Premium = 100
        };
        
        var policy2 = new Policy
        {
            PolicyNumber = "456",
            Premium = 200
        };
        
        _dbContext.Policies.AddRange(policy1, policy2);
        await _dbContext.SaveChangesAsync();

        var policy1Dto = new ReadPolicyDto
        {
            Id = policy1.Id,
            PolicyNumber = policy1.PolicyNumber,
            Premium = policy1.Premium,
            StartDate = policy1.StartDate
        };
        
        var policy2Dto = new ReadPolicyDto
        {
            Id = policy2.Id,
            PolicyNumber = policy2.PolicyNumber,
            Premium = policy2.Premium,
            StartDate = policy2.StartDate
        };
        
        // Act
        var result = await _policyService.ReadPoliciesAsync();
        
        // Assert
        result.Should().NotBeNull();
        result.Should().ContainEquivalentOf(policy1Dto);
        result.Should().ContainEquivalentOf(policy2Dto);
    }
    
    [Fact]
    public async Task ReadPoliciesAsync_ShouldReturnEmptyList_WhenNoPoliciesExist()
    {
        // Arrange
        _dbContext.Policies.RemoveRange(_dbContext.Policies);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _policyService.ReadPoliciesAsync();
        
        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task CreatePolicyAsync_ShouldReturnReadPolicyDto_WhenPolicyIsCreated()
    {
        // Arrange
        var createPolicyDto = new CreatePolicyDto
        {
            PolicyNumber = "123",
            Premium = 100,
            StartDate = DateTime.Now
        };
        
        // Act
        var result = await _policyService.CreatePolicyAsync(createPolicyDto);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ReadPolicyDto>();
        result.PolicyNumber.Should().Be(createPolicyDto.PolicyNumber);
        result.Premium.Should().Be(createPolicyDto.Premium);
        result.StartDate.Should().Be(createPolicyDto.StartDate);
    }
    
    [Fact]
    public async Task CreatePolicyAsync_ShouldReturnNull_WhenPolicyCreationFails()
    {
        // Arrange
        var createPolicyDto = new CreatePolicyDto
        {
            PolicyNumber = null!,
            Premium = 100,
            StartDate = DateTime.Now
        };
        
        // Act
        var result = await _policyService.CreatePolicyAsync(createPolicyDto);
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task CreatePolicyAsync_ShouldLogError_WhenPolicyCreationFails()
    {
        // Arrange
        var createPolicyDto = new CreatePolicyDto
        {
            PolicyNumber = null!,
            Premium = 100,
            StartDate = DateTime.Now
        };
        
        await _dbContext.SaveChangesAsync();
        
        // Act
        Func<Task> act = async () => await _policyService.CreatePolicyAsync(createPolicyDto);
        
        // Assert
        await act.Should().NotThrowAsync<Exception>();
        _logger.Received().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>()!);
    }
}