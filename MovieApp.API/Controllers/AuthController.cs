using Microsoft.AspNetCore.Mvc;
using MovieApp.Application.DTOs;
using MovieApp.Application.Services;

namespace MovieApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResult>> Register(RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                if (!result.Success)
                {
                    _logger.LogWarning("Registration failed for email {Email}: {Errors}", 
                        request.Email, string.Join(", ", result.Errors));
                    return BadRequest(result);
                }

                _logger.LogInformation("User registered successfully: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email {Email}", request.Email);
                return StatusCode(500, new AuthResult 
                { 
                    Success = false, 
                    Token = null, 
                    Errors = new[] { "An error occurred during registration" } 
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResult>> Login(LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                if (!result.Success)
                {
                    _logger.LogWarning("Login failed for email {Email}: {Errors}", 
                        request.Email, string.Join(", ", result.Errors));
                    return BadRequest(result);
                }

                _logger.LogInformation("User logged in successfully: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}", request.Email);
                return StatusCode(500, new AuthResult 
                { 
                    Success = false, 
                    Token = null, 
                    Errors = new[] { "An error occurred during login" } 
                });
            }
        }
    }
} 