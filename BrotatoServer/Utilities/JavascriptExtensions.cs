using Microsoft.JSInterop;

namespace BrotatoServer.Utilities;

public enum ToastType
{
    Success,
    Warning,
    Error
}

public static class JavascriptExtensions
{
    public static void Toast(this IJSRuntime runtime, string message, ToastType toastType = ToastType.Success)
    {
        var toastTypeStr = toastType.ToString().ToLowerInvariant();

        runtime.InvokeVoidAsync($"toastr.{toastTypeStr}", message);
    }
}