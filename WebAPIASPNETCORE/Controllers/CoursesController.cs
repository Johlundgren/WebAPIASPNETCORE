using Infrastructure.Contexts;
using Infrastructure.Dtos;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPIASPNETCORE.Filters;

namespace WebAPIASPNETCORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CoursesController(DataContext context) : ControllerBase
    {
        private readonly DataContext _context = context;

        #region READ ALL
        [UseApiKey]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(string category = "", string searchQuery = "", int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Courses.Include(i => i.Category).AsQueryable();


            if (!string.IsNullOrEmpty(category) && category != "all")
                query = query.Where(x => x.Category!.CategoryName == category);

            if (!string.IsNullOrEmpty(searchQuery))
                query = query.Where(x => x.Title.Contains(searchQuery) || x.Author!.Contains(searchQuery));


            query = query.OrderByDescending(o => o.LastUpdated);

            var courses = await query.ToListAsync();

            var response = new CourseResult
            {
                Succeeded = true,
                TotalItems = await query.CountAsync()
            };
            response.TotalPages = (int)Math.Ceiling(response.TotalItems / (double)pageSize);
            response.Courses = CourseFactory.Create(await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync());


            return Ok(response);
        }
        #endregion

        #region READ ONE
        [UseApiKey]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);
            if (course != null)
            {
                return Ok(course);
            }

            return NotFound();
        }
        #endregion

        #region CREATE
        [UseApiKey]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOne(CourseRegistrationForm form)
        {
            if (ModelState.IsValid)
            {
                var categoryEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == form.CategoryId);

                if (categoryEntity == null)
                {
                    return NotFound("Category not found");
                }
                var courseEntity = new CourseEntity
                {
                    Title = form.Title,
                    Price = form.Price,
                    DiscountPrice = form.DiscountPrice,
                    Hours = form.Hours,
                    IsBestSeller = form.IsBestSeller,
                    LikesInNumbers = form.LikesInNumbers,
                    LikesInProcent = form.LikesInProcent,
                    Author = form.Author,
                    ImageUrl = form.ImageUrl,
                    BigImageUrl = form.BigImageUrl,
                    Category = categoryEntity
                    
                };

                _context.Courses.Add(courseEntity);
                await _context.SaveChangesAsync();

                return Created("", (Course)courseEntity);
            }
            return BadRequest();
        }
        #endregion

        #region UPDATE
        [UseApiKey]
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, CourseRegistrationForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var courseEntity = await _context.Courses.Include(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
            if (courseEntity == null)
            {
                return NotFound($"No course found with ID {id}");
            }

            var categoryEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == form.CategoryId);
            if (categoryEntity == null)
            {
                return NotFound("Category not found");
            }

            courseEntity.Title = form.Title;
            courseEntity.Price = form.Price;
            courseEntity.DiscountPrice = form.DiscountPrice;
            courseEntity.Hours = form.Hours;
            courseEntity.IsBestSeller = form.IsBestSeller;
            courseEntity.LikesInNumbers = form.LikesInNumbers;
            courseEntity.LikesInProcent = form.LikesInProcent;
            courseEntity.Author = form.Author;
            courseEntity.ImageUrl = form.ImageUrl;
            courseEntity.BigImageUrl = form.BigImageUrl;
            courseEntity.Category = categoryEntity;

            _context.Courses.Update(courseEntity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course updated successfully", course = courseEntity });
        }
        #endregion

        #region DELETE
        [UseApiKey]
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound($"Course with ID {id} not found.");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok($"Course with ID {id} has been deleted.");
        }
        #endregion


        [HttpGet("savedcourses")]
        [Authorize]
        public async Task<IActionResult> GetSavedCourses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var savedCourses = await _context.SavedCourses
                                             .Where(sc => sc.UserId == userId)
                                             .Include(sc => sc.Course)
                                             .Select(sc => sc.Course)
                                             .ToListAsync();

            return Ok(savedCourses);
        }
        //funkar inte
        [HttpPost("save/{courseId}")]
        public async Task<IActionResult> SaveCourse(int courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var existingCourse = await _context.Courses.FindAsync(courseId);
            if (existingCourse == null)
            {
                return NotFound("Course not found.");
            }

            var alreadySaved = await _context.SavedCourses.AnyAsync(sc => sc.UserId == userId && sc.CourseId == courseId);
            if (alreadySaved)
            {
                return BadRequest("Course already saved.");
            }

            var savedCourse = new SavedCourseEntity
            {
                UserId = userId,
                CourseId = courseId
            };

            _context.SavedCourses.Add(savedCourse);
            await _context.SaveChangesAsync();

            return Ok("Course saved successfully.");
        }


    }
}