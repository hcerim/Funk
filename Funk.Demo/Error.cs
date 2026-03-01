using System;

namespace Funk.Demo;

public class Error(string message) : Exception(message);

public class InvalidRequestError(string message) : Error(message);

public class UnauthorizedError(string message) : Error(message);

public class ForbiddenError(string message) : Error(message);