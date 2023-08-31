namespace CoreTemplate.Models.Commons
{
    public class StatusMessage
    {
        public MessageType Type { get; set; }
        public string MessageContent { get; set; }
        public string MessageTitle { get; set; }
    }

    public class SuccessMessage : StatusMessage
    {
        public SuccessMessage(string messageContent)
        {
            Type = MessageType.Success;
            MessageContent = messageContent;
            MessageTitle = "Opération réussie";
        }
    }

    public class ErrorMessage : StatusMessage
    {
        public ErrorMessage(string messageContent)
        {
            Type = MessageType.Error;
            MessageContent = messageContent;
            MessageTitle = "Une erreur s'est produite";
        }
    }

    public class WarningMessage : StatusMessage
    {
        public WarningMessage(string messageContent)
        {
            Type = MessageType.Warning;
            MessageContent = messageContent;
            MessageTitle = "Attention";
        }
    }

    public class InfoMessage : StatusMessage
    {
        public InfoMessage(string messageContent)
        {
            Type = MessageType.Info;
            MessageContent = messageContent;
            MessageTitle = "Information";
        }
    }

    public enum MessageType
    {
        Success,
        Warning,
        Error,
        Info
    }
}
