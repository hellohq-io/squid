using System.Linq;
using AutoMapper;
using Q.Squid.DAL;

namespace Q.Squid.Models
{
    /// <summary>
    /// The mapping helper generates mapping configuration.
    /// </summary>
    public static class MappingHelper
    {
        private static IMapper _mapper;

        /// <summary>
        /// Returns the Automapper configuration.
        /// </summary>
        /// <returns></returns>
        public static IMapper GetMapper()
        {
            // init singleton mapper
            if (_mapper == null)
            {
                MapperConfiguration config = new AutoMapper.MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<RepositoryConfigModel, RepositoryConfig>()
                        .ReverseMap();
                    cfg.CreateMap<RepositoryConfigFormModel, RepositoryConfig>()
                        .ReverseMap();

                    cfg.CreateMap<NuGetFeedModel, NugetFeedConfig>()
                        .ReverseMap();
                    cfg.CreateMap<NuGetFeedFormModel, NugetFeedConfig>()
                        .ReverseMap();
                });

                _mapper = config.CreateMapper();
            }

            return _mapper;
        }
    }
}