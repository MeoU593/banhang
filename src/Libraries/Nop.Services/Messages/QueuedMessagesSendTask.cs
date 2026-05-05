using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Messages;

/// <summary>
/// Represents a task for sending queued message 
/// </summary>
public partial class QueuedMessagesSendTask : IScheduleTask
{
    #region Fields

    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IEmailSender _emailSender;
    protected readonly ILogger _logger;
    protected readonly IQueuedEmailService _queuedEmailService;
    private static readonly char[] _separator = [';'];

    #endregion

    #region Ctor

    public QueuedMessagesSendTask(IEmailAccountService emailAccountService,
        IEmailSender emailSender,
        ILogger logger,
        IQueuedEmailService queuedEmailService)
    {
        _emailAccountService = emailAccountService;
        _emailSender = emailSender;
        _logger = logger;
        _queuedEmailService = queuedEmailService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
        await Task.CompletedTask;
    }

    #endregion
}
