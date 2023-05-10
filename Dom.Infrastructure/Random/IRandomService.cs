namespace Taxiverk.Infrastructure.Random
{
    public interface IRandomService
    {
        string Generate(int length = 4);
    }
}
