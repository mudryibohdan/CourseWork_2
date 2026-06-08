using AutoMapper;
using CourseWork.Api.Extensions;
using CourseWork.Api.Models;
using CourseWork.Business.DTOs;
using CourseWork.Business.Services;
using CourseWork.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToursController : ControllerBase
{
    private readonly ITourService _tourService;
    private readonly IMapper _mapper;

    public ToursController(ITourService tourService, IMapper mapper)
    {
        _tourService = tourService;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<TourResponse>>> Search([FromQuery] TourSearchRequest request, CancellationToken cancellationToken)
    {
        var filter = _mapper.Map<TourSearchFilterDto>(request);
        var tours = await _tourService.SearchAsync(filter, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<TourResponse>>(tours));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<TourResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var tour = await _tourService.GetByIdAsync(id, cancellationToken);
        return Ok(_mapper.Map<TourResponse>(tour));
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<ActionResult<TourResponse>> Create([FromBody] CreateTourRequest request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<CreateTourDto>(request);
        var tour = await _tourService.CreateAsync(dto, HttpContext.GetUserRole(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = tour.Id }, _mapper.Map<TourResponse>(tour));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<ActionResult<TourResponse>> Update(int id, [FromBody] UpdateTourRequest request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<UpdateTourDto>(request);
        var tour = await _tourService.UpdateAsync(id, dto, HttpContext.GetUserRole(), cancellationToken);
        return Ok(_mapper.Map<TourResponse>(tour));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _tourService.DeleteAsync(id, HttpContext.GetUserRole(), cancellationToken);
        return NoContent();
    }
}
