namespace DriveYOU_WebClient.Models
{
    public class ErrorModel
    {
        public ErrorModel() { }
        public ErrorModel(string _caption, string _message)
        {
            Caption = _caption;
            Message = _message;
        }

        public string Caption { get; set; }
        public string Message { get; set; }
    }
}
