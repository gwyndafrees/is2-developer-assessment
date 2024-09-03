﻿using DataExporter.Dtos;
using DataExporter.Model;
using DataExporter.Services;
using Microsoft.EntityFrameworkCore;

namespace DataExporter.Tests.Services;

public class PolicyServiceTests
{
    private readonly PolicyService _policyService;
    private readonly ExporterDbContext _dbContext;
    
    public PolicyServiceTests()
    {
        var options = new DbContextOptionsBuilder<ExporterDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _dbContext = new ExporterDbContext(options);
        _policyService = new PolicyService(_dbContext);
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
}