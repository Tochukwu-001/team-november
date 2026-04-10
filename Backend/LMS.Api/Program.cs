using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using LMS.Api.Extensions;
using LMS.Application.Common;
using LMS.Application.Common.Options;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Application.Services;
using LMS.Application.Validators.Lesson;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Repositories;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 1. Add Persistence (EF Core + Npgsql)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 2. Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseBrowseRepository, CourseBrowseRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDiscussionRepository, DiscussionRepository>();
builder.Services.AddScoped<IInstructorRoleRequestRepository, InstructorRoleRequestRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<ILessonNoteRepository, LessonNoteRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IProgressRepository, ProgressRepository>();
builder.Services.AddScoped<ILessonProgressRepository, LessonProgressRepository>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 🔹 3. Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDiscussionService, DiscussionService>();
builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddHttpClient<IFileStorageService, CloudinaryFileStorageService>();
builder.Services.AddHostedService<TokenCleanupService>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();


// 🔹 4. Shared Infrastructure
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();

// 🔹 5. Validation (FluentValidation)
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateLessonRequestValidator>();


// 🔹 6. JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection(FileStorageOptions.SectionName));
builder.Services.Configure<GoogleAuthOptions>(builder.Configuration.GetSection(GoogleAuthOptions.SectionName));
builder.Services.Configure<JwtOptions>(jwtSection);
var secret = jwtSection["Secret"];
if (string.IsNullOrWhiteSpace(secret))
{
    throw new InvalidOperationException("JWT Secret is not configured");
}
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSection = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Secret"]!)),
        ClockSkew = TimeSpan.Zero
    };
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 🔹 7. Swagger (with JWT Support)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LMS API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        db.Database.Migrate();

        foreach (var teamDefinition in TeamCatalog.DefaultTeams)
        {
            var existingTeam = db.Teams.FirstOrDefault(team => team.Name == teamDefinition.Name);
            if (existingTeam == null)
            {
                db.Teams.Add(Team.Create(teamDefinition.Name, teamDefinition.Description));
            }
        }

        db.SaveChanges();

        var teamsByName = db.Teams.ToDictionary(team => team.Name, StringComparer.OrdinalIgnoreCase);
        var usersWithoutTeams = db.Users
            .Where(user => user.TeamId == null && !string.IsNullOrWhiteSpace(user.Discipline))
            .ToList();

        foreach (var user in usersWithoutTeams)
        {
            if (!TeamCatalog.IsSupportedDiscipline(user.Discipline))
                continue;

            var teamName = TeamCatalog.GetTeamNameForDiscipline(user.Discipline);
            if (!teamsByName.TryGetValue(teamName, out var team))
                continue;

            user.AssignToTeam(team.Id);
        }

        db.SaveChanges();

        SeedDemoData(db, hasher);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Migration failed");
    }
}
app.UseSwagger();
app.UseSwaggerUI();


if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();

app.UseRequestResponseLogging();


app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void SeedDemoData(ApplicationDbContext db, IPasswordHasher hasher)
{
    const string demoAdminEmail = "admin@talentflow.demo";
    if (db.Users.Any(user => user.Email == demoAdminEmail))
        return;

    var teamsByName = db.Teams.ToDictionary(team => team.Name, StringComparer.OrdinalIgnoreCase);

    Team ResolveTeam(string discipline)
    {
        var teamName = TeamCatalog.GetTeamNameForDiscipline(discipline);
        return teamsByName[teamName];
    }

    var admin = User.Create(
        "Amina",
        "Okafor",
        demoAdminEmail,
        "Administration",
        hasher.HashPassword("Admin@123"),
        UserRole.Admin,
        null,
        LearnerProfileDefaults.CohortLabel,
        LearnerProfileDefaults.Location
    );

    var instructor = User.Create(
        "Emeka",
        "Obi",
        "emeka.obi@talentflow.demo",
        "UI/UX Design",
        hasher.HashPassword("Instructor@123"),
        UserRole.Instructor,
        ResolveTeam("UI/UX Design").Id,
        LearnerProfileDefaults.CohortLabel,
        LearnerProfileDefaults.Location
    );

    var learnerOne = User.Create(
        "Adaeze",
        "Okoro",
        "adaeze.okoro@talentflow.demo",
        "UI/UX Design",
        hasher.HashPassword("Learner@123"),
        UserRole.Learner,
        ResolveTeam("UI/UX Design").Id,
        LearnerProfileDefaults.CohortLabel,
        LearnerProfileDefaults.Location
    );

    var learnerTwo = User.Create(
        "Kolade",
        "Ige",
        "kolade.ige@talentflow.demo",
        "Front-end Engineering",
        hasher.HashPassword("Learner@123"),
        UserRole.Learner,
        ResolveTeam("Front-end Engineering").Id,
        LearnerProfileDefaults.CohortLabel,
        LearnerProfileDefaults.Location
    );

    var learnerThree = User.Create(
        "Fatima",
        "Aliyu",
        "fatima.aliyu@talentflow.demo",
        "Product Management",
        hasher.HashPassword("Learner@123"),
        UserRole.Learner,
        ResolveTeam("Product Management").Id,
        LearnerProfileDefaults.CohortLabel,
        LearnerProfileDefaults.Location
    );

    var learnerFour = User.Create(
        "Ruth",
        "Nwosu",
        "ruth.nwosu@talentflow.demo",
        "Digital Marketing",
        hasher.HashPassword("Learner@123"),
        UserRole.Learner,
        ResolveTeam("Digital Marketing").Id,
        LearnerProfileDefaults.CohortLabel,
        LearnerProfileDefaults.Location
    );

    db.Users.AddRange(admin, instructor, learnerOne, learnerTwo, learnerThree, learnerFour);
    db.SaveChanges();

    var courseOne = Course.Create(
        "UI/UX Fundamentals",
        "Learn the core principles of user interface design, research, and wireframing.",
        "Design",
        6,
        null,
        instructor.Id
    );

    var courseTwo = Course.Create(
        "Product Thinking",
        "Understand product strategy, user research, and stakeholder alignment.",
        "Product",
        5,
        null,
        instructor.Id
    );

    db.Courses.AddRange(courseOne, courseTwo);
    db.SaveChanges();

    var lessons = new[]
    {
        Lesson.Create(courseOne.Id, "Introduction to UI Design", 1),
        Lesson.Create(courseOne.Id, "User Research Methods", 2),
        Lesson.Create(courseOne.Id, "Wireframing and Prototyping", 3),
        Lesson.Create(courseOne.Id, "Design Systems", 4),
        Lesson.Create(courseTwo.Id, "Product Discovery", 1),
        Lesson.Create(courseTwo.Id, "User Journey Mapping", 2),
        Lesson.Create(courseTwo.Id, "Stakeholder Communication", 3)
    };

    foreach (var lesson in lessons)
    {
        db.Lessons.Add(lesson);
    }

    db.SaveChanges();

    foreach (var lesson in lessons)
    {
        db.LessonContents.Add(LessonContent.CreateText(lesson.Id, $"{lesson.Title} notes and learning objectives."));
        db.LessonContents.Add(LessonContent.CreatePdf(lesson.Id, $"https://cdn.talentflow.demo/resources/{lesson.Id}.pdf"));
    }

    db.SaveChanges();

    var enrollments = new[]
    {
        Enrollment.Create(learnerOne.Id, courseOne.Id),
        Enrollment.Create(learnerOne.Id, courseTwo.Id),
        Enrollment.Create(learnerTwo.Id, courseOne.Id),
        Enrollment.Create(learnerThree.Id, courseTwo.Id),
        Enrollment.Create(learnerFour.Id, courseTwo.Id)
    };

    db.Enrollments.AddRange(enrollments);
    db.SaveChanges();

    var progressOne = Progress.Create(learnerOne.Id, courseOne.Id, 4);
    progressOne.UpdateProgress(3);
    var progressTwo = Progress.Create(learnerOne.Id, courseTwo.Id, 3);
    progressTwo.UpdateProgress(1);
    var progressThree = Progress.Create(learnerTwo.Id, courseOne.Id, 4);
    progressThree.UpdateProgress(2);
    var progressFour = Progress.Create(learnerThree.Id, courseTwo.Id, 3);
    progressFour.UpdateProgress(3);
    var progressFive = Progress.Create(learnerFour.Id, courseTwo.Id, 3);
    progressFive.UpdateProgress(1);

    db.Progresses.AddRange(progressOne, progressTwo, progressThree, progressFour, progressFive);
    db.SaveChanges();

    var learnerOneLessonProgress = new[]
    {
        LessonProgress.Create(learnerOne.Id, lessons[0].Id),
        LessonProgress.Create(learnerOne.Id, lessons[1].Id),
        LessonProgress.Create(learnerOne.Id, lessons[2].Id),
        LessonProgress.Create(learnerOne.Id, lessons[4].Id)
    };

    foreach (var item in learnerOneLessonProgress)
    {
        item.MarkAsCompleted();
    }

    var learnerTwoLessonProgress = new[]
    {
        LessonProgress.Create(learnerTwo.Id, lessons[0].Id),
        LessonProgress.Create(learnerTwo.Id, lessons[1].Id)
    };

    foreach (var item in learnerTwoLessonProgress)
    {
        item.MarkAsCompleted();
    }

    var learnerThreeLessonProgress = new[]
    {
        LessonProgress.Create(learnerThree.Id, lessons[4].Id),
        LessonProgress.Create(learnerThree.Id, lessons[5].Id),
        LessonProgress.Create(learnerThree.Id, lessons[6].Id)
    };

    foreach (var item in learnerThreeLessonProgress)
    {
        item.MarkAsCompleted();
    }

    var learnerFourLessonProgress = new[]
    {
        LessonProgress.Create(learnerFour.Id, lessons[4].Id)
    };

    learnerFourLessonProgress[0].MarkAsCompleted();

    db.LessonProgresses.AddRange(
        learnerOneLessonProgress
            .Concat(learnerTwoLessonProgress)
            .Concat(learnerThreeLessonProgress)
            .Concat(learnerFourLessonProgress));
    db.SaveChanges();

    db.LessonNotes.Add(LessonNote.Create(learnerOne.Id, lessons[2].Id, "Remember to refine the checkout flow wireframes."));
    db.SaveChanges();

    var assignments = new[]
    {
        Assignment.Create(courseOne.Id, "Wireframe Challenge #3", "Design a complete e-commerce checkout flow.", DateTime.UtcNow.AddDays(7)),
        Assignment.Create(courseTwo.Id, "User Journey Map", "Create a detailed journey map for a food delivery app.", DateTime.UtcNow.AddDays(10)),
        Assignment.Create(courseTwo.Id, "Sprint Retrospective Report", "Reflect on the last sprint and identify improvements.", DateTime.UtcNow.AddDays(14))
    };

    db.Assignments.AddRange(assignments);
    db.SaveChanges();

    var submissionOne = Submission.Create(assignments[0].Id, learnerOne.Id, "Shared Figma prototype link for the checkout flow.");
    submissionOne.Grade(88, "Strong flow and clear checkout states.");
    var submissionTwo = Submission.Create(assignments[1].Id, learnerOne.Id, "Detailed journey map covering onboarding to checkout.");
    var submissionThree = Submission.Create(assignments[2].Id, learnerThree.Id, "Sprint report with team insights and blockers.");
    submissionThree.Grade(92, "Great reflection and concise improvement plan.");

    db.Submissions.AddRange(submissionOne, submissionTwo, submissionThree);
    db.SaveChanges();

    var certificate = Certificate.Create(learnerThree.Id, courseTwo.Id, 92);
    db.Certificates.Add(certificate);
    db.SaveChanges();

    var discussionTags = new[]
    {
        DiscussionTag.Create("UI/UX Design"),
        DiscussionTag.Create("Accessibility"),
        DiscussionTag.Create("Research"),
        DiscussionTag.Create("General")
    };

    db.DiscussionTags.AddRange(discussionTags);
    db.SaveChanges();

    var postOne = DiscussionPost.Create(learnerOne.Id, "Best practices for designing accessible color palettes?", "I have been working on a color system and need tips for better contrast decisions.");
    var postTwo = DiscussionPost.Create(learnerTwo.Id, "How do you run user interviews remotely?", "Need to conduct 3 interviews remotely and would love advice on structure and tooling.");
    var postThree = DiscussionPost.Create(learnerThree.Id, "Week 6 cohort meet up - who's joining?", "Planning a casual Friday hangout for the cohort after standup.");

    db.DiscussionPosts.AddRange(postOne, postTwo, postThree);
    db.SaveChanges();

    db.DiscussionPostTags.AddRange(
        DiscussionPostTag.Create(postOne.Id, discussionTags[0].Id),
        DiscussionPostTag.Create(postOne.Id, discussionTags[1].Id),
        DiscussionPostTag.Create(postTwo.Id, discussionTags[2].Id),
        DiscussionPostTag.Create(postThree.Id, discussionTags[3].Id)
    );

    db.DiscussionReplies.AddRange(
        DiscussionReply.Create(postOne.Id, learnerTwo.Id, "Start with WCAG contrast checks and test your colors on gray backgrounds too."),
        DiscussionReply.Create(postOne.Id, instructor.Id, "Pair your palette decisions with semantic usage rules so teams stay consistent."),
        DiscussionReply.Create(postTwo.Id, learnerThree.Id, "Use a consent intro, warm-up questions, and a clear wrap-up section."),
        DiscussionReply.Create(postThree.Id, learnerOne.Id, "I'm in if we can meet right after the Friday check-in.")
    );
    db.SaveChanges();

    var pendingRoleRequest = InstructorRoleRequest.Create(
        learnerTwo.Id,
        "Front-end engineer with mentoring experience and strong interest in teaching design systems.",
        "Front-end Engineering, mentoring, design systems"
    );

    var approvedRoleRequest = InstructorRoleRequest.Create(
        learnerThree.Id,
        "Product manager with experience coaching junior interns through discovery workshops.",
        "Product management, user research, facilitation"
    );
    approvedRoleRequest.Approve(admin.Id);

    var rejectedRoleRequest = InstructorRoleRequest.Create(
        learnerFour.Id,
        "Digital marketer interested in supporting cohort sessions part-time.",
        "Digital marketing, content strategy"
    );
    rejectedRoleRequest.Reject(admin.Id, "Please complete the current mentorship training track first.");

    db.InstructorRoleRequests.AddRange(pendingRoleRequest, approvedRoleRequest, rejectedRoleRequest);

    var notifications = new[]
    {
        Notification.Create(learnerOne.Id, NotificationType.System, "Welcome to TalentFlow", "Your account is ready. Explore your courses, team, and upcoming tasks.", "/dashboard"),
        Notification.Create(learnerOne.Id, NotificationType.TeamUpdate, "Team Update", "You have been added to the Design x Engineering cross-functional team.", "/my-team"),
        Notification.Create(learnerOne.Id, NotificationType.AssignmentGraded, "Assignment Graded", "Your submission for Wireframe Challenge #3 has been graded.", "/assignments"),
        Notification.Create(learnerThree.Id, NotificationType.CertificateMilestone, "Certificate Milestone", "You earned a certificate for completing Product Thinking.", "/certificates"),
        Notification.Create(learnerTwo.Id, NotificationType.System, "Role Request Pending", "Your instructor role request has been submitted and is awaiting admin review.", "/profile")
    };

    notifications[0].MarkAsRead();
    notifications[2].MarkAsRead();

    db.Notifications.AddRange(notifications);
    db.SaveChanges();
}
