using Microsoft.AspNetCore.Mvc;
using ScheduleX.Core.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Web.Controllers.Admin;

[Route("api/admin/change-password")]
[ApiController]
public class ChangePasswordController : ControllerBase
{
    private readonly IChangePasswordRepository _repository;
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ChangePasswordController(
        IChangePasswordRepository repository,
        IDbContextFactory<AppDbContext> contextFactory)
    {
        _repository = repository;
        _contextFactory = contextFactory;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var admin = await context.Users
            .FirstOrDefaultAsync(x =>
                x.Role == ScheduleX.Core.Entities.UserRole.Admin &&
                x.IsActive);

        if (admin == null)
            return BadRequest("Admin not found");

        var result = await _repository.ChangePasswordAsync(
            admin.UserId,
            request.CurrentPassword,
            request.NewPassword);

        if (!result)
            return BadRequest("Invalid current password");

        return Ok("Password changed successfully");
    }
}