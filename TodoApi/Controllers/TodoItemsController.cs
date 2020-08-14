using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // Get All Task List
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // Method Get 
        // Parameter Id
        // To Search Specific Task via parameter Id
        [HttpGet("{id}")]
        public ActionResult<TodoItem> GetTodoItem(long id)
        {
            var todoItems = _context.TodoItems.Find(id);
            if(todoItems == null)
            {
                return NotFound();
            }
            return todoItems;
        }


        // This function purpose to search task based of interval time (Today, Tomorrow, One week)
        [HttpGet("task/{interval}")]
        public ActionResult<TodoItem> GetTaskInterval(string interval)
        {
            var now = DateTime.Today.ToString("dd/MM/yyyy");
            if (interval == "Today")
            {
                return Ok(_context.TodoItems.Where(t => t.Date == now).ToList());
            }
            else if (interval == "Tomorrow")
            {
                var tomorrow = DateTime.Today.AddDays(1).ToString("dd/MM/yyyy");
                return Ok(_context.TodoItems.Where(t => t.Date == tomorrow).ToList());
            }
            else
            {
                var one_week = DateTime.Today.AddDays(7).ToString("dd/MM/yyyy");
                return Ok(_context.TodoItems.Where(t => t.Date == one_week).ToList());
            }
        }

        //Function To Mark Finished Task
        //Need parameter Id to assign update Value
        [HttpPut("mark/{id}")]
        public IActionResult MarkTaskFinish(long Id)
        {
            var std = _context.TodoItems.Find(Id);
            std.Percent = 100;
            std.IsComplete = true;
            _context.SaveChanges();
            return Content(std.Title +  " " + "Is Finished");
        }


        // Function for set percentage of Task
        //Need parameter Id to assign update Value
        [HttpPut("percentage/{id}/{percent}")]
        public IActionResult SetPercentTask(long Id, int percent)
        {
            var std = _context.TodoItems.Find(Id);
            std.Percent = percent;
            _context.SaveChanges();
            return Content("Percentage task" + std.Title + " has changed");
        }

        // Update To Do List
        // Make sure to include Id on Json data
        [HttpPut]
        public IActionResult PutTodoItem(TodoItem todoItem)
        {
            var std = _context.TodoItems.Find(todoItem.Id);
            std.Date = todoItem.Date;
            std.Time = todoItem.Time;
            std.Title = todoItem.Title;
            std.Description = todoItem.Description;
            std.Percent = todoItem.Percent;
            std.IsComplete = todoItem.IsComplete;
            _context.SaveChanges();
            return Content("Data " + std.Title + " is Updated");
        }

        // Make new ToDo List
        [HttpPost]
        public ActionResult<TodoItem> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            _context.SaveChanges();
            return Content("Saved To Database");
        }

        // Delete existing ToDo List
        [HttpDelete("{id}")]
        public ActionResult<TodoItem> DeleteTodoItem(long id)
        {
            var todoItem = _context.TodoItems.Find(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            _context.SaveChanges();

            return todoItem;
        }
    }
}
