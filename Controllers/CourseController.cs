using Architecture.Models;
using Architecture.ViewModel;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Architecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;

        public CourseController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        [HttpGet]
        [Route("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var results = await _courseRepository.GetAllCourseAsync();
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500,"Internal Server Error. Please contact support.");
            }
        }

        [HttpGet]
        [Route("GetCourse/{courseId}")]
        public async Task<IActionResult> GetCourseAsync(int courseId)
        {
            try
            {
                var result = await _courseRepository.GetCourseAsync(courseId);

                if (result == null) return NotFound("Course does not exist");

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support");
            }
        }

        [HttpPost]
        [Route("AddCourse")]
        public async Task<IActionResult> AddCourse(CourseViewModel cvm)
        {
            var course = new Course { Name = cvm.Name, Description = cvm.Description, Duration = cvm.Duration };

            try
            {
                _courseRepository.Add(course);
                await _courseRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest("Invalid transaction");
            }

            return Ok(course);
        }

        [HttpPut]
        [Route("EditCourse/{courseId}")]
        public async Task<ActionResult<CourseViewModel>> EditCourse(int courseId, CourseViewModel courseModel)
        {
            try
            {
                var existingCourse = await _courseRepository.GetCourseAsync(courseId);
                if (existingCourse == null) return NotFound($"The course does not exist");

                existingCourse.Name = courseModel.Name;
                existingCourse.Description = courseModel.Description;
                existingCourse.Duration = courseModel.Duration;
                

                if (await _courseRepository.SaveChangesAsync())
                {
                    return Ok(existingCourse);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
            return BadRequest("Your request is invalid.");
        }

        [HttpDelete]
        [Route("DeleteCourse/{courseId}")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            try
            {
                var existingCourse = await _courseRepository.GetCourseAsync(courseId);

                if (existingCourse == null) return NotFound($"The course does not exist");

                _courseRepository.Delete(existingCourse);

                if (await _courseRepository.SaveChangesAsync()) 
                    return Ok(existingCourse);

            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
            return BadRequest("Your request is invalid.");
        }




    }




}

