# Modular Programming in C#

Modular programming is a software design technique that emphasizes separating functionality into independent, interchangeable modules. Each module contains everything necessary to execute one aspect of the desired functionality. This approach makes code easier to understand, maintain, test, and reuse.

## Definition
**Modular** means building software out of small, independent pieces (modules) that each do one job. These pieces can be put together in different ways to make bigger systems.

## Explain Like I'm 8
Imagine building with LEGO blocks. Each block is a module. You can use different blocks to build a house, a car, or a spaceship. If you want to change your spaceship, you can swap out just one block without rebuilding everything. In software, modular code is like LEGO: you can add, remove, or change parts easily.

## Modular Middleware Example
Each middleware in ASP.NET Core is a module. You can add a logging module, an error-handling module, or an authentication module. You can mix and match them as needed.

```csharp
// Logging module (middleware)
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    public LoggingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("Logging: " + context.Request.Path);
        await _next(context); // Pass to the next module
    }
}

// Error handling module (middleware)
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Pass to the next module
        }
        catch
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Oops! Something went wrong.");
        }
    }
}
```

## Modular C# Code Examples (Non-Middleware)

### 1. Modular Class with Interface
```csharp
public interface ICalculator
{
    int Add(int a, int b);
    int Subtract(int a, int b);
}

public class SimpleCalculator : ICalculator
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
}

// Usage
ICalculator calc = new SimpleCalculator();
Console.WriteLine(calc.Add(2, 3)); // 5
```

### 2. Modular Utility Class
```csharp
public static class StringUtils
{
    public static bool IsNullOrEmpty(string value) => string.IsNullOrEmpty(value);
    public static string ToTitleCase(string value) =>
        System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
}

// Usage
Console.WriteLine(StringUtils.ToTitleCase("hello world")); // Hello World
```

### 3. Modular Event Handler
```csharp
public class Button
{
    public event Action Clicked;
    public void Click() => Clicked?.Invoke();
}

public class Logger
{
    public void LogClick() => Console.WriteLine("Button was clicked!");
}

// Usage
var button = new Button();
var logger = new Logger();
button.Clicked += logger.LogClick;
button.Click(); // Output: Button was clicked!
```

### 4. Modular Data Access Layer
```csharp
public interface IRepository<T>
{
    void Add(T item);
    IEnumerable<T> GetAll();
}

public class InMemoryRepository<T> : IRepository<T>
{
    private readonly List<T> _items = new();
    public void Add(T item) => _items.Add(item);
    public IEnumerable<T> GetAll() => _items;
}

// Usage
IRepository<string> repo = new InMemoryRepository<string>();
repo.Add("Hello");
repo.Add("World");
foreach (var item in repo.GetAll())
    Console.WriteLine(item);
```

## Summary
Modular programming in C# means building your application from small, focused, and reusable pieces. This makes your codebase easier to understand, test, and maintain—just like building with LEGO blocks!
