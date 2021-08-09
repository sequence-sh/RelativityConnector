using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Flurl.Http;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Kepler.Services;
using Relativity.Services.ServiceProxy;

namespace Reductech.EDR.Connectors.Relativity.Managers
{



public record ManagerGenerator(
    Type Type,
    IReadOnlyList<string> ServicePrefixes,
    IReadOnlyList<string> ExtraNamespaces) { }

public class CodeGenerator
{
    public static readonly IReadOnlyList<string> RelativityObjectModelPrefixes = new List<string>()
    {
        "relativity-object-model", "v{RelativitySettings.APIVersionNumber}",
    };

    public static readonly IReadOnlyList<string> RelativityEnvironmentPrefixes = new List<string>()
    {
        "relativity-environment", "v{RelativitySettings.APIVersionNumber}",
    };

    public static IEnumerable<ManagerGenerator> ManagerGenerators
    {
        get
        {
            yield return new ManagerGenerator(
                typeof(IMatterManager1),
                RelativityEnvironmentPrefixes,
                new List<string>() { "Relativity.Shared.V1.Models" }
            );

            yield return new ManagerGenerator(
                typeof(IWorkspaceManager1),
                RelativityEnvironmentPrefixes,
                new List<string>()
                {
                    "Relativity.Environment.V1.Shared.Models", "Relativity.Shared.V1.Models",
                }
            );

            yield return new ManagerGenerator(
                typeof(IObjectManager1),
                new List<string>(){"Relativity.Objects"},
                new List<string>()
                {
                    "Relativity.Services.Objects",
                    "Relativity.Services.Objects.DataContracts",
                    "Relativity.Services.DataContracts.DTOs",
                    "Relativity.Services.DataContracts.DTOs.Results",
                    "Relativity.Services.Interfaces.Shared"
                }
            );

            yield return new ManagerGenerator(
                typeof(IDocumentFileManager1),
                RelativityObjectModelPrefixes,
                new List<string>()
                {
                    "Relativity.Services.Interfaces.Document",
                    "Relativity.Services.Interfaces.Document.Models",
                }
            );

            yield return new ManagerGenerator(
                typeof(IKeywordSearchManager1),
                new List<string>()
                {
                    "Relativity.Services.Search.ISearchModule",
                    
                    "Keyword Search Manager"
                },
                new List<string>()
                {
                    "Relativity.Services.Search",
                    "Relativity.Services.Interfaces.Document",
                    "Relativity.Services.Interfaces.Document.Models",
                }
            );
            
            yield return new ManagerGenerator(
                typeof(IFolderManager1),
                new List<string>()
                {
                    "Relativity.Services.Folder.IFolderModule",
                    "Folder Manager"
                },
                new List<string>()
                {
                    "Relativity.Services.Interfaces.Document",
                    "Relativity.Services.Interfaces.Document.Models",
                }
            );
        }
    }

    public static string Generate(ManagerGenerator managerGenerator)
    {
        var sb = new StringBuilder();

        foreach (var ns in Namespaces.Concat(managerGenerator.ExtraNamespaces).Distinct())
        {
            sb.AppendLine($"using {ns};");
        }

        sb.AppendLine();

        sb.AppendLine("namespace Reductech.EDR.Connectors.Relativity.Managers");
        sb.AppendLine("{");

        var text = GetFileText(managerGenerator);

        sb.AppendLine(text);
        sb.AppendLine();

        sb.AppendLine("}");

        return sb.ToString().Trim();
    }

    private static IEnumerable<string> Namespaces
    {
        get
        {
            yield return "System";
            yield return "System.CodeDom.Compiler";
            yield return "System.Collections.Generic";
            yield return "System.Linq";
            yield return "System.Text";
            yield return "System.Threading";
            yield return "System.Threading.Tasks";
            yield return "Flurl.Http";
            yield return "Reductech.EDR.Connectors.Relativity.ManagerInterfaces";

            yield return "Relativity.Kepler.Transport";
            yield return "Relativity.Environment.V1.Matter";
            yield return "Relativity.Environment.V1.Matter.Models";
            yield return "Relativity.Environment.V1.Workspace.Models";
            yield return "Relativity.Environment.V1.Workspace";
        }
    }

    public static string GetFileText(ManagerGenerator managerGenerator)
    {
        var type = managerGenerator.Type;
        var sb   = new CodeStringBuilder();

        var className = "Template" + type.Name[1..];

        sb.AppendLine("[GeneratedCode(\"CodeGenerator\", \"1\")]");
        sb.AppendLine($"public class {className} : ManagerBase, {type.Name}");
        sb.AppendLine("{");
        sb.Indent();

        sb.AppendLine(
            $"public {className}(RelativitySettings relativitySettings, IFlurlClient flurlClient)"
        );

        sb.AppendLine(":base(relativitySettings, flurlClient) { }");

        sb.AppendLine("");
        sb.AppendLine("/// <inheritdoc />");
        sb.AppendLine("protected override IEnumerable<string> Prefixes");
        sb.AppendLine("{");
        sb.Indent();
        sb.AppendLine("get");
        sb.AppendLine("{");
        sb.Indent();

        foreach (var prefix in managerGenerator.ServicePrefixes)
        {
            sb.AppendLine($"yield return $\"{prefix}\";");
        }

        sb.AppendLine("yield break;");
        sb.UnIndent();
        sb.AppendLine("}");
        sb.UnIndent();
        sb.AppendLine("}");
        sb.AppendLine("");

        var route = type.GetCustomAttribute<RoutePrefixAttribute>();

        var routePrefix = FixRouteTemplate(route?.Prefix ?? "");

        foreach (var methodInfo in type.GetMethods())
        {
            AppendMethod(sb, methodInfo, routePrefix);
        }

        sb.UnIndent();
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static void AppendMethod(
        CodeStringBuilder sb,
        MethodInfo methodInfo,
        string routePrefix)
    {
        StringBuilder signature = new();

        signature.Append("public ");
        signature.Append(ToGenericTypeString(methodInfo.ReturnType));
        signature.Append(" ");
        signature.Append(methodInfo.Name);
        signature.Append("(");

        var parameters = methodInfo.GetParameters()
            .Select(x => ToGenericTypeString(x.ParameterType) + " " + x.Name);

        signature.Append(string.Join(", ", parameters));
        signature.Append(")");

        sb.AppendLine("");
        sb.AppendLine(signature.ToString());
        sb.AppendLine("{");
        sb.Indent();

        var ctParameter = methodInfo.GetParameters()
            .FirstOrDefault(x => x.ParameterType == typeof(CancellationToken));

        if (ctParameter is null)
        {
            sb.AppendLine("var cancellationToken = CancellationToken.None;");
        }
        else
        {
            sb.AppendLine($"var cancellationToken = {ctParameter.Name};");
        }

        var route = methodInfo.GetCustomAttribute<RouteAttribute>();

        if (route is null)
            throw new Exception(
                $"{methodInfo.DeclaringType.FullName}.{methodInfo.Name} does not have a route attribute"
            );

        sb.AppendLine($"var route = $\"{routePrefix}/{FixRouteTemplate(route.Template)}\";");


        void AppendCreateJsonObject()
        {
            sb.AppendLine("var jsonObject = new {");
            sb.Indent();

            var jsonParameters = methodInfo.GetParameters()
                .Where(x => x.GetCustomAttribute<JsonParameterAttribute>() is not null)
                .ToList();

            if (!jsonParameters.Any())
            {
                throw new Exception($"{methodInfo.Name} has no Json Parameters");
            }

            foreach (var jsonParameter in jsonParameters)
            {
                sb.AppendLine(jsonParameter.Name + ",");
            }

            sb.UnIndent();
            sb.AppendLine("};");
        }

        if (methodInfo.GetCustomAttribute<HttpPostAttribute>() is not null)
        {
            AppendCreateJsonObject();
            if (methodInfo.ReturnType.IsGenericType)
            {
                var returnType = ToGenericTypeString(methodInfo.ReturnType.GenericTypeArguments[0]);

                sb.AppendLine(
                    $"return PostJsonAsync<{returnType}>(route, jsonObject, cancellationToken);"
                );
            }
            else
            {
                sb.AppendLine($"return PostJsonAsync(route, jsonObject, cancellationToken);");
            }
        }
        else if (methodInfo.GetCustomAttribute<HttpPutAttribute>() is not null)
        {
            AppendCreateJsonObject();
            if (methodInfo.ReturnType.IsGenericType)
            {
                var returnType = ToGenericTypeString(methodInfo.ReturnType.GenericTypeArguments[0]);

                sb.AppendLine(
                    $"return PutAsync<{returnType}>(route, jsonObject, cancellationToken);"
                );
            }
            else
            {
                sb.AppendLine($"return PutAsync(route, jsonObject, cancellationToken);");
            }
        }
        else if (methodInfo.GetCustomAttribute<HttpGetAttribute>() is not null)
        {
            var returnType = ToGenericTypeString(methodInfo.ReturnType.GenericTypeArguments[0]);
            sb.AppendLine($"return GetJsonAsync<{returnType}>(route, cancellationToken);");
        }
        else if (methodInfo.GetCustomAttribute<HttpDeleteAttribute>() is not null)
        {
            sb.AppendLine($"return DeleteAsync(route, cancellationToken);");
        }

        else
        {
            sb.AppendLine("throw new NotImplementedException();");
        }

        sb.UnIndent();
        sb.AppendLine("}");
        sb.AppendLine("");
    }

    private static string FixRouteTemplate(string s)
    {
        return ParameterRegex.Replace(s, match => $"{{{match.Groups["name"].Value}}}");
    }

    private static readonly Regex ParameterRegex = new(
        @"{(?<name>\w+)\s*:\s*(?<type>\w+)}",
        RegexOptions.Compiled
    );

    public static string ToGenericTypeString(Type t)
    {
        if (!t.IsGenericType)
            return t.Name;

        string genericTypeName = t.GetGenericTypeDefinition().Name;

        genericTypeName = genericTypeName.Substring(
            0,
            genericTypeName.IndexOf('`')
        );

        string genericArgs = string.Join(
            ",",
            t.GetGenericArguments()
                .Select(ToGenericTypeString)
                .ToArray()
        );

        return genericTypeName + "<" + genericArgs + ">";
    }

    private class CodeStringBuilder
    {
        private StringBuilder StringBuilder { get; } = new();
        private int Indentation { get; set; } = 0;

        public void AppendLine(string s)
        {
            var indent = new string('\t', Indentation);

            StringBuilder.AppendLine(indent + s);
        }

        public void Indent() => Indentation++;

        public void UnIndent() => Indentation--;

        /// <inheritdoc />
        public override string ToString() => StringBuilder.ToString();
    }
}

public class TemplateServiceFactoryFactory : IServiceFactoryFactory
{
    public TemplateServiceFactoryFactory(IFlurlClient flurlClient) => FlurlClient = flurlClient;

    public IFlurlClient FlurlClient { get; }

    /// <inheritdoc />
    public IServiceFactory CreateServiceFactory(RelativitySettings relativitySettings)
    {
        return new TemplateServiceFactory(relativitySettings, FlurlClient);
    }

    public class TemplateServiceFactory : IServiceFactory
    {
        public TemplateServiceFactory(
            RelativitySettings relativitySettings,
            IFlurlClient flurlClient)
        {
            RelativitySettings = relativitySettings;
            FlurlClient        = flurlClient;
                

            var types = typeof(TemplateServiceFactory).Assembly.GetTypes()
                .Where(x => !x.IsAbstract && x.IsAssignableTo(typeof(ManagerBase)));

            Managers = types.Select(
                    t =>
                        (ManagerBase)
                        Activator.CreateInstance(
                            t,
                            new object?[] { RelativitySettings, FlurlClient },
                            null
                        )!
                )
                .ToList();
        }

        public RelativitySettings RelativitySettings { get; }
        public IFlurlClient FlurlClient { get; }

        public readonly IReadOnlyList<ManagerBase> Managers;

        /// <inheritdoc />
        public T CreateProxy<T>() where T : IDisposable
        {
            var proxy = Managers.OfType<T>().FirstOrDefault();

            return proxy ??
                   throw new InvalidOperationException(
                       $"Could not create a proxy of type {typeof(T).Name} with a TemplateServiceFactory"
                   );
        }
    }
}

}
