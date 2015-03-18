﻿using CommonUtil;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using TaskManager.Models;

namespace TaskManager.ViewModels
{
    public class TaskUpdateViewModel : ObservableObject
    {
        private TaskManagerViewModel _taskManager;

        private TaskUpdateModel _selectedTaskUpdate;
        private ObservableCollection<TaskUpdateModel> _taskUpdates;
        private ObservableCollection<TaskUpdateModel> _allTaskUpdates;

        private ICommand _addTaskUpdateCommand;
        private ICommand _removeTaskUpdateCommand;

        public TaskUpdateViewModel(TaskManagerViewModel taskManager)
        {
            _taskManager = taskManager;
            _allTaskUpdates = new ObservableCollection<TaskUpdateModel>();
        }

        public TaskUpdateModel SelectedTaskUpdate
        {
            get { return _selectedTaskUpdate; }
            set
            {
                if (value != _selectedTaskUpdate)
                {
                    _selectedTaskUpdate = value;
                    OnPropertyChanged("SelectedTaskUpdate");
                }
            }
        }
        public ObservableCollection<TaskUpdateModel> TaskUpdates
        {
            get
            {
                if (_taskUpdates == null) { _taskUpdates = new ObservableCollection<TaskUpdateModel>(); }
                return _taskUpdates;
            }
        }

        public ICommand AddTaskUpdateCommand
        {
            get
            {
                if (_addTaskUpdateCommand == null)
                {
                    _addTaskUpdateCommand = new RelayCommand(
                        param => AddTaskUpdate(),
                        param => (_taskManager.Task.SelectedTask != null)
                    );
                }
                return _addTaskUpdateCommand;
            }
        }
        public ICommand RemoveTaskUpdateCommand
        {
            get
            {
                if (_removeTaskUpdateCommand == null)
                {
                    _removeTaskUpdateCommand = new RelayCommand(
                        param => RemoveTask(),
                        param => ((_taskManager.Task.SelectedTask != null) && (TaskUpdates.Count > 0))
                    );
                }
                return _removeTaskUpdateCommand;
            }
        }

        public void AddTaskUpdate()
        {
            TaskUpdateModel newTaskUpdate = new TaskUpdateModel(_taskManager.Task.SelectedTask.ID);
            TaskUpdates.Add(newTaskUpdate);
            _allTaskUpdates.Add(newTaskUpdate);
            SelectedTaskUpdate = newTaskUpdate;
            _taskManager.ApplicationMessage = new ApplicationMessageModel(ApplicationMessageModel.TYPE.INFO, "New task update added.");
        }
        /* Incomplete */
        public void RemoveTask()
        {
            if (SelectedTaskUpdate == null)
            {
                _taskManager.ApplicationMessage = new ApplicationMessageModel(ApplicationMessageModel.TYPE.ERROR, "Please select a task update to be removed");
            }
            else
            {
                // Show Confirmation Box First
                if (TaskUpdates.Remove(SelectedTaskUpdate) && _allTaskUpdates.Remove(SelectedTaskUpdate))
                {
                    _taskManager.ApplicationMessage = new ApplicationMessageModel(ApplicationMessageModel.TYPE.INFO, "Task succefully removed update.");
                    SelectedTaskUpdate = (TaskUpdates.Count > 0) ? TaskUpdates[TaskUpdates.Count - 1] : null;
                }
                else
                {
                    _taskManager.ApplicationMessage = new ApplicationMessageModel(ApplicationMessageModel.TYPE.ERROR, "Failed to remove Task update.");
                }
            }
        }
        public int RemoveAllUpdatesOfTask(int taskID)
        {
            int count = 0;
            if (_allTaskUpdates != null)
            {
                foreach (TaskUpdateModel taskUpdate in _allTaskUpdates)
                {
                    if (taskUpdate.TaskID == taskID)
                    {
                        TaskUpdates.Remove(taskUpdate);
                        count++;
                    }
                }
            }
            return count;
        }
        public void FindTaskUpdates()
        {
            if (_taskManager.Task.SelectedTask !=null && _allTaskUpdates != null)
            {
                foreach (TaskUpdateModel taskUpdate in _allTaskUpdates)
                {
                    if (taskUpdate.TaskID == _taskManager.Task.SelectedTask.ID)
                    {
                        TaskUpdates.Add(taskUpdate);
                    }
                }
            }
        }
        public void UpdateTaskUpdatesList()
        {
            TaskUpdates.Clear();
            FindTaskUpdates();
        }
    }
}
