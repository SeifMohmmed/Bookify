using Dapper;
using System.Data;

namespace Bookify.Infrastructure.Data;
/// <summary>
/// Custom Dapper type handler for DateOnly.
/// Dapper does not natively support DateOnly,
/// so we map it manually to/from DateTime.
/// </summary>
internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    /// <summary>
    /// Converts database value (DateTime) to DateOnly.
    /// Called when reading from the database.
    /// </summary>
    public override DateOnly Parse(object value) =>
        DateOnly.FromDateTime((DateTime)value);

    /// <summary>
    /// Converts DateOnly to a database-compatible type.
    /// Called when writing to the database.
    /// </summary>
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;  // Explicitly map to SQL DATE type (without time)

        // Dapper/Npgsql can handle DateOnly directly,
        // but setting DbType ensures consistency
        parameter.Value = value;
    }
}