using AutoMapper;
using TodoListApp.Services.Database.Entities;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Mappings;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        // Списки справ (US01-US04)
        this.CreateMap<TodoListMember, TodoListMemberModel>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

        this.CreateMap<TodoListEntity, TodoListModel>()
            .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.TodoListMembers))
            .ReverseMap();

        // Теги (US17-US21)
        this.CreateMap<TagEntity, TagModel>().ReverseMap();

        // Таски (US05-US16)
        // Таски (US05-US16)
        this.CreateMap<TaskEntity, TaskModel>();

        this.CreateMap<TaskModel, TaskEntity>()
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Коментарі (US22-US25)
        this.CreateMap<CommentEntity, CommentModel>().ReverseMap()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
