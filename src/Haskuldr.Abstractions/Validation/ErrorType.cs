﻿namespace Haskuldr.Abstractions.Validation;

public enum ErrorType
{
    Validation = 0,
    NotFound = 1,
    Unauthorized = 2,
    Forbidden = 3,
    Conflict = 4,
    Internal = 5,
}