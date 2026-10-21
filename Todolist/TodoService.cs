namespace Todolist;

// Как мы можем работать со списком задач 
public class TodoService
{
    // обязан иметь хранилище которое соблюдает требования ITaskRepository
    private readonly ITaskRepository _repository;

    // Список в памяти во время работы
    private List<TodoModel> _tasks = new();

    // принимает хранилище и сохраняет его в поле _repository
    public TodoService(ITaskRepository repository)
    {
        _repository = repository;
    }

    // Функция возвращает копию списка задач в памяти
    public List<TodoModel> GetAllTasks()
    {
        return new List<TodoModel>(_tasks); 
    }

    // Загружаем список задач из хранилища в память
    public void LoadTasks()
    {
        _tasks = _repository.Load();
    }
    
    // Сохраняем список задач из памяти в хранилище
    public void SaveTasks()
    {
        _repository.Save(_tasks);
    }

    // добавляем задачу в список в памяти, проверяем что задача не нулевое
    public void AddTask(TodoModel task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task), "Задача не может быть пустой");
        _tasks.Add(task);
    }
    
    // Удаляем по Id и возвращаем true если удалили, false если не нашли
    public bool DeleteTask(Guid id)
    {
        // Перебираем задачи и звписываем в taskToRemove ту, у которой совпадает Id с требуемым
        TodoModel? taskToRemove = _tasks.FirstOrDefault(t => t.Id == id);
        if (taskToRemove != null)
        {
            _tasks.Remove(taskToRemove);
            return true;
        }
        
        return false;
    }

    // Обновляем в существующей задаче поля, которые пришли в updatedTask
    public void UpdateTask(TodoModel updatedTask)
    {
        if (updatedTask == null) throw new ArgumentNullException(nameof(updatedTask)); // ошибка если null
        // перебираем задачи и ищем ту, у которой совпадает Id с требуемым
        TodoModel? existingTask = _tasks.FirstOrDefault(t => t.Id == updatedTask.Id);
        if (existingTask == null) throw new ArgumentNullException(nameof(updatedTask), "Задача не найдена");
        
        existingTask.Title = updatedTask.Title;
        existingTask.Description = updatedTask.Description;
        existingTask.DueDate = updatedTask.DueDate;        // в существующей задаче меняем поля на новые
        existingTask.Priority = updatedTask.Priority;
        existingTask.Status = updatedTask.Status;
    }

    // Меняем статус задачи по Id
    public void ChangeStatus(Guid id, TaskStatus newStatus)
    {
        TodoModel? task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) throw new InvalidOperationException("Задача не найдена."); // кидаем ошибку если не нашли
        task.Status = newStatus;
    }
}