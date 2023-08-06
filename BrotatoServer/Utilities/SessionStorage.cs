namespace BrotatoServer.Utilities;

public class SessionStorage
{
    private string? _lastUsername;
    
    public string? LastUsername
    {
        get => _lastUsername;
        set
        {
            if (value == _lastUsername)
                return;
            
            _lastUsername = value;
            NotifyStateChanged();
        }
    }

    public event Action OnStateChange;

    /// <summary>
    /// The state change event notification
    /// </summary>
    private void NotifyStateChanged() => OnStateChange?.Invoke();
}