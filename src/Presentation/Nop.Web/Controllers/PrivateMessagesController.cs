using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class PrivateMessagesController : BasePublicController
{
    #region Fields

    protected readonly ForumSettings _forumSettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IForumService _forumService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;
    protected readonly IStoreContext _storeContext;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public PrivateMessagesController(ForumSettings forumSettings,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IForumService forumService,
        ILocalizationService localizationService,
        IPrivateMessagesModelFactory privateMessagesModelFactory,
        IStoreContext storeContext,
        IVendorService vendorService,
        IWorkContext workContext)
    {
        _forumSettings = forumSettings;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _forumService = forumService;
        _localizationService = localizationService;
        _privateMessagesModelFactory = privateMessagesModelFactory;
        _storeContext = storeContext;
        _vendorService = vendorService;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> Index(int? pageNumber, string tab)
    {
        if (!_forumSettings.AllowPrivateMessages)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var model = await _privateMessagesModelFactory.PreparePrivateMessageIndexModelAsync(pageNumber, tab);
        return View(model);
    }

    [HttpPost, FormValueRequired("delete-inbox"), ActionName("InboxUpdate")]
    public virtual async Task<IActionResult> DeleteInboxPM(IFormCollection formCollection)
    {
        foreach (var key in formCollection.Keys)
        {
            var value = formCollection[key];

            if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
            {
                var id = key.Replace("pm", "").Trim();
                if (int.TryParse(id, out var privateMessageId))
                {
                    var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                    if (pm != null)
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();

                        if (pm.ToCustomerId == customer.Id)
                        {
                            pm.IsDeletedByRecipient = true;
                            await _forumService.UpdatePrivateMessageAsync(pm);
                        }
                    }
                }
            }
        }
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
    }

    [HttpPost, FormValueRequired("mark-unread"), ActionName("InboxUpdate")]
    public virtual async Task<IActionResult> MarkUnread(IFormCollection formCollection)
    {
        foreach (var key in formCollection.Keys)
        {
            var value = formCollection[key];

            if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
            {
                var id = key.Replace("pm", "").Trim();
                if (int.TryParse(id, out var privateMessageId))
                {
                    var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                    if (pm != null)
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();

                        if (pm.ToCustomerId == customer.Id)
                        {
                            pm.IsRead = false;
                            await _forumService.UpdatePrivateMessageAsync(pm);
                        }
                    }
                }
            }
        }
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
    }

    //updates sent items (deletes PrivateMessages)
    [HttpPost, FormValueRequired("delete-sent"), ActionName("SentUpdate")]
    public virtual async Task<IActionResult> DeleteSentPM(IFormCollection formCollection)
    {
        foreach (var key in formCollection.Keys)
        {
            var value = formCollection[key];

            if (value.Equals("on") && key.StartsWith("si", StringComparison.InvariantCultureIgnoreCase))
            {
                var id = key.Replace("si", "").Trim();
                if (int.TryParse(id, out var privateMessageId))
                {
                    var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
                    if (pm != null)
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();

                        if (pm.FromCustomerId == customer.Id)
                        {
                            pm.IsDeletedByAuthor = true;
                            await _forumService.UpdatePrivateMessageAsync(pm);
                        }
                    }
                }
            }
        }
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES, new { tab = "sent" });
    }

    public virtual async Task<IActionResult> SendPM(int toCustomerId, int? replyToMessageId)
    {
        if (!_forumSettings.AllowPrivateMessages)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var customerTo = await _customerService.GetCustomerByIdAsync(toCustomerId);
        if (customerTo == null || await _customerService.IsGuestAsync(customerTo))
            return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);

        PrivateMessage replyToPM = null;
        if (replyToMessageId.HasValue)
        {
            //reply to a previous PM
            replyToPM = await _forumService.GetPrivateMessageByIdAsync(replyToMessageId.Value);
        }

        var model = await _privateMessagesModelFactory.PrepareSendPrivateMessageModelAsync(customerTo, replyToPM);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> SendPM(SendPrivateMessageModel model)
    {
        if (!_forumSettings.AllowPrivateMessages)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        Customer toCustomer;
        var replyToPM = await _forumService.GetPrivateMessageByIdAsync(model.ReplyToMessageId);
        if (replyToPM != null)
        {
            //reply to a previous PM
            if (replyToPM.ToCustomerId == customer.Id || replyToPM.FromCustomerId == customer.Id)
            {
                //Reply to already sent PM (by current customer) should not be sent to yourself
                toCustomer = await _customerService.GetCustomerByIdAsync(replyToPM.FromCustomerId == customer.Id
                    ? replyToPM.ToCustomerId
                    : replyToPM.FromCustomerId);
            }
            else
                return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
        }
        else
        {
            //first PM
            toCustomer = await _customerService.GetCustomerByIdAsync(model.ToCustomerId);
        }

        if (toCustomer == null || await _customerService.IsGuestAsync(toCustomer))
            return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);

        if (ModelState.IsValid)
        {
            try
            {
                var subject = model.Subject;
                if (_forumSettings.PMSubjectMaxLength > 0 && subject.Length > _forumSettings.PMSubjectMaxLength)
                    subject = subject[0.._forumSettings.PMSubjectMaxLength];

                var text = model.Message;
                if (_forumSettings.PMTextMaxLength > 0 && text.Length > _forumSettings.PMTextMaxLength)
                    text = text[0.._forumSettings.PMTextMaxLength];

                var nowUtc = DateTime.UtcNow;
                var store = await _storeContext.GetCurrentStoreAsync();

                var privateMessage = new PrivateMessage
                {
                    StoreId = store.Id,
                    ToCustomerId = toCustomer.Id,
                    FromCustomerId = customer.Id,
                    Subject = subject,
                    Text = text,
                    IsDeletedByAuthor = false,
                    IsDeletedByRecipient = false,
                    UnitContactVendorId = replyToPM?.UnitContactVendorId,
                    IsRead = false,
                    CreatedOnUtc = nowUtc
                };

                await _forumService.InsertPrivateMessageAsync(privateMessage);

                //activity log
                await _customerActivityService.InsertActivityAsync("PublicStore.SendPM",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.SendPM"), toCustomer.Email), toCustomer);

                return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES, new { tab = "sent" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        model = await _privateMessagesModelFactory.PrepareSendPrivateMessageModelAsync(toCustomer, replyToPM);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> UnitContactHistory(int vendorId)
    {
        var validation = await ValidateUnitContactChatAsync(vendorId);
        if (!validation.Success)
            return Json(new { success = false, validation.LoginRequired, validation.Message });

        var store = await _storeContext.GetCurrentStoreAsync();
        var messages = await _forumService.GetUnitContactPrivateMessagesAsync(validation.Vendor.Id, validation.Customer.Id, validation.ContactCustomer.Id, store.Id);

        foreach (var message in messages.Where(message => message.ToCustomerId == validation.Customer.Id && !message.IsRead))
        {
            message.IsRead = true;
            await _forumService.UpdatePrivateMessageAsync(message);
        }

        return Json(new
        {
            success = true,
            contactCustomerName = await _customerService.FormatUsernameAsync(validation.ContactCustomer),
            messages = await PrepareUnitContactMessagesAsync(messages, validation.Customer.Id)
        });
    }

    [HttpPost]
    public virtual async Task<IActionResult> UnitContactSend(int vendorId, string message)
    {
        var validation = await ValidateUnitContactChatAsync(vendorId);
        if (!validation.Success)
            return Json(new { success = false, validation.LoginRequired, validation.Message });

        message = (message ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(message))
            return Json(new { success = false, message = "Vui lòng nhập nội dung tin nhắn." });

        if (_forumSettings.PMTextMaxLength > 0 && message.Length > _forumSettings.PMTextMaxLength)
            message = message[0.._forumSettings.PMTextMaxLength];

        var subject = $"Liên hệ đơn vị: {validation.Vendor.Name}";
        if (_forumSettings.PMSubjectMaxLength > 0 && subject.Length > _forumSettings.PMSubjectMaxLength)
            subject = subject[0.._forumSettings.PMSubjectMaxLength];

        var store = await _storeContext.GetCurrentStoreAsync();
        var privateMessage = new PrivateMessage
        {
            StoreId = store.Id,
            ToCustomerId = validation.ContactCustomer.Id,
            FromCustomerId = validation.Customer.Id,
            Subject = subject,
            Text = message,
            IsDeletedByAuthor = false,
            IsDeletedByRecipient = false,
            UnitContactVendorId = validation.Vendor.Id,
            IsRead = false,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _forumService.InsertPrivateMessageAsync(privateMessage);

        await _customerActivityService.InsertActivityAsync("PublicStore.SendUnitContactPM",
            $"Sent a unit contact private message to {validation.ContactCustomer.Email}", validation.ContactCustomer);

        return Json(new
        {
            success = true,
            message = await PrepareUnitContactMessageAsync(privateMessage, validation.Customer.Id)
        });
    }

    protected virtual async Task<(bool Success, bool LoginRequired, string Message, Vendor Vendor, Customer Customer, Customer ContactCustomer)> ValidateUnitContactChatAsync(int vendorId)
    {
        if (!_forumSettings.AllowPrivateMessages)
            return (false, false, "Tin nhắn riêng đang tắt.", null, null, null);

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
            return (false, true, "Vui lòng đăng nhập để liên hệ đơn vị.", null, customer, null);

        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
        if (vendor == null || vendor.Deleted || !vendor.Active)
            return (false, false, "Không tìm thấy đơn vị.", vendor, customer, null);

        if (!vendor.ContactCustomerId.HasValue)
            return (false, false, "Đơn vị chưa thiết lập phụ trách liên hệ.", vendor, customer, null);

        if (vendor.ContactCustomerId.Value == customer.Id)
            return (false, false, "Bạn đang là phụ trách liên hệ của đơn vị này.", vendor, customer, null);

        var contactCustomer = await _customerService.GetCustomerByIdAsync(vendor.ContactCustomerId.Value);
        if (contactCustomer == null || !contactCustomer.Active || contactCustomer.Deleted || await _customerService.IsGuestAsync(contactCustomer))
            return (false, false, "Tài khoản phụ trách liên hệ không hợp lệ.", vendor, customer, null);

        return (true, false, null, vendor, customer, contactCustomer);
    }

    protected virtual async Task<IList<object>> PrepareUnitContactMessagesAsync(IList<PrivateMessage> messages, int currentCustomerId)
    {
        var result = new List<object>();
        foreach (var message in messages)
            result.Add(await PrepareUnitContactMessageAsync(message, currentCustomerId));

        return result;
    }

    protected virtual async Task<object> PrepareUnitContactMessageAsync(PrivateMessage message, int currentCustomerId)
    {
        var fromCustomer = await _customerService.GetCustomerByIdAsync(message.FromCustomerId);

        return new
        {
            id = message.Id,
            mine = message.FromCustomerId == currentCustomerId,
            author = fromCustomer == null ? "Tài khoản" : await _customerService.FormatUsernameAsync(fromCustomer),
            text = message.Text,
            createdOnUtc = message.CreatedOnUtc.ToString("O")
        };
    }

    public virtual async Task<IActionResult> ViewPM(int privateMessageId)
    {
        if (!_forumSettings.AllowPrivateMessages)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
        if (pm != null)
        {
            if (pm.ToCustomerId != customer.Id && pm.FromCustomerId != customer.Id)
                return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);

            if (!pm.IsRead && pm.ToCustomerId == customer.Id)
            {
                pm.IsRead = true;
                await _forumService.UpdatePrivateMessageAsync(pm);
            }
        }
        else
            return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);

        var model = await _privateMessagesModelFactory.PreparePrivateMessageModelAsync(pm);
        return View(model);
    }

    public virtual async Task<IActionResult> DeletePM(int privateMessageId)
    {
        if (!_forumSettings.AllowPrivateMessages)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        var pm = await _forumService.GetPrivateMessageByIdAsync(privateMessageId);
        if (pm != null)
        {
            if (pm.FromCustomerId == customer.Id)
            {
                pm.IsDeletedByAuthor = true;
                await _forumService.UpdatePrivateMessageAsync(pm);
            }

            if (pm.ToCustomerId == customer.Id)
            {
                pm.IsDeletedByRecipient = true;
                await _forumService.UpdatePrivateMessageAsync(pm);
            }
        }
        return RedirectToRoute(NopRouteNames.Standard.PRIVATE_MESSAGES);
    }

    #endregion
}
