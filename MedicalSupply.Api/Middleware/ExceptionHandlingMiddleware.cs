using MedicalSupply.Application.Exceptions;
using MedicalSupply.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MedicalSupply.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;

            var (statusCode, code, message) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND", exception.Message),

                InsufficientStockException => (HttpStatusCode.Conflict, "INSUFFICIENT_STOCK", exception.Message),
                InvalidStatusTransitionException => (HttpStatusCode.Conflict, "INVALID_STATUS_TRANSITION", exception.Message),
                DuplicateApprovalException => (HttpStatusCode.Conflict, "DUPLICATE_APPROVAL", exception.Message),
                BudgetExceededException => (HttpStatusCode.Conflict, "BUDGET_EXCEEDED", exception.Message),
                BusinessRuleException => (HttpStatusCode.Conflict, "BUSINESS_RULE_VIOLATION", exception.Message),

                DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "CONCURRENCY_CONFLICT",
                    "The item was modified by another operation. Please retry."),

                ArgumentException => (HttpStatusCode.BadRequest, "INVALID_INPUT", exception.Message),

                _ => (HttpStatusCode.InternalServerError, "INTERNAL_SERVER_ERROR", "An unexpected error occurred.")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);
            }
            else
            {
                _logger.LogWarning("Handled exception: {ExceptionType}. TraceId: {TraceId}. Message: {Message}",
                    exception.GetType().Name, traceId, exception.Message);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                code,
                message,
                traceId
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}