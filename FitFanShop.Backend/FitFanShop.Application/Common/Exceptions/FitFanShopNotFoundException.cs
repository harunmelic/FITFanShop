namespace FitFanShop.Application.Common.Exceptions;

public sealed class FitFanShopNotFoundException : Exception
{
    public FitFanShopNotFoundException(string message) : base(message) { }
}
