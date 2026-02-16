namespace Outclass.BuildingBlocks.Domain;

public abstract class DomainException : Exception
{
    public string Code { get; }

    protected DomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, Guid id)
        : base("NOT_FOUND", $"{entityName} with id '{id}' was not found.") { }
}

public class ConflictException : DomainException
{
    public ConflictException(string message) : base("CONFLICT", message) { }
}

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "Access denied.") : base("FORBIDDEN", message) { }
}

public class TenantNotResolvedException : DomainException
{
    public TenantNotResolvedException() : base("TENANT_NOT_RESOLVED", "Tenant context could not be resolved.") { }
}

public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("VALIDATION_FAILED", "One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}
