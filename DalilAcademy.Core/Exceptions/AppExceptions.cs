namespace DalilAcademy.Core.Exceptions;

// Each exception names the problem — and maps to one status code in GlobalExceptionHandler.
public class NotFoundException(string message) : Exception(message);       // -> 404
public class ConflictException(string message) : Exception(message);       // -> 409
public class UnauthorizedException(string message) : Exception(message);   // -> 401
