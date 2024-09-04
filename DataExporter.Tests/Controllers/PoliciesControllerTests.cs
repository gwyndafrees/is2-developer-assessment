using DataExporter.Controllers;
using DataExporter.Dtos;
using DataExporter.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ReturnsExtensions;

namespace DataExporter.Tests.Controllers;

public class PoliciesControllerTests
{
    private readonly IPolicyService _policyService = Substitute.For<IPolicyService>();
    private readonly PoliciesController _controller;

    public PoliciesControllerTests()
    {
        _controller = new PoliciesController(_policyService);
    }
    
    [Fact]
    public async Task GetPolicy_ShouldReturnOkObjectResult_WhenPolicyExists()
    {
        // Arrange
        var readPolicyDto = new ReadPolicyDto
        {
            PolicyNumber = "123",
            Premium = 100,
            StartDate = DateTime.Now
        };
    
        _policyService.ReadPolicyAsync(readPolicyDto.Id).Returns(readPolicyDto);
    
        // Act
        var result = await _controller.GetPolicy(readPolicyDto.Id);
    
        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPolicy_ShouldReturnExpectedPolicy_WhenPolicyExists()
    {
        // Arrange
        var readPolicyDto = new ReadPolicyDto
        {
            PolicyNumber = "123",
            Premium = 100,
            StartDate = DateTime.Now
        };
    
        _policyService.ReadPolicyAsync(readPolicyDto.Id).Returns(readPolicyDto);
    
        // Act
        var result = await _controller.GetPolicy(readPolicyDto.Id);
    
        // Assert
        var resultValue = (result as OkObjectResult)!.Value;
        resultValue.Should().NotBeNull();
        resultValue.Should().BeEquivalentTo(readPolicyDto);
    }

    [Fact]
    public async Task GetPolicy_ShouldReturnNotFound_WhenPolicyDoesNotExist()
    {
        // Arrange
        const int policyId = 1;
        _policyService.ReadPolicyAsync(policyId).ReturnsNull();
        
        // Act
        var result = await _controller.GetPolicy(policyId);
        
        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
    
    [Fact]
    public async Task GetPolicies_ShouldReturnOkObjectResult_WhenPoliciesExist()
    {
        // Arrange
        var readPolicyDto = new ReadPolicyDto
        {
            PolicyNumber = "123",
            Premium = 100,
            StartDate = DateTime.Now
        };
    
        _policyService.ReadPoliciesAsync().Returns(new List<ReadPolicyDto> { readPolicyDto });
    
        // Act
        var result = await _controller.GetPolicies();
    
        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetPolicies_ShouldReturnExpectedPolicies_WhenPoliciesExist()
    {
        // Arrange
        var readPolicyDto = new ReadPolicyDto
        {
            PolicyNumber = "123",
            Premium = 100,
            StartDate = DateTime.Now
        };

        var expectedResponse = new List<ReadPolicyDto> { readPolicyDto };
        
        _policyService.ReadPoliciesAsync().Returns(expectedResponse);
    
        // Act
        var result = await _controller.GetPolicies();
    
        // Assert
        var resultValue = (result as OkObjectResult)!.Value;
        resultValue.Should().NotBeNull();
        resultValue.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task GetPolicies_ShouldReturnOkObjectResult_WhenNoPoliciesExist()
    {
        // Arrange
        _policyService.ReadPoliciesAsync().Returns(new List<ReadPolicyDto>());
        
        // Act
        var result = await _controller.GetPolicies();
        
        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetPolicies_ShouldReturnEmptyList_WhenNoPoliciesExist()
    {
        // Arrange
        _policyService.ReadPoliciesAsync().Returns(new List<ReadPolicyDto>());
        
        // Act
        var result = await _controller.GetPolicies();
        
        // Assert
        var resultValue = (result as OkObjectResult)!.Value;
        resultValue.Should().BeOfType<List<ReadPolicyDto>>();
        
        var resultList = resultValue as List<ReadPolicyDto>;
        resultList.Should().NotBeNull();
        resultList.Count.Should().Be(0);
    }
}