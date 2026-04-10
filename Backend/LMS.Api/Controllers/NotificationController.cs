using LMS.Application.Common;
using LMS.Application.DTOs.Notification;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<NotificationResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyNotifications()
    {
        var result = await _notificationService.GetMyNotificationsAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(BaseResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var result = await _notificationService.GetUnreadCountAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{notificationId:guid}/read")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAsRead(Guid notificationId)
    {
        var result = await _notificationService.MarkAsReadAsync(notificationId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("read-all")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var result = await _notificationService.MarkAllAsReadAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
