using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ST10480375_PROG6221_POE_PART_3
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Reminder { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class DatabaseHelper
    {
        // replace YOUR_SERVER_NAME with your server name from SSMS
        private string connectionString =
            "Server=LabVM2049939\\SQLEXPRESS;Database=ccp_chatbot;Integrated Security=True;";

        // adds a new task to the database
        public bool AddTask(string title, string description, string reminder)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO tasks (title, description, reminder) " +
                                   "VALUES (@title, @desc, @reminder)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.Parameters.AddWithValue("@reminder", reminder ?? "");
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddTask error: " + ex.Message);
                return false;
            }
        }

        // gets all tasks from the database
        public List<TaskItem> GetAllTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, title, description, reminder, is_completed FROM tasks";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Reminder = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            IsCompleted = reader.GetBoolean(4)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllTasks error: " + ex.Message);
            }
            return tasks;
        }

        // marks a task as completed
        public bool CompleteTask(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE tasks SET is_completed = 1 WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CompleteTask error: " + ex.Message);
                return false;
            }
        }

        // deletes a task from the database
        public bool DeleteTask(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM tasks WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteTask error: " + ex.Message);
                return false;
            }
        }
    }
}