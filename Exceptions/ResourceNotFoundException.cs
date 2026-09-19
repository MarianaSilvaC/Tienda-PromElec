namespace TiendaPromElec.Exceptions;

public sealed class ResourceNotFoundException(string message) : Exception(message);
