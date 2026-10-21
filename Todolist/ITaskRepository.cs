namespace Todolist;


public interface ITaskRepository    //требования к хранилищу задач, что оно должно уметь делать
{
    void Save(List<TodoModel> tasks); // сохранить
    List<TodoModel> Load(); // загрузить
}