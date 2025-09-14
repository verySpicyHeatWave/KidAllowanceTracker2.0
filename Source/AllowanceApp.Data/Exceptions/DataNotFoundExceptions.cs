namespace AllowanceApp.Data.Exceptions
{
    [Serializable]
    public class DataNotFoundException(string message) : Exception(message) { }
}
