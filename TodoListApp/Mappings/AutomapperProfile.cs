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
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

        this.CreateMap<TaskModel, TaskEntity>()
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore());

        this.CreateMap<CommentEntity, CommentModel>()
            .ReverseMap()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
    }
}
