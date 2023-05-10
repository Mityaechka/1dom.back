using System.Text;

namespace Taxiverk.Infrastructure.Random
{
    public class RandomService : IRandomService
    {
        public string Generate(int length = 4)
        {
            var random = new System.Random();
            var passwordBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                passwordBuilder.Append(random.Next(0, 9));
            }

            return passwordBuilder.ToString();
        }
    }
}
