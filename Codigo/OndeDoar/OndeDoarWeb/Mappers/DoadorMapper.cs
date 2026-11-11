
using Mapster;
using Models;
using Core;

namespace Mappers
{
    public class DoadorMapper
    {
        public DoadorMapper()
        {
            TypeAdapterConfig<Doador, DoadorViewModel>
                .NewConfig()
                .TwoWays();
        }
    }
}
