namespace Todolist;

//описание того что хранит в себе каждая задача
public class TodoModel
{
    public Guid Id { get; set; } = Guid.NewGuid(); //уник айди 

    //Храним значения отдельно, чтобы проверять их
    private string _title = string.Empty;
    private string _description = string.Empty;
    private DateTime _dueDate;
    
    public string Title
    {
        get => _title; // если хотят прочитать выдаем значение
        set // если вписать
        {
            if (string.IsNullOrEmpty(value)) // если пусто то ошибка
                throw new ArgumentException("Название не может быть пустым");
            // Убираем случайные пробелы по краям
            _title = value.Trim(); // иначе вставляем
        }
    }

    public string Description
    {
        get => _description; 
        set
        {
            if (value != null)
            {
                _description = value;
            }
            else
            {
                _description = string.Empty; // вписываем пустую строку
            }
        }
    }

    public DateTime CreatedAt { get; set; } = DateTime.Now; // текущее время записываем в createdat

    public DateTime DueDate
    {
        get => _dueDate;
        set
        {
            if (value < CreatedAt.Date)
                throw new ArgumentException("Срок выполнения не может быть раньше даты создания.");
            _dueDate = value;
        }
    }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium; // ставим дефолтный, но могут поменять

    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

    public TodoModel() { }


    public TodoModel(string title, string description, DateTime dueDate, TaskPriority priority = TaskPriority.Medium)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;                  // Функция для создания задачи со всеми проверками
        Priority = priority;
        Status = TaskStatus.NotStarted;
    }
}