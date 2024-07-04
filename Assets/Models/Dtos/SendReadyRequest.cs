namespace Assets.Models.Dtos
{
    public class SendReadyRequest
    {
        public string PlayerToken { get; set; }
        public bool IsReady { get; set; }
    }
}
