using AutoMapper;
using TodoList.Data.Entities;
using TodoListShared.Models.Models;

namespace TodoListApp.Mappings;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        this.CreateMap<TodoListEntity, TodoListModel>()
           .ReverseMap();

        this.CreateMap<TagEntity, TagModel>()
            .ReverseMap();

        this.CreateMap<TaskEntity, TaskModel>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ReverseMap();
    }
}
