using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimer.Models
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Project>().Wait();
            _database.CreateTableAsync<PTask>().Wait();
            _database.CreateTableAsync<Log>().Wait();
        }

        public Task<List<Project>> GetAllProjects()
        {
            return _database.Table<Project>().ToListAsync();
        }

        public Task AddNewProject(Project Project)
        {
            return _database.InsertAsync(Project);
        }

        public Task RemoveProject(Project Project)
        {
            _database.Table<PTask>().Where(x => x.ProjectId == Project.ProjectId).DeleteAsync();
            return _database.DeleteAsync(Project);
        }

        public Task UpdateProject(Project Project)
        {
            return _database.UpdateAsync(Project);
        }

        public Task<List<PTask>> GetAllTasks(int ProjectId)
        {
            return _database.Table<PTask>().Where(x => x.ProjectId == ProjectId).ToListAsync();
        }

        public Task AddNewTask(PTask Task)
        {
            return _database.InsertAsync(Task);
        }

        public Task RemoveTask(PTask Task)
        {
            return _database.DeleteAsync(Task);
        }      

        public Task UpdateTask(PTask Task)
        {
            return _database.UpdateAsync(Task);
        }

        public Task AddNewLog(Log Log)
        {
            return _database.InsertAsync(Log);
        }
    }
}