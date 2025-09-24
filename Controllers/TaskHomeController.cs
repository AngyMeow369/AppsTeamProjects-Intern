using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using TaskLoggerV1.Models;

namespace TaskLoggerV1.Controllers
{
    public class TaskHomeController : Controller
    {
        private readonly string connStr;

        public TaskHomeController()
        {
            var cs = ConfigurationManager.ConnectionStrings["TaskLoggerDBConnectionString"];
            if (cs == null)
                throw new Exception("Connection string 'TaskLoggerDBConnectionString' not found in Web.config");

            connStr = cs.ConnectionString;
        }

        public ActionResult Index()
        {
            var tasks = new List<TaskModel>();
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_GetAllTasks", con) { CommandType = CommandType.StoredProcedure })
            {
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TaskModel
                        {
                            Id = Guid.Parse(reader["Id"].ToString()),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            TaskDate = Convert.ToDateTime(reader["TaskDate"]),
                            HoursLogged = Convert.ToDouble(reader["HoursLogged"]),
                            IsCompleted = Convert.ToBoolean(reader["IsCompleted"])
                        });
                    }
                }
            }
            return View(tasks);
        }

        public ActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskModel task)
        {
            if (!ModelState.IsValid) return View(task);

            task.Id = Guid.NewGuid();
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_AddTask", con) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", task.Id);
                cmd.Parameters.AddWithValue("@Title", task.Title);
                cmd.Parameters.AddWithValue("@Description", task.Description);
                cmd.Parameters.AddWithValue("@TaskDate", task.TaskDate);
                cmd.Parameters.AddWithValue("@HoursLogged", task.HoursLogged);
                cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid id)
        {
            TaskModel task = null;
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_GetTaskById", con) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        task = new TaskModel
                        {
                            Id = Guid.Parse(reader["Id"].ToString()),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            TaskDate = Convert.ToDateTime(reader["TaskDate"]),
                            HoursLogged = Convert.ToDouble(reader["HoursLogged"]),
                            IsCompleted = Convert.ToBoolean(reader["IsCompleted"])
                        };
                    }
                }
            }

            if (task == null)
                return HttpNotFound();

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskModel task)
        {
            if (!ModelState.IsValid) return View(task);

            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_UpdateTask", con) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", task.Id);
                cmd.Parameters.AddWithValue("@Title", task.Title);
                cmd.Parameters.AddWithValue("@Description", task.Description);
                cmd.Parameters.AddWithValue("@TaskDate", task.TaskDate);
                cmd.Parameters.AddWithValue("@HoursLogged", task.HoursLogged);
                cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            TaskModel task = null;
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_GetTaskById", con) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        task = new TaskModel
                        {
                            Id = Guid.Parse(reader["Id"].ToString()),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            TaskDate = Convert.ToDateTime(reader["TaskDate"]),
                            HoursLogged = Convert.ToDouble(reader["HoursLogged"]),
                            IsCompleted = Convert.ToBoolean(reader["IsCompleted"])
                        };
                    }
                }
            }

            if (task == null)
                return HttpNotFound();

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_DeleteTask", con) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Search(DateTime? date)
        {
            if (date == null)
                return View();

            var tasks = new List<TaskModel>();
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_SearchTasksByDate", con) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@TaskDate", date.Value);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TaskModel
                        {
                            Id = Guid.Parse(reader["Id"].ToString()),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            TaskDate = Convert.ToDateTime(reader["TaskDate"]),
                            HoursLogged = Convert.ToDouble(reader["HoursLogged"]),
                            IsCompleted = Convert.ToBoolean(reader["IsCompleted"])
                        });
                    }
                }
            }

            return View(tasks);
        }
    }
}

