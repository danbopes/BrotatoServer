using System.ComponentModel.DataAnnotations;

namespace BrotatoServer.Models.Views;

public class UserSettingsViewModel
{
    [StringLength(400)]
    public string OnRunStartedMessage { get; set; } = "";
    [StringLength(400)]
    public string OnRunWonMessage { get; set; } = "";
    [StringLength(400)]
    public string OnRunLostMessage { get; set; } = "";

    public bool ClipOnRunWon { get; set; } = false;
    public bool ClipOnRunLost { get; set; } = false;
    
    [Range(0, 80, ErrorMessage = "Clip Delay Seconds must be between 0 and 80 seconds.")]
    public int ClipDelaySeconds { get; set; } = 30;
}