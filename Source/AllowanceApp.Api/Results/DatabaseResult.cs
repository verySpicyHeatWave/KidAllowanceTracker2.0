namespace AllowanceApp.Api.Results
{
    public record DatabaseResults<T>(T? Response, string? Message, int StatusCode)
    {
        public bool IsSuccess => StatusCode == 200 && Response is not null;
    }
}