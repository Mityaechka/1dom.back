using System.Text;

namespace Taxiverk.Infrastructure.Random
{
    public class MockRandomService : IRandomService
    {
        public string Generate(int length = 4)
        {
            var passwordBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                passwordBuilder.Append(9);
            }

            return passwordBuilder.ToString();
        }
    }
}
