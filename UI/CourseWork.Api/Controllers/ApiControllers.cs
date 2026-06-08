using AutoMapper;
using CourseWork.Api.Extensions;
using CourseWork.Api.Models;
using CourseWork.Business.DTOs;
using CourseWork.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public AuthController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.RegisterAsync(_mapper.Map<RegisterUserDto>(request), cancellationToken);
        return Ok(_mapper.Map<AuthResponse>(result));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.LoginAsync(_mapper.Map<LoginDto>(request), cancellationToken);
        return Ok(_mapper.Map<AuthResponse>(result));
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IMapper _mapper;

    public BookingsController(IBookingService bookingService, IMapper mapper)
    {
        _bookingService = bookingService;
        _mapper = mapper;
    }

    [HttpPost("tours")]
    public async Task<ActionResult<TourBookingResponse>> BookTour([FromBody] CreateTourBookingRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId() ?? throw new InvalidOperationException();
        var booking = await _bookingService.BookTourAsync(
            userId,
            _mapper.Map<CreateTourBookingDto>(request),
            HttpContext.GetUserRole(),
            cancellationToken);

        return Ok(_mapper.Map<TourBookingResponse>(booking));
    }

    [HttpGet("tours")]
    public async Task<ActionResult<IReadOnlyList<TourBookingResponse>>> GetMyTourBookings(CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId() ?? throw new InvalidOperationException();
        var bookings = await _bookingService.GetUserBookingsAsync(userId, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<TourBookingResponse>>(bookings));
    }

    [HttpDelete("tours/{id:int}")]
    public async Task<IActionResult> CancelTourBooking(int id, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId() ?? throw new InvalidOperationException();
        await _bookingService.CancelBookingAsync(id, userId, HttpContext.GetUserRole(), cancellationToken);
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketBookingsController : ControllerBase
{
    private readonly ITicketBookingService _ticketBookingService;
    private readonly IMapper _mapper;

    public TicketBookingsController(ITicketBookingService ticketBookingService, IMapper mapper)
    {
        _ticketBookingService = ticketBookingService;
        _mapper = mapper;
    }

    [HttpPost("transport")]
    public async Task<ActionResult<TicketBookingResponse>> BookTransport([FromBody] CreateTransportBookingRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId() ?? throw new InvalidOperationException();
        var booking = await _ticketBookingService.BookTransportAsync(
            userId,
            _mapper.Map<CreateTransportTicketBookingDto>(request),
            HttpContext.GetUserRole(),
            cancellationToken);

        return Ok(_mapper.Map<TicketBookingResponse>(booking));
    }

    [HttpPost("hotel")]
    public async Task<ActionResult<TicketBookingResponse>> BookHotelRoom([FromBody] CreateHotelRoomBookingRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId() ?? throw new InvalidOperationException();
        var booking = await _ticketBookingService.BookHotelRoomAsync(
            userId,
            _mapper.Map<CreateHotelRoomBookingDto>(request),
            HttpContext.GetUserRole(),
            cancellationToken);

        return Ok(_mapper.Map<TicketBookingResponse>(booking));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketBookingResponse>>> GetMyBookings(CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId() ?? throw new InvalidOperationException();
        var bookings = await _ticketBookingService.GetUserBookingsAsync(userId, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<TicketBookingResponse>>(bookings));
    }
}

[ApiController]
[Route("api/[controller]")]
public class ReferenceController : ControllerBase
{
    private readonly IReferenceDataService _referenceDataService;
    private readonly IMapper _mapper;

    public ReferenceController(IReferenceDataService referenceDataService, IMapper mapper)
    {
        _referenceDataService = referenceDataService;
        _mapper = mapper;
    }

    [HttpGet("countries")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<CountryResponse>>> GetCountries(CancellationToken cancellationToken)
    {
        var countries = await _referenceDataService.GetCountriesAsync(cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<CountryResponse>>(countries));
    }

    [HttpGet("transports")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<TransportResponse>>> GetTransports(CancellationToken cancellationToken)
    {
        var transports = await _referenceDataService.GetTransportsAsync(cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<TransportResponse>>(transports));
    }

    [HttpGet("hotel-rooms")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<HotelRoomResponse>>> GetHotelRooms(CancellationToken cancellationToken)
    {
        var rooms = await _referenceDataService.GetAvailableHotelRoomsAsync(cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<HotelRoomResponse>>(rooms));
    }
}
