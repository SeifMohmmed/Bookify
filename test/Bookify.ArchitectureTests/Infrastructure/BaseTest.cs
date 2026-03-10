using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure;
using System.Reflection;

namespace Bookify.ArchitectureTests.Infrastructure;
/// <summary>
/// Base class used by architecture tests.
/// It exposes references to the main project assemblies
/// (Domain, Application, Infrastructure, Presentation)
/// so tests can verify architectural rules between layers.
/// </summary>
public abstract class BaseTest
{
    /// <summary>
    /// Gets the Domain layer assembly.
    /// We use the Entity type as a marker to locate the Domain project assembly.
    /// </summary>
    protected static Assembly DomainAssembly => typeof(Entity).Assembly;

    /// <summary>
    /// Gets the Application layer assembly.
    /// IBaseCommand is used as a marker interface to locate the Application project assembly.
    /// </summary>
    protected static Assembly ApplicationAssembly => typeof(IBaseCommand).Assembly;

    /// <summary>
    /// Gets the Infrastructure layer assembly.
    /// ApplicationDbContext is used as a reference type to identify the Infrastructure assembly.
    /// </summary>
    protected static Assembly InfrastructureAssembly => typeof(ApplicationDbContext).Assembly;

    /// <summary>
    /// Gets the Presentation layer assembly (API project).
    /// Program class is used as a marker to reference the Web/API assembly.
    /// </summary>
    protected static Assembly PresentationAssembly => typeof(Program).Assembly;
}