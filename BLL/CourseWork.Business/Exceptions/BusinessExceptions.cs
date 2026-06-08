namespace CourseWork.Business.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}

public class NotFoundException : BusinessException
{
    public NotFoundException(string entityName, int id)
        : base($"{entityName} з ідентифікатором {id} не знайдено.") { }
}

public class ValidationException : BusinessException
{
    public ValidationException(string message) : base(message) { }
}

public class UnauthorizedBusinessException : BusinessException
{
    public UnauthorizedBusinessException(string message) : base(message) { }
}

public class InsufficientAvailabilityException : BusinessException
{
    public InsufficientAvailabilityException(string message) : base(message) { }
}
