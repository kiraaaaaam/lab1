using System.Windows;
using System.Windows.Controls;

namespace Todolist;


public partial class MainWindow : Window
{
    // Сервис выполняет операции с данными; окно не работает с JSON напрямую.
    private readonly TodoService _todoService;

    // Задача, выбранная в таблице и сейчас отображённая в форме.
    private TodoModel? _selectedTask;
    
    public MainWindow()
    {
        InitializeComponent();

        // Создаём объект хранения и передаём его сервису (внедрение зависимости)
        ITaskRepository repository = new JsonTaskRepository();
        _todoService = new TodoService(repository);

        // Источником вариантов для выпадающих списков служат перечисления TaskPriority и TaskStatus
        cbPriority.ItemsSource = Enum.GetValues(typeof(TaskPriority));
        cbPriority.SelectedItem = TaskPriority.Medium;

        cbStatus.ItemsSource = Enum.GetValues(typeof(TaskStatus));
        cbStatus.SelectedItem = TaskStatus.NotStarted;

        // Для новой задачи предлагаем завтрашний день как срок по умолчанию
        dpDueDate.SelectedDate = DateTime.Today.AddDays(1);

        // Загружаем сохранённые данные при запуске и сразу показываем их в таблице
        try
        {
            _todoService.LoadTasks();
            RefreshTable();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось загрузить задачи: {ex.Message}", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
    
    private void RefreshTable()
    {
        dgTasks.ItemsSource = null;
        dgTasks.ItemsSource = _todoService.GetAllTasks();
    }
    
    private void ClearInputFields()
    {
        txtTitle.Clear();
        txtDescription.Clear();
        dpDueDate.SelectedDate = DateTime.Today.AddDays(1);
        cbPriority.SelectedItem = TaskPriority.Medium;
        cbStatus.SelectedItem = TaskStatus.NotStarted;
        _selectedTask = null;
        dgTasks.SelectedItem = null;
    }
    
    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string title = txtTitle.Text;
            string description = txtDescription.Text;
            DateTime dueDate = dpDueDate.SelectedDate ?? DateTime.Today;
            TaskPriority priority = (TaskPriority)(cbPriority.SelectedItem ?? TaskPriority.Medium);
            TaskStatus status = (TaskStatus)(cbStatus.SelectedItem ?? TaskStatus.NotStarted);

            // Конструктор и свойства TodoModel проверяют обязательные значения.
            TodoModel newTask = new TodoModel(title, description, dueDate, priority)
            {
                Status = status
            };

            _todoService.AddTask(newTask);
            _todoService.SaveTasks(); // Изменение сразу записывается в файл.

            RefreshTable();
            ClearInputFields();

            MessageBox.Show("Задача успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (ArgumentException ex)
        {
            // Ошибки проверки данных показываются как предупреждение, а не как сбой приложения.
            MessageBox.Show(ex.Message, "Ошибка валидации данных", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void DgTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgTasks.SelectedItem is TodoModel task)
        {
            _selectedTask = task;
            txtTitle.Text = task.Title;
            txtDescription.Text = task.Description;
            dpDueDate.SelectedDate = task.DueDate;
            cbPriority.SelectedItem = task.Priority;
            cbStatus.SelectedItem = task.Status;
        }
    }
    
    private void BtnUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedTask == null)
        {
            MessageBox.Show("Пожалуйста, выберите задачу в таблице для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            // Считываем форму в выбранный объект; присваивания запускают проверки TodoModel.
            _selectedTask.Title = txtTitle.Text;
            _selectedTask.Description = txtDescription.Text;
            _selectedTask.DueDate = dpDueDate.SelectedDate ?? DateTime.Today;
            _selectedTask.Priority = (TaskPriority)(cbPriority.SelectedItem ?? TaskPriority.Medium);
            _selectedTask.Status = (TaskStatus)(cbStatus.SelectedItem ?? TaskStatus.NotStarted);

            _todoService.UpdateTask(_selectedTask);
            _todoService.SaveTasks();

            RefreshTable();
            ClearInputFields();

            MessageBox.Show("Задача успешно обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Ошибка валидации данных", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (dgTasks.SelectedItem is not TodoModel taskToDelete)
        {
            MessageBox.Show("Пожалуйста, выберите задачу для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"Вы действительно хотите удалить задачу «{taskToDelete.Title}»?",
                                     "Подтверждение удаления",
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _todoService.DeleteTask(taskToDelete.Id);
            _todoService.SaveTasks();

            RefreshTable();
            ClearInputFields();

            MessageBox.Show("Задача удалена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    
    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        ClearInputFields();
    }
    
    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _todoService.SaveTasks();
            MessageBox.Show("Задачи успешно сохранены в файл tasks.json!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось сохранить задачи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void BtnLoad_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _todoService.LoadTasks();
            RefreshTable();
            ClearInputFields();
            MessageBox.Show("Задачи успешно загружены из файла tasks.json!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось загрузить задачи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
