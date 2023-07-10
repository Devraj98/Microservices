using AutoMapper;
using Grpc.Core;
using PlatformService;
using PlatformService.Data;

namespace platformservice.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response =  new PlatformResponse();
            var Platforms = _repository.GetAllPlatforms();

            foreach ( var platform in Platforms )
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
            }
            return Task.FromResult(response);
        }
    }
}
