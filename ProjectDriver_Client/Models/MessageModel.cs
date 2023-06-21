namespace DriveYOU_WebClient.Models
{
    public class MessageModel
    {
        public MessageModel() { }
        public MessageModel(string _caption, string _message)
        {
            Caption = _caption;
            Message = _message;
        }

        public string Caption { get; set; }
        public string Message { get; set; }
    }
}
