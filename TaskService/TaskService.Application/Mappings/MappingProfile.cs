using AutoMapper;
using TaskService.Application.Contracts.Jobs;
using TaskService.Domain.Entities;

namespace TaskService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Job, JobResponse>();
        CreateMap<CreateJobRequest, Job>();
        CreateMap<UpdateJobRequest, Job>();
    }
}