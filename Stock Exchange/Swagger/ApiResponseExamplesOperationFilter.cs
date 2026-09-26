using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;

namespace Stock_Exchange.Swagger;

/// <summary>
/// Attaches switchable <c>ApiResponse</c> examples (with a representative
/// <c>data</c> payload per endpoint) to every documented status code, so the
/// Swagger UI "Responses" section shows an Examples dropdown per code instead
/// of just a schema.
/// </summary>
public class ApiResponseExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var (statusCodeKey, response) in operation.Responses)
        {
            if (!int.TryParse(statusCodeKey, out var statusCode))
                continue;

            if (!response.Content.TryGetValue("application/json", out var mediaType) || mediaType is null)
                continue;

            var dataSample = ResolveDataSample(context, statusCode);

            mediaType.Examples = statusCode switch
            {
                >= 200 and < 300 => new Dictionary<string, OpenApiExample>
                {
                    ["success"] = new OpenApiExample
                    {
                        Summary = "Success",
                        Value = ErrorEnvelope(false, dataSample, SuccessMessage(statusCode), statusCode)
                    }
                },
                400 => new Dictionary<string, OpenApiExample>
                {
                    ["validationError"] = new OpenApiExample
                    {
                        Summary = "Validation error (field-level)",
                        Value = ErrorEnvelope(
                            true,
                            new OpenApiNull(),
                            "One or more validation errors occurred.",
                            400,
                            new Dictionary<string, string[]>
                            {
                                ["email"] = ["Invalid email address format."]
                            })
                    },
                    ["businessError"] = new OpenApiExample
                    {
                        Summary = "Business rule violation",
                        Value = ErrorEnvelope(
                            true,
                            new OpenApiNull(),
                            "The request is invalid.",
                            400,
                            new Dictionary<string, string[]>
                            {
                                ["General"] = ["Invalid refresh token."]
                            })
                    }
                },
                401 => new Dictionary<string, OpenApiExample>
                {
                    ["missingOrMalformed"] = new OpenApiExample
                    {
                        Summary = "Missing or malformed token",
                        Value = ErrorEnvelope(
                            true,
                            new OpenApiNull(),
                            "You are not authorized to perform this action.",
                            401,
                            new Dictionary<string, string[]>
                            {
                                ["General"] = ["You are not authorized to perform this action."]
                            })
                    },
                    ["expiredOrRevokedSession"] = new OpenApiExample
                    {
                        Summary = "Expired session — log in (or refresh) again",
                        Description = "The access token expired, was killed by logout/refresh rotation, or belongs to a deactivated account. Obtain a new token via login or refresh-token.",
                        Value = ErrorEnvelope(
                            true,
                            new OpenApiNull(),
                            "You are not authorized to perform this action.",
                            401,
                            new Dictionary<string, string[]>
                            {
                                ["General"] = ["You are not authorized to perform this action."]
                            })
                    }
                },
                _ => new Dictionary<string, OpenApiExample>
                {
                    ["error"] = new OpenApiExample
                    {
                        Summary = ErrorSummary(statusCode),
                        Value = ErrorEnvelope(
                            true,
                            new OpenApiNull(),
                            ErrorMessage(statusCode),
                            statusCode,
                            new Dictionary<string, string[]>
                            {
                                ["General"] = [ErrorMessage(statusCode)]
                            })
                    }
                }
            };
        }
    }

    private static IOpenApiAny ResolveDataSample(OperationFilterContext context, int statusCode)
    {
        var declaredType = context.ApiDescription.SupportedResponseTypes
            .FirstOrDefault(r => r.StatusCode == statusCode)?.Type;

        if (declaredType is null)
            return new OpenApiNull();

        // Unwrap ApiResponse<TData> to sample TData.
        var payloadType = declaredType.IsGenericType
            && declaredType.GetGenericTypeDefinition().Name == "ApiResponse`1"
            ? declaredType.GetGenericArguments()[0]
            : declaredType;

        return SampleOf(payloadType, depth: 0);
    }

    private static IOpenApiAny SampleOf(Type type, int depth)
    {
        if (depth > 3)
            return new OpenApiString("...");

        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying is not null)
            return SampleOf(underlying, depth);

        if (type == typeof(string) || type == typeof(char))
            return new OpenApiString("string");

        if (type == typeof(bool))
            return new OpenApiBoolean(true);

        if (type == typeof(Guid))
            return new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6");

        if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
            return new OpenApiString("2026-09-26T22:32:00");

        if (type == typeof(TimeSpan))
            return new OpenApiString("00:10:00");

        if (type.IsEnum)
        {
            var names = Enum.GetNames(type);
            return new OpenApiString(names.Length > 0 ? names[0] : type.Name);
        }

        if (type == typeof(int) || type == typeof(short) || type == typeof(byte))
            return new OpenApiInteger(0);

        if (type == typeof(long))
            return new OpenApiLong(0);

        if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
            return new OpenApiDouble(0);

        if (typeof(IDictionary).IsAssignableFrom(type))
            return new OpenApiObject();

        if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
        {
            var elementType = type.IsArray
                ? type.GetElementType()!
                : type.IsGenericType
                    ? type.GetGenericArguments()[0]
                    : typeof(object);

            return new OpenApiArray { SampleOf(elementType, depth + 1) };
        }

        if (type.IsClass || (type.IsValueType && !type.IsPrimitive))
        {
            var obj = new OpenApiObject();
            foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (!prop.CanRead || prop.GetIndexParameters().Length > 0)
                    continue;

                obj.Add(ToCamelCase(prop.Name), SampleOf(prop.PropertyType, depth + 1));
            }

            return obj;
        }

        return new OpenApiString("string");
    }

    private static IOpenApiAny ErrorEnvelope(
        bool failed,
        IOpenApiAny data,
        string message,
        int statusCode,
        IDictionary<string, string[]>? errors = null)
    {
        var errorsObject = new OpenApiObject();
        if (errors is not null)
        {
            foreach (var (field, messages) in errors)
            {
                var arr = new OpenApiArray();
                foreach (var m in messages)
                    arr.Add(new OpenApiString(m));

                errorsObject.Add(field, arr);
            }
        }

        return new OpenApiObject
        {
            ["success"] = new OpenApiBoolean(!failed),
            ["isSuccess"] = new OpenApiBoolean(!failed),
            ["errors"] = errorsObject,
            ["data"] = data,
            ["message"] = new OpenApiString(message),
            ["statusCode"] = new OpenApiInteger(statusCode)
        };
    }

    private static string ToCamelCase(string name) =>
        string.IsNullOrEmpty(name) ? name : char.ToLowerInvariant(name[0]) + name[1..];

    private static string SuccessMessage(int statusCode) => statusCode switch
    {
        201 => "Resource created successfully.",
        202 => "Request accepted for processing.",
        _ => "Operation completed successfully."
    };

    private static string ErrorSummary(int statusCode) => statusCode switch
    {
        403 => "Forbidden — missing role/permission",
        404 => "Not found",
        409 => "Conflict",
        429 => "Too many requests",
        _ => "Error"
    };

    private static string ErrorMessage(int statusCode) => statusCode switch
    {
        403 => "You do not have permission to access this resource.",
        404 => "The requested resource was not found.",
        409 => "A conflict occurred with the current state of the resource.",
        429 => "Too many requests have been sent. Please try again later.",
        _ => "The request is invalid."
    };
}
