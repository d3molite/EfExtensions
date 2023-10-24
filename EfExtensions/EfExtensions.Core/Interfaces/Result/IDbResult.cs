namespace EfExtensions.Core.Interfaces.Result;

public interface IDbResult<T>
{
    public T Item { get; set; }
    
    public bool Success { get; set; }
    
    public string ErrorMessage { get; set; }
}