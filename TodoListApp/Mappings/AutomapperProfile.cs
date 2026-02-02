using AutoMapper;
using TodoListApp.Data.Models;
using TodoListApp.Entities;

namespace TodoListApp.Mappings;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        this.CreateMap<TodoListEntity, TodoListModel>()
           .ReverseMap();

        this.CreateMap<TaskEntity, TaskModel>()
            .ReverseMap();
    }
}
