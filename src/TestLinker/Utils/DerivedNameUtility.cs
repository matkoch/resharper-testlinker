// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.Managed;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace TestLinker.Utils
{
    public static class DerivedNameUtility
    {
        public static string GetDerivedNamespace (ITypeElement sourceType, ITypeElement templateLinkedType)
        {
            var sourceDefaultNamespace = GetDefaultNamespace(sourceType.GetSingleOrDefaultSourceFile().GetProject().NotNull());
            var linkedDefaultNamespace = GetDefaultNamespace(templateLinkedType.GetSingleOrDefaultSourceFile().GetProject().NotNull());
            var sourceNamespaceTail = sourceType.GetContainingNamespace().QualifiedName.TrimFromStart(sourceDefaultNamespace);

            return linkedDefaultNamespace + sourceNamespaceTail;
        }

        private static string GetDefaultNamespace (IProject sourceProject)
        {
            var projectBuildSettings = (IManagedProjectBuildSettings) sourceProject.ProjectProperties.BuildSettings;
            return projectBuildSettings?.DefaultNamespace ?? string.Empty;
        }

        public static string GetDerivedName (string sourceName, string templateSourceName, string templateLinkedName)
        {
            return templateSourceName.Contains(templateLinkedName)
                ? sourceName.Replace(templateSourceName.Replace(templateLinkedName, string.Empty), string.Empty)
                : templateLinkedName.Replace(templateSourceName, sourceName);
        }

        public static bool IsDerivedName (string baseName, string derivedName)
        {
            return derivedName.Contains(baseName);
        }

        public static bool IsDerivedNameAny (string name1, string name2)
        {
            if (name1.Length == name2.Length)
                return name1 == name2;

            return name1.Length > name2.Length
                ? IsDerivedName(name2, name1)
                : IsDerivedName(name1, name2);
        }
    }
}
