namespace FitFanShop.Application.Common.Exceptions;
public sealed class FitFanShopConflictException : Exception
{
    public FitFanShopConflictException(string message) : base(message) { }
}
public sealed class FitFanShopInvalidCredentialsException : Exception
{
    public FitFanShopInvalidCredentialsException(string message) : base(message) { }
}
