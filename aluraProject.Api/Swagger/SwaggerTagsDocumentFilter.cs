using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace aluraProject.Api.Swagger;

public sealed class SwaggerTagsDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags =
        [
            new OpenApiTag
            {
                Name = "Auth",
                Description = "Authentication endpoints. Public routes for register/login and JWT acquisition."
            },
            new OpenApiTag
            {
                Name = "Courses",
                Description = "Course catalog management. Public read and protected write routes."
            },
            new OpenApiTag
            {
                Name = "Students",
                Description = "Student profile management and enrollment queries."
            },
            new OpenApiTag
            {
                Name = "Enrollments",
                Description = "Enrollment creation with duplicate protection and role-based behavior."
            }
        ];
    }
}
