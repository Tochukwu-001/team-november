namespace LMS.Application.Common;

public sealed record TeamDefinition(string Name, string Description, string[] Disciplines);

public static class TeamCatalog
{
    public static readonly TeamDefinition[] DefaultTeams =
    [
        new(
            "Design x Engineering",
            "UI/UX and engineering contributors building product experiences.",
            ["UI/UX Design", "Front-end Engineering", "Back-end Engineering", "Mobile Development"]
        ),
        new(
            "Data & Product",
            "Members focused on product direction, research, and analytics.",
            ["Data Analysis", "Product Management"]
        ),
        new(
            "Growth & Marketing",
            "Members shaping growth, brand communication, and audience strategy.",
            ["Digital Marketing", "Content Strategy"]
        ),
        new(
            "DevOps & QA",
            "Members supporting delivery pipelines, reliability, and product quality.",
            ["DevOps", "QA Engineering"]
        )
    ];

    public static IReadOnlyCollection<string> SupportedDisciplines =>
        DefaultTeams.SelectMany(team => team.Disciplines).ToArray();

    public static bool IsSupportedDiscipline(string discipline) =>
        TryNormalizeDiscipline(discipline, out _);

    public static string NormalizeDiscipline(string discipline)
    {
        if (!TryNormalizeDiscipline(discipline, out var normalizedDiscipline))
            throw new ArgumentException("Invalid discipline", nameof(discipline));

        return normalizedDiscipline;
    }

    public static string GetTeamNameForDiscipline(string discipline)
    {
        var normalizedDiscipline = NormalizeDiscipline(discipline);

        var team = DefaultTeams.First(team =>
            team.Disciplines.Contains(normalizedDiscipline, StringComparer.OrdinalIgnoreCase));

        return team.Name;
    }

    public static TeamDefinition GetTeamDefinition(string teamName)
    {
        var team = DefaultTeams.FirstOrDefault(team =>
            string.Equals(team.Name, teamName, StringComparison.OrdinalIgnoreCase));

        return team ?? throw new ArgumentException("Invalid team name", nameof(teamName));
    }

    private static bool TryNormalizeDiscipline(string discipline, out string normalizedDiscipline)
    {
        normalizedDiscipline = SupportedDisciplines.FirstOrDefault(item =>
            string.Equals(item, discipline?.Trim(), StringComparison.OrdinalIgnoreCase)) ?? string.Empty;

        return !string.IsNullOrWhiteSpace(normalizedDiscipline);
    }
}
