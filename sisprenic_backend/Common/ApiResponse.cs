using System.Text.Json.Serialization;

namespace sisprenic_backend.Common;

public sealed record ApiResponse<T>(
    T Data,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyList<ApiMessage>? Messages = null
);