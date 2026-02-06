using AutoMapper;
using TodoList.Data.Entities;
using TodoListShared.Models.Models;

namespace TodoListApp.Mappings;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        // Списки справ (US01-US04)
        this.CreateMap<TodoListEntity, TodoListModel>().ReverseMap();

        // Учасники списку (для Epic 1 & 6)
        this.CreateMap<TodoListMember, TodoListMemberModel>().ReverseMap();

        // Теги (US17-US21)
        this.CreateMap<TagEntity, TagModel>().ReverseMap();

        // Таски (US05-US16)
        // Таски (US05-US16)
        this.CreateMap<TaskEntity, TaskModel>();

        this.CreateMap<TaskModel, TaskEntity>()
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
            .ForMember(dest => dest.AssigneeId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Коментарі (US22-US25)
        this.CreateMap<CommentEntity, CommentModel>().ReverseMap()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
