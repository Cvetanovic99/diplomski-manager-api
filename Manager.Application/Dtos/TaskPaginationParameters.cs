namespace Manager.Application.Dtos
{
    public class TaskPaginationParameters : PaginationParameters
    {
        public int? UserId { get; set; }
    }
}