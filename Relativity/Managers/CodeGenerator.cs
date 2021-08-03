using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Relativity.Environment.V1.Matter;
using Relativity.Environment.V1.Workspace;
using Relativity.Kepler.Services;
using Relativity.Services.Folder;
using Relativity.Services.Search;
using Relativity.Services.ServiceProxy;
using Relativity.Services.Objects;
using IFieldManager = Relativity.Services.Interfaces.Field.IFieldManager;

namespace Reductech.EDR.Connectors.Relativity
{
    internal class CodeGenerator
    {
        public static IEnumerable<Type> Types
        {
            get
            {
                yield return typeof(IMatterManager);
                yield return typeof(IWorkspaceManager);
                yield return typeof(IFieldManager);
                yield return typeof(IKeywordSearchManager);
                yield return typeof(IFolderManager);
                
                yield return typeof(IObjectManager);
            }
        }

        public static string Generate()
        {
            var sb = new StringBuilder();

            foreach (var ns in Namespaces)
            {
                sb.AppendLine($"using {ns};");
            }

            sb.AppendLine();

            sb.AppendLine("namespace Reductech.EDR.Connectors.Relativity");
            sb.AppendLine("{");

            foreach (var type in Types)
            {
                var text = GetFileText(type);

                sb.AppendLine(text);
                sb.AppendLine();
            }

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

                yield return "Relativity.Environment.V1.Matter";
                yield return "Relativity.Environment.V1.Matter.Models";
                yield return "Relativity.Environment.V1.Shared.Models";
                yield return "Relativity.Shared.V1.Models";
                yield return "Relativity.Environment.V1.Workspace.Models";
                yield return "Relativity.Environment.V1.Workspace";
            }
        }

        public static string GetFileText(Type type)
        {
            var sb = new CodeStringBuilder();

            var className = "Template" + type.Name[1..];

            sb.AppendLine("[GeneratedCode(\"CodeGenerator\", \"1\")]");
            sb.AppendLine($"public class {className} : ManagerBase, {type.Name}");
            sb.AppendLine("{");
            sb.Indent();

            sb.AppendLine($"public {className}(RelativitySettings relativitySettings, IFlurlClient flurlClient)");
            sb.AppendLine(":base(relativitySettings, flurlClient) { }");

            var route = type.GetCustomAttribute<RoutePrefixAttribute>();

            if (route is null)
                throw new Exception($"Type {type.FullName} does not have a RoutePrefixAttribute");

            sb.AppendLine($"public override string RoutePrefix => \"{route.Prefix}\";");

            foreach (var methodInfo in type.GetMethods())
            {
                AppendMethod(sb, methodInfo);
            }

            sb.UnIndent();
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static void AppendMethod(CodeStringBuilder sb, MethodInfo methodInfo)
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

            var route = methodInfo.GetCustomAttribute<RouteAttribute>()!;
            sb.AppendLine($"var route = $\"{FixRouteTemplate(route.Template)}\";");


            if (methodInfo.GetCustomAttribute<HttpPostAttribute>() is not null)
            {
                var arg1 = methodInfo.GetParameters()[0].Name;
                if (methodInfo.ReturnType.IsGenericType)
                {
                    var returnType = ToGenericTypeString(methodInfo.ReturnType.GenericTypeArguments[0]);
                    sb.AppendLine($"return PostJsonAsync<{returnType}>(route, new{{{arg1}}}, cancellationToken);");
                }
                else
                {
                    sb.AppendLine($"return PostJsonAsync(route, new{{{arg1}}}, cancellationToken);");
                }
            }
            else if (methodInfo.GetCustomAttribute<HttpPutAttribute>() is not null)
            {
                var arg1 = methodInfo.GetParameters().Last(x => x.ParameterType != typeof(DateTime)).Name;

                if (methodInfo.ReturnType.IsGenericType)
                {
                    var returnType = ToGenericTypeString(methodInfo.ReturnType.GenericTypeArguments[0]);
                    sb.AppendLine($"return PutAsync<{returnType}>(route, new{{{arg1}}}, cancellationToken);");
                }
                else
                {
                    sb.AppendLine($"return PutAsync(route, new{{{arg1}}}, cancellationToken);");
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
        }

        private static string FixRouteTemplate(string s)
        {
            return ParameterRegex.Replace(s, match => $"{{{match.Groups["name"].Value}}}");
        }

        private static readonly Regex ParameterRegex = new(@"{(?<name>\w+)\s*:\s*(?<type>\w+)}", RegexOptions.Compiled);


        public static string ToGenericTypeString(Type t)
        {
            if (!t.IsGenericType)
                return t.Name;
            string genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ToGenericTypeString).ToArray());
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
            public TemplateServiceFactory(RelativitySettings relativitySettings, IFlurlClient flurlClient)
            {
                RelativitySettings = relativitySettings;
                FlurlClient = flurlClient;

                var types = typeof(TemplateServiceFactory).Assembly.GetTypes()
                    .Where(x => !x.IsAbstract && x.IsAssignableTo(typeof(ManagerBase)));


                Managers = types.Select(t =>
                    (ManagerBase)
                    Activator.CreateInstance(t, new object?[] { RelativitySettings, FlurlClient }, null)).ToList();
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
                           $"Could not create a proxy of type {typeof(T).Name} with a TemplateServiceFactory");
            }
        }
    }

    public abstract class ManagerBase : IDisposable
    {
        protected ManagerBase(RelativitySettings relativitySettings, IFlurlClient flurlClient)
        {
            RelativitySettings = relativitySettings;
            FlurlClient = flurlClient;
        }

        public RelativitySettings RelativitySettings { get; }

        public IFlurlClient FlurlClient { get; }


        public abstract string RoutePrefix { get; }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        public Task PutAsync(string routeSuffix, object data, CancellationToken cancellationToken)
        {
            var completeRoute = CreateCompleteRoute(RoutePrefix, routeSuffix);

            return FlurlClient.SetupRelativityRequest(RelativitySettings, completeRoute)
                    .PutJsonAsync(data, cancellationToken)
                ;
        }

        public Task<T> PutAsync<T>(string routeSuffix, object data, CancellationToken cancellationToken)
        {
            var completeRoute = CreateCompleteRoute(RoutePrefix, routeSuffix);

            return FlurlClient.SetupRelativityRequest(RelativitySettings, completeRoute)
                    .PutJsonAsync(data, cancellationToken)
                    .ReceiveJson<T>()
                ;
        }

        public Task DeleteAsync(string routeSuffix, CancellationToken cancellationToken)
        {
            var completeRoute = CreateCompleteRoute(RoutePrefix, routeSuffix);

            return FlurlClient.SetupRelativityRequest(RelativitySettings, completeRoute)
                .DeleteAsync(cancellationToken);
        }

        public Task<T> GetJsonAsync<T>(string routeSuffix, CancellationToken cancellationToken)
        {
            var completeRoute = CreateCompleteRoute(RoutePrefix, routeSuffix);

            return FlurlClient.SetupRelativityRequest(RelativitySettings, completeRoute)
                .GetJsonAsync<T>(cancellationToken);
        }


        public Task<T> PostJsonAsync<T>(string routeSuffix, object thing, CancellationToken cancellation)
        {
            var completeRoute = CreateCompleteRoute(RoutePrefix, routeSuffix);

            return FlurlClient.SetupRelativityRequest(RelativitySettings,
                    completeRoute)
                .PostJsonAsync(thing, cancellation).ReceiveJson<T>();
        }

        public Task PostJsonAsync(string routeSuffix, object thing, CancellationToken cancellation)
        {
            var completeRoute = CreateCompleteRoute(RoutePrefix, routeSuffix);

            return FlurlClient.SetupRelativityRequest(RelativitySettings,
                    completeRoute)
                .PostJsonAsync(thing, cancellation);
        }


        private string[] CreateCompleteRoute(string managerPrefix, string suffix)
        {
            var routeSuffix = SplitRoute(suffix);

            if (routeSuffix.Any() && routeSuffix.First() == "~")
            {
                return Prefixes.Concat(routeSuffix[1..]).ToArray();
            }

            return Prefixes.Concat(SplitRoute(managerPrefix)).Concat(routeSuffix).ToArray();
        }

        private IEnumerable<string> Prefixes
        {
            get
            {
                yield return "relativity-environment";
                yield return "v" + RelativitySettings.APIVersionNumber;
            }
        }

        private static string[] SplitRoute(string s)
        {
            return s.Split("/");
        }
    }
}