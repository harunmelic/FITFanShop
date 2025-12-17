namespace FitFanShop.Shared.Resources;

/// <summary>
/// Centralized messages for API responses and error handling.
/// All messages are in English (default language).
/// </summary>
public static class Messages
{
    // Product messages
    public static string ProductNotFound => "Product not found.";
    public static string ProductCreatedSuccessfully => "Product created successfully.";
    public static string ProductUpdatedSuccessfully => "Product updated successfully.";
    public static string ProductDeletedSuccessfully => "Product deleted successfully.";
    public static string InsufficientStock => "Insufficient stock available.";
    
    // Order messages
    public static string OrderNotFound => "Order not found.";
    public static string OrderCreatedSuccessfully => "Order created successfully.";
    public static string OrderUpdatedSuccessfully => "Order updated successfully.";
    public static string OrderCancelled => "Order cancelled successfully.";
    
    // Event messages
    public static string EventNotFound => "Event not found.";
    public static string EventNotAvailable => "Event is not available.";
    public static string TicketPurchasedSuccessfully => "Ticket purchased successfully.";
    
    // Membership messages
    public static string MembershipRequired => "Active membership required.";
    public static string MembershipExpired => "Your membership has expired.";
    public static string ExclusiveProductMembersOnly => "This product is exclusive to active members only.";
    
    // Discount messages
    public static string DiscountNotFound => "Discount not found.";
    public static string DiscountNotActive => "Discount is not currently active.";
    public static string DiscountMembersOnly => "This discount is only available to active members.";
    
    // Authentication messages
    public static string InvalidCredentials => "Invalid email or password.";
    public static string UserNotFound => "User not found.";
    public static string EmailAlreadyExists => "Email already exists.";
    public static string RegistrationSuccessful => "Registration successful.";
    public static string LoginSuccessful => "Login successful.";
    public static string LogoutSuccessful => "Logout successful.";
    
    // Validation messages
    public static string ValidationFailed => "Validation failed.";
    public static string RequiredField => "This field is required.";
    public static string InvalidEmailFormat => "Invalid email format.";
    public static string PasswordTooShort => "Password must be at least 6 characters.";
    
    // Category messages
    public static string CategoryNotFound => "Category not found.";
    public static string CategoryCreatedSuccessfully => "Category created successfully.";
    public static string CategoryUpdatedSuccessfully => "Category updated successfully.";
    public static string CategoryDeletedSuccessfully => "Category deleted successfully.";
    
    // Cart messages
    public static string CartItemAdded => "Item added to cart.";
    public static string CartItemRemoved => "Item removed from cart.";
    public static string CartCleared => "Cart cleared.";
    public static string CartIsEmpty => "Cart is empty.";
    
    // Ticket messages
    public static string TicketNotFound => "Ticket not found.";
    public static string TicketCancelled => "Ticket cancelled successfully.";
    public static string NoTicketsAvailable => "No tickets available.";
    
    // General messages
    public static string OperationSuccessful => "Operation completed successfully.";
    public static string OperationFailed => "Operation failed.";
    public static string UnauthorizedAccess => "Unauthorized access.";
    public static string ForbiddenAccess => "Access forbidden.";
    public static string InternalServerError => "Internal server error occurred.";
}
