using DataExporter.Dtos;
using DataExporter.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataExporter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PoliciesController : ControllerBase
    {
        private readonly IPolicyService _policyService;

        public PoliciesController(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        [HttpPost]
        public async Task<IActionResult> PostPolicies([FromBody]CreatePolicyDto createPolicyDto)
        {
            var result = await _policyService.CreatePolicyAsync(createPolicyDto);
            return result != null ? Ok(result) : BadRequest();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPolicies()
        {
            return Ok(await _policyService.ReadPoliciesAsync());
        }

        [HttpGet("{policyId}")]
        public async Task<IActionResult> GetPolicy(int policyId)
        {
            // Awaited the async method
            var result = await _policyService.ReadPolicyAsync(policyId);
            return result == null ? NotFound() : Ok(result);
        }
        
        [HttpPost("export")]
        public async Task<IActionResult> ExportData([FromQuery]DateTime startDate, [FromQuery] DateTime endDate)
        {
            return Ok();
        }
    }
}
