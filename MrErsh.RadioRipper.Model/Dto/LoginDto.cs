namespace MrErsh.RadioRipper.Model.Dto
{
    public record LoginDto(
        string UserName,
        string Password,
        bool CreateNew = false);
}
