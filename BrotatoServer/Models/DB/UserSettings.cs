using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrotatoServer.Models.DB;

public class UserSettings
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong UserId { get; set; }
    
    [MaxLength(400)]
    public string OnRunStartedMessage { get; set; } = "";
    [MaxLength(400)]
    public string OnRunWonMessage { get; set; } = "";
    [MaxLength(400)]
    public string OnRunLostMessage { get; set; } = "";

    public bool ClipOnRunWon { get; set; } = false;
    public bool ClipOnRunLost { get; set; } = false;
    
    public int ClipDelaySeconds { get; set; } = 30;
}