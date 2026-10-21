using System.IO; //ввод вывод
using System.Text.Json; // работа с JSON
namespace Todolist;

public class JsonTaskRepository : ITaskRepository // как должны работать с сохранением
{
    // сохраняем неизменяемый путь к файлу
    private readonly string _filePath;

    // настройка чтобы json был читаемым, с отступами
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    // Принимаем путь к файлу и сохраняем 
    public JsonTaskRepository(string filePath = "tasks.json")
    {
        _filePath = filePath;
    }

    // логика сохранения
    public void Save(List<TodoModel> tasks)
    {
        string json = JsonSerializer.Serialize(tasks, _jsonOptions); // записываем в json список tasks по настройкам _jsonOptions
        File.WriteAllText(_filePath, json); // сохраняем по пути _filePath
    }

    // логика загрузки
    public List<TodoModel> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<TodoModel>();  // если файла нет, возвращаем пустой список задач
        }

        string json = File.ReadAllText(_filePath); // читаем содержимое файла в строку json
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<TodoModel>();  // если файл пустой, возвращаем пустой список задач
        }
        
        // если все ок, превращаем json в список задач
        List<TodoModel>? tasks = JsonSerializer.Deserialize<List<TodoModel>>(json, _jsonOptions);
        return tasks ?? new List<TodoModel>(); // если ошибка, возвращаем пустой список задач
    }
}