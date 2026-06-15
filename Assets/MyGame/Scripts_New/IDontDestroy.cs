using System.Threading.Tasks;
public interface IDontDestroy
{
    bool IsInitialized { get; }
    Task InitializeAsync();
    void Instantiate();
}
