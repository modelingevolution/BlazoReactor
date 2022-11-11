using Microsoft.JSInterop;

namespace BlazoReactor.Clipboard
{
    public interface IClipboardService
    {
        /// <summary>
        /// Requests text from the system clipboard.
        /// </summary>
        /// <returns>
        /// A Task that resolves with a string containing the textual contents of the clipboard. A JSException is thrown if the caller does not have permission to write to the clipboard.
        /// </returns>
        Task<string> ReadTextAsync();

        /// <summary>
        /// Writes text to the system clipboard.
        /// </summary>
        /// <param name="newClipText">The string to be written to the clipboard.</param>
        /// <returns>
        /// A Task which is resolved once the text is fully copied into the clipboard. Returns an empty string if the clipboard is empty, does not contain text, or does not include a textual representation among the DataTransfer objects representing the clipboard's contents.
        /// </returns>
        Task WriteTextAsync(string newClipText);

        /// <summary>
        /// Detects if the browser supports the Clipboard API.
        /// </summary>
        /// <returns>
        /// A Task which resolves to a boolean stating if the clipboard is supported or not.
        /// </returns>
        Task<bool> IsSupportedAsync();
    }

    public class ClipboardService : IClipboardService
    {
        static readonly IDictionary<Guid, TaskCompletionSource<string>> PendingReadRequests =
            new Dictionary<Guid, TaskCompletionSource<string>>();
        static readonly IDictionary<Guid, TaskCompletionSource<object>> PendingWriteRequests =
            new Dictionary<Guid, TaskCompletionSource<object>>();
        private readonly IJSRuntime _jSRuntime;

        public ClipboardService(IJSRuntime jSRuntime)
        {
            this._jSRuntime = jSRuntime;
        }

        /// <summary>
        /// Requests text from the system clipboard.
        /// </summary>
        /// <returns>
        /// A Task that resolves with a string containing the textual contents of the clipboard. A JSException is thrown if the caller does not have permission to write to the clipboard.
        /// </returns>
        public async Task<string> ReadTextAsync()
        {
            var tcs = new TaskCompletionSource<string>();
            var requestId = Guid.NewGuid();
            PendingReadRequests.Add(requestId, tcs);
            await _jSRuntime
                .InvokeAsync<object>("ModelingEvolution.Clipboard.ReadText", requestId)
                .ConfigureAwait(false);
            return await tcs.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Writes text to the system clipboard.
        /// </summary>
        /// <param name="newClipText">The string to be written to the clipboard.</param>
        /// <returns>
        /// A Task which is resolved once the text is fully copied into the clipboard. Returns an empty string if the clipboard is empty, does not contain text, or does not include a textual representation among the DataTransfer objects representing the clipboard's contents.
        /// </returns>
        public async Task WriteTextAsync(string newClipText)
        {
            var tcs = new TaskCompletionSource<object>();
            var requestId = Guid.NewGuid();
            PendingWriteRequests.Add(requestId, tcs);
            await _jSRuntime
                .InvokeAsync<object>("ModelingEvolution.Clipboard.WriteText", requestId, newClipText)
                .ConfigureAwait(false);
            await tcs.Task.ConfigureAwait(false);
            return;
        }

        /// <summary>
        /// Detects if the browser supports the Clipboard API.
        /// </summary>
        /// <returns>
        /// A Task which resolves to a boolean stating if the clipboard is supported or not.
        /// </returns>
        public Task<bool> IsSupportedAsync()
        {
            return _jSRuntime.InvokeAsync<bool>("ModelingEvolution.Clipboard.IsSupported")
                .AsTask();
        }

        [JSInvokable]
        public static void ReceiveReadResponse(string id, string text)
        {
            var idVal = Guid.Parse(id);
            var pendingTask = PendingReadRequests[idVal];
            pendingTask.SetResult(text);
            PendingReadRequests.Remove(idVal);
        }

        [JSInvokable]
        public static void ReceiveWriteResponse(string id)
        {
            var idVal = Guid.Parse(id);
            var pendingTask = PendingWriteRequests[idVal];
            pendingTask.SetResult(null);
            PendingWriteRequests.Remove(idVal);
        }
    }
}