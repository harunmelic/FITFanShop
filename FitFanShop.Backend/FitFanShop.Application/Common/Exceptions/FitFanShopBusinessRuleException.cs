namespace FitFanShop.Application.Common.Exceptions;
public sealed class FitFanShopBusinessRuleException : Exception
{
    public string Code { get; }
    public FitFanShopBusinessRuleException(string code, string message)
        : base(message)
    {
        Code = code;
    }
    public FitFanShopBusinessRuleException(string code, string message, Exception? innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}