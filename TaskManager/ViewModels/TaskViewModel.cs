﻿using CommonUtil;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager
{
    public class TaskViewModel : ObservableObject
    {
        private TaskManagerViewModel _taskManager;

        private TaskModel _selectedTask;
        private ObservableCollection<TaskModel> _taskList;

        private ICommand _addTaskCommand;
        private ICommand _removeTaskCommand;

        private bool _hasTaskSelected;

        public TaskViewModel(TaskManagerViewModel taskManager)
        {
            _taskManager = taskManager;
        }

        int count = 0;
        public TaskModel SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                if (value != _selectedTask)
                {
                    _selectedTask = value;
                    OnPropertyChanged("SelectedTask");
                    _taskManager.ApplicationMessage = new ApplicationMessageModel(ApplicationMessageModel.TYPE.INFO, "Task changed " + (count++) + " P: " + _selectedTask.TaskPriority);
                    HasTaskSelected = (_selectedTask != null);
                }
            }
        }
        public ObservableCollection<TaskModel> TaskList
        {
            get 
            {
                if (_taskList == null)
                {
                    _taskList = new ObservableCollection<TaskModel>();
                }
                return _taskList;
            }
        }
        
        public ICommand AddTaskCommand
        {
            get
            {
                if (_addTaskCommand == null)
                {
                    _addTaskCommand = new RelayCommand(
                        param => AddTask()
                    );
                }
                return _addTaskCommand;
            }
        }
        public ICommand RemoveTaskCommand
        {
            get
            {
                if (_removeTaskCommand == null)
                {
                    _removeTaskCommand = new RelayCommand(
                        param => RemoveTask(),
                        param => ((TaskList.Count > 0) && (SelectedTask != null))
                    );
                }
                return _removeTaskCommand;
            }
        }
        
        public void AddTask ()
        {
            TaskModel newTask = new TaskModel();
            TaskList.Add(newTask);
            _taskManager.ApplicationMessage = new ApplicationMessageModel(ApplicationMessageModel.TYPE.INFO, "New task added");
            SelectedTask = newTask;
        }
        public void RemoveTask()
        {
            if (SelectedTask == null)
            {
                _taskManager.ApplicationMessage = new ApplicationMessageModel(ApplicationMessageModel.TYPE.ERROR, "Please select a task to be removed");
            }
            else
            {
                System.Windows.MessageBoxResult removeChoice = System.Windows.MessageBox.Show(System.Windows.Application.Current.MainWindow, "Are you sure you want to remove task?", "Remove Task", System.Windows.MessageBoxButton.YesNoCancel);
                if (removeChoice != System.Windows.MessageBoxResult.Yes) return;

                int taskID = SelectedTask.ID;
                if (TaskList.Remove(SelectedTask))
                {
                    int updatesRemoved = 0;
                    SelectedTask = TaskList.Count == 0 ? null : TaskList[TaskList.Count - 1];
                    updatesRemoved = _taskManager.TaskUpdate.RemoveAllUpdatesOfTask(taskID);

                    string message = "Task succefully removed. ";
                    if (updatesRemoved > 0)
                    {
                        message = string.Format("{0} {1} Task Update{2} was also removed", message, updatesRemoved, (updatesRemoved > 1 ? "s" : ""));
                    }

                    _taskManager.ApplicationMessage = new ApplicationMessageModel(
                        ApplicationMessageModel.TYPE.INFO,
                        message
                    );
                }
                else
                {
                    _taskManager.ApplicationMessage = new ApplicationMessageModel(ApplicationMessageModel.TYPE.ERROR, "Failed to remove Task.");
                }
            }
        }

        public bool HasTaskSelected {
            get { return _hasTaskSelected; }
            set
            {
                if (value != _hasTaskSelected)
                {
                    _hasTaskSelected = value;
                    _taskManager.TaskUpdate.UpdateTaskUpdatesList();
                    OnPropertyChanged("HasTaskSelected");
                }
            }
        }
    }
}
