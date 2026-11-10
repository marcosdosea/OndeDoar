
using Mapster;
using Models;
using Core;

namespace Mappers
{
    public class DoadorMapper
    {
        public DoadorMapper()
        {
            TypeAdapterConfig<Doador, DoadorModel>
                .NewConfig()
                .TwoWays();
        }
    }
}
