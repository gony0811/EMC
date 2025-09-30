namespace EGGPLANT
{
    public sealed record ScreenItemDto(int ScreenId, string Code, string Name, bool Granted, bool IsEnabled, bool CanEdit);
    public sealed record RoleScreensGroupDto(int RoleId, string RoleName, IReadOnlyList<ScreenItemDto> Screens);
    public sealed record RoleScreenFlat(int RoleId, string RoleName, int ScreenId, string Code, string Name, bool Granted, bool IsEnabled, bool CanEdit);
}
