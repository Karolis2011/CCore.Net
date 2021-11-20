namespace CCore.Net.JsRt
{
    /// <summary>
    ///     The possible states for a Promise object.
    /// </summary>
    public enum JsPromiseState
    {
        Pending = 0x0,
        Fulfilled = 0x1,
        Rejected = 0x2
    }
}