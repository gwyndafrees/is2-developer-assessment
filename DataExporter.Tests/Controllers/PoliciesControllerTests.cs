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
    private readonly DateTime _now = new(2024, 1, 1);
    
    private const string TestPolicyNumber = "123";
    private const int TestPremium = 100;

    public PoliciesControllerTests()
    {
        _controller = new PoliciesController(_policyService);
    }
    
    [Fact]
    public async Task GetPolicy_ShouldReturnOkObjectResult_WhenPolicyExists()
    {
        // Arrange
        const int policyId = 1;
        var readPolicyDto = new ReadPolicyDto
        {
            Id = policyId,
            PolicyNumber = TestPolicyNumber,
            Premium = TestPremium,
            StartDate = _now
        };
    
        _policyService.ReadPolicyAsync(policyId).Returns(readPolicyDto);
    
        // Act
        var result = await _controller.GetPolicy(policyId);
    
        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPolicy_ShouldReturnExpectedPolicy_WhenPolicyExists()
    {
        // Arrange
        const int policyId = 1;
        var readPolicyDto = new ReadPolicyDto
        {
            Id = policyId,
            PolicyNumber = TestPolicyNumber,
            Premium = TestPremium,
            StartDate = _now
        };
    
        _policyService.ReadPolicyAsync(policyId).Returns(readPolicyDto);
    
        // Act
        var result = await _controller.GetPolicy(policyId);
    
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
            PolicyNumber = TestPolicyNumber,
            Premium = TestPremium,
            StartDate = _now
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
            PolicyNumber = TestPolicyNumber,
            Premium = TestPremium,
            StartDate = _now
        };

        var expectedResponse = new List<ReadPolicyDto> { readPolicyDto };
        
        _policyService.ReadPoliciesAsync().Returns(expectedResponse);
    
        // Act
        var result = await _controller.GetPolicies();
    
        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.Value.Should().NotBeNull();
        okResult.Value.Should().BeEquivalentTo(expectedResponse);
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
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.Value.Should().BeOfType<List<ReadPolicyDto>>();
        
        var resultList = okResult.Value as List<ReadPolicyDto>;
        resultList.Should().NotBeNull();
        resultList.Should().BeEmpty();
    }
    
    [Fact]
    public async Task ExportData_ShouldReturnOkObjectResult_WhenPoliciesExistWithinDateRange()
    {
        // Arrange
        var startDate = _now;
        var endDate = _now.AddDays(1);
        var exportDto = new ExportDto
        {
            PolicyNumber = TestPolicyNumber,
            Premium = TestPremium,
            StartDate = _now
        };
    
        _policyService.ExportDataAsync(startDate, endDate).Returns(new List<ExportDto> { exportDto });
    
        // Act
        var result = await _controller.ExportData(startDate, endDate);
    
        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task ExportData_ShouldReturnExpectedPolicies_WhenPoliciesExistWithinDateRange()
    {
        // Arrange
        var startDate = _now;
        var endDate = _now.AddDays(1);
        var exportDto = new ExportDto
        {
            PolicyNumber = TestPolicyNumber,
            Premium = TestPremium,
            StartDate = _now
        };

        var expectedResponse = new List<ExportDto> { exportDto };
        
        _policyService.ExportDataAsync(startDate, endDate).Returns(expectedResponse);
    
        // Act
        var result = await _controller.ExportData(startDate, endDate);
    
        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.Value.Should().NotBeNull();
        okResult.Value.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task ExportData_ShouldReturnOkObjectResult_WhenNoPoliciesExistWithinDateRange()
    {
        // Arrange
        var startDate = _now;
        var endDate = _now.AddDays(1);
        
        _policyService.ExportDataAsync(startDate, endDate).Returns(new List<ExportDto>());
    
        // Act
        var result = await _controller.ExportData(startDate, endDate);
    
        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task ExportData_ShouldReturnEmptyList_WhenNoPoliciesExistWithinDateRange()
    {
        // Arrange
        var startDate = _now;
        var endDate = _now.AddDays(1);
        
        _policyService.ExportDataAsync(startDate, endDate).Returns(new List<ExportDto>());
    
        // Act
        var result = await _controller.ExportData(startDate, endDate);
    
        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.Value.Should().BeOfType<List<ExportDto>>();
        
        var resultList = okResult.Value as List<ExportDto>;
        resultList.Should().NotBeNull();
        resultList.Should().BeEmpty();
    }
}