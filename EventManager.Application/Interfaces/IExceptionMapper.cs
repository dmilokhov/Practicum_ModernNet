using EventManager.Application.Model.Mapping;

namespace EventManager.Application.Interfaces;

public interface IExceptionMapper
{
    ExceptionMappingModel? Map(Exception exception);
}
