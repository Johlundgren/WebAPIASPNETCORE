

using Infrastructure.Entities;
using Infrastructure.Models;

namespace Infrastructure.Factories;

public class CourseFactory
{
    public static Course Create(CourseEntity entity)
    {
        try
        {
            return new Course
            {
                Id = entity.Id,
                Title = entity.Title,
                Price = entity.Price,
                DiscountPrice = entity.DiscountPrice,
                Hours = entity.Hours,
                IsBestSeller = entity.IsBestSeller,
                LikesInNumbers = entity.LikesInNumbers,
                LikesInProcent = entity.LikesInProcent,
                Author = entity.Author,
                ImageUrl = entity.ImageUrl,
                BigImageUrl = entity.BigImageUrl,
                Category = entity.Category!.CategoryName
            };
        }
        catch { }
        return null!;
    }


    public static IEnumerable<Course> Create(List<CourseEntity> entities)
    {
        List<Course> courses = [];

        try
        {
            foreach (var entity in entities)
                courses.Add(Create(entity));
        }
        catch { }
        return courses;
    }
}
