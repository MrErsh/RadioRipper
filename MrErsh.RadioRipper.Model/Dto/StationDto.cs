using System;
using System.Collections.Generic;

namespace MrErsh.RadioRipper.Model.Dto
{
    public sealed record StationDto(
        Guid Id,
        string Name,
        string Url,
        string Comment,
        bool IsRunning,
        List<DateTime> Dates);
}
