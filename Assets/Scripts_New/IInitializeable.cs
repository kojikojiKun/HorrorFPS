using System.Threading.Tasks;
public interface IInitializeable
{
    bool IsInitialized { get; }
    Task InitializeAsync();
    void Instantiate();
}
